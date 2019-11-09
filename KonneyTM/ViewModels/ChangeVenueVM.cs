using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KonneyTM.ViewModels
{
    public class ChangeVenueVM
    {
        public ChangeVenueVM()
        {
            this.Venues = VenueViewModel.GetAllAsOrderedList();
        }

        public int EventID { get; set; }

        public List<VenueViewModel> Venues { get; set; }
    }
}