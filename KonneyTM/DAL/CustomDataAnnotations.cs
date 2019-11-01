using System;
using System.ComponentModel.DataAnnotations;

namespace KonneyTM.DAL.CustomDataAnnotations
{
    public class CurrentDateAttribute : ValidationAttribute
    {
        public CurrentDateAttribute()
        {
        }

        public override bool IsValid(object value)
        {
            var dt = (DateTime)value;
            if (dt >= DateTime.Now && dt <= DateTime.Now.AddYears(3))
            { 
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}