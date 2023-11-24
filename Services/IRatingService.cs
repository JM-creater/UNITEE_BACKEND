using UNITEE_BACKEND.Entities;
using UNITEE_BACKEND.Enum;
using UNITEE_BACKEND.Models.Request;

namespace UNITEE_BACKEND.Services
{
    public interface IRatingService
    {
        public Task<Rating> SubmitRatingProduct(RatingRequest request);
        public Task<Rating> SubmitRatingSupplier(RatingRequest request);
        public Task<List<Rating>> GetRatingByUser(int userId);
        public Task<double> GetAverageProductRating(int productId);
        public Task<double> GetAverageSupplierRating(int supplierId);
    }
}
