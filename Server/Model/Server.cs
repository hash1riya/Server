using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Server.Model;
public class Server
{
    IPEndPoint endp;
    Socket socket;
    public byte[] Buffer = new byte[1024];

    public Server(string ip, int port)
    {
        endp = new IPEndPoint(IPAddress.Parse(ip), port);
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
        socket.Bind(endp);
        socket.Listen(10);
    }
    public async Task<Socket> ClientAccept() => await socket.AcceptAsync();
    public void Stop()
    {
        if (socket != null)
            try
            {
                socket.Close();
                socket.Dispose();
                socket = null;
            }
            catch (SocketException ex)
            {
                Debug.WriteLine(ex.Message);
            }
    }
    public async Task<int> Receive(Socket s) => await s.ReceiveAsync(this.Buffer);
    public async void Send(string strSend, Socket client)
    {
        if (socket != null)
            await client.SendAsync(System.Text.Encoding.UTF8.GetBytes(strSend));
    }
}