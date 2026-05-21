using System;
using System.IO;
using System.Net.Sockets;
using System.Windows.Forms;

namespace Client
{
    public partial class Client : Form
    {
        public Client()
        {
            InitializeComponent();
        }
        string selectedFilePath = "";
        TcpClient tcpClient;
        NetworkStream ns;
        private async void connectBtn_Click(object sender, EventArgs e)
        {
            try
            {
                connectBtn.Enabled = false;
                logText.Text += "Connecting..." + Environment.NewLine;

                tcpClient = new TcpClient();
                await tcpClient.ConnectAsync("127.0.0.1", 9050);
                ns = tcpClient.GetStream();

                logText.Text += "Connected to server." + Environment.NewLine;
                browseBtn.Enabled = true;
            }
            catch (Exception ex)
            {
                logText.Text += $"Error: {ex.Message}" + Environment.NewLine;
                connectBtn.Enabled = true;
            }
        }

        private void browseBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                selectedFilePath = dlg.FileName;
                filePathTxt.Text = selectedFilePath;
                logText.Text += "File selected." + Environment.NewLine;
                sendBtn.Enabled = true;
            }
        }

        private async void sendBtn_Click(object sender, EventArgs e)
        {
            if (selectedFilePath == "")
            {
                MessageBox.Show("Please select a file first.");
                return;
            }

            if (tcpClient == null || !tcpClient.Connected)
            {
                MessageBox.Show("Please connect to the server first.");
                return;
            }

            sendBtn.Enabled = false;
            browseBtn.Enabled = false;

            try
            {
                // read file into memory
                byte[] fileData = File.ReadAllBytes(selectedFilePath);
                int fileSize = fileData.Length;
                logText.Text += $"Sending file... ({fileSize} bytes)" + Environment.NewLine;

                // send file size then file data
                byte[] sizeBuf = BitConverter.GetBytes(fileSize);
                await ns.WriteAsync(sizeBuf, 0, 4);
                await ns.WriteAsync(fileData, 0, fileSize);
                await ns.FlushAsync();
                logText.Text += $"Original: {fileSize} bytes" + Environment.NewLine;
                logText.Text += "File sent. Waiting for compressed file..." + Environment.NewLine;

                // read compressed size
                byte[] compSizeBuf = new byte[4];
                int recv = await ns.ReadAsync(compSizeBuf, 0, 4);
                int compSize = BitConverter.ToInt32(compSizeBuf, 0);

                // read compressed file bytes
                byte[] compData = new byte[compSize];
                int totalRead = 0;
                while (totalRead < compSize)
                {
                    recv = await ns.ReadAsync(compData, totalRead, (compSize - totalRead));
                    if (recv == 0) throw new Exception("Connection closed unexpectedly.");
                    totalRead += recv;
                }

                // save compressed file
                string savePath = selectedFilePath + ".gz";
                File.WriteAllBytes(savePath, compData);
                logText.Text += $"Compressed: {compSize} bytes" + Environment.NewLine;
                logText.Text += $"Done! Saved as: {savePath}" + Environment.NewLine;
            }
            catch (Exception ex)
            {
                logText.Text += $"Error: {ex.Message}" + Environment.NewLine;
            }
            finally
            {
                connectBtn.Enabled = true;
            }
        }
    }
}
