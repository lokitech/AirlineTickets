using AirlineTickets.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineTickets.Repo
{
    public class FlightOfferSearchesRepo
    {
        public async static Task<int> Create(FlightOfferSearch fos)
        {
            using (AirlineTicketsDBEntities db = new AirlineTicketsDBEntities())
            {
                try
                {
                    db.FlightOfferSearches.Add(fos);
                    await db.SaveChangesAsync();
                    return fos.Id;
                }
                catch (Exception ex)
                {
                    throw new Exception("Greška kod spremanja FlightOfferSearch u bazu podataka.");
                }

            }
        }

        public async static Task<FlightOfferSearch> ReadByUrl(string reqUrl)
        {
            using (AirlineTicketsDBEntities db = new AirlineTicketsDBEntities())
            {
                FlightOfferSearch fos = db.FlightOfferSearches.Where(f => f.URLQuery == reqUrl)
                    .Include(f => f.Airport)
                    .Include(f => f.Airport1)
                    .Include(f => f.FlightOfferResults)
                    .Include(f => f.FlightOfferResults.Select(s => s.FlightsForOffers))
                    .Include(f => f.FlightOfferResults.Select(ffo => ffo.FlightsForOffers.Select(fl => fl.Flights)))
                    .Include(f => f.FlightOfferResults.Select(ffo => ffo.FlightsForOffers.Select(fl => fl.Flights.Select(a => a.Airport))))
                    .Include(f => f.FlightOfferResults.Select(ffo => ffo.FlightsForOffers.Select(fl => fl.Flights.Select(a => a.Airport1))))
                    .FirstOrDefault();
                //foreach(var fOfferResult in fos.FlightOfferResults)
                //{
                //    foreach(var flightForOffer in fOfferResult.FlightsForOffers)
                //    {
                //        foreach(var flight in flightForOffer.Flights)
                //        {
                //            flight.Airport = await AirportsRepo.ReadById(flight.DepartureAirportId);
                //            flight.Airport1 = await AirportsRepo.ReadById(flight.ArrivalAirportId);
                //        }
                //    }
                //}
                return fos;
            }
        }

        public async static Task<FlightOfferSearch> ReadById(int id)
        {
            using (AirlineTicketsDBEntities db = new AirlineTicketsDBEntities())
            {
                FlightOfferSearch fos = db.FlightOfferSearches.Where(f => f.Id == id)
                    .Include(f => f.Airport)
                    .Include(f => f.Airport1)
                    .Include(f => f.FlightOfferResults)
                    .Include(f => f.FlightOfferResults.Select(s => s.FlightsForOffers))
                    .Include(f => f.FlightOfferResults.Select(or => or.FlightsForOffers.Select(fl => fl.Flights)))
                    .Include(f => f.FlightOfferResults.Select(ffo => ffo.FlightsForOffers.Select(fl => fl.Flights.Select(a => a.Airport))))
                    .Include(f => f.FlightOfferResults.Select(ffo => ffo.FlightsForOffers.Select(fl => fl.Flights.Select(a => a.Airport1))))
                    .FirstOrDefault();
                return fos;
            }
        }


        public static async Task<bool> DeleteAll()
        {
            using (AirlineTicketsDBEntities db = new AirlineTicketsDBEntities())
            {
                try
                {
                    db.FlightOfferSearches.RemoveRange(db.FlightOfferSearches);
                    await db.SaveChangesAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    throw new Exception("Greška kod brisanja pretraga.");
                }
            }
        }

        public async static Task<bool> DeleteAllSearches()
        {
            using (AirlineTicketsDBEntities db = new AirlineTicketsDBEntities())
            {
                try
                {
                    await FlightsRepo.DeleteAll();
                    await FlightsForOfferRepo.DeleteAll();
                    await FlightOfferResultsRepo.DeleteAll();
                    await DeleteAll();
                    return true;
                }
                catch(Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
