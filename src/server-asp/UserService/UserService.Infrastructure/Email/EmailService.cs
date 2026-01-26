using System.Net;
using System.Net.Mail;
using System.Text;

using Microsoft.Extensions.Options;

using UserService.Application.Interfaces.Email;

namespace UserService.Infrastructure.Email;

public class EmailService : IEmailService
{
	private readonly EmailModel _emailOptions;

	public EmailService(IOptions<EmailModel> emailOptions)
	{
		_emailOptions = emailOptions.Value;
	}

	public async Task SendMail(string code, string recipient)
	{
		using SmtpClient smtpClient = new(
			_emailOptions.Server,
			_emailOptions.Port);
		smtpClient.Credentials = new NetworkCredential(
			_emailOptions.Email,
			_emailOptions.Password);
		smtpClient.EnableSsl = true;

		using MailMessage mailMessage = new();
		mailMessage.From = new MailAddress(_emailOptions.Email);
		mailMessage.To.Add($"{recipient}");
		mailMessage.Subject = "КОД ДЛЯ ПОДТВЕРЖДЕНИЯ";

		mailMessage.IsBodyHtml = true;
		StringBuilder htmlBody = new();
		htmlBody.Append("<!DOCTYPE html>");
		htmlBody.Append("<html lang=\"en\">");
		htmlBody.Append("<head><meta charset=\"UTF-8\"><title>Email Template</title></head>");
		htmlBody.Append("<body>");
		htmlBody.Append("<h1>КОД ДЛЯ ПОДТВЕРЖДЕНИЯ</h1>");
		htmlBody.Append("<p>Дорогой пользователь,</p>");
		htmlBody.Append($"<div style=\"background-color: #007bff; color: #fff; padding: 10px 20px; border-radius: 5px; font-size: 25px; text-align: center;\">" +
							$"<strong>{code}</strong>" +
						$"</div>");
		htmlBody.Append("</body>");
		htmlBody.Append("</html>");

		mailMessage.Body = htmlBody.ToString();

		await smtpClient.SendMailAsync(mailMessage);
	}
}