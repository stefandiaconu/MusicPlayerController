using System;
using System.Text;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using UIKit;

namespace MusicPlayerController.iOS
{
    public partial class ViewController : UIViewController
    {
        static MqttFactory factory = new MqttFactory();

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Perform any additional setup after loading the view, typically from a nib.
            okButton.AccessibilityIdentifier = "okButton";
            okButton.TouchUpInside += async delegate
            {
                await Test("OK"); Console.WriteLine("OK button has been pressed");
            };

            upButton.AccessibilityIdentifier = "upButton";
            upButton.TouchUpInside += async delegate
            {
                await Test("UP"); Console.WriteLine("UP button has been pressed");
            };

            rightButton.AccessibilityIdentifier = "rightButton";
            rightButton.TouchUpInside += async delegate
            {
                await Test("RIGHT"); Console.WriteLine("RIGHT button has been pressed");
            };

            downButton.AccessibilityIdentifier = "downButton";
            downButton.TouchUpInside += async delegate
            {
                await Test("DOWN"); Console.WriteLine("DOWN button has been pressed");
            };

            leftButton.AccessibilityIdentifier = "leftButton";
            leftButton.TouchUpInside += async delegate
            {
                await Test("LEFT"); Console.WriteLine("LEFT button has been pressed");
            };


        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.		
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
