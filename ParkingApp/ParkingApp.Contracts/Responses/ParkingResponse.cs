namespace ParkingApp.Contracts.Responses;

public class ParkingResponse
{
    public string RegistrationNumber { get; set; } = null!;
    public int SpaceNumber { get; set; }
    public DateTime ParkedAt { get; set; }
}