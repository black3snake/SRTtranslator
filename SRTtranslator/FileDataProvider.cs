using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SRTtranslator
{
    internal class FileSaveDataProvider : ISaveDataProvider
    {
        private string mask = "*.bak";
        private bool bakfileexist;
        public string maskBak { get => mask; }
        public bool BakFileExist
        {
            get => bakfileexist; set
            {
                bakfileexist = value;
            }
        }

        public string SaveData(MyClass myClass)
        {
            string strF = string.Empty;
            string strBakFile = myClass.FileName.Remove(myClass.FileName.LastIndexOf('.') + 1) + "bak";
            bakfileexist = TestBakFile(strBakFile);
            if (!bakfileexist)
            {
                bakfileexist = CreateFileBak(myClass.FileName, strBakFile);
                if (bakfileexist)
                    Form1.logger.Info($"Файлу {myClass.FileName}, создана копия {strBakFile}");
            }
            else
            {
                Form1.logger.Info($"Файлу {myClass.FileName}, уже существует копия {strBakFile}");
            }

            using (StreamReader readerS = new StreamReader(myClass.FileName, Encoding.UTF8))
            {
                strF = readerS.ReadToEnd();

            }

            foreach (var item in myClass.Dict)
            {
                strF = Regex.Replace(strF, item.Key, item.Value, RegexOptions.Multiline);
            }
            FileInfo fileInfoSRC = new FileInfo(myClass.FileName);
            FileInfo fileInfoBAK = new FileInfo(strBakFile);

            if (fileInfoSRC.Length == fileInfoBAK.Length)
            {
                using (StreamWriter writerS = new StreamWriter(myClass.FileName, false, Encoding.UTF8))
                {
                    try
                    {
                        writerS.Write(strF);
                        Form1.logger.Info($"Файл {myClass.FileName}, перезаписан с новыми данными перевода.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                return "Переведен и записан";
            }
            else
            {
                Form1.logger.Info($"Файлы {myClass.FileName} и {strBakFile} имеют разную длинну, возможно они уже переведены.");
            }

            return "Был переведен ранее или что-то пошло не так :-)";
        }

        internal bool TestBakFile(string filename)
        {
            if (!File.Exists(filename))
                BakFileExist = false;
            else
                BakFileExist = true;

            return BakFileExist;
        }

        internal bool CreateFileBak(string filename, string filenamebak)
        {
            try
            {
                File.Copy(filename, filenamebak);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return true;
        }

    }
}
