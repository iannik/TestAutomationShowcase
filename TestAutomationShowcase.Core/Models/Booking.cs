using System.Net;

namespace TestAutomationShowcase.Core.Models
{
    public class ApiResponse<T>(HttpStatusCode statusCode, T? value, string rawContent)
    {
        public HttpStatusCode StatusCode { get; set; } = statusCode;
        public T? Value { get; set; } = value;
        public string RawContent { get; set; } = rawContent;
    }

    public class BookingSummary
    {
        public int bookingid { get; set; }
    }

    public class BookingDates
    {
        public string checkin { get; set; }
        public string checkout { get; set; }
    }

    public class Booking
    {
        public string firstname { get; set; }
        public string lastname { get; set; }
        public int totalprice { get; set; }
        public bool depositpaid { get; set; }
        public BookingDates bookingdates { get; set; }
        public string? additionalneeds { get; set; }
    }

    public class CreateBookingResponse
    {
        public int bookingid { get; set; }
        public Booking booking { get; set; }
    }

    public class UpdateBookingRequest
    {
        public string? firstname { get; set; }
        public string? lastname { get; set; }
        public int? totalprice { get; set; }
        public bool? depositpaid { get; set; }
        public UpdateBookingDates? bookingdates { get; set; }
        public string? additionalneeds { get; set; }
    }

    public class UpdateBookingDates
    {
        public string? checkin { get; set; }
        public string? checkout { get; set; }
    }

}
