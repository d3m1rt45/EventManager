using KonneyTM.DAL;
using KonneyTM.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace KonneyTM.Models
{
    public class Event
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Title { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh-mm}", ApplyFormatInEditMode = true)]
        public DateTime Time { get; set; }

        [DataType(DataType.ImageUrl)]
        public string ImagePath { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<Person> PeopleAttending { get; set; }

        public virtual Venue Place { get; set; }


        //Converts this Event object to an EventViewModel object
        public EventViewModel ToEventViewModel(KonneyContext db)
        {
            var eventVM = new EventViewModel(db, this.User.ID)
            {
                ID = this.ID,
                UserID = this.User.ID,
                Title = this.Title,
                Place = VenueViewModel.FromVenue(this.Place),
                PlaceID = this.Place.ID,
                Date = this.Date,
                Time = this.Time,
                PeopleAttending = Person.ToViewModelList(this.PeopleAttending),
                InvitedPeopleIDs = Person.GetIDsFromPersonList(this.PeopleAttending),
                ImagePath = this.ImagePath
            };

            return eventVM;
        }

        //Returns all the Events in the Database as a List of EventViewModels ordered by date.
        public static List<EventViewModel> GetAllAsViewModelList(KonneyContext db, string userID)
        {
            var events = db.Events.Where(x => x.User.ID == userID).ToList();

            var eventsVM = new List<EventViewModel>();
            foreach (var ev in events)
            {
                var eventVM = new EventViewModel(db, userID)
                {
                    ID = ev.ID,
                    UserID = ev.User.ID,
                    Title = ev.Title,
                    Place = VenueViewModel.FromVenue(ev.Place),
                    PlaceID = ev.Place.ID,
                    Date = ev.Date,
                    Time = ev.Time,
                    PeopleAttending = Person.ToViewModelList(ev.PeopleAttending),
                    InvitedPeopleIDs = Person.GetIDsFromPersonList(ev.PeopleAttending),
                    ImagePath = ev.ImagePath
                };

                eventsVM.Add(eventVM);
            }

            return eventsVM.OrderBy(ev => ev.Date).ToList();
        }

        // Saves a new Event to the database based on an EventViewModel object
        public static void NewByViewModel(KonneyContext db, EventViewModel eventVM)
        {
            var newEvent = new Event
            {
                Title = eventVM.Title,
                Date = Convert.ToDateTime(eventVM.Date),
                Time = Convert.ToDateTime(eventVM.Time),
                Place = db.Venues.First(v => v.ID == eventVM.PlaceID),
                PeopleAttending = new List<Person>(),
                ImagePath = eventVM.ImagePath
            };

            foreach (var id in eventVM.InvitedPeopleIDs)
            {
                newEvent.PeopleAttending.Add(db.People.First(p => p.ID == id));
            }

            var user = db.Users.Single(x => x.ID == eventVM.UserID);
            user.Events.Add(newEvent);
            db.SaveChanges();
        }

        // Uses an EventViewModel object's fields to update the corresponding Event in the database
        public static void SubmitChangesByViewModel(KonneyContext db, EventViewModel eventVM)
        {
            var subjectEvent = db.Events.First(p => p.ID == eventVM.ID);

            subjectEvent.Title = eventVM.Title;
            subjectEvent.Date = Convert.ToDateTime(eventVM.Date);
            subjectEvent.Time = Convert.ToDateTime(eventVM.Time);
            subjectEvent.Place = db.Venues.First(v => v.ID == eventVM.PlaceID);
            subjectEvent.ImagePath = eventVM.ImagePath;

            subjectEvent.PeopleAttending.Clear();
            foreach (var id in eventVM.InvitedPeopleIDs)
            {
                subjectEvent.PeopleAttending.Add(db.People.First(p => p.ID == id));
            }

            db.SaveChanges();
        }
    }
}