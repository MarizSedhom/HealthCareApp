namespace HealthCareApp.Service
{
    public interface IFileService
    {
        Task<string> uploadFileAsync(IFormFile file, string FullPathExceptFile);
        bool DeleteFile(string fileName, string FullPathExceptFile);
    }
}