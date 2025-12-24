using System;

namespace Server.Authentication.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string body);
    Task SendVerificationEmailAsync(User user, string code);
}
