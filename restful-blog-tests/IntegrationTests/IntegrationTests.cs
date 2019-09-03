using AngleSharp.Html.Dom;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using RazorPagesProject.Tests.Helpers;
using restful_blog;
using restful_blog.Data;
using restful_blog.Pages.Blog;
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
    public class IntegrationTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private HttpClient client;

        public IntegrationTests(WebApplicationFactory<Startup> factory)
        {
            client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async void submitting_valid_blog_post_redirects_to_index()
        {
            /*  GET the "create" page first, just like a web browser would. 
                We need this for the validation token and anti-forgery cookie */
            var defaultPage = await client.GetAsync("/blog/create");
            var content = await HtmlHelpers.GetDocumentAsync(defaultPage);

            // Act
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
        public async void submitting_invalid_blog_post_doesnt_redirect()
        {
            /*  GET the "create" page first, just like a web browser would. 
                We need this for the validation token and anti-forgery cookie */
            var defaultPage = await client.GetAsync("/blog/create");
            var content = await HtmlHelpers.GetDocumentAsync(defaultPage);

            // Act
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
    }
}
