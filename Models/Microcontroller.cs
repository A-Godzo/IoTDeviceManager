using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IoTDeviceManager.Models;

public class Microcontroller
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "MAC Address")]
    // format (00:1A:2B:3C:4D:5E)
    [RegularExpression(@"^([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})$", ErrorMessage = "Invalid MAC Address format.")]
    public string MACAddress { get; set; }

    [Display(Name = "Chip Model")]
    public string ChipModel { get; set; }

    [Display(Name = "Firmware Version")]
    public string? FirmwareVersion { get; set; }

    [Display(Name = "Last Seen")]
    public DateTime? LastSeenAt { get; set; }

    [Display(Name = "Deployment Site")]
    public int DeploymentSiteId { get; set; }

    // Navigation Properties
    [ForeignKey("DeploymentSiteId")]
    public DeploymentSite? Site { get; set; }

    public ICollection<DeviceConfiguration>? DeviceConfigurations { get; set; }
    public ICollection<TelemetryLog>? TelemetryLogs { get; set; }
}
