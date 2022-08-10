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

namespace KanarkiHercenskie.Pages.Wyniki
{
    public class EditModel : PageModel
    {
        private readonly KanarkiHercenskie.Data.ApplicationDbContext _context;

        public EditModel(KanarkiHercenskie.Data.ApplicationDbContext context)
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

            var wynik =  await _context.Wyniki.FirstOrDefaultAsync(m => m.ID == id);
            if (wynik == null)
            {
                return NotFound();
            }
            Wynik = wynik;
           ViewData["ID_Klatki"] = new SelectList(_context.Klatki, "ID", "ID");
           ViewData["NazwaCechySpiewu"] = new SelectList(_context.CechySpiewuCOM, "Nazwa", "Nazwa");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Wynik).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WynikExists(Wynik.ID))
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

        private bool WynikExists(int id)
        {
          return (_context.Wyniki?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
