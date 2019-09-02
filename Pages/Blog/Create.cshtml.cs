﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Query.Internal;
using restful_blog.Models;

namespace restful_blog.Pages.Blog
{
    public class CreateModel : PageModel
    {
        private readonly restful_blog.Models.BlogDbContext _context;

        public CreateModel(restful_blog.Models.BlogDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Models.Blog Blog { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Set CreatedAt to now
            Blog.CreatedAt = DateTime.Now;

            _context.BlogModel.Add(Blog);

            // We don't want to change UpdatedAt, since we're only just creating the object
            _context.Entry(Blog).Property("UpdatedAt").IsModified = false;

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}