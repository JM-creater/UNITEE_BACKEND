using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UNITEE_BACKEND.DatabaseContext;
using UNITEE_BACKEND.Dto;
using UNITEE_BACKEND.Entities;
using UNITEE_BACKEND.Models.Request;

namespace UNITEE_BACKEND.Services
{
    public class SizeQuantityService : ISizeQuantityService
    {
        private readonly AppDbContext context;
        private readonly IMapper _mapper;

        public SizeQuantityService(AppDbContext dbcontext, IMapper mapper)
        {
            context = dbcontext;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SizeQuantity>> GetAll()
            => await context.SizeQuantities.ToListAsync();

        public async Task<SizeQuantity> AddSizeQuantity(SizeRequest request)
        {
            try
            {
                var newSizeQuantity = new SizeQuantity
                {
                    ProductId = request.ProductId,
                    Size = request.Size,
                    Quantity = request.Quantity
                };

                await context.SizeQuantities.AddAsync(newSizeQuantity);
                await context.SaveChangesAsync();

                return newSizeQuantity;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        public async Task<IEnumerable<SizeQuantity>> GetByProductId(int productId)
        {
            try
            {
                var sizesAndQuantities = await context.SizeQuantities
                                                      .Where(sq => sq.ProductId == productId)
                                                      .ToListAsync();

                if (sizesAndQuantities == null || sizesAndQuantities.Count == 0)
                {
                    throw new Exception("Size and Quantity Not Found");
                }

                return sizesAndQuantities;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        public async Task<Dictionary<string, int>> GetSizeQuantitiesForProduct(int productId)
        {
            try
            {
                var sizeQuantities = await context.SizeQuantities
                                                  .Where(sq => sq.ProductId == productId)
                                                  .ToListAsync();

                var result = new Dictionary<string, int>();

                foreach (var sq in sizeQuantities)
                {
                    result[sq.Size] = sq.Id;
                }

                return result;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }
    
        public async Task<SizeQuantity> Update(int id, UpdateSizeQuantityDto dto)
        {
            try
            {
                var sizequantity = await context.SizeQuantities
                                                .Where(a => a.Id == id)
                                                .FirstOrDefaultAsync();

                if (sizequantity == null)
                    throw new InvalidOperationException("SizeQuantity Not Found");

                _mapper.Map(dto, sizequantity);

                await context.SaveChangesAsync();

                return sizequantity;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        public async Task<SizeQuantity> Create(CreateSizeQuantityDto dto)
        {
            try
            {
                var sizeQuantity = _mapper.Map<SizeQuantity>(dto);

                await context.SizeQuantities.AddAsync(sizeQuantity);

                await context.SaveChangesAsync();

                return sizeQuantity;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        public async Task<IEnumerable<GetSizeQuantityByIdDto>> GetSizesByProductId(int productId)
        {
            try
            {
                var sizes = await context.SizeQuantities
                                         .Where(sq => sq.ProductId == productId)
                                         .ToListAsync();

                return _mapper.Map<IEnumerable<GetSizeQuantityByIdDto>>(sizes);
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        public async Task<SizeQuantity> AddSizeToProduct(int productId, SizeQuantityDto dto)
        {
            try
            {
                var existingSize = await context.SizeQuantities
                                                .Where(sq => sq.ProductId == productId && sq.Size == dto.Size)
                                                .FirstOrDefaultAsync();

                SizeQuantity sizeQuantity;

                if (existingSize != null)
                {
                    existingSize.Quantity = dto.Quantity;
                    sizeQuantity = existingSize; 
                }
                else
                {
                    sizeQuantity = new SizeQuantity
                    {
                        ProductId = productId,
                        Size = dto.Size,
                        Quantity = dto.Quantity
                    };
                    context.SizeQuantities.Add(sizeQuantity);
                }

                await context.SaveChangesAsync();

                return sizeQuantity;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        public async Task<SizeQuantity> UpdateQuantity(int id, int productId, string size, int newQuantity)
        {
            try
            {
                var sizeQuantity = await context.SizeQuantities
                                                .Where(a => a.Id == id && a.ProductId == productId && a.Size == size)
                                                .FirstOrDefaultAsync();

                if (sizeQuantity == null)
                    throw new InvalidOperationException("Size and Quantity not Found");

                sizeQuantity.Quantity = newQuantity;

                await context.SaveChangesAsync();

                return sizeQuantity;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        public async Task<SizeQuantity> DeleteSizeQuantity(int id)
        {
            try
            {
                var sizeQuantity = await context.SizeQuantities
                                                .Where(p => p.Id == id)
                                                .FirstOrDefaultAsync();

                if (sizeQuantity == null)
                    throw new InvalidOperationException("SizeQuantity not found.");

                context.SizeQuantities.Remove(sizeQuantity);
                await context.SaveChangesAsync();

                return sizeQuantity;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }
    }
}
