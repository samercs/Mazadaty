using System.Collections.Generic;
using Mzayad.Models;
using Mzayad.Models.Enum;
using OrangeJetpack.Localization;

namespace Mzayad.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class InsertEmailTemplate : DbMigration
    {
        public override void Up()
        {
            var emailTemplates = new List<EmailTemplate>
            {
                new EmailTemplate
                {
                    TemplateType = EmailTemplateType.AccountRegistration,
                    Description = "Sent as a welcome message to newly registered users.",
                    Subject = (new[]
                    {
                        new LocalizedContent("en", "Welcome to {0}"),
                        new LocalizedContent("ar", "{0} ÃåáÇð Èß Ýí")
                    }).Serialize(),
                    Message = (new[]
                    {
                        new LocalizedContent("en", @"<p>Hi {0}!</p><p>I wanted to personally thank you for registering on the K7L cosmetics website. As a &#39;Thank You&#39;, please find below a 20% discount coupon for you to use on your purchase, which I hope will be soon?</p><p>I look forward to hearing your reviews and feedback on your purchases in the future. If you have any tips or ideas you would like to share with me, I would LOVE to hear from you! Please email me at <a href=""mailto:areej@K7L.com"">areej@K7L.com</a>.</p><p>If you have not joined us on social media yet, please find us at @k7lcosmetics on Instagram and Twitter, and K7L Cosmetics on Facebook. Make sure you follow us for a chance to win lots of products! Dont forget to tag us with #k7lcosmetics so we can find you!</p><p>Stay Pretty!</p><p>Areej</p><p>&nbsp;</p><p>20% Discount Code: <strong>{1}</strong></p>"),
                        new LocalizedContent("ar", @"<p>ãÑÍÈÇð {0}!</p><p>ÃæÏø Ãä ÃÔßÑß ÔÎÕíÇð Úáì ÊÓÌáß Ýí ãæÞÚ ãÓÊÍÖÑÇÊ ÊÌãíá K7L. Åáíß Ýí ãÇ íáí ÞÓíãÉ ÎÕã ÈäÓÈÉ 20% íãßäß ÇáÇÓÊÝÇÏÉ ãäåÇ ßÚÑÈæä ÔßÑ ãä ÞÈáí ÚäÏ ÅÊãÇã Ãí ÚãáíÉ ÔÑÇÁ ÚÈÑ åÐÇ ÇáãæÞÚ. áÇ ÊÊÃÎÑí ÈÇÓÊÎÏÇãåÇ!</p><p>ÃäÇ ÃÊØáÚ ÞÏãÇð áÓãÇÚ ÊÚáíÞÇÊß æÂÑÇÆß ÈÔÃä ãÔÊÑíÇÊß¡ ÝÅÐÇ ßÇä áÏíß Ãí äÕÇÆÍ Ãæ ÃÝßÇÑ ÊæÏíä ãÔÇÑßÊåÇ ãÚí¡ Óíßæä ãä ÏæÇÚí ÓÑæÑí ÇáÊæÇÕá ãÚß! ÑÇÓáíäí ãä ÝÖáß Úáì  <a href=""mailto:areej@K7L.co"">areej@K7L.com</a>.</p><p>ÅÐÇ áã ÊäÖãí ÈÚÏ Åáì ÕÝÍÇÊäÇ Úáì ÔÈßÇÊ ÇáÊæÇÕá ÇáÇÌÊãÇÚí¡ ÇÈÍËí Úä  @k7lcosmetics Úáì ÅäÓÊÛÑÇã æ ÊæíÊÑ æÚä K7L Costmetics . ÇÍÑÕí Úáì Ãä ÊÊÇÈÚíäÇ áÝÑÕÉ ÇáÝæÒ ÈÇáßËíÑ ãä ÇáãäÊÌÇÊ! æáÇ ÊäÓí Ãä ÊÖÚí åÇÔ ÊÇÛ # hash tagþ  #k7lcosmetics áßí äÑì ãÇ ÊÔÇÑßíäå ÚäÇ! </p><p>æÔßÑÇð!</p><p>ÃÑíÌ</p><p>&nbsp;</p><p>ÑãÒ ÎÕã 20%  <strong>{1}</strong></p>")
                    }).Serialize()
                },
                new EmailTemplate
                {
                    TemplateType = EmailTemplateType.PasswordReset,
                    Description = "Sent when a user requests a password reset.",
                    Subject = (new[]
                    {
                        new LocalizedContent("en", "Reset Password"),
                        new LocalizedContent("ar", "ÅÚÇÏÉ ÖÈØ ßáãÉ ÇáãÑæÑ")
                    }).Serialize(),
                    Message = (new[]
                    {
                        new LocalizedContent("en", @"<p>Hello {0}, we have received a request to reset your password. Please follow the link below to begin resetting your password.</p><p><a href=""{1}"">Reset Password</a></p>"),
                        new LocalizedContent("ar", @"<p>ãÑÍÈÇð {0}¡ áÞÏ ÊáÞíäÇ ØáÈÇð áÅÚÇÏÉ ÖÈØ ßáãÉ ãÑæÑß. ÇÊÈÚí ãä ÝÖáß ÇáÑÇÈØ ÃÏäÇå ááÈÏÁ ÈÅÚÇÏÉ ÖÈØ ßáãÉ ãÑæÑß.</p><p><a href=""{1}"">ÅÚÇÏÉ ÖÈØ ßáãÉ ÇáãÑæÑ</a></p>")
                    }).Serialize()
                }
                ,
                new EmailTemplate
                {
                    TemplateType = EmailTemplateType.PasswordChanged,
                    Description = "Sent as a confirmation that a password was changed.",
                    Subject = (new[]
                    {
                        new LocalizedContent("en", "Password Changed"),
                        new LocalizedContent("ar", "Êã ÊÛííÑ ßáãÉ ÇáãÑæÑ")
                    }).Serialize(),
                    Message = (new[]
                    {
                        new LocalizedContent("en", @"<p>Hi {0}, this email is just to confirm your password was recently changed on the {1} website. If you did not make this change please let us know.</p>"),
                        new LocalizedContent("ar", @"<p>ãÑÍÈÇð {0}¡ äÑÓá áß åÐå ÇáÑÓÇáÉ ááÊÃßíÏ Úáì Ãäøß ÞãÊ ÈÊÛííÑ ßáãÉ ãÑæÑß ãÄÎÑÇð Úáì ãæÞÚ {1} ÇáÅáßÊÑæäí.</p>")
                    }).Serialize()
                }
                ,
                new EmailTemplate
                {
                    TemplateType = EmailTemplateType.EmailChanged,
                    Description = "Sent as a confirmation that an email was changed.",
                    Subject = (new[]
                    {
                        new LocalizedContent("en", "Email Changed"),
                        new LocalizedContent("ar", "Êã ÊÛííÑ ÚäæÇä ÇáÈÑíÏ ÇáÅáßÊÑæäí")
                    }).Serialize(),
                    Message = (new[]
                    {
                        new LocalizedContent("en", @"Hi {0}, this email is just to confirm your email address was recently changed on the {1} website. If you did not make this change please let us know."),
                        new LocalizedContent("ar", @"ãÑÍÈÇð {0}¡ äÑÓá áß åÐå ÇáÑÓÇáÉ ááÊÃßíÏ Úáì Ãäøå Êã ÊÛííÑ ÚäæÇä ÈÑíÏß ÇáÅáßÊÑæäí ãÄÎÑÇð Úáì ãæÞÚ {1} ÇáÅáßÊÑæäí. ÇáÑÌÇÁ ÅÈáÇÛäÇ Ýí ÍÇá áã ÊÞæãí ÃäÊ ÈåÐÇ ÇáÊÛííÑ.")
                    }).Serialize()
                },
                new EmailTemplate
                {
                    TemplateType = EmailTemplateType.TrohpyEarned,
                    Description = "Send as a confirmation that a trophies have been earned",
                    Subject = (new[]
                    {
                        new LocalizedContent("en", "Trophy"),
                        new LocalizedContent("ar", "كأس")
                    }).Serialize(),
                    Message = (new[]
                    {
                        new LocalizedContent("en", @"Hi {0}, this email is just to inform you have earned trophies below:</br></br>{1}"),
                        new LocalizedContent("ar", @"مبروك انت فزت بالكؤوس الأتية </br></br>{1}")
                    }).Serialize()
                },
                new EmailTemplate
                {
                    TemplateType = EmailTemplateType.LevelUp,
                    Description="Send as confirmation that a user level has been up",
                    Subject = (new[] {
                        new LocalizedContent("en",@"Level up"),
                        new LocalizedContent("ar",@"مستوى أعلى")
                    }).Serialize(),
                    Message = (new[] {
                        new LocalizedContent("en",@"Hi {0}, this email is just to inform you moved one level up: your new level is {1}"),
                        new LocalizedContent("ar",@"{0}, مبروك انت الأن فى المستوى {1}")
                    }).Serialize()
                }


            };

            foreach (var emailTemplate in emailTemplates)
            {
                Sql(string.Format("INSERT INTO [dbo].[EmailTemplates] ([TemplateType], [Description], [Subject], [Message]) VALUES ({0}, N'{1}', N'{2}', N'{3}');", (int)emailTemplate.TemplateType, emailTemplate.Description, emailTemplate.Subject, emailTemplate.Message));
            }
        }

        public override void Down()
        {
        }
    }
}
