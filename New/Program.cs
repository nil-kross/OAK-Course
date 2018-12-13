using Course.Debug;
using System;

namespace Course {
    class Program {
        static void Main(String[] args) {
            var isDone = false;

            Message.Info("Начато выполнение программы!");
            do {
                Application app = new Application();

                app.Start();

                Message.Text("Нажмите клавишу [Escape], чтобы завершить выполнение программы.");
                isDone = ConsoleKey.Escape == Input.Key("Нажмите любую другую клавишу для повторного выполнения программы..");
            }
            while (!isDone);
            Message.Info("Выполнение программы завершено!");
        }
    }
}