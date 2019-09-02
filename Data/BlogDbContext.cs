using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace restful_blog.Data
{
    public class BlogDbContext : DbContext
    {
        public BlogDbContext (DbContextOptions<BlogDbContext> options) : base(options)
        {
        }

        public DbSet<Blog> BlogModel { get; set; }

        /*
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
        {
            ChangeTracker.Entries().Where(E => E.State == EntityState.Added).ToList().ForEach(E =>
            {
                E.Property("CreatedAt").CurrentValue = DateTime.Now;
                E.Property("UpdatedAt").CurrentValue = null;
            });

            ChangeTracker.Entries().Where(E => E.State == EntityState.Modified).ToList().ForEach(E =>
            {
                E.Property("CreatedAt").CurrentValue = E.Property("CreatedAt").OriginalValue;
                E.Property("UpdatedAt").CurrentValue = DateTime.Now;
            });
            
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
        */
    }
}
