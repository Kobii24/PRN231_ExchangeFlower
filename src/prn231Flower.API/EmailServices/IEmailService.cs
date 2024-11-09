using prn231Flower.API.Helper;

namespace prn231Flower.API.EmailServices;

public interface IEmailService
{
    Task SendEmailAsync(MailRequest mailrequest);
}
