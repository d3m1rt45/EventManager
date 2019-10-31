using KonneyTM.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KonneyTM.Models
{
    public class NewEventViewModel
    {
        [Required]
        [StringLength(30)]
        public string Title { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh-mm}", ApplyFormatInEditMode = true)]
        public DateTime Time { get; set; }

        [Required]
        public int SelectedVenueID { get; set; }

        [Required]
        public List<int> InvitedPeopleIDs { get; set; }

        public List<Venue> VenueList { get; set; }
        public List<SelectListItem> PeopleList { get; set; }

        public NewEventViewModel()
        {
            var db = new KonneyContext();
            
            this.VenueList = db.Venues.ToList();
            this.PeopleList = new List<SelectListItem>();
            this.InvitedPeopleIDs = new List<int>();

            foreach (var p in db.People)
            {
                this.PeopleList.Add(new SelectListItem { Text = $"{p.FirstName} {p.LastName}", Value = p.ID.ToString() });
            }
        }

    }
}