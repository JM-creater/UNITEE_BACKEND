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
            this.context = dbcontext;
            this._mapper = mapper;
        }

        public IEnumerable<SizeQuantity> GetAll()
            => context.SizeQuantities.AsEnumerable();

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
                await this.Save();

                return newSizeQuantity;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
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
                throw new Exception(e.Message);
            }
        }

        public async Task<Dictionary<string, int>> GetSizeQuantitiesForProduct(int productId)
        {
            try
            {
                var sizeQuantities = await context.SizeQuantities.Where(sq => sq.ProductId == productId).ToListAsync();

                var result = new Dictionary<string, int>();

                foreach (var sq in sizeQuantities)
                {
                    result[sq.Size] = sq.Id;
                }

                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    
        public async Task<SizeQuantity> Update(int id, UpdateSizeQuantityDto dto)
        {
            try
            {
                var sizequantity = await context.SizeQuantities.FirstOrDefaultAsync(a => a.Id == id);

                if (sizequantity == null)
                    throw new Exception("SizeQuantity Not Found");

                _mapper.Map(dto, sizequantity);

                await this.Save();

                return sizequantity;

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<SizeQuantity> Create(CreateSizeQuantityDto dto)
        {
            try
            {
                var sizeQuantity = _mapper.Map<SizeQuantity>(dto);
                await context.SizeQuantities.AddAsync(sizeQuantity);
                await this.Save();

                return sizeQuantity;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<SizeQuantity> DeleteSizeQuantity(int productId)
        {
            try
            {
                var sizeQuantity = await context.SizeQuantities
                                                .Where(p => p.ProductId == productId)
                                                .FirstOrDefaultAsync();

                if (sizeQuantity == null)
                    throw new KeyNotFoundException($"SizeQuantity with ID {productId} not found.");

                context.SizeQuantities.Remove(sizeQuantity);
                await this.Save();

                return sizeQuantity;
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
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
                throw new Exception(e.Message);
            }
        }

        // New Services
        public async Task<SizeQuantity> AddSizeToProduct(int productId, SizeQuantityDto dto)
        {
            try
            {
                var existingSize = await context.SizeQuantities
                    .FirstOrDefaultAsync(sq => sq.ProductId == productId && sq.Size == dto.Size);

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
                throw new Exception(e.Message);
            }
        }


        public async Task<bool> DeleteSizeFromProduct(int productId, string sizeLabel)
        {
            try
            {
                var sizeQuantity = await context.SizeQuantities
                .FirstOrDefaultAsync(a => a.ProductId == productId && a.Size == sizeLabel);

                if (sizeQuantity == null)
                    throw new Exception("Size for product not found");

                context.SizeQuantities.Remove(sizeQuantity);
                await this.Save();

                return true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
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
        
        public async Task<SizeQuantity> Save(SizeQuantity request)
        {
            var e = await context.SizeQuantities.AddAsync(request);
            await context.SaveChangesAsync();
            return e.Entity;
        }

        async Task<int> Save()
           => await context.SaveChangesAsync();

    }
}
