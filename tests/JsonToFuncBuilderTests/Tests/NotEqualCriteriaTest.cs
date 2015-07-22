using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Ploeh.AutoFixture;
using Should;

namespace JsonToFuncBuilderTests.Tests
{
    public class NotEqualCriteriaTest:BaseTest
    {
         

        public void find_zero_records_for_string_criteria()
        {

            var testData = TestStrings.NotEqualString;

            var criteriaGroups = JsonConvert.DeserializeObject<IList<CriteriaGroup>>(testData);

            var criteria = CriteriaExtensions.BuildCriteria<Customer>(criteriaGroups);

            var customers = Registry.CreateMany<Customer>(1).ToList();

            customers.First().Name = "Skippy";

            customers.Where(criteria).Count().ShouldEqual(0);
        }

        public void find_zero_records_for_integer_criteria()
        {
            var testData = TestStrings.NotEqualInteger;

            var criteriaGroups = JsonConvert.DeserializeObject<IList<CriteriaGroup>>(testData);

            var criteria = CriteriaExtensions.BuildCriteria<Customer>(criteriaGroups);

            var customers = Registry.CreateMany<Customer>(1).ToList();

            customers.First().Age = 10;

            customers.Where(criteria).Count().ShouldEqual(0);
        }  
        
        public void find_zero_records_for_date_time_criteria()
        {
            var testData = TestStrings.NotEqualDateTime;

            var criteriaGroups = JsonConvert.DeserializeObject<IList<CriteriaGroup>>(testData);

            var criteria = CriteriaExtensions.BuildCriteria<Customer>(criteriaGroups);

            var customers = Registry.CreateMany<Customer>(1).ToList();

            customers.First().BirthDay = DateTime.Parse("10/10/2015");

            customers.Where(criteria).Count().ShouldEqual(0);
        } 
    }
}