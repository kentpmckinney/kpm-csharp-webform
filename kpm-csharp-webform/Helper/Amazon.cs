using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon;
using System;
using System.IO;
using System.Net;

namespace kpm_csharp_webform
{
  class UploadObjectUsingPresignedURLTest
  {
    private const string bucketName = "kpm-csharp-webform";
    private const string objectKey = "kpm-csharp-webform-file";
    private static readonly RegionEndpoint bucketRegion = RegionEndpoint.USWest2;
    private static IAmazonS3 s3Client;

    public static string Upload(string file)
    {
      string accessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID");
      string secretKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY");
      Console.WriteLine("Amazon logging:");
      var credentials = new BasicAWSCredentials(accessKey, secretKey);
      s3Client = new AmazonS3Client(credentials, RegionEndpoint.USWest2);
      var uploadUrl = GeneratePreSignedURLUpload(System.IO.Path.GetFileName(file));
      UploadObject(uploadUrl, file);
      string downloadUrl = GeneratePreSignedURLDownload(System.IO.Path.GetFileName(file));
      return downloadUrl;
    }

    private static void UploadObject(string url, string filePath)
    {
      HttpWebRequest httpRequest = WebRequest.Create(url) as HttpWebRequest;
      httpRequest.Method = "PUT";
      using (Stream dataStream = httpRequest.GetRequestStream())
      {
        var buffer = new byte[8000];
        using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
          int bytesRead = 0;
          while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
          {
            dataStream.Write(buffer, 0, bytesRead);
          }
        }
      }
      HttpWebResponse response = httpRequest.GetResponse() as HttpWebResponse;
    }

    private static string GeneratePreSignedURLUpload(string file)
    {
      var request = new GetPreSignedUrlRequest
      {
        BucketName = bucketName,
        Key = file,
        Verb = HttpVerb.PUT,
        Expires = DateTime.Now.AddMinutes(5)
      };

      string url = s3Client.GetPreSignedURL(request);
      return url;
    }
    private static string GeneratePreSignedURLDownload(string file)
    {
      var request = new GetPreSignedUrlRequest
      {
        BucketName = bucketName,
        Key = file,
        Verb = HttpVerb.GET,
        Expires = DateTime.Now.AddMinutes(60)
      };

      string url = s3Client.GetPreSignedURL(request);
      return url;
    }
  }
}