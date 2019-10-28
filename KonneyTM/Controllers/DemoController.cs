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

        [HttpGet]
        public ActionResult NewEvent()
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
                }) ;
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
            return View("Events.cshtml", db.Events);
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