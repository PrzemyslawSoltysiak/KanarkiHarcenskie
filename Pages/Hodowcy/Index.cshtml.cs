using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using KanarkiHercenskie.Data;
using KanarkiHercenskie.Models;

namespace KanarkiHercenskie.Pages.Hodowcy
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Hodowca> Hodowcy { get; set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Hodowcy != null)
            {
                Hodowcy = await _context.Hodowcy.AsNoTracking().ToListAsync();
            }
        }
    }
}
