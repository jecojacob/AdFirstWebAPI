using AdviceFirstApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace AdviceFirstApi.Persistance
{
    public class AdviceFirstDbContext :DbContext
    {
        public AdviceFirstDbContext(DbContextOptions<AdviceFirstDbContext> options ) : base(options)
        {
            
        }

        public DbSet<AfUser> AfUser { get; set; }
    }
}