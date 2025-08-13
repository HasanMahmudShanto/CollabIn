using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CollabIn.Models
{
    
    public class ValidateAttribute : ValidationAttribute
    {
        private readonly int MinAge;
        public ValidateAttribute(int minAge)
        {
            MinAge = minAge;
            ErrorMessage = $"Age must be at least {MinAge} Years.";
        }

        public override bool IsValid(object value)
        {
            if (value is DateTime dob)
            {
                int age = DateTime.Now.Year - dob.Year;
                if (dob > DateTime.Now.AddYears(-age)) age--;
                return age >= MinAge;
            }
            return false;
        }



    }
}