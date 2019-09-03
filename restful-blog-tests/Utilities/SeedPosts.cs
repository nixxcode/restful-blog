using System;
using System.Collections.Generic;
using System.Text;
using restful_blog.Data;

namespace restful_blog_tests.Utilities
{
    class SeedPosts
    {
        public static Blog GetTestPost()
        {
            return new Blog
            {
                Content = "Test Content",
                Title = "Test Title",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
        }
    }
}
