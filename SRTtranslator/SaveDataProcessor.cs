using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRTtranslator
{
    internal class SaveDataProcessor : ISaveDataProcessor
    {
        //public MyClass MyClass => throw new NotImplementedException();

        public void SaveProcessData(ISaveDataProvider saveDataProvider, MyClass myClass)
        {
            saveDataProvider.SaveData(myClass);

        }
    }
}
