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
            return View(db.Events);
        }

        public ActionResult NewEvent()
        {
            var newEventVM = new NewEventViewModel();
            
            return View(newEventVM);
        }

        [HttpPost]
        public ActionResult NewEvent(NewEventViewModel newEventVM)
        {
            if(ModelState.IsValid)
            {
                newEventVM.SaveAsEvent();

                return RedirectToAction("Events");
            }
            else
            {
                return View(newEventVM);
            }
        }

        public ActionResult People()
        {
            return View(db.People.OrderBy(p => p.FirstName));
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
                var person = new Person
                {
                    FirstName = personVM.FirstName,
                    LastName = personVM.LastName,
                    PhoneNumber = personVM.PhoneNumber,
                    Email = personVM.Email
                };

                db.People.Add(person);
                db.SaveChanges();

                return RedirectToAction("People");
            }
            else
            {
                return View(personVM);
            }
        }

        public ActionResult EditPerson(int id)
        {
            var person = db.People.First(p => p.ID == id);

            var personVM = new PersonViewModel
            {
                ID = person.ID,
                FirstName = person.FirstName,
                LastName = person.LastName,
                PhoneNumber = person.PhoneNumber,
                Email = person.Email
            };

            return View(personVM);
        }

        [HttpPost]
        public ActionResult EditPerson(PersonViewModel personVM)
        {
            if (ModelState.IsValid)
            {
                var person = db.People.First(p => p.ID == personVM.ID);

                person.FirstName = personVM.FirstName;
                person.LastName = personVM.LastName;
                person.Email = personVM.Email;
                person.PhoneNumber = personVM.PhoneNumber;

                db.SaveChanges();
                return RedirectToAction("People");
            }

            return View(personVM);
        }

        public ActionResult Venues()
        {
            return View();
        }
    }
}