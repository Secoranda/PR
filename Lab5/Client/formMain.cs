using Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Client
{
    public partial class formMain : Form
    {
        public TcpClient clientSocket;
        public NetworkStream serverStream = default(NetworkStream);
        string readData = null;
        Thread ctThread;
        String name = null;
        UDPSocket c = new UDPSocket();
        List<string> nowChatting = new List<string>();
        List<string> chat = new List<string>();

        public void setName(String title)
        {
            this.Text = title;
            name = title;
        }

        public formMain()
        {
            InitializeComponent();

        }

        private void btnSend_Click(object sender, EventArgs e)
        {
                try
                {
                try
                {
                    c.Client(ClientIPtextBox.Text, int.Parse(ClientPorttextBox.Text));
                    c.Send("TEST!");
                }
                catch (Exception)
                {
                    MessageBox.Show("Not Connected");
                }


                if (!input.Text.Equals(""))
                    {
                        chat.Add("gChat");
                        chat.Add(input.Text);
                        byte[] outStream = ObjectToByteArray(chat);

                        serverStream.Write(outStream, 0, outStream.Length);
                        serverStream.Flush();
                        input.Text = "";
                        chat.Clear();
                    }
                }
                catch (Exception )
                {
                    btnConnect.Enabled = true;
                }
            

        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            clientSocket = new TcpClient();
            IPEndPoint IpEnd = new IPEndPoint(IPAddress.Parse(ClientIPtextBox.Text), int.Parse(ClientPorttextBox.Text));

            try
            {
                try
                {
                    c.Client(ClientIPtextBox.Text, int.Parse(ClientPorttextBox.Text));
                    c.Send("TEST!");
                }
                catch (Exception)
                {
                    MessageBox.Show("Not Connected");
                }

                clientSocket.Connect(IpEnd);
                readData = "Connected to Server ";
                msg();

                serverStream = clientSocket.GetStream();

                byte[] outStream = Encoding.ASCII.GetBytes(name + "$");
                serverStream.Write(outStream, 0, outStream.Length);
                serverStream.Flush();
                btnConnect.Enabled = false;


                ctThread = new Thread(getMessage);
                ctThread.Start();

                UdpClient client = new UdpClient();

            }
            catch (Exception )
            {
                MessageBox.Show("Server Not Started");
            }
        }

        public void getUsers(List<string> parts)
        {
            
            this.Invoke((MethodInvoker)delegate
            {
                try
                {
                    c.Client(ClientIPtextBox.Text, int.Parse(ClientPorttextBox.Text));
                    c.Send("TEST!");
                }
                catch (Exception)
                {
                    MessageBox.Show("Not Connected");
                }
                listBox1.Items.Clear();
                for (int i = 1; i < parts.Count; i++)
                {
                    listBox1.Items.Add(parts[i]);

                }
            });
        }

    

        private void getMessage()
        {
            
                try
                {
                    c.Client(ClientIPtextBox.Text, int.Parse(ClientPorttextBox.Text));
                    c.Send("TEST!");
                MessageBox.Show(c.ToString());
                while (true)
                    {
                       
                        serverStream = clientSocket.GetStream();
                        byte[] inStream = new byte[10025];
                        serverStream.Read(inStream, 0, inStream.Length);
                        List<string> parts = null;



                        parts = (List<string>)ByteArrayToObject(inStream);
                        switch (parts[0])
                        {
                            case "userList":
                                getUsers(parts);
                                break;

                            case "gChat":
                                readData = "" + parts[1];
                                msg();
                                break;

                              
                        }
                    }
                }
                catch (Exception e)
                {
                    ctThread.Abort();
                    clientSocket.Close();
                    btnConnect.Enabled = true;
                    MessageBox.Show(e.ToString());
                }
            
        }

        private void msg()
        {
            if (this.InvokeRequired)
                this.Invoke(new MethodInvoker(msg));
            else
                history.Text = history.Text + Environment.NewLine + " >> " + readData;
        }

        private void formMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dialog = MessageBox.Show("Do you want to exit? ", "Exit", MessageBoxButtons.YesNo);
            if (dialog == DialogResult.Yes)
            {
                try
                {
                    ctThread.Abort();
                    clientSocket.Close();
                }
                catch (Exception ) { }

                Application.ExitThread();
            }
            else if (dialog == DialogResult.No)
            {
                e.Cancel = true;
            }
        }


       

        public byte[] ObjectToByteArray(object _Object)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, _Object);
                return stream.ToArray();
            }
        }

        public Object ByteArrayToObject(byte[] arrBytes)
        {
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = binForm.Deserialize(memStream);
                return obj;
            }
        }

       

 
    

        private void history_TextChanged(object sender, EventArgs e)
        {
            history.SelectionStart = history.TextLength;
            history.ScrollToCaret();
        }

        private void input_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
