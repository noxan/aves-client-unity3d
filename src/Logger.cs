using System;
using System.IO;


public class Logger {
    public static string GetLogFilename() {
        return string.Format(@".\aves-network-{0}.log", DateTime.UtcNow.ToString("yyyyMMddHHmmssffff"));
    }

    private static readonly StreamWriter writer = File.AppendText(GetLogFilename());

    private Logger() {}

    ~Logger() {
        writer.Close();
    }

    public static void Log(string message) {
        writer.WriteLine(message);
        writer.Flush();
    }
}
