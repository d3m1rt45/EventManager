using KonneyTM.DAL;
using KonneyTM.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KonneyTM.Controllers
{
    public class EventsController : Controller
    {
        public ActionResult Index()
        {
            var eventVMs = EventViewModel.GetAllAsOrderedList();
            return View(eventVMs);
        }

        public ActionResult Create()
        {
            var eventVM = new EventViewModel();
            return View(eventVM);
        }

        [HttpPost]
        public ActionResult Create(EventViewModel eventVM)
        {
            if (ModelState.IsValid)
            {
                string extension = Path.GetExtension(eventVM.ImageFile.FileName);
                string imageFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
                eventVM.ImagePath = imageFileName;
                imageFileName = Path.Combine(Server.MapPath("~/Images/Events"), imageFileName);
                eventVM.ImageFile.SaveAs(imageFileName);

                eventVM.SaveToDB(); //Converts the EventViewModel instance to an Event instance and saves to to the database.
                return RedirectToAction("Index");
            }

            return View(eventVM);
        }

        public ActionResult Event(int id)
        {
            using (var db = new KonneyContext())
            { 
                var eventVM = EventViewModel.FromEvent(db.Events.First(e => e.ID == id));
                return View(eventVM);
            }
        }

        [HttpPost]
        public ActionResult Edit(EventViewModel eventVM)
        {
            if (ModelState.IsValid)
            {
                if (eventVM.ImageFile != null)
                {
                    string extension = Path.GetExtension(eventVM.ImageFile.FileName);
                    string imageFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
                    eventVM.ImagePath = imageFileName;
                    imageFileName = Path.Combine(Server.MapPath("~/Images/Events"), imageFileName);
                    eventVM.ImageFile.SaveAs(imageFileName);
                }

                eventVM.SubmitChanges();
                return RedirectToAction("Event", new { id = eventVM.ID });
            }
            return RedirectToAction("Event", new { id = eventVM.ID });
        }

        public ActionResult ChangeVenue(int eventID)
        {
            var changeVenue = new ChangeVenueVM { EventID = eventID };
            return View(changeVenue);
        }

        public ActionResult SubmitVenueChange(int eventID, int venueID)
        {
            using (var db = new KonneyContext())
            {
                var eventToChange = db.Events.First(e => e.ID == eventID);
                eventToChange.Place = db.Venues.First(v => v.ID == venueID);
                db.SaveChanges();
            }

            return RedirectToAction("Event", new { id = eventID });
        }

        public ActionResult AddPerson(int eventID)
        {
            using (var db = new KonneyContext())
            {
                var addPerson = new AddPersonVM { EventID = eventID };
                var relatedEvent = EventViewModel.FromEvent(db.Events.First(e => e.ID == eventID));
                var allPeople = PersonViewModel.GetAllAsOrderedList();

                foreach (var p in allPeople)
                {
                    if (!relatedEvent.InvitedPeopleIDs.Contains(p.ID))
                    {
                        addPerson.People.Add(p);
                    }
                }
                
                return View(addPerson);
            }
        }

        public ActionResult SubmitPerson(int eventID, int personID)
        {
            using (var db = new KonneyContext())
            { 
                var relatedEvent = EventViewModel.FromEvent(db.Events.First(e => e.ID == eventID));
                var relatedPerson = PersonViewModel.FromPerson(db.People.First(p => p.ID == personID));
                relatedEvent.InvitedPeopleIDs.Add(relatedPerson.ID);
                relatedEvent.SubmitChanges();
            }
            return RedirectToAction("Event", new { id = eventID });
        }

        public ActionResult RemovePerson(int eventID, int personID)
        {
            using (var db = new KonneyContext())
            { 
                var relatedEvent = EventViewModel.FromEvent(db.Events.First(e => e.ID == eventID));
                relatedEvent.InvitedPeopleIDs.RemoveAll(i => i == personID);
                relatedEvent.SubmitChanges();
            }
            return RedirectToAction("Event", new { id = eventID });
        }
    }
}