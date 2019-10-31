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
            var newEventVM = new NewEventViewModel();
            
            return View(newEventVM);
        }

        [HttpPost]
        public ActionResult NewEvent(NewEventViewModel newEventVM)
        {
            if(ModelState.IsValid)
            {
                newEventVM.SaveAsEvent();

                return RedirectToAction("Index");
            }
            else
            {
                return View(newEventVM);
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