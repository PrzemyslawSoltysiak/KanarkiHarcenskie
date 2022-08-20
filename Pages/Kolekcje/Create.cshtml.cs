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
        public Kolekcja Kolekcja { get; set; } 


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (_context.Konkursy == null || 
                _context.Hodowcy == null || 
                _context.Kolekcje == null ||
                _context.Klatki == null)
            {
                GenerujListy(_context);
                return Page();
            }

            // TO-DO: Może rozwiązać to w jakiś sprytniejszy sposób
            if (_context.Kolekcje.Any(k => (k.Wlasciciel.SygnumHodowcy == Kolekcja.SygnumWlasciciela &&
                                            k.Konkurs.ID == Kolekcja.ID_Konkursu)))
                throw new Exception("Do każdego Konkursu Hodowca może wystawić TYLKO JEDNĄ Kolekcję.");

            if (_context.Klatki.Any(
                k => (k.Kolekcja.Wlasciciel.SygnumHodowcy == Kolekcja.SygnumWlasciciela &&
                      k.Kolekcja.Konkurs.ID == Kolekcja.ID_Konkursu)))
                throw new Exception("Istnieją już Klatki przypisane do Kolekcji.");


            Kolekcja kolekcja = new Kolekcja()
            {
                SygnumWlasciciela = Kolekcja.SygnumWlasciciela,
                ID_Konkursu = Kolekcja.ID_Konkursu
            };

            kolekcja.DodajKlatki(Kolekcja.Klatki[0].NrObraczkiRodowej, Kolekcja.Klatki[1].NrObraczkiRodowej,
                                 Kolekcja.Klatki[2].NrObraczkiRodowej, Kolekcja.Klatki[3].NrObraczkiRodowej);

            await _context.Kolekcje.AddAsync(kolekcja);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
