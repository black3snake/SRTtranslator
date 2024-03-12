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

        private static HttpClient Client = new HttpClient()
        {
            BaseAddress = new Uri("https://translate.googleapis.com")
        };

        internal void TranslatorHub(List<string> listSelectedFiles)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            Dictionary<string, string> dictO2 = new Dictionary<string, string>();
            
            foreach (string fileName in listSelectedFiles)
            {
                dict = DictSringFromFile(fileName);

                Task<Dictionary<string, string>> taskDic = Task.Run<Dictionary<string, string>>(() => TaskTanslateString3(dict));

                taskDic.ContinueWith((t) =>
                {
                    foreach (var te in t.Result)
                    {
                        Invoke(new Action(() =>
                        {
                            ConsoleTB.AppendText(te.Key + Environment.NewLine);
                            ConsoleTB.AppendText(te.Value + Environment.NewLine);
                            ConsoleTB.AppendText(Environment.NewLine);
                        }));
                    }

                    Invoke(new Action(() =>
                    {
                        if (ConsoleTB.TextLength > 500 && ConsoleTB.ScrollBars != System.Windows.Forms.ScrollBars.Vertical)
                        {
                            ConsoleTB.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
                            ConsoleTB.AppendText(ConsoleTB.TextLength + Environment.NewLine);
                        }
                    }));
                });
            }
        }

        internal async Task<Dictionary<string, string>> TaskTanslateString3(Dictionary<string, string> dict)
        {
            Dictionary<string, string> dict2 = new Dictionary<string, string>();

            foreach (var d in dict.Keys)
            {
                dict2.Add(d, await GetAsync(Client, d, "ru"));
            }
            return dict2;
        }

        internal Dictionary<string, string> DictSringFromFile(string fileName)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            using (StreamReader reader = new StreamReader(fileName, Encoding.UTF8))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (Regex.IsMatch(line, srtWord, RegexOptions.IgnoreCase))
                    {
                        dict.Add(line, null);
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
