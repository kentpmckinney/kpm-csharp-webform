using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace kpm_csharp_webform
{
  class AWS
  {
    private const string bucket = "kpm-csharp-webform";
    private static readonly RegionEndpoint region = RegionEndpoint.USWest2;
    private static string accessKey;
    private static string secretKey;
    private static BasicAWSCredentials credentials;
    private static IAmazonS3 client;

    static AWS()
    {
      // Validate AWS credentials
      if (Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID") == null)
        throw new ArgumentNullException("The environment variable AWS_ACCESS_KEY_ID must not be null");
      accessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID");
      if (accessKey == "")
        throw new ArgumentException("The environment variable AWS_ACCESS_KEY_ID must not be empty");
      if (Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY") == null)
        throw new ArgumentNullException("The environment variable AWS_SECRET_ACCESS_KEY must not be null");
      secretKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY");
      if (secretKey == "")
        throw new ArgumentException("The environment variable AWS_SECRET_ACCESS_KEY must not be empty");

      credentials = new BasicAWSCredentials(accessKey, secretKey);
      client = new AmazonS3Client(credentials, region);

      // Validate the AWS client and bucket
      Task<Boolean> t = client.DoesS3BucketExistAsync(bucket);
      t.Wait();
      bool bucketExists = t.Result;
      if (!t.IsCompletedSuccessfully)
        throw new WebException("DoesS3BucketExistAsync() must run successfully");
      if (!bucketExists)
        throw new WebException($"Amazon S3 bucket must exist");
    }

    public static string Upload(string file)
    {
      if (String.IsNullOrWhiteSpace(file))
        return "";
      if (!File.Exists(file))
        throw new FileNotFoundException("The file being uploaded must exist");
      FileInfo fileInfo = new FileInfo(file);
      if (fileInfo.Length < 1)
        throw new FileLoadException("File being uploaded must be greater than zero bytes");
      if (fileInfo.Length > 10 * 1024 * 1024)
        throw new FileLoadException("File being uploaded must be less than or equal to 10 MiB");

      string uploadUrl = GeneratePreSignedURLForUpload(System.IO.Path.GetFileName(file));
      UploadObject(uploadUrl, file);
      string downloadUrl = GeneratePreSignedURLForDownload(System.IO.Path.GetFileName(file));
      return downloadUrl;
    }

    private static void UploadObject(string url, string filePath)
    {
      if (String.IsNullOrWhiteSpace(url))
        return;
      if (String.IsNullOrWhiteSpace(filePath))
        return;

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
      if (String.IsNullOrWhiteSpace(file))
        return "";

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
      if (String.IsNullOrWhiteSpace(file))
        return "";

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