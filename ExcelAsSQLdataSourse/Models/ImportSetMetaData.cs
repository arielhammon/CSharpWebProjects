using System;
using System.ComponentModel.DataAnnotations;

namespace ExcelAsSQLdataSourse.Models
{
    [MetadataType(typeof(ImportSetMetaData))]
    public partial class ImportSet
    {
    }

    public class ImportSetMetaData
    {
        [StringLength(maximumLength: 32)]
        public string Segment { get; set; }

        [StringLength(maximumLength: 32)]
        public string Country { get; set; }

        [StringLength(maximumLength: 32)]
        public string Product { get; set; }

        [StringLength(maximumLength: 32)]
        public string DiscountBand { get; set; }

        [Range(minimum: 0, maximum: double.MaxValue)]
        public Nullable<double> UnitsSold { get; set; }

        [Range(minimum: 0, maximum: double.MaxValue)]
        public Nullable<double> ManufacturingPrice { get; set; }

        [Range(minimum: 0, maximum: double.MaxValue)]
        public Nullable<double> SalesPrice { get; set; }

        [Range(minimum: 0, maximum: double.MaxValue)]
        public Nullable<double> GrossSales { get; set; }

        [Range(minimum: 0, maximum: double.MaxValue)]
        public Nullable<double> Discounts { get; set; }

        [Range(minimum: 0, maximum: double.MaxValue)]
        public Nullable<double> Sales { get; set; }

        [Range(minimum: 0, maximum: double.MaxValue)]
        public Nullable<double> COGS { get; set; }

        public Nullable<double> Profit { get; set; }

        [DataType(DataType.Date)]
        public Nullable<System.DateTime> DateOf { get; set; }
    }
}