using KonneyTM.DAL;
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


        public static void FindOrCreate(KonneyContext db, string userID)
        {
            var user = db.Users.Find(userID);
            if (user == null)
                db.Users.Add(new User { ID = userID });
            db.SaveChanges();
        }
    }
}