using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using restful_blog.Data;

namespace restful_blog.Pages.Blog
{
    public class EditModel : PageModel
    {
        private readonly restful_blog.Data.BlogDbContext _context;

        public EditModel(restful_blog.Data.BlogDbContext context)
        {
            _context = context;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Set UpdatedAt time to now
            Blog.UpdatedAt = DateTime.Now;

            _context.Attach(Blog).State = EntityState.Modified;

            // We don't want to update CreatedAt, since we're editing the object
            _context.Entry(Blog).Property("CreatedAt").IsModified = false;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BlogModelExists(Blog.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool BlogModelExists(int id)
        {
            return _context.BlogModel.Any(e => e.Id == id);
        }
    }
}
