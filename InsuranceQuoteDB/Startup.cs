using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;

[assembly: OwinStartupAttribute(typeof(InsuranceQuoteDB.Startup))]
namespace InsuranceQuoteDB
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            //CreateRoles();
            //PopulateTablesWithMakeModelOptions();
            //AddOtherOptionPackage();
            //BusinessLogic.OptionPackageRiskLogic.AssignOptionsRiskLevels();
        }

        private void createRoles()
        {
            //I wonder if this context will work since I didn't include identity tables when I created it
            //var context = new InsuranceQuoteDBEntities();
            var context = new InsuranceQuoteDB.Models.ApplicationDbContext();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            if (!roleManager.RoleExists("Visitor"))
            {
                roleManager.Create(new IdentityRole("Visitor"));
            }
            if (!roleManager.RoleExists("Admin"))
            {
                roleManager.Create(new IdentityRole("Admin"));
            }
            if (!roleManager.RoleExists("Customer"))
            {
                roleManager.Create(new IdentityRole("Customer"));
            }
        }

        private void AddOtherOptionPackage()
        {
            using (var db = new Models.InsuranceQuoteDBEntities())
            {
                foreach (var model in db.AutoModels)
                {
                    var options = db.AutoOptions.Where(x => x.AutoModelId == model.Id
                                 && x.OptionDescription == "Other").ToList();
                    if (options.Count == 0)
                    {
                        db.AutoOptions.Add(new Models.AutoOption
                        {
                            AutoModelId = model.Id,
                            OptionDescription = "Other",
                            CurbWeightLbs = null,
                            Horsepower = null,
                            PriceDollars = null,
                            OptionRiskLevelId = null
                        });
                    }
                }
                _ = db.SaveChanges();
            }
        }

        private void PopulateTablesWithMakeModelOptions()
        {
            using (var db = new Models.InsuranceQuoteDBEntities())
            {
                var rows = db.Year_Make_Model_Trim_Full_Specs_by_Teoalida_SAMPLE.ToList();
                foreach (var row in rows)
                {
                    //WARNING: in this particular data set, row.make, row.model. row.year, and row.trim are never empty
                    //consider this if using this routine to ever import another list
                    Models.AutoMake autoMake;
                    Models.AutoModel autoModel;
                    Models.AutoOption autoOption;
                    var makes2 = db.AutoMakes.Where(x => x.MakeName == row.Make).OrderBy(x => x.Id).ToList();
                    if (makes2.Count() != 0)
                    {
                        autoMake = makes2.Last();
                    }
                    else
                    {
                        autoMake = new Models.AutoMake { MakeName = row.Make };
                        db.AutoMakes.Add(autoMake);
                        _ = db.SaveChanges(); //saving sets the autoincemented ID value
                    }
                    var models2 = db.AutoModels.Where(x => x.AutoMakeID == autoMake.Id && x.ModelName == row.Model && x.ModelYear == row.Year).OrderBy(x => x.Id).ToList();
                    if (models2.Count() != 0)
                    {
                        autoModel = models2.Last();
                    }
                    else
                    {
                        autoModel = new Models.AutoModel
                        {
                            AutoMakeID = autoMake.Id,
                            ModelName = row.Model,
                            ModelYear = row.Year
                        };
                        db.AutoModels.Add(autoModel);
                        _ = db.SaveChanges(); //saving sets the autoincemented ID value
                    }
                    var options2 = db.AutoOptions.Where(x => x.AutoModelId == autoModel.Id && x.OptionDescription == row.Trim).OrderBy(x => x.Id).ToList();
                    if (options2.Count() == 0)
                    {
                        autoOption = new Models.AutoOption
                        {
                            OptionDescription = row.Trim,
                            AutoModelId = autoModel.Id,
                            CurbWeightLbs = row.Curb_weight_lbs,
                            Horsepower = row.Horsepower_HP,
                            PriceDollars = row.Price
                        };
                        db.AutoOptions.Add(autoOption);
                        _ = db.SaveChanges();
                    }
                }
            }
        }
    }
}
