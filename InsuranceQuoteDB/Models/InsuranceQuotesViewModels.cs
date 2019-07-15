using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InsuranceQuoteDB.Models
{
    public class AutoMakeVM
    {
        public AutoMakeVM() : this(new AutoMake()) { }
        public AutoMakeVM(AutoMake x)
        {
            if (x != null)
            {
                Id = x.Id;
                MakeName = x.MakeName;
            }
        }
        public int Id { get; set; }
        public string MakeName { get; set; }
    }

    public class AutoModelVM
    {
        public AutoModelVM() : this(new AutoModel()) { }
        public AutoModelVM(AutoModel x)
        {
            if (x != null)
            {
                Id = x.Id;
                AutoMakeID = x.AutoMakeID;
                ModelName = x.ModelName;
                ModelYear = x.ModelYear;

                AutoMake = new AutoMakeVM(x.AutoMake);
            }
        }
        [Required(ErrorMessage = "Auto model is required.")]
        [Range(1,int.MaxValue, ErrorMessage = "Auto model is required.")]
        public int Id { get; set; }
        public Nullable<int> AutoMakeID { get; set; }
        public string ModelName { get; set; }
        public Nullable<int> ModelYear { get; set; }

        public virtual AutoMakeVM AutoMake { get; set; }
    }

    public class AutoOptionVM
    {
        public AutoOptionVM() : this(new AutoOption()) { }
        public AutoOptionVM (AutoOption x)
        {
            if (x != null)
            {
                Id = x.Id;
                AutoModelId = x.AutoModelId;
                OptionDescription = x.OptionDescription;

                AutoModel = new AutoModelVM(x.AutoModel);
            }
        }
        public int Id { get; set; }
        public Nullable<int> AutoModelId { get; set; }
        public string OptionDescription { get; set; }

        public virtual AutoModelVM AutoModel { get; set; }
    }

    public class CoverageLevelVM
    {
        public CoverageLevelVM() : this(new CoverageLevel()) { }
        public CoverageLevelVM(CoverageLevel x)
        {
            if (x != null)
            {
                Id = x.Id;
                Name = x.Name;
                Description = x.Description;
            }
        }
        [Required(ErrorMessage = "A coverage level is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "A coverage level is required.")]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class CustomerVM
    {
        public CustomerVM() : this(new Customer()) { }
        public CustomerVM(Customer x)
        {
            if (x != null)
            {
                Id = x.Id;
                FirstName = x.FirstName;
                LastName = x.LastName;
                DateOfBirth = x.DateOfBirth;
            }
        }
        public int Id { get; set; }
        [Display(Name = "First Name")]
        [Required]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        [Required]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Date of Birth is required.")]
        [Display(Name = "Date of Birth")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}", HtmlEncode = true)]
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> DateOfBirth { get; set; }

        public Nullable<bool> IsVisitor { get; set; }

        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }
    }

    public class CustomerDrivingHistoryVM
    {
        public CustomerDrivingHistoryVM() : this(new CustomerDrivingHistory()) { }
        public CustomerDrivingHistoryVM(CustomerDrivingHistory x)
        {
            if (x != null)
            {
                Id = x.Id;
                DateRecorded = x.DateRecorded;
                CustomerID = x.CustomerID;
                NumDUIs = x.NumDUIs;
                NumSpeedingTickets = x.NumSpeedingTickets;

                Customer = new CustomerVM(x.Customer);
            }
        }
        public int Id { get; set; }
        public Nullable<System.DateTime> DateRecorded { get; set; }
        public Nullable<int> CustomerID { get; set; }
        [Required(ErrorMessage = "Number of DUIs is required. Enter 0 for none.")]
        public Nullable<int> NumDUIs { get; set; }
        [Required(ErrorMessage = "Number of Speeding Tickets is required. Enter 0 for none.")]
        public Nullable<int> NumSpeedingTickets { get; set; }
        public virtual CustomerVM Customer { get; set; }
    }

    public class QuoteVM
    {
        public int CustomerID { get; set; }

        public bool IsVisitor { get; set; }

        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Date of Birth is required.")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}", HtmlEncode = true)]
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> DateOfBirth { get; set; }

        public bool HasDrivingHistory { get; set; }

        [Required(ErrorMessage = "Number of DUIs is required. Enter 0 for none.")]
        [Range(1, int.MaxValue, ErrorMessage = "Number of DUIs is required. Enter 0 for none.")]
        public Nullable<int> NumDUIs { get; set; }

        [Required(ErrorMessage = "Number of Speeding Tickets is required. Enter 0 for none.")]
        [Range(1, int.MaxValue, ErrorMessage = "Number of Speeding Tickets is required. Enter 0 for none.")]
        public Nullable<int> NumSpeedingTickets { get; set; }

        [Required(ErrorMessage = "Auto model is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Auto model is required.")]
        public int AutoModelID { get; set; }

        public Nullable<int> AutoOptionID { get; set; }

        [Required(ErrorMessage = "A covering option is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "A covering option is required.")]
        public int CoverageID { get; set; }

        public Nullable<decimal> PriceQuote { get; set; }
    }

    public class QuoteSummaryVM
    {
        public int QuoteID { get; set; }
        public string CustomerName { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string ModelYear { get; set; }
        public string OptionPackage { get; set; }
        public string Coverage { get; set; }
        public string Price { get; set; }
        [DisplayFormat(DataFormatString ="MM/dd/yyyy")]
        public DateTime DateIssued { get; set; }
    }

    public class QuoteDetailVM
    {
        public int QuoteID { get; set; }
        public int CustomerID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [DisplayFormat(DataFormatString = "MM/dd/yyyy")]
        public DateTime DateOfBirth { get; set; }
        public int NumDUIs { get; set; }
        public int NumTickets { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string ModelYear { get; set; }
        public string OptionPackage { get; set; }
        public string Coverage { get; set; }
        public string Price { get; set; }
        [DisplayFormat(DataFormatString = "MM/dd/yyyy")]
        public DateTime DateIssued { get; set; }
    }
}