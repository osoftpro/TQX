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
    public class HouseStatusController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HouseStatusController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: HouseStatus
        public async Task<IActionResult> Index()
        {
            return View(await _context.HouseStatus.ToListAsync());
        }

        // GET: HouseStatus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var houseStatus = await _context.HouseStatus
                .FirstOrDefaultAsync(m => m.id == id);
            if (houseStatus == null)
            {
                return NotFound();
            }

            return View(houseStatus);
        }

        // GET: HouseStatus/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: HouseStatus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,Name")] HouseStatus houseStatus)
        {
            if (ModelState.IsValid)
            {
                _context.Add(houseStatus);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(houseStatus);
        }

        // GET: HouseStatus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var houseStatus = await _context.HouseStatus.FindAsync(id);
            if (houseStatus == null)
            {
                return NotFound();
            }
            return View(houseStatus);
        }

        // POST: HouseStatus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("id,Name")] HouseStatus houseStatus)
        {
            if (id != houseStatus.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(houseStatus);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HouseStatusExists(houseStatus.id))
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
            return View(houseStatus);
        }

        // GET: HouseStatus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var houseStatus = await _context.HouseStatus
                .FirstOrDefaultAsync(m => m.id == id);
            if (houseStatus == null)
            {
                return NotFound();
            }

            return View(houseStatus);
        }

        // POST: HouseStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var houseStatus = await _context.HouseStatus.FindAsync(id);
            if (houseStatus != null)
            {
                _context.HouseStatus.Remove(houseStatus);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

       

        private bool HouseStatusExists(int? id)
        {
            return _context.HouseStatus.Any(e => e.id == id);
        }
    }
}
