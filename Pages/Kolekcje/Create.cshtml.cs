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

        public IActionResult OnGet(int? idKonkursu = null, string? sygnumWlasciciela = null)
        {
            GenerujListy(_context, idKonkursu, sygnumWlasciciela);

            if (idKonkursu != null)
            {
                PrzekierowanoZListyKonkursow = true;
            }
            if (sygnumWlasciciela != null)
            {
                PrzekierowanoZListyHodowcow = true;
            }

            return Page();
        }

        [BindProperty]
        public Kolekcja Kolekcja { get; set; }


        public bool KolekcjaJuzIstnieje = false;
        public bool PrzekierowanoZListyKonkursow = false;
        public bool PrzekierowanoZListyHodowcow = false;


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

            if (_context.Kolekcje.Any(k => (k.Wlasciciel.SygnumHodowcy == Kolekcja.SygnumWlasciciela &&
                                            k.Konkurs.ID == Kolekcja.ID_Konkursu)))
            {
                KolekcjaJuzIstnieje = true;
                GenerujListy(_context);
                return Page();
            }
            else
            {
                KolekcjaJuzIstnieje = false;
            }

            Kolekcja kolekcja = new Kolekcja()
            {
                SygnumWlasciciela = Kolekcja.SygnumWlasciciela,
                ID_Konkursu = Kolekcja.ID_Konkursu
            };

            kolekcja.DodajKlatki(Kolekcja.Klatki[0].NrObraczkiRodowej, Kolekcja.Klatki[1].NrObraczkiRodowej,
                                 Kolekcja.Klatki[2].NrObraczkiRodowej, Kolekcja.Klatki[3].NrObraczkiRodowej);

            await _context.Kolekcje.AddAsync(kolekcja);
            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}
