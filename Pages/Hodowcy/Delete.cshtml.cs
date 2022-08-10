using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using KanarkiHercenskie.Data;
using KanarkiHercenskie.Models;

namespace KanarkiHercenskie.Pages.Hodowcy
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Hodowca Hodowca { get; set; } = default!;

        public int ZarejestrowaneKolekcje { get; set; } = 0;
        public int ZaplanowanePrzesluchania { get; set; } = 0;


        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null || _context.Hodowcy == null)
            {
                return NotFound();
            }

            var hodowca = await _context.Hodowcy.FirstOrDefaultAsync(m => m.SygnumHodowcy == id);

            if (hodowca == null)
            {
                return NotFound();
            }
            else 
            {
                Hodowca = hodowca;

                if (_context.Kolekcje != null)
                {
                    ZarejestrowaneKolekcje = await _context.Kolekcje.Where(k => k.SygnumWlasciciela == id).CountAsync();

                    if (_context.Przesluchania != null)
                    {
                        ZaplanowanePrzesluchania = await _context.Przesluchania
                            .Where(p => p.PrzesluchiwanaKolekcja.SygnumWlasciciela == id).CountAsync();
                    }
                }
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null || _context.Hodowcy == null)
            {
                return NotFound();
            }
            var hodowca = await _context.Hodowcy.FindAsync(id);

            if (hodowca != null)
            {
                Hodowca = hodowca;

                if (await _context.Przesluchania.Where(p => p.PrzesluchiwanaKolekcja.SygnumWlasciciela == id).AnyAsync())
                {
                    var powiazanePrzesluchania = _context.Przesluchania.Where(p => p.PrzesluchiwanaKolekcja.SygnumWlasciciela == id);
                    _context.Przesluchania.RemoveRange(powiazanePrzesluchania);
                    await _context.SaveChangesAsync();
                }

                _context.Hodowcy.Remove(Hodowca);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
