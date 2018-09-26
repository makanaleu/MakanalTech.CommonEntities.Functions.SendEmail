using MakanalTech.CommonEntities.Core;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Xml;

namespace MakanalTech.CommonEntities.Functions
{
    /// <summary>
    /// Sends an e-mail message through SendGrid using a 
    /// MakanalTech.CommonEntities.Core.EmailMessage.
    /// </summary>
    public static class SendEmail
    {
        /// <summary>
        /// Abstracts the properties of an EmailMessage to build a 
        /// SendGridMessage. The message is then sent using the required
        /// environment variable `SENDGRID_API_KEY`.
        /// </summary>
        /// <remarks>
        /// The e-mail subject should be set in EmailMessage.About.Name. The 
        /// plaintext content should be set in EmailMessage.About.Description.
        /// If HTML content is included, it should be set in EmailMessage.Text.
        /// </remarks>
        /// <param name="emailMessage">An e-mail message.</param>
        /// <returns>The response received from the API call to SendGrid</returns>
        public static async Task<Response> Send(EmailMessage emailMessage)
        {
            string apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            SendGridClient client = new SendGridClient(apiKey);

            string senderEmail; // required
            string senderName = null;

            string recipientEmail; // required
            string recipientName = null;

            // Set sender e-mail and name as an Organization (supersedes Person).
            if (emailMessage.Sender.AsOrganization != null)
            {
                Organization senderOrg = emailMessage.Sender.AsOrganization;
                senderEmail = senderOrg.Email.AsText;
                senderName = senderOrg.Name?.AsText;
            }
            // Set sender e-mail and name as a Person.
            else if (emailMessage.Sender.AsPerson != null)
            {
                Person senderPerson = emailMessage.Sender.AsPerson;

                senderEmail = senderPerson.Email.AsText;

                // Set sender name from full name (supersedes name parts).
                if (senderPerson.Name != null)
                {
                    senderName = senderPerson.Name.AsText;
                }
                // Set sender name from name parts.
                else if (senderPerson.FamilyName != null)
                {
                    senderName = senderPerson.GivenName.AsText
                        + " " + senderPerson.FamilyName.AsText;
                }
            }
            else
            {
                throw new ApplicationException(
                    "A sender Organization or Person is required."
                );
            }

            // Set recipient e-mail and name as an Organization (supersedes Person).
            if (emailMessage.ToRecipient.AsOrganization != null)
            {
                Organization recipOrg = emailMessage.ToRecipient.AsOrganization;
                recipientEmail = recipOrg.Email.AsText;
                recipientName = recipOrg.Name?.AsText;
            }
            // Set recipient e-mail and name as a Person.
            else if (emailMessage.ToRecipient.AsPerson != null)
            {
                Person recipPerson = emailMessage.ToRecipient.AsPerson;

                recipientEmail = recipPerson.Email.AsText;

                // Set recipient name from full name (supersedes name parts).
                if (recipPerson.Name != null)
                {
                    recipientName = recipPerson.Name.AsText;
                }
                // Set recipient name from name parts.
                else if (recipPerson.FamilyName != null)
                {
                    recipientName = recipPerson.GivenName.AsText
                        + " " + recipPerson.FamilyName.AsText;
                }
            }
            else
            {
                throw new ApplicationException(
                    "A recipient Organization or Person is required."
                );
            }

            SendGridMessage sendGridMessage = new SendGridMessage()
            {
                From = new EmailAddress(senderEmail, senderName),
                Subject = emailMessage.About?.Name?.AsText,
                PlainTextContent = emailMessage.About?.Description?.AsText,
                HtmlContent = emailMessage.Text?.AsText
            };

            // TODO Add ccRecipient and bccRecipient.
            sendGridMessage.AddTo(new EmailAddress(recipientEmail, recipientName));

            // TODO Add response handler.
            Response response = await client.SendEmailAsync(sendGridMessage);
            return response;
        }

        /// <summary>
        /// Useful when loading an EmailMessage from Blob Storage or as a Blob
        /// Trigger input.
        /// </summary>
        /// <param name="blobStream">Serialized EmailMessage as a Stream.</param>
        /// <returns></returns>
        public static EmailMessage DeserializeEmailMessage(Stream blobStream)
        {
            DataContractSerializer dataContractSerializer =
                new DataContractSerializer(typeof(EmailMessage));

            blobStream.Position = 0;

            using (XmlReader xmlReader = XmlReader.Create(blobStream))
            {
                EmailMessage emailMessage =
                    (EmailMessage)dataContractSerializer.ReadObject(xmlReader);
                return emailMessage;
            }
        }

        /// <summary>
        /// Serialize an EmailMessage as an XML string.
        /// </summary>
        /// <param name="emailMessage">An e-mail message.</param>
        /// <returns></returns>
        public static string SerializeEmailMessage(EmailMessage emailMessage)
        {
            DataContractSerializer dataSerializer =
                new DataContractSerializer(emailMessage.GetType());

            using (MemoryStream memoryStream = new MemoryStream())
            {
                dataSerializer.WriteObject(memoryStream, emailMessage);

                memoryStream.Seek(0, SeekOrigin.Begin);
                using (StreamReader streamReader = new StreamReader(memoryStream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }
    }
}
