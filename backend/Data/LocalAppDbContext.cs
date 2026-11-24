using Microsoft.EntityFrameworkCore;

namespace API.Data;

// Separate migration track for creating a clean local database.
public sealed class LocalAppDbContext : AppDbContext
{
    public LocalAppDbContext(DbContextOptions<LocalAppDbContext> options)
        : base(options)
    {
    }
}
