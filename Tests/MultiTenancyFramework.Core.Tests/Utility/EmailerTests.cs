using MultiTenancyFramework;
using System.Net.Mail;
using System.Net;
using SimpleInjector;
using MultiTenancyFramework.SimpleInjector;
using NUnit.Framework;

namespace MultiTenancyFramework.CoreTests.Utility
{
    [TestFixture]
    public class EmailerTests
    {
        private SmtpClient client;
        private string sender;

        [TearDown]
        public void Cleanup()
        {
            client?.Dispose();
        }

        [SetUp]
        public void Init()
        {
            var container = new Container();

            var baseContainer = new BaseContainer(null, container);

            // Very Important
            MyServiceLocator.SetIoCContainer(baseContainer.Container);

            client = new SmtpClient
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,

                //Gmail
                //Host = "smtp.gmail.com",
                //Port = 587,
                //Credentials = new NetworkCredential("donotreply.qcpta@gmail.com", "october2014"),

                //GoDaddysmtpout
                Host = "relay-hosting.secureserver.net",
                Port = 25,
                Credentials = new NetworkCredential("logging@schcoosystem.com", "Sw882@1"),
            };

            //Gmail
            //sender = "donotreply.qcpta@gmail.com";

            //GoDaddy
            sender = "logging@schcoosystem.com";
        }

        [Test]
        public void SendEmail_all_correct()
        {
            var sent = Emailer.SendEmail(senderEmail: sender
                , body: "This is a test email"
                , subject: "Testing Mail Sending"
                , client: client
                , toEmails: new[] { "somadinambadiwe@gmail.com" }
                , ccEmails: new[] { "somasystemsng@gmail.com" }
                , bccEmails: null
                , isBodyHtml: false
                , displayName: "SchcooSystem");
            Assert.AreEqual(true, sent);
        }

        [Test]
        public void SendEmail_no_body()
        {
            var sent = Emailer.SendEmail(senderEmail: "donotreply.qcpta@gmail.com"
                , body: null
                , subject: "Testing Mail Sending"
                , client: client
                , toEmails: new[] { "somadinambadiwe@gmail.com" }
                , ccEmails: new[] { "somasystemsng@gmail.com" }
                , bccEmails: null
                , isBodyHtml: false
                , displayName: "SchcooSystem");
            Assert.AreEqual(false, sent);
        }

        [Test]
        public void SendEmail_no_to()
        {
            var sent = Emailer.SendEmail(senderEmail: "donotreply.qcpta@gmail.com"
                , body: "This is a test email"
                , subject: "Testing Mail Sending"
                , client: client
                , toEmails: null
                , ccEmails: new[] { "somasystemsng@gmail.com" }
                , bccEmails: null
                , isBodyHtml: false
                , displayName: "SchcooSystem");
            Assert.AreEqual(false, sent);
        }
    }
}
