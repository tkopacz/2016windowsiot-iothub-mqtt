using Microsoft.Azure.Devices.Client;
using PRIVATE_PASSWORDS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleMQTT
{
    class Program
    {
        private static DeviceClient m_deviceClient;

        static void Main(string[] args)
        {
            Task.Run(async () => {
                try
                {
                    m_deviceClient = DeviceClient.CreateFromConnectionString(ConnectionsIot.DeviceConnectionString, TransportType.Mqtt);
                    await m_deviceClient.SendEventAsync(new Message(System.Text.Encoding.UTF8.GetBytes("ABC")));
                    waitForMessage();
                    Console.WriteLine("Waiting");
                    Console.ReadLine();
                } catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }).Wait();
        }

        private static async void waitForMessage()
        {
            while (true)
            {
                var msg = await m_deviceClient.ReceiveAsync();
                if (msg != null)
                {
                    var str = System.Text.Encoding.UTF8.GetString(msg.GetBytes());
                    Debug.WriteLine(str);
                    Console.WriteLine(str);
                    await m_deviceClient.CompleteAsync(msg);
                }
            }
        }
    }
}
