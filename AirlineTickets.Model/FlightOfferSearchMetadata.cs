using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AirlineTickets.Model
{

    [MetadataType(typeof(FlightOfferSearchMetadata))]
    public partial class FlightOfferSearch
    {
        public void CreateOffer(string originalLocationCode, string destinationLocationCode, string departureDate, string returnDate, short? adults, short? children, short? infants, string travelClass, string currencyCode, bool? nonStop)
        {
            if (String.IsNullOrEmpty(originalLocationCode) || String.IsNullOrEmpty(destinationLocationCode) || String.IsNullOrEmpty(departureDate) || !adults.HasValue || adults.Value <= 0)
            {
                throw new Exception("Sva označena polja su obavezna!");
            }
            using (AirlineTicketsDBEntities db = new AirlineTicketsDBEntities())
            {
                try
                {
                    this.OriginAirportId = db.Airports.Where(a => a.IATA == originalLocationCode).FirstOrDefault().Id;
                    this.DestinationAirportId = db.Airports.Where(a => a.IATA == destinationLocationCode).FirstOrDefault().Id;
                }
                catch (Exception ex)
                {
                    throw new Exception("Nije pronađen aerodrom sa tim IATA kodom.");
                }
            }
            this.URLQuery = $"?originLocationCode={originalLocationCode}&destinationLocationCode={destinationLocationCode}";
            this.DepartureDate = Convert.ToDateTime(departureDate);
            if (this.DepartureDate < DateTime.Now.Date)
            {
                throw new Exception("Datum polaska mora biti kasnije od današnjeg dana.");
            }
            this.Adults = adults.Value;
            if (this.Adults > 9)
            {
                throw new Exception("Broj odraslih ne može biti veći od 9");
            }
            this.URLQuery += $"&departureDate={this.DepartureDate:yyyy-MM-dd}&adults={this.Adults}";

            if (!String.IsNullOrEmpty(returnDate))
            {
                try
                {
                    this.ReturnDate = Convert.ToDateTime(returnDate);
                }
                catch (Exception ex)
                {
                    throw new Exception("Datum mora biti formata MM/DD/YYYY");
                }

                if (this.ReturnDate <= this.DepartureDate)
                {
                    throw new Exception("Datum povratka mora biti nakon datuma polaska.");
                }
                this.URLQuery += $"&returnDate={this.ReturnDate:yyyy-MM-dd}";
            }


            if (children.HasValue && children.Value > 0)
            {
                if (children > 9)
                {
                    throw new Exception("Broj djece ne može biti veći od 9");
                }
                this.Children = children.Value;
                this.URLQuery += $"&children={this.Children}";
            }

            if (infants.HasValue && infants.Value > 0)
            {
                if (infants > this.Adults)
                {
                    throw new Exception("Broj dojenčadi mora biti jednak ili manji broju odraslih.");
                }
                this.Infants = infants.Value;
                this.URLQuery += $"&infants={this.Infants}";
            }
            if (!String.IsNullOrEmpty(travelClass) && travelClass != "0")
            {
                this.TravelClass = short.Parse(travelClass);
                string travelClassCode = Enum.GetName(typeof(TravelClassEnum), this.TravelClass);
                if (String.IsNullOrEmpty(travelClassCode))
                {
                    throw new Exception("Ne postoji odabrani razred putovanja.");
                }
                this.URLQuery += $"&travelClass={Enum.GetName(typeof(TravelClassEnum), this.TravelClass)}";
            }
            if (!String.IsNullOrEmpty(currencyCode))
            {
                this.CurrencyCode = short.Parse(currencyCode);
                string currency = Enum.GetName(typeof(CurrencyCodeEnum), this.CurrencyCode);
                if (String.IsNullOrEmpty(currency))
                {
                    throw new Exception("Ne postoji odabrana valuta.");
                }
                this.URLQuery += $"&currencyCode={currency}";
            }
            if (String.IsNullOrEmpty(currencyCode))
            {
                this.CurrencyCode = 1;
            }
            if (nonStop.HasValue && nonStop.Value == true)
            {

                this.NonStop = nonStop.Value;
                this.URLQuery += $"&nonStop={this.NonStop.ToString().ToLower()}";
            }

        }

        public short TotalTravelers
        {
            get
            {
                short count = 0;
                count += Adults;
                if (Children.HasValue)
                {
                    count += Children.Value;
                }
                if (Infants.HasValue)
                {
                    count += Infants.Value;
                }
                return count;
            }
        }

        [DisplayName("Valuta")]
        public string Currency
        {
            get
            {
                return Enum.GetName(typeof(CurrencyCodeEnum), this.CurrencyCode);
            }
        }
        [DisplayName("Razred")]
        public string TravelClassTranslation
        {
            get
            {
                switch (TravelClass)
                {
                    case 1:
                        return "Ekonomski";

                    case 2:
                        return "Premium ekonomski";

                    case 3:
                        return "Poslovni";

                    case 4:
                        return "Prvi";

                    default:
                        return "Bilo koji";

                }
            }
        }

        [DisplayName("Jednosmjeran let")]
        public bool OneWay
        {
            get
            {
                return !ReturnDate.HasValue;
            }
        }
    }
    public class FlightOfferSearchMetadata
    {
        [DisplayName("Ponuda putovanja")]
        public int Id { get; set; }

        [DisplayName("Polazni aerodrom")]
        [Required]
        public int OriginAirportId { get; set; }

        [DisplayName("Odredišni aerodrom")]
        [Required]
        public int DestinationAirportId { get; set; }

        [DisplayName("Datum polaska")]
        [Required]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime DepartureDate { get; set; }

        [DisplayName("Datum povratka")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime ReturnDate { get; set; }

        [DisplayName("Broj odraslih")]
        [Required]
        public short Adults { get; set; }

        [DisplayName("Broj djece")]
        public short Children { get; set; }

        [DisplayName("Broj dojenčadi")]
        public short Infants { get; set; }

        [DisplayName("Klasa")]
        public short TravelClass { get; set; }

        [DisplayName("Valuta")]
        public short CurrencyCode { get; set; }

        [DisplayName("Bez presjedanja")]
        public bool NonStop { get; set; }

        [DisplayName("URL Zahtjev")]
        [Required]
        public string URLQuery { get; set; }
    }
}
