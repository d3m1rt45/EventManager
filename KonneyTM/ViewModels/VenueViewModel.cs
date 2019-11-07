using KonneyTM.DAL;
using KonneyTM.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KonneyTM.ViewModels
{
    public class VenueViewModel
    {
        public int ID { get; set; }

        [StringLength(30)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Phone number cannot be empty.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "Phone number must be 11 characters long. (Ex. 07956852154)")]
        [RegularExpression("^(?!0+$)(\\+\\d{1,3}[- ]?)?(?!0+$)\\d{10,15}$", ErrorMessage = "Please enter valid phone number. (Ex. 07956852154)")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Venue address cannot be empty.")]
        [StringLength(55, MinimumLength = 10)]
        public string Address { get; set; }

        [Required(ErrorMessage = "Postcode cannot be empty.")]
        [RegularExpression(@"^(?i)([A-PR-UWYZ](([0-9](([0-9]|[A-HJKSTUW])?)?)|([A-HK-Y][0-9]([0-9]|[ABEHMNPRVWXY])?)) [0-9][ABD-HJLNP-UW-Z]{2})|GIR 0AA$", 
            ErrorMessage = "Invalid Postcode")]
        public string PostCode { get; set; }

        [DataType(DataType.ImageUrl)]
        public string ImagePath { get; set; }
        
        public HttpPostedFileBase ImageFile { get; set; }

        public void SaveToDB()
        {
            var db = new KonneyContext();

            db.Venues.Add(new Venue
            {
                Name = this.Name,
                Address = this.Address,
                PostCode = this.PostCode,
                PhoneNumber = this.PhoneNumber,
                ImagePath = this.ImagePath
            });

            db.SaveChanges();
            db.Dispose();
        }

        public static VenueViewModel FromVenue(Venue venue)
        {
            var venueVM = new VenueViewModel
            {
                ID = venue.ID,
                Name = venue.Name,
                Address = venue.Address,
                PostCode = venue.PostCode,
                PhoneNumber = venue.PhoneNumber,
                ImagePath = venue.ImagePath
            };

            return venueVM;
        }

        public Venue ToVenue()
        {
            var venue = new Venue
            {
                Name = this.Name,
                Address = this.Address,
                PostCode = this.PostCode,
                PhoneNumber = this.PhoneNumber,
                ImagePath = this.ImagePath
            };

            return venue;
        }

        public static List<VenueViewModel> GetAllAsOrderedList()
        {
            var db = new KonneyContext();
            var venues = db.Venues;
            
            var venuesVM = new List<VenueViewModel>();
            foreach (var v in venues)
            {
                venuesVM.Add(new VenueViewModel
                {
                    ID = v.ID,
                    Name = v.Name,
                    Address = v.Address,
                    PostCode = v.PostCode,
                    PhoneNumber = v.PhoneNumber,
                    ImagePath = v.ImagePath
                });
            }

            db.Dispose();
            return venuesVM.OrderBy(v => v.Name).ToList();
        }

        internal void SubmitChanges()
        {
            var db = new KonneyContext();

            var venue = db.Venues.First(p => p.ID == this.ID);

            venue.Name = this.Name;
            venue.PhoneNumber = this.PhoneNumber;
            venue.Address = this.Address;
            venue.PostCode = this.PostCode;

            db.SaveChanges();
            db.Dispose();
        }
    }
}