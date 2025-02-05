using NASDataBaseAPI;
using NASDataBaseAPI.Server.Data;
using NASDataBaseAPI.Server.Data.DataBaseSettings;

namespace UnitTests;

public class FastStart
{
    private string _path = "D:\\TestConsole";//путь до проекта
    
    [Fact]
    public void InitTable_3()
    {
        var DBM = new DatabaseManager();
        
        Table DB;

        if (Directory.Exists(_path))//если папка есть, то загружаем БД
        {
            DB = DBM.LoadDB(_path, LoadKey());
        }
        else
        {
            DB = DBM.CreateDatabase<Table>(new DatabaseSettings("TestConsole", "D:\\", 3));
            SaveKey(DB.Settings.Key);
        }
    }

    private void SaveKey(string settingsKey)
    {
        File.WriteAllText(Path.Combine(_path,"Key.txt"), settingsKey);
    }

    private string LoadKey()
    {
        return File.ReadAllText(Path.Combine(_path,"Key.txt"));
    }
}