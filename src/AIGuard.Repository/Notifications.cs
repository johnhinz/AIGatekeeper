using System;
using System.Collections.Generic;
using System.Text;

namespace AIGuard.Repository
{
    public class Notification
    {
        public Guid Id { get; set; }
        public NotificationEnum Method { get; set; }
        public NotificationTarget Target { get; set; }
        public DateTime When { get; set; }
        public bool Success { get; set; }
    }
}
