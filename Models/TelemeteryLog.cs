using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IoTDeviceManager.Models;

public enum SeverityLevel
{
    Info,
    Warning,
    Error
}

public class TelemetryLog
{
    public int Id { get; set; }

    public int MicrocontrollerId { get; set; }

    [Required]
    public string Payload { get; set; } 

    [Display(Name = "Severity")]
    public SeverityLevel Severity { get; set; }

    [Display(Name = "Timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    [ForeignKey("MicrocontrollerId")]
    public Microcontroller? Microcontroller { get; set; }
}