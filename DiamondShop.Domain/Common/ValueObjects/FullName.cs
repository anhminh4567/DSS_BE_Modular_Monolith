using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Common.ValueObjects
{
    public class FullName
    {
        public string FirstName { get; private set; }   
        public string LastName { get; private set; }

        public FullName(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        public string Value => FirstName + " " + LastName;
        public static FullName Create(string firstName , string lastName)
        {
            return new FullName(firstName , lastName);
        }
        public static FullName parse(string firstName, string lastName)
        {
            return Create(firstName, lastName);

        }

    }
}
