using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;

namespace ClarityEmailAPI.Email
{
    public class ClarityEmail
    {
        private static IConfiguration config;

        // Replace recipient@example.com with a "To" address. If your account
        // is still in the sandbox, this address must be verified.
        static string receiverAddress;

        private readonly ILogger<ClarityEmail> _logger;

        public ClarityEmail(IConfiguration configuration)
        {
            config = configuration;
            receiverAddress = config.GetSection("LocalSettings").GetSection("ccAddress").Value;
        }

        public ClarityEmail(ILogger<ClarityEmail> logger)
        {
            _logger = logger;
        }

        public async Task<string> SendEmailAsync(string emailFrom, string emailBody, string emailTo, string emailSubject)
        {
            var _amazonSimpleEmailService = new AmazonSimpleEmailServiceClient();
            var messageId = "";
            List<string> bccAddresses = new List<string>() { receiverAddress };
            List<string> ccAddresses = new List<string>();
            List<string> toAddresses = new List<string>() { emailTo };

            try
            {
                var response = await _amazonSimpleEmailService.SendEmailAsync(
                    new SendEmailRequest
                    {
                        Destination = new Destination
                        {
                            BccAddresses = bccAddresses,
                            CcAddresses = ccAddresses,
                            ToAddresses = toAddresses
                        },
                        Message = new Message
                        {
                            Body = new Body
                            {
                                Html = new Content
                                {
                                    Charset = "UTF-8",
                                    Data = emailBody
                                },
                                Text = new Content
                                {
                                    Charset = "UTF-8",
                                    Data = emailBody
                                }
                            },
                            Subject = new Content
                            {
                                Charset = "UTF-8",
                                Data = emailSubject
                            }
                        },
                        Source = emailFrom
                    });
                messageId = response.MessageId;
            }
            catch (Exception ex)
            {
                Console.WriteLine("SendEmailAsync failed with exception: " + ex.Message);
            }

            return messageId;
            
        }
    }
}
