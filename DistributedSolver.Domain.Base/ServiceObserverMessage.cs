using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace DistributedSolver.Domain.Base;

public readonly record struct ServiceObserverMessage(
    [property: JsonPropertyName("t")] DateTime Time,
    [property: JsonPropertyName("msg")] string Message)
{
    public ServiceObserverMessage(string Message) : this(DateTime.UtcNow, Message) { }

    private static readonly JsonSerializerOptions __JsonOpt = new()
    {
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
    };

    public string ToJSON() => JsonSerializer.Serialize(this, __JsonOpt);

    public byte[] ToByteArray()
    {
        using var stream = new MemoryStream();
        JsonSerializer.Serialize(stream, this, __JsonOpt);
        return stream.ToArray();
    }

    public async ValueTask<byte[]> ToByteArrayAsync(CancellationToken Cancel = default)
    {
        await using var stream = new MemoryStream();
        await JsonSerializer.SerializeAsync(stream, this, __JsonOpt, Cancel).ConfigureAwait(false);
        return stream.ToArray();
    }

    public static async ValueTask<ServiceObserverMessage> DeserializeAsync(byte[] data, CancellationToken Cancel = default)
    {
        using var stream = new MemoryStream(data);
        var msg = await JsonSerializer.DeserializeAsync<ServiceObserverMessage>(stream, __JsonOpt, Cancel).ConfigureAwait(false);
        return msg;
    }
}
