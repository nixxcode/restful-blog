using AngleSharp.Html.Dom;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RazorPagesProject.Tests.Helpers;
using restful_blog;
using restful_blog.Data;
using restful_blog.Pages.Blog;
using restful_blog_tests.IntegrationTests;
using restful_blog_tests.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Xunit;

namespace restful_blog_tests.IntegrationTests
{
    public class BlogIntegrationTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private HttpClient client;
        private CustomWebApplicationFactory<Startup> factory;

        public BlogIntegrationTests(CustomWebApplicationFactory<Startup> factory)
        {
            this.factory = factory;
            client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        /*  We call this whenever we need to instantiate a new web application instance.
         *  Typically used in tests that modify test data, to ensure the changes are isolated
         *  from other tests in the integration suite. */
        public HttpClient clientFromWebHostBuilder()
        {
            return factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var serviceProvider = services.BuildServiceProvider();

                    using (var scope = serviceProvider.CreateScope())
                    {
                        var scopedServices = scope.ServiceProvider;
                        var db = scopedServices
                            .GetRequiredService<BlogDbContext>();
                        var logger = scopedServices
                            .GetRequiredService<ILogger<BlogIntegrationTests>>();
                    }
                });
            })
            .CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        #region Base Page Tests

        [Theory]
        [InlineData("/")]
        [InlineData("/Blog")]
        [InlineData("/Blog/Create")]
        public async void Get_BasePages_ReturnsOK(string url)
        {
            var response = await client.GetAsync(url);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        #endregion



        #region Create Page Tests
        
        // Anti forgery cookie and validation token not present. Should fail no matter what we actually POST
        [Fact]
        public async void Post_CreatePage_InvalidRequest_ReturnsBadRequest()
        {
            var content = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                ["Title"] = "Some Title",
                ["Content"] = "Some Content" 
            });
            var response = await client.PostAsync("/blog/create", content);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async void Post_CreatePage_ValidBlogPost_ReturnsRedirectToBlogIndex()
        {
            /*  This test creates a new blog post in the database. To isolate the changes from the 
             *  remainder of the test suite, we need to use WebHostBuilder to create a new application instance. */
            var client = clientFromWebHostBuilder();

            /*  GET the "create" page first, just like a web browser would. 
                We need this for the validation token and anti-forgery cookie */
            var defaultPage = await client.GetAsync("/Blog/Create");
            var content = await HtmlHelpers.GetDocumentAsync(defaultPage);

            var response = await client.SendAsync(
                (IHtmlFormElement)content.QuerySelector("form"),
                (IHtmlInputElement)content.QuerySelector("input[type='submit']"),
                new Dictionary<string, string>
                {
                    ["BlogPost.Title"] = "Some Title",
                    ["BlogPost.Content"] = "Some Content"
                });

            Assert.Equal(HttpStatusCode.OK, defaultPage.StatusCode);
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Equal("/Blog", response.Headers.Location.OriginalString);
        }

        [Fact]
        public async void Post_CreatePage_InvalidBlogPost_ReturnsOK()
        {
            /*  GET the "create" page first, just like a web browser would. 
                We need this for the validation token and anti-forgery cookie */
            var defaultPage = await client.GetAsync("/blog/create");
            var content = await HtmlHelpers.GetDocumentAsync(defaultPage);

            var response = await client.SendAsync(
                (IHtmlFormElement)content.QuerySelector("form"),
                (IHtmlInputElement)content.QuerySelector("input[type='submit']"),
                new Dictionary<string, string>
                {
                    // We define only title, no content. This makes the post invalid
                    ["BlogPost.Title"] = "Some Title",
                    ["BlogPost.Content"] = ""
                });

            Assert.Equal(HttpStatusCode.OK, defaultPage.StatusCode);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        
        #endregion



        #region Index Page Tests

        [Fact]
        public async void Get_IndexPage_ReturnsPosts()
        {
            var response = await client.GetAsync("/blog");
            var content = await HtmlHelpers.GetDocumentAsync(response);
            var blogPostElements = content.QuerySelectorAll(".post");

            // Check the number of elements is equal to how many we seed the DB with
            Assert.Equal(2, blogPostElements.Length);
        }

        #endregion



        #region Details Page Tests

        [Fact]
        public async void Get_DetailsPage_ForExistingBlogPost_ReturnsOK()
        {
            var response = await client.GetAsync("/Blog/Details?id=1");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async void Get_DetailsPage_ForNonExistingBlogPost_ReturnsNotFound()
        {
            var response = await client.GetAsync("/blog/Details?id=22");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        #endregion



        #region Edit Page Tests

        [Fact]
        public async void Get_EditPage_ForExistingBlogPost_ReturnsOK()
        {
            var response = await client.GetAsync("/Blog/Edit?id=1");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async void Get_EditPage_ForNonExistingBlogPost_ReturnsNotFound()
        {
            var response = await client.GetAsync("/Blog/Edit?id=22");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async void Post_EditPage_ValidBlogPost_ReturnsRedirectToBlogIndex()
        {
            /*  This test edits an existing blog post in the database. To isolate the changes from the 
             *  remainder of the test suite, we need to use WebHostBuilder to create a new application instance. */
            var client = clientFromWebHostBuilder();

            /*  GET the "edit" page first, just like a web browser would. 
                We need this for the validation token and anti-forgery cookie */
            var defaultPage = await client.GetAsync("/Blog/Edit?id=1");
            var content = await HtmlHelpers.GetDocumentAsync(defaultPage);

            var response = await client.SendAsync(
                (IHtmlFormElement)content.QuerySelector("form"),
                (IHtmlInputElement)content.QuerySelector("input[type='submit']"),
                new Dictionary<string, string>
                {
                    ["BlogPost.Title"] = "Changed Title",
                    ["BlogPost.Content"] = "Changed Content"
                });

            Assert.Equal(HttpStatusCode.OK, defaultPage.StatusCode);
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Equal("/Blog", response.Headers.Location.OriginalString);
        }

        [Fact]
        public async void Post_EditPage_InvalidBlogPost_ReturnsOK()
        {
            /*  GET the "edit" page first, just like a web browser would. 
                We need this for the validation token and anti-forgery cookie */
            var defaultPage = await client.GetAsync("/Blog/Edit?id=1");
            var content = await HtmlHelpers.GetDocumentAsync(defaultPage);

            var response = await client.SendAsync(
                (IHtmlFormElement)content.QuerySelector("form"),
                (IHtmlInputElement)content.QuerySelector("input[type='submit']"),
                new Dictionary<string, string>
                {
                    // We need to blank the Content field, as the existing one is sent otherwise
                    ["BlogPost.Title"] = "Changed Title",
                    ["BlogPost.Content"] = ""
                });

            Assert.Equal(HttpStatusCode.OK, defaultPage.StatusCode);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        #endregion



        #region Delete Page Tests

        [Fact]
        public async void Get_DeletePage_ForExistingBlogPost_ReturnsOK()
        {
            var response = await client.GetAsync("/Blog/Delete?id=1");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async void Get_DeletePage_ForNonExistingBlogPost_ReturnsNotFound()
        {
            var response = await client.GetAsync("/Blog/Delete?id=22");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async void Post_DeletePage_ValidPost_ReturnsRedirectToBlogIndex()
        {
            /*  This test deletes a blog post from the database. To isolate the changes from the remainder of
             *  the test suite, we need to use WebHostBuilder to create a new application instance. */
            var client = clientFromWebHostBuilder();

            /*  GET the "delete" page first, just like a web browser would. 
                We need this for the validation token and anti-forgery cookie */
            var defaultPage = await client.GetAsync("/Blog/Delete?id=1");
            var content = await HtmlHelpers.GetDocumentAsync(defaultPage);

            var response = await client.SendAsync(
                (IHtmlFormElement)content.QuerySelector("form"),
                (IHtmlInputElement)content.QuerySelector("input[type='submit']"),
                new Dictionary<string, string>
                {
                    ["BlogPost.Id"] = "1"
                });

            Assert.Equal(HttpStatusCode.OK, defaultPage.StatusCode);
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Equal("/Blog", response.Headers.Location.OriginalString);
        }

        [Fact]
        public async void Post_DeletePage_InvalidPost_ReturnsBadRequest()
        {
            /*  GET the "delete" page first, just like a web browser would. 
                We need this for the validation token and anti-forgery cookie */
            var defaultPage = await client.GetAsync("/Blog/Delete?id=1");
            var content = await HtmlHelpers.GetDocumentAsync(defaultPage);

            var response = await client.SendAsync(
                (IHtmlFormElement)content.QuerySelector("form"),
                (IHtmlInputElement)content.QuerySelector("input[type='submit']"),
                new Dictionary<string, string>
                {
                    // This should cause a BadRequest since POST and url IDs don't match
                    ["BlogPost.Id"] = "2"
                });

            Assert.Equal(HttpStatusCode.OK, defaultPage.StatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        #endregion
    }
}
