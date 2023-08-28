using FluentAssertions;
using Ninject;
using System.Threading.Tasks;
using BhFS.Tippspiel.Utils;
using Snapshooter.Xunit;
using Tippspiel.Contracts;
using Xunit;

namespace Tippspiel.Tests
{
    [Trait("Tippspiel","Openliga Repository")]
    public class OpenligaRepositoryTests
    {
        private readonly IFussballDataRepository _repository;

        public OpenligaRepositoryTests()
        {
            StandardKernel kernel = new StandardKernel( new TippspielModule() );
            _repository = kernel.Get<IFussballDataRepository>();
        }

        [Fact(DisplayName = "Get Matches By Group")]
        public async Task GetMatchesByGroupTest()
        {
            // given
            int groupId = 3;

            // when
            var result = await _repository.GetMatchesByGroupAsync(groupId);

            // then
            Snapshot.Match(result);
        }

        [Fact(DisplayName = "Get Current Group")]
        public async Task GetCurrentGroupTest()
        {
            // given

            // when
            var result = await _repository.GetCurrentGroupAsync();

            // then
            Snapshot.Match(result);
        }

        [Fact(DisplayName = "Get All Groups")]
        public async Task GetAllGroupsTest()
        {
            // given
            int expectedMatches = 34;

            // when
            var groups = await _repository.GetAllGroupsAsync();

            // then
            groups.Count.Should().Be(expectedMatches);
        }

        [Fact(DisplayName = "Get All Matches")]
        public async Task GetAllMatchesTest()
        {
            // given
            int expectedMatches = 306;

            // when
            var matches = await _repository.GetAllMatchesAsync();

            // then
            matches.Count.Should().Be(expectedMatches);
        }


        [Fact(DisplayName = "Get Match By Id")]
        public async Task GetMatchById()
        {
            // given
            int matchId = 66641;

            // when
            var match = await _repository.GetMatchDataAsync(matchId);

            // then
            Snapshot.Match(match);
        }

        [Fact(DisplayName = "Get Match Not Started")]
        public async Task GetMatchByIdNotYetStarted()
        {
            // given
            int matchId = 66648;

            // when
            var match = await _repository.GetMatchDataAsync(matchId);

            // then
            Snapshot.Match(match);
        }

        [Fact(DisplayName = "Get Teams")]
        public async Task GetTeamsTest()
        {
            // given
 
            // when
            var result = await _repository.GetTeamsAsync();

            // then
            Snapshot.Match(result);
        }

        [Fact(DisplayName = "Get Next Match")]
        public async Task GetNextMatch()
        {
            // given

            // when
            var result = await _repository.GetNextMatchAsync();

            // then
            Snapshot.Match(result);
        }

        [Fact(DisplayName = "Get Last Match")]
        public async Task GetLastMatch()
        {
            // given

            // when
            var result = await _repository.GetLastMatchAsync();

            // then
            Snapshot.Match(result);
        }
    }
}
