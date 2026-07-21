using NUnit.Framework;
using Reqnroll;
using System.Net;
using TestAutomationShowcase.Core.ApiClients;
using TestAutomationShowcase.Core.Models;
using TestAutomationShowcase.Core.Models.Helpers;
using TestAutomationShowcase.GherkinTests.Helpers;

namespace TestAutomationShowcase.GherkinTests.StepDefinitions;

[Binding]
public class BookingSteps
{
    private readonly ScenarioContext _scenarioContext;
    private readonly BookingClient _client;

    public const string CurrentStatusCode = "current_status_code";
    public const string CurrentRawContent = "current_raw_content";
    public const string CurrentBookingKey = "current_booking_key";
    public const string CurrentBooking = "current_booking";
    public const string Bookings = "bookings";
    public const string ExpectedBooking = "expected_booking";

    public BookingSteps(ScenarioContext scenarioContext, BookingClient client)
    {
        _scenarioContext = scenarioContext;
        _client = client;
    }

    /// <summary>
    /// Verifies that the API client instance is initialized and not null.
    /// </summary>
    [Given("the API client is initialized")]
    public void GivenApiClientIsInitialized()
    {
        Assert.That(_client, Is.Not.Null);
    }

    /// <summary>
    /// Requests all bookings from the API and stores the result in the scenario context for later use.
    /// </summary>
    [When("the user requests all bookings")]
    public async Task WhenUserRequestsAllBookings()
    {
        var bookings = await _client.GetAllBookingsAsync();

        _scenarioContext.Set(bookings.Value, Bookings);
    }

    /// <summary>
    /// Retrieves a random booking from the scenario context, fetches its details, and stores relevant information in the scenario context.
    /// </summary>
    [When("the user retrieves a random booking")]
    public async Task WhenUserRetrievesARandomBooking()
    {
        var bookings = _scenarioContext.Get<List<BookingSummary>>(Bookings);

        var randomId = bookings[Random.Shared.Next(bookings.Count)].bookingid;

        var booking = await _client.GetBookingAsync(randomId);

        _scenarioContext.Set(randomId, CurrentBookingKey);
        _scenarioContext.Set(booking.Value, CurrentBooking);
        _scenarioContext.Set(booking.StatusCode, CurrentStatusCode);
        _scenarioContext.Set(booking.RawContent, CurrentRawContent);
    }

    /// <summary>
    /// Retrieves a booking by its ID and stores the booking details, status code, and raw response content in the scenario context.
    /// </summary>
    /// <param name="bookingId">The ID of the booking to retrieve.</param>
    [When("the user requests a booking with ID {int}")]
    public async Task WhenUserRequestsBookingWithId(int bookingId)
    {
        var response = await _client.GetBookingAsync(bookingId);

        _scenarioContext.Set(bookingId, CurrentBookingKey);
        _scenarioContext.Set(response.Value, CurrentBooking);
        _scenarioContext.Set(response.StatusCode, CurrentStatusCode);
        _scenarioContext.Set(response.RawContent, CurrentRawContent);
    }

    /// <summary>
    /// Creates a new booking using the details provided in the Gherkin table and stores the created booking's ID, details, status code, and raw response content in the scenario context.
    /// </summary>
    /// <param name="table">Gherkin table with booking details.</param>
    /// <example>
    /// When the user creates a booking with the following details:
	///	| Field            | Value      |
	///	| First name       | John       |
	///	| Last name        | Doe        |
	///	| Total price      |        250 |
	///	| Deposit paid     | true       |
	///	| Check-in date    | 2026-08-01 |
	///	| Check-out date   | 2026-08-10 |
	///	| Additional needs | Breakfast  |
    /// </example>
    [When("the user creates a booking with the following details:")]
    public async Task WhenUserCreatesBookingWithDetails(Table table)
    {
        var booking = table.CreateBooking();
        _scenarioContext.Set(booking, ExpectedBooking);

        var createdBooking = await _client.CreateBookingAsync(booking);
        
        _scenarioContext.Set(createdBooking.Value.bookingid, CurrentBookingKey);
        _scenarioContext.Set(createdBooking.Value.booking, CurrentBooking);
        _scenarioContext.Set(createdBooking.StatusCode, CurrentStatusCode);
        _scenarioContext.Set(createdBooking.RawContent, CurrentRawContent);
    }

