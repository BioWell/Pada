using System;

namespace Pada.Infrastructure.Exceptions
{
    public class CustomException : Exception
    {
        public string Code { get; set; }
        
        public string Datas { get; set; }
        private bool IsSuccess { get; }

        public string AppMessage { get; set; }

        protected CustomException(string message)
        {
            IsSuccess = false;
            AppMessage = message;
        }
    }
}