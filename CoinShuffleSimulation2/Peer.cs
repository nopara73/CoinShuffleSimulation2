using System;
using System.Collections.Generic;

namespace CoinShuffleSimulation2
{
    internal class Peer
    {
        public Peer(string name)
        {
            Name = name;
        }

        public event EventHandler<string> NewMessage;

        public string Name { get; }

        private List<Peer> Connections { get; } = new List<Peer>();
        private List<string> ProcessedMessages { get; } = new List<string>();

        internal void Connect(Peer peer)
        {
            if (Connections.Contains(peer)) return;
            Connections.Add(peer);
            Console.WriteLine($"{Name} connected to {peer.Name}.");

            peer.NewMessage += Peer_NewMessage;
            peer.Connect(this);
        }

        internal void Broadcast(string message)
        {
            NewMessage?.Invoke(this, message);
        }

        private void Peer_NewMessage(object sender, string message)
        {
            if (ProcessedMessages.Contains(message)) return;
            ProcessedMessages.Add(message);
            Console.Write($"{Name} received message: ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{message}");
            Console.ResetColor();

            NewMessage?.Invoke(this, message);
        }
    }
}