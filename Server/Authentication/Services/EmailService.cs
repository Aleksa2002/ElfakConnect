using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using Server.Authentication.Interfaces;

namespace Server.Authentication.Services;

public class EmailOptions
{
    public required string Host { get; set; }
    public required int Port { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}

public class EmailService : IEmailService
{
    private readonly SmtpClient _smtpClient;
    private readonly string _senderEmail;

    public EmailService(IOptions<EmailOptions> options)
    {
        _senderEmail = options.Value.Email;
        
        _smtpClient = new SmtpClient(options.Value.Host)
        {
            Port = options.Value.Port,
            Credentials = new NetworkCredential(
                options.Value.Email,
                options.Value.Password),
            EnableSsl = true,
            UseDefaultCredentials = false,
            DeliveryMethod = SmtpDeliveryMethod.Network
        };
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var mailMessage = new MailMessage
        {
            From = new MailAddress(_senderEmail, "ElfakConnect"),
            Subject = subject,
            Body = body,
            IsBodyHtml = true,
            
        };
        mailMessage.To.Add(to);

        await _smtpClient.SendMailAsync(mailMessage);
    }

    public async Task SendVerificationEmailAsync(User user, string code)
    {
        string subject = "ElfakConnect - Email Verification";
        string body = $@"<html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2>Welcome to ElfakConnect!</h2>
                    <p>Hello {user.Username},</p>
                    <p>Your verification code is:</p>
                    <h3 style='background-color: #f5f5f5; padding: 10px; display: inline-block;'>{code}</h3>
                    <p>Please enter this code in the ElfakConnect app to verify your email address.</p>
                    <p>If you did not request this, please ignore this email.</p>
                    <p>Best regards,<br>
                    The ElfakConnect Team</p>
                </body>
            </html>";

        await SendEmailAsync(user.Email, subject, body);
    }
}
