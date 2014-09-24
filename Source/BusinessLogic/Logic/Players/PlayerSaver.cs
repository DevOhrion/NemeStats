﻿using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using BusinessLogic.DataAccess;
using BusinessLogic.EventTracking;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.DataAccess.Repositories;

namespace BusinessLogic.Logic.Players
{
    public class PlayerSaver : IPlayerSaver
    {
        private IDataContext dataContext;
        private IPlayerRepository playerRepository;
        private NemeStatsEventTracker eventTracker;

        public PlayerSaver(IDataContext dataContext, NemeStatsEventTracker eventTracker, IPlayerRepository playerRepository)
        {
            this.dataContext = dataContext;
            this.eventTracker = eventTracker;
            this.playerRepository = playerRepository;
        }

        public Player Save(Player player, ApplicationUser currentUser)
        {
            ValidatePlayerIsNotNull(player);
            ValidatePlayerNameIsNotNullOrWhiteSpace(player.Name);

            bool isNewPlayer = !player.AlreadyInDatabase();
            try
            {
                Player newPlayer = dataContext.Save<Player>(player, currentUser);
                dataContext.CommitAllChanges();

                if (isNewPlayer)
                {
                    new Task(() => eventTracker.TrackPlayerCreation(currentUser)).Start();
                }else
                {
                    if(!player.Active)
                    {
                        List<int> playerIdsToRecalculate = (from thePlayer in dataContext.GetQueryable<Player>()
                                                            where thePlayer.Active == true
                                                            && thePlayer.Nemesis.NemesisPlayerId == player.Id
                                                            select thePlayer.Id).ToList();

                        foreach (int playerId in playerIdsToRecalculate)
                        {
                            playerRepository.RecalculateNemesis(playerId, currentUser);
                        }
                    }
                }

                return newPlayer;
            }
            catch (DbUpdateException exp)
            {
                    
                throw exp;
            }
        }

        private static void ValidatePlayerIsNotNull(Player player)
        {
            if (player == null)
            {
                throw new ArgumentNullException("player");
            }
        }

        private static void ValidatePlayerNameIsNotNullOrWhiteSpace(string playerName)
        {
            if (string.IsNullOrWhiteSpace(playerName))
            {
                throw new ArgumentNullException("playerName");
            }
        }
    }
}