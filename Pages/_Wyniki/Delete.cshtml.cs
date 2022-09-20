using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using KanarkiHarcenskie.Data;
using KanarkiHarcenskie.Models;

namespace KanarkiHarcenskie.Pages.Wyniki
{
    public class DeleteModel : PageModel
    {
        private readonly KanarkiHarcenskie.Data.ApplicationDbContext _context;

        public DeleteModel(KanarkiHarcenskie.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
      public Wynik Wynik { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Wyniki == null)
            {
                return NotFound();
            }

            var wynik = await _context.Wyniki.FirstOrDefaultAsync(m => m.ID == id);

            if (wynik == null)
            {
                return NotFound();
            }
            else 
            {
                Wynik = wynik;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Wyniki == null)
            {
                return NotFound();
            }
            var wynik = await _context.Wyniki.FindAsync(id);

            if (wynik != null)
            {
                Wynik = wynik;
                _context.Wyniki.Remove(Wynik);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
