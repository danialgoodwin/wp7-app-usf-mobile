using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using Microsoft.Phone.Net.NetworkInformation;
using MySchoolApp.Helpers;

namespace MySchoolApp
{
    public class FeedViewModel : ViewModelBase
    {
        public LoadingState LoadingState { get; set; }

        public FeedViewModel()
        {
            this.FeedLinks = new List<Link>();
        }

        public List<Link> FeedLinks { get; set; }

        private string uri;

        public string Uri
        {
            get
            {
                return uri;
            }
            set
            {
                if (uri == value)
                    return;
                uri = value;
                FeedLinks.Clear();
                LoadFeed();
            }
        }

        public void LoadFeed()
        {
            //check if network and client are available
            if (NetworkInterface.GetIsNetworkAvailable() && !String.IsNullOrEmpty(uri))
            {
                LoadingState = LoadingState.LOADING;
                RaisePropertyChanged("LoadingState");
                var wc = new WebClient();
                wc.DownloadStringCompleted += (sender, e) =>
                {
                    if (e.Error == null)
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            LoadingState = LoadingState.COMPLETED;
                            FeedLinks = Utils.GetLinksFromFeed(e.Result);
                            RaisePropertyChanged("FeedLinks");
                            RaisePropertyChanged("LoadingState");
                        });
                    }
                };
                wc.DownloadStringAsync(new Uri(uri, UriKind.Absolute));
            }
            else
            {
                LoadingState = LoadingState.ERROR;
                RaisePropertyChanged("LoadingState");
            }
        }
    }
}