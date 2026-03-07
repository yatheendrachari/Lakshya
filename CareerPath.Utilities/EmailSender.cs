
using Microsoft.AspNetCore.Identity.UI.Services;

namespace CareerPath.Utilities
{
    public class EmailSender: IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            //logic to send email
            return Task.CompletedTask;
        }
    }
}
