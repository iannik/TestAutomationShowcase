using Allure.NUnit;
using Allure.NUnit.Attributes;
using System.Net;
using TestAutomationShowcase.Core.ApiClients;
using TestAutomationShowcase.Core.Models;
using TestAutomationShowcase.Core.Models.Helpers;
using TestAutomationShowcase.Tests.Helpers;

namespace TestAutomationShowcase.Tests.Tests.API;

/// <summary>
/// Contains tests for the Restful Booker API, covering operations such as retrieving, creating, updating, and deleting bookings.
/// </summary>
/// <remarks>Validates API behavior for both successful and error scenarios, including authentication checks.</remarks>
[TestFixture]
[Parallelizable(ParallelScope.Self)]
[Category("API")]
[AllureNUnit]
[AllureFeature("Restful Booker API")]
[AllureSuite("NUnit API Tests")]
public class BookingApiTests
{
    private BookingClient _client;

    [SetUp]
    public void SetUp() => _client = TestServices.Resolve<BookingClient>();

    [Test]
    public async Task GetAllBookings_Returns200_WithResults()
    {
        var bookings = await _client.GetAllBookingsAsync();

        Assert.That(bookings.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(bookings.Value, Is.Not.Empty);
    }

    [Test]
    public async Task GetBookingById_ReturnsExpectedBooking()
    {
        var bookings = await _client.GetAllBookingsAsync() ?? throw new NullReferenceException("The bookings list is empty.");
        
        var randomId = bookings.Value[Random.Shared.Next(bookings.Value.Count)].bookingid;

        var booking = await _client.GetBookingAsync(randomId) ?? throw new NullReferenceException($"Booking with an ID '{randomId}' is not found.");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(booking.Value, Is.Not.Null);
            Assert.That(booking.Value.firstname, Is.Not.Empty);
            Assert.That(booking.Value.totalprice, Is.GreaterThan(0));
        }
    }

