using Microsoft.Azure.Devices.Client;
using PRIVATE_PASSWORDS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWP_Mqtt
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MqttClient m_mqtt;

        public MainPage()
        {
            this.InitializeComponent();
            setup();
        }
        const string MQTTSendTopic = "devices/mqttdevice1/messages/events/";
        const string MQTTReceiveTopic = "devices/mqttdevice1/messages/devicebound/";
        private DeviceClient m_deviceClient;

        private async Task setup()
        {

            //m_mqtt = new MqttClient(ConnectionsMqtt.Connection, 8883, true, MqttSslProtocols.TLSv1_2);
            ////Device must be registered!
            //m_mqtt.Connect(ConnectionsMqtt.ClientId, ConnectionsMqtt.Username, ConnectionsMqtt.Password);
            //if (m_mqtt.IsConnected == false) throw new ArgumentException("Bad username/password for MQTT");
            ////m_mqtt.MqttMsgPublished += M_mqtt_MqttMsgPublished;
            ////m_mqtt.MqttMsgPublishReceived += M_mqtt_MqttMsgPublishReceived;
            ////m_mqtt.Subscribe(new string[] { MQTTReceiveTopic },new byte[] { 1 });
            //for (int i = 0; i < 10; i++)
            //{
            //    m_mqtt.Publish(MQTTSendTopic, System.Text.Encoding.UTF8.GetBytes("ABC"),1,false);
            //}
            m_deviceClient = DeviceClient.CreateFromConnectionString(ConnectionsIot.DeviceConnectionString, TransportType.Amqp_Tcp_Only);
            await m_deviceClient.SendEventAsync(new Message(System.Text.Encoding.UTF8.GetBytes("ABC")));
            waitForMessage();
        }

        private async void waitForMessage()
        {
            while(true)
            {
                var msg = await m_deviceClient.ReceiveAsync();
                if (msg!=null)
                {
                    Debug.WriteLine(System.Text.Encoding.UTF8.GetString(msg.GetBytes()));
                    await m_deviceClient.CompleteAsync(msg);
                }
            }
        }

        private void M_mqtt_MqttMsgPublishReceived(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishEventArgs e)
        {
        }

        private void M_mqtt_MqttMsgPublished(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishedEventArgs e)
        {
        }
    }
}
