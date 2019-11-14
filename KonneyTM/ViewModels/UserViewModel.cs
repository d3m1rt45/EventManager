using KonneyTM.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KonneyTM.ViewModels
{
    public class UserViewModel
    {
        public string ID { get; set; }
        public List<PersonViewModel> People { get; set; }
        public List<VenueViewModel> Venues { get; set; }
        public List<EventViewModel> Events { get; set; }

        public void Fill(string userID)
        {
            using (var db = new KonneyContext())
            {
                this.ID = userID;
                this.Events = EventViewModel.GetAll(userID);
                this.People = PersonViewModel.GetAll(userID);
                this.Venues = VenueViewModel.GetAll(userID);
            }
        }
    }
}