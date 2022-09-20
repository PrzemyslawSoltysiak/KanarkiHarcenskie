using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using KanarkiHarcenskie.Data;
using KanarkiHarcenskie.Models;

namespace KanarkiHarcenskie.Pages.Kolekcje
{
    public class _CreateEditPageModel : PageModel
    {
        public SelectList ListaKonkursow { get; set; }
        public SelectList ListaHodowcow { get; set; }

        protected void GenerujListy(ApplicationDbContext context, object wybranyKonkurs = null, object wybranyHodowca = null)
        {
            if (context.Konkursy == null || context.Hodowcy == null)
            {
                throw new Exception("Konkursy == null || Hodowcy == null");
            }

            var konkursyQuery = from konkursy in context.Konkursy
                                orderby konkursy.ID
                                select konkursy;

            ListaKonkursow = new SelectList(konkursyQuery, "ID", "Podsumowanie", wybranyKonkurs);

            var hodowcyQuery = from hodowcy in context.Hodowcy
                               orderby hodowcy.SygnumHodowcy
                               select hodowcy;

            ListaHodowcow = new SelectList(hodowcyQuery, "SygnumHodowcy", "Podsumowanie", wybranyHodowca);
        }
    }
}
