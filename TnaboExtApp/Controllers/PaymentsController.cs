using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using TnaboExtApp.Data;
using TnaboExtApp.Models;

namespace TnaboExtApp.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PaymentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Payments
        public async Task<IActionResult> Index()
        {
            var payments = _context.Payments.Include(p => p.House).AsNoTracking().OrderByDescending(p => p.PaymentDate);
            return View(await payments.ToListAsync());
        }

        // GET: Payments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var payment = await _context.Payments
                .Include(p => p.House)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (payment == null) return NotFound();

            return View(payment);
        }

        // GET: Payments/Create
        public IActionResult Create()
        {
            ViewBag.Houses = GetHousesSelectList();
            ViewBag.Periods = GetPeriodsSelectList();
            ViewBag.Methods = GetMethodsSelectList();
            return View(new Payment { PaymentDate = System.DateTime.UtcNow });
        }

        // POST: Payments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,HouseId,Amount,AmountPaid,Period,PaymentDate,Method,Status")] Payment payment)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Houses = new SelectList(_context.House.OrderBy(h => h.HouseNo).Select(h => new { h.Id, Label = h.HouseNo ?? h.OwnerName }), "Id", "Label", payment.HouseId);
                return View(payment);
            }

            //get house details for denormalized fields
            if (payment.HouseId.HasValue)
            {
                var house = await _context.House.FindAsync(payment.HouseId.Value);
                if (house != null)
                {
                    payment.HouseName = house.OwnerName;
                    payment.HouseNo = house.HouseNo;
                }
            }

            payment.RecordedBy = User?.Identity?.Name ?? "system";
            payment.DateRecorded = System.DateTime.UtcNow;

            _context.Add(payment);
            await _context.SaveChangesAsync();

            // Sum up payment by Period into MonthlyIncome model
            if (!string.IsNullOrEmpty(payment.Period))
            {
                await UpdateMonthlyIncomeAsync(payment.Period);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Payments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var payment = await _context.Payments.FindAsync(id);
            if (payment == null) return NotFound();

            ViewBag.Houses = new SelectList(_context.House.OrderBy(h => h.HouseNo).Select(h => new { h.Id, Label = h.HouseNo ?? h.OwnerName }), "Id", "Label", payment.HouseId);
            return View(payment);
        }

        // POST: Payments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,HouseId,Amount,AmountPaid,Period,PaymentDate,Method,Status")] Payment payment)
        {
            if (id != payment.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Houses = new SelectList(_context.House.OrderBy(h => h.HouseNo).Select(h => new { h.Id, Label = h.HouseNo ?? h.OwnerName }), "Id", "Label", payment.HouseId);
                return View(payment);
            }

            try
            {
                var existing = await _context.Payments.FindAsync(id);
                if (existing == null) return NotFound();

                // update allowed fields
                existing.HouseId = payment.HouseId;
                existing.Amount = payment.Amount;
                existing.AmountPaid = payment.AmountPaid;
                existing.Period = payment.Period;
                existing.PaymentDate = payment.PaymentDate;
                existing.Method = payment.Method;
                existing.Status = payment.Status;

                _context.Update(existing);
                await _context.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Payments.Any(e => e.Id == payment.Id)) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Payments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var payment = await _context.Payments
                .Include(p => p.House)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (payment == null) return NotFound();

            return View(payment);
        }

        // Helper: Generate House SelectList
        private SelectList GetHousesSelectList(int? selectedId = null)
        {
            return new SelectList(
                _context.House.AsNoTracking().OrderBy(h => h.HouseNo)
                    .Select(h => new { h.Id, Label = h.OwnerName ?? h.HouseNo }),
                "Id", "Label", selectedId);
        }

        // Helper: Generate Period SelectList (current year, all 12 months)
        private SelectList GetPeriodsSelectList(string? selectedPeriod = null)
        {
            var currentYear = DateTime.UtcNow.Year;
            var periods = Enumerable.Range(1, 12)
                .Select(m => $"{currentYear}-{m:D2}")
                .Select(p => new { Value = p, Text = p })
                .ToList();

            return new SelectList(periods, "Value", "Text", selectedPeriod);
        }

        // Helper: Generate Method SelectList (common payment methods)
        private SelectList GetMethodsSelectList(string? selectedMethod = null)
        {
            var methods = new List<string> { "Cash", "Bank Transfer", "Check", "Mobile Money", "Card" }
                .Select(m => new { Value = m, Text = m })
                .ToList();

            return new SelectList(methods, "Value", "Text", selectedMethod);
        }


        // Helper: Update MonthlyIncome aggregates for a given period
        private async Task UpdateMonthlyIncomeAsync(string period)
        {
            var payments = await _context.Payments
                .Where(p => p.Period == period)
                .AsNoTracking()
                .ToListAsync();

            if (!payments.Any()) return;

            var totalExpected = payments.Sum(p => p.Amount);
            var totalPaid = payments.Sum(p => p.AmountPaid);
            var totalBalance = totalExpected - totalPaid;
            var paymentCount = payments.Count;

            var monthlyIncome = await _context.MonthlyIncome
                .FirstOrDefaultAsync(m => m.Period == period);

            if (monthlyIncome == null)
            {
                monthlyIncome = new MonthlyIncome
                {
                    Period = period,
                    TotalExpected = totalExpected,
                    TotalPaid = totalPaid,
                    TotalBalance = totalBalance,
                    PaymentCount = paymentCount,
                    DateRecorded = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow
                };
                _context.MonthlyIncome.Add(monthlyIncome);
            }
            else
            {
                monthlyIncome.TotalExpected = totalExpected;
                monthlyIncome.TotalPaid = totalPaid;
                monthlyIncome.TotalBalance = totalBalance;
                monthlyIncome.PaymentCount = paymentCount;
                monthlyIncome.DateUpdated = DateTime.UtcNow;
                _context.MonthlyIncome.Update(monthlyIncome);
            }

            await _context.SaveChangesAsync();
        }
    }
}