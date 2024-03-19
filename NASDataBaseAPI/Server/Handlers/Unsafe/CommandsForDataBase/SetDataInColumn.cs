using NASDatabase.Client;
using NASDatabase.Client.Utilities;
using NASDatabase.Interfaces;
using NASDatabase.Server.Data;
using System;

namespace NASDatabase.Server.Handlers.Unsafe.CommandsForDataBase
{
    internal class SetDataInColumn : CommandHandler
    {
        private Action<string, ItemData> _action;
        private IDataConverter _converter;
        private string _columnName;
        private ItemData _DestroyedData;

        public SetDataInColumn(IDataConverter dataConverter, Action<string, ItemData> handler)
        {
            _action = handler;
            _converter = dataConverter;
        }

        public override void SetData(string data)
        {
            var d = data.Split(BaseCommands.SEPARATION.ToCharArray());

            _columnName = d[0];

            _DestroyedData = _converter.GetItemData(d[1]);
        }

        public override string Use()
        {
            _action?.Invoke(_columnName, _DestroyedData);
            return BaseCommands.DONE;
        }
    }
}
