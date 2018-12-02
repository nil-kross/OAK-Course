using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Debug {
    public enum MessageType
    {
        Text, // Уведомление о состоянии (для интерактивного режима)
        Info, // Уведомление о состоянии
        Warning, // Уведомление о возможном некорректном поведении (для интерактивного режима)
        Error // Уведомление об ошибке из-за предполагаемого, но некорректного поведения
    }

    public static class Message {
        private static Dictionary<MessageType, ConsoleColor> messageTypeColors = new Dictionary<MessageType, ConsoleColor>();

        static Message() {
            Message.messageTypeColors[MessageType.Error] = ConsoleColor.Red;
            Message.messageTypeColors[MessageType.Warning] = ConsoleColor.Yellow;
            Message.messageTypeColors[MessageType.Info] = ConsoleColor.Green;
            Message.messageTypeColors[MessageType.Text] = ConsoleColor.Gray;
        }

        public static void Text(String messageText)
        {
            Message.Show(messageText, MessageType.Text);
        }

        public static void Info(String messageText)
        {
            Message.Show(messageText, MessageType.Info);
        }
        public static void Warning(String messageText)
        {
            Message.Show(messageText, MessageType.Warning);
        }

        public static void Error(String messageText)
        {
            Message.Show(messageText, MessageType.Error);
        }

        private  static void Show(String messageText, MessageType? messageType = MessageType.Text) {
            Typewriter.Output(messageText, Message.messageTypeColors[messageType.Value]);
            Journal.Log(messageText);
        }
    }
}