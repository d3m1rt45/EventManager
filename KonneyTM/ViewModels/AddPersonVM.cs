using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KonneyTM.Models;

namespace KonneyTM.ViewModels
{
    public class AddPersonVM
    {
        public AddPersonVM()
        {
            People = new List<PersonViewModel>();
        }

        public int EventID { get; set; }
        public List<PersonViewModel> People { get; set; }
    }
}