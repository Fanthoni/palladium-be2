using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.VisualBasic;
using MongoDB.Bson;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PalladiumBE.Utilities.AWS;


static class S3Helper
{
    static class PathFile
    {
        public static readonly string THUMBNAIL = "itemPhotos/{itemId}/thumbnail";
        public static readonly string PHOTOS = "itemPhotos/{itemId}/photos/";

    }


    static readonly string bucketName = AppConfig.AWSBucketName;
    static readonly AmazonS3Client s3Client = new(AppConfig.AWSAccessKey, AppConfig.AWSSecretKey, Amazon.RegionEndpoint.USWest2);
    static readonly string objectUrlPrefix = $"https://{bucketName}.s3.{Amazon.RegionEndpoint.USWest2.SystemName}.amazonaws.com/";

    public static string RetrieveThumbnailImage (string itemId)
    {
        string objectKey = PathFile.THUMBNAIL.Replace("{itemId}", itemId);
        return $"{objectUrlPrefix}{objectKey}";
    }

    public static async Task<object> RetrieveItemPhotos (string itemId)
    {
        try
        {
            var listObjectsRequest = new ListObjectsRequest
            {
                BucketName = bucketName,
                Prefix = PathFile.PHOTOS.Replace("{itemId}", itemId),
                Delimiter = "/"
            };

            var listObjectsResponse = await s3Client.ListObjectsAsync(listObjectsRequest);
            int objectCount = listObjectsResponse.S3Objects.Count;

            List<string> photoUrls =  new List<string>();

            for (int i = 0; i < objectCount; i++)
            {
                string objectKey = $"{PathFile.PHOTOS.Replace("{itemId}", itemId)}{i}";
                photoUrls.Add($"{objectUrlPrefix}{objectKey}");
            }

            return photoUrls;
        }
        catch (AmazonS3Exception ex)
        {
            Console.WriteLine($"Error retrieving photos from S3: {ex.Message}");
            return new List<string>();
        }        
    }

    public static async Task UploadImage (string itemId, string image, string keyFilePath, string keyName)
    {

        try
        {
            byte[] byteArray = Convert.FromBase64String(image);

            var transferUtility = new TransferUtility(s3Client);

            string key;
            if (!string.IsNullOrEmpty(keyFilePath))
            {
                key = $"itemPhotos/{itemId}/{keyFilePath}/{keyName}";
            }
            else
            {
                key = $"itemPhotos/{itemId}/{keyName}";
            }

            using MemoryStream stream = new(byteArray);
            await transferUtility.UploadAsync(stream, bucketName, key);
        }
        catch (AmazonS3Exception ex)
        {
            Console.WriteLine($"Error uploading file to S3: {ex.Message}");
        }
    }

    public static async Task UploadImages (string itemId, string[] images, string? keyFilePath = null, string? keyName = null) 
    {
        var uploadTasks = new List<Task>();

        for (int i = 0; i < images.Length; i++)
        {
            var uploadTask = UploadImage(itemId, images[i], keyFilePath ?? "", keyName ?? $"{i}");
            uploadTasks.Add(uploadTask);
        }

        await Task.WhenAll(uploadTasks);
    }
}