using System;
using System.Collections.Generic;

namespace PT.Entitiy.ViewModel
{
    public class MailModel
    {
        public string To { get; set; }
        public List<String> ToList { get; set; } = new List<string>();
        public string Subject { get; set; }
        public string Message { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
    }
}
