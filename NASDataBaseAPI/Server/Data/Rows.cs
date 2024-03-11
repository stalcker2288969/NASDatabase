using NASDataBaseAPI.Interfaces;
using System.Text;

namespace NASDataBaseAPI.Server.Data
{
    /// <summary>
    /// Стандартный класс для строки(линии) в базе 
    /// </summary>
    public class Rows : IDatRows
    {
        protected string[] Datas;
        public int ID { get; protected set; }

        public virtual void Init(int ID, params string[] datas)
        {
            Datas = datas;
            this.ID = ID;
        }

        public virtual string[] GetData()
        {
            return Datas;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var data in Datas)
            {
                sb.Append(data.ToString());
                sb.Append("|");
            }
            return sb.ToString();
        }
    }
}
