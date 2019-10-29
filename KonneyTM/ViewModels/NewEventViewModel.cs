using KonneyTM.DAL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace KonneyTM.Models
{
    public class NewEventViewModel
    {
        public string Title { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public virtual List<Person> PeopleAttending { get; set; }
        public virtual Venue Place { get; set; }

        public List<PersonViewModel> People { get; set; }
        public List<VenueViewModel> Venues { get; set; }
    }
}