using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using ParkingApp.Contracts.Responses;
using ParkingApp.Data.Context;
using ParkingApp.Data.Enums;
using ParkingApp.Data.Models;

namespace ParkingApp.Api.Services;

public class ParkingService(IConfiguration configuration, ParkingAppContext context) : IParkingService
{
    private readonly ParkingAppContext _context = context;
    private readonly int _capacity = configuration.GetValue<int>("CarParkCapacity");

    public async Task<ParkingResponse> ParkVehicleAsync(string registrationNumber, VehicleType vehicleType)
    {
        var alreadyParked = await _context.ParkingRecords.AnyAsync(x =>
            string.Equals(x.RegistrationNumber, registrationNumber, StringComparison.OrdinalIgnoreCase) &&
            x.ExitedAt == null);

        if (alreadyParked)
        {
            throw new InvalidOperationException(
                $"The vehicle with registration number {registrationNumber} is already parked.");
        }

        var occupiedSpots = await _context.ParkingRecords.Where(x => x.ExitedAt == null).Select(x => x.SpaceNumber)
            .ToListAsync();

        if (occupiedSpots.Count >= _capacity)
        {
            throw new InvalidOperationException("Parking is full");
        }

        var availableSpot = Enumerable.Range(1, _capacity).Except(occupiedSpots).First();

        var parkingRecord = new ParkingRecord
        {
            RegistrationNumber = registrationNumber,
            VehicleType = vehicleType,
            ParkedAt = DateTime.UtcNow,
            SpaceNumber = availableSpot
        };

        _context.ParkingRecords.Add(parkingRecord);
        await _context.SaveChangesAsync();

        return new ParkingResponse
        {
            RegistrationNumber = parkingRecord.RegistrationNumber,
            ParkedAt = parkingRecord.ParkedAt,
            SpaceNumber = parkingRecord.SpaceNumber
        };
    }

    public async Task<StatusResponse> GetCarparkStatusAsync()
    {
        var occupiedSpots = await _context.ParkingRecords.Where(x => x.ExitedAt == null).ToListAsync();
        return new StatusResponse
        {
            TotalSpaces = _capacity,
            OccupiedSpaces = occupiedSpots.Count,
            AvailableSpaces = _capacity - occupiedSpots.Count
        };  
    }

    public async Task<ExitResponse> ExitVehicleAsync(string registrationNumber)
    {
        var parkingRecords = _context.ParkingRecords.Where(x =>
            string.Equals(x.RegistrationNumber, registrationNumber, StringComparison.OrdinalIgnoreCase));

        var activeRecord = await parkingRecords.FirstOrDefaultAsync(x => x.ExitedAt == null);

        if (activeRecord == null)
        {
            throw new InvalidOperationException(
                $"No active parking record found for vehicle with registration number {registrationNumber}.");
        }

        var exitedAt = DateTime.UtcNow;

        var totalMinutes = Math.Ceiling((exitedAt - activeRecord.ParkedAt).TotalMinutes);

        var parkingRate = activeRecord.VehicleType switch
        {
            VehicleType.Small => 0.10,
            VehicleType.Medium => 0.20,
            VehicleType.Large => 0.40,
            _ => throw new ArgumentOutOfRangeException(nameof(activeRecord.VehicleType), "Unknown vehicle type")
        };

        var totalCharge = totalMinutes * parkingRate + ((totalMinutes / 5) * 1.00);

        activeRecord.ExitedAt = exitedAt;
        activeRecord.SpaceNumber = 0;
        activeRecord.ParkingFee = totalCharge;
        
        await _context.SaveChangesAsync();

        return new ExitResponse
        {
            RegistrationNumber = activeRecord.RegistrationNumber,
            ParkedAt = activeRecord.ParkedAt,
            ExitedAt = exitedAt,
            ParkingFee = totalCharge
        };
    }
}
