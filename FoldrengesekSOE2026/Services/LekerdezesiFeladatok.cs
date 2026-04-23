using FoldrengesekSOE2026.Data;
using FoldrengesekSOE2026.ViewModels;

namespace FoldrengesekSOE2026.Services
{
    public class LekerdezesiFeladatok : ILekerdezesiFeladatok
    {
        private readonly FoldrengesContext _context;
        public LekerdezesiFeladatok(FoldrengesContext context) => _context = context;

        //public IQueryable<string> SomogyTelepulesNevek()
        //{
        //    return _context.Telepulesek
        //        .Where(t => t.Varmegye == "Somogy")
        //        .OrderBy(t => t.Nev)
        //        .Select(t => t.Nev);
        //}

        // A fenti metódus rövidített változata, ami ugyanazt az eredményt adja vissza
        // A két metódus ekvivalens egymással.
        public IQueryable<string> SomogyTelepulesNevek()
            => _context.Telepulesek
                .Where(t => t.Varmegye == "Somogy")
                .OrderBy(t => t.Nev)
                .Select(t => t.Nev);

        public IQueryable<Feladat3ViewModel> VarmegyeiRengesSzamok()
            => _context.Naplok
                .GroupBy(n => n.Telepules!.Varmegye)
                .Select(g => new Feladat3ViewModel
                {
                    Varmegye = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(t => t.Count);

        public Feladat4ViewModel? LegnagyobbMagnitudo()
            => _context.Naplok
                .OrderByDescending(n => n.Magnitudo)
                .Select(n => new Feladat4ViewModel
                {
                    Nev = n.Telepules!.Nev,
                    Datum = n.Datum,
                    Ido = n.Ido,
                    Magnitudo = n.Magnitudo
                })
                .FirstOrDefault();

        public IQueryable<Feladat5ViewModel> AligErzekelheto2022()
        {
            return _context.Naplok
                .Where(x =>
                    x.Datum.Year == 2022 &&
                    x.Intenzitas >= 2.0 &&
                    x.Intenzitas <= 3.0)
                .OrderBy(x => x.Datum)
                .Select(n => new Feladat5ViewModel
                {
                    Nev = n.Telepules!.Nev,
                    Datum = n.Datum,
                    Intenzitas = n.Intenzitas
                });
        }

        public IQueryable<Feladat6ViewModel> Top3Ev_3nalNagyobbIntenzitassal()
        {
            return _context.Naplok
                .Where(n => n.Intenzitas > 3.0)
                .GroupBy(n => n.Datum.Year)
                .Select(g => new Feladat6ViewModel
                {
                    Year = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .Take(3);
        }

    }
}
