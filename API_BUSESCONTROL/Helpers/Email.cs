using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;
using API_BUSESCONTROL.Models;

namespace API_BUSESCONTROL.Helpers {
    public class Email : IEmail {
        private readonly IConfiguration _configuration;

        public Email(IConfiguration configuration) {
            _configuration = configuration;
        }

        public bool EnviarEmail(string email, string tema, string msg) {
            try {
                string host = _configuration.GetValue<string>("SMTP:Host");
                string nome = _configuration.GetValue<string>("SMTP:Nome");
                string userName = _configuration.GetValue<string>("SMTP:UserName");
                string senha = _configuration.GetValue<string>("SMTP:Senha");
                int porta = _configuration.GetValue<int>("SMTP:Porta");

                MailMessage mail = new MailMessage() {
                    From = new MailAddress(userName, nome)
                };

                mail.To.Add(email);
                mail.Subject = tema;
                mail.Body = msg;
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.High;

                using (SmtpClient smtp = new SmtpClient(host, porta)) {
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(userName, senha);
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                    return true;
                }

            }
            catch (Exception) {
                return false;
            }
        }
    }
}

