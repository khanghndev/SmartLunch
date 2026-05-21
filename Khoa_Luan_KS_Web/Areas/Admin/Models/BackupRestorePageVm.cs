using Khoa_Luan_KS_Web.Services;

namespace Khoa_Luan_KS_Web.Areas.Admin.Models;

public class BackupRestorePageVm
{
    public GetSystemBackupsResponse Backups { get; set; } = new();
    public BackupScheduleDto Schedule { get; set; } = new();
    public int Page { get; set; } = 1;
    public DateTime? FilterFrom { get; set; }
    public DateTime? FilterTo { get; set; }
}
