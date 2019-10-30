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
            var events = db.Events;
            return View(events);
        }

        public ActionResult NewEvent()
        {
            var peopleList = new List<PersonViewModel>();
            foreach (var p in db.People)
            {
                peopleList.Add(new PersonViewModel
                {
                    ID = p.ID,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    Email = p.Email,
                    PhoneNumber = p.PhoneNumber,
                    Attending = false
                });
            }

            var venueList = new List<VenueViewModel>();
            foreach (var v in db.Venues)
            {
                venueList.Add(new VenueViewModel
                {
                    ID = v.ID,
                    Name = v.Name,
                    PhoneNumber = v.PhoneNumber,
                    Address = v.Address,
                    PostCode = v.PostCode,
                    Checked = false
                });
            }

            var newEvent = new NewEventViewModel
            {
                People = peopleList,
                Venues = venueList
            };

            return View(newEvent);
        }

        [HttpPost]
        public ActionResult NewEvent(NewEventViewModel nevm)
        {
            if (!ModelState.IsValid)
            {
                var peopleList = new List<PersonViewModel>();
                foreach (var p in db.People)
                {
                    peopleList.Add(new PersonViewModel
                    {
                        FirstName = p.FirstName,
                        LastName = p.LastName,
                        Email = p.Email,
                        PhoneNumber = p.PhoneNumber,
                        Attending = false
                    });
                }

                var venueList = new List<VenueViewModel>();
                foreach (var v in db.Venues)
                {
                    venueList.Add(new VenueViewModel
                    {
                        Name = v.Name,
                        PhoneNumber = v.PhoneNumber,
                        Address = v.Address,
                        PostCode = v.PostCode,
                        Checked = false
                    });
                }

                nevm.People = peopleList;
                nevm.Venues = venueList;
                
                return View(nevm);
            }
            else
            {
                var arrangedVenue = new Venue();
                foreach (var v in nevm.Venues)
                {
                    if (v.Checked)
                    {
                        arrangedVenue = db.Venues.First(o => o.ID == v.ID);
                    }
                }

                db.Events.Add(new Event
                {
                    Title = nevm.Title,
                    Place = arrangedVenue,
                    PeopleAttending = nevm.People,
                    Date = nevm.Date,
                    Time = nevm.Time
                });

                db.SaveChanges();

                return View("Events", db.Events);
            }
        }


        public ActionResult People()
        {
            return View();
        }

        public ActionResult Venues()
        {
            return View();
        }
    }
}