
using System.Net;
using System.Net.Sockets;

using DistributedSolver.Domain.Base;

var msg = new ServiceObserverMessage("request");

const int port = 8887;

//using var udpClient = new UdpClient(8088);
//udpClient.JoinMulticastGroup(IPAddress.Parse("224.100.0.1"), 50);

var listner = new UdpClient().WithSharedPort("10.25.0.227", port);
listner.MulticastLoopback = false;
listner.EnableBroadcast = false;
//listner.JoinMulticastGroup(IPAddress.Parse("10.25.0.227"));

Console.WriteLine("Waiting...");
Console.ReadLine();

using var udp = new UdpClient().WithSharedPort(port);
udp.MulticastLoopback = false;
udp.EnableBroadcast = true;
//udp.JoinMulticastGroup(IPAddress.Broadcast);

var msg_bytes = await msg.ToByteArrayAsync();

await udp.SendAsync(msg_bytes, new IPEndPoint(IPAddress.Broadcast, port));


while (true)
{
    Console.WriteLine("Listen...");

    var data = await listner.ReceiveAsync();
    var msg2 = await ServiceObserverMessage.DeserializeAsync(data.Buffer);
    Console.WriteLine(msg2);

    //Console.ReadLine();
}

