using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
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

        public string ImagePath { get; set; }

        public virtual ICollection<Event> Events { get; set; }
    }
}