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
                // STEP 1: Read file into memory
                byte[] fileData = File.ReadAllBytes(selectedFilePath);
                long fileSize = fileData.Length;
                logText.Text += $"Sending file... ({fileSize} bytes)" + Environment.NewLine;

                // STEP 2: Send file size then file data
                byte[] sizeBuf = BitConverter.GetBytes(fileSize);
                await ns.WriteAsync(sizeBuf, 0, 8);
                await ns.WriteAsync(fileData, 0, (int)fileSize);
                await ns.FlushAsync();
                logText.Text += $"Original: {fileSize} bytes" + Environment.NewLine;
                logText.Text += "File sent. Waiting for compressed file..." + Environment.NewLine;

                // STEP 3: Read compressed size (8 bytes)
                byte[] compSizeBuf = new byte[8];
                int read = await ns.ReadAsync(compSizeBuf, 0, 8);
                long compSize = BitConverter.ToInt64(compSizeBuf, 0);

                // STEP 4: Read compressed file bytes
                byte[] compData = new byte[compSize];
                int totalRead = 0;
                while (totalRead < compSize)
                {
                    read = await ns.ReadAsync(compData, totalRead, (int)(compSize - totalRead));
                    if (read == 0) throw new Exception("Connection closed unexpectedly.");
                    totalRead += read;
                }

                // STEP 5: Save compressed file
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
