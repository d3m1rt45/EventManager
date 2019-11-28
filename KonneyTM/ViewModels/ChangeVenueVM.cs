using KonneyTM.DAL;
using KonneyTM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KonneyTM.ViewModels
{
    public class ChangeVenueVM
    {
        public ChangeVenueVM(KonneyContext db, string userID) { Venues = Venue.GetAllAsViewModelList(db, userID); }
        public int EventID { get; set; }
        public List<VenueViewModel> Venues { get; set; }
    }
}