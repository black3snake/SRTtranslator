using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace SRTtranslator
{
    public partial class Form1
    {
        static async Task<string> GetAsync(HttpClient httpClient, string strInput, string lang)
        {
            string pattern = @"0A0-\d+";
            Regex regex = new Regex(pattern);
            string url = $@"translate_a/single?client=gtx&sl=auto&tl={lang}&dt=t&q={HttpUtility.UrlEncode(strInput)}";

            try
            {
                using (HttpResponseMessage response = await httpClient.GetAsync(url))
                {
                    response.EnsureSuccessStatusCode();
                    string content = await response.Content.ReadAsStringAsync();

                    var json = JsonConvert.DeserializeObject<object[][][]>(content, new JsonSerializerSettings
                    {
                        Error = (_, e) => { e.ErrorContext.Handled = true; }
                    });
                    var result = string.Join(" ", json[0].SelectMany(x => x.Skip(0)?.Take(1)).Cast<string>()).Replace("\\n", "\n").Replace("\n ", "\n");

                    return regex.Replace(result, "");
                }

            }
            catch (HttpRequestException ex) 
            //when (ex is HttpStatusCode.NotFound  ) 
            //{ StatusCode: HttpStatusCode.NotFound })
            {
                // 404
                logger.Error(ex.Message);
                return "Error404";

            }



        }

    }
}
