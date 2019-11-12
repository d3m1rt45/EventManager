using KonneyTM.DAL;
using KonneyTM.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Web;
using System.Web.Mvc;

namespace KonneyTM.Controllers
{
    public class PeopleController : Controller
    {
        public ActionResult Index()
        {
            if(User.Identity.IsAuthenticated)
            {
                var userID = User.Identity.GetUserId();
                return View(PersonViewModel.GetAll(userID));
            }
            else
            {
                return View(PersonViewModel.GetAll("demo"));
            }
        }

        public ActionResult Create()
        {
            var person = new PersonViewModel();
            return View(person);
        }

        [HttpPost]
        public ActionResult Create(PersonViewModel personVM)
        {
            if (ModelState.IsValid)
            {
                if(User.Identity.IsAuthenticated)
                {
                    var userID = User.Identity.GetUserId();
                    personVM.SaveToDB(userID);
                }
                else
                {
                    personVM.SaveToDB("demo");
                }

                return RedirectToAction("Index");
            }
            return View(personVM);
        }

        public ActionResult Edit(int id)
        {
            using (var db = new KonneyContext())
            {
                var personVM = PersonViewModel.FromPerson(db.People.First(p => p.ID == id));

                if (User.Identity.IsAuthenticated)
                {
                    var userID = User.Identity.GetUserId();
                    if (personVM.UserID == userID)
                    {
                        return View(personVM);
                    }
                    else
                    {
                        throw new AuthenticationException("You are not authorized to edit this person.");
                    }
                }
                else if(personVM.UserID == "demo")
                {
                    return View(personVM);
                }
                else
                {
                    throw new Exception("Something went wrong...");
                }
            }
        }

        [HttpPost]
        public ActionResult Edit(PersonViewModel personVM)
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    var userID = User.Identity.GetUserId();
                    if (personVM.UserID == userID)
                    {
                        personVM.SubmitChanges(userID);
                        
                    }
                    else
                    {
                        throw new AuthenticationException("You are not authorized to edit this person.");
                    }
                }
                else if (personVM.UserID == "demo")
                {
                    personVM.SubmitChanges("demo");
                }
                else
                {
                    throw new Exception("Something went wrong...");
                }

                return RedirectToAction("Index");
            }
            return View(personVM);
        }

        public ActionResult Delete(int id)
        {
            using (var db = new KonneyContext())
            {
                var person = db.People.First(p => p.ID == id);

                if (User.Identity.IsAuthenticated)
                {
                    var userID = User.Identity.GetUserId();
                    if (person.User.ID == userID)
                    {
                        db.People.Remove(person);

                    }
                    else
                    {
                        throw new AuthenticationException("You are not authorized to delete this person.");
                    }
                }
                else if (person.User.ID == "demo")
                {
                    db.People.Remove(person);
                    
                }
                else
                {
                    throw new Exception("Something went wrong...");
                }

                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}