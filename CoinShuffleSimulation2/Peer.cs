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
        private Script OutputScript => PublicKey.ScriptPubKey;
        private List<PubKey> AllPubKeys { get; } = new List<PubKey>();
        private List<string> Onions { get; } = new List<string>();

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
                var pubKey = new PubKey(message.GetDataString());
                AllPubKeys.Add(pubKey);

                if (AllPubKeys.Count == RequiredPeerCount)
                {
                    // every participant (say participant i in a predefined shuffling
                    // order) uses the encryption keys of every participant j > i to create a layered
                    // encryption of her output address.
                    string onion = OnionEncrypt(AllPubKeys, OutputScript.ToString());
                    Broadcast(new Message(MessageType.Onions, onion));
                }
            }
            else if (message.Type == MessageType.Onions)
            {
                var onions = message.GetDataCollection();
                if (onions.Count() == 1)
                {
                    Onions.Add(onions.Single());

                    if (Onions.Count == RequiredPeerCount)
                    {
                        if (SecretKey.CanDecrypt(Onions.First()))
                        {
                            var stripped = Decrypt(SecretKey, Onions).ToList();
                            stripped.Shuffle();
                            Broadcast(new Message(MessageType.Onions, stripped));
                        }
                    }
                }
                else if (SecretKey.CanDecrypt(onions.First()))
                {
                    var stripped = Decrypt(SecretKey, onions).ToList();
                    stripped.Shuffle();

                    if (NBitcoinHelpers.IsScript(stripped.First()))
                    {
                        Broadcast(new Message(MessageType.ShuffledScripts, stripped));
                    }
                    else
                    {
                        Broadcast(new Message(MessageType.Onions, stripped));
                    }
                }
            }
        }

        private static IEnumerable<string> Decrypt(Key secretKey, IEnumerable<string> onions)
        {
            foreach (var onion in onions)
            {
                yield return secretKey.Decrypt(onion);
            }
        }

        private static string OnionEncrypt(IEnumerable<PubKey> pubKeys, string message)
        {
            string onion = null;
            foreach (var pk in pubKeys.OrderBy(x => x.ToString()))
            {
                if (onion == null)
                {
                    onion = pk.Encrypt(message);
                }
                else
                {
                    onion = pk.Encrypt(onion);
                }
            }

            return onion;
        }
    }
}