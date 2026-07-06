using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IoTDeviceManager.Data;
using IoTDeviceManager.Models;

namespace IoTDeviceManager.Controllers
{
    public class DeviceConfigurationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DeviceConfigurationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DeviceConfigurationContoller
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.DeviceConfigurations.Include(d => d.Microcontroller).Include(d => d.SensorModule);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: DeviceConfigurationContoller/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deviceConfiguration = await _context.DeviceConfigurations
                .Include(d => d.Microcontroller)
                .Include(d => d.SensorModule)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (deviceConfiguration == null)
            {
                return NotFound();
            }

            return View(deviceConfiguration);
        }

        // GET: DeviceConfigurationContoller/Create
        public async Task<IActionResult> Create(int? mcuId)
        {
            if (mcuId.HasValue)
            {
                var mcu = await _context.Microcontrollers.FindAsync(mcuId.Value);
                if (mcu != null)
                {
                    ViewData["IsContextual"] = true;
                    ViewData["TargetMcuId"] = mcu.Id;
                    ViewData["TargetMcuName"] = $"{mcu.ChipModel} ({mcu.MACAddress})";
                }
            }

            ViewData["MicrocontrollerId"] = new SelectList(_context.Microcontrollers, "Id", "MACAddress", mcuId);
            ViewData["SensorModuleId"] = new SelectList(_context.SensorModules, "Id", "SensorType");
            return View();
        }

        // POST: DeviceConfigurationContoller/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DataPin,IsActive,MicrocontrollerId,SensorModuleId")] DeviceConfiguration deviceConfiguration, bool fromMcu = false)
        {
            
            var pinConflict = await _context.DeviceConfigurations
                .AnyAsync(dc => dc.MicrocontrollerId == deviceConfiguration.MicrocontrollerId 
                                && dc.DataPin == deviceConfiguration.DataPin 
                                && dc.IsActive);

            if (pinConflict)
            {
                ModelState.AddModelError("DataPin", "Hardware Conflict! This data pin is already in use.");
            }
            
            if (ModelState.IsValid)
            {
                deviceConfiguration.IsActive = true;
                _context.Add(deviceConfiguration);
                await _context.SaveChangesAsync();
                
                if (fromMcu || Request.Form["fromMcu"] == "true")
                {
                    return RedirectToAction("Details", "Microcontroller", new { id = deviceConfiguration.MicrocontrollerId });
                }
                return RedirectToAction(nameof(Index));
            }
            
            
            if (fromMcu || Request.Form["fromMcu"] == "true")
            {
                var mcu = await _context.Microcontrollers.FindAsync(deviceConfiguration.MicrocontrollerId);
                ViewData["IsContextual"] = true;
                ViewData["TargetMcuId"] = deviceConfiguration.MicrocontrollerId;
                ViewData["TargetMcuName"] = mcu?.ChipModel;
            }

            ViewData["MicrocontrollerId"] = new SelectList(_context.Microcontrollers, "Id", "MACAddress", deviceConfiguration.MicrocontrollerId);
            ViewData["SensorModuleId"] = new SelectList(_context.SensorModules, "Id", "SensorType", deviceConfiguration.SensorModuleId);
            return View(deviceConfiguration);
        }

        // GET: DeviceConfigurationContoller/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deviceConfiguration = await _context.DeviceConfigurations.FindAsync(id);
            if (deviceConfiguration == null)
            {
                return NotFound();
            }
            ViewData["MicrocontrollerId"] = new SelectList(_context.Microcontrollers, "Id", "MACAddress", deviceConfiguration.MicrocontrollerId);
            ViewData["SensorModuleId"] = new SelectList(_context.SensorModules, "Id", "SensorType", deviceConfiguration.SensorModuleId);
            return View(deviceConfiguration);
        }

        // POST: DeviceConfigurationContoller/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DataPin,IsActive,MicrocontrollerId,SensorModuleId")] DeviceConfiguration deviceConfiguration)
        {
            if (id != deviceConfiguration.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(deviceConfiguration);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DeviceConfigurationExists(deviceConfiguration.Id))
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
            ViewData["MicrocontrollerId"] = new SelectList(_context.Microcontrollers, "Id", "MACAddress", deviceConfiguration.MicrocontrollerId);
            ViewData["SensorModuleId"] = new SelectList(_context.SensorModules, "Id", "SensorType", deviceConfiguration.SensorModuleId);
            return View(deviceConfiguration);
        }

        // GET: DeviceConfigurationContoller/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deviceConfiguration = await _context.DeviceConfigurations
                .Include(d => d.Microcontroller)
                .Include(d => d.SensorModule)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (deviceConfiguration == null)
            {
                return NotFound();
            }

            return View(deviceConfiguration);
        }

        // POST: DeviceConfigurationContoller/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deviceConfiguration = await _context.DeviceConfigurations.FindAsync(id);
            if (deviceConfiguration != null)
            {
                _context.DeviceConfigurations.Remove(deviceConfiguration);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DeviceConfigurationExists(int id)
        {
            return _context.DeviceConfigurations.Any(e => e.Id == id);
        }
        
        public async Task<IActionResult> Decommission(int id)
        {
            var config = await _context.DeviceConfigurations.FindAsync(id);
            if (config != null)
            {
                config.IsActive = false; // Mark it as removed from service
                await _context.SaveChangesAsync();
        
                return RedirectToAction("Details", "Microcontroller", new { id = config.MicrocontrollerId });
            }
            return BadRequest();
        }
    }
}
