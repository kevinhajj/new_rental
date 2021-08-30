﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Helpers;
using VacationRental.Api.Models;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/calendar")]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        private readonly IDictionary<int, RentalViewModel> _rentals;
        private readonly IDictionary<int, BookingViewModel> _bookings;

        public CalendarController(
            IDictionary<int, RentalViewModel> rentals,
            IDictionary<int, BookingViewModel> bookings)
        {
            _rentals = rentals;
            _bookings = bookings;
        }

        [HttpGet]
        public CalendarViewModel Get(int rentalId, DateTime start, int nights)
        {
            if (nights < 0)
                throw new ApplicationException("Nights must be positive");
            if (!_rentals.ContainsKey(rentalId))
                throw new ApplicationException("Rental not found");

            var rental = _rentals.Values.Where(r => r.Id == rentalId).FirstOrDefault();
            if (rental == null)
            {
                throw new ApplicationException("Rental not found");
            }

            var result = new CalendarViewModel
            {
                RentalId = rentalId,
                Dates = new List<CalendarDateViewModel>()
            };
            for (var i = 0; i < nights; i++)
            {
                var date = new CalendarDateViewModel
                {
                    Date = start.Date.AddDays(i),
                    Bookings = new List<CalendarBookingViewModel>()
                };

                foreach (var booking in _bookings.Values)
                {
                    if (booking.RentalId == rentalId)
                    {
                        if (booking.Start <= date.Date && booking.Start.AddDays(booking.Nights) > date.Date)
                        {
                            date.Bookings.Add(new CalendarBookingViewModel
                            {
                                Id = booking.Id,
                                Unit = booking.Unit
                            });
                        }
                        if (rental.PreparationTimeInDays > 0
                            && VacationRentalHelper.IsDateInBetween(date.Date, booking.Start.AddDays(booking.Nights), booking.Start.AddDays(booking.Nights - 1).AddDays(rental.PreparationTimeInDays)))
                        {
                            date.PreparationTimes.Add(new CalendarPreparationTimeViewModel { Unit = booking.Unit });
                        }
                    }
                }

                result.Dates.Add(date);
            }

            return result;
        }
    }
}
