using MakanalTech.CommonEntities.Core;
using MakanalTech.CommonEntities.DataType;
using MakanalTech.CommonEntities.MultiType.Combo;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MakanalTech.CommonEntities.Functions.Test
{
    /// <summary>
    /// Test the integration with SendGrid.
    /// </summary>
    [TestClass]
    public class SendMailTest
    {
        /// <summary>
        /// Allows integration testing with SendGrid. By default, this test
        /// is ignored to avoid inadvertently sending real e-mails.
        /// </summary>
        /// <remarks>
        /// To use this test, ensure the environment variable `SENDGRID_API_KEY`
        /// is set along with valid e-mail addresses for the `Sender` and
        /// `ToRecipient`.
        /// </remarks>
        [TestMethod, Ignore]
        public void SendEmail_EmailMessage_ToSendGrid()
        {
            EmailMessage emailMessage = new EmailMessage()
            {
                Sender = new AudienceOrOgranizationOrPerson()
                {
                    AsOrganization = new Organization()
                    {
                        Email = new Text("sender@example.com"),
                        Name = new Text("Common Entities")
                    }
                },
                ToRecipient = new Recipient()
                {
                    AsPerson = new Person()
                    {
                        Email = new Text("recipient@example.com"),
                        Name = new Text("Application Tester")
                    }
                },
                About = new Thing()
                {
                    Name = new Text("Sample e-mail from Common Entities."),
                    Description = new Text(
                        "This is the plaintext content sent in the EmailMessage" +
                        " body." +
                        "\r\n" +
                        "This is the second paragraph of the plaintext content.")
                },
                Text = new Text(
                    "<p>This is the <b>HTML content</b> sent in the EmailMessage body.</p>" +
                    "<p>This is the second paragraph of the HTML content.</p>")
            };

            SendEmail.Send(emailMessage).Wait();
        }
    }
}
