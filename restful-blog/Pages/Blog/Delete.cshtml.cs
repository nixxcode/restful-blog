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
    public class DeleteModel : PageModel
    {
        private readonly restful_blog.Data.BlogDbContext _context;

        public DeleteModel(restful_blog.Data.BlogDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Data.BlogPost BlogPost { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            BlogPost = await _context.Blog.FirstOrDefaultAsync(m => m.Id == id);

            if (BlogPost == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            BlogPost = await _context.Blog.FindAsync(id);

            if (BlogPost != null)
            {
                _context.Blog.Remove(BlogPost);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }

    }
}
