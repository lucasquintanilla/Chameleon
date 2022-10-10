using Core.Data.Repositories;
using Core.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Voxed.WebApp.Constants;

namespace Voxed.WebApp.Services
{
    public interface IUserVoxActionService
    {
        Task<UserVoxAction> GetUserVoxActions(Guid voxId, Guid? userId);
        Task<string> ManageUserVoxAction(Guid userId, Guid voxId, string actionId);
    }

    public class UserVoxActionService : IUserVoxActionService
    {
        private readonly ILogger<UserVoxActionService> _logger;
        private readonly IVoxedRepository _voxedRepository;

        public UserVoxActionService(ILogger<UserVoxActionService> logger, IVoxedRepository voxedRepository)
        {
            _logger = logger;
            _voxedRepository = voxedRepository;
        }

        public async Task<UserVoxAction> GetUserVoxActions(Guid voxId, Guid? userId)
        {
            var userVoxAction = new UserVoxAction();

            if (userId == null) return userVoxAction;

            var actions = await _voxedRepository.UserVoxActions.GetByUserIdVoxId(userId.Value, voxId);
            if (actions == null) return userVoxAction;

            return actions;
        }

        public async Task<string> ManageUserVoxAction(Guid userId, Guid voxId, string actionId)
        {
            string actionResult;
            var userVoxAction = await _voxedRepository.UserVoxActions.GetByUserIdVoxId(userId, voxId);
            if (userVoxAction == null)
            {
                userVoxAction = new UserVoxAction()
                {
                    UserId = userId,
                    VoxId = voxId
                };

                actionResult = SetAction(userVoxAction, actionId);
                await _voxedRepository.UserVoxActions.Add(userVoxAction);
            }
            else
            {
                actionResult = SetAction(userVoxAction, actionId);
            }

            await _voxedRepository.SaveChangesAsync();
            return actionResult;
        }

        private string SetAction(UserVoxAction userVoxAction, string action)
        {
            switch (action)
            {
                case Constants.Action.Hide:
                    if (userVoxAction.IsHidden)
                    {
                        userVoxAction.IsHidden = false;
                        return "delete";
                    }
                    userVoxAction.IsHidden = true;
                    return "create";
                case Constants.Action.Favorite:
                    if (userVoxAction.IsFavorite)
                    {
                        userVoxAction.IsFavorite = false;
                        return "delete";
                    }
                    userVoxAction.IsFavorite = true;
                    return "create";
                case Constants.Action.Follow:
                    if (userVoxAction.IsFollowed)
                    {
                        userVoxAction.IsFollowed = false;
                        return "delete";
                    }
                    userVoxAction.IsFollowed = true;
                    return "create";
                default:
                    throw new Exception($"Invalid {nameof(action)}");
            }
        }
    }

    
}
