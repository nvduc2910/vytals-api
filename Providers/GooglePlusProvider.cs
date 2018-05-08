using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Vytals.Exceptions;

namespace Vytals.Providers
{
    public class GooglePlusProvider : IGooglePlusProvider
    {
        //public async Task<UserGooglePlusReturn> GetUserInfoAsync(string accessToken)
        //{
        //    try
        //    {
        //        var url = "https://www.googleapis.com/oauth2/v1/userinfo?alt=json&access_token=" + accessToken;
        //        var response = await new HttpClient().GetAsync(url);
        //        var stringJson = await response.Content.ReadAsStringAsync();
        //        return JsonConvert.DeserializeObject<UserGooglePlusReturn>(stringJson);
        //    }
        //    catch(Exception ex)
        //    {
        //        throw new InvalidFacebookTokenException(ex.Message);
        //    }
        //}
    }
}
