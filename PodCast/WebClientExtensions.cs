using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PodCast
{
    public static class WebClientExtensions
    {
        public static Task<string> DownloadStringTask(this WebClient client, Uri address)
        {
            var tcs = new TaskCompletionSource<string>();
            DownloadStringCompletedEventHandler handler = null;
            handler = (sender, e) => 
                {
                    client.DownloadStringCompleted -= handler;

                    if (e.Error != null)
                    {
                        tcs.SetException(e.Error);
                    }

                    tcs.SetResult(e.Result);
                };

            client.DownloadStringCompleted += handler;
            client.DownloadStringAsync(address);

            return tcs.Task;
        }
    }
}
