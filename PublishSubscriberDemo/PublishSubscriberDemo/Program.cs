using System;
using System.Text;
using RabbitMQ.Client;

namespace PublishSubscriberDemo
{
    class Program
    {
        static void Main(string[] args)
        {
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

            while (true)
            {
                Console.WriteLine("Enter message: ");
                string message = Console.ReadLine();

                if (message == "exit")
                    break;

                channel.BasicPublish("ex.fanout", "", null, Encoding.UTF8.GetBytes(message));

                channel.Close();
                conn.Close();
            }
        }
    }
}
