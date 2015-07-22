using System.Collections.Generic;
using System.Linq;
using JsonToFuncBuilder;
using Newtonsoft.Json;
using Ploeh.AutoFixture;
using Should;

namespace JsonToFuncBuilderTests.Tests
{
    public class TwoCriteriaTest:BaseTest
    {
         private IList<CriteriaGroup> _criteriaGroups;


         public TwoCriteriaTest()
        {
            var testData = TestStrings.OneGroupTwoCriteria;

            _criteriaGroups = JsonConvert.DeserializeObject<IList<CriteriaGroup>>(testData);
        }

        public void can_find_more_than_1_record_out_of_10_that_match_criteria()
        {
            var criteria = CriteriaExtensions.BuildCriteria<Customer>(_criteriaGroups);

            var customers = Registry.CreateMany<Customer>(10).ToList();

            customers.First().Name = "Skippy";
            customers[2].Age = 50;

            customers.Where(criteria).Count().ShouldBeGreaterThan(1);
        } 
    }
}