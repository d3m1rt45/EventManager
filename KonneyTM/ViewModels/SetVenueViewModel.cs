using KonneyTM.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KonneyTM.Models
{
    public class SetVenueViewModel
    {
        public int SelectedVenueID { get; set; }

        public List<Venue> VenueList { get; set; }

        public SetVenueViewModel()
        {
            var db = new KonneyContext();
            this.VenueList = db.Venues.ToList();
        }
    }
}