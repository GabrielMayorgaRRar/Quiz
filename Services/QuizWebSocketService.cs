using System;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Quiz.Services;

public class QuizWebSocketService
{
    private ClientWebSocket? _ws;

    // El ViewModel se suscribe a este evento para reaccionar
    // a lo que manda el servidor en tiempo real
    public event Action<string, JsonElement>? EventReceived;

    public async Task ConnectAsync(string roomKey)
    {
        _ws = new ClientWebSocket();
        await _ws.ConnectAsync(
            new Uri($"ws://localhost:4100/ws/{roomKey}"),
            CancellationToken.None
        );

        // Escucha mensajes en segundo plano
        _ = Task.Run(ListenAsync);
    }

    private async Task ListenAsync()
    {
        var buffer = new byte[8192];

        while (_ws?.State == WebSocketState.Open)
        {
            try
            {
                var result = await _ws.ReceiveAsync(buffer, CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                    break;

                var json = Encoding.UTF8.GetString(buffer, 0, result.Count);
                var msg = JsonSerializer.Deserialize<JsonElement>(json);

                var eventName = msg.GetProperty("event").GetString() ?? "";
                var data = msg.GetProperty("data");

                // Avisa al ViewModel qué evento llegó y con qué datos
                EventReceived?.Invoke(eventName, data);
            }
            catch
            {
                break;
            }
        }
    }

    public async Task DisconnectAsync()
    {
        if (_ws?.State == WebSocketState.Open)
            await _ws.CloseAsync(
                WebSocketCloseStatus.NormalClosure,
                "bye",
                CancellationToken.None
            );
    }
}