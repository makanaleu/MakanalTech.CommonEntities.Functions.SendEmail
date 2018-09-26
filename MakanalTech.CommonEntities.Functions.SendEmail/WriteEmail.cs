using MakanalTech.CommonEntities.Core;
using MakanalTech.CommonEntities.DataType;
using MakanalTech.CommonEntities.Listify.Intangible;
using MakanalTech.CommonEntities.MultiType.Combo;

namespace MakanalTech.CommonEntities.Functions
{
    /// <summary>
    /// Utilities for writing an EmailMessage including MailMerge and
    /// SerializeEmailMessage.
    /// </summary>
    public static class WriteEmail
    {
        /// <summary>
        /// Merges a ItemList containing a list of values with an EmailMessage
        /// template.
        /// </summary>
        /// <param name="emailTemplate">
        /// A partially complete EmailMessage containing the plaintext content
        /// and if desired the HTML content with interpolation expressions.
        /// Other EmailMessage properties may also be pre-filled.
        /// </param>
        /// <param name="valueList">
        /// A list of values to insert into the e-mail template. The ItemList
        /// should contain only ListItems, using ListItem.Name as the item
        /// key and ListItem.Description as the item value.
        /// 
        /// If valueList contains recipient or sender values, they will override
        /// the emailTemplate preset values.
        /// 
        /// Values from valueList are replaced into the e-mail subject,
        /// plaintext content and HTML content when matched to a `{Placeholder}`.
        /// </param>
        /// <returns>An EmailMessage containing the merged text.</returns>
        public static EmailMessage MailMerge(
            EmailMessage emailTemplate,
            ItemList valueList)
        {
            // Check if recipient e-mail is included in valueList.
            ListItemThingOrText toRecipientEmail = valueList.ItemListElements?
                .Find(x => x.AsListItem.Item.Name.AsText == "ToRecipientEmail");

            // Check if recipient name is included in valueList.
            ListItemThingOrText toRecipientName = valueList.ItemListElements?
                .Find(x => x.AsListItem.Item.Name.AsText == "ToRecipientName");

            // Check if sender e-mail is included in valueList.
            ListItemThingOrText senderEmail = valueList.ItemListElements?
                .Find(x => x.AsListItem.Item.Name.AsText == "SenderEmail");

            // Check if sender name is included in valueList.
            ListItemThingOrText senderName = valueList.ItemListElements?
                .Find(x => x.AsListItem.Item.Name.AsText == "SenderName");

            
            if (toRecipientEmail != null || toRecipientName != null)
            {
                // Set up ToRecipient object if not already in template.
                if (emailTemplate.ToRecipient == null)
                {
                    emailTemplate.ToRecipient = new Recipient()
                    {
                        AsPerson = new Person()
                    };
                }

                // Override template recipient e-mail if set in valueList.
                if (toRecipientEmail != null)
                {
                    emailTemplate.ToRecipient.AsPerson.Email =
                        toRecipientEmail.AsListItem.Item.Description;
                }

                // Override template recipient name if set in valueList.
                if (toRecipientName != null)
                {
                    emailTemplate.ToRecipient.AsPerson.Name =
                        toRecipientName.AsListItem.Item.Description;
                }
            }

            if (senderEmail != null || senderName != null)
            {
                // Set up Sender object if not already in template.
                if (emailTemplate.Sender == null)
                {
                    emailTemplate.Sender = new AudienceOrOgranizationOrPerson()
                    {
                        AsPerson = new Person()
                    };
                }

                // Override template sender e-mail if set in valueList.
                if (senderEmail != null)
                {
                    emailTemplate.Sender.AsPerson.Email =
                        senderEmail.AsListItem.Item.Description;
                }

                // Override template sender name if set in valueList.
                if (senderName != null)
                {
                    emailTemplate.Sender.AsPerson.Email =
                        senderName.AsListItem.Item.Description;
                }
            }

            // Replace valueList items into template when matched to {Placeholders}.
            foreach (ListItemThingOrText value in valueList.ItemListElements)
            {
                // Replace {Placeholders} in the e-mail subject.
                if (emailTemplate.About?.Name != null)
                {
                    emailTemplate.About.Name = new Text(
                        emailTemplate.About.Name.AsText.Replace(
                            "{" + value.AsListItem.Item.Name.AsText + "}",
                            value.AsListItem.Item.Description.AsText
                        )
                    );
                }

                // Replace {Placeholders} in the e-mail plaintext content.
                if (emailTemplate.About?.Description != null)
                {
                    emailTemplate.About.Description = new Text(
                        emailTemplate.About.Description.AsText.Replace(
                            "{" + value.AsListItem.Item.Name.AsText + "}",
                            value.AsListItem.Item.Description.AsText
                        )
                    );
                }

                // Replace {Placeholders} in the e-mail HTML content.
                if (emailTemplate.Text != null)
                {
                    emailTemplate.Text = new Text(
                        emailTemplate.Text.AsText.Replace(
                            "{" + value.AsListItem.Item.Name.AsText + "}",
                            value.AsListItem.Item.Description.AsText
                         )
                    );
                }
            }
            return emailTemplate;
        }
    }
}
