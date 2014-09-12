// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IoC.cs" company="Web Advanced">
// Copyright 2012 Web Advanced (www.webadvanced.com)
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.GamingGroups;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.DataAccess.Security;
using BusinessLogic.EventTracking;
using BusinessLogic.Logic;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Logic.Players;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity;
using StructureMap;
using StructureMap.Graph;
using System.Configuration.Abstractions;
using System.Data.Entity;
using System.Web.Mvc;
using UI.Controllers.Helpers;
using UI.Filters;
using UI.Models.PlayedGame;
using UI.Transformations;
using UI.Transformations.Player;
using UniversalAnalyticsHttpWrapper;
namespace UI.DependencyResolution {
    public static class IoC {
        public static IContainer Initialize() {
            ObjectFactory.Initialize(x =>
                        {
                            x.Scan(scan =>
                                    {
                                        scan.TheCallingAssembly();
                                        scan.WithDefaultConventions();
                                    });
                            //TODO MAKE THIS PER REQUEST
                            x.For<DbContext>().Use<NemeStatsDbContext>();
                            x.For<IDataContext>().Use<NemeStatsDataContext>();
                            x.For<IPlayerRepository>().Use<EntityFrameworkPlayerRepository>();
                            x.For<IGameDefinitionRetriever>().Use<GameDefinitionRetriever>();
                            x.For<IPlayedGameRetriever>().Use<PlayedGameRetriever>();
                            x.For<IPlayedGameDetailsViewModelBuilder>().Use<PlayedGameDetailsViewModelBuilder>();
                            x.For<IGameResultViewModelBuilder>().Use<GameResultViewModelBuilder>();
                            x.For<IPlayerDetailsViewModelBuilder>().Use<PlayerDetailsViewModelBuilder>();
                            x.For<IGamingGroupViewModelBuilder>()
                                .Use<GamingGroupViewModelBuilder>();
                            x.For<IGamingGroupInvitationViewModelBuilder>()
                                .Use<GamingGroupInvitationViewModelBuilder>();
                            x.For<IGamingGroupAccessGranter>().Use<EntityFrameworkGamingGroupAccessGranter>();
                            x.For<IGamingGroupInviteConsumer>().Use<GamingGroupInviteConsumer>();
                            x.For<Microsoft.AspNet.Identity.IUserStore<ApplicationUser>>()
                                .Use<Microsoft.AspNet.Identity.EntityFramework.UserStore<ApplicationUser>>();
                            x.For<IGamingGroupCreator>().Use<GamingGroupCreator>();
                            x.For<IGameDefinitionViewModelBuilder>()
                                .Use<GameDefinitionViewModelBuilder>();
                            x.For<IShowingXResultsMessageBuilder>().Use<ShowingXResultsMessageBuilder>();
                            x.For<IGamingGroupRetriever>().Use<GamingGroupRetriever>();
                            x.For<IPendingGamingGroupInvitationRetriever>().Use<PendingGamingGroupInvitationRetriever>();
                            x.For<IPlayedGameCreator>().Use<PlayedGameCreator>();
                            x.For<NemeStatsEventTracker>().Use<UniversalAnalyticsNemeStatsEventTracker>();
                            x.For<IEventTracker>().Use<EventTracker>();
                            x.For<IUniversalAnalyticsEvent>().Use<UniversalAnalyticsEvent>();
                            x.For<IUniversalAnalyticsEventFactory>().Use<UniversalAnalyticsEventFactory>();
                            x.For<IConfigurationManager>().Use<ConfigurationManager>();
                            x.For<ITopPlayerViewModelBuilder>().Use<TopPlayerViewModelBuilder>();
                            x.For<IPlayerSummaryBuilder>().Use<PlayerSummaryBuilder>();
                            x.For<IPlayerSaver>().Use<PlayerSaver>();
                            x.For<IGameDefinitionSaver>().Use<GameDefinitionSaver>();
                            x.For<IPlayerRetriever>().Use<PlayerRetriever>();
                        });
            return ObjectFactory.Container;
        }
    }
}