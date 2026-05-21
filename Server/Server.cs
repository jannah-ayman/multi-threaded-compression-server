using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Text;
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
        int activeConnections = 0;

        private async void startBtn_Click(object sender, EventArgs e)
        {
            listener = new TcpListener(IPAddress.Any, 9050);
            listener.Start();
            logText.Text += "Server started on port 9050..." + Environment.NewLine;
            startBtn.Enabled = false;

            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                _ = HandleClient(client);
            }
        }

        private async Task HandleClient(TcpClient client)
        {
            NetworkStream ns = client.GetStream();
            activeConnections++;
            logText.Text += $"Client connected. Active connections: {activeConnections}" + Environment.NewLine;
            try
            {
                //read file size
                byte[] sizeBuf = new byte[4];
                int recv = await ns.ReadAsync(sizeBuf, 0, 4);
                int fileSize = BitConverter.ToInt32(sizeBuf, 0);

                logText.Text += $"Expecting file of {fileSize} bytes..." + Environment.NewLine;

                //read file bytes
                byte[] fileData = new byte[fileSize];
                int totalRead = 0;
                while (totalRead < fileSize)
                {
                    recv = await ns.ReadAsync(fileData, totalRead, fileSize - totalRead);
                    if (recv == 0)
                    {
                        logText.Text += "Connection closed unexpectedly." + Environment.NewLine;
                        return;
                    }
                        
                    totalRead += recv;
                }
                logText.Text += "File received, compressing..." + Environment.NewLine;

                MemoryStream ms = new MemoryStream();
                using (GZipStream gz = new GZipStream(ms, CompressionMode.Compress)) {
                    gz.Write(fileData, 0, fileData.Length);
                }
                byte[] compressedData = ms.ToArray();
                logText.Text += $"Compressed size: {compressedData.Length} bytes" + Environment.NewLine;

                // send compressed size then compressed data
                byte[] compSizeBuf = BitConverter.GetBytes(compressedData.Length);
                await ns.WriteAsync(compSizeBuf, 0, 4);
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