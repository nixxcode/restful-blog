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
    public class IndexPageIntegrationTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private HttpClient client;

        public IndexPageIntegrationTests(WebApplicationFactory<Startup> factory)
        {
            client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async void Get_IndexPage_ReturnsOK()
        {
            var response = await client.GetAsync("/blog");
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
