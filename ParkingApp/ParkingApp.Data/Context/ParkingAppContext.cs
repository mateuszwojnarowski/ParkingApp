using Microsoft.EntityFrameworkCore;
using ParkingApp.Data.Models;

namespace ParkingApp.Data.Context;
public class ParkingAppContext(DbContextOptions<ParkingAppContext> options) : DbContext(options)
{
    public DbSet<ParkingRecord> ParkingRecords { get; set; }
}
