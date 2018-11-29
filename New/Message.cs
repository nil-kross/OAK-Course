using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course {
    public enum MessageType {
        Error,
        Warning,
        Info,
        Notification
    }

    public static class Message {
        private static Dictionary<MessageType, ConsoleColor> messageTypeColors = new Dictionary<MessageType, ConsoleColor>();

        static Message() {
            Message.messageTypeColors[MessageType.Error] = ConsoleColor.Red;
            Message.messageTypeColors[MessageType.Warning] = ConsoleColor.Yellow;
            Message.messageTypeColors[MessageType.Info] = ConsoleColor.Blue;
            Message.messageTypeColors[MessageType.Notification] = ConsoleColor.Gray;
        }

        public static void Show(String messageText, MessageType? messageType = MessageType.Notification) {
            Message.ShowWithColor(" [", ConsoleColor.White);
            Message.ShowWithColor(DateTime.Now.ToString("HH:mm.ss"), ConsoleColor.DarkGray);
            Message.ShowWithColor("] ", ConsoleColor.White);
            Message.ShowWithColor(messageText + Environment.NewLine, Message.messageTypeColors[messageType.Value]);
        }

        private static void ShowWithColor(String messageText, ConsoleColor consoleColor) {
            Console.ForegroundColor = consoleColor;
            Console.Write(messageText);
        }
    }
}