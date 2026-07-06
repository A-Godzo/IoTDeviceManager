using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using IoTDeviceManager.Models;
using Microsoft.AspNetCore.Identity;

namespace IoTDeviceManager.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<DeploymentSite> DeploymentSites { get; set; }
    public DbSet<Microcontroller> Microcontrollers { get; set; }
    public DbSet<SensorModule> SensorModules { get; set; }
    public DbSet<DeviceConfiguration> DeviceConfigurations { get; set; }
    public DbSet<TelemetryLog> TelemetryLogs { get; set; }
}