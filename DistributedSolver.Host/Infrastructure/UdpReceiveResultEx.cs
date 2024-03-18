using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DistributedSolver.Host.Infrastructure;

internal static class UdpReceiveResultEx
{
    public static void Deconstruct(this UdpReceiveResult result, out byte[] data, out IPEndPoint Address) => (data, Address) = (result.Buffer, result.RemoteEndPoint);
}
