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
            using (var db = new KonneyContext())
            { 
                this.VenueList = db.Venues.ToList();
                this.PeopleList = new List<SelectListItem>();
                this.PeopleAttending = new List<PersonViewModel>();

                foreach (var p in db.People)
                {
                    this.PeopleList.Add(new SelectListItem { Text = $"{p.FirstName} {p.LastName}", Value = p.ID.ToString() });
                }
            }
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

        [Required]
        public int PlaceID { get; set; }
        public VenueViewModel Place { get; set; }
        public List<Venue> VenueList { get; set; }

        [Required]
        public List<int> InvitedPeopleIDs { get; set; }
        public List<PersonViewModel> PeopleAttending { get; set; }
        public List<SelectListItem> PeopleList { get; set; }

        [DataType(DataType.ImageUrl)]
        public string ImagePath { get; set; }
        public HttpPostedFileBase ImageFile { get; set; }



        //Converts an Event object to an EventViewModel object
        public static EventViewModel FromEvent(Event ev)
        {
            var eventVM = new EventViewModel
            {
                ID = ev.ID,
                Title = ev.Title,
                Place = VenueViewModel.FromVenue(ev.Place),
                PlaceID = ev.Place.ID,
                Date = ev.Date,
                Time = ev.Time,
                PeopleAttending = PersonViewModel.FromPersonList(ev.PeopleAttending),
                InvitedPeopleIDs = GetIDsFromPersonList(ev.PeopleAttending),
                ImagePath = ev.ImagePath
            };

            return eventVM;
        }

        //Creates a list of int based on the ID properties of a List of Person
        public static List<int> GetIDsFromPersonList(ICollection<Person> people)
        {
            var ids = new List<int>();

            foreach (var p in people)
            {
                ids.Add(p.ID);
            }

            return ids;
        }

        //Returns all the Events in the Database as a List of EventViewModels ordered by date.
        public static List<EventViewModel> GetAll(string userID)
        {
            var db = new KonneyContext();
            var events = db.Events.Where(x => x.User.ID == userID).ToList();

            var eventsVM = new List<EventViewModel>();
            foreach (var ev in events)
            {
                var eventVM = new EventViewModel
                {
                    ID = ev.ID,
                    Title = ev.Title,
                    Place = VenueViewModel.FromVenue(ev.Place),
                    PlaceID = ev.Place.ID,
                    Date = ev.Date,
                    Time = ev.Time,
                    PeopleAttending = PersonViewModel.FromPersonList(ev.PeopleAttending),
                    InvitedPeopleIDs = GetIDsFromPersonList(ev.PeopleAttending),
                    ImagePath = ev.ImagePath
                };

                eventsVM.Add(eventVM);
            }

            db.Dispose();
            return eventsVM.OrderBy(ev => ev.Date).ToList();
        }

        //Saves this EventViewModel object as an Event entity to the database.
        public void SaveToDB(string userID)
        {
            using (var db = new KonneyContext())
            {
                var newEvent = new Event
                {
                    Title = this.Title,
                    Date = Convert.ToDateTime(this.Date),
                    Time = Convert.ToDateTime(this.Time),
                    Place = db.Venues.First(v => v.ID == this.PlaceID),
                    PeopleAttending = new List<Person>(),
                    ImagePath = this.ImagePath
                };

                foreach(var id in this.InvitedPeopleIDs)
                {
                    newEvent.PeopleAttending.Add(db.People.First(p => p.ID == id));
                }

                db.Users.Single(x => x.ID == userID).Events.Add(newEvent);
                db.SaveChanges();
            }
        }

        //Updates the Event in the database that corresponds to this EventViewModel object
        public void SubmitChanges()
        {
            using (var db = new KonneyContext())
            {
                var ev = db.Events.First(p => p.ID == this.ID);

                ev.Title = this.Title;
                ev.Date = Convert.ToDateTime(this.Date);
                ev.Time = Convert.ToDateTime(this.Time);
                ev.Place = db.Venues.First(v => v.ID == this.PlaceID);
                ev.ImagePath = this.ImagePath;

                ev.PeopleAttending.Clear();
                foreach (var id in this.InvitedPeopleIDs)
                {
                    ev.PeopleAttending.Add(db.People.First(p => p.ID == id));
                }

                db.SaveChanges();
            }
        }
    }
}