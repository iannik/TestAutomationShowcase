using Reqnroll;
using TestAutomationShowcase.Core.Models;

namespace TestAutomationShowcase.GherkinTests.Helpers;

/// <summary>
/// Provides extension methods for creating and updating booking information from a Gherkin Table.
/// </summary>
public static class BookingTableExtensions
{
    public static Booking CreateBooking(this Table table)
    {
        return new Booking
        {
            firstname = table.GetValue("First name"),
            lastname = table.GetValue("Last name"),
            totalprice = table.GetIntValue("Total price"),
            depositpaid = table.GetBoolValue("Deposit paid"),
            bookingdates = new BookingDates
            {
                checkin = table.GetValue("Check-in date"),
                checkout = table.GetValue("Check-out date")
            },
            additionalneeds = table.GetValue("Additional needs")
        };
    }

    private static string GetValue(this Table table, string field) => table.Rows.Single(x => x["Field"] == field)["Value"];

    private static int GetIntValue(this Table table, string field) => int.Parse(table.GetValue(field));

    private static bool GetBoolValue(this Table table, string field) => bool.Parse(table.GetValue(field));

    public static UpdateBookingRequest UpdateBooking(this Table table)
    {
        var request = new UpdateBookingRequest
        {
            firstname = table.TryGetValue("First name"),
            lastname = table.TryGetValue("Last name"),
            totalprice = table.TryGetIntValue("Total price"),
            depositpaid = table.TryGetBoolValue("Deposit paid"),
            additionalneeds = table.TryGetValue("Additional needs")
        };

        var checkin = table.TryGetValue("Check-in date");
        var checkout = table.TryGetValue("Check-out date");

        if (checkin is not null || checkout is not null)
        {
            request.bookingdates = new UpdateBookingDates
            {
                checkin = checkin,
                checkout = checkout
            };
        }

        return request;
    }

    private static string? TryGetValue(this Table table, string field)
    {
        return table.Rows.Any(x => x["Field"] == field) ? table.GetValue(field) : null;
    }

    private static int? TryGetIntValue(this Table table, string field)
    {
        var value = table.TryGetValue(field);
        return value is null ? null : int.Parse(value);
    }

    private static bool? TryGetBoolValue(this Table table, string field)
    {
        var value = table.TryGetValue(field);
        return value is null ? null : bool.Parse(value);
    }
}
