using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Lawyer.Helpers
{
    public static class Extension
    {
        public static bool CheckFileSize(this IFormFile file,int kb)
        {
            return file.Length / 1024 <= kb;
        }
        public static bool CheckFileType(this IFormFile file, string type)
        {
            return file.ContentType.Contains(type);
        }
        public static async Task<string> SaveFileAsync(this IFormFile file, string root,params string[] folders)
        {
            string fileName = Guid.NewGuid().ToString() + file.FileName;
            string resultPath = Path.Combine(Helper.GetPath(root,folders),fileName);
            using (var fileStream = new FileStream(resultPath, FileMode.Create))
            {
               await file.CopyToAsync(fileStream);
            }
            return fileName;
        }
    }
}
