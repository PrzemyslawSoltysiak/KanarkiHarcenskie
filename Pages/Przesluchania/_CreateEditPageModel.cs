using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using KanarkiHarcenskie.Data;
using KanarkiHarcenskie.Models;

namespace KanarkiHarcenskie.Pages.Przesluchania
{
    public class _CreateEditPageModel : PageModel
    {
        public SelectList ListaKolekcji { get; set; }

        protected void GenerujListeKolekcji(ApplicationDbContext context, object wybranaKolekcja = null)
        {
            if (context.Kolekcje == null || context.Hodowcy == null || context.Konkursy == null)
            {
                throw new Exception("context.Kolekcje == null || context.Hodowcy == null || context.Konkursy == null");
            }

            var slownikKolekcje = context.Kolekcje.ToDictionary(
                k => k.ID,
                v => (context.Hodowcy.Where(h => h.SygnumHodowcy == v.SygnumWlasciciela).First().Podsumowanie) + ", " +
                     context.Konkursy.Where(k => k.ID == v.ID_Konkursu).First().Podsumowanie);

            ListaKolekcji = new SelectList(slownikKolekcje, "Key", "Value", wybranaKolekcja);
        }

    }
}
