using Microsoft.Extensions.Options;

namespace UNITEE_BACKEND.Models.ImageDirectory
{
    public class ImageDirectoryPath
    {
        private readonly ImagePathOptions _imagePathOptions;

        public ImageDirectoryPath(IOptions<ImagePathOptions> imagePathOptions)
        {
            _imagePathOptions = imagePathOptions.Value;
        }

        public string GetImagePath()
        {
            return _imagePathOptions.Images;
        }

        public string GetProductPath()
        {
            return _imagePathOptions.ProductImages;
        }

        public string GetSupplierPath()
        {
            return _imagePathOptions.SupplierImage;
        }

        public string GetBIRPath()
        {
            return _imagePathOptions.BIR;
        }

        public string GetCityPermitPath()
        {
            return _imagePathOptions.CityPermit;
        }

        public string GetSchoolPermitPath()
        {
            return _imagePathOptions.SchoolPermit;
        }

        public string GetProofOfPayment()
        {
            return _imagePathOptions.ProofOfPayment;
        }
    }
}
