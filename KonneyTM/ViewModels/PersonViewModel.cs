using KonneyTM.DAL;
using KonneyTM.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace KonneyTM.ViewModels
{
    public class PersonViewModel
    {   
        public int ID { get; set; }

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


        //Returns all the Persons in the Database as a List of PersonViewModel instances ordered by FirstName
        public static List<PersonViewModel> GetAllAsOrderedList()
        {
            using (var db = new KonneyContext())
            {
                var people = db.People.ToList();
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

        //Takes a Person instance and returns a PersonViewModel instance
        public static PersonViewModel FromPerson(Person person)
        {
            var personVM = new PersonViewModel
            {
                ID = person.ID,
                FirstName = person.FirstName,
                LastName = person.LastName,
                PhoneNumber = person.PhoneNumber,
                Email = person.Email
            };
            return personVM;
        }

        //Takes a Person list and returns a PersonViewModels list
        public static List<PersonViewModel> FromPersonList(ICollection<Person> people)
        {
            var personVMList = new List<PersonViewModel>();

            foreach(var p in people)
            {
                personVMList.Add(new PersonViewModel
                {
                    ID = p.ID,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    Email = p.Email,
                    PhoneNumber = p.PhoneNumber
                });
            }

            return personVMList;
        }

        //Converts this instance to Person and saves it to the database all at once.
        public void SaveToDB()
        {
            using (var db = new KonneyContext())
            {
                db.People.Add(new Person
                {
                    FirstName = this.FirstName,
                    LastName = this.LastName,
                    PhoneNumber = this.PhoneNumber,
                    Email = this.Email
                });

                db.SaveChanges();
            }
        }
        public void SubmitChanges()
        {
            using (var db = new KonneyContext())
            {
                var person = db.People.First(p => p.ID == this.ID);

                person.FirstName = this.FirstName;
                person.LastName = this.LastName;
                person.Email = this.Email;
                person.PhoneNumber = this.PhoneNumber;

                db.SaveChanges();
            }
        }
    }
}