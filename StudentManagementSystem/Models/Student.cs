using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudentManagementSystem.Models
{
    public class Student
    {
        public int ID { get; internal set; }
        public string FirstName { get; internal set; }
        public string LastName { get; internal set; }
    }
}