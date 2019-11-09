using KonneyTM.DAL;
using KonneyTM.ExtensionMethods;
using KonneyTM.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KonneyTM.Controllers
{
    public class VenuesController : Controller
    {
        public ActionResult Index()
        {
            return View(VenueViewModel.GetAllAsOrderedList());
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
                string extension = Path.GetExtension(venueVM.ImageFile.FileName);
                string imageFileName = venueVM.PhoneNumber.RemoveWhitespace() + venueVM.PostCode.RemoveWhitespace() + extension;
                venueVM.ImagePath = imageFileName;
                imageFileName = Path.Combine(Server.MapPath("~/Images/Venues"), imageFileName);
                venueVM.ImageFile.SaveAs(imageFileName);

                venueVM.SaveToDB();
                return RedirectToAction("Index");
            }

            return View(venueVM);
        }

        public ActionResult Edit(int id)
        {
            using (var db = new KonneyContext())
            {
                return View(VenueViewModel.FromVenue(db.Venues.First(p => p.ID == id)));
            }
        }

        [HttpPost]
        public ActionResult Edit(VenueViewModel venueVM)
        {
            if (ModelState.IsValid)
            {
                if (venueVM.ImageFile != null)
                {
                    //Image Upload Logic
                    string extension = Path.GetExtension(venueVM.ImageFile.FileName);
                    string imageFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
                    venueVM.ImagePath = imageFileName;
                    imageFileName = Path.Combine(Server.MapPath("~/Images/Venues"), imageFileName);
                    venueVM.ImageFile.SaveAs(imageFileName);
                }

                venueVM.SubmitChanges();
                return RedirectToAction("Index");
            }
            return View(venueVM);
        }

        public ActionResult Delete(int id)
        {
            using (var db = new KonneyContext())
            {
                //Remove the related events to make the deletion of Venue possible.
                var relatedEvents = db.Events.Where(p => p.Place.ID == id).ToList();
                foreach (var ev in relatedEvents)
                {
                    db.Events.Remove(ev);
                }

                var venue = db.Venues.First(v => v.ID == id);
                db.Venues.Remove(venue);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}