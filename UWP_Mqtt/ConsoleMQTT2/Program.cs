using Microsoft.Azure.Devices.Client;
using PRIVATE_PASSWORDS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace ConsoleMQTT2
{
    class Program
    {
        private static DeviceClient m_deviceClient;

        static void Main(string[] args)
        {
            //testDeviceClientIotSDK();
            testM2Mqtt();
        }

        const string MQTTSendTopic = "devices/mqttdevice1/messages/events/";
        const string MQTTReceiveTopic = "devices/mqttdevice1/messages/devicebound/#";
        private static MqttClient m_mqtt;

        private static void testM2Mqtt()
        {
            m_mqtt = new MqttClient(ConnectionsMqtt.Connection, 8883, true, MqttSslProtocols.TLSv1_2,null,null);
            m_mqtt.MqttMsgPublishReceived += M_mqtt_MqttMsgPublishReceived;
            m_mqtt.Subscribe(new string[] { MQTTReceiveTopic },new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });
            m_mqtt.Connect(ConnectionsMqtt.ClientId, ConnectionsMqtt.Username, ConnectionsMqtt.Password);
            if (m_mqtt.IsConnected == false) throw new ArgumentException("Bad username/password for MQTT");
            m_mqtt.Publish(MQTTSendTopic, System.Text.Encoding.UTF8.GetBytes("ABC"));
            Console.WriteLine("Waiting");
            Console.ReadLine();
        }

        private static void M_mqtt_MqttMsgPublishReceived(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishEventArgs e)
        {
            Console.WriteLine(System.Text.Encoding.UTF8.GetString(e.Message));
        }

        private static void testDeviceClientIotSDK()
        {
            Task.Run(async () =>
            {
                try
                {
                    m_deviceClient = DeviceClient.CreateFromConnectionString(ConnectionsIot.DeviceConnectionString, TransportType.Mqtt);
                    await m_deviceClient.SendEventAsync(new Message(System.Text.Encoding.UTF8.GetBytes("ABC")));
                    waitForMessage();
                    Console.WriteLine("Waiting");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                Console.ReadLine();
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
                    Console.WriteLine(str);
                    await m_deviceClient.CompleteAsync(msg);
                }
            }
        }
    }
}
