﻿using Newtonsoft.Json.Linq;
using Xunit;

namespace Chronological.Tests
{
    public class UniqueValuesAggregateTests
    {
        public JProperty ExpectedResult()
        {
            var measureJArray = new JArray();
            measureJArray.Add(new JObject(new JProperty("max", new JObject(TestType1JProperties.Value))));
            return new JProperty("measures", measureJArray);
        }

        public static JProperty ExpectedLastResult()
        {
            return new JProperty("measures", new JArray(new JObject(TestType1JProperties.LastMeasureWithOrderBy)));
        }

        public static JProperty ExpectedFirstResult()
        {
            return new JProperty("measures", new JArray(new JObject(TestType1JProperties.FirstMeasureWithOrderBy)));
        }

        [Fact]
        public void Test1()
        {
            var builder = new AggregateBuilder<TestType1>();
            var aggregate = builder.UniqueValues(x => x.Value, 10, new { Maximum = builder.Maximum(x => x.Value) });

            var test = aggregate.ToChildJProperty();
            var expected = ExpectedResult();
            Assert.True(JToken.DeepEquals(test, expected));
        }

        [Fact]
        public void UsingLastInAggregateShouldGiveCorrectJson()
        {
            var builder = new AggregateBuilder<TestType1>();
            var aggregate = builder.UniqueValues(x => x.DataType, 10, new { Last = builder.Last(x => x.Value, y => y.Date) });

            var test = aggregate.ToChildJProperty();
            var expected = ExpectedLastResult();
            Assert.True(JToken.DeepEquals(test, expected));
        }

        [Fact]
        public void UsingFirstInAggregateShouldGiveCorrectJson()
        {
            var builder = new AggregateBuilder<TestType1>();
            var aggregate = builder.UniqueValues(x => x.DataType, 10, new { Last = builder.First(x => x.Value, y => y.Date) });

            var test = aggregate.ToChildJProperty();
            var expected = ExpectedFirstResult();
            Assert.True(JToken.DeepEquals(test, expected));
        }

        public JProperty ExpectedNestedResult()
        {
            var test = JToken.Parse(@"{'aggregate': {
                                      'dimension': {
                                        'uniqueValues': {
                                          'input': {
                                            'property': 'data.value',
                                            'type': 'Double'
                                          },
                                          'take': 10
                                        }
                                      },
                                      'measures': [
                                        {
                                          'max': {
                                            'input': {
                                              'property': 'data.value',
                                              'type': 'Double'
                                            }
                                          }
                                        }
                                      ]
                                    }}");     
            return (JProperty)test.First;
        }

        [Fact]
        public void NestedAggregate()
        {
            var builder = new AggregateBuilder<TestType1>();
            var aggregate = builder.UniqueValues(x => x.Value, 10, 
                                builder.UniqueValues(x => x.Value, 10, 
                                    new { Maximum = builder.Maximum(x => x.Value) }));

            var test = aggregate.ToChildJProperty();
            var expected = ExpectedNestedResult();
            Assert.True(JToken.DeepEquals(test, expected));
        }
        public JProperty ExpectedLastNestedResult()
        {
            var test = JToken.Parse(@"{'aggregate': {
                                      'dimension': {
                                        'uniqueValues': {
                                          'input': {
                                            'property': 'data.value',
                                            'type': 'Double'
                                          },
                                          'take': 10
                                        }
                                      },
                                      'measures': [
                                        {
                                          'last': {
                                            'input': {
                                              'property': 'data.value',
                                              'type': 'Double'
                                            }
                                          }
                                        }
                                      ]
                                    }}");
            return (JProperty)test.First;
        }

        [Fact]
        public void NestedAggregateWithLast()
        {
            var builder = new AggregateBuilder<TestType1>();
            var aggregate = builder.UniqueValues(x => x.Value, 10,
                builder.UniqueValues(x => x.Value, 10,
                    new { Last = builder.Last(x => x.Value) }));

            var test = aggregate.ToChildJProperty();
            var expected = ExpectedLastNestedResult();
            Assert.True(JToken.DeepEquals(test, expected));
        }
    }
}
