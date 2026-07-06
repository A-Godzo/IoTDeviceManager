using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IoTDeviceManager.Data;
using IoTDeviceManager.Models;
using Microsoft.AspNetCore.Authorization;

namespace IoTDeviceManager.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TelemetryController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TelemetryController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public class TelemetryPayload
    {
        public string MacAddress { get; set; } = string.Empty;
        public string DataPin { get; set; } = string.Empty;
        public double SensorValue { get; set; }
    }

    // POST: api/telemetry
    [HttpPost]
    public async Task<IActionResult> PostReading([FromBody] TelemetryPayload payload)
    {
        // 1. Validation: Reject completely empty payloads
        if (payload == null || string.IsNullOrEmpty(payload.MacAddress) || string.IsNullOrEmpty(payload.DataPin))
        {
            return BadRequest(new { message = "Invalid data packet structure." });
        }

        // 2. Hardware Verification: Does this microcontroller exist in our system?
        var mcu = await _context.Microcontrollers
            .FirstOrDefaultAsync(m => m.MACAddress == payload.MacAddress);

        if (mcu == null)
        {
            return NotFound(new { message = $"Device with MAC Address '{payload.MacAddress}' is not registered in the system." });
        }
        
        var dbConfigs = await _context.DeviceConfigurations
            .Where(dc => dc.MicrocontrollerId == mcu.Id)
            .ToListAsync();

        Console.WriteLine($"\n🔍 [DIAGNOSTIC] Found {dbConfigs.Count} total configurations for MCU ID {mcu.Id}:");
        foreach (var config in dbConfigs)
        {
            Console.WriteLine($"   -> Config ID: {config.Id} | Pin in DB: '{config.DataPin}' (Length: {config.DataPin?.Length}) | IsActive: {config.IsActive}");
        }
        Console.WriteLine($"   -> Python Sent Pin: '{payload.DataPin}' (Length: {payload.DataPin?.Length})\n");
// 👆 END OF DIAGNOSTIC BLOCK 👆

        // 3. Configuration Verification: Is there a sensor actually wired to this pin?
        var activeConfig = await _context.DeviceConfigurations
            .Include(dc => dc.SensorModule)
            .FirstOrDefaultAsync(dc => dc.MicrocontrollerId == mcu.Id 
                                     && dc.DataPin == payload.DataPin 
                                     && dc.IsActive);

        if (activeConfig == null)
        {
            return BadRequest(new { message = $"Hardware Mismatch! MAC {payload.MacAddress} has no active sensor mapped to pin '{payload.DataPin}'." });
        }

        // 4. Update the Microcontroller's "Last Seen" timestamp
        mcu.LastSeenAt = DateTime.UtcNow;
        _context.Update(mcu);

        // 5. Success! For now, we will log it to the server console.
        // Console.WriteLine($"[Telemetry Received] Site ID: {mcu.DeploymentSiteId} | MCU: {mcu.ChipModel} | Sensor: {activeConfig.SensorModule?.SensorType} on {payload.DataPin} | Value: {payload.SensorValue}");
        string logPayload = $"Sensor on pin {payload.DataPin} reported value: {payload.SensorValue}";
        
        SeverityLevel logSeverity = SeverityLevel.Info;
        if (payload.SensorValue > 30.0) // Dynamic rule: Flag high temperatures
        {
            logSeverity = SeverityLevel.Warning;
        }
        

        // 6. Instantiate your exact TelemetryLog model
        var telemetryLog = new TelemetryLog
        {
            MicrocontrollerId = mcu.Id,
            Payload = logPayload,
            Severity = logSeverity,
            Timestamp = DateTime.UtcNow
        };

        _context.TelemetryLogs.Add(telemetryLog); 
        await _context.SaveChangesAsync();

// Existing console print statement
        _context.TelemetryLogs.Add(telemetryLog); // (Assumes your DbSet is named TelemetryLogs)
        await _context.SaveChangesAsync();

        Console.WriteLine($"[Saved to DB] MCU ID: {mcu.Id} | Severity: {logSeverity} | {logPayload}");

        return Ok(new { 
            message = "Data packet logged successfully.", 
            timestamp = telemetryLog.Timestamp 
        });
    }
    [HttpPost("ClearAll")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ClearAll(int microcontrollerId)
    {
        // 1. Find only the logs belonging to this specific microcontroller
        var deviceLogs = _context.TelemetryLogs.Where(l => l.MicrocontrollerId == microcontrollerId);
    
        // 2. Wipe them out
        _context.TelemetryLogs.RemoveRange(deviceLogs);
        await _context.SaveChangesAsync();
    
        // 3. Explicitly redirect back to: /Microcontrollers/Details/{id}
        return RedirectToAction("Details", "Microcontroller", new { id = microcontrollerId });
    }
}