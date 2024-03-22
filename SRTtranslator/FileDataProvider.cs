using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SRTtranslator
{
    internal class FileSaveDataProvider : ISaveDataProvider
    {
        public string SaveData(MyClass myClass)
        {
            string strF = string.Empty;
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
    }
}
