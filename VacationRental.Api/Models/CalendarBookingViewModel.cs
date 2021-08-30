namespace VacationRental.Api.Models
{
    public class CalendarBookingViewModel
    {
        public int Id { get; set; }

        /// <summary>
        /// If the calendar model contains `Booking` with `Unit` 2 then it means that `Booking` is booked at `Unit` number 2. 
        /// **It does NOT mean that the booking occupies two units**.
        /// </summary>
        public int Unit { get; set; }
    }
}
