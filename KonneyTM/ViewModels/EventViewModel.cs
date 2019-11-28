using KonneyTM.DAL;
using KonneyTM.DAL.CustomDataAnnotations;
using KonneyTM.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KonneyTM.ViewModels
{
    public class EventViewModel
    {
        public EventViewModel()
        {
            using(var db = new KonneyContext())
            {
                VenueList = db.Venues.Where(v=>v.User.ID == UserID).ToList();
                PeopleList = new List<SelectListItem>();
                PeopleAttending = new List<PersonViewModel>();

                foreach (var p in db.People.Where(x => x.User.ID == this.UserID ))
                {
                    this.PeopleList.Add(new SelectListItem { Text = $"{p.FirstName} {p.LastName}", Value = p.ID.ToString() });
                }
            }
        }

        public EventViewModel(KonneyContext db, string userID)
        {
            this.UserID = userID;
            this.VenueList = db.Venues.Where(v => v.User.ID == userID).ToList();
            this.PeopleList = new List<SelectListItem>();
            this.PeopleAttending = new List<PersonViewModel>();

            foreach (var p in db.People.Where(p => p.User.ID == userID))
            {
                this.PeopleList.Add(new SelectListItem { Text = $"{p.FirstName} {p.LastName}", Value = p.ID.ToString() });
            }
        }

        public int ID { get; set; }

        public string UserID { get; set; }

        [Required]
        [StringLength(30)]
        public string Title { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [CurrentDate(ErrorMessage = "Date must be between tomorrow and 3 years from now.")]
        public DateTime Date { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh-mm}", ApplyFormatInEditMode = true)]
        public DateTime Time { get; set; }

        [Required]
        public int PlaceID { get; set; }
        public VenueViewModel Place { get; set; }
        public List<Venue> VenueList { get; set; }

        [Required]
        public List<int> InvitedPeopleIDs { get; set; }
        public List<PersonViewModel> PeopleAttending { get; set; }
        public List<SelectListItem> PeopleList { get; set; }

        [DataType(DataType.ImageUrl)]
        public string ImagePath { get; set; }
        public HttpPostedFileBase ImageFile { get; set; }
    }
}