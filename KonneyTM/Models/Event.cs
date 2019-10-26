using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace KonneyTM.Models
{
    public class Event
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Title { get; set; }
        public DateTime Time { get; set; }

        public virtual ICollection<Person> PeopleAttending { get; set; }
        public virtual Venue Place { get; set; }
    }
}