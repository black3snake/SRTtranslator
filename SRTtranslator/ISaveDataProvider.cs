using System.Collections.Generic;

namespace SRTtranslator
{
    internal interface ISaveDataProvider
    {
        string maskBak { get; }
        bool BakFileExist { get; set; }
        string SaveData(MyClass myClass);
    }

    internal interface ISaveDataProcessor
    {
        //MyClass MyClass { get; }
        void SaveProcessData(ISaveDataProvider saveDataProvider, MyClass myClass);
    }


}
