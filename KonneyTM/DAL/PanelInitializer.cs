using KonneyTM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KonneyTM.DAL
{
    public class PanelInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<PanelContext>
    {
        protected override void Seed(PanelContext context)
        {
            var people = new List<Person>
            {
                new Person{FirstName="Carson", LastName="Alexander", PhoneNumber="07852125782", Email="carsonalexander@example.com"},
                new Person{FirstName="Meredith", LastName="Alonso", PhoneNumber="07852152153", Email="meredithalonso@example.com"},
                new Person{FirstName="Arturo", LastName="Anand", PhoneNumber="07855625152", Email="arturoanand@example.com"},
                new Person{FirstName="Gytis", LastName="Barzdukas", PhoneNumber="07852132147", Email="gyatisbarzdukas@example.com"},
                new Person{FirstName="Yan", LastName="Li", PhoneNumber="05798152152", Email="yanli@example.com"},
                new Person{FirstName="Peggy", LastName="Justice", PhoneNumber="07257896158", Email="peggyjustice@example.com"},
                new Person{FirstName="Laura", LastName="Norman", PhoneNumber="07578152159", Email="lauranorman@example.com"},
                new Person{FirstName="Nino", LastName="Olivetto", PhoneNumber="07858872152", Email="ninoolivetto@example.com"}
            };

            people.ForEach(p => context.People.Add(p));
            context.SaveChanges();


            var venues = new List<Venue>
            {
                new Venue{Name="Cavendish Conference Centre", Address="Cavendish Venues, 22 Duchess Mews, Marylebone, London", PostCode="W1G 9DT", PhoneNumber="02077067700"},
                new Venue{Name="Lisa's House", Address="26 Hartbrooke Road, Enfield", PostCode="EN2 0DE", PhoneNumber="07958569896"},
                new Venue{Name="The Harp", Address="47 Chandos Pl, Charing Cross, London", PostCode="WC2N 4HS", PhoneNumber="020 7836 0291"}
            };

            venues.ForEach(v => context.Venues.Add(v));
            context.SaveChanges();


            var events = new List<Event>
            {
                new Event{Title="Oxford Law (17-18) Reunion", PeopleAttending=people, Place=venues[0], Time=DateTime.Now.AddDays(24)},
                new Event{Title="Party!! For like... No reason!", PeopleAttending=people, Place=venues[1], Time=DateTime.Now.AddDays(7)},
            };

            events.ForEach(e => context.Events.Add(e));
            context.SaveChanges();
        }
    }
}