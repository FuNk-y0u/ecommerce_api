using ecommerce.Models;

namespace ecommerce.Interfaces
{
    public interface IEmailService
    {
        public string get_username();
        public string get_password();
        public string get_mailaddress();
        public string get_mailserv();
        public void send_mail(string To, string Subject, string link);
    }
}
