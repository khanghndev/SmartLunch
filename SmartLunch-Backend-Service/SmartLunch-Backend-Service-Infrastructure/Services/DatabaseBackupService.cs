using System.Diagnostics;
using System.Globalization;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.Helpers.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace SmartLunch.Backend.Service.Infrastructure.Services;

public class DatabaseBackupService : IDatabaseBackupService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<DatabaseBackupService> _logger;
    private readonly ISystemBackupRepository _systemBackupRepository;
    private readonly IStorageService _storage;
    private readonly HttpClient _httpClient;

    public DatabaseBackupService(
        IConfiguration configuration,
        ISystemBackupRepository systemBackupRepository,
        IStorageService storage,
        ILogger<DatabaseBackupService> logger)
    {
        _configuration = configuration;
        _systemBackupRepository = systemBackupRepository;
        _storage = storage;
        _logger = logger;
        _httpClient = new HttpClient();
    }

    public async Task<(string FilePath, long SizeBytes, DateTime CreatedAtUtc)> CreateBackupAsync(
        string backupSource = "Manual",
        CancellationToken cancellationToken = default)
    {
        var opts = ReadOptions();
        Directory.CreateDirectory(opts.OutputDirectory);

        var createdAt = VietnamTime.Now;
        var dumpExtension = string.IsNullOrWhiteSpace(opts.FileExtension) ? "sql" : opts.FileExtension.Trim().TrimStart('.');
        var storageExtension = string.IsNullOrWhiteSpace(opts.StorageFileExtension)
            ? dumpExtension
            : opts.StorageFileExtension.Trim().TrimStart('.');

        var dumpFileName = $"{opts.FilePrefix}-{createdAt:yyyyMMddHHmmss}.{dumpExtension}";
        var storageFileName = string.Equals(dumpExtension, storageExtension, StringComparison.OrdinalIgnoreCase)
            ? dumpFileName
            : $"{opts.FilePrefix}-{createdAt:yyyyMMddHHmmss}.{storageExtension}";
        var filePath = Path.Combine(opts.OutputDirectory, dumpFileName);

        var (host, port, database, user, password) = ParseConnectionString(opts.ConnectionString);

        var dumpArgs =
            $"--user={EscapeArg(user)} --password={EscapeArg(password)} " +
            $"--databases {EscapeArg(database)} --routines --events --triggers --single-transaction --quick --set-gtid-purged=OFF";

        string exe;
        string args;
        if (opts.UseDocker)
        {
            var container = string.IsNullOrWhiteSpace(opts.DockerContainerName) ? "smart-lunch-mysql" : opts.DockerContainerName.Trim();
            exe = ResolveToolPath(null, "docker.exe", "docker");
            args = $"exec {EscapeArg(container)} mysqldump {dumpArgs}";
        }
        else
        {
            var mysqldump = ResolveToolPath(opts.ToolsDirectory, "mysqldump.exe", "mysqldump");
            exe = mysqldump;
            args =
                $"--host={EscapeArg(host)} --port={port.ToString(CultureInfo.InvariantCulture)} " +
                $"{dumpArgs}";
        }

        _logger.LogInformation("Starting database backup to {FilePath}", filePath);

        await RunProcessCaptureStdoutToFileAsync(
            exePath: exe,
            args: args,
            outputFilePath: filePath,
            cancellationToken: cancellationToken);

        var size = new FileInfo(filePath).Length;
        _logger.LogInformation("Database backup completed. File={FilePath} SizeBytes={SizeBytes}", filePath, size);

        // Upload to Appwrite immediately (do not keep local backups).
        var bucketId = _configuration["Appwrite:BucketId"] ?? string.Empty;
        var objectName = $"backups/{createdAt:yyyy}/{createdAt:MM}/{storageFileName}";
        var contentType = storageExtension.Equals("sql", StringComparison.OrdinalIgnoreCase)
            ? "application/sql"
            : "application/octet-stream";

        await using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            await _storage.UploadObjectAsync(objectName, fs, contentType, cancellationToken);
        }

        try
        {
            await _systemBackupRepository.CreateAsync(new Domain.Entities.SystemBackup
            {
                FileName = storageFileName,
                StorageBucket = bucketId,
                StorageObjectName = objectName,
                SizeBytes = size,
                BackupSource = string.IsNullOrWhiteSpace(backupSource) ? "Manual" : backupSource,
                CreatedAtUtc = createdAt,
                IsDeleted = false
            });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to persist system backup metadata for {FilePath}", filePath);
        }

        // Delete local file after upload to storage
        try
        {
            File.Delete(filePath);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to delete local backup file {FilePath}", filePath);
        }

        await CleanupOldBackupsAsync(cancellationToken);

        return (filePath, size, createdAt);
    }

    public async Task<(string FilePath, DateTime RestoredAtUtc)> RestoreLatestAsync(CancellationToken cancellationToken = default)
    {
        var latest = await _systemBackupRepository.GetLatestAsync();
        if (latest == null)
            throw new InvalidOperationException("No backup file found to restore.");

        return await RestoreFromStorageAsync(latest, cancellationToken);
    }

    public async Task<(string FilePath, DateTime RestoredAtUtc)> RestoreAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var opts = ReadOptions();
        if (filePath.StartsWith("storage://", StringComparison.OrdinalIgnoreCase))
        {
            var objectName = filePath["storage://".Length..];
            var backup = new Domain.Entities.SystemBackup
            {
                FileName = Path.GetFileName(objectName),
                StorageBucket = _configuration["Appwrite:BucketId"] ?? string.Empty,
                StorageObjectName = objectName
            };
            return await RestoreFromStorageAsync(backup, cancellationToken);
        }

        if (!File.Exists(filePath))
            throw new FileNotFoundException("Backup file not found.", filePath);

        var (host, port, database, user, password) = ParseConnectionString(opts.ConnectionString);
        var clientArgs =
            $"--user={EscapeArg(user)} --password={EscapeArg(password)} {EscapeArg(database)}";

        string exe;
        string args;
        if (opts.UseDocker)
        {
            var container = string.IsNullOrWhiteSpace(opts.DockerContainerName) ? "smart-lunch-mysql" : opts.DockerContainerName.Trim();
            exe = ResolveToolPath(null, "docker.exe", "docker");
            args = $"exec -i {EscapeArg(container)} mysql {clientArgs}";
        }
        else
        {
            var mysql = ResolveToolPath(opts.ToolsDirectory, "mysql.exe", "mysql");
            exe = mysql;
            args =
                $"--host={EscapeArg(host)} --port={port.ToString(CultureInfo.InvariantCulture)} " +
                $"{clientArgs}";
        }

        _logger.LogWarning("Starting database restore from {FilePath}", filePath);

        await RunProcessPipeFileToStdinAsync(
            exePath: exe,
            args: args,
            inputFilePath: filePath,
            cancellationToken: cancellationToken);

        var restoredAt = VietnamTime.Now;
        _logger.LogWarning("Database restore completed. File={FilePath}", filePath);

        try
        {
            // Mark restored time if we have a record
            // (best-effort; do not fail restore on metadata update)
            var latest = await _systemBackupRepository.GetLatestAsync();
            if (latest != null && File.Exists(filePath) && string.Equals(latest.FileName, Path.GetFileName(filePath), StringComparison.OrdinalIgnoreCase))
            {
                latest.RestoredAtUtc = restoredAt;
                await _systemBackupRepository.UpdateAsync(latest);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to update restore metadata for {FilePath}", filePath);
        }

        return (filePath, restoredAt);
    }

    public Task<string?> GetLatestBackupFileAsync(CancellationToken cancellationToken = default)
    {
        // Local backups are not retained. Kept for backward compatibility.
        return Task.FromResult<string?>(null);
    }

    public Task<int> CleanupOldBackupsAsync(CancellationToken cancellationToken = default)
    {
        var opts = ReadOptions();
        if (!Directory.Exists(opts.OutputDirectory))
            return Task.FromResult(0);

        var extension = string.IsNullOrWhiteSpace(opts.FileExtension) ? "sql" : opts.FileExtension.Trim().TrimStart('.');
        var cutoff = VietnamTime.Now.AddDays(-Math.Max(1, opts.RetentionDays));
        var deleted = 0;

        foreach (var file in Directory.EnumerateFiles(opts.OutputDirectory, $"{opts.FilePrefix}-*.{extension}", SearchOption.TopDirectoryOnly))
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                var fi = new FileInfo(file);
                if (fi.CreationTimeUtc < cutoff)
                {
                    fi.Delete();
                    deleted++;

                    try
                    {
                        // Best-effort: mark as deleted in DB metadata (if exists).
                        // We don't have a direct lookup by path in repo, so we skip that for now.
                    }
                    catch
                    {
                        // ignore
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to delete old backup file {File}", file);
            }
        }

        if (deleted > 0)
            _logger.LogInformation("Deleted {Count} old backup files", deleted);

        return Task.FromResult(deleted);
    }

    private async Task<(string FilePath, DateTime RestoredAtUtc)> RestoreFromStorageAsync(
        Domain.Entities.SystemBackup backup,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(backup.StorageObjectName))
            throw new InvalidOperationException("Backup storage object name is missing.");

        // Download to temp, restore, then cleanup temp file
        var signed = await _storage.CreateSignedUrlAsync(
            backup.StorageObjectName,
            HttpMethod.Get,
            contentType: null,
            expiresIn: TimeSpan.FromMinutes(15));

        var tempPath = Path.Combine(Path.GetTempPath(), backup.FileName);
        _logger.LogWarning("Downloading backup from storage to temp {TempPath}", tempPath);

        using (var res = await _httpClient.GetAsync(signed.Url, cancellationToken))
        {
            res.EnsureSuccessStatusCode();
            await using var fs = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None);
            await res.Content.CopyToAsync(fs, cancellationToken);
        }

        try
        {
            return await RestoreAsync(tempPath, cancellationToken);
        }
        finally
        {
            try { File.Delete(tempPath); } catch { /* ignore */ }
        }
    }

    private DatabaseBackupOptions ReadOptions()
    {
        var section = _configuration.GetSection("DatabaseBackup");
        var opts = new DatabaseBackupOptions();
        section.Bind(opts);

        if (string.IsNullOrWhiteSpace(opts.ConnectionString))
        {
            // fallback to main DB connection string
            opts.ConnectionString = _configuration.GetConnectionString("SmartLunchDatabase") ?? string.Empty;
        }

        if (string.IsNullOrWhiteSpace(opts.ConnectionString))
            throw new InvalidOperationException("DatabaseBackup.ConnectionString (or ConnectionStrings:SmartLunchDatabase) is not configured.");

        if (string.IsNullOrWhiteSpace(opts.OutputDirectory))
            opts.OutputDirectory = Path.Combine(AppContext.BaseDirectory, "Backups");

        if (string.IsNullOrWhiteSpace(opts.FilePrefix))
            opts.FilePrefix = "smartlunch";

        return opts;
    }

    private static (string Host, int Port, string Database, string User, string Password) ParseConnectionString(string cs)
    {
        // Expected: Server=localhost;Port=3307;Database=SmartLunch;User=root;Password=...
        var dict = cs.Split(';', StringSplitOptions.RemoveEmptyEntries)
            .Select(part => part.Split('=', 2))
            .Where(parts => parts.Length == 2)
            .ToDictionary(parts => parts[0].Trim(), parts => parts[1].Trim(), StringComparer.OrdinalIgnoreCase);

        var host = dict.TryGetValue("Server", out var v1) ? v1 :
            dict.TryGetValue("Host", out var v2) ? v2 : "localhost";
        var port = dict.TryGetValue("Port", out var p) && int.TryParse(p, out var pi) ? pi : 3306;
        var database = dict.TryGetValue("Database", out var db) ? db :
            dict.TryGetValue("Initial Catalog", out var db2) ? db2 : throw new InvalidOperationException("Database not found in connection string.");
        var user = dict.TryGetValue("User", out var u) ? u :
            dict.TryGetValue("Uid", out var u2) ? u2 :
            dict.TryGetValue("User Id", out var u3) ? u3 : "root";
        var password = dict.TryGetValue("Password", out var pw) ? pw :
            dict.TryGetValue("Pwd", out var pw2) ? pw2 : string.Empty;

        return (host, port, database, user, password);
    }

    private static string ResolveToolPath(string? toolsDirectory, string windowsExeName, string unixName)
    {
        if (!string.IsNullOrWhiteSpace(toolsDirectory))
        {
            var candidate = Path.Combine(toolsDirectory, windowsExeName);
            if (File.Exists(candidate))
                return candidate;
        }

        // rely on PATH
        return OperatingSystem.IsWindows() ? windowsExeName : unixName;
    }

    private static string EscapeArg(string value)
    {
        // Keep it simple: if value has spaces, wrap in quotes.
        if (value.Contains(' ') || value.Contains('"'))
            return "\"" + value.Replace("\"", "\\\"") + "\"";
        return value;
    }

    private static async Task RunProcessCaptureStdoutToFileAsync(
        string exePath,
        string args,
        string outputFilePath,
        CancellationToken cancellationToken)
    {
        var psi = new ProcessStartInfo
        {
            FileName = exePath,
            Arguments = args,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(psi) ?? throw new InvalidOperationException($"Failed to start process: {exePath}");

        await using var fs = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.None);
        var copyTask = process.StandardOutput.BaseStream.CopyToAsync(fs, cancellationToken);

        var stderrTask = process.StandardError.ReadToEndAsync(cancellationToken);

        await Task.WhenAll(copyTask, stderrTask);

        await process.WaitForExitAsync(cancellationToken);
        if (process.ExitCode != 0)
        {
            var stderr = await stderrTask;
            throw new InvalidOperationException($"Backup process failed (exit {process.ExitCode}): {stderr}");
        }
    }

    private static async Task RunProcessPipeFileToStdinAsync(
        string exePath,
        string args,
        string inputFilePath,
        CancellationToken cancellationToken)
    {
        var psi = new ProcessStartInfo
        {
            FileName = exePath,
            Arguments = args,
            RedirectStandardInput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(psi) ?? throw new InvalidOperationException($"Failed to start process: {exePath}");

        await using (var fs = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            await fs.CopyToAsync(process.StandardInput.BaseStream, cancellationToken);
        }

        process.StandardInput.Close();

        var stderr = await process.StandardError.ReadToEndAsync(cancellationToken);
        await process.WaitForExitAsync(cancellationToken);

        if (process.ExitCode != 0)
            throw new InvalidOperationException($"Restore process failed (exit {process.ExitCode}): {stderr}");
    }

    private class DatabaseBackupOptions
    {
        public bool Enabled { get; set; } = false;
        public string ConnectionString { get; set; } = string.Empty;
        public string OutputDirectory { get; set; } = string.Empty;
        public string FilePrefix { get; set; } = "smartlunch";
        public string FileExtension { get; set; } = "sql";
        /// <summary>Đuôi file khi upload Appwrite. Để trống thì dùng <see cref="FileExtension"/> (mặc định sql).</summary>
        public string StorageFileExtension { get; set; } = "";
        public int RetentionDays { get; set; } = 7;
        public string? ToolsDirectory { get; set; }
        public bool UseDocker { get; set; } = false;
        public string? DockerContainerName { get; set; }
    }
}

