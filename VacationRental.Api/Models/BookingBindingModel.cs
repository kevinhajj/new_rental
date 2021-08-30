using System;

namespace VacationRental.Api.Models
{
    public class BookingBindingModel
    {
        public int RentalId { get; set; }

        public DateTime Start
        {
            get => _startIgnoreTime;
            set => _startIgnoreTime = value.Date;
        }

        private DateTime _startIgnoreTime;
        public int Nights { get; set; }

        /// <summary>
        /// This property is a number that references one of the available units in the Rental. 
        /// For example, if the Rental has a total of 3 units, the Unit property can accept the values 1, 2,  or 3.
        /// </summary>
        public int Unit { get; set; }
    }
}
