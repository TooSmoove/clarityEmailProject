using ClarityEmailAPI.Email;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;

namespace ClarityEmailAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ClarityEmailController : ControllerBase
    {
        private readonly ILogger<ClarityEmailController> _logger;
        private IConfiguration _configuration;
        public ClarityEmailController(ILogger<ClarityEmailController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [EnableCors]
        [AllowAnonymous]
        [HttpPost("ClarityEmailSend")]
        public  async Task<string> SendClarityEmail(JsonObject requestOptions)
        {
            JsonObject RequestOptions = requestOptions;
            string result = "";
            string path = _configuration.GetSection("LocalSettings").GetSection("EmailLogFile").Value;

            string From = RequestOptions["From"].ToString();
            string To = RequestOptions["To"].ToString();
            string Message = RequestOptions["Message"].ToString();
            string Subject = RequestOptions["Subject"].ToString();

            using (StreamWriter writer = new FileInfo(path).AppendText())
            {
                writer.WriteLine("Date: " + DateTime.Now);
                writer.WriteLine("From: " + From);
                writer.WriteLine("To: " + To);
                writer.WriteLine("Subject: " + Subject);
                writer.WriteLine("Message: " + Message);
                writer.WriteLine("\n");
                writer.WriteLine("/*********************************");
            }


            ClarityEmail email = new ClarityEmail(_configuration);

            for (int tries = 0; tries < 4; tries++)
            {
                try
                {
                    await email.SendEmailAsync(From, Message, To, Subject);
                    result = "Email sent!";
                    break;
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);

                    if (tries == 3)
                    {
                        result = "Email failed 3 times; check your addess and try again";
                        break;
                    }

                    result = "Email Error";
                }
            }
            

            return result;
        }
    }
}
