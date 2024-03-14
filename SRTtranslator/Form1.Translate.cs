using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SRTtranslator
{
    public partial class Form1
    {
        static readonly string srtWord = @"^[^\d{1,}]\w.*$";
        object locker = new object();
        ParallelOptions options = new ParallelOptions();

        static BlockingCollection<MyClass> bc = new BlockingCollection<MyClass>();


        private static HttpClient Client = new HttpClient()
        {
            BaseAddress = new Uri("https://translate.googleapis.com")
        };

        internal async void TranslatorHub(List<string> listSelectedFiles)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            Dictionary<string, string> dictO2 = new Dictionary<string, string>();

            //options.MaxDegreeOfParallelism = Environment.ProcessorCount > 2 ? Environment.ProcessorCount - 1 : 1;
            ConsoleTB.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;

            await Zapusk(listSelectedFiles);

            OutputC(bc);

            //listSelectedFiles.AsParallel().WithDegreeOfParallelism(options.MaxDegreeOfParallelism).ForAll(ls => { MyTask(ls); });


            /*foreach (string fileName in listSelectedFiles)
            {
                dict = DictSringFromFile(fileName);

                Task<Dictionary<string, string>> taskDic = Task.Run<Dictionary<string, string>>(() => TaskTanslateString3(dict));
                //                Task<Dictionary<string, string>> taskDic = new Task<Dictionary<string, string>>(TaskTanslateString2, dict);
                //Task<Dictionary<string, string>> taskDic2 = new Task<Dictionary<string, string>>(() => TaskTanslateString3(dict));
                //taskDic2.Start();
                //Task.Run<Dictionary<string, string>>(() => TaskTanslateString3(dict));

                taskDic.ContinueWith((t) =>
                {
                    lock (locker)
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

                        Invoke(new Action(() =>
                        {
                            ConsoleTB.AppendText($"Task Id ended : {Task.CurrentId}" + Environment.NewLine);
                            ConsoleTB.AppendText($"Thread Id ended : {Thread.CurrentThread.ManagedThreadId}" + Environment.NewLine);
                            ConsoleTB.AppendText($"файл: {fileName}");
                        }));
                    }
                }, TaskContinuationOptions.AttachedToParent); ;
            }*/
        }

        internal Task Zapusk(List<string> listSelectedFiles)
        {
            options.MaxDegreeOfParallelism = Environment.ProcessorCount > 2 ? Environment.ProcessorCount - 1 : 1;

            listSelectedFiles.AsParallel().WithDegreeOfParallelism(options.MaxDegreeOfParallelism).ForAll(ls => { MyTask(ls); });

            return Task.CompletedTask;
        }

        internal void MyTask(object arg)
        {
            string ft = (string)arg;

            logger.Info($"MyTask: CurrentId {Task.CurrentId} with ManagedThreadId {Thread.CurrentThread.ManagedThreadId} запущен, имя файла {ft}" + Environment.NewLine);

            var dict = DictSringFromFile(ft);
            var resultT = TaskTanslateString3(dict);

            MyClass myClass = new MyClass(ft, resultT.Result);
            bc.TryAdd(myClass);

            #region Random
            /*var random = new Random();
            var lowerBound = 3000;
            var upperBound = 7000;
            var rNum = random.Next(lowerBound, upperBound);

            Thread.Sleep(rNum);*/
            #endregion

            logger.Info($"MyTask: CurrentId " + Task.CurrentId + " завершен." + Environment.NewLine);
        }

        internal void OutputC(BlockingCollection<MyClass> bc)
        {
            foreach (var item in bc)
            {
                ConsoleTB.AppendText(item.FileName + Environment.NewLine);
                foreach (var te in item.Dict)
                {
                    ConsoleTB.AppendText(te.Key + Environment.NewLine);
                    ConsoleTB.AppendText(te.Value + Environment.NewLine);
                    ConsoleTB.AppendText(Environment.NewLine);

                }

                ConsoleTB.AppendText($"Конец файла: {item.FileName}" + Environment.NewLine);
                ConsoleTB.AppendText(Environment.NewLine);
            }

            if (ConsoleTB.TextLength > 500)
            {
                ConsoleTB.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
                ConsoleTB.AppendText(ConsoleTB.TextLength + Environment.NewLine);
            }

        }


        internal Dictionary<string, string> TaskTanslateString2(Dictionary<string, string> dict)
        {
            //Dictionary<string, string> dict2 = new Dictionary<string, string>();
            return dict;
        }
        
       
        internal async Task<Dictionary<string, string>> TaskTanslateString3(Dictionary<string, string> dict)
        {
            Dictionary<string, string> dict2 = new Dictionary<string, string>();
            /*Invoke(new Action(() =>
            {
                ConsoleTB.AppendText($"Task Id метода TaskTanslateString3 : {Task.CurrentId}" + Environment.NewLine);
                ConsoleTB.AppendText($"Thread Id метода DTaskTanslateString3 : {Thread.CurrentThread.ManagedThreadId}" + Environment.NewLine);
            }));*/

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

                /*if (ConsoleTB.TextLength > 500)
                {
                    ConsoleTB.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
                }*/
            }


            return dict;
        }


    }

    internal class MyClass
    {
        public string FileName;
        public Dictionary<string, string> Dict;

        public MyClass(string fileName, Dictionary<string, string> dict)
        {
            this.FileName = fileName;
            this.Dict = dict;
        }
    }
}
