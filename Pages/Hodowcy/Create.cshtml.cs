using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using KanarkiHarcenskie.Data;
using KanarkiHarcenskie.Models;

namespace KanarkiHarcenskie.Pages.Hodowcy
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
        public Hodowca Hodowca { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            Hodowca.SygnumHodowcy = await Hodowca.WyznaczSygnumAsync(_context, Hodowca.Nazwisko);

            if (!ModelState.IsValid || _context.Hodowcy == null || Hodowca == null)
            {
                return Page();
            }

            _context.Hodowcy.Add(Hodowca);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
