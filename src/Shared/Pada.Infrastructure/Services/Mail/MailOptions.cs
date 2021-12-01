﻿namespace Pada.Infrastructure.Services.Mail
{
    public class MailOptions
    {
        public string From { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
        public bool Enable { get; set; }
    }
}