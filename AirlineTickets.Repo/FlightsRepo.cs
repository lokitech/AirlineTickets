using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using AirlineTickets.Model;

namespace AirlineTickets.Repo
{
    public class FlightsRepo
    {
        public static async Task<int> Create(Flight flight)
        {
            using (AirlineTicketsDBEntities db = new AirlineTicketsDBEntities())
            {
                try
                {
                    db.Flights.Add(flight);
                    await db.SaveChangesAsync();
                    return flight.Id;
                }
                catch(Exception ex)
                {
                    throw new Exception("Greška kod dodavanja leta u bazu.");
                }
            }
        }

        public static async Task<bool> DeleteAll()
        {
            using (AirlineTicketsDBEntities db = new AirlineTicketsDBEntities())
            {
                try
                {
                    db.Flights.RemoveRange(db.Flights);
                    await db.SaveChangesAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    throw new Exception("Greška kod brisanja letova.");
                }
            }
        }
    }
}
