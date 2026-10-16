using Microsoft.EntityFrameworkCore;
using RaceDay.Models;
namespace RaceDay.Data
{
    public class RaceDayContext :DbContext
    {
        public RaceDayContext(DbContextOptions<RaceDayContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Profile> Profiles => Set<Profile>();
        public DbSet<Event> Events => Set<Event>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Enrolment> Enrolments => Set<Enrolment>();
        public DbSet<Result> Results => Set<Result>();
        public DbSet<EventBanner> EventBanners => Set<EventBanner>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<EventBanner>().HasKey(b => b.BannerID);
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.UserID);
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.FirstName).HasColumnType("varchar(50)").IsRequired();
                entity.Property(u => u.LastName).HasColumnType("varchar(50)").IsRequired();
                entity.Property(u => u.Email).HasColumnType("varchar(100)").IsRequired(false);
                entity.Property(u => u.PasswordHash).HasColumnType("varchar(255)").IsRequired();
                entity.Property(u => u.Role).HasColumnType("varchar(20)").IsRequired();
                entity.Property(u => u.Phone).HasColumnType("varchar(20)").IsRequired(false);
                entity.ToTable(t => t.HasCheckConstraint("CK_Uer_Role", "[Role] IN ('Organiser', 'Participant')"));
            });

            modelBuilder.Entity<Profile>(entity =>
            {
                entity.HasKey(p => p.ProfileID);
                entity.HasIndex(p => p.UserID).IsUnique();
                entity.Property(p => p.Address).HasMaxLength(255);
                entity.Property(p => p.ProfilePictureURL).HasMaxLength(500);
                entity.HasOne(P => P.User).WithOne(u => u.Profile).HasForeignKey<Profile>(p => p.UserID);
            }
            );

            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.EventID);
                entity.Property(e => e.Name).HasMaxLength(150).IsRequired();
                entity.Property(e => e.EventDate).HasColumnType("date").IsRequired();
                entity.Property(e => e.Location).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Distance).HasPrecision(6, 2);
                entity.Property(e => e.EventType).HasMaxLength(20).IsRequired();
                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("CK_Event_Type", "[EventType] IN('Run', 'Walk', 'Cycle')");
                    t.HasCheckConstraint("CK_Event_Distance", "[Distance] >0");
                });
                entity.HasOne(e => e.Organiser).WithMany(u => u.OrganisedEvents).HasForeignKey(e => e.OrganiserID).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.CategoryID);
                entity.Property(c => c.CategoryName).HasMaxLength(100).IsRequired();
                entity.Property(c => c.Description).HasMaxLength(255);
                entity.HasOne(c => c.Event).WithMany(e => e.Categories).HasForeignKey(c => c.EventID).OnDelete(DeleteBehavior.Cascade);

            });

            modelBuilder.Entity<Enrolment>(entity =>
            {
                entity.HasKey(e => e.EnrolmentID);
                entity.Property(e => e.EnrolmentDate).HasColumnType("date").HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.Status).HasMaxLength(20).HasDefaultValue("Confirmed").IsRequired();
                entity.ToTable(t => t.HasCheckConstraint("CK_Enrolment_Status", "[Status] IN ('Pending', 'Confirmed', 'Cancelled')"));
                entity.HasIndex(e => new
                {
                    e.ParticipantID,
                    e.EventID
                }).IsUnique();



                entity.HasOne(e => e.Participant).WithMany(u => u.Enrolments).HasForeignKey(e => e.ParticipantID).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Event).WithMany(e => e.Enrolments).HasForeignKey(e => e.EventID).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Category).WithMany(e => e.Enrolments).HasForeignKey(e => e.CategoryID).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Result>(entity =>
            {
                entity.HasKey(r => r.ResultID);
                entity.HasIndex(r => r.EnrolmentID).IsUnique();
                entity.Property(r => r.FinishTime).HasColumnType("time").IsRequired();
                entity.Property(r => r.FinishPosition).IsRequired();
                entity.Property(r => r.ResultDate).HasColumnType("date").HasDefaultValueSql("GETDATE()");
                entity.ToTable(t => t.HasCheckConstraint("CK_Result_Position", "[FinishPosition] >0"));
                entity.HasOne(r => r.Enrolment).WithOne(e => e.Result).HasForeignKey<Result>(r => r.EnrolmentID);
            });

            modelBuilder.Entity<EventBanner>(entity =>
            {
                entity.HasKey(b => b.BannerID);
                entity.HasIndex(b => b.EventID).IsUnique();
                entity.Property(b => b.ImageURL).HasMaxLength(500).IsRequired();
                entity.Property(b => b.UploadedDate).HasColumnType("date").HasDefaultValueSql("GETDATE()");
                entity.HasOne(b => b.Event).WithMany().HasForeignKey(b => b.EventID).OnDelete(DeleteBehavior.Cascade);

            });
           
            
        }
    }
}
