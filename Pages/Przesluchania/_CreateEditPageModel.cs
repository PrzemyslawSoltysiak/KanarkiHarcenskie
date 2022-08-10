using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using KanarkiHercenskie.Data;
using KanarkiHercenskie.Models;

namespace KanarkiHercenskie.Pages.Przesluchania
{
    public class _CreateEditPageModel : PageModel
    {
        protected readonly ApplicationDbContext _context;

        public _CreateEditPageModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public SelectList ListaKolekcji { get; set; }

        protected void GenerujListeKolekcji()
        {
            if (_context == null || _context.Kolekcje == null || 
                _context.Hodowcy == null || _context.Konkursy == null)
            {
                throw new Exception();
            }

            var slownikKolekcje = _context.Kolekcje.ToDictionary(
                k => k.ID,
                v => (_context.Hodowcy.Where(h => h.SygnumHodowcy == v.SygnumWlasciciela).First().Podsumowanie) + ", " +
                     _context.Konkursy.Where(k => k.ID == v.ID_Konkursu).First().Podsumowanie);

            ListaKolekcji = new SelectList(slownikKolekcje, "Key", "Value");
        }

    }
}
