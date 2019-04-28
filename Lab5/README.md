# Laboratory work NR5.

## Sockets. TCP/IP and UDP Protocols
Theory: *TCP – is for connection orientated applications. It has built in error checking and will re transmit missing packets. 
UDP – is for connection less applications. It has no has built in error checking and will not re transmit missing packets.*

Was done a chat application based on sending and receiving messages via TCP/IP. So, we have a server and clients which communicate.
To get connected with the server, each user should know and enter the right IP and Port. 

![image](https://user-images.githubusercontent.com/24621285/56871171-9bfd1e80-6a22-11e9-9a7c-6b5ac408f23a.png)

Once connected they can chat. Client's message is visible for everyone, however server sends private each message accordingly to client selected.

![image](https://user-images.githubusercontent.com/24621285/56871310-a28c9580-6a24-11e9-946f-b704f5395107.png)

When somebody want to disconect, we have a dialog box with option yes/no and once disconected will get a message on server chat part

![image](https://user-images.githubusercontent.com/24621285/56871380-745b8580-6a25-11e9-93cc-8e38d98e9ade.png)
![image](https://user-images.githubusercontent.com/24621285/56871390-abca3200-6a25-11e9-8dc1-dcdc5857b591.png)

Project was done in Visual Studio using Windows.Forms.
All clients are divided by threads and each there action is synchronized through
```c#
this.Invoke((MethodInvoker)delegate
```
This is a synchronization mechanism, contained in all controls. All graphic/GUI updates, must only be executed from the GUI thread. (This is most likely the main thread.)
In such a way list of messages and list of participants is updated

```c#

        public void updateUI(String m)
        {
       
            this.Invoke((MethodInvoker)delegate // To Write the Received data
            {
                textBox1.AppendText(">>" + m + Environment.NewLine);
            });
        }
```
To deal with data, was used MemoryStream. So, we keep all data in MemoryStream and convert when needed.

```c#
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

```

All project is based on TCP/IP. However, we have a class UDPSocket which deals with connections more or less.
A easy way, is to send a message and once we'll receive it back we'll mean than is connected.
On client part that means to test a port. As mentioned we can just send a message and wait it back,
or other option is to use UdpClient, set a receive timeout on the underlying socket, make a connection to that remote server/port, Send some small message (byte[] !) and call Receive.
This aproach will have same result.
Also, to avoid any issues was used try catch blocks.

I'm still dealing with server part. Have some issues with Socket. 

P.S. Is not that easy to integrate 2 protocols...


