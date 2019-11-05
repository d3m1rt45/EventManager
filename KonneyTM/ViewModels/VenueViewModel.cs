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

        public void SaveAsVenue()
        {
            var db = new KonneyContext();

            db.Venues.Add(new Venue
            {
                Name = this.Name,
                Address = this.Address,
                PostCode = this.PostCode,
                PhoneNumber = this.PhoneNumber
            });

            db.SaveChanges();

            db.Dispose();
        }
    }
}