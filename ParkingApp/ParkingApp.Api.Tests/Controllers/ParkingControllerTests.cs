using Moq;
using NUnit.Framework;
using ParkingApp.Api.Controllers;
using ParkingApp.Api.Services;
using ParkingApp.Contracts.Requests;

namespace ParkingApp.Api.Tests.Controllers;

[TestFixture]
public class ParkingControllerTests
{
    private ParkingController _controller;
    private Mock<IParkingService> _mockService;

    [SetUp]
    public void Setup()
    {
        _mockService = new Mock<IParkingService>();
        _controller = new ParkingController(_mockService.Object);
    }

    [Test]
    public async Task ParkVehicle_ShouldCallParkingService()
    {
        // Data
        var registrationNumber = "DX 66600";
        var vehicleType = Data.Enums.VehicleType.Small;

        var request = new ParkingRequest
        {
            RegistrationNumber = registrationNumber,
            VehicleType = vehicleType
        };

        // Test
        await _controller.ParkVehicle(request);

        // Assert
        _mockService.Verify(s => s.ParkVehicleAsync(registrationNumber, vehicleType), Times.Once);
    }

    [Test]
    public async Task GetCarparkStatus_ShouldCallParkingService()
    {
        // Test
        await _controller.GetCarparkStatus();

        // Assert
        _mockService.Verify(s => s.GetCarparkStatusAsync(), Times.Once);
    }

    [Test]
    public async Task ExitVehicle_ShouldCallParkingService()
    {
        // Data
        var registrationNumber = "DX 66600";

        var request = new ExitRequest
        {
            RegistrationNumber = registrationNumber,
        };

        // Test
        await _controller.ExitVehicle(request);

        // Assert
        _mockService.Verify(s => s.ExitVehicleAsync(registrationNumber), Times.Once);
    }
}
