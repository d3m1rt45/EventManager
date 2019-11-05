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
            return View(db.Events);
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

                return RedirectToAction("Events");
            }
            else
            {
                return View(newEventVM);
            }
        }

        public ActionResult People()
        {
            return View(db.People.OrderBy(p => p.FirstName));
        }

        public ActionResult EditPerson(int id)
        {
            var person = db.People.First(p => p.ID == id);
            return View(person);
        }

        public ActionResult Venues()
        {
            return View();
        }
    }
}