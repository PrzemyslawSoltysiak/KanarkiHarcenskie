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

namespace KanarkiHarcenskie.Pages.CechySpiewu
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public CechaSpiewuCOM CechaSpiewuCOM { get; set; } = default!;

        public WagaPunktow _Wagi { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null || _context.CechySpiewuCOM == null)
            {
                return NotFound();
            }

            var cechaspiewucom =  await _context.CechySpiewuCOM.FirstOrDefaultAsync(m => m.Nazwa == id);
            if (cechaspiewucom == null)
            {
                return NotFound();
            }
            CechaSpiewuCOM = cechaspiewucom;
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

            _context.Attach(CechaSpiewuCOM).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CechaSpiewuCOMExists(CechaSpiewuCOM.Nazwa))
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

        private bool CechaSpiewuCOMExists(string id)
        {
          return (_context.CechySpiewuCOM?.Any(e => e.Nazwa == id)).GetValueOrDefault();
        }
    }
}
