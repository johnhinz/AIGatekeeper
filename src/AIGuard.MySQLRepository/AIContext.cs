using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AIGuard.MySQLRepository
{
    class AIContext : DbContext
    {
        public DbSet<Capture> Captures { get; set; }
        public DbSet<Detection> Detections { get; set; }
    }
}
