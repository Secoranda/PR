﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server
{
    public partial class Server : Form
    {
       

        TcpListener listener;
        TcpClient client;
        String clNo;
        Dictionary<string, TcpClient> clientList = new Dictionary<string, TcpClient>();
        List<string> chat = new List<string>();

        public Server()
        {
            InitializeComponent();
            IPAddress[] localIP = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress address in localIP)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    ServerIPtextBox.Text = address.ToString();
                }
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            startServer();
        }

        public void updateUI(String m)
        {
       
            this.Invoke((MethodInvoker)delegate // To Write the Received data
            {
                textBox1.AppendText(">>" + m + Environment.NewLine);
            });
        }


        



    public async void startServer()
        {
            
         
             listener =  new TcpListener(IPAddress.Any, int.Parse(ServerPorttextBox.Text));
            listener.Start();
            
            
            updateUI("Server Started at " + listener.LocalEndpoint);
            updateUI("Waiting for Clients");
            try
            {
                int counter = 0;
                while (true)
                {
                    counter++;  
                    client = await Task.Run(() => listener.AcceptTcpClientAsync());

                    /* get username */
                    byte[] name = new byte[50];
                    NetworkStream stre = client.GetStream(); //Gets The Stream of The Connection
                    stre.Read(name, 0, name.Length); //Receives Data 
                    String username = Encoding.ASCII.GetString(name); // Converts Bytes Received to String
                    username = username.Substring(0, username.IndexOf("$"));

                    /* add to dictionary, listbox and send userList  */
                    clientList.Add(username, client);
                    listBox1.Items.Add(username);
                    updateUI("Connected to user " + username + " - " + client.Client.RemoteEndPoint);
                    announce(username + " Joined ", username, false);

                    await Task.Delay(1000).ContinueWith(t => sendUsersList());
                   
                    var c = new Thread(() => ServerReceive(client, username));
                    c.Start();
                    
                }
            }
            catch (Exception)
            {
                listener.Stop();
                
            }
        }

        public void announce(string msg, string uName, bool flag)
        {
            try
            {
                foreach (var Item in clientList)
                {
                    TcpClient broadcastSocket;
                    broadcastSocket = (TcpClient)Item.Value;
                    NetworkStream broadcastStream = broadcastSocket.GetStream();
                    Byte[] broadcastBytes = null;

                    if (flag)
                    {
                        chat.Add("gChat");
                        chat.Add(uName + " says : " + msg);
                        broadcastBytes = ObjectToByteArray(chat);
                    }
                    else
                    {
                        chat.Add("gChat");
                        chat.Add(msg);
                        broadcastBytes = ObjectToByteArray(chat);

                    }
                    broadcastStream.Write(broadcastBytes, 0, broadcastBytes.Length);
                    broadcastStream.Flush();
                    chat.Clear();
                }
            }
            catch (Exception er)
            {

            }
        }  //end broadcast function


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

        public byte[] ObjectToByteArray(Object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }


        public void ServerReceive(TcpClient clientn, String username)
        {
            byte[] data = new byte[1000];
            String text = null;
            while (true)
            {
                try
                {
                    NetworkStream stream = clientn.GetStream(); //Gets The Stream of The Connection
                    stream.Read(data, 0, data.Length); //Receives Data 
                    List<string> parts = (List<string>)ByteArrayToObject(data);

                    switch (parts[0])
                    {
                        case "gChat":
                            this.Invoke((MethodInvoker)delegate // To Write the Received data
                            {
                                textBox1.Text += username + ": " + parts[1] + Environment.NewLine;
                            });
                            announce(parts[1], username, true);
                            break;
                    }

                    parts.Clear();
                }
                catch (Exception r)
                {
                    updateUI("Client Disconnected: " + username);
                    announce("Client Disconnected: " + username + "$", username, false);
                    clientList.Remove(username);

                    this.Invoke((MethodInvoker)delegate
                    {
                        listBox1.Items.Remove(username);
                    });
                    sendUsersList();
                    break;
                }
            }
        }

     

        private void Private_Click(object sender, EventArgs e)
        {
            //UDPSocket s = new UDPSocket();
            //s.ServerUDP("192.168.1.24", 123);

            if (listBox1.SelectedIndex != -1)
            {
                String clientName = listBox1.GetItemText(listBox1.SelectedItem);

                chat.Clear();
                chat.Add("gChat");
                chat.Add("Admin : " + inputPrivate.Text);

                byte[] byData = ObjectToByteArray(chat);
                TcpClient workerSocket = null;
                workerSocket = (TcpClient)clientList.FirstOrDefault(x => x.Key == clientName).Value; //find the client by username in dictionary

                NetworkStream stm = workerSocket.GetStream();
                stm.Write(byData, 0, byData.Length);
                stm.Flush();
                chat.Clear();

            }
        }

       

        public void sendUsersList()
        {
            try
            {
                byte[] userList = new byte[1024];
                string[] clist = listBox1.Items.OfType<string>().ToArray();
                List<string> users = new List<string>();

                users.Add("userList");
                foreach (String name in clist)
                {
                    users.Add(name);
                }
                userList = ObjectToByteArray(users);

                foreach (var Item in clientList)
                {
                    TcpClient broadcastSocket;
                    broadcastSocket = (TcpClient)Item.Value;
                    NetworkStream broadcastStream = broadcastSocket.GetStream();
                    broadcastStream.Write(userList, 0, userList.Length);
                    broadcastStream.Flush();
                    users.Clear();
                }
            }
            catch (SocketException se)
            {
            }
        }

    

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.SelectionStart = textBox1.TextLength;
            textBox1.ScrollToCaret();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
