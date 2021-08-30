using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Helpers;
using VacationRental.Api.Models;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/bookings")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IDictionary<int, RentalViewModel> _rentals;
        private readonly IDictionary<int, BookingViewModel> _bookings;

        public BookingsController(
            IDictionary<int, RentalViewModel> rentals,
            IDictionary<int, BookingViewModel> bookings)
        {
            _rentals = rentals;
            _bookings = bookings;
        }

        [HttpGet]
        [Route("{bookingId:int}")]
        public BookingViewModel Get(int bookingId)
        {
            if (!_bookings.ContainsKey(bookingId))
                throw new ApplicationException("Booking not found");

            return _bookings[bookingId];
        }

        [HttpPost]
        public ResourceIdViewModel Post(BookingBindingModel model)
        {
            if (model.Nights <= 0)
                throw new ApplicationException("Nigts must be positive");
            if (!_rentals.ContainsKey(model.RentalId))
                throw new ApplicationException("Rental not found");

            var rental = _rentals.Values.Where(r => r.Id == model.RentalId).FirstOrDefault();
            if (rental == null)
            {
                throw new ApplicationException("Rental not found");
            }

            if (model.Unit <= 0)
            {
                throw new ApplicationException("Unit must be positive");
            }
            if (model.Unit > rental.Units)
            {
                throw new ApplicationException("Unit does not exist");
            }

            var bookingsLst = _bookings.Values;
            var alreadyBooked = false;
            if (bookingsLst.Any())
            {
                var startDate = model.Start;
                var endDate = model.Start.AddDays(model.Nights);
                alreadyBooked = bookingsLst.Any(b =>
                    b.RentalId == model.RentalId
                    && b.Unit == model.Unit
                    && VacationRentalHelper.IsDateInBetween(startDate, b.Start, b.Start.AddDays(b.Nights - 1).AddDays(rental.PreparationTimeInDays))
                );
            }

            if (alreadyBooked)
            {
                throw new ApplicationException("Not available - Already booked");
            }

            var key = new ResourceIdViewModel { Id = _bookings.Keys.Count + 1 };

            _bookings.Add(key.Id, new BookingViewModel
            {
                Id = key.Id,
                Nights = model.Nights,
                RentalId = model.RentalId,
                Start = model.Start.Date,
                Unit = model.Unit
            });

            return key;
        }
    }
}
