using Golf.Core.Common.Odoo.OdooResponse;
using Golf.Core.Dtos.Controllers.ShopControler.Response;
using Golf.Core.Exceptions;
using Golf.Domain.Shared.OdooAPI;
using Golf.Domain.Shared.Setting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Golf.Services.OdooAPI
{
    public class OdooAPIService
    {
        public readonly AppSettings _appSettings;
        private CookieContainer _cookieContainer = new CookieContainer();
        private HttpClient HttpClient { get; set; }
        private HttpClientHandler HttpClientHandler { get; set; }
        // private HttpContext httpContext { get; set; }
        public OdooAPIService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
            HttpClientHandler = new HttpClientHandler
            {
                AllowAutoRedirect = true,
                UseCookies = true,
                CookieContainer = _cookieContainer
            };
            HttpClient = new HttpClient(HttpClientHandler);
        }
        public CookieContainer Cookies
        {
            get { return _cookieContainer; }
            set { _cookieContainer = value; }
        }
        public int GetUserID()
        {
            var cookies = Cookies.GetCookies(new Uri(_appSettings.BaseOdooUrl));
            var userIDCookie = cookies.Where(c => c.Name == _appSettings.OdooUserID).FirstOrDefault();
            if (userIDCookie == null)
            {
                throw new ForbiddenException("No session exists");
            }
            return int.Parse(userIDCookie.Value);
        }
        public int GetPartnerID()
        {
            var cookies = Cookies.GetCookies(new Uri(_appSettings.BaseOdooUrl));
            var partnerIDCookie = cookies.Where(c => c.Name == _appSettings.OdooPartnerID).FirstOrDefault();
            if (partnerIDCookie == null)
            {
                throw new ForbiddenException("No session exists");
            }
            return int.Parse(partnerIDCookie.Value);
        }

        public async Task<K> CallAPI<T, K>(APIMethod method, string url, T requestData) 
        {
            HttpResponseMessage httpResponseMessage;

            switch (method)
            {
                case APIMethod.GET:
                    {
                        //var json = JsonConvert.SerializeObject(requestData);
                        //var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json"); // use MediaTypeNames.Application.Json in Core 3.0+ and Standard 2.1+
                        if (requestData != null)
                        {
                            url = url + "/" + requestData.ToString();
                        }
                        httpResponseMessage = await HttpClient.GetAsync(url);
                        break;
                    }
                case APIMethod.POST:
                    {
                        var json = JsonConvert.SerializeObject(requestData);
                        var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json"); // use MediaTypeNames.Application.Json in Core 3.0+ and Standard 2.1+
                        httpResponseMessage = await HttpClient.PostAsync(url, stringContent);
                        break;
                    }
                case APIMethod.PUT:
                    {
                        var json = JsonConvert.SerializeObject(requestData);
                        var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json"); // use MediaTypeNames.Application.Json in Core 3.0+ and Standard 2.1+
                        httpResponseMessage = await HttpClient.PutAsync(url, stringContent);
                        break;
                    }
                case APIMethod.DELETE:
                    {
                        var json = JsonConvert.SerializeObject(requestData);
                        var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json"); // use MediaTypeNames.Application.Json in Core 3.0+ and Standard 2.1+
                        httpResponseMessage = await HttpClient.DeleteAsync(url);
                        break;
                    }
                case APIMethod.PATCH:
                    {
                        var json = JsonConvert.SerializeObject(requestData);
                        var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json"); // use MediaTypeNames.Application.Json in Core 3.0+ and Standard 2.1+
                        httpResponseMessage = await HttpClient.PostAsync(url, stringContent);
                        break;
                    }
                default:
                    {
                        throw new BadRequestException("Invalid method");
                    }
            }
            var content = await httpResponseMessage.Content.ReadAsStringAsync();
            try
            {
                var check = JsonConvert.DeserializeObject<OdooResponse<OdooResult<dynamic>>>(content);
                if (check.Result == null)
                {
                    var checktmp = JsonConvert.DeserializeObject<OdooResult<dynamic>>(content);

                    if (checktmp.Code != 200 && checktmp.Code != 0)
                    {
                        var tmp = JsonConvert.DeserializeObject<OdooResult<DataResponseBase>>(content);
                        throw new Exception(tmp.Data.Message);
                    }
                }
                else
                {
                    if (check.Result.Code != 200 && check.Result.Code != 0)
                    {
                        var tmp = JsonConvert.DeserializeObject<OdooResponse<OdooResult<DataResponseBase>>>(content);
                        throw new Exception(tmp.Result.Data.Message);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            };
            var result = JsonConvert.DeserializeObject<K>(content);
            return result;
        }
        public async Task<K> CallAPI<K>(APIMethod method, string url, MultipartFormDataContent requestData)
        {
            HttpResponseMessage httpResponseMessage;
            switch (method)
            {
                case APIMethod.POST:
                    {
                        httpResponseMessage = await HttpClient.PostAsync(url, requestData);
                        break;
                    }
                case APIMethod.PUT:
                    {
                        httpResponseMessage = await HttpClient.PostAsync(url, requestData);
                        break;
                    }
                case APIMethod.DELETE:
                    {
                        httpResponseMessage = await HttpClient.PostAsync(url, requestData);
                        break;
                    }
                case APIMethod.PATCH:
                    {
                        httpResponseMessage = await HttpClient.PostAsync(url, requestData);
                        break;
                    }
                default:
                    {
                        throw new BadRequestException("Invalid method");
                    }
            }
            var content = await httpResponseMessage.Content.ReadAsStringAsync();
            try
            {
                var check = JsonConvert.DeserializeObject<OdooResponse<OdooResult<dynamic>>>(content);
                if (check.Result == null)
                {
                    var checktmp = JsonConvert.DeserializeObject<OdooResult<dynamic>>(content);

                    if (checktmp.Code != 200 && checktmp.Code != 0)
                    {
                        var tmp = JsonConvert.DeserializeObject<OdooResult<DataResponseBase>>(content);
                        throw new Exception(tmp.Data.Message);
                    }
                }
                else
                {
                    if (check.Result.Code != 200 && check.Result.Code != 0)
                    {
                        var tmp = JsonConvert.DeserializeObject<OdooResponse<OdooResult<DataResponseBase>>>(content);
                        throw new Exception(tmp.Result.Data.Message);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            };
            var result = JsonConvert.DeserializeObject<K>(content);
            return result;

        }
    }
}
