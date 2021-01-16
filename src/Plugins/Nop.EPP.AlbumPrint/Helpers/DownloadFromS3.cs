using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Ionic.Zip;
using ZipFile = Ionic.Zip.ZipFile;

namespace Nop.EPP.AlbumPrint.Helpers
{
    public class DownloadFromS3
    {
        public async Task<string> Download(string cloudFolderPath, string zipFileName)
        {
            string regionName = "us-east-1";
            string accessKey = "ORDGGMVA93T8UPAO99WI";
            string secretKey = "FrjXN7FzYZ5zIGXCV6heShpfrGjo1seVEeaejoJO";

            //-- Set the region or service url in the config
            AmazonS3Config config = new AmazonS3Config();
            config.ServiceURL = "https://s3.us-east-1.wasabisys.com";
            //config.RegionEndpoint = Amazon.RegionEndpoint.USEast1;

            //-- Create the client
            AmazonS3Client client = new AmazonS3Client(accessKey, secretKey, config);

            //var cloudFolderPath = "AlbumPrints/NUvdbruFcBPOA54";
            var bucketName = "epp";
            var destinationFolder = "C:\\Temp\\S3Files\\";
            var folderId = cloudFolderPath.Replace("AlbumPrints/", "");

            ListObjectsRequest request = new ListObjectsRequest
            {
                BucketName = bucketName,
                Prefix = cloudFolderPath
            };

            ListObjectsResponse response = await client.ListObjectsAsync(request);
            foreach (S3Object obj in response.S3Objects)
            {
                Console.WriteLine(obj.Key);

                GetObjectRequest fileRequest = new GetObjectRequest();
                fileRequest.BucketName = bucketName;
                fileRequest.Key = obj.Key;//"/file_manager/epp/AlbumPrints/NUvdbruFcBPOA54/Photo0 Album_Glossy_Sheet.jpg";

                var fileName = Path.GetFileName(obj.Key);

                var subFolderName = obj.Key.Replace(cloudFolderPath, "");
                subFolderName = subFolderName.Replace(fileName, "").Replace("/", "");

                var completeDestiFolderPath = destinationFolder + $"\\{folderId}\\" + subFolderName;

                if(!string.IsNullOrEmpty(subFolderName))
                    completeDestiFolderPath = destinationFolder + $"\\{folderId}\\" + subFolderName;
                else
                    completeDestiFolderPath = destinationFolder + $"\\{folderId}";

                if (!Directory.Exists(completeDestiFolderPath))
                    Directory.CreateDirectory(completeDestiFolderPath);

                GetObjectResponse fileResponse = await client.GetObjectAsync(fileRequest);
                await fileResponse.WriteResponseStreamToFileAsync(completeDestiFolderPath + "\\" + fileName, false, new System.Threading.CancellationToken());
            }

            CompressFolder(destinationFolder, $"C:\\Temp\\{zipFileName}");

            return $"C:\\Temp\\{zipFileName}";
        }

        public void CompressFolder(string folderPath, string zipFilePath)
        {

            using (ZipFile zip = new ZipFile())
            {
                zip.UseUnicodeAsNecessary = true;
                zip.AddDirectory(folderPath);
                zip.CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression;
                zip.Comment = "This zip was created at " + System.DateTime.Now.ToString("G");
                zip.Save(zipFilePath);
            }
        }
    }
}
