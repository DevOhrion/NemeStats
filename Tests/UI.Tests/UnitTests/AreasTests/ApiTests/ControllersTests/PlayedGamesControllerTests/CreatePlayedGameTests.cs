﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using BusinessLogic.Logic;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.AutoMocking;
using UI.Areas.Api.Controllers;
using UI.Areas.Api.Models;
using UI.Attributes;

namespace UI.Tests.UnitTests.AreasTests.ApiTests.ControllersTests.PlayedGamesControllerTests
{
    [TestFixture]
    public class PlayedGamesControllerTests
    {
        private RhinoAutoMocker<PlayedGamesController> autoMocker;
        private ApplicationUser applicationUser;
        private PlayedGameMessage playedGameMessage;
        private const int EXPECTED_PLAYED_GAME_ID = 1;
        private const int EXPECTED_GAMING_GROUP_ID = 2;

        [SetUp]
        public void SetUp()
        {
            this.autoMocker = new RhinoAutoMocker<PlayedGamesController>();
            var controllerContextMock = MockRepository.GeneratePartialMock<HttpControllerContext>();
            HttpActionDescriptor actionDescriptorMock = MockRepository.GenerateMock<HttpActionDescriptor>();
            this.autoMocker.ClassUnderTest.ActionContext = new HttpActionContext(controllerContextMock, actionDescriptorMock);
            this.autoMocker.ClassUnderTest.Request = new HttpRequestMessage();
            this.autoMocker.ClassUnderTest.Request.SetConfiguration(new HttpConfiguration());

            this.applicationUser = new ApplicationUser
            {
                Id = "application user id",
                CurrentGamingGroupId = EXPECTED_GAMING_GROUP_ID
            };

            this.playedGameMessage = new PlayedGameMessage
            {
                DatePlayed = "2015-04-10",
                GameDefinitionId = 1,
                Notes = "some notes"
            };

            PlayedGame expectedPlayedGame = new PlayedGame
            {
                Id = EXPECTED_PLAYED_GAME_ID
            };
            this.autoMocker.Get<IPlayedGameCreator>().Expect(mock => mock.CreatePlayedGame(
                                                                          Arg<NewlyCompletedGame>.Is.Anything,
                                                                          Arg<TransactionSource>.Is.Anything,
                                                                          Arg<ApplicationUser>.Is.Anything))
                    .Return(expectedPlayedGame);
        }

