using NASDatabase.Interfaces;
using System.Text;

namespace NASDatabase.Server.Data
{
    /// <summary>
    /// Стандартный класс для строки(линии) в базе 
    /// </summary>
    public class Row : IDataRow
    {
        protected string[] Datas;
        public int ID { get; private set; }

        public void Init(int ID, params string[] datas)
        {
            Datas = datas;
            this.ID = ID;
        }

        public string[] GetData()
        {
            return Datas;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"{ID}|");
            foreach (var data in Datas)
            {
                sb.Append(data);
                sb.Append("|");
            }
            return sb.ToString();
        }
    }
}
