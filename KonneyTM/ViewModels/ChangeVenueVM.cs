using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KonneyTM.ViewModels
{
    public class ChangeVenueVM
    {
        public int EventID { get; set; }

        public List<VenueViewModel> Venues = VenueViewModel.GetAllAsOrderedList();
    }
}