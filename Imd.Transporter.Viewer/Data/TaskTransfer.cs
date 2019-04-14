namespace Imd.Transporter.Viewer.Data
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TaskTransfer")]
    public partial class TaskTransfer
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

        [StringLength(50)]
        public string Status { get; set; }

        [StringLength(1000)]
        public string WatchFolder { get; set; }

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

        [StringLength(1000)]
        public string Destination { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? TransferStarted { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? TransferCompleted { get; set; }

        public int? TransferTime { get; set; }

        public int? TransferSpeed { get; set; }

        public int StatusId { get; set; }
    }
}
