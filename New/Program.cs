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

            Message.Info("Начато выполнение программы!");
            do
            {
                Application app = new Application();

                app.Start();

                Message.Text("Нажмите клавишу [Escape], чтобы завершить выполнение программы.");
                isDone = Console.ReadKey().Key == ConsoleKey.Escape;
            } 
            while (!isDone);
            Message.Info("Выполнение программы завершено!");
        }
    }
}