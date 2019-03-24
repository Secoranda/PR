# TASK:
Use the SMTP/POP3 protocols to send/receive emails (bonus for attachments and html formatted text):

## Solution
```
Note: If you wanna run this code
```

In this laboratory work, was used [Mailkit](https://github.com/jstedfast/MailKit) (Open Source cross-platform .NET mail-client library that is based on MimeKit) and [Mimekit](http://www.mimekit.net/) (MimeKit is a popular high-performance MIME framework for .NET.)

There are two processes:
1. Send
2. Receive

For **Send** was used **SmtpClient**. It allows sending ot e-mail notificatoins using SMTP server.
Simple Mail Transfer Protocol (SMTP) is a widely used protocol for the delivery of e-mails between TCP/IP systems and users.

```c#
  using (var client = new SmtpClient(new ProtocolLogger(Console.OpenStandardOutput())))
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    client.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    client.Authenticate(sender, psender);
                    client.Send(message);
                    client.Disconnect(true);

                    Console.WriteLine("Send Mail Success.");
                }
```

With **BodyBuilder** Method, is possible to send inside the message or as attachements every format/file. It constructs the message body based on the text-based bodies, the linked resources, and the attachments.
 ```c#
            var builder = new BodyBuilder();
            builder.TextBody = messagetext;

            var image = builder.LinkedResources.Add(@"C:\Users\Daniela\Desktop\simba.png");
            image.ContentId = MimeUtils.GenerateMessageId();

            builder.HtmlBody = string.Format($"<p>{messagetext} </p><br> "+
                @"<center><img src=""cid:{0}""></center>", image.ContentId);
            builder.Attachments.Add(@"C:\Users\Daniela\Desktop\WordFile.docx");
            message.Body = builder.ToMessageBody();
 ```
For more examples of BodyBuilder, you can see [link](https://csharp.hotexamples.com/ru/examples/MimeKit/BodyBuilder/ToMessageBody/php-bodybuilder-tomessagebody-method-examples.html)

For **Receive** was used **POP3**.
POP3 (Post Office Protocol 3) is the most recent version of a standard protocol for receiving e-mail. POP3 is a client/server protocol in which e-mail is received and held for you by your Internet server. 
```c#
using (var clientreceive = new Pop3Client())
                {
                    clientreceive.Connect("pop.gmail.com", 995, true);

                    clientreceive.Authenticate(sender, psender);

                    var emails = new List<MimeMessage>();
                    for (var i = 0; i < clientreceive.Count && i < maxCount; i++)
                    {
                        emails.Add(clientreceive.GetMessage(i));
                    }
                    emails.ForEach(x => Console.WriteLine($"From: {x.From.Mailboxes.First()}\n" +
                           $"Subject: {x.Subject}\nContent: {x.TextBody}"));

                    clientreceive.Disconnect(true);
                    Console.WriteLine("Receive Mail Success.");
                }
```
# Results

**Sending**



**Receiving**


