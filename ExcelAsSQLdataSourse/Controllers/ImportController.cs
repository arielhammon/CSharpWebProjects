using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExcelAsSQLdataSourse.Models;
using System.Xml;
using System.IO;
using System.IO.Compression;

namespace ExcelAsSQLdataSourse.Controllers
{
    public class ImportController : Controller
    {
        private FinancialDBEntities db = new FinancialDBEntities();

        // GET: Import
        public ActionResult Index(UploadFileVM model = null)
        {
            //display contents of database
            //var alldata = db.ImportSets.ToList();
            //var alldata2 = new List<ImportSetExcelRow>();
            //foreach (ImportSet item in alldata)
            //{
            //    var item2 = new ImportSetExcelRow();
            //    item2.ImportSet = item;
            //    alldata2.Add(item2);
            //}
            //return View("Results", alldata2);
            if (model == null) model = new UploadFileVM();
            return View(model);
        }

        public ActionResult GetExcelFileSample()
        {
            string path = "~/App_Data/";
            string filename = "FinancialSample.xlsx";
            string type = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(path + filename, type, filename);
        }

        [HttpPost]
        public ActionResult UploadFile(UploadFileVM upload)
        {
            if (string.IsNullOrEmpty(upload.SetName) || string.IsNullOrEmpty(upload.UserName))
            {
                upload.ErrorMessage = "Please provide both a name for the data set and your name.";
                return RedirectToAction("Index", new { model = upload });
            }
            HttpPostedFileBase file = upload.UploadFile;
            if (file != null && file.ContentLength > 0)
            {
                string fileName = Path.GetFileName(file.FileName);
                string folderGUID = Guid.NewGuid().ToString();
                string path = Server.MapPath("~/App_Data/Uploads/" + folderGUID + "/");
                string pathFileName = Path.Combine(path, fileName);
                try
                {
                    Directory.CreateDirectory(path);
                    upload.UploadFile.SaveAs(pathFileName);
                }
                catch (Exception)
                {
                    Directory.Delete(path, recursive: true);
                    upload.ErrorMessage = "Sorry, we were unable to upload your file.";
                    return RedirectToAction("Index", new { model = upload });
                }
                var importSets = new List<ImportSetExcelRow>();
                try
                {
                    importSets = ImportFileXML(path, pathFileName, upload);
                }
                catch (Exception)
                {
                    Directory.Delete(path, recursive: true);
                    upload.ErrorMessage = "Sorry, we were unable to import your file.";
                    return RedirectToAction("Index", new { model = upload });
                }
                Directory.Delete(path, recursive: true);
                int numErrorFreeLines = 0;
                foreach (ImportSetExcelRow item in importSets)
                {
                    if (!item.HasError) numErrorFreeLines++;
                }
                if (numErrorFreeLines > 0)
                {
                    SetName setName = new SetName();
                    try
                    {
                        List<SetName> setNames = db.SetNames.Where(x => x.Name.StartsWith(upload.SetName)).ToList();
                        if (setNames.Count > 0) //look at existing names to ensure uniqueness
                        {
                            bool exactMatch = false;
                            int n = int.MinValue; //highest number that has been appended to the current name
                            foreach (SetName item in setNames)
                            {
                                if (item.Name == upload.SetName) exactMatch = true;
                                string temp = item.Name.Replace(upload.SetName, ""); //remove current name from string
                                int len = temp.Length;
                                //check to see if an integer has been appended to the current name such as "example(3)"
                                if (len > 2 && temp.Substring(0, 1) == "(" && temp.Substring(len - 1, 1) == ")")
                                {
                                    string temp2 = temp.Substring(1, len - 2);
                                    if (int.TryParse(temp2, out int nn))
                                        if (nn > n) n = nn;
                                }
                            }
                            if (exactMatch)
                                if (n > int.MinValue)
                                    upload.SetName += "(" + (++n) + ")";
                                else
                                    upload.SetName += "(1)";
                        }
                        setName = new SetName
                        {
                            DateUploaded = DateTime.Now,
                            HasErrors = (importSets.Count > numErrorFreeLines ? true : false),
                            Name = upload.SetName,
                            UserName = upload.UserName
                        };
                        db.SetNames.Add(setName);
                        _ = db.SaveChanges();
                    }
                    catch (Exception)
                    {
                        upload.ErrorMessage = "Sorry, something went wrong. Please try again later.";
                        return RedirectToAction("Index", new { model = upload });
                    }
                    foreach (ImportSetExcelRow row in importSets)
                    {
                        if (!row.HasError)
                        {
                            row.ImportSet.SetID = setName.ID;
                            db.ImportSets.Add(row.ImportSet);
                        }
                    }
                    _ = db.SaveChanges();
                }
                return View("Results", importSets);
            }
            else
            {
                upload.ErrorMessage = "The file is empty. Please try again.";
                return RedirectToAction("Index", new { model = upload });
            }
        }

