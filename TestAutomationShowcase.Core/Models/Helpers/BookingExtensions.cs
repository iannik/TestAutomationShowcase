namespace TestAutomationShowcase.Core.Models.Helpers;

/// <summary>
/// Provides extension methods for updating Booking instances.
/// </summary>
public static class BookingExtensions
{
    public static void ApplyUpdate(this Booking booking, UpdateBookingRequest update)
    {
        if (update.firstname is not null)
            booking.firstname = update.firstname;

        if (update.lastname is not null)
            booking.lastname = update.lastname;

        if (update.totalprice.HasValue)
            booking.totalprice = update.totalprice.Value;

        if (update.depositpaid.HasValue)
            booking.depositpaid = update.depositpaid.Value;

        if (update.bookingdates is not null)
        {
            if (update.bookingdates.checkin is not null)
                booking.bookingdates.checkin = update.bookingdates.checkin;
            if (update.bookingdates.checkout is not null)
                booking.bookingdates.checkout = update.bookingdates.checkout;
        }

        if (update.additionalneeds is not null)
            booking.additionalneeds = update.additionalneeds;
    }
}
