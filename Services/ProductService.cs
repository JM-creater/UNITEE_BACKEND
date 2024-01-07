using Microsoft.EntityFrameworkCore;
using UNITEE_BACKEND.DatabaseContext;
using UNITEE_BACKEND.Dto;
using UNITEE_BACKEND.Entities;
using UNITEE_BACKEND.Models.ImageDirectory;
using UNITEE_BACKEND.Models.Request;

namespace UNITEE_BACKEND.Services
{
    public class ProductService : IProductService
    {

        private readonly AppDbContext context;

        public ProductService(AppDbContext dbcontext)
        {
            context = dbcontext;
        }

        public async Task<int> AddProduct(ProductRequest request)
        {
            try
            {
                var existingProductName = await context.Products
                                                       .Where(p => p.ProductName == request.ProductName)
                                                       .FirstOrDefaultAsync();

                if (existingProductName != null)
                {
                    throw new ArgumentException("Product name already exists");
                }

                var imagePath = await new ImagePathConfig().SaveProductImage(request.Image);
                var frontImagePath = await new ImagePathConfig().SaveFrontImage(request.FrontViewImage);
                var sideImagePath = await new ImagePathConfig().SaveSideImage(request.SideViewImage);
                var backImagePath = await new ImagePathConfig().SaveBackImage(request.BackViewImage);
                var sizeGuidePath = await new ImagePathConfig().SaveSizeGuide(request.SizeGuide);

                var newProduct = new Product
                {
                    SupplierId = request.SupplierId,
                    ProductTypeId = request.ProductTypeId,
                    ProductName = request.ProductName,
                    Description = request.Description,
                    Category = request.Category,
                    Price = request.Price,
                    Image = imagePath,
                    FrontViewImage = frontImagePath,
                    SideViewImage = sideImagePath,
                    BackViewImage = backImagePath,
                    SizeGuide = sizeGuidePath,
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

                context.Products.Add(newProduct);
                await context.SaveChangesAsync();

                return newProduct.ProductId;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        public async Task<IEnumerable<Product>> RecommendProducts(string search, int userId)
        {
            var searchToLower = search.ToLower();
            var user = await context.Users
                                    .Include(u => u.Department)
                                    .Where(u => u.Id == userId)
                                    .FirstOrDefaultAsync();

            return await context.Products
                                .Include(p => p.Sizes)
                                .Include(p => p.ProductDepartments).ThenInclude(pd => pd.Department)
                                .Include(p => p.ProductType)
                                .Include(p => p.Rating)
                                .Where(p => p.ProductName.ToLower().Contains(searchToLower) ||
                                            p.Description.ToLower().Contains(searchToLower) ||
                                            p.Sizes.Any(s => s.Size.ToLower().Contains(searchToLower)) ||
                                            p.ProductDepartments.Any(pd => pd.Department.Department_Name.ToLower().Contains(searchToLower)) ||
                                            p.ProductType.Product_Type.ToLower().Contains(searchToLower))
                                .Where(p => p.ProductDepartments.Any(pd => pd.DepartmentId == user.DepartmentId))
                                .OrderByDescending(p => p.Rating.Value)
                                .Take(5)
                                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> RecommendProductsPurchase(int userId)
        {
            var userDepartmentId = await context.Users
                                                .Where(u => u.Id == userId)
                                                .Select(u => u.DepartmentId)
                                                .FirstOrDefaultAsync();

            if (!userDepartmentId.HasValue)
            {
                return Enumerable.Empty<Product>();
            }

            var purchasedProductIds = await context.OrderItems
                                                    .Where(oi => oi.Order.UserId == userId && !oi.Order.IsDeleted)
                                                    .Select(oi => oi.ProductId)
                                                    .Distinct()
                                                    .ToListAsync();

            var purchasedProductsInfo = await context.Products
                                                      .Where(p => purchasedProductIds.Contains(p.ProductId))
                                                      .Select(p => new { p.Category, p.ProductTypeId, p.SupplierId })
                                                      .Distinct()
                                                      .ToListAsync();

            var categories = purchasedProductsInfo.Select(p => p.Category).Distinct();
            var productTypes = purchasedProductsInfo.Select(p => p.ProductTypeId).Distinct();
            var suppliers = purchasedProductsInfo.Select(p => p.SupplierId).Distinct();

            var recommendedProducts = await context.Products
                                                    .Where(p => !purchasedProductIds.Contains(p.ProductId) &&
                                                                productTypes.Contains(p.ProductTypeId) &&
                                                                categories.Contains(p.Category) &&
                                                                suppliers.Contains(p.SupplierId) &&
                                                                p.ProductDepartments.Any(pd => pd.DepartmentId == userDepartmentId.Value))
                                                    .ToListAsync();

            return recommendedProducts.Distinct().ToList();
        }

        public async Task<IEnumerable<Product>> GetSearchProductByUserDepartment(int userId)
        {
            try
            {
                var user = await context.Users
                                    .Include(u => u.Department)
                                    .Where(u => u.Id == userId)
                                    .FirstOrDefaultAsync();

                if (user?.DepartmentId == null)
                {
                    return Enumerable.Empty<Product>();
                }

                return await context.Products
                                    .Where(p => p.ProductDepartments.Any(pd => pd.DepartmentId == user.DepartmentId))
                                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        public async Task<int> GetQuantityBySize(int productId, int sizeQuantityId)
        {
            try
            {
                var sizeQuantity = await context.SizeQuantities
                                        .Where(sq => sq.ProductId == productId && sq.Id == sizeQuantityId)
                                        .FirstOrDefaultAsync();

                return sizeQuantity?.Quantity ?? 0;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        public float CalculateProductRevenue(int productId)
        {
            float totalRevenue = 0;
            var productOrderItems = context.OrderItems.Where(oi => oi.ProductId == productId);

            foreach (var item in productOrderItems)
            {
                totalRevenue += item.Quantity * item.Product.Price;
            }

            return totalRevenue;
        }

        public async Task<IEnumerable<Product>> GetTopSellingProducts(int topCount)
        {
            try
            {
                var products = await context.Products.ToListAsync();

                return products.OrderByDescending(p => CalculateProductRevenue(p.ProductId))
                              .Take(topCount);
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        public float CalculateShopProductRevenue(IEnumerable<OrderItem> orderItems, int productId)
        {
            float totalRevenue = 0;

            foreach (var item in orderItems.Where(oi => oi.ProductId == productId))
            {
                totalRevenue += item.Quantity * item.Product.Price;
            }

            return totalRevenue;
        }

        public async Task<IEnumerable<Product>> GetTopSellingProductsByShop(int shopId, int topCount)
        {
            try
            {
                var products = await context.Products.Where(p => p.SupplierId == shopId).ToListAsync();
                var orderItems = await context.OrderItems.Where(oi => oi.Product.SupplierId == shopId).ToListAsync();

                return products
                    .Select(p => new
                    {
                        Product = p,
                        Revenue = CalculateShopProductRevenue(orderItems, p.ProductId)
                    })
                    .OrderByDescending(x => x.Revenue)
                    .Take(topCount)
                    .Select(x => x.Product)
                    .ToList();
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        public async Task<IEnumerable<Product>> GetAll()
        {
            try
            {
                var products = await context.Products
                                            .Include(p => p.Supplier)
                                            .Include(p => p.Sizes)
                                            .Include(p => p.ProductType)
                                            .Include(p => p.ProductDepartments)
                                            .ToListAsync();

                return products;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        public async Task<IEnumerable<Product>> GetProductsByShopId(int shopId)
            => await context.Products
                            .Include(s => s.Sizes)
                            .Where(p => p.SupplierId == shopId)
                            .ToListAsync();

        public async Task<IEnumerable<Product>> GetProductsByShopIdAndDepartmentId(int shopId, int departmentId)
            => await context.Products
                            .Include(p => p.ProductDepartments)
                               .ThenInclude(pd => pd.Department)
                            .Include(p => p.Sizes)
                            .Where(p => p.SupplierId == shopId &&
                                      p.ProductDepartments.Any(pd => pd.DepartmentId == departmentId))
                            .ToListAsync();

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
                    FrontViewImage = product.FrontViewImage,
                    SideViewImage = product.SideViewImage,
                    BackViewImage = product.BackViewImage,
                    SizeGuide = product.SizeGuide,
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
                    existingProduct.Image = await new ImagePathConfig().SaveProductImage(request.Image); 
                }

                if (request.FrontViewImage != null)
                {
                    existingProduct.FrontViewImage = await new ImagePathConfig().SaveFrontImage(request.FrontViewImage);
                }

                if (request.SideViewImage != null)
                {
                    existingProduct.SideViewImage = await new ImagePathConfig().SaveSideImage(request.SideViewImage);
                }

                if (request.BackViewImage != null)
                {
                    existingProduct.BackViewImage = await new ImagePathConfig().SaveBackImage(request.BackViewImage);
                }

                if (request.SizeGuide != null)
                {
                    existingProduct.SizeGuide = await new ImagePathConfig().SaveSizeGuide(request.SizeGuide);
                }

                existingProduct.ProductTypeId = request.ProductTypeId;
                existingProduct.ProductName = request.ProductName;
                existingProduct.Description = request.Description;
                existingProduct.Category = request.Category;
                existingProduct.Price = request.Price;

                // Update sizes
                foreach (var sizeQuantityDto in request.Sizes)
                {
                    var existingSize = existingProduct.Sizes
                                                      .Where(s => s.Size == sizeQuantityDto.Size)
                                                      .FirstOrDefault();

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

                context.Products.Update(existingProduct);
                await context.SaveChangesAsync();

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

                context.Products.Update(existingProduct);
                await context.SaveChangesAsync();

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

        public async Task<int> GetQuantityBySize(int productId, int sizeQuantityId)
        {
            try
            {
                var sizeQuantity = await context.SizeQuantities
                                                .Where(sq => sq.ProductId == productId
                                                           && sq.Id == sizeQuantityId)
                                                .FirstOrDefaultAsync();

                return sizeQuantity?.Quantity ?? 0;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        async Task<int> Save()
            => await context.SaveChangesAsync();

    }
}
