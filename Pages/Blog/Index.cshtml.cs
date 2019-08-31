using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using restful_blog.Models;

namespace restful_blog.Pages.Blog
{
    public class IndexModel : PageModel
    {
        private readonly restful_blog.Models.BlogDbContext _context;

        public IndexModel(restful_blog.Models.BlogDbContext context)
        {
            _context = context;
        }

        public IList<BlogModel> BlogModel { get;set; }

        public async Task OnGetAsync()
        {
            BlogModel = await _context.BlogModel.ToListAsync();
        }
    }
}