    /// <summary>
    /// Creates a test booking and stores relevant information in the scenario context.
    /// </summary>
    [Given("the user creates a booking")]
    public async Task GivenUserCreatesBooking()
    {
        var createdBooking = await _client.CreateTestBookingAsync();

        _scenarioContext.Set(createdBooking.Value.bookingid, CurrentBookingKey);
        _scenarioContext.Set(createdBooking.Value.booking, CurrentBooking);
        _scenarioContext.Set(createdBooking.StatusCode, CurrentStatusCode);
        _scenarioContext.Set(createdBooking.RawContent, CurrentRawContent);
    }

    /// <summary>
    /// Updates the current booking with the specified details from the table.
    /// </summary>
    /// <param name="table">A table containing the updated booking details.</param>
    /// <example>
    /// When the user updates the booking with the following details:
	///	| Field            | Value         |
	///	| First name       | John_upd      |
	///	| Last name        | Doe_upd       |
	///	| Total price      |          1000 |
	///	| Deposit paid     | false         |
	///	| Check-in date    | 2032-08-01    |
	///	| Check-out date   | 2032-08-10    |
	///	| Additional needs | Breakfast_upd |
    /// </example>
    [When("the user updates the booking with the following details:")]
    public async Task WhenUserUpdatesBookingWithDetails(Table table)
    {
        var bookingId = _scenarioContext.Get<int>(CurrentBookingKey);
        var updatedBooking = table.CreateBooking();
        _scenarioContext.Set(updatedBooking, ExpectedBooking);

        var response = await _client.UpdateBookingAsync(bookingId, updatedBooking);
        
        _scenarioContext.Set(response.Value, CurrentBooking);
        _scenarioContext.Set(response.StatusCode, CurrentStatusCode);
        _scenarioContext.Set(response.RawContent, CurrentRawContent);
    }

    /// <summary>
    /// Partially updates the current booking with the specified details.
    /// </summary>
    /// <param name="table">The table containing the booking details to update.</param>
    /// <example>
    /// When the user partially updates the booking with the following details:
	///	| Field            | Value         |
	///	| First name       | John_upd      |
	///	| Last name        | Doe_upd       |
	///	| Additional needs | Breakfast_upd |
    /// </example>
    [When("the user partially updates the booking with the following details:")]
    public async Task WhenUserPartiallyUpdatesBookingWithDetails(Table table)
    {
        var bookingId = _scenarioContext.Get<int>(CurrentBookingKey);
        var updatedBooking = table.UpdateBooking();

        var createdBooking = _scenarioContext.Get<Booking>(CurrentBooking);
        createdBooking.ApplyUpdate(updatedBooking);
        _scenarioContext.Set(createdBooking, ExpectedBooking);

        var response = await _client.PartiallyUpdateBookingAsync(bookingId, updatedBooking);

        _scenarioContext.Set(response.Value, CurrentBooking);
        _scenarioContext.Set(response.StatusCode, CurrentStatusCode);
        _scenarioContext.Set(response.RawContent, CurrentRawContent);
    }

    /// <summary>
    /// Partially updates an existing booking without authentication and stores the response status code and raw content in the scenario context.
    /// </summary>
    [When("the user partially updates an existing booking without authentication")]
    public async Task WhenUserPartiallyUpdatesBookingWithoutAuthentication()
    {
        var bookingId = _scenarioContext.Get<int>(CurrentBookingKey);
        var updatedBooking = new UpdateBookingRequest()
        {
            firstname = "John_upd_skipAuth"
        };
        var skipAuth = true;

        var response = await _client.PartiallyUpdateBookingAsync(bookingId, updatedBooking, skipAuth);

        _scenarioContext.Set(bookingId, CurrentBookingKey);
        _scenarioContext.Set(response.StatusCode, CurrentStatusCode);
        _scenarioContext.Set(response.RawContent, CurrentRawContent);
    }

