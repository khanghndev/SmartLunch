using System;
using System.Security.Cryptography;
using System.Text;

var objectName = "users/2/dish-image/image/2026/04/c597d705df8945d0bd8e886cb1ed25cd.png";
var normalized = objectName.Trim();
var hash = SHA256.HashData(Encoding.UTF8.GetBytes(normalized));
var hex = Convert.ToHexString(hash).ToLowerInvariant();
var fileId = $"f_{hex[..34]}";
Console.WriteLine($"ObjectName: {objectName}");
Console.WriteLine($"Calculated FileId: {fileId}");
