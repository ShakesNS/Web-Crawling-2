using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace IndividualReport.Models
{
    public class Individual
    {
        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string LastName { get; set; }

        public string FullName 
        {
            get 
            {
                if (MiddleName == null)
                    return $"{FirstName} {LastName}";
                else
                    return $"{FirstName} {MiddleName} {LastName}";
            }
        }
        public string UniqueIdentifier { get; set; }
        public string DateOfBirth { get; set; }
        public string PlaceOfBirth { get; set; }
        public string SanctionStatus { get; set; }

    }
}