        [Test]
        public void ItReturnsAnHttp401NotAuthorizedIfTheGamingGroupIdDoesntMatchTheCurrentlyLoggedInUser()
        {
            var actualResponse = this.autoMocker.ClassUnderTest.RecordPlayedGame(this.playedGameMessage, -1, this.applicationUser);

            Assert.That(actualResponse.Content, Is.TypeOf(typeof(ObjectContent<HttpError>)));
            var content = actualResponse.Content as ObjectContent<HttpError>;
            var httpError = content.Value as HttpError;
            Assert.That(actualResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
            Assert.That(httpError.Message, Is.EqualTo(ApiAuthenticationAttribute.UNAUTHORIZED_MESSAGE));

        }

        [Test]
        public void ItRecordsThePlayedGameWithTheTransactionSourceSetToRestApi()
        {
            this.autoMocker.ClassUnderTest.RecordPlayedGame(this.playedGameMessage, this.applicationUser.CurrentGamingGroupId.Value, this.applicationUser);

            this.autoMocker.Get<IPlayedGameCreator>().AssertWasCalled(mock => mock.CreatePlayedGame(
                Arg<NewlyCompletedGame>.Is.Anything,
                Arg<TransactionSource>.Is.Equal(TransactionSource.RestApi),
                Arg<ApplicationUser>.Is.Same(this.applicationUser)));
        }

        [Test]
        public void ItRecordsTheDatePlayed()
        {
            DateTime expectedDateTime = new DateTime(2015, 4, 10);

            this.autoMocker.ClassUnderTest.RecordPlayedGame(this.playedGameMessage, this.applicationUser.CurrentGamingGroupId.Value, this.applicationUser);

            this.autoMocker.Get<IPlayedGameCreator>().AssertWasCalled(mock => mock.CreatePlayedGame(
                Arg<NewlyCompletedGame>.Matches(x => x.DatePlayed.Date == expectedDateTime.Date),
                Arg<TransactionSource>.Is.Anything,
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItSetsTheDatePlayedToTodayIfItIsNotSpecified()
        {
            this.playedGameMessage.DatePlayed = null;

            this.autoMocker.ClassUnderTest.RecordPlayedGame(this.playedGameMessage, this.applicationUser.CurrentGamingGroupId.Value, this.applicationUser);

            this.autoMocker.Get<IPlayedGameCreator>().AssertWasCalled(mock => mock.CreatePlayedGame(
                Arg<NewlyCompletedGame>.Matches(x => x.DatePlayed.Date == DateTime.UtcNow.Date),
                Arg<TransactionSource>.Is.Anything,
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItSetsTheGameDefinitionId()
        {
            this.autoMocker.ClassUnderTest.RecordPlayedGame(this.playedGameMessage, this.applicationUser.CurrentGamingGroupId.Value, this.applicationUser);

            this.autoMocker.Get<IPlayedGameCreator>().AssertWasCalled(mock => mock.CreatePlayedGame(
                Arg<NewlyCompletedGame>.Matches(x => x.GameDefinitionId == this.playedGameMessage.GameDefinitionId),
                Arg<TransactionSource>.Is.Anything,
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItSetsTheNotes()
        {
            this.autoMocker.ClassUnderTest.RecordPlayedGame(this.playedGameMessage, this.applicationUser.CurrentGamingGroupId.Value, this.applicationUser);

            this.autoMocker.Get<IPlayedGameCreator>().AssertWasCalled(mock => mock.CreatePlayedGame(
                Arg<NewlyCompletedGame>.Matches(x => x.Notes == this.playedGameMessage.Notes),
                Arg<TransactionSource>.Is.Anything,
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItSetsThePlayerRanks()
        {
            this.playedGameMessage.PlayerRanks = new List<PlayerRank>
            {
                new PlayerRank() { GameRank = 1, PlayerId = 100, PointsScored = 10 },
                new PlayerRank() { GameRank = 2, PlayerId = 200, PointsScored = 8 }
            };
            this.autoMocker.ClassUnderTest.RecordPlayedGame(this.playedGameMessage, this.applicationUser.CurrentGamingGroupId.Value, this.applicationUser);
            IList<object[]> arguments = this.autoMocker.Get<IPlayedGameCreator>().GetArgumentsForCallsMadeOn(mock => mock.CreatePlayedGame(
                Arg<NewlyCompletedGame>.Is.Anything,
                Arg<TransactionSource>.Is.Anything,
                Arg<ApplicationUser>.Is.Anything));

            NewlyCompletedGame newlyCompletedGame = arguments[0][0] as NewlyCompletedGame;
            Assert.That(newlyCompletedGame.PlayerRanks[0].GameRank, Is.EqualTo(this.playedGameMessage.PlayerRanks[0].GameRank));
            Assert.That(newlyCompletedGame.PlayerRanks[0].PlayerId, Is.EqualTo(this.playedGameMessage.PlayerRanks[0].PlayerId));
            Assert.That(newlyCompletedGame.PlayerRanks[0].PointsScored, Is.EqualTo(this.playedGameMessage.PlayerRanks[0].PointsScored));
            Assert.That(newlyCompletedGame.PlayerRanks[1].GameRank, Is.EqualTo(this.playedGameMessage.PlayerRanks[1].GameRank));
            Assert.That(newlyCompletedGame.PlayerRanks[1].PlayerId, Is.EqualTo(this.playedGameMessage.PlayerRanks[1].PlayerId));
            Assert.That(newlyCompletedGame.PlayerRanks[1].PointsScored, Is.EqualTo(this.playedGameMessage.PlayerRanks[1].PointsScored));
        }

        [Test]
        public void ItReturnsThePlayedGameIdOfTheNewlyCreatedPlayedGame()
        {
            var actualResponse = this.autoMocker.ClassUnderTest.RecordPlayedGame(this.playedGameMessage, this.applicationUser.CurrentGamingGroupId.Value, this.applicationUser);

            Assert.That(actualResponse.Content, Is.TypeOf(typeof(ObjectContent<NewlyRecordedPlayedGameMessage>)));
            ObjectContent<NewlyRecordedPlayedGameMessage> content = actualResponse.Content as ObjectContent<NewlyRecordedPlayedGameMessage>;
            NewlyRecordedPlayedGameMessage newlyRecordedPlayedGameMessage = content.Value as NewlyRecordedPlayedGameMessage;
            Assert.That(newlyRecordedPlayedGameMessage.PlayedGameId, Is.EqualTo(EXPECTED_PLAYED_GAME_ID));
        }
    }
}