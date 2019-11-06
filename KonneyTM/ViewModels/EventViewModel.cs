using KonneyTM.DAL;
using KonneyTM.DAL.CustomDataAnnotations;
using KonneyTM.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KonneyTM.ViewModels
{
    public class EventViewModel
    {
        public EventViewModel()
        {
            var db = new KonneyContext();
            
            this.VenueList = db.Venues.ToList();
            this.PeopleList = new List<SelectListItem>();
            this.PeopleAttending = new List<PersonViewModel>();

            foreach (var p in db.People)
            {
                this.PeopleList.Add(new SelectListItem { Text = $"{p.FirstName} {p.LastName}", Value = p.ID.ToString() });
            }
            db.Dispose();
        }

        public int ID { get; set; }

        [Required]
        [StringLength(30)]
        public string Title { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [CurrentDate(ErrorMessage = "Date must be between tomorrow and 3 years from now.")]
        public DateTime Date { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh-mm}", ApplyFormatInEditMode = true)]
        public DateTime Time { get; set; }

        public VenueViewModel Place { get; set; }

        public List<PersonViewModel> PeopleAttending { get; set; }


        //IDs for populating the above lists
        [Required]
        public int PlaceID { get; set; }
        [Required]
        public List<int> InvitedPeopleIDs { get; set; }


        //CHECKBOX and RADIOBUTTON Properties
        public List<Venue> VenueList { get; set; }
        public List<SelectListItem> PeopleList { get; set; }

        //Converts the ViewModel instance to an Event and saves it to the database.
        public void SaveToDB() 
        {
            var db = new KonneyContext();

            var newEvent = new Event
            {
                Title = this.Title,
                Date = Convert.ToDateTime(this.Date),
                Time = Convert.ToDateTime(this.Time),
                Place = db.Venues.First(v => v.ID == this.PlaceID),
                PeopleAttending = new List<Person>()
            };

            foreach(var id in this.InvitedPeopleIDs)
            {
                newEvent.PeopleAttending.Add(db.People.First(p => p.ID == id));
            }

            db.Events.Add(newEvent);
            db.SaveChanges();
            db.Dispose();
        }

        //Returns all the Events in the Database as a List of EventViewModels ordered by date.
        public static List<EventViewModel> GetAllAsOrderedList()
        {
            var db = new KonneyContext();
            var events = db.Events.ToList();

            var eventsVM = new List<EventViewModel>();
            foreach (var ev in events)
            {
                var eventVM = new EventViewModel
                {
                    ID = ev.ID,
                    Title = ev.Title,
                    Date = ev.Date,
                    Time = ev.Time,
                    Place = VenueViewModel.FromVenue(ev.Place)
                };
                foreach (var p in ev.PeopleAttending)
                {
                    eventVM.PeopleAttending.Add(PersonViewModel.FromPerson(p));
                }
                eventVM.PeopleList = new List<SelectListItem>();
                eventVM.PeopleAttending = new List<PersonViewModel>();

                foreach (var p in db.People)
                {
                    eventVM.PeopleList.Add(new SelectListItem { Text = $"{p.FirstName} {p.LastName}", Value = p.ID.ToString() });
                }

                eventsVM.Add(eventVM);
            }

            db.Dispose();
            return eventsVM.OrderBy(ev => ev.Date).ToList();
        }
    }
}