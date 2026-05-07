using FoldrengesekSOE2026.Data;
using FoldrengesekSOE2026.Models;
using FoldrengesekSOE2026.Services;
using FoldrengesekSOE2026.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FoldrengesekSOE2026.Controllers
{
    [Authorize(Roles = "User,Admin")]
    public class FeladatokController : Controller
    {
        private readonly FoldrengesContext _context;

        private readonly ILekerdezesiFeladatok _queries;

        public FeladatokController(FoldrengesContext context, ILekerdezesiFeladatok queries)
        {
            _context = context;
            _queries = queries;
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
            var results = _queries.SomogyTelepulesNevek();

            return View(results);
        }

        //SELECT varmegye, COUNT(*)
        //FROM telepulesek
        //INNER JOIN naplok ON naplok.telepulesid = telepulesek.id
        //GROUP BY varmegye
        //ORDER BY COUNT(*) DESC
        public IActionResult Feladat3()
        {
            var results = _queries.VarmegyeiRengesSzamok();

            return View(results);
        }

        //SELECT nev, datum, ido, magnitudo
        //FROM naplok
        //INNER JOIN telepulesek ON telepulesek.id = naplok.telepulesid
        //ORDER BY magnitudo DESC
        //LIMIT 1
        public IActionResult Feladat4()
        {
            var result = _queries.LegnagyobbMagnitudo();

            return View(result);
        }

        //SELECT nev, datum, intenzitas
        //FROM telepulesek
        //INNER JOIN naplok ON naplok.telepulesid = telepulesek.id
        //WHERE YEAR(datum) = 2022 AND intenzitas BETWEEN 2.0 AND 3.0
        //ORDER BY datum
        public IActionResult Feladat5()
        {
            var results = _queries.AligErzekelheto2022();

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
            var results = _queries.Top3Ev_3nalNagyobbIntenzitassal();

            return View(results);
        }
    }
}
