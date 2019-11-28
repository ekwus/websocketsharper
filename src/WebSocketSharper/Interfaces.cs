using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using WebSocketSharper.Net;

namespace WebSocketSharper
{
    public enum ReconnectionType
    {
        /// <summary>
        /// Type used for initial connection to websocket stream
        /// </summary>
        Initial = 0,

        /// <summary>
        /// Type used when connection to websocket was lost in meantime
        /// </summary>
        Lost = 1,

        /// <summary>
        /// Type used when connection to websocket was lost by not receiving any message in given time-range
        /// </summary>
        NoMessageReceived = 2,

        /// <summary>
        /// Type used after unsuccessful previous reconnection
        /// </summary>
        Error = 3,

        /// <summary>
        /// Type used when reconnection was requested by user
        /// </summary>
        ByUser = 4
    }

    public enum DisconnectionType
    {
        /// <summary>
        /// Type used for exit event, disposing of the websocket client
        /// </summary>
        Exit = 0,

        /// <summary>
        /// Type used when connection to websocket was lost in meantime
        /// </summary>
        Lost = 1,

        /// <summary>
        /// Type used when connection to websocket was lost by not receiving any message in given time-range
        /// </summary>
        NoMessageReceived = 2,

        /// <summary>
        /// Type used when connection or reconnection returned error
        /// </summary>
        Error = 3,

        /// <summary>
        /// Type used when disconnection was requested by user
        /// </summary>
        ByUser = 4
    }

    public class WebMessage
    {
        private WebMessage(byte[] binary, string text, WebSocketMessageType messageType)
        {
            Binary = binary;
            Text = text;
            MessageType = messageType;
        }

        /// <summary>
        /// Received text message (only if type = WebSocketMessageType.Text)
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Received text message (only if type = WebSocketMessageType.Binary)
        /// </summary>
        public byte[] Binary { get; }

        /// <summary>
        /// Current message type (Text or Binary)
        /// </summary>
        public WebSocketMessageType MessageType { get; }

        /// <summary>
        /// Return string info about the message
        /// </summary>
        public override string ToString()
        {
            if (MessageType == WebSocketMessageType.Text)
            {
                return Text;
            }

            return $"Type binary, length: {Binary?.Length}";
        }

        /// <summary>
        /// Create text response message
        /// </summary>
        public static WebMessage TextMessage(string data)
        {
            return new WebMessage(null, data, WebSocketMessageType.Text);
        }

        /// <summary>
        /// Create binary response message
        /// </summary>
        public static WebMessage BinaryMessage(byte[] data)
        {
            return new WebMessage(data, null, WebSocketMessageType.Binary);
        }
    }

}
