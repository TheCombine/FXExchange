using System;

namespace FXExchange.Services
{
    public interface IResultWriter
    {
        void WriteLine(string message);
    }

    internal class ResultWriter : IResultWriter
    {
        public void WriteLine(string message)
        {
            Console.WriteLine(message);
        }
    }
}
