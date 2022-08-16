using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using KanarkiHercenskie.Data;
using KanarkiHercenskie.Models;

namespace KanarkiHercenskie.Pages.KartaOceny
{
    public class WierszWynikow
    {
        public WierszWynikow()
        {
            ID_Klatek = new int[4];
            PrzyznanePunkty = new int[4];
            BledyWalidacji = new bool[4];
        }

        public void DodajKolekcje(Kolekcja kolekcja)
        {
            ID_Klatek[0] = kolekcja.Klatki[0].ID;
            ID_Klatek[2] = kolekcja.Klatki[1].ID;
            ID_Klatek[3] = kolekcja.Klatki[2].ID;
            ID_Klatek[4] = kolekcja.Klatki[3].ID;
        }

        public bool Waliduj(ApplicationDbContext context)
        {
            bool bladWalidacji = false;
            for (int i = 0; i < 4; ++i)
            {
                if (PrzyznanePunkty[i] > context.CechySpiewuCOM.Find(NazwaCechySpiewu).MaksPunktow)
                {
                    bladWalidacji = true;
                    BledyWalidacji[i] = true;
                }
                else
                {
                    BledyWalidacji[i] = false;
                }
            }
            return bladWalidacji;
        }

        public Wynik this[int index]
        {
            get
            {
                return new Wynik() 
                { 
                    NazwaCechySpiewu = this.NazwaCechySpiewu,
                    ID_Klatki = ID_Klatek[index],
                    PrzyznanePunkty = this.PrzyznanePunkty[index]
                };
            }
        }

        public string NazwaCechySpiewu { get; set; }
        public int[] ID_Klatek { get; set; }
        public int[] PrzyznanePunkty { get; set; }
        public bool[] BledyWalidacji { get; set; }
    }
}
