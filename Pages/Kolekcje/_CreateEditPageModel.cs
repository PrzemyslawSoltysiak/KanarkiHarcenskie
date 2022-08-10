using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using KanarkiHercenskie.Data;
using KanarkiHercenskie.Models;

namespace KanarkiHercenskie.Pages.Kolekcje
{
    public class _CreateEditPageModel : PageModel
    {
        protected readonly ApplicationDbContext _context;

        public _CreateEditPageModel(ApplicationDbContext context)
        {
            _context = context;
        }


        // TO-DO: Sprytniejszy (?) sposób generowania list
        public SelectList ListaKonkursow { get; set; }
        public SelectList ListaHodowcow { get; set; }

        protected void GenerujListy()
        {
            if (_context.Konkursy == null || _context.Hodowcy == null)
            {
                throw new Exception("Konkursy == null || Hodowcy == null");
            }

            var slownikKonkursy = _context.Konkursy.ToDictionary
                (k => k.ID,
                 v => v.ToString());

            ListaKonkursow = new SelectList(slownikKonkursy, "Key", "Value");

            var slownikHodowcy = _context.Hodowcy.ToDictionary
                (k => k.SygnumHodowcy,
                 v => v.ToString());

            ListaHodowcow = new SelectList(slownikHodowcy, "Key", "Value");
        }
    }
}
