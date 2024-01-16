using Microsoft.EntityFrameworkCore;
using UNITEE_BACKEND.DatabaseContext;
using UNITEE_BACKEND.Entities;
using UNITEE_BACKEND.Enum;
using UNITEE_BACKEND.Models.Request;

namespace UNITEE_BACKEND.Services
{
    public class RatingService : IRatingService
    {
        private readonly AppDbContext context;
        public RatingService(AppDbContext dbcontext)
        {
            context = dbcontext;
        }

        public async Task<List<Rating>> GetRatingByUser(int userId)
            =>  await context.Ratings
                             .Where(r => r.UserId == userId)
                             .ToListAsync();

        public async Task<Rating> SubmitRatingProduct(RatingRequest request)
        {
            try
            {
                var rating = new Rating
                {
                    UserId = request.UserId,
                    Value = request.Value,
                    ProductId = request.ProductId,
                    SupplierId = request.SupplierId,
                    DateCreated = DateTime.Now,
                    Role = RatingRole.Product,
                    Comment = request.Comment,
                    IsRated = true
                };

                context.Ratings.Add(rating);
                await context.SaveChangesAsync();

                return rating;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        public async Task<Rating> SubmitRatingSupplier(RatingRequest request)
        {
            try
            {
                var rating = new Rating
                {
                    UserId = request.UserId,
                    Value = request.Value,
                    ProductId = request.ProductId,
                    SupplierId = request.SupplierId,
                    DateCreated = DateTime.Now,
                    Role = RatingRole.Supplier,
                    IsRated = true
                };

                context.Ratings.Add(rating);
                await context.SaveChangesAsync();

                return rating;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        public async Task<double> GetAverageProductRating(int productId)
        {
            try
            {
                var ratings = await context.Ratings
                                           .Where(r => r.ProductId == productId && r.Role == RatingRole.Product)
                                           .ToListAsync();

                if (!ratings.Any()) return 0;

                double sumOfRatings = ratings.Sum(r => r.Value);
                int numberOfRatings = ratings.Count;

                return sumOfRatings / numberOfRatings;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        public async Task<double> GetAverageSupplierRating(int supplierId)
        {
            try
            {
                var ratings = await context.Ratings
                                       .Where(r => r.SupplierId == supplierId && r.Role == RatingRole.Supplier)
                                       .ToListAsync();

                if (!ratings.Any()) return 0;

                double sumOfRatings = ratings.Sum(r => r.Value);
                double totalNumberOfRatings = ratings.Count();

                return totalNumberOfRatings == 0 ? 0 : sumOfRatings / totalNumberOfRatings;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        public async Task<IEnumerable<Rating>> GetTop3RatingsBySupplier(int supplierId)
           => await context.Ratings
                           .Include(r => r.Product)
                           .Where(r => r.SupplierId == supplierId && r.Role == RatingRole.Product)
                           .GroupBy(r => r.ProductId)
                           .Select(group => group.OrderByDescending(r => r.Value).First())
                           .Take(3)
                           .ToListAsync();

        public async Task<IEnumerable<Rating>> GetFeedbackByProduct(int productId)  
            => await context.Ratings
                            .Include(r => r.User)
                            .Where(r => r.ProductId == productId && r.Role == RatingRole.Product)
                            .OrderByDescending(r => r.DateCreated)
                            .ToListAsync();

    }
}
