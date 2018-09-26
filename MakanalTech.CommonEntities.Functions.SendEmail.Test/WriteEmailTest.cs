using MakanalTech.CommonEntities.Core;
using MakanalTech.CommonEntities.Core.Intangible;
using MakanalTech.CommonEntities.DataType;
using MakanalTech.CommonEntities.MultiType.Combo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using ItemList = MakanalTech.CommonEntities.Listify.Intangible.ItemList;

namespace MakanalTech.CommonEntities.Functions.Test
{
    [TestClass]
    public class WriteEmailTest
    {
        [TestMethod]
        public void Assert_MailMerge_Replaces_Recipient()
        {
            ItemList valueList = ValueList();
            EmailMessage emailTemplate = EmailTemplate();

            EmailMessage mergedMessage =
                WriteEmail.MailMerge(emailTemplate, valueList);

            Assert.AreEqual(
                "randall@example.com",
                mergedMessage.ToRecipient.AsPerson.Email.AsText
            );
        }

        [TestMethod]
        public void Assert_MailMerge_Replaces_Placeholders()
        {
            ItemList valueList = ValueList();
            EmailMessage emailTemplate = EmailTemplate();

            EmailMessage mergedMessage =
                WriteEmail.MailMerge(emailTemplate, valueList);

            Assert.AreEqual(
                "Dear Randall,",
                mergedMessage.Text.AsText
            );
        }

        private ItemList ValueList()
        {
            List<ListItemThingOrText> itemListElements = new List<ListItemThingOrText>
            {
                new ListItemThingOrText()
                {
                    AsListItem = new ListItem()
                    {
                        Item = new Thing()
                        {
                            Name = new Text("ToRecipientEmail"),
                            Description = new Text("randall@example.com")
                        }
                    }
                },
                new ListItemThingOrText()
                {
                    AsListItem = new ListItem()
                    {
                        Item = new Thing()
                        {
                            Name = new Text("CustomerFullName"),
                            Description = new Text("Randall Jenkins")
                        }
                    }
                },
                new ListItemThingOrText()
                {
                    AsListItem = new ListItem()
                    {
                        Item = new Thing()
                        {
                            Name = new Text("CustomerGivenName"),
                            Description = new Text("Randall")
                        }
                    }
                },
                new ListItemThingOrText()
                {
                    AsListItem = new ListItem()
                    {
                        Item = new Thing()
                        {
                            Name = new Text("CustomerFamilyName"),
                            Description = new Text("Jenkins")
                        }
                    }
                }
            };

            return new ItemList()
            {
                ItemListElements = itemListElements
            };
        }

        private EmailMessage EmailTemplate()
        {
            return new EmailMessage()
            {
                Sender = new AudienceOrOgranizationOrPerson()
                {
                    AsOrganization = new Organization()
                    {
                        Email = new Text("sender@example.com"),
                        Name = new Text("Common Entities")
                    }
                },
                Text = new Text("Dear {CustomerGivenName},")
            };
        }
    }
}
