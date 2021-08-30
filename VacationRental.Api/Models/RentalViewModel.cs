namespace VacationRental.Api.Models
{
    public class RentalViewModel
    {
        public int Id { get; set; }
        public int Units { get; set; }

        /// <summary>
        /// PreparationTimeInDays: we should block additional **X days** after each booking
        /// </summary>
        public int PreparationTimeInDays { get; set; }
    }
}
