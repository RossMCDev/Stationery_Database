using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Stationers.Models;
using System.Data.Entity;
using System.Net;
using System.Web.Mvc.Ajax;



namespace StationaryStore.Controllers
{
    public class HomeController : Controller
    {
        private MVCStationaryEntities1 db = new MVCStationaryEntities1();
        public ActionResult Index(String stationeryCategory, String searchString)
        {
            var CategoryList = new List<string>();

            var CategoryQuery = from d in db.Stationaries
                             orderby d.Category
                             select d.Category;

            CategoryList.AddRange(CategoryQuery.Distinct());

            ViewBag.stationeryCategory = new SelectList(CategoryList);

            LoadSubjects();

            //collection of model
            var stationaries = from s in db.Stationaries
                         select s;


            //narrow down collection
            if (!String.IsNullOrEmpty(stationeryCategory))
            {
                stationaries = stationaries.Where(x => x.Category == stationeryCategory);
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                stationaries = stationaries.Where(s => s.Name.Contains(searchString));

            }

             foreach (var item in stationaries)
                {

                    foreach (var vari in item.Variants)
                    {

                        vari.ColourDisplay = vari.Colour.ToLower();
                    }


                    item.Amount = item.Variants.Where(x => x.Stocked).Count();
                }

            stationaries = stationaries.OrderBy(x => x.ForSubject);
            return View(stationaries);
        }


        public ActionResult Create()
        {
            LoadSubjects();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Stationary_ID,Category,Name,Image,ForSubject,Description,Price")]Stationary stationery)
        {
            LoadSubjects();

            if (ModelState.IsValid)
            {
                if(stationery.Image == null ||stationery.Image == "" ){
                    stationery.Image = "/Content/Default.jpg";
                }
                if (stationery.Description == null || stationery.Description == "")
                {
                    stationery.Description = "No Description";
                }
                stationery.Favorite = false;

                db.Stationaries.Add(stationery);
                db.SaveChanges();

                Variant vari = new Variant();
                vari.Stocked = false;
                vari.Colour = "Default";
                vari.Status = "New";
                vari.Stationary_ID = stationery.Stationary_ID;
                vari.Amount = 1;
                db.Variants.Add(vari);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View();
        }

        public ActionResult NewVariant(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Variant vari = new Variant();
            vari.Stationary_ID = id.Value;
            return View(vari);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewVariant([Bind(Include = "Colour,Status,Stocked,Stationary_ID,Amount")]Variant variant)
        {
            variant.submit = true;
            if (ModelState.IsValid)
            {
                if (variant.Colour == null || variant.Colour == "")
                {
                    variant.Colour = "Default";
                }
                else 
                {
                    variant.Colour = variant.Colour.Replace(" ","");
                }
                if (variant.Status == null || variant.Status == "")
                {
                    variant.Status = "New";
                }
                variant.creationsucess = true;

                for (int i = 0; i < variant.Amount; i++)
                {
                    db.Variants.Add(variant);
                    db.SaveChanges();
                }
                return View(variant);
            }
            variant.creationsucess = false;
            return View(variant);
        }

        public ActionResult Display(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }

            Stationary stationery = db.Stationaries.Find(id);

            if (stationery == null)
            {
                return HttpNotFound();
            }
            return View(stationery);
        }

        public ActionResult Delete(int? id)
        {
            Stationary stationery = db.Stationaries.Find(id);
            return View(stationery);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int? id)
        {
            Stationary stationery = db.Stationaries.Find(id);
            List<Variant> variants = new List<Variant>();
            variants = db.Variants.Where(x => x.Stationary_ID == id).ToList();
            foreach (var vari in variants)
            {
                        db.Variants.Remove(vari);
            }
            db.Stationaries.Remove(stationery);
            db.SaveChanges();
            return RedirectToAction("Index");

        }

        public ActionResult Edit(int? id)
        {
            Stationary stationery = db.Stationaries.Find(id);
            stationery.Variants = stationery.Variants.OrderBy(x => x.Colour).ToList();
            return View(stationery);
        }


        [HttpPost, ActionName("Edit")]

        public ActionResult Edit(
            [Bind(Include = "Stationary_ID,Category,Name,ForSubject,Image,Description,Price,Variants")]
            Stationary stationery)
        {
            int count = 1;
            foreach (Variant vari in stationery.Variants)
                 {
                     if (stationery.Variants.Count != count)
                        {
                           if (vari.markedfordeletion)
                          {
                              count++;
                              Variant variant = db.Variants.Find(vari.Variant_ID);
                              db.Variants.Remove(variant);
                          }
                      }
                  }
                        db.SaveChanges();

            if (ModelState.IsValid)
             {
                 foreach (Variant vari in stationery.Variants)
                 {
                     if (!vari.markedfordeletion)
                     {
                         if (vari.Colour == null || vari.Colour == "")
                         {
                             vari.Colour = "Default";
                         }

                         if (vari.Status == null || vari.Status == "")
                         {
                             vari.Status = "Unspecified";
                         }

                         db.Entry(vari).State = EntityState.Modified;
                     }
                 }
     
            db.Entry(stationery).State = EntityState.Modified;


            db.SaveChanges();


            return RedirectToAction("Index");
             }
             return View(stationery);
        }


        public void LoadSubjects()
        {
            var SubjectList = new List<string>();

            SubjectList.Add("General");
            SubjectList.Add("Math");
            SubjectList.Add("Art");

            Stationary.SubjectList = new SelectList(SubjectList);

        }
	}
}