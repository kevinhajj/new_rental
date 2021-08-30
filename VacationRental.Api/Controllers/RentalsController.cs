using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Helpers;
using VacationRental.Api.Models;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/rentals")]
    [ApiController]
    public class RentalsController : ControllerBase
    {
        private readonly IDictionary<int, RentalViewModel> _rentals;
        private readonly IDictionary<int, BookingViewModel> _bookings;

        public RentalsController(IDictionary<int, RentalViewModel> rentals,
            IDictionary<int, BookingViewModel> bookings)
        {
            _rentals = rentals;
            _bookings = bookings;
        }

        [HttpGet]
        [Route("{rentalId:int}")]
        public RentalViewModel Get(int rentalId)
        {
            if (!_rentals.ContainsKey(rentalId))
                throw new ApplicationException("Rental not found");

            return _rentals[rentalId];
        }

        [HttpPost]
        public ResourceIdViewModel Post(RentalBindingModel model)
        {
            var key = new ResourceIdViewModel { Id = _rentals.Keys.Count + 1 };

            if (model.Units <= 0)
            {
                throw new ApplicationException("Units should be greater than 0 (Units > 0)");
            }

            if (model.PreparationTimeInDays < 0)
            {
                throw new ApplicationException("'PreparationTimeInDays' should be greater or equal to 0 (PreparationTimeInDays>=0)");
            }

            _rentals.Add(key.Id, new RentalViewModel
            {
                Id = key.Id,
                Units = model.Units,
                PreparationTimeInDays = model.PreparationTimeInDays
            });

            return key;
        }

        [HttpPut]
        [Route("{rentalId:int}")]
        public ResourceIdUpdateModel Put(int rentalId, RentalBindingModel model)
        {

            if (rentalId <= 0 || !_rentals.ContainsKey(rentalId))
            {
                throw new ApplicationException("Rental not found");
            }

            if (model.Units <= 0)
            {
                throw new ApplicationException("Units should be greater than 0 (Units > 0)");
            }

            if (model.PreparationTimeInDays < 0)
            {
                throw new ApplicationException("'PreparationTimeInDays' should be greater or equal to 0 (PreparationTimeInDays>=0)");
            }

            var rentalToEdit = _rentals.Where(r => r.Key == rentalId).FirstOrDefault();
            var rental = rentalToEdit.Value;
            if (rental == null)
            {
                throw new ApplicationException("Rental not found");
            }

            var bookingsLst = _bookings.Values.Where(b => b.RentalId == rentalId).ToList();
            var conflictExists = false;
            if (bookingsLst.Any())
            {
                if (rental.Units > model.Units)
                {
                    //// The user is reducing the number of units => Check if there is any unit booked from the removed units \\\\
                    if (bookingsLst.Any(b => b.Unit > model.Units))
                    {
                        throw new ApplicationException("Booked unit(s) cannot be deleted");
                    }
                }
                if (rental.PreparationTimeInDays < model.PreparationTimeInDays)
                {
                    //// The user is increasing the PreparationTimeInDays => Check if there is any conflict with the bookings list \\\\
                    foreach (var bookingItem in bookingsLst)
                    {
                        var startDate = bookingItem.Start;
                        var endDate = bookingItem.Start.AddDays(bookingItem.Nights);
                        conflictExists = bookingsLst.Any(b =>
                            b.Id != bookingItem.Id
                            && b.Unit == bookingItem.Unit
                            && VacationRentalHelper.IsDateInBetween(startDate, b.Start, b.Start.AddDays(b.Nights - 1).AddDays(model.PreparationTimeInDays))
                        );

                        if (conflictExists)
                        {
                            throw new ApplicationException("'PreparationTimeInDays' is causing conflict with the bookings list");
                        }
                    }
                }
            }


            rental.Units = model.Units;
            rental.PreparationTimeInDays = model.PreparationTimeInDays;

            var retVal = new ResourceIdUpdateModel
            {
                Id = rentalId,
                Units = rental.Units,
                PreparationTimeInDays = rental.PreparationTimeInDays
            };

            return retVal;
        }
    }
}
