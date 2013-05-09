using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;


public class Net {
    public const int BUFFER_SIZE = 1024;

    private byte[] byteBuffer = new byte[BUFFER_SIZE];
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

            readThread = new Thread(new ThreadStart(ReadThread));
            readThread.Start();
        } catch(SocketException socketException) {
            Logger.Log("Connect: SocketException " + socketException);
        } catch(Exception exception) {
            Logger.Log("Connect: Exception " + exception);
        }
    }

    private void ReadThread() {
        while(true) {
            try {
                if(stream.DataAvailable) {
                    int num = stream.Read(byteBuffer, 0, BUFFER_SIZE);
                    if(num < 1) {
                        Logger.Log("Read: End of stream, connection closed.");
                        break;
                    }
                    string data = Encoding.ASCII.GetString(buf, 0, size);
                }
            } catch(Exception exception) {
                Logger.Log("Read: " + exception);
                break;
            }
        }
    }

    public void Disconnect() {
        stream.Close();
        tcpClient.Close();
        readThread.Abort();
        connectThread.Abort();
    }
}
