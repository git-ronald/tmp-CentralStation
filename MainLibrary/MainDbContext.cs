using CentralStation.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace CentralStation.Data;

public class MainDbContext : DbContext
{
    // TODO: MainRepository with automatic SaveChangesAsync after add, update or delete on IDisposeAsync

    public DbSet<Peer> Peers => Set<Peer>();
    public DbSet<PeerNode> PeerNodes => Set<PeerNode>();
    public DbSet<PeerConnection> PeerConnections => Set<PeerConnection>();
    public DbSet<NavPage> NavPages => Set<NavPage>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "", "data.db");
        optionsBuilder.UseSqlite($"Data Source={path}");
    }
}
