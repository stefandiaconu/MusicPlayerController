using Android.App;
using Android.Widget;
using Android.OS;
using MQTTnet;
using System.Threading.Tasks;
using MQTTnet.Client;
using System.Text;
using System;

namespace MusicPlayerController.Droid
{
    [Activity(Label = "MusicPlayerController", MainLauncher = true, Icon = "@mipmap/icon", Theme = "@android:style/Theme.DeviceDefault.NoActionBar")]
    public class MainActivity : Activity
    {
        static MqttFactory factory = new MqttFactory();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button ok = FindViewById<Button>(Resource.Id.okButton);
            Button up = FindViewById<Button>(Resource.Id.upButton);
            Button right = FindViewById<Button>(Resource.Id.rightButton);
            Button down = FindViewById<Button>(Resource.Id.downButton);
            Button left = FindViewById<Button>(Resource.Id.leftButton);

            ok.Click += async delegate { Console.WriteLine("OK button has been pressed"); await Test("OK"); };
            up.Click += async delegate { Console.WriteLine("UP button has been pressed"); await Test("UP"); };
            right.Click += async delegate { Console.WriteLine("RIGHT button has been pressed"); await Test("RIGHT"); };
            down.Click += async delegate { Console.WriteLine("DOWN button has been pressed"); await Test("DOWN"); };
            left.Click += async delegate { Console.WriteLine("LEFT button has been pressed"); await Test("LEFT"); };
        }

        public static async Task Test(String text)
        {



            // Create a new MQTT client.
            var mqttClient = factory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
            .WithTcpServer("broker.hivemq.com", 1883) // Port is optional
            .Build();

            await mqttClient.ConnectAsync(options);

            var message = new MqttApplicationMessageBuilder()
            .WithTopic("testtopic/milan")
            .WithPayload(text)
            .WithExactlyOnceQoS()
            .WithRetainFlag()
            .Build();

            await mqttClient.PublishAsync(message);


            mqttClient.ApplicationMessageReceived += (s, e) =>
            {
                Console.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
                Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
                Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                Console.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
                Console.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");
                Console.WriteLine();
            };

            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();
        }
    }
}

