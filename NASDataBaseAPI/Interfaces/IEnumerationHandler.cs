using NASDataBaseAPI.Server.Data;
using System;

namespace NASDataBaseAPI.Interfaces 
{
    public interface IEnumerationHandler
    {
        void ForLine(Action<BaseLine> Handler);
        void ForLine<T>(Action<T> Handler) where T : IDataLine;
        void ForBoxes(string InColumn, Action<ItemData> Handler);
        void ForBoxes(AColumn inAColumn, Action<ItemData> Handler);
    }
}
