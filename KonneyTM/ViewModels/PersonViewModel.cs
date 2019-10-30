﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KonneyTM.Models
{
    public class PersonViewModel
    {
        public int ID { get; set; }
        public bool Attending { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }
}