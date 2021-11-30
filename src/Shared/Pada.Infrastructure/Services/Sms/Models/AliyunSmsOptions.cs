namespace Pada.Infrastructure.Services.Sms.Models
{
    public class AliyunSmsOptions
    {
        public string RegionId { get; set; }
        public string AccessKeyId { get; set; }
        public string AccessKeySecret { get; set; }
        public bool IsTest { get; set; }
        public string TemplateCode { get; set; }
        public string SignName { get; set; }
    }
}