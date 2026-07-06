using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IoTDeviceManager.Data;
using IoTDeviceManager.Models;
using Microsoft.AspNetCore.Authorization;

namespace IoTDeviceManager.Controllers
{
    [Authorize(Roles = "Admin,Technician,Viewer")]
    public class MicrocontrollerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MicrocontrollerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Microcontroller
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Microcontrollers.Include(m => m.Site);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Microcontroller/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var microcontroller = await _context.Microcontrollers
                .Include(m => m.Site)
                .Include(m => m.DeviceConfigurations)
                .ThenInclude(dc => dc.SensorModule)
                .Include(m => m.TelemetryLogs) 
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (microcontroller == null)
            {
                return NotFound();
            }

            return View(microcontroller);
        }

        public IActionResult CreateForSite(int? siteId)
        {
            
            ViewBag.siteId = siteId;
            var site = _context.DeploymentSites.FirstOrDefault(s => (s.Id == siteId));
            ViewBag.siteName = site.SiteName;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateForSite([Bind("Id,MACAddress,ChipModel,FirmwareVersion,LastSeenAt,DeploymentSiteId")] Microcontroller microcontroller)
        {
            if (ModelState.IsValid)
            {
                _context.Add(microcontroller);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "DeploymentSite", new {id = microcontroller.DeploymentSiteId});
            }
            ViewData["DeploymentSiteId"] = new SelectList(_context.DeploymentSites, "Id", "SiteName", microcontroller.DeploymentSiteId);
            return View(microcontroller);
        }

        // GET: Microcontroller/Create
        public IActionResult Create()
        {
            ViewData["DeploymentSiteId"] = new SelectList(_context.DeploymentSites, "Id", "SiteName");
            return View();
        }

        // POST: Microcontroller/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MACAddress,ChipModel,FirmwareVersion,LastSeenAt,DeploymentSiteId")] Microcontroller microcontroller)
        {
            if (ModelState.IsValid)
            {
                _context.Add(microcontroller);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DeploymentSiteId"] = new SelectList(_context.DeploymentSites, "Id", "SiteName", microcontroller.DeploymentSiteId);
            return View(microcontroller);
        }

        // GET: Microcontroller/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var microcontroller = await _context.Microcontrollers.FindAsync(id);
            if (microcontroller == null)
            {
                return NotFound();
            }
            ViewData["DeploymentSiteId"] = new SelectList(_context.DeploymentSites, "Id", "SiteName", microcontroller.DeploymentSiteId);
            return View(microcontroller);
        }

        // POST: Microcontroller/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MACAddress,ChipModel,FirmwareVersion,LastSeenAt,DeploymentSiteId")] Microcontroller microcontroller)
        {
            if (id != microcontroller.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(microcontroller);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MicrocontrollerExists(microcontroller.Id))
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
            ViewData["DeploymentSiteId"] = new SelectList(_context.DeploymentSites, "Id", "SiteName", microcontroller.DeploymentSiteId);
            return View(microcontroller);
        }

        // GET: Microcontroller/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var microcontroller = await _context.Microcontrollers
                .Include(m => m.Site)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (microcontroller == null)
            {
                return NotFound();
            }

            return View(microcontroller);
        }

        // POST: Microcontroller/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var microcontroller = await _context.Microcontrollers.FindAsync(id);
            if (microcontroller != null)
            {
                _context.Microcontrollers.Remove(microcontroller);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MicrocontrollerExists(int id)
        {
            return _context.Microcontrollers.Any(e => e.Id == id);
        }
    }
}
