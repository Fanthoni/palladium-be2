using Amazon.S3;
using Amazon.S3.Transfer;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PalladiumBE.Utilities.AWS;

static class S3Helper
{
    public static async Task UploadImage (string itemId, string image, string keyName)
    {
        string accessKeyId = AppConfig.AWSAccessKey;
        string secretKey = AppConfig.AWSSecretKey;
        string bucketName = AppConfig.AWSBucketName;

        try
        {
            byte[] byteArray = Convert.FromBase64String(image);

            var s3Client = new AmazonS3Client(accessKeyId, secretKey, Amazon.RegionEndpoint.USWest2);
            var transferUtility = new TransferUtility(s3Client);
            string key = $"itemPhotos/{itemId}/{keyName}";

            using MemoryStream stream = new(byteArray);
            await transferUtility.UploadAsync(stream, bucketName, key);
        }
        catch (AmazonS3Exception ex)
        {
            Console.WriteLine($"Error uploading file to S3: {ex.Message}");
        }
    }

    public static async Task UploadImages (string itemId, string[] images, string? keyName) 
    {
        var uploadTasks = new List<Task>();

        for (int i = 0; i < images.Length; i++)
        {
            var uploadTask = UploadImage(itemId, images[i], keyName ?? $"{i}");
            uploadTasks.Add(uploadTask);
        }

        await Task.WhenAll(uploadTasks);
    }
}