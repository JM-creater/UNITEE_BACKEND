using AutoMapper;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using UNITEE_BACKEND.DatabaseContext;
using UNITEE_BACKEND.Dto;
using UNITEE_BACKEND.Entities;
using UNITEE_BACKEND.Models.Request;
using static System.Net.Mime.MediaTypeNames;

namespace UNITEE_BACKEND.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext context;
        private readonly IMapper _mapper;

        public ProductService(AppDbContext dbcontext, IMapper mapper)
        {
            this.context = dbcontext;
            this._mapper = mapper;
        }

        public async Task<int> AddProduct(ProductRequest request)
        {
            try
            {
                var imagePath = await ProductImage(request.Image);

                var newProduct = new Product
                {
                    SupplierId = request.SupplierId,
                    ProductTypeId = request.ProductTypeId,
                    DepartmentId = request.DepartmentId,
                    ProductName = request.ProductName,
                    Description = request.Description,
                    Category = request.Category,
                    Price = request.Price,
                    Image = imagePath,
                    IsActive = true
                };

                await context.Products.AddAsync(newProduct);
                await this.Save();

                return newProduct.ProductId;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<string?> ProductImage(IFormFile? imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                return null;

            string folder = Path.Combine(Directory.GetCurrentDirectory(), "ProductImages");
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return Path.Combine("ProductImages", fileName);
        }

        public IEnumerable<Product> GetAll()
        {
            try
            {
                var products = context.Products.Include(a => a.Sizes).AsEnumerable();

                return products;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public IEnumerable<Product> GetProductsByShopId(int shopId)
        {
            return context.Products.Include(s => s.Sizes).Where(p => p.SupplierId == shopId).AsEnumerable();
        }

        public IEnumerable<Product> GetProductsByShopIdAndDepartmentId(int shopId, int departmentId)
        {
            return context.Products
                        .Include(s => s.Sizes)
                        .Where(p => p.SupplierId == shopId && p.DepartmentId == departmentId)
                        .AsEnumerable();
        }

        public async Task<Product> GetById(int productId)
        {
            try
            {
                var product = await context.Products
                                           .Include(x => x.Sizes)
                                           .FirstOrDefaultAsync(a => a.ProductId == productId);

                if (product == null)
                    throw new Exception("Product Not Found");

                return product;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<IEnumerable<ProductWithSizeQuantityDto>> GetProductsBySupplier(int supplierId)
        {
            try
            {
                var products = await context.Products
                                            .Include(product => product.Sizes)
                                            .Where(product => product.SupplierId == supplierId)
                                            .ToListAsync();

                var productDto = products.Select(product => new ProductWithSizeQuantityDto
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    ProductTypeId = product.ProductTypeId,
                    DepartmentId = product.DepartmentId,
                    Category = product.Category,
                    Price = product.Price,
                    Image = product.Image,
                    IsActive = product.IsActive,
                    Sizes = product.Sizes.Select(size => new SizeQuantityDto
                    {
                        Size = size.Size,
                        Quantity = size.Quantity
                    })
                }).ToList();

                return productDto;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<IEnumerable<Product>> GetProductsByDepartment(int departmentId)
        {
            try
            {
                var products = await context.Products
                                            .Include(a => a.Sizes)
                                            .Where(p => p.DepartmentId == departmentId)
                                            .ToListAsync();

                return products;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<Product> UpdateProduct(int productId, ProductUpdateRequest request)
        {
            var existingProduct = await context.Products
                                               .Include(p => p.Sizes) 
                                               .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (existingProduct == null)
            {
                throw new Exception("Product not found");
            }

            if (request.Image != null)
            {
                existingProduct.Image = await ProductImage(request.Image);
            }

            existingProduct.ProductTypeId = request.ProductTypeId;
            existingProduct.DepartmentId = request.DepartmentId;
            existingProduct.ProductName = request.ProductName;
            existingProduct.Description = request.Description;
            existingProduct.Category = request.Category;
            existingProduct.Price = request.Price;

            // Update sizes
            foreach (var sizeQuantityDto in request.Sizes)
            {
                var existingSize = existingProduct.Sizes.FirstOrDefault(s => s.Size == sizeQuantityDto.Size);

                if (existingSize != null)
                {
                    existingSize.Quantity = sizeQuantityDto.Quantity;
                }
                else
                {
                    existingProduct.Sizes.Add(new SizeQuantity
                    {
                        Size = sizeQuantityDto.Size,
                        Quantity = sizeQuantityDto.Quantity
                    });
                }
            }

            await this.Save();

            return existingProduct;
        }

        public async Task<Product> UpdateActivationStatus(int productId)
        {
            try
            {
                var existingProduct = await context.Products
                                                .Where(a => a.ProductId == productId)
                                                .FirstOrDefaultAsync();

                if (existingProduct == null)
                    throw new Exception("Product not found");

                existingProduct.IsActive = true;

                await this.Save();

                return existingProduct;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<Product> UpdateDectivationStatus(int productId)
        {
            try
            {
                var existingProduct = await context.Products
                                                .Where(a => a.ProductId == productId)
                                                .FirstOrDefaultAsync();

                if (existingProduct == null)
                    throw new Exception("Product not found");

                existingProduct.IsActive = false;

                await this.Save();

                return existingProduct;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<Product> Delete(int id)
        {
            var e = await this.GetById(id);
            var result = context.Remove(e);
            await Save();
            return result.Entity;
        }

        async Task<int> Save()
            => await context.SaveChangesAsync();

    }
}
