using System;
using System.Text;
using RabbitMQ.Client;

namespace FanoutPublisher
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

            //Creating exchange
            channel.ExchangeDeclare(
                "ex.fanout",
                "fanout",
                true,
                false,
                null);

            //Creating queues
            channel.QueueDeclare(
                "my.queue1",
                true,
                false,
                false,
                null);

            channel.QueueDeclare(
               "my.queue2",
               true,
               false,
               false,
               null);

            //Linkando as filas com as exchanges
            channel.QueueBind("my.queue1", "ex.fanout", "");
            channel.QueueBind("my.queue2", "ex.fanout", "");

            //Publicando mensagens
            channel.BasicPublish(
                "ex.fanout",
                "",
                null, 
                Encoding.UTF8.GetBytes("Message 1")
                );

            channel.BasicPublish(
                "ex.fanout",
                "",
                null,
                Encoding.UTF8.GetBytes("Message 2")
                );

            Console.WriteLine("Press a key to exit");
            Console.ReadKey();

            //Deletando exchanges e queues
            channel.QueueDelete("my.queue1");
            channel.QueueDelete("my.queue2");
            channel.ExchangeDelete("ex.fanout");

            //Fechando conexões
            channel.Close();
            conn.Close();
        }
    }
}
