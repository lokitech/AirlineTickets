using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineTickets.Model
{

    [MetadataType(typeof(AirportMetadata))]
    public partial class Airport
    {
        public string WholeString
        {
            get
            {
                return Location + " (" + IATA + ")"; 
            }
        }
    }
    public class AirportMetadata
    {
        [DisplayName("Areodrom")]
        public int Id { get; set; }

        [DisplayName("IATA")]
        [Required]
        [MaxLength(3)]
        public string IATA { get; set; }

        [DisplayName("ICAO")]
        [MaxLength(4)]
        public string ICAO { get; set; }

        [DisplayName("Naziv aerodroma")]
        [MaxLength(200)]
        [Required]
        public string Name { get; set; }

        [DisplayName("Lokcaija aerodroma")]
        [MaxLength(200)]
        [Required]
        public string Location { get; set; }
    }
}
