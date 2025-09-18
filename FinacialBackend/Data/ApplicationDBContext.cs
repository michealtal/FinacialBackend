using Microsoft.EntityFrameworkCore;
using FinacialBackend.Model;
namespace FinacialBackend.Data
{
    public class ApplicationDBContext : DbContext
    {
       public ApplicationDBContext(DbContextOptions<ApplicationDBContext> dbContextoptions) : base(dbContextoptions)
       {
          
        }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Comment> Comments { get; set; }
    }
}
