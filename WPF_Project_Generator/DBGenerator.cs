using Flights_project.DAO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static WPF_Project_Generator.ViewModel;

namespace WPF_Project_Generator
{
    class DBGenerator 
    {
        private const string URL = "https://randomuser.me/api";

        private static Random random = new Random();

        List<Flights_project.Country> CountryList = new List<Flights_project.Country>();
        List<Flights_project.AirlineCompany> AirlineCompanies = new List<Flights_project.AirlineCompany>();
        List<Flights_project.Customer> Customers = new List<Flights_project.Customer>();

        internal int Total { get; set; }
        internal int ProgressValue { get; set; }

        ViewModel _vm;
        public DBGenerator(ViewModel vm)
        {
            _vm = vm;
        }

        public void AddingCountries(string countryTB)
        {
            Flights_project.CountryDAOMSSQL countryDAOMSSQL = new Flights_project.CountryDAOMSSQL();
            CountryList.AddRange(countryDAOMSSQL.GetAll());

            HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(string.Format("https://restcountries.eu/rest/v2"));
            WebReq.Method = "GET";
            HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();

            string jsonString;
            using (Stream stream = WebResp.GetResponseStream())
            {
                StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8);
                jsonString = reader.ReadToEnd();
            }

            List<RootCountryObject> items = JsonConvert.DeserializeObject<List<RootCountryObject>>(jsonString);
            int counter = 0;
            foreach (var a in items)
            {
                int CountryRecordeAmount = Int32.Parse(countryTB);
                Flights_project.Country c = new Flights_project.Country(a.name);
                countryDAOMSSQL.Add(c);
                c = countryDAOMSSQL.GetCountryByName(c.CountryName);
                CountryList.Add(c);
                _vm.Current += 1;
                _vm.Status = (_vm.Current / Total) * 100;
                
                //  ProgressBarWork();

                if (++counter > CountryRecordeAmount)
                    break;
            }
        }

        public void AddingCustomers(string customerTB)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
            int num = Int32.Parse(customerTB);
            for (int i = 0; i < num; i++)
            {
                HttpResponseMessage response = client.GetAsync("").Result;
                if (response.IsSuccessStatusCode)
                {
                    Flights_project.CustomerDAOMSSQL customerDAOMSSQL = new Flights_project.CustomerDAOMSSQL();

                    WebResult dataObjects = response.Content.ReadAsAsync<WebResult>().Result;
                    Flights_project.Customer c = new Flights_project.Customer();

                    c.Address = dataObjects.results[0].location.city;
                    c.CreditCardNumber = dataObjects.results[0].cell;
                    c.FirstName = dataObjects.results[0].name.first;
                    c.LastName = dataObjects.results[0].name.last;
                    c.Password = dataObjects.results[0].login.password;
                    c.UserName = dataObjects.results[0].login.username;
                    c.PhoneNO = dataObjects.results[0].phone;

                    customerDAOMSSQL.Add(c);
                    c = customerDAOMSSQL.GetCustomerByUserName(c.UserName);
                    _vm.Current += 1;
                    _vm.Status = (_vm.Current / Total) * 100;
                    //  ProgressBarWork();
                }
                else
                {
                    Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                }
                // client.Dispose();

            }
        }

        public void AddingAirLines(string AirLineTB)
        {
            Flights_project.AirLineDAOMSSQL airLineDAOMSSQL = new Flights_project.AirLineDAOMSSQL();
            int num = Int32.Parse(AirLineTB);
            for (int i = 0; i < num; i++)
            {
                Flights_project.AirlineCompany a = new Flights_project.AirlineCompany();
                Flights_project.Country c = new Flights_project.Country();

                c = CountryList[random.Next(0, CountryList.Count)];
                a.AirlineName = $"{c.CountryName}" + "AirLine";
                a.CountryCode = c.ID;

                var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

                var userStringChars = new char[8];
                var PasswordStringChars = new char[8];

                for (int j = 0; j < userStringChars.Length; j++)
                {
                    userStringChars[j] = chars[random.Next(chars.Length)];
                    PasswordStringChars[j] = chars[random.Next(chars.Length)];
                }
                var finalStringUser = new String(userStringChars);
                var finalStringPassword = new String(PasswordStringChars);
                a.Password = finalStringPassword;
                a.UserName = finalStringUser;
                airLineDAOMSSQL.Add(a);
                a = airLineDAOMSSQL.GetAirLineByUserName(a.UserName);
                AirlineCompanies.Add(a);
                _vm.Current += 1;
                _vm.Status = (_vm.Current / Total) * 100;
                //  ProgressBarWork();

            }
        }

        public void AddingFligths(string FlightTB)
        {
            DateTime start = (DateTime.Today);
            int range = (start - new DateTime(2018, 1, 1)).Days;

            Flights_project.FlightDAOMSSQL flightDAOMSSQL = new Flights_project.FlightDAOMSSQL();
            int num = Int32.Parse(FlightTB);
            for (int i = 0; i < num; i++)
            {
                Flights_project.Flight f = new Flights_project.Flight();
                f.AirLineCompanyID = AirlineCompanies[random.Next(0, AirlineCompanies.Count)].ID;
                f.OriginCountryCode = CountryList[random.Next(0, CountryList.Count)].ID;
                f.DestinationCountryCode = CountryList[random.Next(0, CountryList.Count)].ID;
                f.RemainingTickets = random.Next(0, 250);
                f.LandingTime = start.AddDays(random.Next(range));
                f.DepartureTime = start.AddDays(random.Next(range));

                flightDAOMSSQL.Add(f);
                _vm.Current += 1;
                _vm.Status = (_vm.Current / Total) * 100;
                //  ProgressBarWork();

            }
        }

        public void AddingTickets(string TicketsTB)
        {
            Flights_project.TicketDAOMSSQL ticketDAOMSSQL = new Flights_project.TicketDAOMSSQL();
            Flights_project.FlightDAOMSSQL flightDAOMSSQL = new Flights_project.FlightDAOMSSQL();
            Flights_project.CustomerDAOMSSQL customerDAOMSSQL = new Flights_project.CustomerDAOMSSQL();

            IList<Flights_project.Flight> flightList = flightDAOMSSQL.GetAll();
            IList<Flights_project.Customer> CustomersList = customerDAOMSSQL.GetAll();

            int num = Int32.Parse(TicketsTB);
            for (int i = 0; i < num; i++)
            {
                Flights_project.Ticket t = new Flights_project.Ticket();
                t.CustomerID = CustomersList[random.Next(0, CustomersList.Count)].ID;
                t.FlightID = flightList[random.Next(0, flightList.Count)].ID;

                ticketDAOMSSQL.Add(t);
                double num1 = num / Total;
                _vm.Current += 1;
                _vm.Status = (_vm.Current / Total) * 100;
                //  ProgressBarWork();

            }
        }

        public void CleanDB()
        {
            AdminDAOMSSQL adminDAOMSSQL = new AdminDAOMSSQL();
            adminDAOMSSQL.DeleteAllTablesInfo();
        }
    }
}
