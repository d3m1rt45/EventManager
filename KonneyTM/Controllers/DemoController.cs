using KonneyTM.DAL;
using KonneyTM.Models;
using KonneyTM.ViewModels;
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
            return View(EventViewModel.GetAllAsOrderedList());
        }

        public ActionResult NewEvent()
        {
            return View(new EventViewModel());
        }

        [HttpPost]
        public ActionResult NewEvent(EventViewModel eventVM)
        {
            if(ModelState.IsValid)
            {
                eventVM.SaveAsEvent();
                return RedirectToAction("Events");
            }

            return View(eventVM);
        }

        public ActionResult People()
        {
            //Returns a list of PersonViewModel populated by each person in db.People, ordered by their first name
            return View(PersonViewModel.GetAllAsOrderedList());
        }

        public ActionResult NewPerson()
        {
            return View(new PersonViewModel());
        }

        [HttpPost]
        public ActionResult NewPerson(PersonViewModel personVM)
        {
            if (ModelState.IsValid)
            {
                personVM.SaveAsPerson();
                return RedirectToAction("People");
            }
            return View(personVM);
        }

        public ActionResult EditPerson(int id)
        {
            var personVM = PersonViewModel.FromPerson(db.People.First(p => p.ID == id));
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
            return View(VenueViewModel.GetAllAsOrderedList());
        }

        public ActionResult NewVenue()
        {
            var venueVM = new VenueViewModel();

            return View(venueVM);
        }

        [HttpPost]
        public ActionResult NewVenue(VenueViewModel venueVM)
        {
            if (ModelState.IsValid)
            {
                venueVM.SaveAsVenue();
                return RedirectToAction("Venues");
            }

            return View(venueVM);
        }

        public ActionResult EditVenue(int id)
        {
            //Return a view model based on the id that's being passed.
            return View(VenueViewModel.FromVenue(db.Venues.First(p => p.ID == id)));
        }

        [HttpPost]
        public ActionResult EditVenue(VenueViewModel venueVM)
        {
            if(ModelState.IsValid)
            {
                venueVM.SubmitChanges();
                return RedirectToAction("Venues");
            }
            return View(venueVM);
        }
    }
}