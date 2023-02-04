using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectMonitor.Api.Database.Models
{
    public class Config
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int RowId { get; set; }

        [Required]
        public string ToEmailAddress { get; set; }

        [Required]
        public string ToEmailAddressName { get; set; }

        [Required]
        public string FromEmailAddress { get; set; }

        [Required]
        public string FromEmailAddressName { get; set; }

        [Required]
        public string ApiKey { get; set; }

        [Required]
        public string PMErrorsTemplateId { get; set; }

        [Required]
        public string PMErrorsResolvedTemplateId { get; set; }

        [Required]
        public string HFConnectionString { get; set; }

        [Required]
        public int ChatId { get; set; }

        [Required]
        public string MMSApiKey { get; set; }
    }
}