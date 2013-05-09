using System.IO;


public class Logger {
    private StreamWriter writer;

    private Logger() {
        writer = File.AppendText(@".\aves-network.log");
    }

    public void Log(string message) {
        writer.WriteLine(message);
    }
}
