using System;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace ExcelAsSQLdataSourse.Models
{
    public class UploadFileVM
    {
        public UploadFileVM()
        {
            UploadFile = null;
            SetName = "";
            UserName = "";
            HasHeaders = false;
            ErrorMessage = null;
        }
        
        [Required]
        public HttpPostedFileBase UploadFile { get; set; }

        [Required]
        [StringLength(maximumLength: 128, MinimumLength = 1)]
        public string SetName { get; set; }

        [Required]
        [StringLength(maximumLength: 128, MinimumLength = 1)]
        public string UserName { get; set; }

        public bool HasHeaders { get; set; }
        public string ErrorMessage { get; set; }
    }
}