using Core.Data.Repositories;
using Core.Extensions;
using Core.Services.Moderation.Models;
using Core.Services.Telegram;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Voxed.WebApp.Services.Moderation
{
    public interface IModerationService
    {
        Task Report(ReportRequest request);
    }

    public class ModerationService : IModerationService
    {
        private readonly ILogger<ModerationService> _logger;
        private readonly IBlogRepository _blogRepository;
        private readonly ITelegramService _telegramService;

        public ModerationService(
            ILogger<ModerationService> logger,
            IBlogRepository blogRepository,
            ITelegramService telegramService)
        {
            _logger = logger;
            _blogRepository = blogRepository;
            _telegramService = telegramService;
        }

        public async Task Report(ReportRequest request)
        {
            string message = null;

            switch (request.ContentType)
            {
                case 0:
                    var comment = await _blogRepository.Comments.GetByHash(request.ContentId);
                    message = $"NUEVA DENUNCIA \n Reason: {request.Reason}. \n https://voxed.club/vox/{comment.PostId.ToShortString()}#{comment.Hash}";
                    break;

                case 1:
                    message = $"NUEVA DENUNCIA \n Reason: {request.Reason}. \n https://voxed.club/vox/{new Guid(request.ContentId).ToShortString()}";
                    break;
            }

            // TODO Agregar botones de acciones a tomar
            await _telegramService.SendMessage(message);
        }
    }
}
