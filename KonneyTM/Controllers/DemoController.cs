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

        public ActionResult Index()
        {
            var events = db.Events;
            return View(events);
        }

        public ActionResult NewEvent()
        {
            var nevm = new NewEventViewModel();
            
            return View(nevm);
        }

        [HttpPost]
        public ActionResult NewEvent(NewEventViewModel nevm)
        {
            if(ModelState.IsValid)
            {
                var newEvent = new Event
                {
                    Title = nevm.Title,
                    Date = nevm.Date,
                    Time = nevm.Time,
                    Place = db.Venues.First(v => v.ID == nevm.SelectedVenueID),
                    PeopleAttending = db.People.Where(x => nevm.InvitedPeopleIDs.Contains(x.ID)).ToList()
                };

                db.Events.Add(newEvent);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            else
            {
                return View(nevm);
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