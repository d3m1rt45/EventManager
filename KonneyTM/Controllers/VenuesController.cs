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
    [HandleError]
    public class VenuesController : Controller
    {
        public ActionResult Index()
        {
            if(User.Identity.IsAuthenticated)
            {
                var userID = User.Identity.GetUserId();
                return View(VenueViewModel.GetAll(userID));
            }
            else
            {
                return View(VenueViewModel.GetAll("demo"));
            }
        }

        public ActionResult Create()
        {
            var venueVM = new VenueViewModel();

            return View(venueVM);
        }

        [HttpPost]
        public ActionResult Create(VenueViewModel venueVM)
        {
            if (ModelState.IsValid)
            {
                //Image Upload Logic
                if (User.Identity.IsAuthenticated)
                {
                    var userID = User.Identity.GetUserId();
                    UploadImage(venueVM, userID);
                    venueVM.SaveToDB(userID);
                }
                else
                {
                    UploadImage(venueVM, "demo");
                    venueVM.SaveToDB("demo");
                }

                
                return RedirectToAction("Index");
            }

            return View(venueVM);
        }

        public ActionResult Edit(int id)
        {
            using (var db = new KonneyContext())
            {
                var venue = db.Venues.First(e => e.ID == id);
                var venueVM = VenueViewModel.FromVenue(venue);

                if (User.Identity.IsAuthenticated)
                {
                    string userID = User.Identity.GetUserId();

                    if (venueVM.UserID == userID)
                    {
                        return View(venueVM);
                    }
                    else
                    {
                        throw new AuthenticationException("You are not authorized to edit this event.");
                    }
                }
                else if (venueVM.UserID == "demo")
                {
                    return View(venueVM);
                }
                else
                {
                    throw new Exception("Something went wrong...");
                }
            }
        }

        [HttpPost]
        public ActionResult Edit(VenueViewModel venueVM)
        {
            if (ModelState.IsValid)
            {
                using (var context = new KonneyContext())
                {
                    //If the user is logged in
                    if (User.Identity.IsAuthenticated)
                    {
                        var userID = User.Identity.GetUserId();

                        //If the venue belongs to the user
                        if (venueVM.UserID == userID)
                        {
                            //If an image file is selected
                            if (venueVM.ImageFile != null)
                            {
                                UploadImage(venueVM, userID);
                            }
                        }
                        else
                        {
                            throw new AuthenticationException("You are not authorized to edit this event.");
                        }
                    }
                    //If the application is in demo mode
                    else if (venueVM.UserID == "demo")
                    {
                        if (venueVM.ImageFile != null)
                        {
                            UploadImage(venueVM, "demo");
                        }
                    }
                    else
                    {
                        throw new Exception("Something went wrong...");
                    }

                    venueVM.Update();
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int venueID)
        {
            using (var db = new KonneyContext())
            {
                var venueVM = VenueViewModel.FromVenue(db.Venues.SingleOrDefault(v => v.ID == venueID));
                
                //If the user is logged in
                if (User.Identity.IsAuthenticated)
                {
                    var userID = User.Identity.GetUserId();

                    //If the venue DOESN'T belong to the user
                    if (venueVM.UserID != userID)
                    {
                        throw new AuthenticationException("You are not authorized to delete this event.");   
                    }
                }
                //If the venue DOESN'T belong to demo run
                else if (venueVM.UserID != "demo")
                {
                    throw new Exception("Something went wrong...");
                }
                venueVM.Delete();
                return RedirectToAction("Index");

            }
        }

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