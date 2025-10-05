namespace ParkingApp.Contracts.Responses;
public class ExistResponse
{
    public string RegistrationNumber { get; set; }
    public DateTime ParkedAt { get; set; }
    public DateTime ExitedAt { get; set; }
    public decimal ParkingFee { get; set; }
}
