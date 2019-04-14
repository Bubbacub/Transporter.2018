namespace Imd.Transporter.Viewer.Data
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Transfer
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(50)]
        public string Transporter { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(250)]
        public string Filename { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(50)]
        public string Status { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(1000)]
        public string Destination { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? StartDate { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? EndDate { get; set; }

        public int? TransferTime { get; set; }

        public int? TransferSpeed { get; set; }

        [Key]
        [Column(Order = 4)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TransferId { get; set; }

        [Key]
        [Column(Order = 5)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TaskId { get; set; }
    }
}
