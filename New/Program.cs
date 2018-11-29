using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Course
{
    class Program
    {
        static void Main(string[] args)
        {
            var isDone = false;

            do
            {
                Application app = new Application();

                app.Start();

                Message.Show("Нажмите клавишу [Escape], чтобы завершить выполнение программы.", MessageType.Notification);
                isDone = Console.ReadKey().Key == ConsoleKey.Escape;
            } 
            while (!isDone);
        }
    }
}