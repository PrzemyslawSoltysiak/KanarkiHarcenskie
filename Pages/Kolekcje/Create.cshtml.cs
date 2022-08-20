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
    public class CreateModel : _CreateEditPageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            GenerujListy(_context);
            return Page();
        }

        [BindProperty]
        public string SygnumWlasciciela { get; set; }

        [BindProperty]
        public int ID_Konkursu { get; set; }

        // TO-DO: Podmiana na tablicę Klatek (patrz: strona Edycji)
        [BindProperty]
        public int NrObraczkiRodowej_1 { get; set; }
        [BindProperty]
        public int NrObraczkiRodowej_2 { get; set; }
        [BindProperty]
        public int NrObraczkiRodowej_3 { get; set; }
        [BindProperty]
        public int NrObraczkiRodowej_4 { get; set; }


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || 
                _context.Konkursy == null || 
                _context.Hodowcy == null || 
                _context.Kolekcje == null ||
                _context.Klatki == null)
            {
                return Page();
            }

            // TO-DO: Może rozwiązać to w jakiś sprytniejszy sposób
            if (_context.Kolekcje.Any(k => (k.Wlasciciel.SygnumHodowcy == SygnumWlasciciela &&
                                            k.Konkurs.ID == ID_Konkursu)))
                throw new Exception("Do każdego Konkursu Hodowca może wystawić TYLKO JEDNĄ Kolekcję.");

            if (_context.Klatki.Any(
                k => (k.Kolekcja.Wlasciciel.SygnumHodowcy == SygnumWlasciciela &&
                      k.Kolekcja.Konkurs.ID == ID_Konkursu)))
                throw new Exception("Istnieją już Klatki przypisane do Kolekcji.");


            Kolekcja kolekcja = new Kolekcja()
            {
                SygnumWlasciciela = this.SygnumWlasciciela,
                ID_Konkursu = this.ID_Konkursu
            };

            kolekcja.DodajKlatki(NrObraczkiRodowej_1, NrObraczkiRodowej_2, NrObraczkiRodowej_3, NrObraczkiRodowej_4);

            await _context.Kolekcje.AddAsync(kolekcja);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
