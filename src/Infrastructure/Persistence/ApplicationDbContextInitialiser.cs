﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace UrlShortenerService.Infrastructure.Persistence;

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;

    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            if (_context.Database.IsSqlServer())
            {
                await _context.Database.MigrateAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        // Default data
        // Seed, if necessary
        // await Task.CompletedTask;
        var result = await _context.Urls.FirstOrDefaultAsync(x => x.OriginalUrl == "https://www.google.com");

        if(result != null) return;

        _ = _context.Urls.Add(new() {
            OriginalUrl = "https://www.google.com"
        });
        _ = await _context.SaveChangesAsync();
    }
}
