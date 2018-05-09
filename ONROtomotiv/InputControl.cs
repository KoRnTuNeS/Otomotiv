using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace ONROtomotiv
{
    public class InputControl
    {
        public bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool IsName(string input)
        {
            if (string.IsNullOrEmpty(input)) return false;
            if (Regex.IsMatch(input, @"^[A-Za-zığüşöçİĞÜŞÖÇ]+\s?\s?[A-Za-zığüşöçİĞÜŞÖÇ]+$"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool IsNumeric(string input)
        {
            if (string.IsNullOrEmpty(input)) return false;
            if (Regex.IsMatch(input, "^[0-9]*$"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool IsDecimal(string input)
        {
            if (string.IsNullOrEmpty(input)) return false;
            if (Regex.IsMatch(input, @"\d+\,?\d*"))
                return true;
            else
                return false;
        }
        public bool IsNullableNumeric(string input)
        {
            if (Regex.IsMatch(input, "^[0-9]*$"))
                return true;
            else
                return false;
        }
    }
}