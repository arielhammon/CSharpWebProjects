using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InsuranceQuoteDB.BusinessLogic
{
    public static class QuoteLogic
    {
        public static decimal GetPriceQuote(Models.Quote quote)
        {
            //From an insurance (and maintenance) standpoint, the following logic could be greatly improved
            //but it's what was called for. We'll go along with it for demo purposes.
            //We've also separate conditions that could be combined to ease future alterations.

            decimal price = 0m;
            if (quote.CoverageID == 1 || quote.CoverageID == 3) //lowest liability coverage
            {
                price += 50m;
            }
            else if (quote.CoverageID == 2 || quote.CoverageID == 4) //higher liability coverage
            {
                price += 75m;
            }

            using (var db = new Models.InsuranceQuoteDBEntities())
            {
                //////////////////User's Age//////////////////////
                DateTime dob;
                if (Visitor.IsVisitor(quote.CustomerID))
                {
                    //this query will only contain one element since (DrivingHistoryID) is a key
                    dob = (DateTime)db.VisitorSessions.Where(x => x.DrivingHistoryId == quote.DrivingHistoryID).First().DateOfBirth;
                }
                else
                {
                    dob = (DateTime)db.Customers.Find(quote.CustomerID).DateOfBirth;
                }
                double ageYears = ((DateTime)quote.DateIssued - dob).TotalDays / 365.25;
                //age could differ from legal age by approx 8 hours depending on where leap year lands
                if (ageYears < 18)
                {
                    price += 100m;
                }
                else if (ageYears < 25)
                {
                    price += 25m;
                }
                else if (ageYears > 100)
                {
                    price += 100m;
                }

                ////////////////////Model Year//////////////////
                var autoModel = db.AutoModels.Find(quote.AutoModelID);
                var autoMake = db.AutoMakes.Find(autoModel.AutoMakeID);
                var autoOption = db.AutoOptions.Find(quote.AutoOptionID);
                if (autoModel.ModelYear < 2000)
                {
                    price += 25m;
                }
                else if (autoModel.ModelYear > 2015)
                {
                    price += 25m;
                }

                //The following logic approximates the logic that was called for but works better for
                //all makes and models and incorporates risk factors based on horsepower, curb weight, and MSRP (retail price)
                ////////////////////Auto (vehicle) Risk Factors/////////////////////
                switch (autoMake.MakeRiskLevelID)
                {
                    case 1: //low, do nothing
                        break;
                    case 2: //moderate
                        price += 12.50m;
                        break;
                    case 3: //high
                        price += 25m;
                        break;
                    default: //higher, doesn't currently exist
                        price += 50m;
                        break;
                }
                switch (autoModel.ModelRiskLevelID)
                {
                    case 1: //low, do nothing
                        break;
                    case 2: //moderate
                        price += 12.50m;
                        break;
                    case 3: //high
                        price += 25m;
                        break;
                    default: //higher, doesn't currently exist
                        price += 50m;
                        break;
                }
                if (autoOption != null) //option package is not required
                {
                    switch (autoOption.OptionRiskLevelId)
                    {
                        case null:
                        case 1: //low, do nothing
                            break;
                        case 2: //moderate
                            price += 10m;
                            break;
                        case 3: //high
                            price += 20m;
                            break;
                        case 4: //very high
                            price += 30m;
                            break;
                        case 5: //extreme
                            price += 40m;
                            break;
                        default: //higher, doesn't currently exist
                            price += 50m;
                            break;
                    }
                }

                ///////////////////Driving History/////////////////////////
                var cdh = db.CustomerDrivingHistories.Find(quote.DrivingHistoryID);
                price += (decimal)cdh.NumSpeedingTickets * 10m; //increase $10 for each speeding ticket
                if (cdh.NumDUIs > 0) price *= 1.25m; //increase by 25%
            }

            //////////////////Full Coverage Options////////////////////
            if (quote.CoverageID == 3) //high deductible
            {
                price *= 1.25m; //increase by 25%
            }
            else if (quote.CoverageID == 4) //low deductible
            {
                price *= 1.5m;
            }

            return price;
        }
    }
}