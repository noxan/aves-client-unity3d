using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;


public class Net {
    private TcpClient tcpClient;
    private NetworkStream stream;

    private Thread connectThread;
    private Thread readThread;

    public void Connect() {
        connectThread = new Thread(new ThreadStart(ConnectThread));
        connectThread.Start();
    }

    private void ConnectThread() {
        try {
            tcpClient = new TcpClient();
            tcpClient.Client.Connect("localhost", 1666);
            stream = tcpClient.GetStream();
        } catch(SocketException socketException) {
            Logger.Log("Connect: SocketException " + socketException);
        } catch(Exception exception) {
            Logger.Log("Connect: Exception " + exception);
        }
    }
}
