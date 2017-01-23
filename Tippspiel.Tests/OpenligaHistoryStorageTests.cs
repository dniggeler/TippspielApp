using FluentAssertions;
using Ninject;
using System.Linq;
using System.Threading.Tasks;
using Tippspiel.Contracts;
using Tippspiel.Implementation;
using Xunit;

namespace Tippspiel.Tests
{
    [Trait("Tippspiel","History Storage")]
    public class OpenligaHistoryStorageTests
    {
        private readonly StandardKernel _kernel;
        private readonly IMatchHistory _matchHistoryStorage;

        public OpenligaHistoryStorageTests()
        {
            _kernel = new StandardKernel( new ClientModule() );
            _matchHistoryStorage = _kernel.Get<IMatchHistory>();
        }

        [Fact(DisplayName = "Get All Groups")]
        public void GetAllGroupsTest()
        {
            // given
            int expectedMatches = 0;

            // when
            var matches = _matchHistoryStorage.GetAllGroups();

            // then
            matches.Count.Should().Be(expectedMatches);
        }

        [Fact(DisplayName = "Get All Matches")]
        public void GetAllMatchesTest()
        {
            // given
            var groups = _matchHistoryStorage.GetAllMatches();
            int expectedMatches = groups.Count()*9;

            // when
            var matches = _matchHistoryStorage.GetAllMatches();

            // then
            matches.Count.Should().Be(expectedMatches);
        }
    }
}
