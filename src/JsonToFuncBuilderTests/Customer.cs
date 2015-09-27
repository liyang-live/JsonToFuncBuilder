using System;

namespace JsonToFuncBuilderTests
{
    public class Customer
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public DateTime BirthDay { get; set; }

        public Location Address { get; set; }
    }

    public class Location
    {
        public string Street { get; set; }
    }
}