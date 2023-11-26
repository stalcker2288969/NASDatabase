using NASDataBaseAPI.Data;
using NASDataBaseAPI.Data.DataBaseSettings;
using NASDataBaseAPI.Data.Interfases;
using NASDataBaseAPI.Datas;
using NASDataBaseAPI.SmartSearchSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ggH
{
    internal class Program
    {
        static DataBase dataBase;
        static void Main(string[] args)
        {
            string[] Acc = new string[] { "Stalcker", "asd", "Bober", "Luser", "Nagibator", "NoNEm", 
                "KkOrs", "Nigers", "Killer228", "Peta1" };
            Random random = new Random();
            dataBase = DataBaseMenager.LoadDB("D:\\Test_2Test_2");
         
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("/----------\\");
                Console.WriteLine("? - help");
                Console.ResetColor();
                Console.Write("Comand >>");
                ParserComand(Console.ReadLine().Split(' '));
            }            
        }

        private static void ParserComand(string[] agrs)
        {
            foreach (string agr in agrs)
            {
                switch (agr)
                {
                    case "Print":
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(dataBase.PrintBase());
                        Console.ResetColor();
                        break;
                    case "Delite":
                        dataBase.RemoveDataByID((uint)Convert.ToInt32(agrs[1]));
                        break;
                    case "Add":
                        List<string> list = new List<string>();

                        for (int i = 1; i < agrs.Length; i++)
                        {
                            list.Add(agrs[i]);
                        }

                        dataBase.AddData(list.ToArray());

                        break;

                    case "AddTable":
                        dataBase.AddTable(agrs[1]);
                        break;
                    case "GetAll":
                        List<List<ItemData>> itemDatas = dataBase.GetAllDataInBaseByTableName(agrs[1], agrs[2]);
                        Console.ForegroundColor = ConsoleColor.Green;

                        foreach (List<ItemData> itemData in itemDatas)
                        {
                            Console.Write(itemData[0].IDInTable + " | ");
                            foreach (ItemData item in itemData)
                            {
                                Console.Write(item.Data + " | ");
                            }
                            Console.WriteLine();
                        }
                        Console.WriteLine($"/Общее кол-во: {itemDatas.Count}\\");
                        Console.ResetColor();
                        break;
                    case "Rename":
                        dataBase[agrs[1]].Name = agrs[2];
                        break;
                    case "GetIDS":
                        int[] ids = dataBase[agrs[1]].FindeIDS(agrs[2]);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("-----------------------");
                        foreach (int id in ids)
                        {
                            Console.WriteLine("| " + id + " |");
                        }
                        Console.WriteLine("-----------------------");
                        Console.ResetColor();
                        break;
                    case "DeliteList":
                        for (int i = 1; i < agrs.Length; i++)
                        {
                            dataBase.RemoveDataByID((uint)Convert.ToInt32(agrs[i]));
                        }
                        break;
                    case "Sort":
                        int x = int.Parse(agrs[1]);
                        Tables[] tables = new Tables[x];
                        SearchType[] searchType = new SearchType[x];
                        string[] _Params = new string[x];
                        
                        for(int i = 0; i < x; i++)
                        {
                            string[] p = agrs[i + 2].Split(',');

                            tables[i] = new Tables(p[0], GetType(p[1]), 0);
                        }

                        for(int i = x+2; i < x+x+2; i++)
                        {
                            searchType[i-x-2] = GetSearchType(agrs[i]);
                        }

                        for(int i = x+x+2; i < agrs.Length;i++)
                        {
                            _Params[i - x - x - 2] = agrs[i];
                        }

                        List<List<ItemData>> _itemDatas = dataBase.SmartSearch(tables, searchType, _Params, 1);

                        for (int i = 0; i < _itemDatas.Count; i++)
                        {
                            Console.WriteLine(_itemDatas[i][0].IDInTable + "|/*\\|" + _itemDatas[i][0].Data + "|/*\\|" + _itemDatas[i][1].Data);
                        }
                        Console.WriteLine(_itemDatas.Count);
                        break;
                    case "Stop":
                        DataBaseMenager.StopDataBase(dataBase);
                        
                        break;
                    case "?":
                        Console.WriteLine("Print - Печатать табличку");
                        Console.WriteLine("Add {список параметров}- Добавляет данные в таблицу ");
                        Console.WriteLine("Delite {ID} - Удоляет данные из таблички по id");
                        Console.WriteLine("DeliteList {IDS} - Удоляет сразу список по айдишникам(Нужно понимать что таблица с каждым удаление смещается!)");
                        Console.WriteLine("AddTable {Name} - Создает новую табличку");
                        Console.WriteLine("GetAll {NameTable} {Data} - Достает данные по параметрам");
                        Console.WriteLine("Rename {OldName} {NewName} - Переименуем столбец");
                        Console.WriteLine("GetIDS {NameTable} {Data} - получение айдишников подходящих по параметру");
                        Console.WriteLine("Sort {кол-во параметров} {Tabeles{Name,Type}} {SeatchType} {Params1,Params2...ParamsN}");
                        Console.WriteLine("Stop - выключение базы");
                        Console.WriteLine("? - Помощь");
                        break;
                }
            }
        }

        private static DataType GetType(string p) {
            DataType dataType = null;
            switch (p)
            {
                case "Int":
                    dataType = DataTypesInTable.Int;
                    break;
                case "Float":
                    dataType = DataTypesInTable.SemicolonNumbers; break;
                case "Text":
                    dataType = DataTypesInTable.Text; break;
                case "Boolean":
                    dataType = DataTypesInTable.Boolean; break;
            }
            return dataType;
        }

        private static SearchType GetSearchType(string p)
        {
            switch (p)
            {
                case "More":
                    return SearchType.More;
                    break;
                case "Less":
                    return SearchType.Less;
                    break;
                case "NotMore":
                    return SearchType.NotMore;
                    break;
                case "NotLess":
                    return SearchType.NotLess;
                    break;
                case "Equally":
                    return SearchType.Equally;
                    break;
                case "NotEqually":
                    return SearchType.NotEqually;
                    break;
                case "MoreOrEqually":
                    return SearchType.MoreOrEqually;
                    break;
                case "LessOrEqually":
                    return SearchType.LessOrEqually;
                    break;
                case "StartWith":
                    return SearchType.StartWith;
                    break;
                case "StopWith":
                    return SearchType.StopWith;
                    break;
                case "ByRange":
                    return SearchType.ByRange;
                    break;
                case "NotInRange":
                    return SearchType.NotInRange;
                    break;
                case "Multiple":
                    return SearchType.Multiple;
                    break;
                case "NotMultiple":
                    return SearchType.NotMultiple;
                    break;
                default:
                    return SearchType.Equally;
                    break;
            }
        }
    }
}
