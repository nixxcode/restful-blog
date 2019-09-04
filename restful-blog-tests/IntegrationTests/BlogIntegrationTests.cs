using AngleSharp.Html.Dom;
using Microsoft.AspNetCore.Mvc.Testing;
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

namespace restful_blog_tests.Tests
{
    public class BlogIntegrationTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private HttpClient client;

        public BlogIntegrationTests(CustomWebApplicationFactory<Startup> factory)
        {
            client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Theory]
        [InlineData("/")]
        [InlineData("/Blog")]
        [InlineData("/Blog/Create")]
        public async void Get_BasePages_ReturnsOK(string url)
        {
            var response = await client.GetAsync(url);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

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
        public async void Post_CreatePage_ValidBlogPost_ReturnsRedirectToIndex()
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
                    ["BlogPost.Title"] = "Some Title"
                });

            Assert.Equal(HttpStatusCode.OK, defaultPage.StatusCode);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }







        /*
        [Fact]
        public async void Get_IndexPage_ReturnsPosts()
        {
            var response = await client.GetAsync("/blog");
            var content = await HtmlHelpers.GetDocumentAsync(defaultPage);

            Assert.Equal(HttpStatusCode.OK, defaultPage.StatusCode);
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Equal("/Blog", response.Headers.Location.OriginalString);
        }
        */
    }
}
