using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using System.Globalization;
using Microsoft.AspNetCore.Builder;
using System.Net.Http;
using Vytals.Exceptions;
using Vytals.Resources;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Net;
using System;

namespace Vytals.Providers
{
    public class FacebookProvider : IFacebookProvider
    {
        private readonly IOptions<FacebookOptions> facebookOptions;
        private readonly IHttpContextAccessor httpContextAccessor;

        public FacebookProvider(IOptions<FacebookOptions> facebookOptions, IHttpContextAccessor httpContextAccessor)
        {
            this.facebookOptions = facebookOptions;
            this.httpContextAccessor = httpContextAccessor;
        }

        //public async Task<UserInfoModel> GetUserInfoAsync(string accessToken)
        //{
        //    var endpoint = QueryHelpers.AddQueryString(facebookOptions.Value.UserInformationEndpoint, "access_token",
        //        accessToken);

        //    if (facebookOptions.Value.SendAppSecretProof)
        //    {
        //        endpoint = QueryHelpers.AddQueryString(endpoint, "appsecret_proof", GenerateAppSecretProof(accessToken));
        //    }
        //    if (facebookOptions.Value.Fields.Count > 0)
        //    {
        //        endpoint = QueryHelpers.AddQueryString(endpoint, "fields",
        //            string.Join(",", facebookOptions.Value.Fields));
        //    }

        //    var response = await new HttpClient().GetAsync(endpoint, httpContextAccessor.HttpContext.RequestAborted);

        //    if (!response.IsSuccessStatusCode)
        //    {
        //        throw new InvalidFacebookTokenException(FailureReturnMessages.InvalidFacebookToken);
        //    }

        //    JObject payload = JObject.Parse(await response.Content.ReadAsStringAsync());

        //    var facebookPic = await GetPictureUrl(payload["id"].ToString());

        //    if (payload["email"] == null)

        //        return new UserInfoModel
        //        {
        //            FullName = payload["name"].ToString(),
        //            Email = payload["id"].ToString(),
        //            Avatar = facebookPic
        //        };

        //    try
        //    {
        //        return new UserInfoModel
        //        {
        //            FullName = payload["name"].ToString(),
        //            Email = payload["email"].ToString(),
        //            Avatar = facebookPic
        //        };
        //    }
        //    catch
        //    {
        //        throw new InvalidFacebookTokenException(FailureReturnMessages.GetFacebookInfoFailed);
        //    }
        //}

        public async Task<string> GetPictureUrl(string faceBookId)
        {
            WebResponse response = null;
            string pictureUrl = string.Empty;
            try
            {
                WebRequest request = WebRequest.Create(string.Format("https://graph.facebook.com/{0}/picture?type=large", faceBookId));
                response = await request.GetResponseAsync();
                pictureUrl = response.ResponseUri.ToString();
            }
            catch (Exception ex)
            {
                //? handle
            }
            finally
            {
                if (response != null) response.Dispose();
            }
            return pictureUrl;
        }

        private string GenerateAppSecretProof(string accessToken)
        {
            using (var algorithm = new HMACSHA256(Encoding.ASCII.GetBytes(facebookOptions.Value.AppSecret)))
            {
                var hash = algorithm.ComputeHash(Encoding.ASCII.GetBytes(accessToken));
                var builder = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    builder.Append(hash[i].ToString("x2", CultureInfo.InvariantCulture));
                }
                return builder.ToString();
            }
        }
    }
}
