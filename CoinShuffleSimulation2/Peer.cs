using CoinShuffleSimulation2.Messages;
using NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoinShuffleSimulation2
{
    internal class Peer
    {
        private const int RequiredPeerCount = 3;

        public Peer(string name)
        {
            Name = name;
        }

        public event EventHandler<Message> NewMessage;

        public string Name { get; }

        private List<Peer> Connections { get; } = new List<Peer>();

        private Key SecretKey { get; } = new Key();
        private PubKey PublicKey => SecretKey.PubKey;
        private Script Script => PublicKey.ScriptPubKey;
        private List<PubKey> AllPubKeys { get; } = new List<PubKey>();

        internal void BroadcastPubKey()
        {
            Broadcast(new Message(MessageType.PublicKey, PublicKey.ToString()));
        }

        private List<Message> ProcessedMessages { get; } = new List<Message>();

        internal void Connect(Peer peer)
        {
            if (Connections.Contains(peer)) return;
            Connections.Add(peer);
            Console.WriteLine($"{Name} connected to {peer.Name}.");

            peer.NewMessage += Peer_NewMessage;
            peer.Connect(this);
        }

        internal void Broadcast(Message message)
        {
            NewMessage?.Invoke(this, message);
        }

        private void Peer_NewMessage(object sender, Message message)
        {
            if (ProcessedMessages.Contains(message)) return;
            ProcessedMessages.Add(message);
            Console.Write($"{Name} received message: ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ResetColor();

            NewMessage?.Invoke(this, message);

            if (message.Type == MessageType.PublicKey)
            {
                var pubKey = new PubKey(message.Data);
                AllPubKeys.Add(pubKey);

                if (AllPubKeys.Count == RequiredPeerCount)
                {
                    // every participant (say participant i in a predefined shuffling
                    // order) uses the encryption keys of every participant j > i to create a layered
                    // encryption of her output address.
                    string layeredEncryption = LayeredEncrypt();
                    Broadcast(new Message(MessageType.LayeredEncryption, layeredEncryption));
                }
            }
            else if (message.Type == MessageType.LayeredEncryption)
            {

            }
        }

        private string LayeredEncrypt()
        {
            string layeredEncryption = null;
            foreach (var pk in AllPubKeys.OrderBy(x => x.ToString()))
            {
                if (layeredEncryption == null)
                {
                    layeredEncryption = pk.Encrypt(PublicKey.ToString());
                }
                else
                {
                    layeredEncryption = pk.Encrypt(layeredEncryption);
                }
            }

            return layeredEncryption;
        }
    }
}