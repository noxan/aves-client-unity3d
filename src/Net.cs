using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;


public delegate void DataHandler(string msg);

public class Net {
    public const int BUFFER_SIZE = 1024;

    private byte[] byteBuffer = new byte[BUFFER_SIZE];
    private TcpClient tcpClient;
    private NetworkStream stream;

    private Thread connectThread;
    private Thread readThread;

    private DataHandler dataHandler;

    private List<NetEventListener> listeners;

    public void SetDataHandler(DataHandler dataHandler) {
        this.dataHandler = dataHandler;
    }

    public void AddNetEventListener(NetEventListener listener) {
        listeners.Add(listener);
    }

    private void fireNetEvent(NetEvent netEvent) {
        foreach(NetEventListener listener in listeners) {
            listener(netEvent);
        }
    }

    public Net() {
        listeners = new List<NetEventListener>();
    }

    public void Connect() {
        Logger.Log("Connect: Start");
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

            fireNetEvent(NetEvent.CONNECT);
            Logger.Log("Connect: Successful");
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
                    HandleData(byteBuffer, num);
                }
            } catch(Exception exception) {
                Logger.Log("Read: " + exception);
                break;
            }
        }
    }

    private void HandleData(byte[] buf, int size) {
        if(dataHandler != null) {
            string data = Encoding.ASCII.GetString(buf, 0, size);
            Logger.Log(string.Format("DataHandler: {0}", data));
            dataHandler(data);
            fireNetEvent(NetEvent.DATA);
        } else {
            Logger.Log("DataHandler: is null");
        }
    }

    public void Write(string message) {
        byte[] array = Encoding.ASCII.GetBytes(message);
        stream.Write(array, 0, array.Length);
    }

    public bool IsConnected() {
        return tcpClient!=null && tcpClient.Client.Connected;
    }

    public void Disconnect() {
        stream.Close();
        tcpClient.Close();
        readThread.Abort();
        connectThread.Abort();
        fireNetEvent(NetEvent.DISCONNECT);
        Logger.Log("Disconnect: Successful");
    }
}
