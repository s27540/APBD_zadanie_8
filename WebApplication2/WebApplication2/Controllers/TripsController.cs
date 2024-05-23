using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Context;
using WebApplication2.DTO;
using WebApplication2.Models;

namespace WebApplication2.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TripsController : ControllerBase
{
    private readonly TripContext _context;

    public TripsController(TripContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TripDto>>> GetTrips()
    {
        var trips = await _context.Trips
            .OrderByDescending(t => t.DateFrom)
            .Select(t => new TripDto
            {
                Name = t.Name,
                Description = t.Description,
                DateFrom = t.DateFrom,
                DateTo = t.DateTo,
                MaxPeople = t.MaxPeople,
                Countries = t.Country_Trips.Select(ct => new CountryDto { Name = ct.Country.Name }).ToList(),
                Clients = t.Client_Trips.Select(ct => new ClientDto { FirstName = ct.Client.FirstName, LastName = ct.Client.LastName }).ToList()
            }).ToListAsync();

        return Ok(trips);
    }

    [HttpPost("{idTrip}/clients")]
    public async Task<IActionResult> AddClientToTrip(int idTrip, AddClientToTripDto dto)
    {
        var trip = await _context.Trips.FindAsync(idTrip);
        if (trip == null)
        {
            return NotFound(new { Message = "Trip not found" });
        }

        var client = await _context.Clients.SingleOrDefaultAsync(c => c.Pesel == dto.Pesel);
        if (client == null)
        {
            client = new Client
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Telephone = dto.Telephone,
                Pesel = dto.Pesel
            };
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
        }

        var existingClientTrip = await _context.Client_Trips.FindAsync(client.IdClient, idTrip);
        if (existingClientTrip != null)
        {
            return BadRequest(new { Message = "Client is already registered for this trip" });
        }

        var clientTrip = new Client_Trip
        {
            IdClient = client.IdClient,
            IdTrip = idTrip,
            RegisteredAt = DateTime.UtcNow,
            PaymentDate = dto.PaymentDate
        };

        _context.Client_Trips.Add(clientTrip);
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpDelete("{idClient}")]
    public async Task<IActionResult> DeleteClient(int idClient)
    {
        var client = await _context.Clients
            .Include(c => c.Client_Trips)
            .SingleOrDefaultAsync(c => c.IdClient == idClient);

        if (client == null)
        {
            return NotFound(new { Message = "Client not found" });
        }

        if (client.Client_Trips.Any())
        {
            return BadRequest(new { Message = "Client has trips and cannot be deleted" });
        }

        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}