using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon;
using System;
using System.IO;
using System.Net;

namespace kpm_csharp_webform
{
  class AWS
  {
    private const string bucket = "kpm-csharp-webform";
    private static readonly RegionEndpoint region = RegionEndpoint.USWest2;
    private static string accessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID");
    private static string secretKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY");
    private static BasicAWSCredentials credentials = new BasicAWSCredentials(accessKey, secretKey);
    private static IAmazonS3 client = new AmazonS3Client(credentials, region);

    public static string Upload(string file)
    {
      string uploadUrl = GeneratePreSignedURLForUpload(System.IO.Path.GetFileName(file));
      UploadObject(uploadUrl, file);
      string downloadUrl = GeneratePreSignedURLForDownload(System.IO.Path.GetFileName(file));
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

    private static string GeneratePreSignedURLForUpload(string file)
    {
      var request = new GetPreSignedUrlRequest
      {
        BucketName = bucket,
        Key = file,
        Verb = HttpVerb.PUT,
        Expires = DateTime.Now.AddMinutes(5)
      };

      string url = client.GetPreSignedURL(request);
      return url;
    }
    private static string GeneratePreSignedURLForDownload(string file)
    {
      var request = new GetPreSignedUrlRequest
      {
        BucketName = bucket,
        Key = file,
        Verb = HttpVerb.GET,
        Expires = DateTime.Now.AddMinutes(60)
      };

      string url = client.GetPreSignedURL(request);
      return url;
    }
  }
}