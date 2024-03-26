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
        string SaveProcessData(ISaveDataProvider saveDataProvider, MyClass myClass);
    }


}
