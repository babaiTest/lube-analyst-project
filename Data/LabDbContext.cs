using Microsoft.EntityFrameworkCore;

namespace LubeAnalyst.Data
{
    public class LabDbContext : DbContext
    {
        public LabDbContext(DbContextOptions<LabDbContext> options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<TestRequest> TestRequests { get; set; }
        public DbSet<LabReport> LabReports { get; set; }
        //public DbSet<TestType> TestTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Customers
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.CustomerID);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.CompanyName).HasMaxLength(200);
            });

            // TestRequests
            modelBuilder.Entity<TestRequest>(entity =>
            {
                entity.HasKey(e => e.TestRequestID);
                entity.Property(e => e.TestType).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Status).HasMaxLength(50).HasDefaultValue("Pending");

                entity.HasOne(e => e.Customer)
                      .WithMany(c => c.TestRequests)
                      .HasForeignKey(e => e.CustomerID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // LabReports
            modelBuilder.Entity<LabReport>(entity =>
            {
                entity.HasKey(e => e.LabReportID);
                entity.Property(e => e.ReportFilePath).HasMaxLength(500);

                entity.HasOne(e => e.TestRequest)
                      .WithMany(t => t.LabReports)
                      .HasForeignKey(e => e.TestRequestID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // TestTypes
            //modelBuilder.Entity<TestType>(entity =>
            //{
            //    entity.HasKey(e => e.TestTypeID);
            //    entity.Property(e => e.TestTypeName).IsRequired().HasMaxLength(100);
            //});
        }
    }
}
