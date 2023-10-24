using UNITEE_BACKEND.Entities;
using UNITEE_BACKEND.Models.Request;

namespace UNITEE_BACKEND.Services
{
    public interface IRatingService
    {
        public Task<Rating> SubmitRatingProduct(RatingRequest request);
        public Task<Rating> SubmitRatingSupplier(RatingRequest request);
    }
}
