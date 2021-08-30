using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VacationRental.Api.Helpers
{
    public static class VacationRentalHelper
    {
        #region Public Methods

        public static bool IsDateInBetween(DateTime dateToCheck, DateTime startDate, DateTime endDate)
        {
            var retVal = false;
            try
            {
                retVal = dateToCheck >= startDate && dateToCheck <= endDate;
            }
            catch (Exception e)
            {
                // ignore
            }

            return retVal;
        }

        #endregion Public Methods
    }
}
