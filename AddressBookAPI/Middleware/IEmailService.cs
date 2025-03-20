using System.Threading.Tasks;

namespace AddressBookAPI.Middleware
{
    public interface IEmailService
    {
        void SendEmail(string toEmail, string subject, string body);
    }
}
