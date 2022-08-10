using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using KanarkiHercenskie.Data;
using KanarkiHercenskie.Models;

namespace KanarkiHercenskie.Pages.Przesluchania
{
    public class CreateModel : _CreateEditPageModel
    {
        public CreateModel(ApplicationDbContext context) : base(context) { }

        public IActionResult OnGet()
        {
            GenerujListeKolekcji();
            return Page();
        }

        [BindProperty]
        public Przesluchanie Przesluchanie { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.Przesluchania == null || Przesluchanie == null)
            {
                return Page();
            }

            _context.Przesluchania.Add(Przesluchanie);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
