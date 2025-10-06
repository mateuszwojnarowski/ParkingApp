using Microsoft.AspNetCore.Mvc;
using ParkingApp.Api.Services;
using ParkingApp.Contracts.Requests;

namespace ParkingApp.Api.Controllers;
public class ParkingController(IParkingService parkingService) : Controller
{
    private readonly IParkingService _parkingService = parkingService;

    [HttpPost("/parking")]
    public async Task<IActionResult> ParkVehicle([FromBody] ParkingRequest request)
    {
        var response = await _parkingService.ParkVehicleAsync(request.RegistrationNumber, request.VehicleType);
        return Ok(response);
    }
    [HttpGet("/parking")]
    public async Task<IActionResult> GetCarparkStatus()
    {
        var response = await _parkingService.GetCarparkStatusAsync();
        return Ok(response);
    }
    [HttpPost("/parking/exit")]
    public async Task<IActionResult> ExitVehicle([FromBody] ExitRequest request)
    {
        var response = await _parkingService.ExitVehicleAsync(request.RegistrationNumber);
        return Ok(response);
    }
}
