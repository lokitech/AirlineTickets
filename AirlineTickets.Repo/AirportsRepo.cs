using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.UI;
using AirlineTickets.Model;
using HtmlAgilityPack;

namespace AirlineTickets.Repo
{
    public class AirportsRepo
    {

        public static async Task<int> Create(Airport airport)
        {
            using(AirlineTicketsDBEntities db = new AirlineTicketsDBEntities())
            {
                if (airport == null)
                {
                    throw new Exception("Airport instance was not set.");
                }
                if(db.Airports != null && db.Airports.Where(a => a.IATA == airport.IATA).Count() > 0)
                {
                    return db.Airports.Where(a => a.IATA == airport.IATA).FirstOrDefault().Id;
                }
                db.Airports.Add(airport);
                await db.SaveChangesAsync();
                return airport.Id;
            }
        }

        public static async void CreateRange(List<Airport> airports)
        {
            using (AirlineTicketsDBEntities db = new AirlineTicketsDBEntities())
            {
                if (airports == null)
                {
                    throw new Exception("Airport instance was not set.");
                }
                db.Airports.AddRange(airports);
                await db.SaveChangesAsync();
            }
        }

        public static async Task<Airport> ReadById(int id)
        {
            using (AirlineTicketsDBEntities db = new AirlineTicketsDBEntities())
            {
                Airport airport = new Airport();
                airport = await db.Airports.FindAsync(id);
                if(airport == null)
                {
                    throw new Exception("Airport with " + id + " ID was not found.");
                }
                return airport;
            }
        }
        public static async Task<int> ReadByIATA(string IATA)
        {
            using (AirlineTicketsDBEntities db = new AirlineTicketsDBEntities())
            {
                Airport airport = new Airport();
                airport = await db.Airports.Where(a => a.IATA == IATA).FirstOrDefaultAsync();
                if (airport == null)
                {
                    throw new Exception("Airport with " + IATA + " IATA was not found.");
                }
                return airport.Id;
            }
        }

        public static async Task<List<Airport>> ReadByName(string name)
        {
            using (AirlineTicketsDBEntities db = new AirlineTicketsDBEntities())
            {
                List<Airport> airports = new List<Airport>();
                airports = await db.Airports.Where(a => a.Name.Contains(name)).ToListAsync();
                if (airports == null)
                {
                    throw new Exception(name + " was not found.");
                }
                return airports;
            }
        }

        public static async Task<List<Airport>> ReadAll()
        {
            using (AirlineTicketsDBEntities db = new AirlineTicketsDBEntities())
            {
                List<Airport> airports = new List<Airport>();
                airports = await db.Airports.ToListAsync();
                return airports;
            }
        }

        public static async Task<int> Count()
        {
            using (AirlineTicketsDBEntities db = new AirlineTicketsDBEntities())
            {
                int count = await db.Airports.CountAsync();
                return count;
            }
        }

        public static async Task<bool> DeleteAll()
        {
            using (AirlineTicketsDBEntities db = new AirlineTicketsDBEntities())
            {
                try
                {
                    db.Airports.RemoveRange(db.Airports);
                    await db.SaveChangesAsync();
                    return true;
                }
                catch(Exception ex)
                {
                    throw new Exception("Prvo je potrebno obrisati pretrage!");
                }
            }
        }

        public static async Task<List<Airport>> Search(string search, string otherSearch)
        {
            using (AirlineTicketsDBEntities db = new AirlineTicketsDBEntities())
            {
                if(otherSearch != "")
                {
                    otherSearch = GetIATA(otherSearch);
                }

                List<Airport> airports = new List<Airport>();
                if(search.Count() <= 3)
                {
                    airports = await db.Airports.Where(a => a.IATA.StartsWith(search.ToUpper()) && a.IATA != otherSearch).Take(10).ToListAsync();
                }
                else if(airports.Count == 0)
                {
                    airports = await db.Airports.Where(a => (a.Location.ToLower().Contains(search.ToLower()) || a.Name.ToLower().Contains(search.ToLower())) && a.IATA != otherSearch).Take(10).ToListAsync();
                }
                

                return airports;
            }
        }

        public static string GetIATA(string text)
        {
            string IATA;
            IATA = Regex.Match(text, @"\(([^)]*)\)").Groups[1].Value;
            return IATA;
        }

        public static List<Airport> ScrapeAirports(string link = "")
        {
            List<Airport> airports = new List<Airport>();
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(link);
            //table -> tbody -> tr
            HtmlNode[] tableRows = doc.DocumentNode.SelectNodes("//table[contains(@class, 'wikitable sortable')]/tbody/tr").Skip(2).ToArray();
            int i = 0;
            foreach (HtmlNode row in tableRows)
            {
                if (row.SelectNodes("td") != null)
                {
                    
                    Airport airport = new Airport();
                    HtmlNode[] tableData = row.SelectNodes("td").ToArray();
                    airport.IATA = tableData[0].FirstChild.InnerText.Trim();
                    airport.ICAO = tableData[1].FirstChild == null ? "" : tableData[1].FirstChild.InnerText.Trim();
                    airport.Name = tableData[2].FirstChild == null ? "" : tableData[2].FirstChild.InnerText.Trim();
                    string[] location = tableData[3].InnerText.Split(',');
                    if (location.Length == 3)
                    {
                        string city = location[0].Trim();
                        string county = location[1].Trim();
                        string country = location[1].Trim();
                    }
                    else if(location.Length == 2)
                    {
                        string city = location[0].Trim();
                        string country = location[1].Trim();
                    }
                    else if(location.Length == 1)
                    {
                        string city = location[0].Trim();
                    }
                    airport.Location = tableData[3].InnerText.Trim();
                    airports.Add(airport);
                    i++;
                }
               
            }
            return airports;
        }
        public static List<Airport> ScrapeAllAirports()
        {
            List<Airport> allAirports = new List<Airport>();
            string baseLink = "https://en.wikipedia.org/wiki/List_of_airports_by_IATA_airport_code:_";
            for (char c = 'A'; c <= 'Z'; c++)
            {
                allAirports.AddRange(ScrapeAirports(baseLink + c));
            }
            return allAirports;
        }
    }
}
