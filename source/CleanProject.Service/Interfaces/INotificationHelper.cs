using System;

namespace CleanProject.Service.Interfaces
{
    public interface INotificationHelper
    {
        void WriteMessage(string message);

        void WriteVerboseMessage(string message);

        void WriteColorMessage(ConsoleColor colour, string message);
    }
}
