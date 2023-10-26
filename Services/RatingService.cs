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
                    Role = RatingRole.Product
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
                    Role = RatingRole.Supplier
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
    }
}
