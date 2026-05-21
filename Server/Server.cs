using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server
{
    public partial class Server : Form
    {
        public Server()
        {
            InitializeComponent();
        }

        TcpListener listener;
        private static int activeConnections = 0;

        private async void startBtn_Click(object sender, EventArgs e)
        {
            listener = new TcpListener(IPAddress.Any, 9050);
            listener.Start();
            logText.Text += "Server started on port 9050..." + Environment.NewLine;
            startBtn.Enabled = false;

            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                _ = HandleClientAsync(client);
            }
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            NetworkStream ns = client.GetStream();
            activeConnections++;
            logText.Text += $"Client connected. Active connections: {activeConnections}" + Environment.NewLine;

            try
            {
                // read file size
                byte[] sizeBuf = new byte[8];
                int read = await ns.ReadAsync(sizeBuf, 0, 8);
                long fileSize = BitConverter.ToInt64(sizeBuf, 0);

                logText.Text += $"Expecting file of {fileSize} bytes..." + Environment.NewLine;

                // read file bytes
                byte[] fileData = new byte[fileSize];
                int totalRead = 0;
                while (totalRead < fileSize)
                {
                    int r = await ns.ReadAsync(fileData, totalRead, (int)(fileSize - totalRead));
                    if (r == 0) throw new Exception("Connection closed unexpectedly.");
                    totalRead += r;
                }
                logText.Text += "File received, compressing..." + Environment.NewLine;

                // compress using gzip
                MemoryStream ms = new MemoryStream();
                using (GZipStream gz = new GZipStream(ms,CompressionMode.Compress))
                {
                    gz.Write(fileData, 0, fileData.Length);
                }
                byte[] compressedData = ms.ToArray();
                logText.Text += $"Compressed size: {compressedData.Length} bytes" + Environment.NewLine;

                // send compressed size then compressed data
                byte[] compSizeBuf = BitConverter.GetBytes((long)compressedData.Length);
                await ns.WriteAsync(compSizeBuf, 0, 8);
                await ns.WriteAsync(compressedData, 0, compressedData.Length);
                await ns.FlushAsync();
                logText.Text += "Compressed file sent to client." + Environment.NewLine;
            }
            catch (Exception ex)
            {
                logText.Text += $"Error: {ex.Message}" + Environment.NewLine;
            }
            finally
            {
                ns.Close();
                client.Close();
                activeConnections--;
                logText.Text += $"Client disconnected. Active connections: {activeConnections}" + Environment.NewLine;
            }
        }
    }
}