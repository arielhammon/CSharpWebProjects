using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using InsuranceQuoteDB.Models;
using Microsoft.AspNet.Identity;

namespace InsuranceQuoteDB.Controllers
{
    struct IDNamePair
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    public class QuotesController : Controller
    {
        private InsuranceQuoteDBEntities db = new InsuranceQuoteDBEntities();

        [HttpGet]
        public ActionResult Start()
        {
            if (Visitor.IsVisitor(CustID()))
            {
                return RedirectToAction("Create");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public ActionResult Index()
        {
            var quotes = new List<QuoteSummaryVM>();
            int customerID = CustID();
            if (Visitor.IsVisitor(customerID))
            {
                //we don't want to show a random visitor the quotes for all visitors
                quotes = null;
            }
            else
            {
                var quotesDB = db.Quotes.Where(x => x.CustomerID == customerID).OrderBy(x => x.DateIssued).ToList();
                foreach (var quote in quotesDB)
                {
                    quotes.Add(GetSummaryFromQuote(quote));
                }
            }
            return View(quotes);
        }

        public int CustID()
        {
            var userId = User.Identity.GetUserId();
            if (userId is null) //user is not logged in, user Visitor profile
            {
                return Visitor.VisitorID();
            }
            else //user is logged in
            {
                return db.CustomerAspNetUsers.Where(x => x.AspNetUserID == userId).First().CustomerID;
            }
        }

        // GET: Quotes/Create
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.CoverageID = new SelectList(db.CoverageLevels, "Id", "Name");

            var userId = User.Identity.GetUserId();
            int customerID = CustID();
            bool isVisitor = Visitor.IsVisitor(customerID);
            Customer cust = db.Customers.Find(customerID);
            //get driving history
            CustomerDrivingHistory driveHist = null;
            if (!isVisitor)
            {
                var list = db.CustomerDrivingHistories.Where(x => x.CustomerID == customerID).OrderBy(x => x.DateRecorded).ToList();
                if (list.Count > 0) driveHist = list.Last();
            }
            var model = new QuoteVM
            {
                CustomerID = customerID,
                AutoModelID = -1,
                AutoOptionID = -1,
                CoverageID = -1,
                CustomerName = cust.FirstName + " " + cust.LastName,
                DateOfBirth = cust.DateOfBirth,
                HasDrivingHistory = driveHist is null ? false : true,
                IsVisitor = isVisitor,
                NumDUIs = driveHist is null ? null : driveHist.NumDUIs,
                NumSpeedingTickets = driveHist is null ? null : driveHist.NumSpeedingTickets
            };
            return View(model);
        }

        // GET: Quotes/FetchMakes
        [HttpGet]
        public JsonResult FetchMakes()
        {
            IDNamePair[] makes = db.AutoMakes.OrderBy(x => x.MakeName).ToList().Select(x => new IDNamePair { ID = x.Id, Name = x.MakeName }).ToArray();
            return Json(makes, JsonRequestBehavior.AllowGet);
        }
        // GET: Quotes/FetchYears?AutoMakeID=7
        [HttpGet]
        public JsonResult FetchYears(int AutoMakeID)
        {
            IDNamePair[] years = db.AutoModels.Where(x => x.AutoMakeID == AutoMakeID).OrderBy(x => x.ModelYear).ToList()
                                .Select(x => new IDNamePair { ID = (int)x.ModelYear, Name = ((int)x.ModelYear).ToString() } ).ToArray();
            return Json(years, JsonRequestBehavior.AllowGet);
        }
        // GET: Quotes/FetchModels?AutoMakeID=7&AutoYear=2013
        [HttpGet]
        public JsonResult FetchModels(int AutoMakeID, int AutoYear)
        {
            IDNamePair[] models = db.AutoModels.Where(x => x.AutoMakeID == AutoMakeID && x.ModelYear == AutoYear).OrderBy(x => x.ModelName).ToList()
                                  .Select(x => new IDNamePair { ID = x.Id, Name = x.ModelName }).ToArray();
            return Json(models, JsonRequestBehavior.AllowGet);
        }
        // GET: Quotes/FetchModels?AutoMakeID=7&AutoYear=2013
        [HttpGet]
        public JsonResult FetchOptions(int AutoModelID)
        {
            IDNamePair[] options = db.AutoOptions.Where(x => x.AutoModelId == AutoModelID).OrderBy(x => x.OptionDescription.Length).ToList()
                                   .Select(x => new IDNamePair { ID = x.Id, Name = x.OptionDescription }).ToArray();
            return Json(options, JsonRequestBehavior.AllowGet);
        }

        // POST: Quotes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("GetQuote")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(QuoteVM quote)
        //[Bind(Include = "Id,CustomerID,AutoModelID,DrivingHistoryID,CoverageID,AutoOptionID,PriceQuote")]
        {
            CustomerDrivingHistory cdhDB = null;
            if (Visitor.IsVisitor(quote.CustomerID)) //Save Visitor driving history
            {
                var list = db.CustomerDrivingHistories.Where(x => x.CustomerID == quote.CustomerID).OrderBy(x => x.DateRecorded).ToList();
                if (list.Count > 0)
                {
                    cdhDB = list.Last();
                    if (quote.NumDUIs != cdhDB.NumDUIs || quote.NumSpeedingTickets != cdhDB.NumSpeedingTickets) //the history changed
                    {
                        cdhDB = null; //set to null so that we record the new history
                    }
                }
            }
            if (cdhDB is null)
            {
                cdhDB = new CustomerDrivingHistory
                {
                    CustomerID = quote.CustomerID,
                    DateRecorded = DateTime.Now,
                    NumDUIs = quote.NumDUIs,
                    NumSpeedingTickets = quote.NumSpeedingTickets
                };
                db.CustomerDrivingHistories.Add(cdhDB);
                _ = db.SaveChanges();
            }

            if (Visitor.IsVisitor(quote.CustomerID))
            {
                //save Visitor's birthday and driving history ID in a separate table for reference
                var vs = new VisitorSession
                {
                    DateOfBirth = quote.DateOfBirth,
                    DrivingHistoryId = cdhDB.Id
                };
                db.VisitorSessions.Add(vs);
                _ = db.SaveChanges();
            }
            
            Quote quoteDB = new Quote();
            quoteDB.CustomerID = quote.CustomerID;
            quoteDB.AutoModelID = quote.AutoModelID;
            quoteDB.AutoOptionID = quote.AutoOptionID;
            quoteDB.CoverageID = quote.CoverageID;
            quoteDB.DrivingHistoryID = cdhDB.Id;
            quoteDB.DateIssued = DateTime.Now;
            quoteDB.PriceQuote = BusinessLogic.QuoteLogic.GetPriceQuote(quoteDB);
            db.Quotes.Add(quoteDB);
            _ = db.SaveChanges();

            return RedirectToAction("Details", quoteDB);
        }

        public QuoteSummaryVM GetSummaryFromQuote(Quote quote)
        {
            if (quote is null) return null;
            if (quote.Id < 1) return null; //the quote didn't come from the database
            var cust = db.Customers.Find(quote.CustomerID);
            var model = db.AutoModels.Find(quote.AutoModelID);
            var option = quote.AutoOptionID == null ? null : db.AutoOptions.Find(quote.AutoOptionID);
            var qsVM = new QuoteSummaryVM
            {
                QuoteID = quote.Id,
                Coverage = db.CoverageLevels.Find(quote.CoverageID).Name,
                CustomerName = cust.FirstName + " " + cust.LastName,
                DateIssued = (DateTime)quote.DateIssued,
                Make = db.AutoMakes.Find(model.AutoMakeID).MakeName,
                Model = model.ModelName,
                ModelYear = ((int)model.ModelYear).ToString(),
                OptionPackage = option is null ? "" : option.OptionDescription,
                Price = ((decimal)quote.PriceQuote).ToString("C2")
            };
            return qsVM;
        }

        public ActionResult Details(Quote quote)
        {
            if (quote is null) return RedirectToAction("Home");
            return View(GetSummaryFromQuote(quote));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
