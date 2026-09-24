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
    public class HouseImagesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HouseImagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: HouseImages
        public async Task<IActionResult> Index()
        {
            return View(await _context.HouseImage.ToListAsync());
        }

        // GET: HouseImages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var houseImage = await _context.HouseImage
                .FirstOrDefaultAsync(m => m.Id == id);
            if (houseImage == null)
            {
                return NotFound();
            }

            return View(houseImage);
        }

        // GET: HouseImages/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: HouseImages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,HouseId,HouseNo,Image,RecordedBy,DateRecorded")] HouseImage houseImage)
        {
            if (ModelState.IsValid)
            {
                _context.Add(houseImage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(houseImage);
        }

        // GET: HouseImages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var houseImage = await _context.HouseImage.FindAsync(id);
            if (houseImage == null)
            {
                return NotFound();
            }
            return View(houseImage);
        }

        // POST: HouseImages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,HouseId,HouseNo,Image,RecordedBy,DateRecorded")] HouseImage houseImage)
        {
            if (id != houseImage.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(houseImage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HouseImageExists(houseImage.Id))
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
            return View(houseImage);
        }

        // GET: HouseImages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var houseImage = await _context.HouseImage
                .FirstOrDefaultAsync(m => m.Id == id);
            if (houseImage == null)
            {
                return NotFound();
            }

            return View(houseImage);
        }

        // POST: HouseImages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var houseImage = await _context.HouseImage.FindAsync(id);
            if (houseImage != null)
            {
                _context.HouseImage.Remove(houseImage);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HouseImageExists(int id)
        {
            return _context.HouseImage.Any(e => e.Id == id);
        }
    }
}
