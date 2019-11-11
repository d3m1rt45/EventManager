using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KonneyTM.Models
{
    public class User
    {
        public string ID { get; set; }

        public virtual ICollection<Event> Events { get; set; }
        public virtual ICollection<Venue> Venues { get; set; }
        public virtual ICollection<Person> People { get; set; }
    }
}