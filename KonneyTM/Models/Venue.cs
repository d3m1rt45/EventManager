using KonneyTM.DAL;
using KonneyTM.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace KonneyTM.Models
{
    public class Venue
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string PostCode { get; set; }

        [DataType(DataType.ImageUrl)]
        public string ImagePath { get; set; }

        public virtual User User { get; set; }

        public virtual ICollection<Event> Events { get; set; }


        /*METHODS*/

        // Convert this Venue object to a VenueViewModel object
        public VenueViewModel ToViewModel()
        {
            var venueVM = new VenueViewModel
            {
                ID = this.ID,
                UserID = this.User.ID,
                Name = this.Name,
                Address = this.Address,
                PostCode = this.PostCode,
                PhoneNumber = this.PhoneNumber,
                ImagePath = this.ImagePath
            };

            return venueVM;
        }

        // Return all the Venues in the database that belongs to the user whose ID is passed as VenuViewModel List
        public static List<VenueViewModel> GetAllAsViewModelList(KonneyContext db, string userID)
        {
            var venues = db.Venues.Where(v => v.User.ID == userID);

            var venuesVM = new List<VenueViewModel>();
            foreach (var v in venues)
            {
                venuesVM.Add(new VenueViewModel
                {
                    ID = v.ID,
                    UserID = v.User.ID,
                    Name = v.Name,
                    Address = v.Address,
                    PostCode = v.PostCode,
                    PhoneNumber = v.PhoneNumber,
                    ImagePath = v.ImagePath
                });
            }

            return venuesVM.OrderBy(v => v.Name).ToList();
        }

        // Saves a new Person to the database based on an PersonViewModel object
        public static void NewByViewModel(KonneyContext db, VenueViewModel venueVM)
        {
            var user = db.Users.Find(venueVM.UserID);
            var venue = new Venue
            {
                Name = venueVM.Name,
                Address = venueVM.Address,
                PostCode = venueVM.PostCode,
                PhoneNumber = venueVM.PhoneNumber,
                ImagePath = venueVM.ImagePath
            };

            user.Venues.Add(venue);
            db.SaveChanges();
        }

        // Deletes the venue from the database that corresponds to the VenueViewModel object passed in
        public static void DeleteByViewModel(KonneyContext db, VenueViewModel venueVM)
        {
            var venue = db.Venues.FirstOrDefault(v => v.ID == venueVM.ID);
            var events = db.Events.Where(e => e.Place.ID == venueVM.ID).ToList();

            events.ForEach(e => db.Events.Remove(e));
            db.Venues.Remove(venue);
            db.SaveChanges();
        }

        //Updates the Venue entity in the database that corresponds to this VenueViewModel object
        public static void UpdateByViewModel(KonneyContext db, VenueViewModel venueVM)
        {
            var venue = db.Venues.Find(venueVM.ID);

            venue.Name = venueVM.Name;
            venue.PhoneNumber = venueVM.PhoneNumber;
            venue.Address = venueVM.Address;
            venue.PostCode = venueVM.PostCode;

            if (venueVM.ImagePath != null)
                venue.ImagePath = venueVM.ImagePath;

            db.SaveChanges();
        }

    }
}