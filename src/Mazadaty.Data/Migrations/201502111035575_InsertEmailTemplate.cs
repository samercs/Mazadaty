using System.Collections.Generic;
using Mazadaty.Models;
using Mazadaty.Models.Enum;
using Mazadaty.Models.Enums;
using OrangeJetpack.Localization;

namespace Mazadaty.Data.Migrations
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
                        new LocalizedContent("ar", "{0} ???C? E? ??")
                    }).Serialize(),
                    Message = (new[]
                    {
                        new LocalizedContent("en", @"<p>Hi {0}!</p><p>I wanted to personally thank you for registering on the K7L cosmetics website. As a &#39;Thank You&#39;, please find below a 20% discount coupon for you to use on your purchase, which I hope will be soon?</p><p>I look forward to hearing your reviews and feedback on your purchases in the future. If you have any tips or ideas you would like to share with me, I would LOVE to hear from you! Please email me at <a href=""mailto:areej@K7L.com"">areej@K7L.com</a>.</p><p>If you have not joined us on social media yet, please find us at @k7lcosmetics on Instagram and Twitter, and K7L Cosmetics on Facebook. Make sure you follow us for a chance to win lots of products! Dont forget to tag us with #k7lcosmetics so we can find you!</p><p>Stay Pretty!</p><p>Areej</p><p>&nbsp;</p><p>20% Discount Code: <strong>{1}</strong></p>"),
                        new LocalizedContent("ar", @"<p>???EC? {0}!</p><p>??I? ?? ?O??? OI??C? ??? E???? ?? ???? ??E???CE E???? K7L. ???? ?? ?C ??? ????E I?? E??EE 20% ????? C?C?E?CIE ???C ???E?? O?? ?? ?E?? ??I ?E?C? ?? ????E O?C? ?E? ??C C?????. ?C EE?I?? EC?EIIC??C!</p><p>??C ?E??? ?I?C? ???C? E????CE? ?A?C?? EO?? ?OE??CE?? ???C ?C? ?I?? ?? ??C?? ?? ???C? E?I?? ?OC??E?C ???? ????? ?? I?C?? ????? C?E?C?? ???! ?C????? ?? ???? ???  <a href=""mailto:areej@K7L.co"">areej@K7L.com</a>.</p><p>??C ?? E???? E?I ??? ???CE?C ??? OE?CE C?E?C?? C?C?E?C??? CE?E? ??  @k7lcosmetics ??? ???EU?C? ? E??E? ??? K7L Costmetics . C???? ??? ?? EECE???C ????E C???? EC??E?? ?? C???E?CE! ??C E??? ?? E??? ?CO ECU # hash tag?  #k7lcosmetics ??? ??? ?C EOC????? ??C! </p><p>?O??C?!</p><p>????</p><p>&nbsp;</p><p>??? I?? 20%  <strong>{1}</strong></p>")
                    }).Serialize()
                },
                new EmailTemplate
                {
                    TemplateType = EmailTemplateType.PasswordReset,
                    Description = "Sent when a user requests a password reset.",
                    Subject = (new[]
                    {
                        new LocalizedContent("en", "Reset Password"),
                        new LocalizedContent("ar", "??CIE ?E? ???E C?????")
                    }).Serialize(),
                    Message = (new[]
                    {
                        new LocalizedContent("en", @"<p>Hello {0}, we have received a request to reset your password. Please follow the link below to begin resetting your password.</p><p><a href=""{1}"">Reset Password</a></p>"),
                        new LocalizedContent("ar", @"<p>???EC? {0}? ??I E????C ??EC? ???CIE ?E? ???E ?????. CEE?? ?? ???? C??CE? ?I?C? ??EI? E??CIE ?E? ???E ?????.</p><p><a href=""{1}"">??CIE ?E? ???E C?????</a></p>")
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
                        new LocalizedContent("ar", "E? EU??? ???E C?????")
                    }).Serialize(),
                    Message = (new[]
                    {
                        new LocalizedContent("en", @"<p>Hi {0}, this email is just to confirm your password was recently changed on the {1} website. If you did not make this change please let us know.</p>"),
                        new LocalizedContent("ar", @"<p>???EC? {0}? ???? ?? ??? C???C?E ??E???I ??? ???? ??E EEU??? ???E ????? ??I?C? ??? ???? {1} C????E????.</p>")
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
                        new LocalizedContent("ar", "E? EU??? ???C? C?E??I C????E????")
                    }).Serialize(),
                    Message = (new[]
                    {
                        new LocalizedContent("en", @"Hi {0}, this email is just to confirm your email address was recently changed on the {1} website. If you did not make this change please let us know."),
                        new LocalizedContent("ar", @"???EC? {0}? ???? ?? ??? C???C?E ??E???I ??? ???? E? EU??? ???C? E??I? C????E???? ??I?C? ??? ???? {1} C????E????. C???C? ?E?CU?C ?? ?C? ?? E???? ??E E??C C?EU???.")
                    }).Serialize()
                },
                new EmailTemplate
                {
                    TemplateType = EmailTemplateType.TrophyEarned,
                    Description = "Send as a confirmation that a trophies have been earned.",
                    Subject = (new[]
                    {
                        new LocalizedContent("en", "Trophy"),
                        new LocalizedContent("ar", "ﬂ√”")
                    }).Serialize(),
                    Message = (new[]
                    {
                        new LocalizedContent("en", @"Hi {0}, this email is just to inform you have earned trophies below:</br></br>{1}"),
                        new LocalizedContent("ar", @"„»—Êﬂ «‰  ›“  »«·ﬂƒÊ” «·√ Ì… </br></br>{1}")
                    }).Serialize()
                },
                new EmailTemplate
                {
                    TemplateType = EmailTemplateType.LevelUp,
                    Description="Send as confirmation that a user level has been up",
                    Subject = (new[] {
                        new LocalizedContent("en",@"Level up"),
                        new LocalizedContent("ar",@"„” ÊÏ √⁄·Ï")
                    }).Serialize(),
                    Message = (new[] {
                        new LocalizedContent("en",@"Hi {0}, this email is just to inform you moved one level up: your new level is {1}"),
                        new LocalizedContent("ar",@"{0}, „»—Êﬂ «‰  «·√‰ ›Ï «·„” ÊÏ {1}")
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
