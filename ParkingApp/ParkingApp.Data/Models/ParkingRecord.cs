using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingApp.Data.Models;
public class ParkingRecord
{
    public Guid Id { get; set; }
    public string RegistrationNumber { get; set; }
    public int SpaceNumber { get; set; }
    public DateTime ParkedAt { get; set; }
    public DateTime? ExitedAt { get; set; }
    public decimal? ParkingFee { get; set; }
}
