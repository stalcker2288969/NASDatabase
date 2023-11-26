using NASDataBaseAPI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASDataBaseAPI.SmartSearchSettings
{
    internal class NotInRange : ISearch
    {
        public List<int> SearchID(Table TableParams, Table In, string Params)
        {
            List<int> data = new List<int>();

            switch (In.DataType.Name)
            {
                case "Text":
                    ItemData[] datas = In.GetDatas();

                    foreach (ItemData item in datas)
                    {
                        if (!item.Data.Contains(Params))
                        {
                            data.Add(item.IDInTable);
                        }
                    }
                    break;
                case "Int":

                    string[] NumbersText = Params.Split(',');
                    int[] ints = new int[NumbersText.Length];
                    for (int i = 0; i < NumbersText.Length; i++)
                    {
                        ints[i] = int.Parse(NumbersText[i]);
                    }
                    datas = In.GetDatas();
                    foreach (ItemData item in datas)
                    {
                        bool result = true;
                        foreach (int i in ints)
                        {
                            if (int.Parse(item.Data) == i)
                            {
                                result = false; break;
                            }
                        }
                        if (result == true)
                        {
                            data.Add(item.IDInTable);
                        }
                    }
                    break;
                case "Float":
                    datas = In.GetDatas();
                    NumbersText = Params.Split(',');
                    decimal[] decs = new decimal[NumbersText.Length];
                    for (int i = 0; i < NumbersText.Length; i++)
                    {
                        decs[i] = int.Parse(NumbersText[i]);
                    }
                    datas = In.GetDatas();
                    foreach (ItemData item in datas)
                    {
                        bool result = true;
                        foreach (int i in decs)
                        {
                            if (decimal.Parse(item.Data) == i)
                            {
                                result = false; break;
                            }
                        }
                        if (result == true)
                        {
                            data.Add(item.IDInTable);
                        }
                    }
                    break;
                case "Time":
                    datas = In.GetDatas();
                    NumbersText = Params.Split(',');
                    datas = In.GetDatas();
                    foreach (ItemData item in datas)
                    {
                        bool result = true;
                        foreach (string i in NumbersText)
                        {
                            if (item.Data == i)
                            {
                                result = false; break;
                            }
                        }
                        if (result == true)
                        {
                            data.Add(item.IDInTable);
                        }
                    }
                    break;
            }

            return data;
        }
    }
}

