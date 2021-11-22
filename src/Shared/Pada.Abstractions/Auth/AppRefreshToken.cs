using System;

namespace Pada.Abstractions.Auth
{
    public class AppRefreshToken
    {
        public string Token { get; set; }
        public string UserId { get; set; }
        public bool IsExpired => DateTime.Now >= ExpiryOn;
        public bool IsRevoked => RevokedOn != null;
        public bool IsActive => !IsRevoked && !IsExpired;
        public DateTime ExpiryOn { get; set; }
        public DateTime? RevokedOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedByIp { get; set; }
        public string RevokedByIp { get; set; }
    }
}