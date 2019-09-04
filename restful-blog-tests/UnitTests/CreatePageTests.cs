using System;
using System.Linq;
using restful_blog.Data;
using restful_blog.Pages.Blog;
using restful_blog_tests.Utilities;
using Xunit;

namespace restful_blog_tests
{
    public class CreatePageTests
    {
        [Fact]
        public async void CanCreatePost()
        {
            await WithTestDatabase.Run(async (BlogDbContext context) =>
            {
                var pageModel = new CreateModel(context);

                pageModel.BlogPost = DbUtil.GetTestPost();
                await pageModel.OnPostAsync();

                Assert.Equal(1, context.Blog.Count());
            });
        }
    }
}
