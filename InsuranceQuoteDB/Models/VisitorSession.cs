//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace InsuranceQuoteDB.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class VisitorSession
    {
        public int Id { get; set; }
        public Nullable<System.DateTime> DateOfBirth { get; set; }
        public Nullable<int> DrivingHistoryId { get; set; }
    
        public virtual CustomerDrivingHistory CustomerDrivingHistory { get; set; }
    }
}
