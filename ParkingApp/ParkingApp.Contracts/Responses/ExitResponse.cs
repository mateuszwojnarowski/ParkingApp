namespace ParkingApp.Contracts.Responses;
public class ExitResponse
{
    public string RegistrationNumber { get; set; } = null!;
    public DateTime ParkedAt { get; set; }
    public DateTime ExitedAt { get; set; }
    public double ParkingFee { get; set; }
}
