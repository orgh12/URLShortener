using Microsoft.EntityFrameworkCore;
using UrlShortenerApi.Models;

namespace UrlShortenerApi.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) 
        : base(options)
    {
        
    }
    
    public DbSet<UrlMapping> UrlMappings { get; set; }
}