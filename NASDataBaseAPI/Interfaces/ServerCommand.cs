using System;
using System.Xml.Serialization;

namespace NASDataBaseAPI.Interfaces
{
    public abstract class ServerCommand
    {
        public abstract string Use();
        public virtual void SetData(string data) { }
    }
}