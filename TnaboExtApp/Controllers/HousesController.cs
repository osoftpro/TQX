using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TnaboExtApp.Data;
using TnaboExtApp.Models;
using TnaboExtApp.Models.ViewModel;

namespace TnaboExtApp.Controllers
{
    public class HousesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HousesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Houses
        public async Task<IActionResult> Index(int? id, string HouseNo, string OwnerName)
        {
            ViewBag.StreetId = id;
            ViewBag.StreetName = _context.Street.Find(id).Name;
            ViewBag.HouseNo = HouseNo;
            ViewBag.OwnerName = OwnerName;

            return View(await _context.House.OrderByDescending(t => t.DateRecorded).Where(t => t.StreetId == id).ToListAsync());
        }

        // GET: Houses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var house = await _context.House
                .FirstOrDefaultAsync(m => m.Id == id);
            if (house == null)
            {
                return NotFound();
            }

            return View(house);
        }

        // GET: Houses/Create
        public IActionResult Create(int? StreetId, string HouseNo, string OwnerName, string Fail)
        {
            ViewBag.streetId = StreetId;
            var getstreet = _context.Street.Find(StreetId);
            var getphase = _context.Phase.Find(getstreet.PhaseId);

            ViewBag.StreetName = getstreet.Name;
            ViewBag.PhaseName = getphase.Name;
            ViewBag.PhaseId = getphase.Id;
            ViewBag.HouseNo = HouseNo;
            ViewBag.OwnerName = OwnerName;
            ViewBag.Fail = Fail;    


            ViewBag.Status = _context.HouseStatus.Select(t => new SelectListItem { Text = t.Name, Value = t.Name });

            return View();
        }

        // POST: Houses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,OwnerName,StreetName,StreetId,PhaseName,PhaseId,PhoneNumber,Email,Longitude,Latitude,Status,HouseNo,CreatedBy,DateRecorded,Photo")] House house, IFormFile FileUpload)
        {

            //if (ModelState.IsValid)
            //{
            //check if house already exist
            var checkhouse = _context.House.FirstOrDefault(t => t.OwnerName == house.OwnerName&&t.StreetId==house.StreetId);
            if (checkhouse!=null)
            {
                return RedirectToAction(nameof(Create), new { StreetId = house.StreetId, Fail = "Sorry House Numbering with this detail already exist"});
            }

            //generating house number
            var getphase = _context.Phase.Find(house.PhaseId);
            var getstreet = _context.Street.Find(house.StreetId);

            //get last number
            var getlastnumber = getstreet.HouseNumber + 1;

            //saving last number
            getstreet.HouseNumber = getlastnumber;
            _context.Update(getstreet);
            _context.SaveChanges();

            //house number
            house.HouseNo = $"{getphase.PhaseCode}/{getstreet.StreetCode}/Plot{getlastnumber}";

            house.CreatedBy = User.Identity.Name;
            house.DateRecorded = DateTime.UtcNow;

            _context.Add(house);
            await _context.SaveChangesAsync();


            //converting image to byte
            if (FileUpload != null)
            {
                if (FileUpload.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        FileUpload.CopyTo(ms);
                        var Photo = ms.ToArray();

                        //create image instance
                        var newhouseimage = new HouseImage
                        {
                            DateRecorded = DateTime.UtcNow,
                            HouseId = house.Id,
                            HouseNo = house.HouseNo,
                            Image = Photo,
                            RecordedBy = User.Identity.Name
                        };

                        _context.Add(newhouseimage);
                        _context.SaveChanges();
                    }
                }

            }

            return RedirectToAction(nameof(Create), new { StreetId = house.StreetId, house.HouseNo, house.OwnerName });

            //}
            //return View(house);
        }

        // GET: Houses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var house = await _context.House.FindAsync(id);
            if (house == null)
            {
                return NotFound();
            }
            return View(house);
        }

        // POST: Houses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OwnerName,StreetName,StreetId,PhaseName,PhaseId,PhoneNumber,Email,Longitude,Latitude,Status,HouseNo,CreatedBy,DateRecorded,Photo")] House house)
        {
            if (id != house.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(house);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HouseExists(house.Id))
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
            return View(house);
        }

        // GET: Houses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var house = await _context.House
                .FirstOrDefaultAsync(m => m.Id == id);
            if (house == null)
            {
                return NotFound();
            }

            return View(house);
        }

        // POST: Houses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var house = await _context.House.FindAsync(id);
            if (house != null)
            {
                _context.House.Remove(house);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public IActionResult ImageCapture(int? HouseId)
        {
            ViewBag.HouseId = HouseId;
            return View();
        }

        public async Task<string> Updateimage()
        {
            var listhouse = await _context.House.Where(t => t.Photo != null).Take(10).ToListAsync();
            foreach (var house in listhouse)
            {
                //check of house already exist
                var checkhouse = await _context.HouseImage.FirstOrDefaultAsync(t => t.HouseId == house.Id);
                if (checkhouse == null)
                {
                    var newhouseImage = new HouseImage
                    {
                        HouseId = house.Id,
                        DateRecorded = house.DateRecorded,
                        HouseNo = house.HouseNo,
                        Image = house.Photo,
                        RecordedBy = house.CreatedBy
                    };
                    _context.HouseImage.Add(newhouseImage);
                    _context.SaveChanges();

                    house.Photo = null;
                    _context.Update(house);
                    _context.SaveChanges();
                }
            }

            return "Successful";
        }

        [HttpPost]
        public IActionResult SavePhoto([FromBody] PhotoModel model)
        {
            string base64 = model.Image.Replace("data:image/png;base64,", "");

            byte[] imageBytes = Convert.FromBase64String(base64);

            var gethouse = _context.House.Find(model.HouseId);
            gethouse.Photo = imageBytes;

            _context.Update(gethouse);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index), new { id = gethouse.StreetId });
        }

        private bool HouseExists(int id)
        {
            return _context.House.Any(e => e.Id == id);
        }
    }
}
