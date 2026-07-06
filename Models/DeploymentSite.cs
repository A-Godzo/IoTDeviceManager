using System.ComponentModel.DataAnnotations;

namespace IoTDeviceManager.Models;

public class DeploymentSite
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Site Name")]
    public string SiteName { get; set; }

    [Display(Name = "Physical Address")]
    public string? PhysicalAddress { get; set; }

    [Display(Name = "Network Subnet")]
    public string? NetworkSubnet { get; set; }

    [Display(Name = "Active Status")]
    public bool IsActive { get; set; } = true;

    public ICollection<Microcontroller>? Microcontrollers { get; set; }
}