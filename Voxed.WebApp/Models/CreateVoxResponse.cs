using Core.Shared;
using System;

namespace Voxed.WebApp.Models
{
    public class CreateVoxResponse : BaseResponse
    {
        public string VoxHash { get; set; }
        public static CreateVoxResponse Succesful(Guid voxId)
        {
            return new CreateVoxResponse()
            {
                VoxHash = GuidConverter.ToShortString(voxId),
                Status = true
            };
        }

        public static CreateVoxResponse Failure(string errorMessage)
        {
            return new CreateVoxResponse()
            {
                Swal = errorMessage,
                Status = false
            };
        }
    }
}
