using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Subscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter the queue name:");
            string queueName = Console.ReadLine();

            //Connection to RabbitMQ
            IConnection conn;

            //Channel to RabbitMQ
            IModel channel;

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

            //consuming
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, e) =>
              {
                  string message = Encoding.UTF8.GetString(e.Body.ToArray());
                  Console.WriteLine("Subscriber [" + queueName +"] Message: "+ message);
              };

            var consumerTag = channel.BasicConsume(queueName, true, consumer);

            Console.WriteLine($"Subscribed to the queue '{queueName}'. press a key to exit");
            Console.ReadKey();

            channel.Close();
            conn.Close();
        }
    }
}
