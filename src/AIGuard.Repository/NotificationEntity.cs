using System;
using System.Collections.Generic;
using System.Text;

namespace AIGuard.Repository
{
    public class NotificationEntity
    {
        public Guid Id { get; set; }
        public NotificationEnum Method { get; set; }
        public NotificationTargetEntity Target { get; set; }
        public DateTime When { get; set; }
        public bool Success { get; set; }
    }
}
