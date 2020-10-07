using AirlineTickets.Model;
using AirlineTickets.Repo;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Net;

namespace AirlineTickets.App.Controllers
{
    public class HomeController : Controller
    {
        
        public async Task<ActionResult> Index(string originalLocationCode, string originalLocation, string destinationLocationCode, string destinationLocation, string departureDate, string returnDate, short? adults, short? children, short? infants, string travelClass, string currencyCode, bool? nonStop, int? page)
        {
            
            //int count = await AirportsRepo.Count();
            //if (count == 0)
            //{
            //    List<Airport> airports = AirportsRepo.ScrapeAllAirports();
            //    foreach (Airport airport in airports)
            //    {
            //        await AirportsRepo.Create(airport);
            //    }
            //}

            List<SelectListItem> numberOfAdults = new List<SelectListItem>();
            List<SelectListItem> numberOfChildren = new List<SelectListItem>();
            List<SelectListItem> numberOfInfants = new List<SelectListItem>();
            
            for (int i=0; i<=9; i++)
            {
                if(i != 0)
                {
                    numberOfAdults.Add(new SelectListItem
                    {
                        Text = i.ToString(),
                        Value = i.ToString()
                    });
                }
                numberOfChildren.Add(new SelectListItem
                {
                    Text = i.ToString(),
                    Value = i.ToString()
                });
                numberOfInfants.Add(new SelectListItem
                {
                    Text = i.ToString(),
                    Value = i.ToString()
                });
            }
            ViewBag.adults = numberOfAdults;
            ViewBag.children = numberOfChildren;
            ViewBag.infants = numberOfInfants;

            if (Request.IsAjaxRequest())
            {
                return RedirectToAction("List", new
                {
                    originalLocationCode,
                    originalLocation,
                    destinationLocationCode,
                    destinationLocation,
                    departureDate,
                    returnDate,
                    adults,
                    children,
                    infants,
                    travelClass,
                    currencyCode,
                    nonStop,
                    page
                });
            }

            return View();
        }

        

        public async Task<ActionResult> List(string originalLocationCode, string originalLocation, string destinationLocationCode, string destinationLocation, string departureDate, string returnDate, short? adults, short? children, short? infants, string travelClass, string currencyCode, bool? nonStop, int? page)
        {
            try
            {
                if(String.IsNullOrEmpty(originalLocationCode) || String.IsNullOrEmpty(destinationLocationCode) || String.IsNullOrEmpty(departureDate))
                {
                    return null;
                }

                int pageSize = 10;
                int pageNumber = (page ?? 1);

                FlightOfferSearch flightOfferSearch = new FlightOfferSearch();
                flightOfferSearch.CreateOffer(originalLocationCode, destinationLocationCode, departureDate, returnDate, adults, children, infants, travelClass, currencyCode, nonStop);
                FlightOfferSearch search = await FlightOfferSearchesRepo.ReadByUrl(flightOfferSearch.URLQuery);
                if(search == null)
                {
                    int searchId = await AmadeusRepo.PassToDatabase(flightOfferSearch);
                    search = await FlightOfferSearchesRepo.ReadById(searchId);
                    return PartialView(search.FlightOfferResults.ToPagedList(pageNumber, pageSize));
                }
                else
                {
                    return PartialView(search.FlightOfferResults.ToList().ToPagedList(pageNumber, pageSize));
                }
            }
            catch(Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Options()
        {
            return View();
        } 

        [HttpPost]
        public async Task<JsonResult> SearchAirports(string search, string otherSearch)
        {
            List<Airport> airports = await AirportsRepo.Search(search, otherSearch);
            var airportList = airports.Select(a => new { a.IATA, a.Name, a.Location });
            return Json(airportList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> ScapreAirports()
        {
            try
            {
                int count = await AirportsRepo.Count();
                if (count == 0)
                {
                    List<Airport> airports = AirportsRepo.ScrapeAllAirports();
                    foreach (Airport airport in airports)
                    {
                        await AirportsRepo.Create(airport);
                    }
                    return Json(new { Message = "Uspješno spremljeni aerodromi!" });
                }
                return Json(new { Message = "" });
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Message = ex.Message + " Molim ponovno učitajte stranicu." });
            }
        }

        public async Task<JsonResult> DeleteAirports()
        {
            try
            {
                await AirportsRepo.DeleteAll();
                return Json(new { Message = "Uspješno obrisano!" });
            }
            catch(Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Message = ex.Message });
            }
            
        }
        public async Task<JsonResult> DeleteSearches()
        {
            try
            {
                await FlightOfferSearchesRepo.DeleteAllSearches();
                return Json(new { Message = "Uspješno obrisano!" });
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Message = ex.Message });
            }
            
        }
    }
}