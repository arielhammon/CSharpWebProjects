using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models
{
    public class Instructor : Person
    {
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Hire Date")]
        public DateTime HireDate { get; set; }

        /////////////////////////////////////////////////////
        private ICollection<Course> _courses;
        public virtual ICollection<Course> Courses
        {
            get
            {
                return _courses ?? (_courses = new List<Course>());
            }
            set
            {
                _courses = value;
            }
        }
        public virtual OfficeAssignment OfficeAssignment { get; set; }
    }
}