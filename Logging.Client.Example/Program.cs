using Bogus;
using MassTransit;

namespace Buffer.Service;


class Program
{
    public static string[] endpoints = { "getFacility", "auth", "exportData", "deleteObject" };
    public static string[] actions = { "Просмотр оборудования", "Вход в систему", "Удаление объекта из системы", "Выгрузка данных в xlsx" };

    public static async Task Main()
    {
        Console.WriteLine("Connection to RabbitMq...");
        var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
        {
            cfg.Host("rabbitmq", "/", h =>
            {
                h.Username("guest");
                h.Password("guest");
            });

        });

        await busControl.StartAsync();

        var fakerLog = new Faker<LogEntity>()
            .RuleForType(typeof(Guid), f => Guid.NewGuid())
            .RuleFor(u => u.endpoint, f => f.PickRandom(endpoints))
            .RuleFor(u => u.action_description, f => f.PickRandom(actions))
            .RuleFor(u => u.ip, f => f.Internet.Ip());

        var t = 1;
        Console.WriteLine("Connected to RabbitMq");
        try
        {
            while (true)
            {
                Console.WriteLine($"Start sending batch logs - {t++}");
                for (int i = 0; i < 6; i++)
                {
                    var log = fakerLog.Generate();

                    await busControl.Publish(log);
                }

                await Task.Delay(5000);
                Console.WriteLine($"Sended batch logs - {t++}");
            }
        }
        finally
        {
            await busControl.StopAsync();
        }
    }
}

