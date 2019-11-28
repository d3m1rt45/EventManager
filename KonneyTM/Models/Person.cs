using KonneyTM.DAL;
using KonneyTM.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Authentication;
using System.Web;

namespace KonneyTM.Models
{
    public class Person
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Display(Name="First Name")]
        public string FirstName { get; set; }

        [Display(Name="Surname")]
        public string LastName { get; set; }

        [Display(Name="Phone Number")]
        public string PhoneNumber { get; set; }

        [Display(Name="E-mail")]
        public string Email { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<Event> Events { get; set; }

        public static AddPersonVM ReturnAddPersonVMIfIDsMatch(KonneyContext db, Event relatedEvent, string userID)
        {
            var addPerson = new AddPersonVM { EventID = relatedEvent.ID };
            var eventVM = relatedEvent.ToEventViewModel(db);
            List<PersonViewModel> allPeople;

            if (relatedEvent.User.ID == userID)
                allPeople = PersonViewModel.GetAll(db, userID);
            else
                throw new AuthenticationException("You are not authorized to add people to this event.");

            foreach (var p in allPeople)
            {
                if (!eventVM.InvitedPeopleIDs.Contains(p.ID))
                    addPerson.People.Add(p);
            }

            return addPerson;
        }


        //Creates a list of int based on the ID properties of a List of Person
        public static List<int> GetIDsFromPersonList(ICollection<Person> people)
        {
            var ids = new List<int>();

            foreach (var p in people)
                ids.Add(p.ID);

            return ids;
        }
    }
}