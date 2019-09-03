using System;
using System.Linq;
using restful_blog.Data;
using restful_blog.Pages.Blog;
using restful_blog_tests.Utilities;
using Xunit;

namespace restful_blog_tests
{
    public class EditPageTests
    {
        [Fact]
        public async void CanEditPost()
        {
            await WithTestDatabase.Run(async (BlogDbContext context) =>
            {
                var pageModel = new EditModel(context);
                pageModel.BlogPost = SeedPosts.GetTestPost();
                await pageModel.OnPostAsync();

                Assert.Equal(1, context.Blog.Count());
            });
        }
    }
}
