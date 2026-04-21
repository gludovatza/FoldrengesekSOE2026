using FoldrengesekSOE2026.Data;
using FoldrengesekSOE2026.Models;
using FoldrengesekSOE2026.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FoldrengesekSOE2026.Controllers
{
    public class FeladatokController : Controller
    {
        private readonly FoldrengesContext _context;

        public FeladatokController(FoldrengesContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        // SELECT nev
        // FROM telepulesek
        // WHERE varmegye = "Somogy"
        // ORDER BY nev
        public IActionResult Feladat2()
        {
            var results = _context.Telepulesek
                .Where(t => t.Varmegye == "Somogy")
                .OrderBy(t => t.Nev)
                .Select(t => t.Nev);

            return View(results);
        }

        //SELECT varmegye, COUNT(*)
        //FROM telepulesek
        //INNER JOIN naplok ON naplok.telepulesid = telepulesek.id
        //GROUP BY varmegye
        //ORDER BY COUNT(*) DESC
        public IActionResult Feladat3()
        {
            var results = _context.Naplok
                .GroupBy(n => n.Telepules!.Varmegye)
                .Select(g => new Feladat3ViewModel
                {
                    Varmegye = g.Key, // a mező, ami szerint csoportosítva van: Varmegye
                    Count = g.Count()
                })
                .OrderByDescending(t => t.Count);

            return View(results);
        }

        //SELECT nev, datum, ido, magnitudo
        //FROM naplok
        //INNER JOIN telepulesek ON telepulesek.id = naplok.telepulesid
        //ORDER BY magnitudo DESC
        //LIMIT 1
        public IActionResult Feladat4()
        {
            var result = _context.Naplok
                .OrderByDescending(n => n.Magnitudo)
                .Select(n => new Feladat4ViewModel
                {
                    Nev = n.Telepules!.Nev,
                    Datum = n.Datum,
                    Ido = n.Ido,
                    Magnitudo = n.Magnitudo
                })
                .FirstOrDefault();
            return View(result);
        }

        //SELECT nev, datum, intenzitas
        //FROM telepulesek
        //INNER JOIN naplok ON naplok.telepulesid = telepulesek.id
        //WHERE YEAR(datum) = 2022 AND intenzitas BETWEEN 2.0 AND 3.0
        //ORDER BY datum
        public IActionResult Feladat5()
        {
            var results = _context.Naplok
                .Where(n => n.Datum.Year == 2022 && n.Intenzitas >= 2.0 && n.Intenzitas <= 3.0)
                .OrderBy(n => n.Datum)
                .Select(n => new Feladat5ViewModel
                {
                    Nev = n.Telepules!.Nev,
                    Datum = n.Datum,
                    Intenzitas = n.Intenzitas
                });
            return View(results);
        }

        //SELECT YEAR(datum), COUNT(*)
        //FROM naplok
        //WHERE intenzitas > 3.0
        //GROUP BY YEAR(datum)
        //ORDER BY COUNT(*) DESC
        //LIMIT 3
        public IActionResult Feladat6()
        {
            var results = _context.Naplok
                .Where(n => n.Intenzitas > 3.0)
                .GroupBy(n => n.Datum.Year)
                .Select(g => new Feladat6ViewModel
                {
                    Year = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(t => t.Count)
                .Take(3);
            return View(results);
        }


    }
}
