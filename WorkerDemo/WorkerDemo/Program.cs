using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace WorkerDemo
{
    class Program
    {
        //Connection to RabbitMQ
        static IConnection conn;

        //Channel to RabbitMQ
        static IModel channel;
        static void Main(string[] args)
        {
            Console.WriteLine("Enter the name of worker");
            string workerNamer = Console.ReadLine();

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

            channel.BasicQos(0, 1, false);

            //Creating consumer
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, e) =>
             {
                 string message = Encoding.UTF8.GetString(e.Body.ToArray());
                 int durationInSeconds = Int32.Parse(message);

                 Console.WriteLine("["+workerNamer+"]" + "Task started. Duration: "+ durationInSeconds);
                 Thread.Sleep(durationInSeconds * 1000);

                 Console.WriteLine("Task finished");

                 channel.BasicAck(e.DeliveryTag, false);
             };

            //Subscribe to the queue
            var consumerTag = channel.BasicConsume("my.queue1", false, consumer);

            Console.WriteLine("Waiting for messages. press a key to exit.");
            Console.ReadKey();

            channel.Close();
            conn.Close();

        }
    }
}
