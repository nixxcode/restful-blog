using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using restful_blog.Data;

namespace restful_blog.Pages.Blog
{
    public class DetailsModel : PageModel
    {
        private readonly restful_blog.Data.BlogDbContext _context;

        public DetailsModel(restful_blog.Data.BlogDbContext context)
        {
            _context = context;
        }

        public Data.Blog Blog { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Blog = await _context.BlogModel.FirstOrDefaultAsync(m => m.Id == id);

            if (Blog == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
