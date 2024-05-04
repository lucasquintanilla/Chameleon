using Core.Data.Repositories;
using Core.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Core.Services
{
    public interface IUserVoxActionService
    {
        Task<UserPostAction> GetUserVoxActions(Guid voxId, Guid? userId);
        Task<string> ManageUserVoxAction(Guid userId, Guid voxId, string actionId);
    }

    public class UserVoxActionService : IUserVoxActionService
    {
        private readonly ILogger<UserVoxActionService> _logger;
        private readonly IBlogRepository _blogRepository;

        public UserVoxActionService(
            ILogger<UserVoxActionService> logger, 
            IBlogRepository blogRepository)
        {
            _logger = logger;
            _blogRepository = blogRepository;
        }

        public async Task<UserPostAction> GetUserVoxActions(Guid voxId, Guid? userId)
        {
            var userVoxAction = new UserPostAction();

            if (userId == null) return userVoxAction;

            var actions = await _blogRepository.UserPostActions.GetByUserIdPostId(userId.Value, voxId);
            if (actions == null) return userVoxAction;

            return actions;
        }

        public async Task<string> ManageUserVoxAction(Guid userId, Guid voxId, string actionId)
        {
            string actionResult;
            var userVoxAction = await _blogRepository.UserPostActions.GetByUserIdPostId(userId, voxId);
            if (userVoxAction == null)
            {
                userVoxAction = new UserPostAction()
                {
                    UserId = userId,
                    PostId = voxId
                };

                actionResult = SetAction(userVoxAction, actionId);
                await _blogRepository.UserPostActions.Add(userVoxAction);
            }
            else
            {
                actionResult = SetAction(userVoxAction, actionId);
            }

            await _blogRepository.SaveChangesAsync();
            return actionResult;
        }

        private string SetAction(UserPostAction userVoxAction, string actionId)
        {
            switch (actionId)
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
                    throw new Exception($"Invalid {nameof(actionId)}");
            }
        }
    }


}
