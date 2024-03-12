using NASDatabase.Interfaces;
using System;

namespace NASDatabase.Client.Handleres.Base
{
    public class ERROR : CommandHandler
    {
        private string _data;

        public override void SetData(string data)
        {
            _data = data;
        }

        public override string Use()
        {
            throw new ArgumentException(Client.ErrorOnServerExceptionText + _data);
        }
    }
}
