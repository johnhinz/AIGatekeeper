using System;
using System.Collections.Generic;

namespace AIGuard.Repository
{
    public class FileEntity
    {
        public Guid Id { get; set; }
        public bool Success { get; set; }
        public ICollection<ItemEntity> Items { get; set; }
        public ICollection<NotificationEntity> Notifications { get; set; }
    }
}
