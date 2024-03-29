﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Resources;
using System.Xml.Linq;
using Microsoft.Phone.Net.NetworkInformation;
using MySchoolApp.Helpers;

namespace MySchoolApp
{
    public class MainViewModel : ViewModelBase
    {
        #region Properties

        public List<Link> NewsLinks { get; private set; }

        public List<Contact> Contacts { get; private set; }

        public List<Link> Links { get; private set; }

        public List<Link> Athletics { get; private set; }

        public List<Club> Clubs { get; private set; }

        public List<Forecast> Forecasts { get; private set; }

        public CurrentForecast CurrentForecast { get; private set; }

        public Settings Settings { get; set; }

        public LoadingState LoadingState { get; set; }

        public List<Location> Locations { get; set; }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        #endregion Properties

        public void LoadData()
        {
            loadSettings();
            loadLocations();
            loadNews();
            loadContacts();
            loadLinks();
            loadClubs();
            loadWeather();
            loadAthletics();
            IsDataLoaded = true;
        }

        #region Locations

        private void loadLocations()
        {
            StreamResourceInfo xml = Application.GetResourceStream(new Uri("/MySchoolApp;component/Data/Locations.xml", UriKind.Relative));
            NumberFormatInfo nfi = new NumberFormatInfo() { NumberDecimalSeparator = "." };
            var elems = XElement.Load(xml.Stream).Elements("location");
            Locations = (from item in elems
                         select new Location
                         {
                             Latitude = double.Parse(item.Attribute("latitude").Value, nfi),
                             Longitude = double.Parse(item.Attribute("longitude").Value, nfi),
                             Title = item.Attribute("title").Value
                         }).ToList();
            RaisePropertyChanged("Locations");
        }

        public string BingStaticMapUrl
        {
            get
            {
                NumberFormatInfo nfi = new NumberFormatInfo() { NumberDecimalSeparator = "." };

                try
                {
                    if (Locations.Count > 0)
                        return string.Format("http://dev.virtualearth.net/REST/v1/Imagery/Map/Road/{0},{1}/15?mapSize=376,376&pp={0},{1};21&mapVersion=v1&key={2}", Locations[0].Latitude.ToString(nfi), Locations[0].Longitude.ToString(nfi), Settings.BingMapsKey);
                    else
                        return string.Empty;
                }
                catch (NullReferenceException e)
                {
                    return string.Empty;
                }
            }
        }

        #endregion Locations

        #region Settings

        private void loadSettings()
        {
            StreamResourceInfo xml = Application.GetResourceStream(new Uri("/MySchoolApp;component/Data/Settings.xml", UriKind.Relative));
            XDocument settingsDoc;
            try
            {
                settingsDoc = XDocument.Load(xml.Stream);
            }
            catch (Exception e)
            {
                settingsDoc = null;
            }

            if (settingsDoc != null)
            {
                Settings = new Settings()
                {
                    Name = settingsDoc.Root.Element("name").Value,
                    NewsUrl = settingsDoc.Root.Element("newsUrl").Value,
                    BingMapsKey = settingsDoc.Root.Element("bingMapsKey").Value,
                    ThemeColor1 = Utils.GetColorFromRGB(settingsDoc.Root.Element("themeColor1").Value),
                    ThemeColor2 = Utils.GetColorFromRGB(settingsDoc.Root.Element("themeColor2").Value)
                };
            }
        }

        #endregion Settings

        #region News

