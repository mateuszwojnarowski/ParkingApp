using ParkingApp.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace ParkingApp.Data.Models;
public class ParkingRecord
{
    [Key]
    public string RegistrationNumber { get; set; } = null!;
    public VehicleType VehicleType { get; set; }
    public int SpaceNumber { get; set; }
    public DateTime ParkedAt { get; set; }
    public DateTime? ExitedAt { get; set; }
    public double? ParkingFee { get; set; }
}
