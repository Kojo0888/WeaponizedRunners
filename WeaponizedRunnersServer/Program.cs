using System;
using System.Threading;
using WeaponizedRunnersShared;

namespace GameServer
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.Title = "Weaponized Runners Server";
                Server.Start(50);

                while (true)
                {
                    var message = Console.ReadLine();
                    Server.Send.Message(Server.Clients[1], message);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            Console.WriteLine("Finished...");
            Console.ReadKey();
        }
    }
}
