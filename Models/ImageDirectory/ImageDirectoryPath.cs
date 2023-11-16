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

        public string GetMainFolderPath()
        {
            return _imagePathOptions.PathImages;
        }

        public string GetImagePath()
        {
            return Path.Combine(_imagePathOptions.PathImages, _imagePathOptions.Images);
        }

        public string GetStudyLoadPath()
        {
            return Path.Combine(_imagePathOptions.PathImages, _imagePathOptions.StudyLoad);
        }

        public string GetProductPath()
        {
            return Path.Combine(_imagePathOptions.PathImages, _imagePathOptions.ProductImages);
        }

        public string GetFrontImagePath()
        {
            return Path.Combine(_imagePathOptions.PathImages, _imagePathOptions.FrontViewImage);
        }

        public string GetSideImagePath()
        {
            return Path.Combine(_imagePathOptions.PathImages, _imagePathOptions.SideViewImage);
        }

        public string GetBackImagePath()
        {
            return Path.Combine(_imagePathOptions.PathImages, _imagePathOptions.BackViewImage);
        }

        public string GetSupplierPath()
        {
            return Path.Combine(_imagePathOptions.PathImages, _imagePathOptions.SupplierImage);
        }

        public string GetBIRPath()
        {
            return Path.Combine(_imagePathOptions.PathImages, _imagePathOptions.BIR);
        }

        public string GetCityPermitPath()
        {
            return Path.Combine(_imagePathOptions.PathImages, _imagePathOptions.CityPermit);
        }

        public string GetSchoolPermitPath()
        {
            return Path.Combine(_imagePathOptions.PathImages, _imagePathOptions.SchoolPermit);
        }

        public string GetProofOfPayment()
        {
            return Path.Combine(_imagePathOptions.PathImages, _imagePathOptions.ProofOfPayment);
        }
    }
}
