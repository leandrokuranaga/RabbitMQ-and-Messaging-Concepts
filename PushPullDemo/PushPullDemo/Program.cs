using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PushPullDemo
{
    class Program
    {
        //Connection to RabbitMQ
        static IConnection conn;

        //Channel to RabbitMQ
        static IModel channel;
        static void Main(string[] args)
        {
            //Connection factory to RabbitMQ
            ConnectionFactory factory = new();
            factory.HostName = "localhost";
            factory.VirtualHost = "/";
            factory.Port = 5672;
            factory.UserName = "guest";
            factory.Password = "guest";

            //Criando conexão
            conn = factory.CreateConnection();
            //Criando canal 
            channel = conn.CreateModel();

            //readMessagesWithPushModel();
            readMessagesWithPullModel();

            channel.Close();
            conn.Close();

        }

        private static void readMessagesWithPushModel()
        {
            EventingBasicConsumer consumer = new(channel);
            consumer.Received += (sender, e) =>
              {
                  string message = Encoding.UTF8.GetString(e.Body.ToArray());
                  Console.WriteLine(message);
              };

            string consumerTag = channel.BasicConsume("my.queue1", true, consumer);

            Console.WriteLine("Press any key");
            Console.ReadKey();

            //unsubscribing to the tag
            channel.BasicCancel(consumerTag);
        }

        private static void readMessagesWithPullModel()
        {
            Console.WriteLine("Reading messages from queue. press 'e' to exit");

            while (true)
            {
                Console.WriteLine("Trying to get a message from the queue...");

                BasicGetResult result = channel.BasicGet("my.queue1", true);

                if(result != null)
                {
                    string message = Encoding.UTF8.GetString(result.Body.ToArray());
                    Console.WriteLine(message);
                }

                if (Console.KeyAvailable)
                {
                    var keyInfo = Console.ReadKey();

                    if (keyInfo.KeyChar == 'e')
                    {
                        return;
                    }
                }

                Thread.Sleep(2000);
            }

        }
    }
}
