using System.Collections.Generic;
using System.Linq;
using JsonToFuncBuilder;
using Newtonsoft.Json;
using Ploeh.AutoFixture;
using Should;

namespace JsonToFuncBuilderTests.Tests
{
    public class OneCriteriaTest:BaseTest
    {
        private IList<CriteriaGroup> _criteriaGroups;
        

        public OneCriteriaTest()
        {
            var testData = TestStrings.OneCriteria;

            _criteriaGroups = JsonConvert.DeserializeObject<IList<CriteriaGroup>>(testData);
        }

        public void can_find_1_record_out_of_10_that_match_criteria()
        {
            var criteria = CriteriaExtensions.BuildCriteria<Customer>(_criteriaGroups);

            var customers = Registry.CreateMany<Customer>(10).ToList();

            customers.First().Name = "Skippy";

            customers.Where(criteria).Count().ShouldEqual(1);
        }

    }
}