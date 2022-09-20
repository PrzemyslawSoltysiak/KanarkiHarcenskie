using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using KanarkiHarcenskie.Data;
using KanarkiHarcenskie.Models;

namespace KanarkiHarcenskie.Pages.CechySpiewu
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<CechaSpiewuCOM> CechaSpiewuCOM { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.CechySpiewuCOM != null)
            {
                CechaSpiewuCOM = await _context.CechySpiewuCOM
                    .OrderByDescending(c => c.WagaPunktow)
                    .ThenByDescending(c => c.MaksPunktow)
                    .ThenBy(c => c.Nazwa)
                    .AsNoTracking().ToListAsync();
            }
        }
    }
}
