using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FoldrengesekSOE2026.Data;
using FoldrengesekSOE2026.Models;

namespace FoldrengesekSOE2026.Controllers
{
    public class NaploController : Controller
    {
        private readonly FoldrengesContext _context;

        public NaploController(FoldrengesContext context)
        {
            _context = context;
        }

        // GET: Naplo
        public async Task<IActionResult> Index(DateTime? datum, int? telepulesid, double? magMin, double? magMax)
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

            ViewData["TelepulesID"] = new SelectList(
                _context.Telepulesek,
                "ID",
                "Nev",
                telepulesid ?? 0
            );

            return View(await foldrengesek.ToListAsync());

        }

        // GET: Naplo/Details/5
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
