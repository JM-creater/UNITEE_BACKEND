namespace UNITEE_BACKEND.Models.ImageDirectory;

public class ImagePathConfig
{

    public async Task<string?> SaveImage(IFormFile? imageFile)
    {
        if (imageFile == null || imageFile.Length == 0)
            return null;

        string mainFolder = Path.Combine(Directory.GetCurrentDirectory(), "PathImages");
        string subFolder = Path.Combine(mainFolder, "Images");

        if (!Directory.Exists(mainFolder))
        {
            Directory.CreateDirectory(mainFolder);
        }
        if (!Directory.Exists(subFolder))
        {
            Directory.CreateDirectory(subFolder);
        }

        var fileName = Path.GetFileName(imageFile.FileName);
        var filePath = Path.Combine(subFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await imageFile.CopyToAsync(stream);
        }

        return Path.Combine("PathImages", "Images", fileName);
    }

    public async Task<string?> SaveStudyLoad(IFormFile? imageFile)
    {
        if (imageFile == null || imageFile.Length == 0)
            return null;

        string mainFolder = Path.Combine(Directory.GetCurrentDirectory(), "PathImages");
        string subFolder = Path.Combine(mainFolder, "StudyLoad");

        if (!Directory.Exists(mainFolder))
        {
            Directory.CreateDirectory(mainFolder);
        }
        if (!Directory.Exists(subFolder))
        {
            Directory.CreateDirectory(subFolder);
        }

        var fileName = Path.GetFileName(imageFile.FileName);
        var filePath = Path.Combine(subFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await imageFile.CopyToAsync(stream);
        }

        return Path.Combine("PathImages", "StudyLoad", fileName);
    }

    public async Task<string?> SaveProductImage(IFormFile? imageFile)
    {
        if (imageFile == null || imageFile.Length == 0)
            return null;

        string mainFolder = Path.Combine(Directory.GetCurrentDirectory(), "PathImages");
        string subFolder = Path.Combine(mainFolder, "ProductImages");

        if (!Directory.Exists(mainFolder))
        {
            Directory.CreateDirectory(mainFolder);
        }
        if (!Directory.Exists(subFolder))
        {
            Directory.CreateDirectory(subFolder);
        }

        var fileName = Path.GetFileName(imageFile.FileName);
        var filePath = Path.Combine(subFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await imageFile.CopyToAsync(stream);
        }

        return Path.Combine("PathImages", "ProductImages", fileName);
    }

    public async Task<string?> SaveSupplierImage(IFormFile? imageFile)
    {
        if (imageFile == null || imageFile.Length == 0)
            return null;

        string mainFolder = Path.Combine(Directory.GetCurrentDirectory(), "PathImages");
        string subFolder = Path.Combine(mainFolder, "SupplierImage");

        if (!Directory.Exists(mainFolder))
        {
            Directory.CreateDirectory(mainFolder);
        }
        if (!Directory.Exists(subFolder))
        {
            Directory.CreateDirectory(subFolder);
        }

        var fileName = Path.GetFileName(imageFile.FileName);
        var filePath = Path.Combine(subFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await imageFile.CopyToAsync(stream);
        }

        return Path.Combine("PathImages", "SupplierImage", fileName);
    }

    public async Task<string?> SaveBIR(IFormFile? imageFile)
    {
        if (imageFile == null || imageFile.Length == 0)
            return null;

        string mainFolder = Path.Combine(Directory.GetCurrentDirectory(), "PathImages");
        string subFolder = Path.Combine(mainFolder, "BIR");

        if (!Directory.Exists(mainFolder))
        {
            Directory.CreateDirectory(mainFolder);
        }
        if (!Directory.Exists(subFolder))
        {
            Directory.CreateDirectory(subFolder);
        }

        var fileName = Path.GetFileName(imageFile.FileName);
        var filePath = Path.Combine(subFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await imageFile.CopyToAsync(stream);
        }

        return Path.Combine("PathImages", "BIR", fileName);
    }

    public async Task<string?> SaveCityPermit(IFormFile? imageFile)
    {
        if (imageFile == null || imageFile.Length == 0)
            return null;

        string mainFolder = Path.Combine(Directory.GetCurrentDirectory(), "PathImages");
        string subFolder = Path.Combine(mainFolder, "CityPermit");

        if (!Directory.Exists(mainFolder))
        {
            Directory.CreateDirectory(mainFolder);
        }
        if (!Directory.Exists(subFolder))
        {
            Directory.CreateDirectory(subFolder);
        }

        var fileName = Path.GetFileName(imageFile.FileName);
        var filePath = Path.Combine(subFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await imageFile.CopyToAsync(stream);
        }

        return Path.Combine("PathImages", "CityPermit", fileName);
    }

