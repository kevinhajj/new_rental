using System;
using System.Collections.Generic;

namespace VacationRental.Api.Models
{
    public class CalendarDateViewModel
    {
        #region Constructor

        public CalendarDateViewModel()
        {
            Bookings = new List<CalendarBookingViewModel>();
            PreparationTimes = new List<CalendarPreparationTimeViewModel>();
        }

        #endregion Constructor

        #region Properties

        public DateTime Date { get; set; }
        public List<CalendarBookingViewModel> Bookings { get; set; }
        public List<CalendarPreparationTimeViewModel> PreparationTimes { get; set; }

        #endregion Properties
    }
}
