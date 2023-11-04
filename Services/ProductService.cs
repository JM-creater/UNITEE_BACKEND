using AutoMapper;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
                var existingProduct = await context.Products
                                                   .Where(p => p.ProductName == request.ProductName)
                                                   .FirstOrDefaultAsync();

                if (existingProduct != null)
                {
                    throw new ArgumentException("Product already exists");
                }

                var imagePath = await ProductImage(request.Image);

                var newProduct = new Product
                {
                    SupplierId = request.SupplierId,
                    ProductTypeId = request.ProductTypeId,
                    ProductName = request.ProductName,
                    Description = request.Description,
                    Category = request.Category,
                    Price = request.Price,
                    Image = imagePath,
                    IsActive = true
                };

                foreach (var departmentId in request.DepartmentIds)
                {
                    var department = await context.Departments.FindAsync(departmentId);
                    if (department != null)
                    {
                        newProduct.ProductDepartments.Add(new ProductDepartment { Department = department });
                    }
                    else
                    {
                        continue;
                    }
                }

                await context.Products.AddAsync(newProduct);
                await context.SaveChangesAsync();

                return newProduct.ProductId;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
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

        public async Task<IEnumerable<Product>> RecommendProducts(string description, string size, string departmentName, string productType, string productName)
        {
            try
            {
                var products = await context.Products
                                    .Include(p => p.Sizes)
                                    .Include(p => p.ProductType)
                                    .Where(p => p.Description.Contains(description) &&
                                                p.Sizes.Any(s => s.Size == size) &&
                                                p.ProductType.Product_Type == productType && 
                                                p.ProductName == productName)
                                    .ToListAsync();
                return products;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
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
                  .Include(p => p.ProductDepartments)
                      .ThenInclude(pd => pd.Department)
                  .Include(p => p.Sizes)
                  .Where(p => p.SupplierId == shopId &&
                              p.ProductDepartments.Any(pd => pd.DepartmentId == departmentId))
                  .ToList(); 
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
                                            .Include(product => product.ProductDepartments)
                                                .ThenInclude(pd => pd.Department)
                                            .Where(product => product.SupplierId == supplierId)
                                            .ToListAsync();

                var productDto = products.Select(product => new ProductWithSizeQuantityDto
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    ProductTypeId = product.ProductTypeId,
                    Description = product.Description,
                    Category = product.Category,
                    Price = product.Price,
                    Image = product.Image,
                    IsActive = product.IsActive,
                    Sizes = product.Sizes.Select(size => new SizeQuantityDto
                    {
                        Id = size.Id,
                        Size = size.Size,
                        Quantity = size.Quantity
                    }).ToList(),
                    Departments = product.ProductDepartments.Select(pd => new DepartmentDto
                    {
                        DepartmentId = pd.DepartmentId,
                        Department_Name = pd.Department.Department_Name
                    }).ToList()
                }).ToList();

                return productDto;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        public async Task<IEnumerable<Product>> GetProductsByDepartment(int departmentId)
        {
            try
            {
                var products = await context.Products
                                            .Include(a => a.Sizes)
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
            try
            {
                var existingProduct = await context.Products
                                                  .Include(p => p.Sizes)
                                                  .Include(p => p.ProductDepartments)
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

                // Remove Departments
                var departmentsToRemove = existingProduct.ProductDepartments
                                                        .Where(pd => !request.DepartmentIds.Contains(pd.DepartmentId))
                                                        .ToList();

                foreach (var toRemove in departmentsToRemove)
                {
                    existingProduct.ProductDepartments.Remove(toRemove);
                    context.ProductDepartments.Remove(toRemove); 
                }

                // Update Departments
                foreach (var departmentId in request.DepartmentIds)
                {
                    if (!existingProduct.ProductDepartments.Any(pd => pd.DepartmentId == departmentId))
                    {
                        existingProduct.ProductDepartments.Add(new ProductDepartment
                        {
                            ProductId = existingProduct.ProductId,
                            DepartmentId = departmentId
                        });
                    }
                }

                await context.SaveChangesAsync();

                return existingProduct;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
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