    public async Task<string?> SaveSchoolPermit(IFormFile? imageFile)
    {
        if (imageFile == null || imageFile.Length == 0)
            return null;

        string mainFolder = Path.Combine(Directory.GetCurrentDirectory(), "PathImages");
        string subFolder = Path.Combine(mainFolder, "SchoolPermit");

        if (!Directory.Exists(mainFolder))
        {
            Directory.CreateDirectory(mainFolder);
        }
        if (!Directory.Exists(subFolder))
        {
            Directory.CreateDirectory(subFolder);
        }

        var fileName = Path.GetFileName(imageFile.FileName);
        var filePath = Path.Combine(subFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await imageFile.CopyToAsync(stream);
        }

        return Path.Combine("PathImages", "SchoolPermit", fileName);
    }

    public async Task<string?> SaveProofofPayment(IFormFile? imageFile)
    {
        if (imageFile == null || imageFile.Length == 0)
            return null;

        string mainFolder = Path.Combine(Directory.GetCurrentDirectory(), "PathImages");
        string subFolder = Path.Combine(mainFolder, "ProofOfPayment");

        if (!Directory.Exists(mainFolder))
        {
            Directory.CreateDirectory(mainFolder);
        }
        if (!Directory.Exists(subFolder))
        {
            Directory.CreateDirectory(subFolder);
        }

        var fileName = Path.GetFileName(imageFile.FileName);
        var filePath = Path.Combine(subFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await imageFile.CopyToAsync(stream);
        }

        return Path.Combine("PathImages", "ProofOfPayment", fileName);
    }

    public async Task<string?> SaveFrontImage(IFormFile? imageFile)
    {
        if (imageFile == null || imageFile.Length == 0)
            return null;

        string mainFolder = Path.Combine(Directory.GetCurrentDirectory(), "PathImages");
        string subFolder = Path.Combine(mainFolder, "FrontViewImage");

        if (!Directory.Exists(mainFolder))
        {
            Directory.CreateDirectory(mainFolder);
        }
        if (!Directory.Exists(subFolder))
        {
            Directory.CreateDirectory(subFolder);
        }

        var fileName = Path.GetFileName(imageFile.FileName);
        var filePath = Path.Combine(subFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await imageFile.CopyToAsync(stream);
        }

        return Path.Combine("PathImages", "FrontViewImage", fileName);
    }

    public async Task<string?> SaveSideImage(IFormFile? imageFile)
    {
        if (imageFile == null || imageFile.Length == 0)
            return null;

        string mainFolder = Path.Combine(Directory.GetCurrentDirectory(), "PathImages");
        string subFolder = Path.Combine(mainFolder, "SideViewImage");

        if (!Directory.Exists(mainFolder))
        {
            Directory.CreateDirectory(mainFolder);
        }
        if (!Directory.Exists(subFolder))
        {
            Directory.CreateDirectory(subFolder);
        }

        var fileName = Path.GetFileName(imageFile.FileName);
        var filePath = Path.Combine(subFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await imageFile.CopyToAsync(stream);
        }

        return Path.Combine("PathImages", "SideViewImage", fileName);
    }

    public async Task<string?> SaveBackImage(IFormFile? imageFile)
    {
        if (imageFile == null || imageFile.Length == 0)
            return null;

        string mainFolder = Path.Combine(Directory.GetCurrentDirectory(), "PathImages");
        string subFolder = Path.Combine(mainFolder, "BackViewImage");

        if (!Directory.Exists(mainFolder))
        {
            Directory.CreateDirectory(mainFolder);
        }
        if (!Directory.Exists(subFolder))
        {
            Directory.CreateDirectory(subFolder);
        }

        var fileName = Path.GetFileName(imageFile.FileName);
        var filePath = Path.Combine(subFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await imageFile.CopyToAsync(stream);
        }

        return Path.Combine("PathImages", "BackViewImage", fileName);
    }

    public async Task<string?> SaveSizeGuide(IFormFile? imageFile)
    {
        if (imageFile == null || imageFile.Length == 0)
            return null;

        string mainFolder = Path.Combine(Directory.GetCurrentDirectory(), "PathImages");
        string subFolder = Path.Combine(mainFolder, "SizeGuide");

        if (!Directory.Exists(mainFolder))
        {
            Directory.CreateDirectory(mainFolder);
        }
        if (!Directory.Exists(subFolder))
        {
            Directory.CreateDirectory(subFolder);
        }

        var fileName = Path.GetFileName(imageFile.FileName);
        var filePath = Path.Combine(subFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await imageFile.CopyToAsync(stream);
        }

        return Path.Combine("PathImages", "SizeGuide", fileName);
    }
}

