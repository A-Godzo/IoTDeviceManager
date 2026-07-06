using System.ComponentModel.DataAnnotations;

namespace IoTDeviceManager.Models;

public class SensorModule
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Sensor Type")]
    public string SensorType { get; set; }

    [Display(Name = "Communication Protocol")]
    public string CommunicationProtocol { get; set; }

    public string? Manufacturer { get; set; }

    public ICollection<DeviceConfiguration>? DeviceConfigurations { get; set; }
}