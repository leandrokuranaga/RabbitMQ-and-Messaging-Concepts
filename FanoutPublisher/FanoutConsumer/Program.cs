using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace FanoutConsumer
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

            //Para poder consumir é necessário criar um objeto consumer
            EventingBasicConsumer consumer = new(channel);
            consumer.Received += Consumer_Received;

            // Subscribe to the queue
            var consumerTag = channel.BasicConsume("my.queue1", false, consumer);

            Console.WriteLine("Waiting for messages, press any key to exit.");
            Console.ReadKey();
        }

        private static void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            //Rabbit changed their API in february so it needs to convert ToArray() to work
            string msg = Encoding.UTF8.GetString(e.Body.ToArray());
            Console.WriteLine(msg);

            //processed this message with this deliverytag and its ok to remove it from the queue
            //channel.BasicAck(e.DeliveryTag, false);

            //reject a msg, and last parameter put the message on queue again
            //channel.BasicNack(e.DeliveryTag, false, true);

            //reject a msg, and last parameter doesnt put the message on queue again
            channel.BasicNack(e.DeliveryTag, false, false);


        }
    }
}
