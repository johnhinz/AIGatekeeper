using System;
using System.Collections.Generic;
using System.Text;

namespace AIGuard.Repository
{
    public class Notification
    {
        public Guid Id { get; set; }
        public string Method { get; set; }
        public DateTime When { get; set; }
        public bool Success { get; set; }
    }
}
