using Microsoft.EntityFrameworkCore;
using UNITEE_BACKEND.DatabaseContext;
using UNITEE_BACKEND.Entities;
using UNITEE_BACKEND.Models.Request;
using static System.Net.Mime.MediaTypeNames;

namespace UNITEE_BACKEND.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext context;

        public ProductService(AppDbContext dbcontext)
        {
            this.context = dbcontext;
        }

        public async Task<Product> AddProduct(ProductRequest request)
        {
            var imagePath = await ProductImage(request.Image);

            var newProduct = new Product
            {
                SupplierId = request.SupplierId,
                ProductTypeId = request.ProductTypeId,
                DepartmentId = request.DepartmentId,
                ProductName = request.ProductName,
                Description = request.Description,
                Sizes = string.Join(',', request.Sizes),
                Category = request.Category,
                Price = request.Price,
                Stocks = request.Stocks,
                Image = imagePath,
                IsActive = true
            };

            await context.Products.AddAsync(newProduct);
            await this.Save();

            return newProduct;

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
            => context.Products.AsEnumerable();

        public async Task<IEnumerable<Product>> GetProductsBySupplier(int supplierId)
        {
            var products = await context.Products
                .Where(product => product.SupplierId == supplierId)
                .ToListAsync();

            return products;
        }

        public async Task<Product> GetById(int id)
        {
            var e = await context.Products.Where(a => a.ProductId == id).FirstOrDefaultAsync();

            if (e == null)
                throw new Exception("Product Not Found");

            return e;
        }

        public async Task<Product> Save(Product request)
        {
            var e = await context.Products.AddAsync(request);
            await this.Save();
            return e.Entity;
        }

        public async Task<Product> Update(int productId, ProductRequest request)
        {
            var existingProduct = await context.Products.FindAsync(productId);

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

            // Check if sizes are present before joining them
            existingProduct.Sizes = request.Sizes != null
                ? string.Join(',', request.Sizes)
                : null;

            existingProduct.Category = request.Category;
            existingProduct.Price = request.Price;
            existingProduct.Stocks = request.Stocks;

            await this.Save();

            return existingProduct;
        }

        public async Task<Product> UpdateActivationStatus(int productId, bool isActive)
        {
            var existingProduct = await context.Products.FindAsync(productId);

            if (existingProduct == null)
            {
                throw new Exception("Product not found");
            }

            existingProduct.IsActive = isActive;
            await this.Save();

            return existingProduct;
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
