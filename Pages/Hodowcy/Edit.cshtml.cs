using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KanarkiHarcenskie.Data;
using KanarkiHarcenskie.Models;

namespace KanarkiHarcenskie.Pages.Hodowcy
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Hodowca Hodowca { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null || _context.Hodowcy == null)
            {
                return NotFound();
            }

            var hodowca =  await _context.Hodowcy.FirstOrDefaultAsync(m => m.SygnumHodowcy == id);
            if (hodowca == null)
            {
                return NotFound();
            }
            Hodowca = hodowca;
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

            _context.Attach(Hodowca).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HodowcaExists(Hodowca.SygnumHodowcy))
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

        private bool HodowcaExists(string id)
        {
          return (_context.Hodowcy?.Any(e => e.SygnumHodowcy == id)).GetValueOrDefault();
        }
    }
}
