using KonneyTM.DAL;
using KonneyTM.DAL.CustomDataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KonneyTM.ViewModels
{
    public class EditEventVM
    {
        public int ID { get; set; }

        [Required]
        [StringLength(30)]
        public string Title { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [CurrentDate(ErrorMessage = "Date must be between tomorrow and 3 years from now.")]
        public DateTime Date { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh-mm}", ApplyFormatInEditMode = true)]
        public DateTime Time { get; set; }

        [DataType(DataType.ImageUrl)]
        public string ImagePath { get; set; }

        [Required]
        public HttpPostedFileBase ImageFile { get; set; }

        public void Submit()
        {
            var db = new KonneyContext();
            var correspondingEvent = db.Events.First(e => e.ID == this.ID);

            if(this.ImagePath != null)
                correspondingEvent.ImagePath = this.ImagePath;

            if (this.Title != null)
                correspondingEvent.Title = this.Title;
            
            if (this.Date != null)
                correspondingEvent.Date = this.Date;
            
            if(this.Time != null)
                correspondingEvent.Time = this.Time;

            db.SaveChanges();
            db.Dispose();
        }
    }
}