using System;
using System.Collections.Generic;
using System.Linq;
using MailKit;
using MailKit.Net.Pop3;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Utils;

namespace Lab3
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Subject:");
            var subject = Console.ReadLine();
            Console.WriteLine("Receiver EMAIL: ");
            var receiver = Console.ReadLine();
            Console.WriteLine("Sender EMAIL:");
            var sender = Console.ReadLine();
            Console.WriteLine("Sender PASSWORD:");
            var psender = Console.ReadLine();
            Console.WriteLine("Type Message:");
            var messagetext = Console.ReadLine();

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(sender));
            message.To.Add(new MailboxAddress(receiver));
            message.Subject = subject;

            var builder = new BodyBuilder();
            builder.TextBody = messagetext;

            var image = builder.LinkedResources.Add(@"C:\Users\Daniela\Desktop\simba.png");
            image.ContentId = MimeUtils.GenerateMessageId();

            builder.HtmlBody = string.Format($"<p>{messagetext} </p><br> "+
                @"<center><img src=""cid:{0}""></center>", image.ContentId);
            builder.Attachments.Add(@"C:\Users\Daniela\Desktop\WordFile.docx");
            message.Body = builder.ToMessageBody();

            try
            {

                using (var client = new SmtpClient(new ProtocolLogger(Console.OpenStandardOutput())))
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    client.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    client.Authenticate(sender, psender);
                    client.Send(message);
                    client.Disconnect(true);

                    Console.WriteLine("Send Mail Success.");
                }

                Console.WriteLine("Nummber of message to receive:");
                var maxCount = Convert.ToInt32(Console.ReadLine());

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
            }
            catch (Exception e)
            {
                Console.WriteLine("Send Mail Failed : " + e.Message);
            }
            Console.ReadLine();
        }
    }
}