    /// <summary>
    /// Updates a booking with an invalid ID and stores the response status code and raw content in the scenario context.
    /// </summary>
    /// <param name="bookingId">Invalid booking ID</param>
    [When("the user updates booking with invalid ID {int}")]
    public async Task WhenUserUpdatesBookingWithInvalidId(int bookingId)
    {
        var updatedBooking = _client.StandardBooking;

        var response = await _client.UpdateBookingAsync(bookingId, updatedBooking);

        _scenarioContext.Set(bookingId, CurrentBookingKey);
        _scenarioContext.Set(response.StatusCode, CurrentStatusCode);
        _scenarioContext.Set(response.RawContent, CurrentRawContent);
    }

    /// <summary>
    /// Partially updates a booking with an invalid ID and stores the response status code and raw content in the scenario context.
    /// </summary>
    /// <param name="bookingId">Invalid booking ID</param>
    [When("the user partially updates booking with invalid ID {int}")]
    public async Task WhenUserPartiallyUpdatesBookingWithInvalidId(int bookingId)
    {
        var update = new UpdateBookingRequest()
        {
            firstname = "John_upd_invalidID"
        };

        var response = await _client.PartiallyUpdateBookingAsync(bookingId, update);

        _scenarioContext.Set(bookingId, CurrentBookingKey);
        _scenarioContext.Set(response.StatusCode, CurrentStatusCode);
        _scenarioContext.Set(response.RawContent, CurrentRawContent);
    }

    /// <summary>
    /// Updates an existing booking without authentication and stores the response status code and raw content in the scenario context.
    /// </summary>
    [When("the user updates an existing booking without authentication")]
    public async Task WhenUserUpdatesBookingWithoutAuthentication()
    {
        var bookingId = _scenarioContext.Get<int>(CurrentBookingKey);
        var updatedBooking = _client.StandardBooking;
        var skipAuth = true;

        var response = await _client.UpdateBookingAsync(bookingId, updatedBooking, skipAuth);

        _scenarioContext.Set(bookingId, CurrentBookingKey);
        _scenarioContext.Set(response.StatusCode, CurrentStatusCode);
        _scenarioContext.Set(response.RawContent, CurrentRawContent);
    }

    /// <summary>
    /// Retrieves the created, updated, or deleted booking using the booking ID from the scenario context and stores the response details.
    /// </summary>
    [When("the user retrieves the created/updated/deleted booking")]
    public async Task WhenUserRetrievesCreatedBooking()
    {
        var bookingId = _scenarioContext.Get<int>(CurrentBookingKey);
        var response = await _client.GetBookingAsync(bookingId);
        
        _scenarioContext.Set(response.Value, CurrentBooking);
        _scenarioContext.Set(response.StatusCode, CurrentStatusCode);
        _scenarioContext.Set(response.RawContent, CurrentRawContent);
    }

    /// <summary>
    /// Deletes the booking using the booking ID from the scenario context and stores the response status code and raw content in the scenario context.
    /// </summary>
    [When("the user deletes the booking")]
    public async Task WhenUserDeletesBooking()
    {
        var bookingId = _scenarioContext.Get<int>(CurrentBookingKey);
        var response = await _client.DeleteBookingAsync(bookingId);
        
        _scenarioContext.Set(response.StatusCode, CurrentStatusCode);
        _scenarioContext.Set(response.RawContent, CurrentRawContent);
    }

