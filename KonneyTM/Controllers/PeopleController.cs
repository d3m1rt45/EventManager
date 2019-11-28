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
    // <summary>
    // This class deals with all functionality in the application that has to
    // do with people. I have used a similar method of checking whether an item in
    // question belongs to the user trying to make the change, all throughout the app.
    // </summary>

    [HandleError]
    public class PeopleController : Controller
    {
        // Entity Framework Database Context
        readonly KonneyContext db = new KonneyContext();

        // Return people belonging to the user
        public ActionResult Index()
        {
            if(User.Identity.IsAuthenticated)
                return View(PersonViewModel.GetAll(db, User.Identity.GetUserId()));
            else
                return View(PersonViewModel.GetAll(db, "demo"));
        }

        // Navigate to Create Person page
        public ActionResult Create()
        {
            return View(new PersonViewModel());
        }

        // Submit the new Person to the user's people table
        [HttpPost]
        public ActionResult Create(PersonViewModel personVM)
        {
            if (ModelState.IsValid)
            {
                if(User.Identity.IsAuthenticated)
                    personVM.SaveToDB(db, User.Identity.GetUserId());
                else
                    personVM.SaveToDB(db, "demo");

                return RedirectToAction("Index");
            }
            return View(personVM);
        }

        // Navigate to Edit Person page
        public ActionResult Edit(int personID)
        {
            var personVM = PersonViewModel.FromPerson(db.People.First(p => p.ID == personID));

            if (User.Identity.IsAuthenticated)
            {
                if (personVM.UserID != User.Identity.GetUserId())
                    throw new AuthenticationException("You are not authorized to edit this person.");
            }
            else if(personVM.UserID != "demo")
                throw new Exception("Something went wrong...");

            return View(personVM);
        }

        // Submit changes for Person
        [HttpPost]
        public ActionResult Edit(PersonViewModel personVM)
        {
            if (ModelState.IsValid)
            {
                string userID = "demo";

                if (User.Identity.IsAuthenticated)
                {
                    userID = User.Identity.GetUserId();
                    if (personVM.UserID != userID)
                        throw new AuthenticationException("You are not authorized to edit this person.");
                }
                else if (personVM.UserID != "demo")
                    throw new Exception("Something went wrong...");

                personVM.SubmitChanges(db, userID);
                return RedirectToAction("Index");
            }
            return View(personVM);
        }

        // Delete a person from the User's people list
        public ActionResult Delete(int id)
        {
            var person = db.People.First(p => p.ID == id);

            if (User.Identity.IsAuthenticated)
            {
                if (person.User.ID != User.Identity.GetUserId())
                    throw new AuthenticationException("You are not authorized to delete this person.");
            }
            else if (person.User.ID != "demo")
                throw new Exception("Something went wrong...");

            db.People.Remove(person);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}