using System;
using System.Threading.Tasks;
using System.Web;
using Mazadaty.Services.Storage.Enum;


namespace Mazadaty.Services.Storage
{
    public interface IStorageService
    {
        Task<Uri> SaveFile(string containerName, HttpPostedFileBase postedFile);
        Task<Uri[]> SaveImage(string containerName, HttpPostedFileBase postedFile, ImageSettings imageSettings, ImageFormat? outputFormat = null);
        Task DeleteFile(string containerName, string fileName);
    }
}