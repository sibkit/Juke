using Microsoft.EntityFrameworkCore;

namespace Flux.Pcb.Data;

public class PcbOrdersDbContext : DbContext
{
    public PcbOrdersDbContext(DbContextOptions<PcbOrdersDbContext> options) 
        : base(options) 
    { 
    }

    public DbSet<PcbOrder> Orders { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Переопределяем имя таблицы, чтобы в PostgreSQL всё было красиво
        modelBuilder.Entity<PcbOrder>().ToTable("pcb_orders");
    }
}