using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using KanarkiHercenskie.Data;
using KanarkiHercenskie.Models;

namespace KanarkiHercenskie.Pages.Konkursy
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Konkurs Konkurs { get; set; } = default!;

        public int ZarejestrowaneKolekcje { get; set; } = 0;
        public int ZaplanowanePrzesluchania { get; set; } = 0;


        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Konkursy == null)
            {
                return NotFound();
            }

            var konkurs = await _context.Konkursy.FirstOrDefaultAsync(m => m.ID == id);

            if (konkurs == null)
            {
                return NotFound();
            }
            else 
            {
                Konkurs = konkurs;

                if (_context.Kolekcje != null)
                {
                    ZarejestrowaneKolekcje = await _context.Kolekcje.Where(k => k.ID_Konkursu == id).CountAsync();

                    if (_context.Przesluchania != null)
                    {
                        ZaplanowanePrzesluchania = await _context.Przesluchania
                            .Where(p => p.PrzesluchiwanaKolekcja.ID_Konkursu == id).CountAsync();
                    }
                }
                
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Konkursy == null)
            {
                return NotFound();
            }
            var konkurs = await _context.Konkursy.FindAsync(id);

            if (konkurs != null)
            {
                Konkurs = konkurs;

                // TO-DO: Czy da się wprowadzić kaskadowe usuwanie Przesłuchań w automatyczny sposób?
                // (prawdopodobna przyczyna (do sprawdzenia), dlaczego Przesłuchiwania nie są usuwane kaskadowo automatycznie:
                //  są one powiązane z Konkursem niebezpośrednio (poprzez tablicę Kolekcje)).
                if (await _context.Przesluchania.Where(p => p.PrzesluchiwanaKolekcja.ID_Konkursu == id).AnyAsync())
                {
                    var powiazanePrzesluchania = _context.Przesluchania.Where(p => p.PrzesluchiwanaKolekcja.ID_Konkursu == id);
                    _context.Przesluchania.RemoveRange(powiazanePrzesluchania);
                    await _context.SaveChangesAsync();
                }

                _context.Konkursy.Remove(Konkurs);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
