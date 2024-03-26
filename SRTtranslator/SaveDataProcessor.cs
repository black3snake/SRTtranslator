namespace SRTtranslator
{
    internal class SaveDataProcessor : ISaveDataProcessor
    {
        public string SaveProcessData(ISaveDataProvider saveDataProvider, MyClass myClass) => saveDataProvider.SaveData(myClass);
    }
}
