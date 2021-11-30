using System;
using System.Text.RegularExpressions;

namespace Pada.Infrastructure.Utils
{
    public static class ValidatorHelper
    {
        private const string PatternEmail = @"\w[-\w.+]*@([A-Za-z0-9][-A-Za-z0-9]+\.)+[A-Za-z]{2,14}";
        private const string PatternId = @"^[0-9a-f]{8}(-[0-9a-f]{4}){3}-[0-9a-f]{12}$";
        private const string PatternPhone = @"^\d{11}$";
        
        public static bool ValidEmail(string input)
        {
            if (string.IsNullOrWhiteSpace(input.Trim()))
                return false;
            var regex = new Regex(PatternEmail);
            if (!regex.IsMatch(input.Trim()))
                return false;
            return true;
        } 
        
        public static bool ValidPhone(string input)
        {
            if (string.IsNullOrWhiteSpace(input.Trim()))
                return false;
            var regex = new Regex(PatternPhone);
            if (!regex.IsMatch(input.Trim()))
                return false;
            return true;
        } 
        
        public static bool ValidId(Guid input)
        {
            Match match = Regex.Match(input.ToString(), PatternId, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return true;
            }
            return false;
        } 
    }
}