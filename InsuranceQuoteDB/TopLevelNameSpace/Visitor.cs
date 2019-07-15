using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InsuranceQuoteDB
{
    public static class Visitor
    {
        public static bool IsVisitor (int? ID)
        {
            switch (ID) //using switch to make it easy to add other ID's in the future
            {
                case null:
                case 2:
                    return true;
                default:
                    return false;
            }
        }
        public static int VisitorID()
        {
            return 2;
        }
    }
}