        private void loadNews()
        {
            LoadingState = MySchoolApp.LoadingState.LOADING;
            RaisePropertyChanged("LoadingState");

            //check if network and client are available and newsurl exists
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                try
                {
                    if (!string.IsNullOrEmpty(App.ViewModel.Settings.NewsUrl))
                    {
                        string url = App.ViewModel.Settings.NewsUrl;

                        var request = HttpWebRequest.Create(url);
                        var result = (IAsyncResult)request.BeginGetResponse((iar) =>
                        {
                            try
                            {
                                var response = request.EndGetResponse(iar);

                                using (var stream = response.GetResponseStream())
                                using (var reader = new StreamReader(stream))
                                {
                                    string contents = reader.ReadToEnd();

                                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                                    {
                                        NewsLinks = Utils.GetLinksFromFeed(contents);

                                        LoadingState = MySchoolApp.LoadingState.COMPLETED;
                                        RaisePropertyChanged("LoadingState");
                                        RaisePropertyChanged("NewsLinks");
                                    });
                                }
                            }
                            catch
                            {
                                LoadingState = LoadingState.ERROR;
                                RaisePropertyChanged("LoadingState");
                            }
                        }, null);
                    }
                }
                catch (NullReferenceException e)
                {
                    Console.WriteLine("Line 134: Null Reference Exception. Message = {0}", e.Message);
                }
            }
        }

        #endregion News

        #region Contacts

        private void loadContacts()
        {
            StreamResourceInfo xml = Application.GetResourceStream(new Uri("/MySchoolApp;component/Data/Contacts.xml", UriKind.Relative));
            XDocument contactsDoc = XDocument.Load(xml.Stream);

            Contacts = (from item in contactsDoc.Descendants("contact")
                        select new Contact
                        {
                            Name = item.Element("name").Value.ToString(),
                            PhotoUrl = item.Element("photoUrl").Value.ToString(),
                            Email = item.Element("email").Value.ToString(),
                            PhoneNumber = item.Element("phoneNumber").Value.ToString()
                        }).ToList<Contact>();
            RaisePropertyChanged("Contacts");
        }

        #endregion Contacts

        #region Links

        private void loadLinks()
        {
            Links = parseLinkFile("/Data/Links.xml");
            RaisePropertyChanged("Links");
        }

        private List<Link> parseLinkFile(string resourcePath)
        {
            StreamResourceInfo xml = Application.GetResourceStream(new Uri("/MySchoolApp;component" + resourcePath, UriKind.Relative));

            return (from item in XElement.Load(xml.Stream).Elements("link")
                    select new Link
                    {
                        Title = item.Element("title").Value.ToString(),
                        Url = item.Element("url").Value.ToString(),
                        IsRss = item.Attribute("isRSS") != null
                    }).ToList<Link>();
        }

        private void loadAthletics()
        {
            Athletics = parseLinkFile("/Data/Athletics.xml");
            RaisePropertyChanged("Athletics");
        }

        #endregion Links

        #region Clubs

        private void loadClubs()
        {
            StreamResourceInfo xml = Application.GetResourceStream(new Uri("/MySchoolApp;component/Data/Clubs.xml", UriKind.Relative));

            Clubs = (from item in XElement.Load(xml.Stream).Elements("club")
                     select new Club
                     {
                         Title = item.Element("title").Value,
                         Url = item.Element("url").Value,
                         Subtitle = item.Element("subtitle").Value,
                     }).ToList<Club>();

            RaisePropertyChanged("Clubs");
        }

        #endregion Clubs

        #region Weather

        private void loadWeather()
        {
            //check if network and client are available and newsurl exists
            if (NetworkInterface.GetIsNetworkAvailable() && (Locations != null && Locations.Count > 0))
            {
                string googleWeather = "http://www.google.com/ig/api?weather=,,,{0},{1}&hl={2}";
                int latitude = Utils.GeoToGoogleCode(Locations[0].Latitude);
                int longitude = Utils.GeoToGoogleCode(Locations[0].Longitude);
                string url = string.Format(googleWeather, latitude, longitude, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);

                var request = HttpWebRequest.Create(url);
                request.BeginGetResponse((iar) =>
                {
                    try
                    {
                        var response = request.EndGetResponse(iar);

                        using (var stream = response.GetResponseStream())
                        using (var reader = new StreamReader(stream, System.Text.Encoding.GetEncoding("iso-8859-1")))
                        {
                            string contents = reader.ReadToEnd();

                            Deployment.Current.Dispatcher.BeginInvoke(() =>
                            {
                                try
                                {
                                    XElement root = XElement.Parse(contents).Element("weather");

                                    //create new forecasts with name and date
                                    Forecasts = (from item in root.Elements("forecast_conditions")
                                                 select new Forecast
                                                 {
                                                     Name = Utils.ToLongDayOfWeekName(item.Element("day_of_week").Attribute("data").Value),
                                                     LowTemperature = int.Parse(item.Element("low").Attribute("data").Value),
                                                     HighTemperature = int.Parse(item.Element("high").Attribute("data").Value),
                                                     Conditions = item.Element("condition").Attribute("data").Value,
                                                     ImageUrl = "http://www.google.com/" + item.Element("icon").Attribute("data").Value.Replace(".gif", ".png")
                                                 }).ToList<Forecast>();
                                    var currentforecastnode = root.Element("current_conditions");
                                    CurrentForecast = new CurrentForecast()
                                    {
                                        //TODO: change to temp_c if using celcius
                                        Temperature = currentforecastnode.Element("temp_f").Attribute("data").Value,
                                        Wind = currentforecastnode.Element("wind_condition").Attribute("data").Value,
                                        Humidity = currentforecastnode.Element("humidity").Attribute("data").Value,
                                        Conditions = currentforecastnode.Element("condition").Attribute("data").Value,
                                        ImageUrl = "http://www.google.com/" + currentforecastnode.Element("icon").Attribute("data").Value.Replace(".gif", ".png")
                                    };

                                    RaisePropertyChanged("Forecasts");
                                    RaisePropertyChanged("CurrentForecast");
                                }
                                catch (Exception e)
                                {
                                }
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }, null);
            }
        }

        #endregion Weather
    }
}