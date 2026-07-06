using Microsoft.EntityFrameworkCore;
using EazyAssessment.Server.Models;

namespace EazyAssessment.Server.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
}
