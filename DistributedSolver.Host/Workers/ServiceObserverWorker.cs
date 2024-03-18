
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

using DistributedSolver.Domain.Base;

namespace DistributedSolver.Host.Workers;

public class ServiceObserverWorker(
    IConfiguration Configuration,
    ILogger<ServiceObserverWorker> Logger
    ) : BackgroundService
{
    private readonly int _Port = Configuration.GetSection("ServiceObserver").GetValue("Port", 8887);

    protected override async Task ExecuteAsync(CancellationToken StoppingToken)
    {
        var listen_task = ListenAsync(_Port, StoppingToken);

        Logger.LogInformation("Listen port: {port}", _Port);

        await listen_task.ConfigureAwait(false);
    }

    private async Task ListenAsync(int port, CancellationToken Cancel)
    {
        using var udp = new UdpClient(_Port)
        {
            EnableBroadcast = true,
        };

        //udp.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        //udp.ExclusiveAddressUse = false;
        //udp.Client.Bind(new IPEndPoint(IPAddress.Any, _Port));

        Task? processing_data_task = null;

        while (true)
        {
            Cancel.ThrowIfCancellationRequested();

            if (processing_data_task is { })
                await processing_data_task.ConfigureAwait(false);

            var (data, address) = await udp.ReceiveAsync(Cancel).ConfigureAwait(false);

            Logger.LogInformation("Received data (len:{length}) from: {address}", data.Length, address);

            processing_data_task = ProcessDataAsync(udp, data, address, Cancel);
        }
    }

    private async Task ProcessDataAsync(UdpClient client, byte[] data, IPEndPoint address, CancellationToken Cancel)
    {
        ServiceObserverMessage message;
        try
        {
            message = await ServiceObserverMessage.DeserializeAsync(data, Cancel);
        }
        catch (JsonException error)
        {
            Logger.LogWarning("Ошибка десериализации сообщения {error}.\r\nРазмер массива данных {DataLength}\r\nСообщение {Message}",
                error,
                data.Length,
                Encoding.UTF8.GetString(data));
            throw;
        }

        await ProcessMessageAsync(client, message, address.Address, Cancel);
    }

    private async ValueTask ProcessMessageAsync(UdpClient client, ServiceObserverMessage message, IPAddress address, CancellationToken Cancel)
    {
        switch (message.Message)
        {
            default:
                break;

            case "request":
                var msg = new ServiceObserverMessage("response");
                var bytes = await msg.ToByteArrayAsync(Cancel).ConfigureAwait(false);
                await client.SendAsync(bytes, new IPEndPoint(address, _Port), Cancel).ConfigureAwait(false);
                break;
        }
    }
}
