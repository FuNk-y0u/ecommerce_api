using ecommerce.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;
using System;
using ecommerce.Interfaces;
using System.IO;

namespace EmailApi.Services
{
    public class EmailService : IEmailService
    {
        public string _EmailUsername;
        public string _EmailPassword;
        public string _MailAddress;
        public string _MailServ;
        public string _MailTemplate;

        public EmailService(IConfiguration config)
        {
            _EmailUsername = config["EmailUsername"];
            _EmailPassword = config["EmailPassword"];
            _MailAddress = config["MailAddress"];
            _MailServ = config["MailServ"];
            _MailTemplate = config["MailTemplates"];
        }

        public string get_username()
        {
            return _EmailUsername;
        }
        public string get_password()
        {
            return _EmailPassword;
        }

        public string get_mailaddress()
        {
            return _MailAddress;

        }
        public string get_mailserv()
        {
            return _MailServ;
        }

        public void send_mail(string To, string Subject,string link)
        {
            try
            {
                var mail = new MailMessage();
                var client = new SmtpClient(this._MailServ, 2525)
                {
                    Credentials = new NetworkCredential(this._EmailUsername, this._EmailPassword),
                    EnableSsl = true
                };
                mail.From = new MailAddress(this._MailAddress);
                mail.To.Add(To);
                mail.Subject = Subject;
                string Template = File.ReadAllText(_MailTemplate);
                Template = Template.Replace("&", link);


                var htmlView = AlternateView.CreateAlternateViewFromString(Template, null, "text/html");
                mail.AlternateViews.Add(htmlView);
                client.Send(mail);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