    [Test]
    public async Task GetBooking_WithInvalidId_ShouldReturnNotFound()
    {
        const int invalidId = 99999999;
        var response = await _client.GetBookingAsync(invalidId);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task CreateBooking_ShouldCreateBookingSuccessfully()
    {
        var request = new Booking
        {
            firstname = "John",
            lastname = "Doe",
            totalprice = 250,
            depositpaid = true,
            bookingdates = new BookingDates
            {
                checkin = "2026-08-01",
                checkout = "2026-08-10"
            },
            additionalneeds = "Breakfast"
        };

        var createdBooking = await _client.CreateBookingAsync(request);

        Assert.That(createdBooking.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        AssertBookingMatches(createdBooking.Value.booking, request);
    }

    [Test]
    public async Task CreateBooking_ThenRetrieveBooking_ShouldReturnSameData()
    {
        var request = new Booking
        {
            firstname = "Alice",
            lastname = "Smith",
            totalprice = 500,
            depositpaid = false,
            bookingdates = new BookingDates
            {
                checkin = "2026-09-01",
                checkout = "2026-09-05"
            },
            additionalneeds = "Lunch"
        };

        var created = await _client.CreateBookingAsync(request);

        var retrieved = await _client.GetBookingAsync(created.Value.bookingid);

        Assert.That(retrieved.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        AssertBookingMatches(retrieved.Value, request);
    }

    [Test]
    public async Task UpdateExistingBooking_ReturnsUpdatedBooking()
    {
        var booking = await _client.CreateTestBookingAsync();

        var update = new Booking
        {
            firstname = "John_upd",
            lastname = "Doe_upd",
            totalprice = 1000,
            depositpaid = false,
            bookingdates = new BookingDates
            {
                checkin = "2032-08-01",
                checkout = "2032-08-10"
            },
            additionalneeds = "Breakfast_upd"
        };

        var response = await _client.UpdateBookingAsync(booking.Value.bookingid, update);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        AssertBookingMatches(response.Value, update);
    }

    [Test]
    public async Task UpdateExistingBooking_ThenRetrieveBooking_ShouldReturnSameData()
    {
        var booking = await _client.CreateTestBookingAsync();

        var update = new Booking
        {
            firstname = "John_upd",
            lastname = "Doe_upd",
            totalprice = 1000,
            depositpaid = false,
            bookingdates = new BookingDates
            {
                checkin = "2032-08-01",
                checkout = "2032-08-10"
            },
            additionalneeds = "Breakfast_upd"
        };

        var response = await _client.UpdateBookingAsync(booking.Value.bookingid, update);

        var retrieved = await _client.GetBookingAsync(booking.Value.bookingid);

        Assert.That(retrieved.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        AssertBookingMatches(retrieved.Value, update);
    }

    [Test]
    public async Task UpdateBooking_WithInvalidId_ShouldReturnMethodNotAllowed()
    {
        const int invalidId = 99999999;
        var update = _client.StandardBooking;
        var response = await _client.UpdateBookingAsync(invalidId, update);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.MethodNotAllowed));
    }

    [Test]
    public async Task UpdateBooking_WithoutAuthentication_ShouldReturnForbidden()
    {
        var booking = await _client.CreateTestBookingAsync();
        
        var update = _client.StandardBooking;
        bool skipAuth = true;

        var response = await _client.UpdateBookingAsync(booking.Value.bookingid, update, skipAuth);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    [Test]
    public async Task PartiallyUpdateExistingBooking_ReturnsUpdatedBooking()
    {
        var booking = await _client.CreateTestBookingAsync();
        var createdBooking = booking.Value.booking;

        var update = new UpdateBookingRequest
        {
            firstname = "John_upd",
            lastname = "Doe_upd",
            additionalneeds = "Breakfast_upd"
        };
        
        createdBooking.ApplyUpdate(update);

        var response = await _client.PartiallyUpdateBookingAsync(booking.Value.bookingid, update);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        AssertBookingMatches(response.Value, createdBooking);
    }

    [Test]
    public async Task PartiallyUpdateExistingBooking_ThenRetrieveBooking_ShouldReturnSameData()
    {
        var booking = await _client.CreateTestBookingAsync();
        var createdBooking = booking.Value.booking;

        var update = new UpdateBookingRequest
        {
            firstname = "John_upd",
            bookingdates = new UpdateBookingDates()
            {
                checkin = "2027-01-01",
                checkout = "2027-01-10"
            }
        };

        createdBooking.ApplyUpdate(update);
        var response = await _client.PartiallyUpdateBookingAsync(booking.Value.bookingid, update);

        var retrieved = await _client.GetBookingAsync(booking.Value.bookingid);

        Assert.That(retrieved.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        AssertBookingMatches(retrieved.Value, createdBooking);
    }

    [Test]
    public async Task PartiallyUpdateBooking_WithoutAuthentication_ShouldReturnForbidden()
    {
        var booking = await _client.CreateTestBookingAsync();

        var update = new UpdateBookingRequest()
        {
            firstname = "John_upd_skipAuth"
        };
        bool skipAuth = true;

        var response = await _client.PartiallyUpdateBookingAsync(booking.Value.bookingid, update, skipAuth);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    [Test]
    public async Task PartiallyUpdateBooking_WithInvalidId_ShouldReturnMethodNotAllowed()
    {
        const int invalidId = 99999999;

        var update = new UpdateBookingRequest()
        {
            firstname = "John_upd_invalidID"
        };

        var response = await _client.PartiallyUpdateBookingAsync(invalidId, update);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.MethodNotAllowed));
    }

    [Test]
    public async Task DeleteBooking_ShouldReturnCreatedStatusCode()
    {
        var booking = await _client.CreateTestBookingAsync();

        var response = await _client.DeleteBookingAsync(booking.Value.bookingid);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    }

    [Test]
    public async Task DeletedBooking_CannotBeRetrieved()
    {
        var booking = await _client.CreateTestBookingAsync();
        var response = await _client.DeleteBookingAsync(booking.Value.bookingid);

        var retrieved = await _client.GetBookingAsync(booking.Value.bookingid);

        Assert.That(retrieved.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task DeleteBooking_WithoutAuthentication_ShouldReturnForbidden()
    {
        var booking = await _client.CreateTestBookingAsync();
        bool skipAuth = true;

        var response = await _client.DeleteBookingAsync(booking.Value.bookingid, skipAuth);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }


    private static void AssertBookingMatches(Booking actual, Booking expected)
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.firstname, Is.EqualTo(expected.firstname));
            Assert.That(actual.lastname, Is.EqualTo(expected.lastname));
            Assert.That(actual.totalprice, Is.EqualTo(expected.totalprice));
            Assert.That(actual.depositpaid, Is.EqualTo(expected.depositpaid));
            Assert.That(actual.bookingdates.checkin, Is.EqualTo(expected.bookingdates.checkin));
            Assert.That(actual.bookingdates.checkout, Is.EqualTo(expected.bookingdates.checkout));
            Assert.That(actual.additionalneeds, Is.EqualTo(expected.additionalneeds));
        }
    }
}
