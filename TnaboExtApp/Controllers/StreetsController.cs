using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TnaboExtApp.Data;
using TnaboExtApp.Models;

namespace TnaboExtApp.Controllers
{
    public class StreetsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StreetsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Streets
        public async Task<IActionResult> Index(int? id)
        {
            ViewBag.PhaseId = id;
            ViewBag.Phase = _context.Phase.Find(id).Name;

            return View(await _context.Street.Where(t=>t.PhaseId==id).ToListAsync());
        }

        // GET: Streets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var street = await _context.Street
                .FirstOrDefaultAsync(m => m.Id == id);
            if (street == null)
            {
                return NotFound();
            }

            return View(street);
        }

        // GET: Streets/Create
        public IActionResult Create(int? PhaseId)
        {
            ViewBag.PhaseId = PhaseId;
            ViewBag.PhaseName = _context.Phase.Find(PhaseId).Name;

            return View();
        }

        // POST: Streets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PhaseId,Name,StreetCode,CreatedBy,DateRecorded,PhaseName,HouseNumber")] Street street)
        {
            if (ModelState.IsValid)
            {
                //save other details
                street.CreatedBy = User.Identity.Name;
                street.DateRecorded = DateTime.UtcNow;

                _context.Add(street);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new {id = street.PhaseId});
            }
            return View(street);
        }

        // GET: Streets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var street = await _context.Street.FindAsync(id);
            if (street == null)
            {
                return NotFound();
            }
            return View(street);
        }

        // POST: Streets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PhaseId,Name,StreetCode,CreatedBy,DateRecorded,PhaseName,HouseNumber")] Street street)
        {
            if (id != street.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(street);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StreetExists(street.Id))
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
            return View(street);
        }

        // GET: Streets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var street = await _context.Street
                .FirstOrDefaultAsync(m => m.Id == id);
            if (street == null)
            {
                return NotFound();
            }

            return View(street);
        }

        // POST: Streets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var street = await _context.Street.FindAsync(id);
            if (street != null)
            {
                _context.Street.Remove(street);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StreetExists(int id)
        {
            return _context.Street.Any(e => e.Id == id);
        }
    }
}
