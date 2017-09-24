using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using ImageResizer;
using Mazadaty.Services.Storage.Enum;
using Microsoft.WindowsAzure.Storage;

namespace Mazadaty.Services.Storage
{
    public class LocalStorageService: IStorageService
    {
        public async Task<Uri> SaveFile(string containerName, HttpPostedFileBase postedFile)
        {
            var fileData = await GetFileData(postedFile);
            var fileName = Path.GetFileName(postedFile.FileName);
            using (var content =
                new MultipartFormDataContent("Upload----" + DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)))
            {
                if (!Directory.Exists(Path.Combine(HostingEnvironment.ApplicationPhysicalPath,
                    containerName)))
                {
                    Directory.CreateDirectory(Path.Combine(HostingEnvironment.ApplicationPhysicalPath,
                        containerName));
                }

                postedFile.SaveAs(Path.Combine(HostingEnvironment.ApplicationPhysicalPath, $"{containerName}/{fileName}"));
                return new Uri($"/{containerName}/{fileName}");
            }
        }

        public async Task<Uri[]> SaveImage(string containerName, HttpPostedFileBase postedFile, ImageSettings imageSettings,
            ImageFormat? outputFormat = null)
        {
            using (var inputStream =postedFile.InputStream)
            {
                
                var uris = new List<Uri>();
                var widthInts = imageSettings.Widths;
                var rotateFlipType = GetRotateFlipType(inputStream);

                foreach (var width in widthInts)
                {
                    using (var outputStream = new MemoryStream())
                    {
                        string fileName = Path.GetFileName(postedFile.FileName);
                        
                        var safeFileName = GetFileName(fileName, width, outputFormat?.ToString() ?? "");

                        ResizeImage(inputStream, outputStream, width, imageSettings.ForceSquare, imageSettings.BackgroundColor, imageSettings.Watermark, rotateFlipType, outputFormat?.ToString() ?? "");

                        var contentType = "";

                        if (!string.IsNullOrEmpty(outputFormat?.ToString() ?? ""))
                        {
                            contentType = $"image/{outputFormat}";
                        }

                        postedFile.SaveAs(Path.Combine(HostingEnvironment.ApplicationPhysicalPath, $"{containerName}/{fileName}"));
                        uris.Add(new Uri($"/{containerName}/{fileName}"));
                    }
                }

                return uris.ToArray();
            }
        }

        public Task DeleteFile(string containerName, string fileName)
        {
            throw new NotImplementedException();
        }

        private static async Task<byte[]> GetFileData(HttpPostedFileBase postedFile)
        {
            using (var stream = new MemoryStream())
            {
                postedFile.InputStream.Position = 0;
                await postedFile.InputStream.CopyToAsync(stream);
                stream.Position = 0;
                return stream.ToArray();
            }
        }

        private static RotateFlipType GetRotateFlipType(Stream inputStream)
        {
            using (var image = Image.FromStream(inputStream))
            {
                return GetRotateFlipType(image);
            }
        }

        private static RotateFlipType GetRotateFlipType(Image image)
        {
            if (Array.IndexOf(image.PropertyIdList, 274) <= -1)
            {
                return RotateFlipType.RotateNoneFlipNone;
            }

            var orientation = (int)image.GetPropertyItem(274).Value[0];
            switch (orientation)
            {
                case 3: return RotateFlipType.Rotate180FlipNone;
                case 4: return RotateFlipType.Rotate180FlipX;
                case 5: return RotateFlipType.Rotate90FlipX;
                case 6: return RotateFlipType.Rotate90FlipNone;
                case 7: return RotateFlipType.Rotate270FlipX;
                case 8: return RotateFlipType.Rotate270FlipNone;
                default: return RotateFlipType.RotateNoneFlipNone;
            }
        }

        private static string GetFileName(string fileName, int width, string outputFormate = "")
        {
            var name = GenerateSlug(Path.GetFileNameWithoutExtension(fileName));
            if (width > 0)
            {
                name += "-" + width;
            }

            var extension = Path.GetExtension(fileName);

            return string.IsNullOrEmpty(outputFormate) ? $"{name}-{DateTime.UtcNow.Ticks}{extension}" : $"{name}-{DateTime.UtcNow.Ticks}.{outputFormate}";
        }

        private static string GenerateSlug(string txt)
        {
            var str = RemoveAccent(txt).ToLower();

            str = Regex.Replace(str, @"[^a-z0-9\s-]", ""); // invalid chars
            str = Regex.Replace(str, @"\s+", " ").Trim(); // convert multiple spaces into one space
            str = Regex.Replace(str, @"\s", "-"); // hyphens

            return str;
        }

        private static string RemoveAccent(string txt)
        {
            var bytes = Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            return Encoding.ASCII.GetString(bytes);
        }

        private static void ResizeImage(Stream inputStream, Stream outputStream, int width, bool forceSquare, Color backgroundColor, string watermark, RotateFlipType rotateFlipType, string outputFormate = "")
        {
            var resizeSettings = GetResizeSettings(width, forceSquare, backgroundColor, watermark, rotateFlipType, outputFormate);
            inputStream.Seek(0, SeekOrigin.Begin);
            ImageBuilder.Current.Build(inputStream, outputStream, resizeSettings, false);
            outputStream.Seek(0, SeekOrigin.Begin);
        }

        private static ResizeSettings GetResizeSettings(int width, bool forceSquare, Color backgroundColor, string watermark, RotateFlipType rotateFlipType, string outputFormate = "")
        {
            var resizeSettings = new ResizeSettings
            {
                MaxWidth = width,
                Scale = ScaleMode.Both,
                Flip = rotateFlipType
            };

            if (forceSquare)
            {
                resizeSettings.Width = width;
                resizeSettings.Height = width;
                resizeSettings.Mode = FitMode.Pad;
                resizeSettings.Anchor = ContentAlignment.MiddleCenter;
                resizeSettings.BackgroundColor = backgroundColor;
                resizeSettings.Quality = 80;
                if (!string.IsNullOrEmpty(outputFormate))
                {
                    resizeSettings.Format = "jpg";
                }
            }

            if (!string.IsNullOrEmpty(watermark))
            {
                var watermarkKey = $"{watermark}[{width}]";

                Trace.TraceInformation("Adding watermark for key '{0}'.", watermarkKey);

                resizeSettings["Watermark"] = watermarkKey;
            }

            return resizeSettings;
        }

        
    }
}