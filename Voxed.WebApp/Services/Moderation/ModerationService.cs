﻿using Core.Data.Repositories;
using Core.Extensions;
using Core.Services.Telegram;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Voxed.WebApp.Models;

namespace Voxed.WebApp.Services.Moderation
{
    public interface IModerationService
    {
        Task<ReportResponse> Report(ReportRequest request);
    }

    public class ModerationService : IModerationService
    {
        private readonly ILogger<ModerationService> _logger;
        private readonly IVoxedRepository _voxedRepository;
        private readonly ITelegramService _telegramService;

        public ModerationService(
            ILogger<ModerationService> logger,
            IVoxedRepository voxedRepository,
            ITelegramService telegramService)
        {
            _logger = logger;
            _voxedRepository = voxedRepository;
            _telegramService = telegramService;
        }

        public async Task<ReportResponse> Report(ReportRequest request)
        {
            var response = new ReportResponse();

            try
            {
                string message = null;

                switch (request.ContentType)
                {
                    case 0:
                        var comment = await _voxedRepository.Comments.GetByHash(request.ContentId);
                        message = $"NUEVA DENUNCIA \n Reason: {request.Reason}. \n https://voxed.club/vox/{comment.VoxId.ToShortString()}#{comment.Hash}";
                        break;

                    case 1:
                        message = $"NUEVA DENUNCIA \n Reason: {request.Reason}. \n https://voxed.club/vox/{new Guid(request.ContentId).ToShortString()}";
                        break;
                }

                // TODO Agregar botones de acciones a tomar
                await _telegramService.SendMessage(message);

                response.Status = true;
                response.Swal = "Gracias por enviarnos tu denuncia";

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);

                response.Status = false;
                response.Swal = "Hubo un error al enviar tu denuncia";
            }

            return response;
        }
    }
}