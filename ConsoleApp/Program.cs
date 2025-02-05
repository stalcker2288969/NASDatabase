using System.Reflection;
using NASDataBaseAPI.Data.DataTypesInColumn;
using NASDataBaseAPI.Interfaces;
using NASDataBaseAPI.Server.Data;
using NASDataBaseAPI.Server.Data.DataBaseSettings;

class Program
{
    static void Main(string[] agrs)
    {
        DatabaseManager DBM = new DatabaseManager();//Создание экземпляра DataBaseManager
        Table DB;
        
        var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ConsoleInfo");
    
        if (Directory.Exists(path))//если папка есть, то загружаем БД
        {
            DB = DBM.LoadDB<Table>(path, LoadKey());
        }
        else
        {
            DatabaseSettings DBS = new DatabaseSettings("ConsoleInfo", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), 2);//Передаем настройки
            SaveKey(DBS.Key);//Сохраняем ключ
            DB = DBM.CreateDatabase(DBS);
            
            DB.RenameColumn(0, "Names");
            DB.RenameColumn(1, "Old");
            DB.ChengTypeInColumn(1, DataTypesInColumns.Int);
        }

        while (true) // Заполнение БД
        {
            Person newPerson = new Person()
            {
                Name = Console.ReadLine(),
                Old = int.Parse(Console.ReadLine())
            };
            
            DB.AddData(newPerson);
            
            Console.WriteLine(DB.PrintBase());
        }
    
    }

    static string LoadKey(){
        return File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "key.txt"));
    }
    static void SaveKey(string Key){
        File.WriteAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),"key.txt"), Key);
    }
 
}

class Person : IDataRows
{
    public int Old;
    public string Name;
    public int ID { get; private set; }

    public string[] GetData()
    {
        return new string[] { Name, Old.ToString() };
    }

    public void Init(int ID, params string[] datas)
    {
        Old = int.Parse(datas[1]);
        Name = datas[0];
        this.ID = ID;
    }
}

class Table2 : IDataRows
{
    public int _ID;
    public string Name;
    public int Old;
    public int Mode;
    
    public int ID { get; private set; }

    public string[] GetData()
    {
        return new string[] { Name, Old.ToString() };
    }

    public void Init(int ID, params string[] datas)
    {
        Old = int.Parse(datas[1]);
        Name = datas[0];
        this.ID = ID;
    }
}