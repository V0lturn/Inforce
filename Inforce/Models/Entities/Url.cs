using System.ComponentModel.DataAnnotations;

namespace Inforce_Task.Models.Entities
{
    public class Url
    {
        [Key]
        public Guid UrlId { get; set; }
        public string UrlText {  get; set; }
        public string ShortenUrl { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        
        public Url(string urlText, string shortenUrl, Guid createdBy)
        {
            UrlId = Guid.NewGuid();
            UrlText = urlText;
            ShortenUrl = shortenUrl;
            CreatedBy = createdBy;
            CreatedDate = DateTime.Now;
        }

    }
}
