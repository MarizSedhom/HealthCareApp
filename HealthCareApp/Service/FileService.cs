namespace HealthCareApp.Service
{
    static class FilePaths
    {
        static string BasePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        public static string ImgPath = Path.Combine(BasePath, "image");
        public static string DrPath = Path.Combine(ImgPath, "Doctor");
        public static string DrPathRelative = "/uploads/image/Doctor/";


    }
    public class FileService: IFileService
    {
        public async Task< string> uploadFileAsync(IFormFile file,string FullPathExceptFile) { 
            if(file == null||file.Length==0)
                return null;
           
            string fileName= Guid.NewGuid().ToString()+Path.GetExtension(file.FileName);
            var fullPath = Path.Combine(FullPathExceptFile, fileName);

            
            using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);
            return fileName;

        }

        public bool DeleteFile(string fileName, string FullPathExceptFile)
        {
           if (string.IsNullOrEmpty(fileName)|| string.IsNullOrEmpty(FullPathExceptFile))
                return false;

           string filePath =Path.Combine(FullPathExceptFile,fileName);
           if (File.Exists(filePath))
           {
               File.Delete(filePath);
               return true;
           }
           return false;
        }
    }
}
