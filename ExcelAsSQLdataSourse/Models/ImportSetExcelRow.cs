using System;
using System.Collections.Generic;
using ExcelAsSQLdataSourse.Extensions;

namespace ExcelAsSQLdataSourse.Models
{
    public class ImportSetExcelRow
    {
        public ImportSet ImportSet { get; set; }
        public bool HasError { get; set; }
        public List<string> Errors { get; set; }


        public ImportSetExcelRow(int LineNum = 0)
        {
            ImportSet = new ImportSet
            {
                COGS = null,
                Country = null,
                DateOf = null,
                DiscountBand = null,
                Discounts = null,
                GrossSales = null,
                LineNum = LineNum,
                ManufacturingPrice = null,
                Product = null,
                Profit = null,
                Sales = null,
                SalesPrice = null,
                Segment = null,
                UnitsSold = null
            };
            HasError = false;
            Errors = new List<string>();
        }
        
        public void ParseFieldValue(string FieldName, string Value)
        {
            string fieldName = FieldName.Replace(" ", "").ToLower(); //removes all spaces from FieldName and converts to lowercase
            int? n;
            double dValue;
            double? dMin, dMax;
            switch (fieldName)
            {
                case "segment":
                    n = ImportSet.GetMaxLength(x => x.Segment);
                    if (n != null && Value.Length > n)
                    {
                        Value = Value.Substring(0, (int)n);
                        Errors.Add(String.Format("Segment (Warning): The text stored in this field was too long. It has been shortened to {0} characters.", n));
                    }
                    ImportSet.Segment = Value;
                    break;
                case "country":
                    n = ImportSet.GetMaxLength(x => x.Country);
                    if (n != null && Value.Length > n)
                    {
                        Value = Value.Substring(0, (int)n);
                        Errors.Add(String.Format("Country (Warning): The text stored in this field was too long. It has been shortened to {0} characters.", n));
                    }
                    ImportSet.Country = Value;
                    break;
                case "product":
                    n = ImportSet.GetMaxLength(x => x.Product);
                    if (n != null && Value.Length > n)
                    {
                        Value = Value.Substring(0, (int)n);
                        Errors.Add(String.Format("Product (Warning): The text stored in this field was too long. It has been shortened to {0} characters.", n));
                    }
                    ImportSet.Product = Value;
                    break;
                case "discountband":
                    n = ImportSet.GetMaxLength(x => x.DiscountBand);
                    if (n != null && Value.Length > n)
                    {
                        Value = Value.Substring(0, (int)n);
                        Errors.Add(String.Format("DiscountBand (Warning): The text stored in this field was too long. It has been shortened to {0} characters.", n));
                    }
                    ImportSet.DiscountBand = Value;
                    break;
                case "unitssold":
                    if (double.TryParse(Value, out dValue))
                    {
                        dMin = ImportSet.GetMinRange(x => x.UnitsSold);
                        dMax = ImportSet.GetMaxRange(x => x.UnitsSold);
                        if (dMin != null && dValue < dMin)
                        {
                            Errors.Add(string.Format("Units Sold (Error): The value stored in this field is less than the minimum allowed value of {0}.", dMin.ToString()));
                            HasError = true;
                        }
                        else if (dMax != null && dValue > dMax)
                        {
                            Errors.Add(string.Format("Units Sold (Error): The value stored in this field is greater than the maximum allowed value of {0}.", dMax.ToString()));
                            HasError = true;
                        }
                        else ImportSet.UnitsSold = dValue;
                    }
                    else
                    {
                        Errors.Add("Units Sold (Error): The value stored in this field is not numeric.");
                        HasError = true;
                    }
                    break;
                case "manufacturingprice":
                    if (double.TryParse(Value, out dValue))
                    {
                        dMin = ImportSet.GetMinRange(x => x.ManufacturingPrice);
                        dMax = ImportSet.GetMaxRange(x => x.ManufacturingPrice);
                        if (dMin != null && dValue < dMin)
                        {
                            Errors.Add(string.Format("Manufacturing Price (Error): The value stored in this field is less than the minimum allowed value of {0}.", dMin.ToString()));
                            HasError = true;
                        }
                        else if (dMax != null && dValue > dMax)
                        {
                            Errors.Add(string.Format("Manufacturing Price (Error): The value stored in this field is greater than the maximum allowed value of {0}.", dMax.ToString()));
                            HasError = true;
                        }
                        else ImportSet.ManufacturingPrice = dValue;
                    }
                    else
                    {
                        Errors.Add("Manufacturing Price (Error): The value stored in this field is not numeric.");
                        HasError = true;
                    }
                    break;
                case "saleprice":
                    if (double.TryParse(Value, out dValue))
                    {
                        dMin = ImportSet.GetMinRange(x => x.SalesPrice);
                        dMax = ImportSet.GetMaxRange(x => x.SalesPrice);
                        if (dMin != null && dValue < dMin)
                        {
                            Errors.Add(string.Format("Sale Price (Error): The value stored in this field is less than the minimum allowed value of {0}.", dMin.ToString()));
                            HasError = true;
                        }
                        else if (dMax != null && dValue > dMax)
                        {
                            Errors.Add(string.Format("Sale Price (Error): The value stored in this field is greater than the maximum allowed value of {0}.", dMax.ToString()));
                            HasError = true;
                        }
                        else ImportSet.SalesPrice = dValue;
                    }
                    else
                    {
                        Errors.Add("Sale Price (Error): The value stored in this field is not numeric.");
                        HasError = true;
                    }
                    break;
                case "grosssales":
                    if (double.TryParse(Value, out dValue))
                    {
                        dMin = ImportSet.GetMinRange(x => x.GrossSales);
                        dMax = ImportSet.GetMaxRange(x => x.GrossSales);
                        if (dMin != null && dValue < dMin)
                        {
                            Errors.Add(string.Format("Gross Sales (Error): The value stored in this field is less than the minimum allowed value of {0}.", dMin.ToString()));
                            HasError = true;
                        }
                        else if (dMax != null && dValue > dMax)
                        {
                            Errors.Add(string.Format("Gross Sales (Error): The value stored in this field is greater than the maximum allowed value of {0}.", dMax.ToString()));
                            HasError = true;
                        }
                        else ImportSet.GrossSales = dValue;
                    }
                    else
                    {
                        Errors.Add("Gross Sales (Error): The value stored in this field is not numeric.");
                        HasError = true;
                    }
                    break;
                case "discounts":
                    if (double.TryParse(Value, out dValue))
                    {
                        dMin = ImportSet.GetMinRange(x => x.Discounts);
                        dMax = ImportSet.GetMaxRange(x => x.Discounts);
                        if (dMin != null && dValue < dMin)
                        {
                            Errors.Add(string.Format("Discounts (Error): The value stored in this field is less than the minimum allowed value of {0}.", dMin.ToString()));
                            HasError = true;
                        }
                        else if (dMax != null && dValue > dMax)
                        {
                            Errors.Add(string.Format("Discounts (Error): The value stored in this field is greater than the maximum allowed value of {0}.", dMax.ToString()));
                            HasError = true;
                        }
                        else ImportSet.Discounts = dValue;
                    }
                    else
                    {
                        Errors.Add("Discounts (Error): The value stored in this field is not numeric.");
                        HasError = true;
                    }
                    break;
                case "sales":
                    if (double.TryParse(Value, out dValue))
                    {
                        dMin = ImportSet.GetMinRange(x => x.Sales);
                        dMax = ImportSet.GetMaxRange(x => x.Sales);
                        if (dMin != null && dValue < dMin)
                        {
                            Errors.Add(string.Format("Sales (Error): The value stored in this field is less than the minimum allowed value of {0}.", dMin.ToString()));
                            HasError = true;
                        }
                        else if (dMax != null && dValue > dMax)
                        {
                            Errors.Add(string.Format("Sales (Error): The value stored in this field is greater than the maximum allowed value of {0}.", dMax.ToString()));
                            HasError = true;
                        }
                        else ImportSet.Sales = dValue;
                    }
                    else
                    {
                        Errors.Add("Sales (Error): The value stored in this field is not numeric.");
                        HasError = true;
                    }
                    break;
                case "cogs":
                    if (double.TryParse(Value, out dValue))
                    {
                        dMin = ImportSet.GetMinRange(x => x.COGS);
                        dMax = ImportSet.GetMaxRange(x => x.COGS);
                        if (dMin != null && dValue < dMin)
                        {
                            Errors.Add(string.Format("COGS (Error): The value stored in this field is less than the minimum allowed value of {0}.", dMin.ToString()));
                            HasError = true;
                        }
                        else if (dMax != null && dValue > dMax)
                        {
                            Errors.Add(string.Format("COGS (Error): The value stored in this field is greater than the maximum allowed value of {0}.", dMax.ToString()));
                            HasError = true;
                        }
                        else ImportSet.COGS = dValue;
                    }
                    else
                    {
                        Errors.Add("COGS (Error): The value stored in this field is not numeric.");
                        HasError = true;
                    }
                    break;
                case "profit":
                    if (double.TryParse(Value, out dValue))
                    {
                        dMin = ImportSet.GetMinRange(x => x.Profit);
                        dMax = ImportSet.GetMaxRange(x => x.Profit);
                        if (dMin != null && dValue < dMin)
                        {
                            Errors.Add(string.Format("Profit (Error): The value stored in this field is less than the minimum allowed value of {0}.", dMin.ToString()));
                            HasError = true;
                        }
                        else if (dMax != null && dValue > dMax)
                        {
                            Errors.Add(string.Format("Profit (Error): The value stored in this field is greater than the maximum allowed value of {0}.", dMax.ToString()));
                            HasError = true;
                        }
                        else ImportSet.Profit = dValue;
                    }
                    else
                    {
                        Errors.Add("Profit (Error): The value stored in this field is not numeric.");
                        HasError = true;
                    }
                    break;
                case "date":
                    if (double.TryParse(Value, out dValue))
                    {
                        DateTime date = DateTime.FromOADate(dValue);
                        ImportSet.DateOf = date;
                    }
                    else
                    {
                        Errors.Add("Date (Error): The value stored in this field is not a date.");
                        HasError = true;
                    }
                    break;
                default:
                    HasError = true;
                    break;
            }
        }
    }
}