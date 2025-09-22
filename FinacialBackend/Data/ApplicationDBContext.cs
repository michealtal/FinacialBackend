using Microsoft.EntityFrameworkCore;
using FinacialBackend.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
namespace FinacialBackend.Data
{
    public class ApplicationDBContext : IdentityDbContext<AppUser> // with Identity  (DbContext) without identity
    {
       public ApplicationDBContext(DbContextOptions<ApplicationDBContext> dbContextoptions) : base(dbContextoptions)
       {
          
        }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            List<IdentityRole> roles = new List<IdentityRole>
    {
        new IdentityRole
        {
            Id = "11111111-1111-1111-1111-111111111111", //  fixed GUID
            Name = "Admin",
            NormalizedName = "ADMIN",
            ConcurrencyStamp = "a1" //  add fixed stamp (required by IdentityRole)
        },
        new IdentityRole
        {
            Id = "22222222-2222-2222-2222-222222222222", //  fixed GUID
            Name = "User",
            NormalizedName = "USER",
            ConcurrencyStamp = "a2" //  add fixed stamp
        }
    };

            builder.Entity<IdentityRole>().HasData(roles);
        }

    }
}

