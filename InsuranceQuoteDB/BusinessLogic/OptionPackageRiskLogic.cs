using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InsuranceQuoteDB.BusinessLogic
{
    public static class OptionPackageRiskLogic
    {
        public static void AssignOptionsRiskLevels()
        {
            using (var db = new Models.InsuranceQuoteDBEntities())
            {
                foreach (Models.AutoOption item in db.AutoOptions)
                {
                    int? hp = item.Horsepower;
                    int? weight = item.CurbWeightLbs;
                    int? price = item.PriceDollars;
                    hp = hp is null ? 200 : hp; //if null, use 200 HP
                    weight = weight is null ? 3000 : weight; //if null, use 3000 lbs
                    price = price is null ? 25000 : price; //if null, use $25,000
                    double risk = (double)hp / (double)weight * (double)price;
                    if (risk < 2000)
                    {
                        item.OptionRiskLevelId = 1; //low risk
                    }
                    else if (risk < 5000)
                    {
                        item.OptionRiskLevelId = 2; //moderate risk
                    }
                    else if (risk < 10000)
                    {
                        item.OptionRiskLevelId = 3; //high risk
                    }
                    else if (risk < 15000)
                    {
                        item.OptionRiskLevelId = 4; //very high risk
                    }
                    else
                    {
                        item.OptionRiskLevelId = 5; //extreme risk
                    }
                }
                _ = db.SaveChanges();
            }
        }

    }
}