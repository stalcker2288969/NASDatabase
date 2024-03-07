using NASDataBaseAPI.Client;
using NASDataBaseAPI.Client.Utilities;
using NASDataBaseAPI.Interfaces;
using NASDataBaseAPI.Server.Data;
using System;

namespace NASDataBaseAPI.Server.Handlers.Unsafe.CommandsForDataBase
{
    public class SetDataInColumn : ServerCommand
    {
        private Action<string, ItemData> _action;
        private IDataConverter _converter;
        private string _columnName;
        private ItemData _DestroyedData;

        public SetDataInColumn(IDataConverter dataConverter, Action<string, ItemData> Handler)
        {
            _action = Handler;
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
