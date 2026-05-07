using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FoldrengesekSOE2026.Data;
using FoldrengesekSOE2026.Models;
using Microsoft.AspNetCore.Authorization;

namespace FoldrengesekSOE2026.Controllers
{
    [Authorize]
    public class NaploController : Controller
    {
        private readonly FoldrengesContext _context;

        public NaploController(FoldrengesContext context)
        {
            _context = context;
        }

        // GET: Naplo
        [AllowAnonymous]
        public async Task<IActionResult> Index(
            DateTime? datum, 
            int? telepulesid, 
            double? magMin, 
            double? magMax, 
            int page = 1,
            string sort = "datum",
            string dir = "asc"
        )
        {
            var foldrengesek = _context.Naplok.Include(n => n.Telepules).AsQueryable();

            if (datum.HasValue)
            {
                foldrengesek = foldrengesek
                    .Where(n => n.Datum == datum);

                ViewData["AktualisDatumSzuro"] = datum.Value.ToString("yyyy-MM-dd");
            }

            if (telepulesid != null && telepulesid > 0)
            {
                foldrengesek = foldrengesek
                    .Where(b => b.TelepulesID == telepulesid);
            }
            if (magMin.HasValue && magMax.HasValue)
            {
                foldrengesek = foldrengesek
                    .Where(n => n.Magnitudo >= magMin && n.Magnitudo <= magMax);
                ViewData["AktualisMagnitudoMinSzuro"] = magMin.Value;
                ViewData["AktualisMagnitudoMaxSzuro"] = magMax.Value;
            }
            else if (magMin.HasValue)
            {
                foldrengesek = foldrengesek
                    .Where(n => n.Magnitudo >= magMin);
                ViewData["AktualisMagnitudoMinSzuro"] = magMin.Value;
            }
            else if (magMax.HasValue)
            {
                foldrengesek = foldrengesek
                    .Where(n => n.Magnitudo <= magMax);
                ViewData["AktualisMagnitudoMaxSzuro"] = magMax.Value;
            }

            // A TelepulesID SelectList tartalmát a szűrés(ek)nek megfelelően határozzuk meg.
            // Ha volt bármilyen szűrés, akkor a jelenleg rendezett/szűrt naplók településeiből építjük a listát,
            // különben a teljes település lista kerül a SelectList-be.
            bool voltSzures = datum.HasValue || telepulesid.HasValue || magMin.HasValue || magMax.HasValue;

            if (voltSzures)
            {
                var telepulesekSzurt = await foldrengesek
                    .Where(n => n.Telepules != null)
                    .Select(n => new { n.Telepules!.ID, n.Telepules!.Nev })
                    .Distinct()
                    .OrderBy(t => t.Nev)
                    .ToListAsync();

                ViewData["TelepulesID"] = new SelectList(telepulesekSzurt, "ID", "Nev", telepulesid ?? 0);
            }
            else
            {
                var osszesTelepules = await _context.Telepulesek
                    .OrderBy(t => t.Nev)
                    .Select(t => new { t.ID, t.Nev })
                    .ToListAsync();

                ViewData["TelepulesID"] = new SelectList(osszesTelepules, "ID", "Nev", telepulesid ?? 0);
            }
            ViewData["TelepId"] = telepulesid;

            foldrengesek = (sort, dir) switch
            {
                ("datum", "desc") => foldrengesek.OrderByDescending(p => p.Datum),
                ("intenzitas", "asc") => foldrengesek.OrderBy(p => p.Intenzitas),
                ("intenzitas", "desc") => foldrengesek.OrderByDescending(p => p.Intenzitas),
                ("magnitudo", "asc") => foldrengesek.OrderBy(p => p.Magnitudo),
                ("magnitudo", "desc") => foldrengesek.OrderByDescending(p => p.Magnitudo),
                ("telepulesnev", "asc") => foldrengesek.OrderBy(p => p.Telepules!.Nev),
                ("telepulesnev", "desc") => foldrengesek.OrderByDescending(p => p.Telepules!.Nev),
                _ => foldrengesek.OrderBy(p => p.Datum)
            };

            ViewData["CurrentSort"] = sort;
            ViewData["CurrentDir"] = dir;

            int pageSize = 10; // ennyi elem egy oldalon

            int totalCount = await foldrengesek.CountAsync();
            var items = await foldrengesek
                //.OrderBy(p => p.Datum)   // ⚠️ lapozásnál KÖTELEZŐ rendezni
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = (int)Math.Ceiling(totalCount / (double)pageSize);

            ViewData["TotalCount"] = totalCount;

            return View(items);
        }

        // GET: Naplo/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var naplo = await _context.Naplok
                .Include(n => n.Telepules)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (naplo == null)
            {
                return NotFound();
            }

            return View(naplo);
        }

        // GET: Naplo/Create
        [Authorize(Roles = "User,Admin")]
        public IActionResult Create()
        {
            ViewData["TelepulesID"] = new SelectList(_context.Telepulesek, "ID", "Nev");
            return View();
        }

        // POST: Naplo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> Create([Bind("ID,Datum,Ido,Magnitudo,Intenzitas,TelepulesID")] Naplo naplo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(naplo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TelepulesID"] = new SelectList(_context.Telepulesek, "ID", "Nev", naplo.TelepulesID);
            return View(naplo);
        }

        // GET: Naplo/Edit/5
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var naplo = await _context.Naplok.FindAsync(id);
            if (naplo == null)
            {
                return NotFound();
            }
            ViewData["TelepulesID"] = new SelectList(_context.Telepulesek, "ID", "Nev", naplo.TelepulesID);
            return View(naplo);
        }

        // POST: Naplo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Datum,Ido,Magnitudo,Intenzitas,TelepulesID")] Naplo naplo)
        {
            if (id != naplo.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(naplo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NaploExists(naplo.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["TelepulesID"] = new SelectList(_context.Telepulesek, "ID", "Nev", naplo.TelepulesID);
            return View(naplo);
        }

        // GET: Naplo/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var naplo = await _context.Naplok
                .Include(n => n.Telepules)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (naplo == null)
            {
                return NotFound();
            }

            return View(naplo);
        }

        // POST: Naplo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var naplo = await _context.Naplok.FindAsync(id);
            if (naplo != null)
            {
                _context.Naplok.Remove(naplo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NaploExists(int id)
        {
            return _context.Naplok.Any(e => e.ID == id);
        }
    }
}
