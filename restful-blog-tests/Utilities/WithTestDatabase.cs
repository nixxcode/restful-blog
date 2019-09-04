using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using restful_blog.Data;

namespace restful_blog_tests
{
    class WithTestDatabase
    {
        public static async Task Run(Func<BlogDbContext,Task> testFunc)
        {
            var options = new DbContextOptionsBuilder<BlogDbContext>()
                .UseInMemoryDatabase("IN_MEMORY_DATABASE")
                .Options;

            using (var context = new BlogDbContext(options))
            {
                try
                {
                    await context.Database.EnsureCreatedAsync();
                    PrepareTestDatabase(context);
                    await testFunc(context);
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    CleanupTestDatabase(context);
                }
            }
        }

        public static void PrepareTestDatabase(BlogDbContext context)
        {
        }

        public static void CleanupTestDatabase(BlogDbContext context)
        {
            if (context.Database.IsInMemory())
            {
                context.Database.EnsureDeleted();

            }
        }
    }

    
}
