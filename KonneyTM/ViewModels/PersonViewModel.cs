using KonneyTM.DAL;
using KonneyTM.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Authentication;
using System.Web;

namespace KonneyTM.ViewModels
{
    public class PersonViewModel
    {   
        public int ID { get; set; }

        public string UserID { get; set; }

        [Required]
        [StringLength(20)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(20)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Phone number cannot be empty.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "Phone number must be 11 characters long. (Ex. 07956852154)")]
        [RegularExpression("^(?!0+$)(\\+\\d{1,3}[- ]?)?(?!0+$)\\d{10,15}$", ErrorMessage = "Please enter valid phone number. (Ex. 07956852154)")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Email address cannot be empty.")]
        [RegularExpression("^[a-z0-9_\\+-]+(\\.[a-z0-9_\\+-]+)*@[a-z0-9-]+(\\.[a-z0-9]+)*\\.([a-z]{2,4})$", ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }



        //Returns all the Persons in the Database as a List of PersonViewModel objects ordered by FirstName
        public static List<PersonViewModel> GetAll(string userID)
        {
            using (var db = new KonneyContext())
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
        }

        //Converts a Person object to a PersonViewModel object
        public static PersonViewModel FromPerson(Person person)
        {
            var personVM = new PersonViewModel
            {
                ID = person.ID,
                UserID = person.User.ID,
                FirstName = person.FirstName,
                LastName = person.LastName,
                PhoneNumber = person.PhoneNumber,
                Email = person.Email
            };
            return personVM;
        }

        //Converts a Person object list to a PersonViewModel object list
        public static List<PersonViewModel> FromPersonList(ICollection<Person> people)
        {
            var personVMList = new List<PersonViewModel>();

            foreach(var p in people)
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

        //Saves this PersonViewModel object to the database as a Person entity
        public void SaveToDB(string userID)
        {
            using (var db = new KonneyContext())
            {
                var user = db.Users.Single(u => u.ID == userID);
                user.People.Add(new Person
                {
                    FirstName = this.FirstName,
                    LastName = this.LastName,
                    PhoneNumber = this.PhoneNumber,
                    Email = this.Email
                });

                db.SaveChanges();
            }
        }

        //Updates the Person entity in the database that corresponds to this PersonViewModel object
        public void SubmitChanges(string userID)
        {
            using (var db = new KonneyContext())
            {
                var person = db.People.First(p => p.ID == this.ID);

                if (userID == person.User.ID)
                {
                    person.FirstName = this.FirstName;
                    person.LastName = this.LastName;
                    person.Email = this.Email;
                    person.PhoneNumber = this.PhoneNumber;
                    
                    db.SaveChanges();
                }
                else
                {
                    throw new AuthenticationException("You are not authorized to edit this person.");
                }
            }
        }
    }
}