using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class MailObject
    {
        public int SendMail { get; set; }
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }


        public string Error_From { get; set; }
        public string Error_To { get; set; }
        public string Error_Cc { get; set; }
        public string Error_Bcc { get; set; }
        public string Error_Body { get; set; }
        public string Error_Subject { get; set; }

        public void SendEMail(MailObject mail, Attachment attachment)
        {
            try
            {
                MailMessage message = new MailMessage(mail.From, mail.To, mail.Subject, mail.Body);
                var smtpClient = new SmtpClient(mail.SmtpServer)
                {
                    Port = mail.SmtpPort,
                    Credentials = new NetworkCredential(mail.User, mail.Password),
                    EnableSsl = true,
                };

                if (attachment != null)
                {
                    message.Attachments.Add(attachment);
                }

                smtpClient.Send(message);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void SendErrorEMail(MailObject mail, Attachment attachment)
        {
            try
            {
                MailMessage message = new MailMessage(mail.Error_From, mail.Error_To, mail.Error_Subject, mail.Error_Body);
                var smtpClient = new SmtpClient(mail.SmtpServer)
                {
                    Port = mail.SmtpPort,
                    Credentials = new NetworkCredential(mail.User, mail.Password),
                    EnableSsl = true,
                };

                if (attachment != null)
                {
                    message.Attachments.Add(attachment);
                }

                smtpClient.Send(message);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
