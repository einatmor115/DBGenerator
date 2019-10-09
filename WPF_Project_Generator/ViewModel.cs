using Newtonsoft.Json;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace WPF_Project_Generator
{
    public class ViewModel : INotifyPropertyChanged
    {
        public class WebResult
        {
            public WebUser[] results;
        }
        public class WebUser
        {
            public string cell;
            public string phone;
            public Name name;   
            public Location location;
            public Login login;
        }
        public class Login
        {
            public string username;
            public string password;
        }
        public class Location
        {
            //public string street;
            public string city;
            public string state;
            public string postcode;

        }
        public class Name
        {           
            public string first;
            public string last;
        }

        DBGenerator dBGenerator { get; set; }
        private bool _onWorking = false;
        
        public DelegateCommand MyDelegate { get; set; }
        public DelegateCommand MyDeleteDelegate { get; set; }
 
        public event PropertyChangedEventHandler PropertyChanged;

        private int status;
        public int Status
        {
            get
            {
                return this.status;
            }
            set
            {
                this.status = value;
                OnPropertyChanged("Status");
            }
        }

        public int Current { get; set; }

        private string countryTB;
        public string CountryTB
        {
            get
            {
                return this.countryTB;
            }
            set
            {
                this.countryTB = value;
                OnPropertyChanged("CountryTB");
            }
        }

        private string customerTB;
        public string CustomerTB
        {
            get
            {
                return this.customerTB;
            }
            set
            {
                this.customerTB = value;
                OnPropertyChanged("CustomerTB");
            }
        }

        private string airLineTB;
        public string AirLineTB
        {
            get
            {
                return this.airLineTB;
            }
            set
            {
                this.airLineTB = value;
                OnPropertyChanged("AirLineTB");
            }
        }

        private string flightTB;
        public string FlightTB
        {
            get
            {
                return this.flightTB;
            }
            set
            {
                this.flightTB = value;
                OnPropertyChanged("FlightTB");
            }
        }

        private string ticketsTB;
        public string TicketsTB
        {
            get
            {
                return this.ticketsTB;
            }
            set
            {
                this.ticketsTB = value;
                OnPropertyChanged("TicketsTB");
            }
        }

        public ViewModel()
        {
            dBGenerator = new DBGenerator(this);        
            MyDelegate = new DelegateCommand(ExecuteCommand, CanExecuteMethod);
            MyDeleteDelegate = new DelegateCommand(DeleteCommand, DeleteCanExecuteMethod);
            Task.Run(() =>
            {
                while (true)
                {
                    MyDelegate.RaiseCanExecuteChanged();
                    MyDeleteDelegate.RaiseCanExecuteChanged();
                    Thread.Sleep(5);
                }
            });

        }

        private bool CanExecuteMethod()
        {
            return ! _onWorking;
        }


        private async void ExecuteCommand()
        {
            dBGenerator.Total = Convert.ToInt32(countryTB) + Convert.ToInt32(customerTB) + Convert.ToInt32(airLineTB) + Convert.ToInt32(ticketsTB) + Convert.ToInt32(flightTB);
            _onWorking = true;
            dBGenerator.ProgressValue = status = 0 ;
            await Task.Run(() =>
           {
               Current = 0;
               Status = 0;
               dBGenerator.AddingCountries(countryTB);
               dBGenerator.AddingCustomers(customerTB);
               dBGenerator.AddingAirLines(airLineTB);
               dBGenerator.AddingFligths(flightTB);
               dBGenerator.AddingTickets(ticketsTB);

               _onWorking = false;

             });

        }

        private bool DeleteCanExecuteMethod()
        {
            return !_onWorking;
        }

        private async void DeleteCommand()
        {
            dBGenerator.CleanDB();

            dBGenerator.Total = Convert.ToInt32(countryTB) + Convert.ToInt32(customerTB) + Convert.ToInt32(airLineTB) + Convert.ToInt32(ticketsTB) + Convert.ToInt32(flightTB);
            _onWorking = true;
            dBGenerator.ProgressValue = status = 0;
            await Task.Run(() =>
            {
                Current = 0;
                Status = 0;
                dBGenerator.AddingCountries(countryTB);
                dBGenerator.AddingCustomers(customerTB);
                dBGenerator.AddingAirLines(airLineTB);
                dBGenerator.AddingFligths(flightTB);
                dBGenerator.AddingTickets(ticketsTB);

                _onWorking = false;

            });
        }

        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        //public async void ProgressBarWork()
        //{
        //    while (_onWorking)
        //    {
        //        if (dBGenerator.ProgressValue <= 100)
        //        {
        //            Dispatcher.CurrentDispatcher.Invoke(() =>
        //            {
        //                Status = dBGenerator.ProgressValue;
        //            //_pb.Value = 100 * GeneralCounter / MaxTotal;
        //            //_tx.Text = (100 * GeneralCounter / MaxTotal).ToString();

        //        });
        //        }
        //        if (dBGenerator.ProgressValue > 100)
        //        {
        //            Dispatcher.CurrentDispatcher.Invoke(() =>
        //            {
        //                status = 100;
        //                //_pb.Value = 100;
        //                //_tx.Text = "100";
        //            });
        //        }
        //        await Task.Run(() => Thread.Sleep(50));
        //        //await Task.Delay(50);
        //    }
        //}
    }
}

