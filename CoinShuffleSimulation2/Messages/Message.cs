using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace CoinShuffleSimulation2.Messages
{
    public class Message : IEquatable<Message>
    {
        public Message(MessageType type, string data)
        {
            Type = type;
            Data = data;
        }

        public Message(MessageType type, IEnumerable<string> data)
        {
            Type = type;
            Data = string.Join(",,,", data);
        }

        public MessageType Type { get; }
        private string Data { get; }
        public string GetDataString() => Data;
        public IEnumerable<string> GetDataCollection() => Data.Split(",,,", StringSplitOptions.RemoveEmptyEntries);

        public override string ToString()
        {
            return $"{Type}:::{Data}";
        }
        public static Message Parse(string message)
        {
            var parts = message.Split(":::");
            return new Message(Enum.Parse<MessageType>(parts[0]), parts[1]);
        }

        public override bool Equals(object obj) => Equals(obj as Message);

        public bool Equals(Message other) => this == other;

        public override int GetHashCode() => Type.GetHashCode() ^ Data.GetHashCode();

        public static bool operator ==(Message x, Message y) => y?.Type == x?.Type && y?.Data == x?.Data;

        public static bool operator !=(Message x, Message y) => !(x == y);
    }
}
