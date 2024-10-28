using PurchasingSystemDeveloper.Repositories;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PurchasingSystemDeveloper.Areas.MasterData.Models
{
    [Table("MstMeasurement", Schema = "dbo")]
    public class Measurement : UserActivity
    {
        [Key]
        public Guid MeasurementId { get; set; }
        public string MeasurementCode { get; set; }
        public string MeasurementName { get; set; }
        public string? Note { get; set; }
    }
}
