using KonneyTM.DAL;
using KonneyTM.Models;
using KonneyTM.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KonneyTM.Controllers
{
    // <summary>
    // This class deals with all functionality in the application that has to
    // do with events. I have used a similar method of checking whether an item in
    // question belongs to the user trying to make the change, all throughout the app.
    // </summary>

    [HandleError]
    public class EventsController : Controller
    {
        // Entity Framework Database Context
        readonly KonneyContext db = new KonneyContext();

        // If the user is logged in, navigate to their panel. If not, run the demo.
        public ActionResult Index()
        {
            UserViewModel userVM = new UserViewModel();

            if (User.Identity.IsAuthenticated) 
            {
                var userID = User.Identity.GetUserId();
                Models.User.FindOrCreate(db, userID);
                userVM.Fill(db, userID);
            }
            else
                userVM.Fill(db, "demo");

            return View(userVM); 
        }

        // Create an event.
        public ActionResult Create()
        {
            EventViewModel eventVM;

            if (User.Identity.IsAuthenticated)
                eventVM = new EventViewModel(db, User.Identity.GetUserId());
            else
                eventVM = new EventViewModel(db, "demo");

            return View(eventVM);
        }

        [HttpPost]
        public ActionResult Create(EventViewModel eventVM)
        {
            if (ModelState.IsValid)
            {
                UploadImage(eventVM, eventVM.UserID);
                Models.Event.SaveByViewModel(db, eventVM);

                return RedirectToAction("Index");
            }
            return View(eventVM);
        }

        // Return an individual Event page with the given ID
        public ActionResult Event(int id)
        {
            var subjectEvent = db.Events.First(e => e.ID == id);
                
            if (User.Identity.IsAuthenticated)
            {
                if (subjectEvent.User.ID != User.Identity.GetUserId())
                    throw new AuthenticationException("You are not authorized to edit this event.");
            }
            else if (subjectEvent.User.ID != "demo")
                throw new Exception("Something went wrong...");

            return View(subjectEvent.ToEventViewModel(db));
        }

        // Activate up a submit click on any of the edit modals.
        [HttpPost]
        public ActionResult Edit(EventViewModel eventVM)
        {
            if (ModelState.IsValid)
            {
                var subjectEvent = db.Events.Single(e => e.ID == eventVM.ID);
                string userID = "demo";

                if (User.Identity.IsAuthenticated)
                {
                    userID = User.Identity.GetUserId();

                    if (subjectEvent.User.ID != userID) 
                        throw new AuthenticationException("You are not authorized to edit this event.");
                }
                else if (subjectEvent.User.ID != userID)
                    throw new Exception("Something went wrong...");    

                if (eventVM.ImageFile != null)
                    UploadImage(eventVM, userID);

                Models.Event.SubmitChangesByViewModel(db, eventVM);
            }

            return RedirectToAction("Event", new { id = eventVM.ID });
        }

        // Delete an event
        public ActionResult Delete(int id)
        {
            var ev = db.Events.First(e => e.ID == id);

            if (User.Identity.IsAuthenticated)
            {
                if (User.Identity.GetUserId() != ev.User.ID)
                    throw new AuthenticationException("You are not authorized to delete this event.");
            }
            else if (ev.User.ID != "demo")
                throw new Exception("Something went wrong...");

            db.Events.Remove(ev);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // Return list of venues for the user to choose which one to set as the new venue
        public ActionResult ChangeVenue(int eventID)
        {
            var relatedEvent = db.Events.Single(e => e.ID == eventID);
            string userID = "demo";
                
            if(User.Identity.IsAuthenticated)
            {
                userID = User.Identity.GetUserId();
                if(relatedEvent.User.ID != userID)
                    throw new AuthenticationException("You are not authorized to change the venue of this event.");
            }
            else if(relatedEvent.User.ID != "demo")
                throw new Exception("Something went wrong.");

            return View(new ChangeVenueVM(db, userID) { EventID = eventID });
        }

        // Makes the subjected event's venue the venue that was clicked
        public ActionResult SubmitVenueChange(int eventID, int venueID)
        {
            var subjectEvent = db.Events.First(e => e.ID == eventID);

            if (User.Identity.IsAuthenticated)
            {
                if (User.Identity.GetUserId() != subjectEvent.User.ID)
                    throw new AuthenticationException("You are not authorized to change the venue of this event.");
            }
            else if (subjectEvent.User.ID != "demo")
                throw new Exception("Something went wrong...");

            subjectEvent.Place = db.Venues.First(v => v.ID == venueID);
            db.SaveChanges();
            return RedirectToAction("Event", new { id = eventID });
        }

        // Return list of venues for the user to choose which one to add to the Event
        public ActionResult AddPerson(int eventID)
        {
            var relatedEvent = db.Events.First(e => e.ID == eventID);
            string userID = "demo";

            if (User.Identity.IsAuthenticated)
                userID = User.Identity.GetUserId();

            return View(Person.ReturnAddPersonVMIfIDsMatch(db, relatedEvent, User.Identity.GetUserId()));
        }

        // Add the person who was clicked on the AddPerson view to the subject event
        public ActionResult SubmitPerson(int eventID, int personID)
        {
            var subjectEvent = db.Events.First(e => e.ID == eventID);
            var person = db.People.First(p => p.ID == personID);

            if (User.Identity.IsAuthenticated)
            {
                var userID = User.Identity.GetUserId();

                if(userID != subjectEvent.User.ID || userID != subjectEvent.User.ID)
                    throw new AuthenticationException("You are not authorized to add people to this event.");
            }
            else if (person.User.ID != "demo" || person.User.ID != "demo")
                throw new Exception("Something went wrong.");

            var eventVM = subjectEvent.ToEventViewModel(db);
            eventVM.InvitedPeopleIDs.Add(person.ID);
            Models.Event.SubmitChangesByViewModel(db, eventVM);
            return RedirectToAction("Event", new { id = eventID });
        }

        // Remove a person from the event
        public ActionResult RemovePerson(int eventID, int personID)
        {
            var subjectEvent = db.Events.First(e => e.ID == eventID);
            var eventVM = subjectEvent.ToEventViewModel(db);

            if (User.Identity.IsAuthenticated)
            {
                if (subjectEvent.User.ID != User.Identity.GetUserId())
                    throw new AuthenticationException("You are not authorized to remove people from this event.");
            }
            else if (subjectEvent.User.ID != "demo")
                throw new Exception("Something went wrong...");

            eventVM.InvitedPeopleIDs.RemoveAll(i => i == personID);
            Models.Event.SubmitChangesByViewModel(db, eventVM);
            return RedirectToAction("Event", new { id = eventID });
        }

        // Uploading image for event.
        public void UploadImage(EventViewModel eventVM, string userID)
        {
            string extension = Path.GetExtension(eventVM.ImageFile.FileName);
            string imageFileName = $"{userID}{DateTime.Now.ToString("yyyyMMddHHmmss")}{extension}";
            eventVM.ImagePath = imageFileName;
            imageFileName = Path.Combine(Server.MapPath($"~/Images/Events/") + imageFileName);
            eventVM.ImageFile.SaveAs(imageFileName);
        }
    }
}