using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InsuranceQuoteDB.Models;
using InsuranceQuoteDB.Controllers;
using InsuranceQuoteDB.BusinessLogic;

namespace InsuranceQuoteDB.Controllers
{
    public class AdminController : Controller
    {
        private InsuranceQuoteDBEntities db = new InsuranceQuoteDBEntities();
        private QuotesController qCont = new QuotesController();

        [HttpGet]
        public ActionResult Index()
        {
            var quotes = new List<QuoteSummaryVM>();
            if (User.IsInRole("Admin"))
            {
                var quotesDB = db.Quotes.OrderBy(x => x.DateIssued).ToList();
                foreach (var quote in quotesDB)
                {
                    quotes.Add(qCont.GetSummaryFromQuote(quote));
                }   
            }
            else
            {
                //we don't want to show non-Admins the quotes for all customers
                quotes = null;
            }
            return View(quotes);
        }

        [ActionName("Details")]
        public ActionResult Details(int quoteID)
        {
            QuoteDetailVM qdVM;
            if (User.IsInRole("Admin"))
            {
                var quoteDB = db.Quotes.Find(quoteID);
                if (quoteDB is null) return null;
                var cust = db.Customers.Find(quoteDB.CustomerID);
                VisitorSession vs = null;
                bool isVisitor = false;
                if (Visitor.IsVisitor(cust.Id))
                {
                    isVisitor = true;
                    vs = db.VisitorSessions.Where(x => x.DrivingHistoryId == quoteDB.DrivingHistoryID).ToList().First(); //drivingHistoryId is a key
                }
                var dHist = db.CustomerDrivingHistories.Find(quoteDB.DrivingHistoryID);
                var model = db.AutoModels.Find(quoteDB.AutoModelID);
                var option = quoteDB.AutoOptionID == null ? null : db.AutoOptions.Find(quoteDB.AutoOptionID);
                qdVM = new QuoteDetailVM
                {
                    QuoteID = quoteDB.Id,
                    Coverage = db.CoverageLevels.Find(quoteDB.CoverageID).Name,
                    CustomerID = (int)quoteDB.CustomerID,
                    FirstName = cust.FirstName,
                    LastName = cust.LastName,
                    DateOfBirth = (DateTime)(isVisitor ? vs.DateOfBirth : cust.DateOfBirth),
                    NumDUIs = (int)dHist.NumDUIs,
                    NumTickets = (int)dHist.NumSpeedingTickets,
                    DateIssued = (DateTime)quoteDB.DateIssued,
                    Make = db.AutoMakes.Find(model.AutoMakeID).MakeName,
                    Model = model.ModelName,
                    ModelYear = ((int)model.ModelYear).ToString(),
                    OptionPackage = option is null ? "" : option.OptionDescription,
                    Price = ((decimal)quoteDB.PriceQuote).ToString("C2")
                };
            }
            else
            {
                //we don't want to show non-Admins a quote for another customer
                qdVM = null;
            }
            return View(qdVM);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
                qCont.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
