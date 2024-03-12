using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASDatabase.Client.Utilities
{
    public interface ISenderCommands
    {
        /// <summary>
        /// Отправка сообщения по сети
        /// </summary>
        /// <param name="message"></param>
        void Push(string message);
    }
}
