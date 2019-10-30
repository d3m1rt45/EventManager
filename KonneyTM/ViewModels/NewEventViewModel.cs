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
        public string Date { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh-mm}", ApplyFormatInEditMode = true)]
        public string Time { get; set; }

        [Required]
        public virtual List<PersonViewModel> PeopleAttending { get; set; }

        [Required]
        public virtual VenueViewModel Place { get; set; }

        public List<PersonViewModel> People { get; set; }
        public List<VenueViewModel> Venues { get; set; }
    }
}