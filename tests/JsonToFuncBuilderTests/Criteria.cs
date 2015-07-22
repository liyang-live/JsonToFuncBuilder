using System.Collections.Generic;

namespace JsonToFuncBuilderTests
{
    public class CriteriaGroup
    {
        public IEnumerable<Criterion> Criteria { get; set; } 
    }
    public class Criterion
    {
        public string Field { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
    }
}