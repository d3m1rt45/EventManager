using KonneyTM.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

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
        public List<PersonViewModel> People { get; set; } //If we populate this directly, it takes all we pass in.

        public List<PersonViewModel> PeopleAttending //If we populate this, it will indirectly populate People property with only those checked.
        { 
            get
            {
                return People;
            }
            set
            {
                foreach (var p in value)
                {
                    if (p.Attending)
                    {
                        People.Add(p);
                    }
                }
            }
        }


        [Required]
        public VenueViewModel Place { get; set; }

        public List<VenueViewModel> Venues { get; set; }
    }
}