using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using KanarkiHarcenskie.Data;
using KanarkiHarcenskie.Models;

namespace KanarkiHarcenskie.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (await _context.Konkursy.Where(k => k.ID == 1).AnyAsync() && 
                await _context.Hodowcy.Where(h => h.SygnumHodowcy == "P172").AnyAsync())
            {
                return Redirect("/KartaOceny?idKonkursu=1&sygnumHodowcy=P172");
            }

            return Redirect("/KartaOceny/Index");
        }
    }
}
