using System.ComponentModel.DataAnnotations;

namespace Mazadaty.Web.Areas.Admin.Models.Notifications
{
    public class SendViewModel
    {
        [Required]
        public string Topic { get; set; } = "/topics/all";

        [Required, StringLength(40)]
        public string Title { get; set; }

        [Required, StringLength(140)]
        public string Message { get; set; }
    }
}
