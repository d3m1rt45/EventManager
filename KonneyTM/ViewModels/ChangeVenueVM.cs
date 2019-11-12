using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KonneyTM.ViewModels
{
    public class ChangeVenueVM
    {
        public ChangeVenueVM(string userID)
        {
            this.Venues = VenueViewModel.GetAll(userID);
        }

        public int EventID { get; set; }

        public List<VenueViewModel> Venues { get; set; }
    }
}