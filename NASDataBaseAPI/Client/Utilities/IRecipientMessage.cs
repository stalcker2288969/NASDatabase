using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASDatabase.Client.Utilities
{
    public interface IRecipientMessage
    {
        /// <summary>
        /// Слушает сообщение с сети
        /// </summary>
        /// <returns></returns>
        string Listen();
    }
}
