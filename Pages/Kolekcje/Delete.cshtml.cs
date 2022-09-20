using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using KanarkiHarcenskie.Data;
using KanarkiHarcenskie.Models;

namespace KanarkiHarcenskie.Pages.Kolekcje
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Kolekcja Kolekcja { get; set; } = default!;

        public int ZaplanowanePrzesluchania { get; set; } = 0;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Kolekcje == null)
            {
                return NotFound();
            }

            var kolekcja = await _context.Kolekcje.Include(k => k.Wlasciciel).Include(k => k.Konkurs)
                .Include(k => k.Klatki).FirstOrDefaultAsync(m => m.ID == id);

            if (kolekcja == null)
            {
                return NotFound();
            }
            else 
            {
                Kolekcja = kolekcja;
                
                if (_context.Przesluchania != null)
                {
                    ZaplanowanePrzesluchania = await _context.Przesluchania.Where(p => p.ID_Kolekcji == id).CountAsync();
                }
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Kolekcje == null)
            {
                return NotFound();
            }
            var kolekcja = await _context.Kolekcje.FindAsync(id);

            if (kolekcja != null)
            {
                Kolekcja = kolekcja;

                if (await _context.Przesluchania.Where(p => p.ID_Kolekcji == id).AnyAsync())
                {
                    var powiazanePrzesluchania = _context.Przesluchania.Where(p => p.ID_Kolekcji == id);
                    _context.Przesluchania.RemoveRange(powiazanePrzesluchania);
                    await _context.SaveChangesAsync();
                }

                _context.Kolekcje.Remove(Kolekcja);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
