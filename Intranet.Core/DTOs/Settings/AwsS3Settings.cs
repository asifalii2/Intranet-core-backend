namespace Intranet.Core.DTOs.Settings;

public class AwsS3Settings
{
    public string AccessKeyId { get; set; } = string.Empty;
    public string SecretAccessKey { get; set; } = string.Empty;
    public string BucketName { get; set; } = string.Empty;
    public string Region { get; set; } = "us-east-1";
    public string? BaseUrl { get; set; }
    public string? FolderPrefix { get; set; }
}
