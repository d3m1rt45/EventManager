using KonneyTM.DAL;
using KonneyTM.ExtensionMethods;
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
    // <summary>
    // This class deals with all functionality in the application that has to
    // do with Venues. I have used a similar method of checking whether an item in
    // question belongs to the user trying to make the change, all throughout the app.
    // Because of this duplication I have later optimized to make the code as concise
    // as possible. The result was something a little harder to understand.
    // You'll find that they're all structured the same way though:
    //
    // IF user is logged in AND if the userID doesn't match the entity's User ID, THROW an exception.
    // ELSE IF the entity's user ID is not "demo" THROW an exception.
    // </summary>

    [HandleError]
    public class VenuesController : Controller
    {
        // Entity Framework Database Context
        readonly KonneyContext db = new KonneyContext();

        // Return people that belongs to the user
        public ActionResult Index()
        {
            if(User.Identity.IsAuthenticated)
                return View(Venue.GetAllAsViewModelList(db, User.Identity.GetUserId()));
            else
                return View(Venue.GetAllAsViewModelList(db, "demo"));
        }

        // Navigate to Create Venue page
        public ActionResult Create()
        {
            string userID = "demo";

            if (User.Identity.IsAuthenticated)
                userID = User.Identity.GetUserId();

            return View(new VenueViewModel { UserID = userID });
        }

        // Submit the new Venue to the user's venues table
        [HttpPost]
        public ActionResult Create(VenueViewModel venueVM)
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                    UploadImage(venueVM, User.Identity.GetUserId());
                else
                    UploadImage(venueVM, "demo");

                Venue.NewByViewModel(db, venueVM);
                return RedirectToAction("Index");
            }
            return View(venueVM);
        }

        // Navigate to Edit Venue page
        public ActionResult Edit(int venueID)
        {
            var venueVM = db.Venues.Find(venueID).ToViewModel();

            if (User.Identity.IsAuthenticated && venueVM.UserID != User.Identity.GetUserId())
                throw new AuthenticationException("You are not authorized to edit this event.");
            else if (venueVM.UserID != "demo")
                throw new Exception("Something went wrong...");
            if (venueVM.ID <= 3)
                return RedirectToAction("Index");

            return View(venueVM);
        }

        // Submit changes for Venue
        [HttpPost]
        public ActionResult Edit(VenueViewModel venueVM)
        {
            if (ModelState.IsValid)
            {
                var userID = "demo";

                if (User.Identity.IsAuthenticated)
                {
                    userID = User.Identity.GetUserId();
                    if (venueVM.UserID != userID)
                        throw new AuthenticationException("You are not authorized to edit this event.");
                }
                else if (venueVM.UserID != "demo")
                    throw new Exception("Something went wrong...");
                if (venueVM.ID <= 3)
                    return RedirectToAction("Index");

                if (venueVM.ImageFile != null)
                    UploadImage(venueVM, userID);

                Venue.UpdateByViewModel(db, venueVM);
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        // Delete venue from the User's Venues list
        public ActionResult Delete(int venueID)
        {
            using (var db = new KonneyContext())
            {
                var venueVM = db.Venues.Find(venueID).ToViewModel();

                if (User.Identity.IsAuthenticated && venueVM.UserID != User.Identity.GetUserId())
                    throw new AuthenticationException("You are not authorized to delete this event.");
                else if (venueVM.UserID != "demo")
                    throw new Exception("Something went wrong...");
                if (venueVM.ID <= 3)
                    return RedirectToAction("Index");

                Venue.DeleteByViewModel(db, venueVM);
                return RedirectToAction("Index");
            }
        }

        // Upload image for venue
        public void UploadImage(VenueViewModel venueVM, string userID)
        {
            string extension = Path.GetExtension(venueVM.ImageFile.FileName);
            string imageFileName = $"{userID}{DateTime.Now.ToString("yyyyMMddHHmmss")}{extension}";
            venueVM.ImagePath = imageFileName;
            imageFileName = Path.Combine(Server.MapPath("~/Images/Venues/") + imageFileName);
            venueVM.ImageFile.SaveAs(imageFileName);
        }
    }
}