using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IoTDeviceManager.Models;

public class DeviceConfiguration
{
    public int Id { get; set; }
    
    [Required]
    [Display(Name = "Data Pin")]
    public string DataPin { get; set; } 

    [Display(Name = "Active Status")]
    public bool IsActive { get; set; } = true;

    public int MicrocontrollerId { get; set; }
    
    [ForeignKey("MicrocontrollerId")]
    public Microcontroller? Microcontroller { get; set; }

    public int SensorModuleId { get; set; }
    
    [ForeignKey("SensorModuleId")]
    public SensorModule? SensorModule { get; set; }
}