using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Extensions
{
    public static class DateTimeExtensions
    {
        public static int CalculateAge(this DateTime dbo)
        {
           var today = DateTime.Today;
           var age = today.Year - dbo.Year;
           if (dbo.Date > today.AddYears(-age))
           {
             age--; // If today is not the birthday then decrease the age by one year.
           }
            return age;
        }
    }
}