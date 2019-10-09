using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace MicroPost.Helpers {
    public class EmailManager {

        public static bool SendEmail(string to, string subject, string body) {
            try {
                string fromEmail = ConfigurationManager.AppSettings["FromEmail"];
                string fromPassword = ConfigurationManager.AppSettings["FromPassword"];
                var fromAddress = new MailAddress(fromEmail, "Micro Post");
                var toAddress = new MailAddress(to);
                var smtp = new SmtpClient {
                    Host = ConfigurationManager.AppSettings["SMTPHost"],
                    Port = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"]),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };

                using (var message = new MailMessage(fromAddress, toAddress) {
                    IsBodyHtml = true,
                    Subject = subject,
                    Body = body
                }) {
                    smtp.Send(message);
                }
                return true;
            } catch (Exception ex) { return false; }
        }
    }
}