    /// <summary>
    /// Deletes the booking without authentication and stores the response status code and raw content in the scenario context.
    /// </summary>
    /// <returns></returns>
    [When("the user deletes the booking without authentication")]
    public async Task WhenUserDeletesBookingWithoutAuthentication()
    {
        var bookingId = _scenarioContext.Get<int>(CurrentBookingKey);
        var skipAuth = true;
        var response = await _client.DeleteBookingAsync(bookingId, skipAuth);

        _scenarioContext.Set(response.StatusCode, CurrentStatusCode);
        _scenarioContext.Set(response.RawContent, CurrentRawContent);
    }

    /// <summary>
    /// Verifies that the response status code matches the expected status code.
    /// </summary>
    /// <param name="expectedStatusCode">Expected status code</param>
    [Then("the response status code should be {int}")]
    public void ThenResponseStatusCodeShouldBe(int expectedStatusCode)
    {
        var responseCode = _scenarioContext.Get<HttpStatusCode>(CurrentStatusCode);

        Assert.That((int)responseCode, Is.EqualTo(expectedStatusCode));
    }

    /// <summary>
    /// Verifies that the response contains a non-empty list of bookings.
    /// </summary>
    [Then("the response should contain the list of bookings")]
    public void ThenResponseShouldNotBeEmpty()
    {
        var bookings = _scenarioContext.Get<List<BookingSummary>>(Bookings);

        Assert.That(bookings, Is.Not.Empty);
        Assert.That(bookings.Count, Is.Not.EqualTo(0));
    }

    /// <summary>
    /// Asserts that the response content matches the expected value.
    /// </summary>
    /// <param name="expectedContent">The expected content</param>
    [Then("the response content should be {string}")]
    public void ThenResponseContentShouldBe(string expectedContent)
    {
        var rawContent = _scenarioContext.Get<string>(CurrentRawContent);
        Assert.That(rawContent, Is.EqualTo(expectedContent));
    }

    /// <summary>
    /// Asserts that the booking contains a non-empty firstname.
    /// </summary>
    [Then("the booking should have a valid firstname")]
    public void ThenBookingShouldHaveValidFirstname()
    {
        var booking = _scenarioContext.Get<Booking>(CurrentBooking);

        Assert.That(booking.firstname, Is.Not.Empty);
    }

    /// <summary>
    /// Asserts that the booking contains total price greater than 0.
    /// </summary>
    [Then("the booking should have a valid total price")]
    public void ThenBookingShouldHaveValidTotalPrice()
    {
        var booking = _scenarioContext.Get<Booking>(CurrentBooking);

        Assert.That(booking.totalprice, Is.GreaterThan(0));
    }

    /// <summary>
    /// Verifies that the created, retrieved, or updated booking matches the expected booking details.
    /// </summary>
    /// <remarks>Checks first name, last name, total price, deposit paid, booking dates, and additional needs for accuracy.</remarks>
    [Then("the created/retrieved/updated booking should contain the provided details")]
    [Then("the updated booking should contain the provided details, the remaining data should be unchanged")]
    public void ThenCreatedBookingShouldContainProvidedDetails()
    {
        var currentBooking = _scenarioContext.Get<Booking>(CurrentBooking);
        var expectedBooking = _scenarioContext.Get<Booking>(ExpectedBooking);

        using(Assert.EnterMultipleScope())
        {
            Assert.That(currentBooking.firstname, Is.EqualTo(expectedBooking.firstname));
            Assert.That(currentBooking.lastname, Is.EqualTo(expectedBooking.lastname));
            Assert.That(currentBooking.totalprice, Is.EqualTo(expectedBooking.totalprice));
            Assert.That(currentBooking.depositpaid, Is.EqualTo(expectedBooking.depositpaid));
            Assert.That(currentBooking.bookingdates.checkin, Is.EqualTo(expectedBooking.bookingdates.checkin));
            Assert.That(currentBooking.bookingdates.checkout, Is.EqualTo(expectedBooking.bookingdates.checkout));
            Assert.That(currentBooking.additionalneeds, Is.EqualTo(expectedBooking.additionalneeds));
        }
    }
}
