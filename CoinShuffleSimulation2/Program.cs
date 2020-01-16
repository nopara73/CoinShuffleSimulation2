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

            alice.Broadcast("Hello P2P World!");

            Console.WriteLine("Press a key to exit...");
            Console.ReadKey();
        }
    }
}
