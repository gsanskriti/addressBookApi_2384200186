using System.Threading.Tasks;

namespace AddressBookAPI.Middleware
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}
