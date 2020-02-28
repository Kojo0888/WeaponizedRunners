using System;
using System.Threading;
using System.Threading.Tasks;
using WeaponizedRunnersShared;

namespace WeaponizedRunnersClient_Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.Title = "Weaponized runners test client";
                Client client = new Client();
                client.Connect("127.0.0.1", Constants.PortTCP, 10009, Constants.ServerPortUDP);
                Thread.Sleep(500);
                Task task = Task.Run(() => {
                    while (true)
                    {
                        client.ClientReceiveManager.RunAwaitingActions();
                        Thread.Sleep(50);
                    }
                });

                while (true)
                {
                    var message = Console.ReadLine();
                    client.Send.Message(client, message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            //Console.WriteLine("Finished...");
            //Console.ReadKey();
        }
    }
}
