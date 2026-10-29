#nullable enable

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentFoodParcelApp.Models
{
    [Table("DISTRIBUTIONREQUESTS")]
    public class Distributionrequest
    {
        [Key]
        [Column("DISTRIBUTIONREQUESTID")]
        public int Distributionrequestid { get; set; }

        [Column("DISTRIBUTIONID")]
        public int Distributionid { get; set; }

        [Column("REQUESTID")]
        public int Requestid { get; set; }

        [ForeignKey("Distributionid")]
        public virtual Distribution? Distribution { get; set; }

        [ForeignKey("Requestid")]
        public virtual Request? Request { get; set; }
    }
}