using System;

namespace CoinShuffleSimulation2
{
    class Program
    {
        static void Main(string[] args)
        {
            var alice = new Peer("Alice");
            var bob = new Peer("Bob");
            alice.Connect(bob);
            var satoshi = new Peer("Satoshi");
            satoshi.Connect(bob);

            // Tick.
            // Announcement. Every participant generates a fresh ephemeral encryptiondecryption key pair, and broadcasts the resulting public encryption key.
            alice.BroadcastPubKey();
            bob.BroadcastPubKey();
            satoshi.BroadcastPubKey();

            Console.WriteLine("Press a key to exit...");
            Console.ReadKey();
        }
    }
}
