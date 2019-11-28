using KonneyTM.DAL;
using KonneyTM.ExtensionMethods;
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
    // do with venues. I have used a similar method of checking whether an item in
    // question belongs to the user trying to make the change, all throughout the app.
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
                return View(VenueViewModel.GetAll(db, User.Identity.GetUserId()));
            else
                return View(VenueViewModel.GetAll(db, "demo"));
        }

        // Navigate to Create Venue page
        public ActionResult Create()
        {
            return View(new VenueViewModel());
        }

        // Submit the new Venue to the user's venues table
        [HttpPost]
        public ActionResult Create(VenueViewModel venueVM)
        {
            if (ModelState.IsValid)
            {
                var userID = "demo";
                if (User.Identity.IsAuthenticated)
                    userID = User.Identity.GetUserId();

                UploadImage(venueVM, userID);
                venueVM.SaveToDB(db, userID);

                return RedirectToAction("Index");
            }
            return View(venueVM);
        }

        // Navigate to Edit Venue page
        public ActionResult Edit(int id)
        {
            var venue = db.Venues.First(e => e.ID == id);
            var venueVM = VenueViewModel.FromVenue(venue);

            if (User.Identity.IsAuthenticated && venueVM.UserID != User.Identity.GetUserId())
                throw new AuthenticationException("You are not authorized to edit this event.");
            else if (venueVM.UserID != "demo")
                throw new Exception("Something went wrong...");

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

                if (venueVM.ImageFile != null)
                    UploadImage(venueVM, userID);
                venueVM.Update(db);
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        // Delete venue from the User's Venues list
        public ActionResult Delete(int venueID)
        {
            using (var db = new KonneyContext())
            {
                var venueVM = VenueViewModel.FromVenue(db.Venues.SingleOrDefault(v => v.ID == venueID));

                if (User.Identity.IsAuthenticated)
                {
                    var userID = User.Identity.GetUserId();

                    if (venueVM.UserID != userID)
                        throw new AuthenticationException("You are not authorized to delete this event.");   
                }
                else if (venueVM.UserID != "demo")
                    throw new Exception("Something went wrong...");

                venueVM.DeleteFrom(db);
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