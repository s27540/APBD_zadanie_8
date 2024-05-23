﻿using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;

namespace WebApplication2.Context;

public class TripContext : DbContext
{
    public TripContext(DbContextOptions<TripContext> options) : base(options)
    {
    }

    public DbSet<Client> Clients { get; set; }
    public DbSet<Client_Trip> Client_Trips { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<Country_Trip> Country_Trips { get; set; }
    public DbSet<Trip> Trips { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Client_Trip>()
            .HasKey(ct => new { ct.IdClient, ct.IdTrip });

        modelBuilder.Entity<Country_Trip>()
            .HasKey(ct => new { ct.IdCountry, ct.IdTrip });

        modelBuilder.Entity<Client_Trip>()
            .HasOne(ct => ct.Client)
            .WithMany(c => c.Client_Trips)
            .HasForeignKey(ct => ct.IdClient);

        modelBuilder.Entity<Client_Trip>()
            .HasOne(ct => ct.Trip)
            .WithMany(t => t.Client_Trips)
            .HasForeignKey(ct => ct.IdTrip);

        modelBuilder.Entity<Country_Trip>()
            .HasOne(ct => ct.Country)
            .WithMany(c => c.Country_Trips)
            .HasForeignKey(ct => ct.IdCountry);

        modelBuilder.Entity<Country_Trip>()
            .HasOne(ct => ct.Trip)
            .WithMany(t => t.Country_Trips)
            .HasForeignKey(ct => ct.IdTrip);
    }
}