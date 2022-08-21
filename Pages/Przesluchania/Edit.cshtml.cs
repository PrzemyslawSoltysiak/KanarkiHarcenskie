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

namespace KanarkiHercenskie.Pages.Przesluchania
{
    public class EditModel : _CreateEditPageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Przesluchanie Przesluchanie { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Przesluchania == null)
            {
                return NotFound();
            }

            var przesluchanie =  await _context.Przesluchania.FirstOrDefaultAsync(m => m.ID == id);
            if (przesluchanie == null)
            {
                return NotFound();
            }
            Przesluchanie = przesluchanie;
            GenerujListeKolekcji(_context, przesluchanie.ID_Kolekcji);
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

            _context.Attach(Przesluchanie).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PrzesluchanieExists(Przesluchanie.ID))
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

        private bool PrzesluchanieExists(int id)
        {
          return (_context.Przesluchania?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
