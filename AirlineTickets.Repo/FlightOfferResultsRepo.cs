using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirlineTickets.Model;

namespace AirlineTickets.Repo
{
    public class FlightOfferResultsRepo
    {

        public static async Task<int> Create(FlightOfferResult flightOfferResult)
        {
            using (AirlineTicketsDBEntities db = new AirlineTicketsDBEntities())
            {
                try
                {
                    db.FlightOfferResults.Add(flightOfferResult);
                    await db.SaveChangesAsync();
                    return flightOfferResult.Id;
                }
                catch
                {
                    throw new Exception("Greška kod dodavanja FlightOfferResult u bazu");
                }
            }
        }

        public static async Task<bool> DeleteAll()
        {
            using (AirlineTicketsDBEntities db = new AirlineTicketsDBEntities())
            {
                try
                {
                    db.FlightOfferResults.RemoveRange(db.FlightOfferResults);
                    await db.SaveChangesAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    throw new Exception("Greška kod brisanja rezultata pretrage.");
                }
            }
        }
    }
}
