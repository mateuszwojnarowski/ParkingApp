using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using ParkingApp.Api.Services;
using ParkingApp.Data.Context;
using ParkingApp.Data.Enums;

namespace ParkingApp.Api.Tests.Services;

[TestFixture]
public class ParkingServiceTests
{
    private Mock<IConfiguration> _mockConfiguration;
    private Mock<IConfigurationSection> _mockConfigurationSection;
    private ParkingAppContext _context;

    [SetUp]
    public void Setup()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockConfigurationSection = new Mock<IConfigurationSection>();

        _mockConfiguration.Setup(x => x.GetSection(It.IsAny<string>())).Returns(_mockConfigurationSection.Object);
        _mockConfigurationSection.Setup(x => x.Value).Returns("1");

        var options = new DbContextOptionsBuilder<ParkingAppContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ParkingAppContext(options);
    }

    [Test]
    public void Constructor_ShouldGetParkingCapacityFromConfiguration()
    {
        // Test
        new ParkingService(_mockConfiguration.Object, _context);

        // Verify
        _mockConfiguration.Verify(x => x.GetSection("CarParkCapacity"), Times.Once);
    }

    [Test]
    public async Task ParkVehicleAsync_ShoutThrowWhenParkingSameCarAgain()
    {
        // Data
        var registrationNumber = "DW 66600";
        var vehicleType = VehicleType.Large;

        // Setup
        var service = GetParkingService();
        await service.ParkVehicleAsync(registrationNumber, vehicleType);

        // Test / Verify
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await service.ParkVehicleAsync(registrationNumber, vehicleType));
        Assert.That(ex, Is.Not.Null);
        Assert.That(ex.Message, Is.EqualTo($"The vehicle with registration number {registrationNumber} is already parked."));
    }

    [Test]
    public async Task ParkVehicleAsync_ShouldThrowWhenParkingIsFull()
    {
        // Setup
        var service = GetParkingService();
        await service.ParkVehicleAsync("DW 66600", VehicleType.Large);

        // Test / Verify
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await service.ParkVehicleAsync("KT 99900", VehicleType.Small));
        Assert.That(ex, Is.Not.Null);
        Assert.That(ex.Message, Is.EqualTo("Parking is full"));
    }

    [Test]
    public async Task ParkVehicleAsync_ShouldReturnParkingResponse()
    {
        // Data
        var registrationNumber = "DW 66600";
        var vehicleType = VehicleType.Large;

        // Setup
        var service = GetParkingService();

        // Test
        var response = await service.ParkVehicleAsync(registrationNumber, vehicleType);

        // Verify
        Assert.That(response, Is.Not.Null);
        Assert.That(response.RegistrationNumber, Is.EqualTo(registrationNumber));
        Assert.That(response.SpaceNumber, Is.EqualTo(1));
        Assert.That(response.ParkedAt, Is.Not.EqualTo(default(DateTime)));
    }

    [Test]
    public async Task GetCarparkStatus_ShouldReturnStatus()
    {
        // Setup
        var service = GetParkingService();

        // Test
        var response = await service.GetCarparkStatusAsync();

        // Verify
        Assert.That(response, Is.Not.Null);
        Assert.That(response.TotalSpaces, Is.EqualTo(1));
        Assert.That(response.OccupiedSpaces, Is.EqualTo(0));
        Assert.That(response.AvailableSpaces, Is.EqualTo(1));
    }

    [Test]
    public async Task ExitVehicleAsync_ShouldThrowIfCarNotFound()
    {
        // Data
        var registrationNumber = "DW 66600";

        // Setup
        var service = GetParkingService();

        // Test / Verify
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await service.ExitVehicleAsync(registrationNumber));
        Assert.That(ex, Is.Not.Null);
        Assert.That(ex.Message, Is.EqualTo($"No active parking record found for vehicle with registration number {registrationNumber}."));
    }

    [Test]
    public async Task ExitVehicleAsync_ShouldThrowIfThereAreNoActiveParkingRecords()
    {
        // Data
        var registrationNumber = "DW 66600";

        // Setup
        var service = GetParkingService();
        await service.ParkVehicleAsync(registrationNumber, VehicleType.Large);
        await service.ExitVehicleAsync(registrationNumber);

        // Test / Verify
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await service.ExitVehicleAsync(registrationNumber));
        Assert.That(ex, Is.Not.Null);
        Assert.That(ex.Message, Is.EqualTo($"No active parking record found for vehicle with registration number {registrationNumber}."));
    }

    [Test]
    public async Task ExitVehicleAsync_ShouldReturnExitResponse()
    {
        // Data
        var registrationNumber = "DW 66600";
        var vehicleType = VehicleType.Large;

        // Setup
        var service = GetParkingService();
        await service.ParkVehicleAsync(registrationNumber, vehicleType);

        // Test
        var response = await service.ExitVehicleAsync(registrationNumber);

        // Verify
        Assert.That(response, Is.Not.Null);
        Assert.That(response.RegistrationNumber, Is.EqualTo(registrationNumber));
        Assert.That(response.ParkedAt, Is.Not.EqualTo(default(DateTime)));
        Assert.That(response.ExitedAt, Is.Not.EqualTo(default(DateTime)));
        Assert.That(response.ParkingFee, Is.GreaterThan(0));
    }

    [Test]
    public async Task ExitVehicleAsync_ShouldCalculateParkingFee()
    {
        // Data
        var registrationNumber = "DW 66600";
        var vehicleType = VehicleType.Large;

        // Setup
        var service = GetParkingService();
        await service.ParkVehicleAsync(registrationNumber, vehicleType);

        // Test
        var response = await service.ExitVehicleAsync(registrationNumber);

        // Verify
        Assert.That(response, Is.Not.Null);
        Assert.That(response.ParkingFee, Is.EqualTo(0.60).Within(0.1));
    }

    private IParkingService GetParkingService() => new ParkingService(_mockConfiguration.Object, _context);


}
