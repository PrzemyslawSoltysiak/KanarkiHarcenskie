using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using KanarkiHercenskie.Data;
using KanarkiHercenskie.Models;

namespace KanarkiHercenskie.Pages.CechySpiewu
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public CechaSpiewuCOM CechaSpiewuCOM { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null || _context.CechySpiewuCOM == null)
            {
                return NotFound();
            }

            var cechaspiewucom = await _context.CechySpiewuCOM.FirstOrDefaultAsync(m => m.Nazwa == id);

            if (cechaspiewucom == null)
            {
                return NotFound();
            }
            else 
            {
                CechaSpiewuCOM = cechaspiewucom;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null || _context.CechySpiewuCOM == null)
            {
                return NotFound();
            }
            var cechaspiewucom = await _context.CechySpiewuCOM.FindAsync(id);

            if (cechaspiewucom != null)
            {
                CechaSpiewuCOM = cechaspiewucom;
                _context.CechySpiewuCOM.Remove(CechaSpiewuCOM);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
