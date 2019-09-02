﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using restful_blog.Models;

namespace restful_blog.Pages.Blog
{
    public class DeleteModel : PageModel
    {
        private readonly restful_blog.Models.BlogDbContext _context;

        public DeleteModel(restful_blog.Models.BlogDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Models.Blog Blog { get; set; }

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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Blog = await _context.BlogModel.FindAsync(id);

            if (Blog != null)
            {
                _context.BlogModel.Remove(Blog);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }

    }
}
