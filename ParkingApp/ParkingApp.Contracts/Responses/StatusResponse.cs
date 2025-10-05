using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingApp.Contracts.Responses;
public class StatusResponse
{
    public int TotalSpaces { get; set; }
    public int OccupiedSpaces { get; set; }
    public int AvailableSpaces { get; set; }
}
