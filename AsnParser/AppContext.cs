using AsnParser.Models;
using Microsoft.EntityFrameworkCore;

namespace AsnParser;

public class AppContext : DbContext
{
    private readonly string _connectionString;
    public AppContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_connectionString);
    }

    public DbSet<Box> Boxes { get; set; }
    public DbSet<Content> Contents { get; set; }
}