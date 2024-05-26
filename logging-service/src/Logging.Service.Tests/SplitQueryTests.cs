using Logging.Server.StreamData.Validator.Models;
using Logging.Server.StreamData.Validator.Services.Implementation;

namespace Logging.Service.Tests
{
    public class SplitQueryTests
    {
        private MqlQuerySplitter _querySplitter => new MqlQuerySplitter();

        [Fact]
        public void SplitQuery_EmptyQuery_ReturnsEmptyQueryAndEmptyTerms()
        {
            var result = _querySplitter.SplitQuery("");
            Assert.Equal("0", result.Query);
            Assert.Empty(result.Terms);
        }

        [Theory]
        [InlineData("\"term1\" \"term2\"")]
        [InlineData("\"term2\" \"term3\"")]
        [InlineData("\"term1\" sdf")]
        [InlineData("\"term2\" sdf sdffdf")]
        public void SplitQuery_OnlyEscapedFullTerms_ReturnsQueryWithEscapedFullTermsAndEmptyTerms(string query)
        {
            var result = _querySplitter.SplitQuery(query);
            Assert.Equal("0", result.Query);
            Assert.Empty(result.Terms);
        }

        [Theory]
        [InlineData("1")]
        [InlineData("dsffsdf")]
        [InlineData("getPeriod")]
        [InlineData("All")]
        public void SplitQuery_OnlyFullSearchTerms_ReturnsQueryWithFullSearchTermsAndEmptyTerms(string query)
        {
            var result = _querySplitter.SplitQuery(query);
            Assert.Equal("{0}", result.Query);
            Assert.True(result.Terms.First().Type == TermType.FullSearchTerm);
        }

        [Theory]
        [InlineData("!source.status:500 AND (source.method:PUT OR source.method:GET) AND source.ip:exists()")]
        [InlineData("source.status:501 OR source.method:PUT OR source.method:POST AND source.ip:43")]
        [InlineData("!source.status:502 AND (source.method:POST OR source.method:GET) AND source.ip:12")]
        [InlineData("source.status:503 OR source.method:PUT OR source.method:POST AND source.ip:exists()")]

        [InlineData("!source.status:500 AND (source.method:PUT OR source.method:GET)")]
        [InlineData("source.status:501")]
        [InlineData("!source.status:502 AND (source.method:POST OR source.method:GET)")]
        [InlineData("source.status:43 OR source.method:PUT OR source.method:POST AND source.ip:exists()")]

        [InlineData("source.ip:exists()")]
        [InlineData("source.method:PUT")]
        [InlineData("source.ip:12")]
        [InlineData("source.method:PUT OR source.method:POST")]

        [InlineData("!source.status:500")]
        [InlineData("source.ip:43")]
        [InlineData("!source.status:502")]
        [InlineData("source.method:GET OR source.method:POST")]
        public void SplitQuery_MixedTerms_ReturnsQueryWithMixedTermsAndCorrectTerms(string query)
        {
            var result = _querySplitter.SplitQuery(query);
            Assert.True(result.Terms.All(x => x.Type == TermType.CommonTerm));
        }
    }
}
