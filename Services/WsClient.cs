using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;

namespace Quiz.Services;

public class WsClient
{
    private ClientWebSocket _ws = new();
    public event Action<string, JsonElement>? OnMessageReceived;
    private CancellationTokenSource? _cts;

    public async Task ConnectAsync(string key)
    {
        _ws = new ClientWebSocket();
        _cts = new CancellationTokenSource();
        var url = $"ws://10.103.150.110:4100/ws/{key}";
        try
        {
            await _ws.ConnectAsync(new Uri(url), _cts.Token);
            _ = Task.Run(() => ReceiveLoop(_cts.Token), _cts.Token);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error conectando WS: {ex.Message}");
        }
    }

    public async Task DisconnectAsync()
    {
        if (_ws.State == WebSocketState.Open)
        {
            await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Cerrando", CancellationToken.None);
        }
        _cts?.Cancel();
    }

    private async Task ReceiveLoop(CancellationToken ct)
    {
        var buffer = new byte[8192];
        while (_ws.State == WebSocketState.Open && !ct.IsCancellationRequested)
        {
            try
            {
                var result = await _ws.ReceiveAsync(new ArraySegment<byte>(buffer), ct);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "closed", ct);
                    break;
                }

                var text = Encoding.UTF8.GetString(buffer, 0, result.Count);
                var doc = JsonDocument.Parse(text);
                
                if (doc.RootElement.TryGetProperty("event", out var eventProp) && doc.RootElement.TryGetProperty("data", out var dataProp))
                {
                    OnMessageReceived?.Invoke(eventProp.GetString() ?? "", dataProp);
                }
            }
            catch
            {
                break;
            }
        }
    }
}
