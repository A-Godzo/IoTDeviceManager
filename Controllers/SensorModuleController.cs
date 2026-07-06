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
    [Authorize(Roles = "Admin,Technician")]
    public class SensorModuleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SensorModuleController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SensorModule
        public async Task<IActionResult> Index()
        {
            return View(await _context.SensorModules.ToListAsync());
        }

        // GET: SensorModule/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sensorModule = await _context.SensorModules
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sensorModule == null)
            {
                return NotFound();
            }

            return View(sensorModule);
        }

        // GET: SensorModule/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SensorModule/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SensorType,CommunicationProtocol,Manufacturer")] SensorModule sensorModule)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sensorModule);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(sensorModule);
        }

        // GET: SensorModule/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sensorModule = await _context.SensorModules.FindAsync(id);
            if (sensorModule == null)
            {
                return NotFound();
            }
            return View(sensorModule);
        }

        // POST: SensorModule/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SensorType,CommunicationProtocol,Manufacturer")] SensorModule sensorModule)
        {
            if (id != sensorModule.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sensorModule);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SensorModuleExists(sensorModule.Id))
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
            return View(sensorModule);
        }

        // GET: SensorModule/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sensorModule = await _context.SensorModules
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sensorModule == null)
            {
                return NotFound();
            }

            return View(sensorModule);
        }

        // POST: SensorModule/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sensorModule = await _context.SensorModules.FindAsync(id);
            if (sensorModule != null)
            {
                _context.SensorModules.Remove(sensorModule);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SensorModuleExists(int id)
        {
            return _context.SensorModules.Any(e => e.Id == id);
        }
    }
}
