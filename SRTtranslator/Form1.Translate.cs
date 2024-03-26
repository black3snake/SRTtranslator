using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SRTtranslator
{
    public partial class Form1
    {
        static readonly string srtWord = @"^[^\d{1,}]\w*.*$";
        object locker = new object();
        ParallelOptions options = new ParallelOptions();

        //static BlockingCollection<MyClass> bc = new BlockingCollection<MyClass>();


        private static HttpClient Client = new HttpClient()
        {
            BaseAddress = new Uri("https://translate.googleapis.com")
        };

        internal void TranslatorHub(List<string> listSelectedFiles)
        {
            //Dictionary<string, string> dict = new Dictionary<string, string>();
            //Dictionary<string, string> dictO2 = new Dictionary<string, string>();


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

        internal async Task ZapuskAsync(List<string> listSelectedFiles)
        {
            options.MaxDegreeOfParallelism = Environment.ProcessorCount > 2 ? Environment.ProcessorCount - 1 : 1;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            await Task.Run(() =>
            {
                listSelectedFiles.AsParallel().WithDegreeOfParallelism(options.MaxDegreeOfParallelism).ForAll(ls => { MyTask(ls); });
            });


            stopwatch.Stop();
            TimeSpan stopwatchElapsed = stopwatch.Elapsed;
            var milsec = Convert.ToInt32(stopwatchElapsed.TotalMilliseconds);
            var sec = milsec / 1000;
            var ts = TimeSpan.FromSeconds(sec);

            elapsedTimeGetStatus = $"{ts.Hours}ч:{ts.Minutes}м:{ts.Seconds}с";
            logger.Info($"Затраченное время: {ts.Hours}ч:{ts.Minutes}м:{ts.Seconds}с");

        }

        internal void MyTask(object arg)
        {
            string ft = (string)arg;

            logger.Info($"MyTask: CurrentId {Task.CurrentId} with ManagedThreadId {Thread.CurrentThread.ManagedThreadId} запущен, имя файла {ft}" + Environment.NewLine);

            var dict = DictSringFromFile(ft);
            var resultT = TaskTanslateString3(dict);
            
            //bc.TryAdd(myClass);
            MyClass myClass = new MyClass(ft, resultT.Result);

            if(myClass.Dict.ContainsValue("Error404"))
            {
                lock (locker)
                {
                    Invoke(new Action(() =>
                    {
                        ConsoleTB.AppendText($"Ошибка в получении перевода для файла: {myClass.FileName}" + Environment.NewLine);
                        ConsoleTB.AppendText($"Программе для перевода необходим доступ в сеть Интернет" + Environment.NewLine);
                    }));
                }
                return;
            }


            if(checkBoxOutConsole.Checked)
            {
                lock (locker)
                {
                    Invoke(new Action(() =>
                    {
                        ConsoleTB.AppendText($"файл: {myClass.FileName}" + Environment.NewLine);
                    }));

                    foreach (var item in myClass.Dict)
                    {
                        Invoke(new Action(() =>
                        {
                            //ConsoleTB.AppendText($"{item.Key}" + Environment.NewLine);
                            ConsoleTB.AppendText($"{item.Value}" + Environment.NewLine);
                            ConsoleTB.AppendText(Environment.NewLine);
                        }));

                    }
                }
            }

            ISaveDataProcessor saveDataProcessor = new SaveDataProcessor();
            string result = saveDataProcessor.SaveProcessData(new FileSaveDataProvider(), myClass);
            lock (locker)
            {
                Invoke(new Action(() =>
                {
                    ConsoleTB.AppendText($"{myClass.FileName} - {result}" + Environment.NewLine);
                }));
            }

            logger.Info($"MyTask: CurrentId " + Task.CurrentId + " завершен." + Environment.NewLine);
            logger.Info($"{myClass.FileName} - {result}" + Environment.NewLine);
        }

        /*internal void OutputC(BlockingCollection<MyClass> bc)
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

        }*/

        internal async Task<Dictionary<string, string>> TaskTanslateString3(Dictionary<string, string> dict)
        {
            Dictionary<string, string> dict2 = new Dictionary<string, string>();
            MethodBase m = MethodBase.GetCurrentMethod();

            logger.Info($"Характеристики работы Метода {m.ReflectedType.Name} : Task {Task.CurrentId} - Thread {Thread.CurrentThread.ManagedThreadId}" + Environment.NewLine);

            foreach (var d in dict.Keys)
            {
                dict2.Add(d, await GetAsync(Client, d, "ru"));
            }
            return dict2;
        }

        internal Dictionary<string, string> DictSringFromFile(string fileName)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            Random rnd = new Random();

            using (StreamReader reader = new StreamReader(fileName, Encoding.UTF8))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (Regex.IsMatch(line, srtWord, RegexOptions.IgnoreCase))
                    {
                        try
                        {
                            dict.Add(line, null);
                        } catch
                        {
                            dict.Add(line + $" 0A0-{rnd.Next(500)}", null);
                        }

                    }
                }

            }


            return dict;
        }


    }

    public class MyClass
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
