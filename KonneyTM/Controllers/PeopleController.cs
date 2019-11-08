using KonneyTM.DAL;
using KonneyTM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KonneyTM.Controllers
{
    public class PeopleController : Controller
    {
        public ActionResult Index()
        {
            //Returns a list of PersonViewModel populated by each person in db.People, ordered by their first name
            return View(PersonViewModel.GetAllAsOrderedList());
        }

        public ActionResult Create()
        {
            return View(new PersonViewModel());
        }

        [HttpPost]
        public ActionResult Create(PersonViewModel personVM)
        {
            if (ModelState.IsValid)
            {
                personVM.SaveToDB();
                return RedirectToAction("Index");
            }
            return View(personVM);
        }

        public ActionResult Edit(int id)
        {
            using (var db = new KonneyContext())
            { 
                var personVM = PersonViewModel.FromPerson(db.People.First(p => p.ID == id));
                return View(personVM);
            }
        }

        [HttpPost]
        public ActionResult Edit(PersonViewModel personVM)
        {
            if (ModelState.IsValid)
            {
                personVM.SubmitChanges();
                return RedirectToAction("Index");
            }
            return View(personVM);
        }

        public ActionResult Delete(int id)
        {
            using (var db = new KonneyContext())
            {
                var person = db.People.First(p => p.ID == id);
                db.People.Remove(person);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}