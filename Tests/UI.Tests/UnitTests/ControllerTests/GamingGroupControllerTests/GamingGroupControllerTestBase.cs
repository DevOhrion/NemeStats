﻿using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.GamingGroups;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using UI.Controllers;
using UI.Transformations;

namespace UI.Tests.UnitTests.ControllerTests.GamingGroupControllerTests
{
    [TestFixture]
    public class GamingGroupControllerTestBase
    {
        protected DataContext dataContext;
        protected GamingGroupRepository gamingGroupRepositoryMock;
        protected GamingGroupToGamingGroupViewModelTransformation gamingGroupToGamingGroupViewModelTransformationMock;
        protected GamingGroupController gamingGroupController;
        protected GamingGroupAccessGranter gamingGroupAccessGranterMock;
        protected GamingGroupCreator gamingGroupCreator;
        protected ApplicationUser currentUser;

        [SetUp]
        public void SetUp()
        {
            dataContext = MockRepository.GenerateMock<DataContext>();
            gamingGroupRepositoryMock = MockRepository.GenerateMock<GamingGroupRepository>();
            gamingGroupToGamingGroupViewModelTransformationMock = MockRepository.GenerateMock<GamingGroupToGamingGroupViewModelTransformation>();
            gamingGroupAccessGranterMock = MockRepository.GenerateMock<GamingGroupAccessGranter>();
            gamingGroupCreator = MockRepository.GenerateMock<GamingGroupCreator>();
            gamingGroupController = new GamingGroupController(
                dataContext, 
                gamingGroupToGamingGroupViewModelTransformationMock, 
                gamingGroupRepositoryMock,
                gamingGroupAccessGranterMock,
                gamingGroupCreator);
            currentUser = new ApplicationUser()
            {
                Id = "user  id",
                CurrentGamingGroupId = 1315
            };
        }
    }
}