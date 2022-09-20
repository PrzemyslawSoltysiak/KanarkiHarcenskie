using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using KanarkiHarcenskie.Data;
using KanarkiHarcenskie.Models;

namespace KanarkiHarcenskie.Pages.CechySpiewu
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public CechaSpiewuCOM CechaSpiewuCOM { get; set; } = default!;

        public WagaPunktow _Wagi { get; set; }
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || _context.CechySpiewuCOM == null || CechaSpiewuCOM == null)
            {
                return Page();
            }

            _context.CechySpiewuCOM.Add(CechaSpiewuCOM);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
