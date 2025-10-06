using ParkingApp.Contracts.Responses;
using ParkingApp.Data.Enums;

namespace ParkingApp.Api.Services;

public interface IParkingService
{
    Task<ParkingResponse> ParkVehicleAsync(string registrationNumber, VehicleType vehicleType);
    Task<StatusResponse> GetCarparkStatusAsync();
    Task<ExitResponse> ExitVehicleAsync(string registrationNumber);
}
