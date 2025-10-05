using ParkingApp.Data.Enums;

namespace ParkingApp.Contracts.Requests;

public class ParkingRequest
{
    public required string RegistrationNumber { get; set; }
    public required VehicleType Type { get; set; }
}