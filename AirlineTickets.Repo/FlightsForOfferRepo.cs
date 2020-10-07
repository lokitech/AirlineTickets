using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirlineTickets.Model;

namespace AirlineTickets.Repo
{
    public class FlightsForOfferRepo
    {
        public static async Task<int> Create(FlightsForOffer ffo)
        {
            using (AirlineTicketsDBEntities db = new AirlineTicketsDBEntities())
            {
                try
                {
                    db.FlightsForOffers.Add(ffo);
                    await db.SaveChangesAsync();
                    return ffo.Id;
                }
                catch(Exception ex)
                {
                    throw new Exception("Greška kod dodavanja FlightsForOffers u bazu");
                }
            }
        }

        public static async Task<bool> DeleteAll()
        {
            using (AirlineTicketsDBEntities db = new AirlineTicketsDBEntities())
            {
                try
                {
                    db.FlightsForOffers.RemoveRange(db.FlightsForOffers);
                    await db.SaveChangesAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    throw new Exception("Greška kod brisanja FlightsForOffers.");
                }
            }
        }
    }
}
