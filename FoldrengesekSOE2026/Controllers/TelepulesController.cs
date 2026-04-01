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
    public class TelepulesController : Controller
    {
        private readonly FoldrengesContext _context;

        public TelepulesController(FoldrengesContext context)
        {
            _context = context;
        }

        // GET: Telepules
        public async Task<IActionResult> Index(
            string? nev, 
            string? varmegye,
            int page = 1,
            string sort = "nev",
            string dir = "asc"
            )
        {

            var telepulesek = _context.Telepulesek.AsQueryable();

            if (!string.IsNullOrEmpty(nev))
            {
                telepulesek = telepulesek
                    .Where(p => p.Nev!.ToLower().Contains(nev.ToLower()));

                ViewData["AktualisNevSzuro"] = nev;
            }

            if (!string.IsNullOrEmpty(varmegye))
            {
                telepulesek = telepulesek
                    .Where(p => p.Varmegye!.ToLower().Contains(varmegye.ToLower()));

                ViewData["AktualisVarmegyeSzuro"] = varmegye;
            }

            telepulesek = (sort, dir) switch
            {
                ("nev", "desc") => telepulesek.OrderByDescending(p => p.Nev),
                ("varmegye", "asc") => telepulesek.OrderBy(p => p.Varmegye),
                ("varmegye", "desc") => telepulesek.OrderByDescending(p => p.Varmegye),
                _ => telepulesek.OrderBy(p => p.Nev)
            };

            ViewData["CurrentSort"] = sort;
            ViewData["CurrentDir"] = dir;

            int pageSize = 10; // ennyi elem egy oldalon

            int totalCount = await telepulesek.CountAsync();
            var items = await telepulesek
                //.OrderBy(p => p.Nev)   // ⚠️ lapozásnál KÖTELEZŐ rendezni
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = (int)Math.Ceiling(totalCount / (double)pageSize);
            ViewData["TotalCount"] = totalCount;

            return View(items);


        }

        // GET: Telepules/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var telepules = await _context.Telepulesek
                .FirstOrDefaultAsync(m => m.ID == id);
            if (telepules == null)
            {
                return NotFound();
            }

            return View(telepules);
        }

        // GET: Telepules/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Telepules/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Nev,Varmegye")] Telepules telepules)
        {
            if (ModelState.IsValid)
            {
                _context.Add(telepules);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(telepules);
        }

        // GET: Telepules/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var telepules = await _context.Telepulesek.FindAsync(id);
            if (telepules == null)
            {
                return NotFound();
            }
            return View(telepules);
        }

        // POST: Telepules/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Nev,Varmegye")] Telepules telepules)
        {
            if (id != telepules.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(telepules);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TelepulesExists(telepules.ID))
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
            return View(telepules);
        }

        // GET: Telepules/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var telepules = await _context.Telepulesek
                .FirstOrDefaultAsync(m => m.ID == id);
            if (telepules == null)
            {
                return NotFound();
            }

            return View(telepules);
        }

        // POST: Telepules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var telepules = await _context.Telepulesek.FindAsync(id);
            if (telepules != null)
            {
                _context.Telepulesek.Remove(telepules);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TelepulesExists(int id)
        {
            return _context.Telepulesek.Any(e => e.ID == id);
        }
    }
}
