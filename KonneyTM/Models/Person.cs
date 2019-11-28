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


        /*METHODS*/

        // Return AddPersonViewModel based on the subject Event and User ID
        public static AddPersonVM ReturnAddPersonVMIfIDsMatch(KonneyContext db, Event relatedEvent, string userID)
        {
            var addPerson = new AddPersonVM { EventID = relatedEvent.ID };
            var eventVM = relatedEvent.ToEventViewModel(db);
            List<PersonViewModel> allPeople;

            if (relatedEvent.User.ID == userID)
                allPeople = Person.GetAllAsViewModelList(db, userID);
            else
                throw new AuthenticationException("You are not authorized to add people to this event.");

            foreach (var p in allPeople)
            {
                if (!eventVM.InvitedPeopleIDs.Contains(p.ID))
                    addPerson.People.Add(p);
            }

            return addPerson;
        }

        //Returns all the Persons in the Database as a List of PersonViewModel objects ordered by FirstName
        public static List<PersonViewModel> GetAllAsViewModelList(KonneyContext db, string userID)
        {
            var people = db.People.Where(p => p.User.ID == userID).ToList();
            var peopleVM = new List<PersonViewModel>();

            foreach (var p in people)
            {
                peopleVM.Add(new PersonViewModel
                {
                    ID = p.ID,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    PhoneNumber = p.PhoneNumber,
                    Email = p.Email
                });
            }

            return peopleVM.OrderBy(p => p.FirstName).ToList();
        }

        //Converts this Person object to a PersonViewModel object
        public PersonViewModel ToViewModel()
        {
            var personVM = new PersonViewModel
            {
                ID = this.ID,
                UserID = this.User.ID,
                FirstName = this.FirstName,
                LastName = this.LastName,
                PhoneNumber = this.PhoneNumber,
                Email = this.Email
            };

            return personVM;
        }

        //Creates a list of int based on the ID properties of a List of Person
        public static List<int> GetIDsFromPersonList(ICollection<Person> people)
        {
            var ids = new List<int>();

            foreach (var p in people)
                ids.Add(p.ID);

            return ids;
        }

        //Converts a Person object list to a PersonViewModel object list
        public static List<PersonViewModel> ToViewModelList(ICollection<Person> people)
        {
            var personVMList = new List<PersonViewModel>();

            foreach (var p in people)
            {
                personVMList.Add(new PersonViewModel
                {
                    ID = p.ID,
                    UserID = p.User.ID,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    Email = p.Email,
                    PhoneNumber = p.PhoneNumber
                });
            }

            return personVMList;
        }

        // Saves a new Person to the database based on an PersonViewModel object
        public static void NewByViewModel(KonneyContext db, PersonViewModel personVM)
        {
            var user = db.Users.Find(personVM.UserID);
            user.People.Add(new Person
            {
                FirstName = personVM.FirstName,
                LastName = personVM.LastName,
                PhoneNumber = personVM.PhoneNumber,
                Email = personVM.Email
            });

            db.SaveChanges();
        }

        //Updates the Person entity in the database that corresponds to this PersonViewModel object
        public static void UpdateByViewModel(KonneyContext db, PersonViewModel personVM)
        {
            var person = db.People.First(p => p.ID == personVM.ID);

            person.FirstName = personVM.FirstName;
            person.LastName = personVM.LastName;
            person.Email = personVM.Email;
            person.PhoneNumber = personVM.PhoneNumber;

            db.SaveChanges();
        }
    }
}