using Golf.HttpApi.Host.Helpers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
namespace Golf.HttpApi.Host.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class MapController : ControllerBase
    {
        private IHttpClientFactory _factory;

        public MapController(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        private string GetJArrayValue(JObject yourJArray, JToken key)
        {
            string value = "";
            foreach (JToken item in yourJArray.Children())
            {
                var itemProperties = item.Children<JProperty>();
                //If the property name is equal to key, we get the value
                var myElement = itemProperties.FirstOrDefault(x => x.Name == key.ToString());
                value = myElement.Value.ToString(); //It run into an exception here because myElement is null
                break;
            }
            return value;
        }

        // GET: api/<MapController>
        /// <summary>
        /// Lấy khoảng cách giữu hai điểm trên bản đồ
        /// </summary>
        /// <param name="sourceGPSAsdress"></param>
        /// <param name="destGPSAsdress"></param>
        /// <returns></returns>
        [HttpGet()]
        public ActionResult<dynamic> GetDistance(string sourceGPSAsdress,string destGPSAsdress)//sourceGPSAsdress="21.035098,105.850309";destGPSAsdress="21.035793,105.833526"
        {
            HttpClient client = _factory.CreateClient();
            client.BaseAddress = new Uri("https://maps.vietmap.vn");
            var response = client.GetAsync("/api/route?api-version=1.1&apikey=383a90729d0590f9e1074083a11791ff64767fb56c1d9c4f&vehicle=car&point="+sourceGPSAsdress+"&point="+destGPSAsdress).Result;
            string jsonData = response.Content.ReadAsStringAsync().Result;
            string data = JObject.Parse(jsonData)["paths"][0]["distance"].ToString() ;// JsonSerializer.Deserialize<List<string>>(jsonData);
            //var tmp = GetJArrayValue(data, "paths");
            //var tmp1 = GetJArrayValue(tmp, "paths");
            return data;
        }

       
       
    }
}
