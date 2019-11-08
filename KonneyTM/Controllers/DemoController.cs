using KonneyTM.DAL;
using KonneyTM.ExtensionMethods;
using KonneyTM.Models;
using KonneyTM.ViewModels;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
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
            var eventVMs = EventViewModel.GetAllAsOrderedList();
            return View(eventVMs);
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
                string extension = Path.GetExtension(eventVM.ImageFile.FileName);
                string imageFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
                eventVM.ImagePath = imageFileName;
                imageFileName = Path.Combine(Server.MapPath("~/Images/Events"), imageFileName);
                eventVM.ImageFile.SaveAs(imageFileName);

                eventVM.SaveToDB(); //Converts the EventViewModel instance to an Event instance and saves to to the database.
                return RedirectToAction("Events");
            }

            return View(eventVM);
        }

        public ActionResult EventDetails(int id)
        {
            var eventVM = EventViewModel.FromEvent(db.Events.First(e => e.ID == id));
            return View(eventVM);
        }

        public ActionResult EditEvent(EventViewModel eventVM)
        {
            if (ModelState.IsValid)
            {
                eventVM.SubmitChanges();

                return RedirectToAction("EventDetails", new { id = eventVM.ID });
            }
            return RedirectToAction("EventDetails", new { id = eventVM.ID });
        }

        public ActionResult ChangeVenue(int eventID)
        {
            var changeVenue = new ChangeVenueVM { EventID = eventID };
            return View(changeVenue);
        }

        public ActionResult SubmitVenueChange(int eventID, int venueID)
        {
            var eventToChange = db.Events.First(e => e.ID == eventID);
            eventToChange.Place = db.Venues.First(v => v.ID == venueID);
            db.SaveChanges();

            return RedirectToAction("EventDetails", new { id = eventID });
        }

        public ActionResult AddPerson(int eventID)
        {
            var addPerson = new AddPersonVM { EventID = eventID };
            var relatedEvent = EventViewModel.FromEvent(db.Events.First(e => e.ID == eventID));
            var allPeople = PersonViewModel.GetAllAsOrderedList();

            foreach(var p in allPeople)
            {
                if (!relatedEvent.InvitedPeopleIDs.Contains(p.ID))
                {
                    addPerson.People.Add(p);
                }
            }

            return View(addPerson);
        }

        public ActionResult SubmitPerson(int eventID, int personID)
        {
            var relatedEvent = EventViewModel.FromEvent(db.Events.First(e => e.ID == eventID));
            var relatedPerson = PersonViewModel.FromPerson(db.People.First(p => p.ID == personID));
            relatedEvent.InvitedPeopleIDs.Add(relatedPerson.ID);
            relatedEvent.SubmitChanges();
            return RedirectToAction("EventDetails", new { id = eventID });
        }

        public ActionResult RemovePerson(int eventID, int personID)
        {
            var relatedEvent = EventViewModel.FromEvent(db.Events.First(e => e.ID == eventID));
            relatedEvent.InvitedPeopleIDs.RemoveAll(i => i == personID);
            relatedEvent.SubmitChanges();
            return RedirectToAction("EventDetails", new { id = eventID });
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
                personVM.SaveToDB();
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

        public ActionResult DeletePerson(int id)
        {
            var person = db.People.First(p => p.ID == id);
            db.People.Remove(person);
            db.SaveChanges();
            return RedirectToAction("People");
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
                string extension = Path.GetExtension(venueVM.ImageFile.FileName);
                string imageFileName = venueVM.PhoneNumber.RemoveWhitespace() + venueVM.PostCode.RemoveWhitespace() + extension;
                venueVM.ImagePath = imageFileName;
                imageFileName = Path.Combine(Server.MapPath("~/Images/Venues"), imageFileName);
                venueVM.ImageFile.SaveAs(imageFileName);

                venueVM.SaveToDB();
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

        public ActionResult DeleteVenue(int id)
        {
            //Remove the related events to make the deletion of Venue possible.
            var relatedEvents = db.Events.Where(p => p.Place.ID == id).ToList();
            foreach(var ev in relatedEvents)
            {
                db.Events.Remove(ev);
            }

            //Remove the venue and save changes
            var venue = db.Venues.First(v => v.ID == id);
            db.Venues.Remove(venue);
            db.SaveChanges();

            return RedirectToAction("Venues");
        }

        
    }
}