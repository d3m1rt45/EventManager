using KonneyTM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KonneyTM.DAL
{
    public class KonneyInitializer : System.Data.Entity.DropCreateDatabaseAlways<KonneyContext>
    {
        protected override void Seed(KonneyContext context)
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
                new Venue{Name="Cavendish Conference Centre", Address="Cavendish Venues, 22 Duchess Mews, Marylebone, London", PostCode="W1G 9DT", PhoneNumber="02077067700", ImagePath="02077067700W1G9DT.jpg"},
                new Venue{Name="National Theatre", Address="Upper Ground, Lambeth, London", PostCode="SE1 9PX", PhoneNumber="02074523000", ImagePath="0207452000SE19PX.jpg"},
                new Venue{Name="The Harp", Address="47 Chandos Pl, Charing Cross, London", PostCode="WC2N 4HS", PhoneNumber="02078360291", ImagePath="02078360291WC2N4HS.jpg"}
            };

            venues.ForEach(v => context.Venues.Add(v));
            context.SaveChanges();


            var events = new List<Event>
            {
                new Event{Title="Oxford Law (17-18) Reunion", PeopleAttending=people.Take(5).ToList(), Place=venues[0], Date=DateTime.Now.AddDays(24), Time = DateTime.UtcNow, ImagePath="whatlawnow.jpg"},
                new Event{Title="Party!! For like... No reason!", PeopleAttending=people, Place=venues.First(p => p.Name == "The Harp"), Date=DateTime.Now.AddDays(7), Time = DateTime.UtcNow, ImagePath="noreasonforthisparty.jpg"},
            };

            events.ForEach(e => context.Events.Add(e));
            context.SaveChanges();

            var demoUser = new User
            {
                ID = "demo",
                Events = events,
                People = people,
                Venues = venues
            };
            context.Users.Add(demoUser);
            context.SaveChanges();
        }
    }
}