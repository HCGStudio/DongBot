using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace HCGStudio.DongBot.App.Models
{
    public class ServiceRecord
    {
        public int ServiceRecordId { get; set; }
        public long GroupId { get; set; }

        [MaxLength(500)] public string ServiceName { get; set; }

        public bool IsEnabled { get; set; }
    }

    public class ApplicationContext : DbContext
    {
        public DbSet<ServiceRecord> ServiceRecords { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=ServiceRecords.db");
        }
    }
}