namespace Mazadaty.Services.Messaging
{
    public class Email
    {
        public Email()
        {
            
        }

        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public string Message { get; set; }
        public string PreviewText { get; set; }
        public string Subject { get; set; }
        
    }
}
