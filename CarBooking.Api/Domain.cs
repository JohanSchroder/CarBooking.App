using System.Text.Json.Serialization;

namespace CarBooking.Api
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CarCategory
    {
        Small,
        Medium,
        Large,
        SUV,
        Minivan
    }

    public class Booking
    {
        public int BookingNumber { get; set; }
        public required string RegistrationNumber { get; set; }
        public required string CustomerSsn { get; set; }
        public CarCategory CarCategory { get; set; }
        public DateTime PickupDate { get; set; }
        public int PickupKm { get; set; }
        public DateTime? ReturnDate { get; set; }
        public int? ReturnKm { get; set; }
    }
    public class PriceSettings
    {
        public decimal BaseDayRental { get; set; }
        public decimal BaseKmPrice { get; set; }
    }
}
