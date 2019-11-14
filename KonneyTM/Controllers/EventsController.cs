using KonneyTM.DAL;
using KonneyTM.Models;
using KonneyTM.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Authentication;
using System.Web;
using System.Web.Mvc;

namespace KonneyTM.Controllers
{
    public class EventsController : Controller
    {
        public ActionResult Index()
        {
            UserViewModel userVM = new UserViewModel();

            if (User.Identity.IsAuthenticated)
            {
                var userID = User.Identity.GetUserId();

                using (var db = new KonneyContext())
                {
                    if (db.Users.FirstOrDefault(x => x.ID == userID) == null)
                    {
                        db.Users.Add(new User { ID = userID });
                        db.SaveChanges();
                    }
                }

                userVM.Fill(userID);
                return View(userVM);
            }
            else
            {
                userVM.Fill("demo");
                return View(userVM);
            }
        }
        
        public ActionResult Create()
        {
            EventViewModel eventVM;

            if (User.Identity.IsAuthenticated)
            {
                var userID = User.Identity.GetUserId();
                eventVM = new EventViewModel(userID);
            }
            else
            {
                eventVM = new EventViewModel("demo");
            }

            return View(eventVM);
        }

        [HttpPost]
        public ActionResult Create(EventViewModel eventVM)
        {
            if (ModelState.IsValid)
            {
                //Image Upload Logic
                if (User.Identity.IsAuthenticated)
                {
                    var userID = User.Identity.GetUserId();
                    UploadImage(eventVM, userID);
                    eventVM.SaveToDB(userID);
                }
                else
                {
                    UploadImage(eventVM, "demo");
                    eventVM.SaveToDB("demo");
                }

                return RedirectToAction("Index");
            }

            return View(eventVM);
        }

        public ActionResult Event(int id)
        {
            using (var db = new KonneyContext())
            {
                var ev = db.Events.First(e => e.ID == id);
                
                if (User.Identity.IsAuthenticated)
                {
                    string userID = User.Identity.GetUserId();

                    if (userID == ev.User.ID)
                    {
                        var eventVM = EventViewModel.FromEvent(ev);
                        return View(eventVM);
                    }
                    else
                    {
                        throw new AuthenticationException("You are not authorized to edit this event.");
                    }
                }
                else if (ev.User.ID == "demo")
                {
                    var eventVM = EventViewModel.FromEvent(ev);
                    return View(eventVM);
                }
                else
                {
                    throw new Exception("Something went wrong...");
                }
            }
        }

        [HttpPost]
        public ActionResult Edit(EventViewModel eventVM)
        {
            if (ModelState.IsValid)
            {
                using (var context = new KonneyContext())
                {
                    //Get the related event
                    var ev = context.Events.Single(e => e.ID == eventVM.ID);

                    //If the user is logged in
                    if (User.Identity.IsAuthenticated)
                    {
                        //Get User ID
                        var userID = User.Identity.GetUserId();

                        //If the event belongs to the user
                        if (ev.User.ID == userID) 
                        {
                            //If an image file is selected
                            if (eventVM.ImageFile != null)
                            {
                                UploadImage(eventVM, userID);
                            }
                        }
                        else
                        {
                            throw new AuthenticationException("You are not authorized to edit this event.");
                        }
                    }
                    //If the application is in demo mode
                    else if (ev.User.ID == "demo")
                    {
                        if(eventVM.ImageFile != null)
                        {
                            UploadImage(eventVM, "demo");
                        }

                        eventVM.SubmitChanges();
                        return RedirectToAction("Event", new { id = eventVM.ID });
                    }
                    else
                    {
                        throw new Exception("Something went wrong...");
                    }

                    eventVM.SubmitChanges();
                    return RedirectToAction("Event", new { id = eventVM.ID });
                }
            }
            return RedirectToAction("Event", new { id = eventVM.ID });
        }

