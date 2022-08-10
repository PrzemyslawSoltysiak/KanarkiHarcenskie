using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using KanarkiHercenskie.Data;
using KanarkiHercenskie.Models;

namespace KanarkiHercenskie.Pages.Konkursy
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Konkurs> Konkursy { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Konkursy != null)
            {
                Konkursy = await _context.Konkursy.AsNoTracking().ToListAsync();
            }
        }
    }
}
