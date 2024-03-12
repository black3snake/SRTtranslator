using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SRTtranslator
{
    public partial class Form1
    {
        static readonly string srtWord = @"^[^\d{1,}]\w.*$";
        List<string> listSTR = new List<string>();
        Dictionary<string, string> dictOut = new Dictionary<string, string>();

        private static HttpClient Client = new HttpClient()
        {
            BaseAddress = new Uri("https://translate.googleapis.com")
        };

        internal async void TranslatorHub(List<string> listSelectedFiles)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            Dictionary<string, string> dictO2 = new Dictionary<string, string>();
            
            foreach (string fileName in listSelectedFiles)
            {
                dict = DictSringFromFile(fileName);

                dictO2 =  await TaskTanslateString2(dict);

                foreach (var te in dictO2)
                {
                    ConsoleTB.AppendText(te.Key + Environment.NewLine);
                    ConsoleTB.AppendText(te.Value + Environment.NewLine);
                    ConsoleTB.AppendText(Environment.NewLine);
                }

                if (ConsoleTB.TextLength > 500)
                {
                    ConsoleTB.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
                    ConsoleTB.AppendText(ConsoleTB.TextLength + Environment.NewLine);
                }
            }



        }

        internal async Task<Dictionary<string, string>> TaskTanslateString2(Dictionary<string, string> dict)
        {
            Dictionary<string, string> dict2 = new Dictionary<string, string>();

            //var result = await Client.GetAsync(uriName);
            foreach(var d in dict.Keys)
            {
                dict2.Add(d, await GetAsync(Client, d, "ru"));

            }
            //var result = GetAsync(Client, InputString, "ru");

            //listSTR.Add(result.ToString());
            return dict2;
        }
        
        internal async Task<string> TaskTanslateString(string InputString)
        //internal List<string> TaskTanslateString(string InputString) 
        {
            //List<string> listSTR = new List<string>();
            Dictionary<string, string> dict = new Dictionary<string, string>();

            //var result = await Client.GetAsync(uriName);
            var result = await GetAsync(Client, InputString, "ru");
            //var result = GetAsync(Client, InputString, "ru");

            listSTR.Add(result.ToString());
            return result;
        }


        internal Dictionary<string, string> DictSringFromFile(string fileName)
        {
            /*if (ConsoleTB.TextLength > 0)
                ConsoleTB.Clear();*/
            List<string> list = new List<string>();
            Dictionary<string, string> dict = new Dictionary<string, string>();

            using (StreamReader reader = new StreamReader(fileName, Encoding.UTF8))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (Regex.IsMatch(line, srtWord, RegexOptions.IgnoreCase))
                    {
                        //list.Add(line);
                        dict.Add(line, null);
                        //ConsoleTB.AppendText(line + Environment.NewLine);
                        //var test = await TaskTanslateString(line);
                        //ConsoleTB.AppendText(test + Environment.NewLine);
                        //Console.WriteLine(line);


                    }
                }
                if (ConsoleTB.TextLength > 500)
                {
                    ConsoleTB.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
                }
            }


            return dict;
        }


    }
}
