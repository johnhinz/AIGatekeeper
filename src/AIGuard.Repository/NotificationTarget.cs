using System;
using System.Collections.Generic;
using System.Text;

namespace AIGuard.Repository
{
    public class NotificationTarget
    {
        public Guid Id { get; set; }
        public NotificationEnum NotificationType { get; set; }
        public string Endpoint { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
