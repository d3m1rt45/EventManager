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

        public ActionResult SetTitleDateTime()
        {
            var nevm = new TitleDateTimeVM();
            
            return View(nevm);
        }

        [HttpPost]
        public ActionResult SetTitleDateTime(TitleDateTimeVM nevm)
        {
            if(ModelState.IsValid)
            {
                var newEvent = new Event
                {
                    Title = nevm.Title,
                    Date = nevm.Date,
                    Time = nevm.Time
                };

                return RedirectToAction("SetVenue", "Demo", newEvent);
            }

            return View(nevm);
        }

        public ActionResult SetVenue(Event newEvent)
        {
            return View();
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