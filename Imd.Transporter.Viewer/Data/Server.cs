namespace Imd.Transporter.Viewer.Data
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Server")]
    public partial class Server
    {
        [Key]
        [Column(Order = 0)]
        public int ServerId { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string Name { get; set; }

        [Key]
        [Column(Order = 2)]
        public bool Active { get; set; }

        [StringLength(100)]
        public string Description { get; set; }
    }
}
