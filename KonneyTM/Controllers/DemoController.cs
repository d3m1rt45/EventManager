using KonneyTM.DAL;
using KonneyTM.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KonneyTM.Controllers
{
    public class DemoController : Controller
    {
        private KonneyContext db = new KonneyContext();

        public ActionResult Events()
        {
            return View(db.Events.ToList());
        }

        public ActionResult NewEvent()
        {
            var eventVM = new EventViewModel();
            return View(eventVM);
        }

        [HttpPost]
        public ActionResult NewEvent(EventViewModel eventVM)
        {
            if(ModelState.IsValid)
            {
                eventVM.SaveAsEvent();
                return RedirectToAction("Events");
            }
            else
            {
                return View(eventVM);
            }
        }

        public ActionResult People()
        {
            var peopleByName = db.People.OrderBy(p => p.FirstName).ToList();
            return View(peopleByName);
        }

        public ActionResult NewPerson()
        {
            var personVM = new PersonViewModel();

            return View(personVM);
        }

        [HttpPost]
        public ActionResult NewPerson(PersonViewModel personVM)
        {
            if (ModelState.IsValid)
            {
                personVM.SaveAsPerson();
                return RedirectToAction("People");
            }
            else
            {
                return View(personVM);
            }
        }

        public ActionResult EditPerson(int id)
        {
            var personVM = PersonViewModel.ConvertPerson(db.People.First(p => p.ID == id));
            return View(personVM);
        }

        [HttpPost]
        public ActionResult EditPerson(PersonViewModel personVM)
        {
            if (ModelState.IsValid)
            {
                personVM.SubmitChanges();
                return RedirectToAction("People");
            }

            return View(personVM);
        }

        public ActionResult Venues()
        {
            return View(db.Venues.ToList());
        }
    }
}