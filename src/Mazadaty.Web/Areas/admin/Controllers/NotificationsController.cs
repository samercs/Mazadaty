using Mazadaty.Web.Areas.Admin.Models.Notifications;
using Mazadaty.Web.Controllers;
using Mazadaty.Web.Core.Attributes;
using Mazadaty.Web.Core.Identity;
using Mazadaty.Web.Core.Services;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Mazadaty.Web.Areas.Admin.Controllers
{
    [RouteArea("admin"), RoutePrefix("notifications"), RoleAuthorize(Role.Administrator)]
    public class NotificationsController : ApplicationController
    {
        public NotificationsController(IAppServices appServices) : base(appServices)
        {
        }

        [Route("send")]
        public ActionResult Send()
        {
            var model = new SendViewModel();
            return View(model);
        }

        [Route("send")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Send(SendViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://fcm.googleapis.com/fcm/");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("key", "=AAAAmI0Ykdw:APA91bGU5YkyqZAHfia_ZYIFWgyR8Qta-zA8CoR5gVpPxa-MGdUcx8eijnE8cE-hlhIn6Guck9532TbJ0X7ujeQE76F8_zy7Pkb5UEiT18PGd0rxuqPHmuRvR9CqSp1VYri832BYNDB0");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                await client.PostAsync("send", GetMessage(model));

                //var result = await client.PostAsync("send", GetMessage(model));
                //var response = await result.Content.ReadAsStringAsync();
            }

            SetStatusMessage("Message successfully sent.");
            return RedirectToAction(nameof(Send));
        }

        private static StringContent GetMessage(SendViewModel model)
        {
            var message = new
            {
                to = model.Topic,
                notification = new
                {
                    title = model.Title,
                    body = model.Message,
                    sound = "default"
                },
                data = new
                {
                    title = model.Title,
                    body = model.Message
                }
            };

            return new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json");
        }
    }
}
