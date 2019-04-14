namespace Imd.Transporter.Viewer.Data
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Task
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TaskId { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string Transporter { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(250)]
        public string Filename { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(50)]
        public string Status { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? CreatedDate { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? ProcessingStartedDate { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? CompletedDate { get; set; }

        public int? ProcessingTime { get; set; }

        public int? TimeWaitingForFreeThread { get; set; }

        [StringLength(4000)]
        public string ErrorText { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? ErrorDate { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(1000)]
        public string WatchFolder { get; set; }
    }
}
