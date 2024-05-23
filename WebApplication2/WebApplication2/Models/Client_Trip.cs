using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models;

public class Client_Trip
{
    public int IdClient { get; set; }
    public int IdTrip { get; set; }

    [Required]
    public DateTime RegisteredAt { get; set; }

    public DateTime? PaymentDate { get; set; }

    public Client Client { get; set; }
    public Trip Trip { get; set; }
}