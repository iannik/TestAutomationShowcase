using TestAutomationShowcase.Core.Models;

namespace TestAutomationShowcase.Core.ApiClients;

/// <summary>
/// Provides methods for managing bookings through the API, including retrieval, creation, update, and deletion operations.
/// </summary>
public class BookingClient : BaseApiClient
{
    public readonly Booking StandardBooking = new()
    {
        firstname = "John",
        lastname = "Smith",
        totalprice = 240,
        depositpaid = true,
        bookingdates = new BookingDates()
        {
            checkin = "2026-09-05",
            checkout = "2026-09-10"
        },
        additionalneeds = "Breakfast"
    };
    public BookingClient(HttpClient http) : base(http) { }

    public Task<ApiResponse<List<BookingSummary>>> GetAllBookingsAsync() => SendAsync<List<BookingSummary>>(HttpMethod.Get, "/booking");

    public Task<ApiResponse<Booking>> GetBookingAsync(int id) => SendAsync<Booking>(HttpMethod.Get, $"/booking/{id}");

    public Task<ApiResponse<CreateBookingResponse>> CreateBookingAsync(Booking request) => SendAsync<CreateBookingResponse>(HttpMethod.Post, "/booking", request);
    public Task<ApiResponse<CreateBookingResponse>> CreateTestBookingAsync() => SendAsync<CreateBookingResponse>(HttpMethod.Post, "/booking", StandardBooking);
    public Task<ApiResponse<Booking>> UpdateBookingAsync(int id, Booking request, bool skipAuth = false) => SendAsync<Booking>(HttpMethod.Put, $"/booking/{id}", request, skipAuth);
    public Task<ApiResponse<Booking>> PartiallyUpdateBookingAsync(int id, UpdateBookingRequest request, bool skipAuth = false) => SendAsync<Booking>(HttpMethod.Patch, $"/booking/{id}", request, skipAuth);

    public Task<ApiResponse<object>> DeleteBookingAsync(int id, bool skipAuth = false) => SendAsync<object>(HttpMethod.Delete, $"/booking/{id}", null, skipAuth);
}
