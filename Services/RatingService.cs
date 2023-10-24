using UNITEE_BACKEND.DatabaseContext;
using UNITEE_BACKEND.Entities;
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

        public async Task<Rating> SubmitRatingProduct(RatingRequest request)
        {
            try
            {
                var rating = new Rating
                {
                    UserId = request.UserId,
                    Value = request.Value,
                    DateCreated = DateTime.Now
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
                    DateCreated = DateTime.Now
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
