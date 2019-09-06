# RESTful Blog (ASP.NET Core)
Simple blog application written in ASP.NET Razor Pages with an API based on the RESTful architecture, using Entity Framework to manipulate data.

### Operations
The RESTful blog supports the following operations via dedicated URLs:

- Create
- Edit
- Read
- Update
- Delete

### Features
- Input validation via ASP.NET model. HTML tags are also escaped to ensure the app isn't vulnerable to injection
- Basic text formatting is preserved when the post is saved. This includes spaces and line breaks
- Shows post "captions" on the blog index page. Unlike full posts, Captions are limited to either 100 characters or 5 lines, whichever comes first. This is to ensure they do not take up too much space on the index page
- Basic Bootstrap and css layout to sensibly position post elements such as title, body, created/edited dates and edit/delete links

### Integration Tests
The Visual Studio solution is split into two projects. 

- <strong>restful-blog</strong>, which contains the actual application
- <strong>restful-blog-tests</strong>, which contains tests for the application

The tests project features a full suite of integration tests, using the .NET built-in HttpClient and custom code to ensure expected responses/redirects are returned for specific URLs and operations.

### Usage

After cloning the repository, do the following to run the app/tests

APP:
1. Open the solution file with Visual Studio.
2. Build the solution.
3. Navigate to the restful-blog directory in command prompt.
4. Run the following command: dotnet ef database update (this will create the database from the included migrations folder)
5. Run the application

A web browser tab should open for you automatically, pointing to the root page of the application.

TESTS:
1. Navigate to the restful-blog-tests directory in command prompt
2. Run the following command: dotnet test
