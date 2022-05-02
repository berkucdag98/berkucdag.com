using berkucdag.com.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace berkucdag.com.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Blog()
        {
            return View();
        }

        [HttpPost]
        public void SendMail(MailSend mailSend)
        {
            SmtpClient sc = new SmtpClient("");
            sc.Port = 587;
            sc.Host = "mail.berkucdag.com";
            sc.EnableSsl = false;
            sc.Credentials = new NetworkCredential("info@berkucdag.com", "M^7dd2o55");
            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("info@berkucdag.com", mailSend.NameSurname);
            msg.To.Add(new MailAddress("berkda3@gmail.com", "Berk Üçdağ"));
            msg.Subject = mailSend.Subject;
            msg.Body = mailSend.Email + "\n\n" + mailSend.Message;
            sc.Send(msg);

        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
