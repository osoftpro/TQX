
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using TnaboExtApp.Data;
using TnaboExtApp.Models;

namespace TnaboExtApp.Controllers
{
    public class ExpensesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExpensesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Expenses
        public async Task<IActionResult> Index()
        {
            var expenses = _context.Expenses.Include(e => e.RelatedHouse).AsNoTracking().OrderByDescending(e => e.Date);
            return View(await expenses.ToListAsync());
        }

        // GET: Expenses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var expense = await _context.Expenses
                .Include(e => e.RelatedHouse)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (expense == null) return NotFound();

            return View(expense);
        }

        // GET: Expenses/Create
        public IActionResult Create()
        {
            ViewBag.Houses = new SelectList(_context.House.OrderBy(h => h.HouseNo).Select(h => new { h.Id, Label = h.HouseNo ?? h.OwnerName }), "Id", "Label");
            return View(new Expense { Date = System.DateTime.UtcNow });
        }

        // POST: Expenses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Category,Amount,Date,Description,RelatedHouseId,ReceiptUrl")] Expense expense)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Houses = new SelectList(_context.House.OrderBy(h => h.HouseNo).Select(h => new { h.Id, Label = h.HouseNo ?? h.OwnerName }), "Id", "Label", expense.RelatedHouseId);
                return View(expense);
            }

            expense.RecordedBy = User?.Identity?.Name ?? "system";
            expense.DateRecorded = System.DateTime.UtcNow;

            _context.Add(expense);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Expenses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null) return NotFound();

            ViewBag.Houses = new SelectList(_context.House.OrderBy(h => h.HouseNo).Select(h => new { h.Id, Label = h.HouseNo ?? h.OwnerName }), "Id", "Label", expense.RelatedHouseId);
            return View(expense);
        }

        // POST: Expenses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Category,Amount,Date,Description,RelatedHouseId,ReceiptUrl")] Expense expense)
        {
            if (id != expense.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Houses = new SelectList(_context.House.OrderBy(h => h.HouseNo).Select(h => new { h.Id, Label = h.HouseNo ?? h.OwnerName }), "Id", "Label", expense.RelatedHouseId);
                return View(expense);
            }

            try
            {
                var existing = await _context.Expenses.FindAsync(id);
                if (existing == null) return NotFound();

                existing.Category = expense.Category;
                existing.Amount = expense.Amount;
                existing.Date = expense.Date;
                existing.Description = expense.Description;
                existing.RelatedHouseId = expense.RelatedHouseId;
                existing.ReceiptUrl = expense.ReceiptUrl;

                _context.Update(existing);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Expenses.Any(e => e.Id == expense.Id)) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Expenses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var expense = await _context.Expenses
                .Include(e => e.RelatedHouse)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (expense == null) return NotFound();

            return View(expense);
        }

        // POST: Expenses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense != null)
            {
                _context.Expenses.Remove(expense);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}