using Vytals.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Vytals.Services.Notifications
{
    public class OneSignalService
    {
        #region Fields

        private string oneSignalAppId = "e97913c0-0f01-4bac-8fc0-4fecfb87d8f2";
        private string oneSignalApiKey = "Basic " + "NDBmZWQyZDktNDIwZi00NWMxLWEzZWQtODUzMGMxNzhhM2Ez";

        #endregion

        //public async void PushNotification(string message, string deviceId, Notification dataPush)
        // {
        //    try
        //    {
        //        using (var client = new HttpClient())
        //        {
        //            client.BaseAddress = new Uri("https://onesignal.com");
        //            client.DefaultRequestHeaders.Add("authorization", oneSignalApiKey);
        //            client.DefaultRequestHeaders
        //            .Accept
        //            .Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //            var obj = new
        //            {
        //                app_id = oneSignalAppId,
        //                contents = new { en = message },
        //                data = new { receiveId  = dataPush },
        //                include_player_ids = new string[] { deviceId }
        //            };

        //            var jsonPost = JsonConvert.SerializeObject(obj);
        //            var content = new StringContent(jsonPost, Encoding.UTF8, "application/json");

        //            var result = await client.PostAsync("/api/v1/notifications", content);
        //            string resultContent = await result.Content.ReadAsStringAsync();
        //            Console.WriteLine(resultContent);
        //        }
        //    }
        //    catch(Exception ex)
        //    {

        //    }
        //}

    }
}
