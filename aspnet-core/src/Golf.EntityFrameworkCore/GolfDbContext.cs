using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System;

using Golf.Domain.SocialNetwork;
using Golf.Domain.Bookings;
using Golf.Domain.Base;
using Golf.Domain.Post;
using Golf.Domain.Notifications;
using Golf.Domain.Courses;
using Golf.Domain.Resources;
using Golf.Domain.Scorecard;
using Golf.Domain.GolferData;
using Golf.Domain.Messages;
using Golf.Domain.Events;
using Golf.Domain.Tournaments;
using Golf.Domain.Shared.System;
using Golf.Domain.Report;
using System.Threading.Tasks;
using System.Threading;

namespace Golf.EntityFrameworkCore
{
    public class GolfDbContext : IdentityDbContext<Golfer, IdentityRole<Guid>, Guid>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DbSet<Scorecard> Scorecards { get; set; }
        public DbSet<SystemSetting> SystemSettings { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<PostVote> PostVotes { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Relationship> Relationships { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseReview> CourseReviews { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<CourseExtension> CourseExtensions { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Message> Messages  { get; set; }
        public DbSet<MemberShip> MemberShips { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Tournament> Tournaments { get; set; }
        public DbSet<TournamentMember> TournamentMembers { get; set; }
        public DbSet<ScorecardVote> ScorecardVotes { get; set; }
        public DbSet<Report> Reports { get; set; }
       

        public GolfDbContext(DbContextOptions<GolfDbContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            this._httpContextAccessor = httpContextAccessor;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ConfigureGolf();
            var golfSeedData = new GolfSeedData(modelBuilder);
            golfSeedData.SeedDefaultData();
            
        }

        public override int SaveChanges()
        {
            Console.WriteLine(ChangeTracker.Entries());
            var now = DateTime.Now;
            var currentGolfer = (Golfer)_httpContextAccessor.HttpContext.Items["Golfer"];
            Guid? currentGolferId = currentGolfer != null ? (Guid?)currentGolfer.Id : null;
            foreach (var changedEntity in ChangeTracker.Entries())
            {
                if (changedEntity.Entity is IEntityBase entity)
                {
                    switch (changedEntity.State)
                    {
                        case EntityState.Added:
                            entity.CreatedDate = now;
                            entity.ModifiedDate = now;
                            entity.CreatedBy = currentGolferId;
                            entity.ModifiedBy = currentGolferId;
                            break;
                        case EntityState.Modified:
                            Entry(entity).Property(x => x.CreatedBy).IsModified = false;
                            Entry(entity).Property(x => x.CreatedDate).IsModified = false;
                            entity.ModifiedDate = now;
                            entity.ModifiedBy = currentGolferId;
                            break;
                    }
                }
                if (changedEntity.Entity is ISafeEntityBase safeEntity)
                {
                    switch (changedEntity.State)
                    {
                        case EntityState.Added:
                            safeEntity.CreatedDate = now;
                            safeEntity.ModifiedDate = now;
                            safeEntity.CreatedBy = currentGolferId;
                            safeEntity.ModifiedBy = currentGolferId;
                            break;
                        case EntityState.Modified:
                            Entry(safeEntity).Property(x => x.CreatedBy).IsModified = false;
                            Entry(safeEntity).Property(x => x.CreatedDate).IsModified = false;
                            safeEntity.ModifiedDate = now;
                            safeEntity.ModifiedBy = currentGolferId;
                            break;
                        case EntityState.Deleted:
                            Entry(safeEntity).Property(x => x.CreatedBy).IsModified = false;
                            Entry(safeEntity).Property(x => x.CreatedDate).IsModified = false;
                            safeEntity.DeleteBy = currentGolferId;
                            safeEntity.DeleteDate = now;
                            break;

                    }
                }
                if (changedEntity.Entity is Photo photo)
                {
                    photo.CreatedBy = currentGolferId;
                    photo.CreatedDate = now;
                }
            }

            return base.SaveChanges();
        } 
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) //SaveChangesAsync()
        {
            Console.WriteLine(ChangeTracker.Entries());
            var now = DateTime.Now;
            var currentGolfer = (Golfer)_httpContextAccessor.HttpContext.Items["Golfer"];
            Guid? currentGolferId = currentGolfer != null ? (Guid?)currentGolfer.Id : null;
            foreach (var changedEntity in ChangeTracker.Entries())
            {
                if (changedEntity.Entity is IEntityBase entity)
                {
                    switch (changedEntity.State)
                    {
                        case EntityState.Added:
                            entity.CreatedDate = now;
                            entity.ModifiedDate = now;
                            entity.CreatedBy = currentGolferId;
                            entity.ModifiedBy = currentGolferId;
                            break;
                        case EntityState.Modified:
                            Entry(entity).Property(x => x.CreatedBy).IsModified = false;
                            Entry(entity).Property(x => x.CreatedDate).IsModified = false;
                            entity.ModifiedDate = now;
                            entity.ModifiedBy = currentGolferId;
                            break;
                    }
                }
                if (changedEntity.Entity is ISafeEntityBase safeEntity)
                {
                    switch (changedEntity.State)
                    {
                        case EntityState.Added:
                            safeEntity.CreatedDate = now;
                            safeEntity.ModifiedDate = now;
                            safeEntity.CreatedBy = currentGolferId;
                            safeEntity.ModifiedBy = currentGolferId;
                            break;
                        case EntityState.Modified:
                            Entry(safeEntity).Property(x => x.CreatedBy).IsModified = false;
                            Entry(safeEntity).Property(x => x.CreatedDate).IsModified = false;
                            safeEntity.ModifiedDate = now;
                            safeEntity.ModifiedBy = currentGolferId;
                            break;
                        case EntityState.Deleted:
                            Entry(safeEntity).Property(x => x.CreatedBy).IsModified = false;
                            Entry(safeEntity).Property(x => x.CreatedDate).IsModified = false;
                            safeEntity.DeleteBy = currentGolferId;
                            safeEntity.DeleteDate = now;
                            break;
                    }
                }
                if (changedEntity.Entity is Photo photo)
                {
                    photo.CreatedBy = currentGolferId;
                    photo.CreatedDate = now;
                } 
            }

            return base.SaveChangesAsync();
        }
    }
}