        public ActionResult Delete(int id)
        {
            using (var db = new KonneyContext())
            {
                var ev = db.Events.First(e => e.ID == id);

                if (User.Identity.IsAuthenticated)
                {
                    string userID = User.Identity.GetUserId();

                    if (userID == ev.User.ID)
                    {
                        db.Events.Remove(ev);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        throw new AuthenticationException("You are not authorized to delete this event.");
                    }
                }
                else if (ev.User.ID == "demo")
                {
                    db.Events.Remove(ev);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            throw new Exception("Something went wrong...");
        }

        public ActionResult ChangeVenue(int eventID)
        {
            using (var context = new KonneyContext())
            {
                var relatedEvent = context.Events.Single(e => e.ID == eventID);
                
                if(User.Identity.IsAuthenticated)
                {
                    var userID = User.Identity.GetUserId();

                    if(relatedEvent.User.ID == userID)
                    {
                        var changeVenue = new ChangeVenueVM(userID) { EventID = eventID };
                        return View(changeVenue);
                    }
                    else
                    {
                        throw new AuthenticationException("You are not authorized to change the venue of this event.");
                    }
                }
                else if(relatedEvent.User.ID == "demo")
                {
                    var changeVenue = new ChangeVenueVM("demo") { EventID = eventID };
                    return View(changeVenue);
                }
                else
                {
                    throw new Exception("Something went wrong.");
                }
            }
        }

        public ActionResult SubmitVenueChange(int eventID, int venueID)
        {
            using (var db = new KonneyContext())
            {
                var relatedEvent = db.Events.First(e => e.ID == eventID);

                if (User.Identity.IsAuthenticated)
                {
                    var userID = User.Identity.GetUserId();
                    if (relatedEvent.User.ID == userID)
                    {
                        relatedEvent.Place = db.Venues.First(v => v.ID == venueID);
                        db.SaveChanges();
                    }
                    else
                    {
                        throw new AuthenticationException("You are not authorized to change the venue of this event.");
                    }
                }
                else if (relatedEvent.User.ID == "demo")
                {
                    relatedEvent.Place = db.Venues.First(v => v.ID == venueID);
                    db.SaveChanges();
                }
                else
                {
                    throw new Exception("Something went wrong...");
                }
            }

            return RedirectToAction("Event", new { id = eventID });
        }

        public ActionResult AddPerson(int eventID)
        {
            using (var db = new KonneyContext())
            {
                var relatedEvent = db.Events.First(e => e.ID == eventID);

                if (User.Identity.IsAuthenticated)
                {
                    var userID = User.Identity.GetUserId();

                    if (relatedEvent.User.ID == userID)
                    {
                        var addPerson = new AddPersonVM { EventID = eventID };
                        var allPeople = PersonViewModel.GetAll(userID);
                        var eventVM = EventViewModel.FromEvent(relatedEvent);

                        foreach (var p in allPeople)
                        {
                            if (!eventVM.InvitedPeopleIDs.Contains(p.ID))
                            {
                                addPerson.People.Add(p);
                            }
                        }

                        return View(addPerson);
                    }
                    else
                    {
                        throw new AuthenticationException("You are not authorized to add people to this event.");
                    }
                }
                else if (relatedEvent.User.ID == "demo")
                {
                    var addPerson = new AddPersonVM { EventID = eventID };
                    var allPeople = PersonViewModel.GetAll("demo");
                    var eventVM = EventViewModel.FromEvent(relatedEvent);

                    foreach (var p in allPeople)
                    {
                        if (!eventVM.InvitedPeopleIDs.Contains(p.ID))
                        {
                            addPerson.People.Add(p);
                        }
                    }

                    return View(addPerson);
                }
                else
                {
                    throw new Exception("Something went wrong.");
                }
            }
        }

        public ActionResult SubmitPerson(int eventID, int personID)
        {
            using (var db = new KonneyContext())
            { 
                var relatedEvent = db.Events.First(e => e.ID == eventID);
                var relatedPerson = db.People.First(p => p.ID == personID);
                var eventVM = EventViewModel.FromEvent(relatedEvent);

                if (User.Identity.IsAuthenticated)
                {
                    var userID = User.Identity.GetUserId();

                    if(userID == relatedEvent.User.ID && userID == relatedPerson.User.ID)
                    {
                        eventVM.InvitedPeopleIDs.Add(relatedPerson.ID);
                        eventVM.SubmitChanges();
                    }
                    else
                    {
                        throw new AuthenticationException("You are not authorized to add people to this event.");
                    }
                }
                else if (relatedPerson.User.ID == "demo" && relatedEvent.User.ID == "demo")
                {
                    eventVM.InvitedPeopleIDs.Add(relatedPerson.ID);
                    eventVM.SubmitChanges();
                }
                else
                {
                    throw new Exception("Something went wrong.");
                }
            }

            return RedirectToAction("Event", new { id = eventID });
        }

        public ActionResult RemovePerson(int eventID, int personID)
        {
            using (var db = new KonneyContext())
            { 
                var relatedEvent = db.Events.First(e => e.ID == eventID);
                var eventVM = EventViewModel.FromEvent(relatedEvent);

                if (User.Identity.IsAuthenticated)
                {
                    var userID = User.Identity.GetUserId();
                    if (relatedEvent.User.ID == userID)
                    {
                        eventVM.InvitedPeopleIDs.RemoveAll(i => i == personID);
                        eventVM.SubmitChanges();
                    }
                    else
                    {
                        throw new AuthenticationException("You are not authorized to remove people from this event.");
                    }
                }
                else if (relatedEvent.User.ID == "demo")
                {
                    eventVM.InvitedPeopleIDs.RemoveAll(i => i == personID);
                    eventVM.SubmitChanges();
                }
                else
                {
                    throw new Exception("Something went wrong");
                }
            }

            return RedirectToAction("Event", new { id = eventID });
        }

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