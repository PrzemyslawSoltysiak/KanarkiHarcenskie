using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using KanarkiHarcenskie.Data;
using KanarkiHarcenskie.Models;

namespace KanarkiHarcenskie.Pages.Przesluchania
{
    public class CreateModel : _CreateEditPageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet(int? idKolekcji)
        {
            GenerujListeKolekcji(_context, idKolekcji);
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
