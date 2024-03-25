using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
            } else
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

            return "ok";
        }

        internal bool TestBakFile(string filename)
        {
            //string[] filesBakInDir = Directory.GetFiles(filename.Remove(filename.LastIndexOf('\\')+1), maskBak, SearchOption.TopDirectoryOnly);
            //if (filesBakInDir.Contains(filename)) 
            if (!File.Exists(filename)) 
                BakFileExist = false;
             else
                BakFileExist=true;

            return BakFileExist;
        }

        internal bool CreateFileBak(string filename, string filenamebak)
        {
            try
            {
                File.Copy(filename, filenamebak);
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return true;
        }

    }
}
