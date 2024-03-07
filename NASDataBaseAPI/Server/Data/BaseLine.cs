using NASDataBaseAPI.Interfaces;

namespace NASDataBaseAPI.Server.Data
{
    public class BaseLine : IDataLine
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
    }
}