        private List<ImportSetExcelRow> ImportFileXML(string path, string pathFileName, UploadFileVM upload)
        {
            string sheetPath = "", stringsPath = "";

            using (ZipArchive archive = ZipFile.OpenRead(pathFileName))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    //The data could be contained in sheet1, sheet2, sheet3, etc.
                    //However, this import requires it to be in sheet1
                    if (entry.Name.Contains("sheet1.xml"))
                    {
                        sheetPath = Path.Combine(path, "sheet1.xml");
                        entry.ExtractToFile(sheetPath, overwrite: true);
                    }
                    else if (entry.Name.Contains("sharedStrings.xml"))
                    {
                        stringsPath = Path.Combine(path, "sharedStrings.xml");
                        entry.ExtractToFile(stringsPath, overwrite: true);
                    }
                }
            }
            var sharedStrings = new Dictionary<int, string>();
            using (Stream stream = new FileStream(stringsPath, FileMode.Open))
            {
                using (XmlReader reader = XmlReader.Create(stream))
                {
                    int i = 0; //the index
                    bool inSI = false; //in shared item node
                    string temp = "";
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:
                                if (reader.Name == "si")
                                {
                                    temp = ""; //start new shared item
                                    inSI = true;
                                } 
                                break;
                            case XmlNodeType.Text:
                                if (inSI) temp += reader.Value;
                                break;
                            case XmlNodeType.EndElement:
                                if (reader.Name == "si")
                                {
                                    sharedStrings.Add(i++, temp); //add shared item and move to next index
                                    inSI = false;
                                }
                                break;
                        }
                    }
                }
            }
            var importSets = new List<ImportSetExcelRow>();
            using (Stream stream = new FileStream(sheetPath, FileMode.Open))
            {
                using (XmlReader reader = XmlReader.Create(stream))
                {
                    int lineNum = 0;
                    var header = new Dictionary<int, string>();
                    if (upload.HasHeaders)
                    {
                        lineNum = 0;
                    }
                    else
                    {
                        lineNum = 1;
                        header = new Dictionary<int, string>
                        {
                            { 0, "Segment" },
                            { 1, "Country" },
                            { 2, "Product" },
                            { 3, "Discount Band" },
                            { 4, "Units Sold" },
                            { 5, "Manufacturing Price" },
                            { 6, "Sale Price" },
                            { 7, "Gross Sales" },
                            { 8, "Discounts" },
                            { 9, "Sales" },
                            { 10, "COGS" },
                            { 11, "Profit" },
                            { 12, "Date" }
                        };
                    }
                    int i = 0; //the index of the cell in the row
                    string temp = "";
                    bool inCell = false;
                    bool useShared = false;
                    ImportSetExcelRow row = new ImportSetExcelRow();
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:
                                if (reader.Name == "c")
                                {
                                    inCell = true;
                                    temp = "";
                                    if (reader.GetAttribute("t") == "s") //cell type is shared string
                                        useShared = true;
                                    else
                                        useShared = false;
                                }
                                else if (reader.Name == "row")
                                {
                                    row = new ImportSetExcelRow(LineNum: lineNum); //start new row
                                    i = 0;
                                }
                                break;
                            case XmlNodeType.Text:
                                if (inCell)
                                {
                                    temp += reader.Value;
                                    if (useShared)
                                        if (int.TryParse(temp, out int j))
                                            temp = sharedStrings[j];
                                }
                                break;
                            case XmlNodeType.EndElement:
                                if (reader.Name == "c")
                                {
                                    if (lineNum == 0) //populate header
                                    {
                                        header.Add(i, temp);
                                    }
                                    else row.ParseFieldValue(header[i] ,temp);
                                    inCell = false;
                                    i++;
                                }
                                else if (reader.Name == "row") //add shared item and move to next index
                                {   
                                    if (lineNum > 0) importSets.Add(row); //not the header row
                                    lineNum++;
                                }
                                break;
                        }
                    }
                }
            }
            return importSets;
        }


        //private object ImportFileOLE(string path, UploadFileVM upload)
        //{
        //    //taken from https://www.connectionstrings.com
        //    string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source =" + path + 
        //                              "; Extended Properties = \"Excel 12.0 Xml; HDR = " + (upload.HasHeaders ? "YES" : "NO") + "\";";
        //    if (Environment.Is64BitOperatingSystem)
        //    {
        //        //just in case the ACE provider needs to change based on the environment
        //    }
        //    using (var connection = new OleDbConnection(connectionString))
        //    {
        //        string queryString = "select [Segment], [Country], [Product], [Discount Band], [Units Sold], [Manufacturing Price], " +
        //                             "[Sale Price], [Gross Sales], [Discounts], [Sales], [COGS], [Profit], [Date] from [Sheet1$]";
        //        var command = new OleDbCommand(queryString, connection);
        //        var adapter = new OleDbDataAdapter(command);
        //        try
        //        {
        //            connection.Open();
        //        }
        //        catch (Exception ex)
        //        {
        //            return ex;
        //        }
        //        OleDbDataReader reader = command.ExecuteReader();
        //        var setName = new SetName
        //        {
        //            DateUploaded = DateTime.Now,
        //            Name = upload.SetName,
        //            UserName = upload.UserName
        //        };
        //        try
        //        {
        //            db.SetNames.Add(setName);
        //            //_ = db.SaveChanges();
        //        }
        //        catch (SqlException ex)
        //        {
        //            return ex.ToString();
        //        }
        //        int lineNum = 0;
        //        var Exceptions = new Dictionary<int, List<string>>();
        //        while (reader.Read())
        //        {
        //            lineNum++;
        //            var lineItem = new ImportSet();
        //            for (int i = 0; i < reader.FieldCount; i++)
        //            {

        //            }
        //            try
        //            {
                        
        //            }
        //            catch (Exception)
        //            {

        //            }
        //        }
        //        connection.Close();
        //        return true;
        //    }
        //}
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
                base.Dispose(disposing);
            }
        }
    }
}