using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KanarkiHercenskie.Data;
using KanarkiHercenskie.Models;

namespace KanarkiHercenskie.Pages.Kolekcje
{
    public class EditModel : _CreateEditPageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Kolekcja Kolekcja { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Kolekcje == null || _context.Klatki == null)
            {
                return NotFound();
            }

            var kolekcja = await _context.Kolekcje
                .Include(k => k.Klatki).FirstOrDefaultAsync(m => m.ID == id);
            if (kolekcja == null)
            {
                return NotFound();
            }
            Kolekcja = kolekcja;

            GenerujListy(_context);
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            var kolekcja = await _context.Kolekcje.Include(k => k.Klatki)
                .Where(k => (k.SygnumWlasciciela == Kolekcja.SygnumWlasciciela &&
                             k.ID_Konkursu == Kolekcja.ID_Konkursu))
                .FirstAsync();

            kolekcja.Klatki[0].NrObraczkiRodowej = Kolekcja.Klatki[0].NrObraczkiRodowej;
            kolekcja.Klatki[1].NrObraczkiRodowej = Kolekcja.Klatki[1].NrObraczkiRodowej;
            kolekcja.Klatki[2].NrObraczkiRodowej = Kolekcja.Klatki[2].NrObraczkiRodowej;
            kolekcja.Klatki[3].NrObraczkiRodowej = Kolekcja.Klatki[3].NrObraczkiRodowej;

            _context.Attach(kolekcja).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KolekcjaExists(Kolekcja.ID))
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

        private bool KolekcjaExists(int id)
        {
          return (_context.Kolekcje?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
