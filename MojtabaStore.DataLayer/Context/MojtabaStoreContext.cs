using Microsoft.EntityFrameworkCore;
using MojtabaStore.DataLayer.Entities.Course;
using MojtabaStore.DataLayer.Entities.Permissions;
using MojtabaStore.DataLayer.Entities.User;
using MojtabaStore.DataLayer.Entities.Wallet;

namespace MojtabaStore.DataLayer.Context
{
    public class MojtabaStoreContext: DbContext
    {
        public MojtabaStoreContext(DbContextOptions<MojtabaStoreContext> options): base(options)
        {

        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        public DbSet<WalletType> WalletTypes { get; set; }
        public DbSet<Wallet> Wallets { get; set; }

        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

        public DbSet<CourseGroup> CourseGroups { get; set; }
        public DbSet<CourseLevel> CourseLevels { get; set; }
        public DbSet<CourseStatus> CourseStatuses { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseEpisode> CourseEpisodes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasQueryFilter(c => !c.IsDelete);
            modelBuilder.Entity<Role>().HasQueryFilter(c => !c.IsDelete);
            modelBuilder.Entity<CourseGroup>().HasQueryFilter(c => !c.IsDelete);
            modelBuilder.Entity<Course>().HasQueryFilter(c => !c.IsDelete);
            base.OnModelCreating(modelBuilder);
        }


    }
}
