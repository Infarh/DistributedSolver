using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DistributedSolver.Domain.Base;

public static class UdpClientEx
{
    public static UdpClient WithSharedPort(this UdpClient udp, int port) => udp.WithSharedPort(IPAddress.Any, port);

    public static UdpClient WithSharedPort(this UdpClient udp, string address, int port) => udp.WithSharedPort(IPAddress.Parse(address), port);

    public static UdpClient WithSharedPort(this UdpClient udp, IPAddress address, int port)
    {
        udp.ExclusiveAddressUse = false;
        udp.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        udp.Client.Bind(new IPEndPoint(address, port));

        return udp;
    }
}
