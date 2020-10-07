using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Xml.Serialization;
using AirlineTickets.Model;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using RestSharp;

namespace AirlineTickets.Repo
{
    public class AmadeusRepo
    { 

        public static async Task<dynamic> GetFlightOffers(string requestUrl)
        {
            Task<string> getToken = GetToken();
            string baseUrl = "https://test.api.amadeus.com/v2/shopping/flight-offers";
            
            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("GET"), baseUrl + requestUrl))
                {
                    await getToken;
                    request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {getToken.Result}");

                    var response = await httpClient.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        var result = response.Content.ReadAsStringAsync().Result;
                        dynamic json = JObject.Parse(result);
                        if(json.data.Count == 0 || json.data == null)
                        {
                            throw new Exception("Nije pronađen niti jedan let.");
                        }
                        return json.data;
                    }
                    else
                    {
                        throw new Exception(response.StatusCode.ToString() + ": " + response.ReasonPhrase);
                    }
                }
            }
        }

        public static async Task<int> PassToDatabase(FlightOfferSearch flightOfferSearch)
        {
            try
            {
                dynamic flightOffers = await AmadeusRepo.GetFlightOffers(flightOfferSearch.URLQuery);

                flightOfferSearch.Id = await FlightOfferSearchesRepo.Create(flightOfferSearch);
                foreach (var flightOffer in flightOffers)
                {
                    FlightOfferResult flightOfferResult = new FlightOfferResult();
                    flightOfferResult.FlightOfferSearchId = flightOfferSearch.Id;
                    flightOfferResult.PriceForAllTravelers = (decimal)flightOffer.price.total;
                    
                    flightOfferResult.Id = await FlightOfferResultsRepo.Create(flightOfferResult);

                    FlightsForOffer ffoOutgoing = new FlightsForOffer();
                    ffoOutgoing.Duration = flightOffer.itineraries[0].duration;
                    ffoOutgoing.IsReturnFlight = false;
                    ffoOutgoing.FlightOfferResultId = flightOfferResult.Id;
                    ffoOutgoing.Id = await FlightsForOfferRepo.Create(ffoOutgoing);

                    int i = 1;
                    foreach (var flightSegment in flightOffer.itineraries[0].segments)
                    {
                        Flight flight = new Flight();
                        flight.DepartureAirportId = await AirportsRepo.ReadByIATA(flightSegment.departure.iataCode.ToString());
                        flight.ArrivalAirportId = await AirportsRepo.ReadByIATA(flightSegment.arrival.iataCode.ToString());
                        flight.DepartureTime = DateTime.Parse(flightSegment.departure.at.ToString());
                        flight.ArrivalTime = DateTime.Parse(flightSegment.arrival.at.ToString());
                        flight.Duration = flightSegment.duration;
                        flight.Order = short.Parse(i.ToString());
                        flight.FlightForOfferId = ffoOutgoing.Id;
                        await FlightsRepo.Create(flight);
                        i++;
                    }
                    if (flightOffer.itineraries.Count > 1)
                    {
                        FlightsForOffer ffoIncoming = new FlightsForOffer();
                        ffoIncoming.Duration = flightOffer.itineraries[1].duration;
                        ffoIncoming.IsReturnFlight = true;
                        ffoIncoming.FlightOfferResultId = flightOfferResult.Id;
                        ffoIncoming.Id = await FlightsForOfferRepo.Create(ffoIncoming);

                        i = 1;
                        foreach (var flightSegment in flightOffer.itineraries[1].segments)
                        {
                            Flight flight = new Flight();
                            flight.DepartureAirportId = await AirportsRepo.ReadByIATA(flightSegment.departure.iataCode.ToString());
                            flight.ArrivalAirportId = await AirportsRepo.ReadByIATA(flightSegment.arrival.iataCode.ToString());
                            flight.DepartureTime = DateTime.Parse(flightSegment.departure.at.ToString());
                            flight.ArrivalTime = DateTime.Parse(flightSegment.arrival.at.ToString());
                            flight.Duration = flightSegment.duration;
                            flight.Order = short.Parse(i.ToString());
                            flight.FlightForOfferId = ffoIncoming.Id;
                            await FlightsRepo.Create(flight);
                            i++;
                        }
                    }
                }
                return flightOfferSearch.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GenerateUrl(string originalLocationCode, string destinationLocationCode, DateTime departureDate, DateTime? returnDate, short adults, short? children, short? infants, TravelClassEnum travelClass, CurrencyCodeEnum? currencyCode, bool? nonStop)
        {
            string requestUrl;
            if (!String.IsNullOrEmpty(originalLocationCode) && !String.IsNullOrEmpty(destinationLocationCode) && departureDate != null && adults > 0)
            {
                requestUrl = $"?originLocationCode={originalLocationCode}&destinationLocationCode={destinationLocationCode}&departureDate={departureDate:yyyy-MM-dd}&adults={adults}";
            }
            else
            {
                throw new Exception("All required fields have to be filled.");
            }
            if (returnDate.HasValue)
            {
                requestUrl += $"&returnDate={returnDate:yyyy-MM-dd}";
            }
            if (children.HasValue && children > 0)
            {
                requestUrl += $"&childrend={children.Value}";
            }
            if (infants.HasValue && infants > 0)
            {
                requestUrl += $"&infants={infants.Value}";
            }
            if (travelClass != TravelClassEnum.Any)
            {
                requestUrl += $"&travelClass={Enum.GetName(typeof(TravelClassEnum), travelClass)}";
            }
            if (currencyCode.HasValue)
            {
                requestUrl += $"&currencyCode={Enum.GetName(typeof(CurrencyCodeEnum), currencyCode)}";
            }
            if (nonStop.HasValue)
            {
                requestUrl = $"&nonStop={nonStop.Value}";
            }
            return requestUrl;
        }

        public static async Task<string> GetToken()
        {
            const string clientId = "FmUBvGxg9uSleN0XHKtXvKeVm5O59ung";
            const string clientSecret = "HpjUIGUEAd1H5idY";
            const string tokenUrl = "https://test.api.amadeus.com/v1/security/oauth2/token";
            string token;
            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), tokenUrl))
                {
                    request.Content = new StringContent($"grant_type=client_credentials&client_id={clientId}&client_secret={clientSecret}");
                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

                    var response = await httpClient.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        var result = response.Content.ReadAsStringAsync().Result;
                        dynamic json = JObject.Parse(result);
                        token = json.access_token;
                    }
                    else
                    {
                        throw new Exception(response.StatusCode.ToString() + ": " + response.ReasonPhrase);
                    }
                }
            }
            return token;
        }
    }
}
