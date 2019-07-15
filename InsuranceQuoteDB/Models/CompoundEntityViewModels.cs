using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InsuranceQuoteDB.Models
{
    public class RegisterAndCustomerVM
    {
        public RegisterAndCustomerVM() : this(new RegisterViewModel(), new CustomerVM()) { }
        public RegisterAndCustomerVM(RegisterViewModel x, CustomerVM y)
        {
            regVM = x;
            custVM = y;
        }
        public RegisterViewModel regVM { get; set; }
        public CustomerVM custVM { get; set; }
    }

    public class IndexViewAndCustomerVM
    {
        public IndexViewAndCustomerVM() : this(new IndexViewModel(), new CustomerVM()) { }
        public IndexViewAndCustomerVM(IndexViewModel x, CustomerVM y)
        {
            indexVM = x;
            custVM = y;
        }
        public IndexViewModel indexVM { get; set; }
        public CustomerVM custVM { get; set; }
    }
}