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
    public class DeploymentSiteController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DeploymentSiteController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DeploymentSite
        public async Task<IActionResult> Index(string? siteName, string? active)
        {

            var deploymentSites = await _context.DeploymentSites
                .Where(s=>(siteName == null || s.SiteName.Contains(siteName)))
                .ToListAsync();
            
                if (active == "Active")
                {
                    deploymentSites = deploymentSites.Where(s => (s.IsActive)).ToList();
                }
                else if (active == "Inactive")
                {
                    deploymentSites = deploymentSites.Where(s => (!s.IsActive)).ToList();

                }
                ViewData["CurrentSearch"] = siteName;
                ViewData["CurrentActiveFilter"] = active;
            return View(deploymentSites);
        }

        // GET: DeploymentSite/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deploymentSite = await _context.DeploymentSites
                .Include(s => s.Microcontrollers)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (deploymentSite == null)
            {
                return NotFound();
            }

            return View(deploymentSite);
        }

        // GET: DeploymentSite/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DeploymentSite/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SiteName,PhysicalAddress,NetworkSubnet,IsActive")] DeploymentSite deploymentSite)
        {
            if (ModelState.IsValid)
            {
                _context.Add(deploymentSite);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(deploymentSite);
        }

        // GET: DeploymentSite/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deploymentSite = await _context.DeploymentSites.FindAsync(id);
            if (deploymentSite == null)
            {
                return NotFound();
            }
            return View(deploymentSite);
        }

        // POST: DeploymentSite/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SiteName,PhysicalAddress,NetworkSubnet,IsActive")] DeploymentSite deploymentSite)
        {
            if (id != deploymentSite.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(deploymentSite);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DeploymentSiteExists(deploymentSite.Id))
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
            return View(deploymentSite);
        }

        // GET: DeploymentSite/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deploymentSite = await _context.DeploymentSites
                .FirstOrDefaultAsync(m => m.Id == id);
            if (deploymentSite == null)
            {
                return NotFound();
            }

            return View(deploymentSite);
        }

        // POST: DeploymentSite/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deploymentSite = await _context.DeploymentSites.FindAsync(id);
            if (deploymentSite != null)
            {
                _context.DeploymentSites.Remove(deploymentSite);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DeploymentSiteExists(int id)
        {
            return _context.DeploymentSites.Any(e => e.Id == id);
        }
    }
}
