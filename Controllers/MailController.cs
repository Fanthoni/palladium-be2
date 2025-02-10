using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;

namespace PalladiumBE.Controllers;

[ApiController]
[Route("[controller]")]
public class MailController : ControllerBase
{
    private readonly ILogger<MailController> _logger;

    public MailController(ILogger<MailController> logger)
    {
        _logger = logger;
    }

    [HttpPost("send/", Name = "SendMail")]
    public IActionResult sendMail(SendMailRequest request)
    {
        try
        {
            var companyEmail = AppConfig.EmailAddress;
            var fromPassword = AppConfig.EmailPassword;

            if (string.IsNullOrEmpty(companyEmail) || string.IsNullOrEmpty(fromPassword))
            {
                return StatusCode(400, "Company email or password is not set in configuration.");
            }

            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            var fromAddress = new MailAddress(companyEmail, request.Name);
            var toAddress = new MailAddress(companyEmail, "PalladiumFloors");
            string subject = $"Message from {request.Name}";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using var emailMessage = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = request.Message
            };
            smtp.Send(emailMessage);
            return Ok();
        }
        catch(Exception ex)
        {
            _logger.LogError($"Error sending mail: {ex.Message}");
            return StatusCode(500, "Internal Server Error");
        }
    }
}