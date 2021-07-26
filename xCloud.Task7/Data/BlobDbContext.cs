using Microsoft.EntityFrameworkCore;
using xCloud.Task7.Models;

namespace xCloud.Task7.Data
{
    public class BlobDbContext : DbContext
    {
        public BlobDbContext(DbContextOptions<BlobDbContext> options) : base(options)
        {
            
        }
        
        public DbSet<ImageMetadataModel> Images { get; set; }
    }
}