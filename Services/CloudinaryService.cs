using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace pet_spa_system1.Services
{
    public interface ICloudinaryService
    {
        Task<string> UploadImageAsync(IFormFile file, string folder = "uploads");
        Task<bool> DeleteImageAsync(string imageUrl);
    }

    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService()
        {
            var account = new Account(
                "dprp1jbd9", 
                "584135338254938", 
                "QbUYngPIdZcXEn_mipYn8RE5dlo" 
            );
            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadImageAsync(IFormFile file, string folder = "uploads")
        {
            if (file == null || file.Length == 0)
                return string.Empty;

            try
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, file.OpenReadStream()),
                    Folder = folder,
                    PublicId = $"{folder}_{Guid.NewGuid()}",
                    Transformation = new Transformation()
                        .Width(1200).Height(800).Crop("limit")
                        .Quality("auto")
                        .FetchFormat("auto")
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return uploadResult.SecureUrl.ToString();
                }
                else
                {
                    throw new Exception($"Cloudinary upload failed: {uploadResult.Error?.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading image to Cloudinary: {ex.Message}");
                return string.Empty;
            }
        }

        public async Task<bool> DeleteImageAsync(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl) || !imageUrl.Contains("cloudinary.com"))
                return false;

            try
            {
                var publicId = ExtractPublicIdFromUrl(imageUrl);
                if (string.IsNullOrEmpty(publicId))
                    return false;

                var deleteParams = new DeletionParams(publicId);
                var result = await _cloudinary.DestroyAsync(deleteParams);

                return result.StatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting image from Cloudinary: {ex.Message}");
                return false;
            }
        }

        private string ExtractPublicIdFromUrl(string imageUrl)
        {
            try
            {
                var uri = new Uri(imageUrl);
                var segments = uri.AbsolutePath.Split('/');
                
                var uploadIndex = Array.IndexOf(segments, "upload");
                if (uploadIndex >= 0 && uploadIndex + 2 < segments.Length)
                {
                    var pathParts = segments.Skip(uploadIndex + 2).ToArray();
                    var fullPath = string.Join("/", pathParts);
                    
                    var lastDotIndex = fullPath.LastIndexOf('.');
                    if (lastDotIndex > 0)
                    {
                        return fullPath.Substring(0, lastDotIndex);
                    }
                    return fullPath;
                }
                
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
