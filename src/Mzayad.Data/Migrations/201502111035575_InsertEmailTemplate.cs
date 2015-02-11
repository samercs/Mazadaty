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
                        new LocalizedContent("ar", "{0} ÃåáÇğ Èß İí")
                    }).Serialize(),
                    Message = (new[]
                    {
                        new LocalizedContent("en", @"<p>Hi {0}!</p><p>I wanted to personally thank you for registering on the K7L cosmetics website. As a &#39;Thank You&#39;, please find below a 20% discount coupon for you to use on your purchase, which I hope will be soon?</p><p>I look forward to hearing your reviews and feedback on your purchases in the future. If you have any tips or ideas you would like to share with me, I would LOVE to hear from you! Please email me at <a href=""mailto:areej@K7L.com"">areej@K7L.com</a>.</p><p>If you have not joined us on social media yet, please find us at @k7lcosmetics on Instagram and Twitter, and K7L Cosmetics on Facebook. Make sure you follow us for a chance to win lots of products! Dont forget to tag us with #k7lcosmetics so we can find you!</p><p>Stay Pretty!</p><p>Areej</p><p>&nbsp;</p><p>20% Discount Code: <strong>{1}</strong></p>"),
                        new LocalizedContent("ar", @"<p>ãÑÍÈÇğ {0}!</p><p>ÃæÏø Ãä ÃÔßÑß ÔÎÕíÇğ Úáì ÊÓÌáß İí ãæŞÚ ãÓÊÍÖÑÇÊ ÊÌãíá K7L. Åáíß İí ãÇ íáí ŞÓíãÉ ÎÕã ÈäÓÈÉ 20% íãßäß ÇáÇÓÊİÇÏÉ ãäåÇ ßÚÑÈæä ÔßÑ ãä ŞÈáí ÚäÏ ÅÊãÇã Ãí ÚãáíÉ ÔÑÇÁ ÚÈÑ åĞÇ ÇáãæŞÚ. áÇ ÊÊÃÎÑí ÈÇÓÊÎÏÇãåÇ!</p><p>ÃäÇ ÃÊØáÚ ŞÏãÇğ áÓãÇÚ ÊÚáíŞÇÊß æÂÑÇÆß ÈÔÃä ãÔÊÑíÇÊß¡ İÅĞÇ ßÇä áÏíß Ãí äÕÇÆÍ Ãæ ÃİßÇÑ ÊæÏíä ãÔÇÑßÊåÇ ãÚí¡ Óíßæä ãä ÏæÇÚí ÓÑæÑí ÇáÊæÇÕá ãÚß! ÑÇÓáíäí ãä İÖáß Úáì  <a href=""mailto:areej@K7L.co"">areej@K7L.com</a>.</p><p>ÅĞÇ áã ÊäÖãí ÈÚÏ Åáì ÕİÍÇÊäÇ Úáì ÔÈßÇÊ ÇáÊæÇÕá ÇáÇÌÊãÇÚí¡ ÇÈÍËí Úä  @k7lcosmetics Úáì ÅäÓÊÛÑÇã æ ÊæíÊÑ æÚä K7L Costmetics . ÇÍÑÕí Úáì Ãä ÊÊÇÈÚíäÇ áİÑÕÉ ÇáİæÒ ÈÇáßËíÑ ãä ÇáãäÊÌÇÊ! æáÇ ÊäÓí Ãä ÊÖÚí åÇÔ ÊÇÛ # hash tagş  #k7lcosmetics áßí äÑì ãÇ ÊÔÇÑßíäå ÚäÇ! </p><p>æÔßÑÇğ!</p><p>ÃÑíÌ</p><p>&nbsp;</p><p>ÑãÒ ÎÕã 20%  <strong>{1}</strong></p>")
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
                        new LocalizedContent("ar", @"<p>ãÑÍÈÇğ {0}¡ áŞÏ ÊáŞíäÇ ØáÈÇğ áÅÚÇÏÉ ÖÈØ ßáãÉ ãÑæÑß. ÇÊÈÚí ãä İÖáß ÇáÑÇÈØ ÃÏäÇå ááÈÏÁ ÈÅÚÇÏÉ ÖÈØ ßáãÉ ãÑæÑß.</p><p><a href=""{1}"">ÅÚÇÏÉ ÖÈØ ßáãÉ ÇáãÑæÑ</a></p>")
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
                        new LocalizedContent("ar", @"<p>ãÑÍÈÇğ {0}¡ äÑÓá áß åĞå ÇáÑÓÇáÉ ááÊÃßíÏ Úáì Ãäøß ŞãÊ ÈÊÛííÑ ßáãÉ ãÑæÑß ãÄÎÑÇğ Úáì ãæŞÚ {1} ÇáÅáßÊÑæäí.</p>")
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
                        new LocalizedContent("ar", @"ãÑÍÈÇğ {0}¡ äÑÓá áß åĞå ÇáÑÓÇáÉ ááÊÃßíÏ Úáì Ãäøå Êã ÊÛííÑ ÚäæÇä ÈÑíÏß ÇáÅáßÊÑæäí ãÄÎÑÇğ Úáì ãæŞÚ {1} ÇáÅáßÊÑæäí. ÇáÑÌÇÁ ÅÈáÇÛäÇ İí ÍÇá áã ÊŞæãí ÃäÊ ÈåĞÇ ÇáÊÛííÑ.")
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
