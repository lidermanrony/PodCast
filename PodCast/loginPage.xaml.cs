using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace PodCast
{
    public partial class loginPage : PhoneApplicationPage
    {
        private string SID;
        private string LSID;
        private string AUTH;

        public loginPage()
        {
            InitializeComponent();

            DataContext = App.LoginViewModel;
        }

        private async void loginButton_Click(object sender, RoutedEventArgs e)
        {
            App.LoginViewModel.IsWaiting = true;
            try
            {
               await this.Authenticate();
               IEnumerable<Uri> feedUrls = await this.GetFeedUris();
            }
            finally
            {
                App.LoginViewModel.IsWaiting = false;
            }
        }

        private async Task<IEnumerable<Uri>> GetFeedUris()
        {
            string requestUrl = string.Format(
                "https://www.google.com/reader/api/0/subscription/list?allcomments=true&output=json&ck={0}&client=scroll", 
                DateTime.Now.Ticks.ToString());
            WebClient client = new WebClient();
            client.Headers["Authorization"] = String.Format("GoogleLogin auth={0}", AUTH);
            client.Headers["Cookie"] = String.Format("SID={0}", SID);
            string result = await client.DownloadStringTask(new Uri(requestUrl, UriKind.Absolute));
            JObject jsonResponse = JObject.Parse(result);
            var subscriptions = jsonResponse["subscriptions"].Children();
            var subscriptionsWithCategories = subscriptions.Where(x => x["categories"].HasValues);
            //TODO handle case of multiple labels

            var listenSubscriptions = subscriptionsWithCategories.Where(x => x["categories"][0]["label"].ToString().Equals("Listen Subscriptions"));
            var feedUrls = listenSubscriptions.Select(x => x["id"].ToString().Substring("feed/".Length)).ToList();
            return feedUrls.Select(x => new Uri(x));

        }

        private async Task Authenticate()
        {
            var requestUrl = string.Format(
                "https://www.google.com/accounts/ClientLogin?service=reader&Email={0}&Passwd={1}",
                App.LoginViewModel.Username,
                App.LoginViewModel.Password);
            WebClient client = new WebClient();
            string response = await client.DownloadStringTask(new Uri(requestUrl));
            var split = response.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            SID = split[0].Substring(split[0].IndexOf("=") + 1);
            LSID = split[1].Substring(split[1].IndexOf("=") + 1);
            AUTH = split[2].Substring(split[2].IndexOf("=") + 1);
        }

    }
}