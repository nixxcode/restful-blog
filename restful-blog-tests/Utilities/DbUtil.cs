using System;
using System.Collections.Generic;
using System.Text;
using restful_blog.Data;

namespace restful_blog_tests.Utilities
{
    class DbUtil
    {
        public static void InitializeDbForTests(BlogDbContext db)
        {
            db.Blog.AddRange(GetSeedingPosts());
            db.SaveChanges();
        }

        public static List<Blog> GetSeedingPosts()
        {
            return new List<Blog>()
            {
                new Blog()
                {
                    Title = "TEST TITLE: You're standing on my scarf.",
                    Content = "TEST MESSAGE: All hail the flying spaghetti monster!"
                },
                new Blog()
                {
                    Title = "TEST TITLE: Their eyes are everywhere",
                    Content = "TEST MESSAGE: I feel like they're watching my slick moves!"
                }

            };
        }

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
