using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineTickets.Model
{
    [MetadataType(typeof(FlightMetadata))]
    public partial class Flight
    {
    }

    public class FlightMetadata
    {
        [DisplayName("Let")]
        public int Id { get; set; }

        [DisplayName("Mjesto polijetanja")]
        [Required]
        public int DepartureAirportId { get; set; }

        [DisplayName("Vrijeme polijetanja")]
        public DateTime DepartureTime { get; set; }

        [DisplayName("Mjesto slijetanja")]
        [Required]
        public int ArrivalAirportId { get; set; }

        [DisplayName("Vrijeme slijetanja")]
        public DateTime ArrivalTime { get; set; }

        [DisplayName("Vrijeme leta")]
        [MaxLength(30)]
        public string Duration { get; set; }
    }
}
