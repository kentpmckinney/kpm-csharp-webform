using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Net;

namespace kpm_csharp_webform
{
  class UploadObjectUsingPresignedURLTest
  {
    private const string bucketName = "kpm_csharp_webform";
    private const string objectKey = "object";
    private const string filePath = "";
    private static readonly RegionEndpoint bucketRegion = RegionEndpoint.USWest2;
    private static IAmazonS3 s3Client;

    public static void Upload(string file)
    {
      string accessKey = "AKIAIIPHTLARGTVEMFQA";
      string secretKey = "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";

      var credentials = new BasicAWSCredentials(accessKey, secretKey);
      s3Client = new AmazonS3Client(credentials, RegionEndpoint.USWest2);
      var url = GeneratePreSignedURL();
      UploadObject(url);
    }

    private static void UploadObject(string url)
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

    private static string GeneratePreSignedURL()
    {
      var request = new GetPreSignedUrlRequest
      {
        BucketName = bucketName,
        Key = objectKey,
        Verb = HttpVerb.PUT,
        Expires = DateTime.Now.AddMinutes(5)
      };

      string url = s3Client.GetPreSignedURL(request);
      return url;
    }
  }
}
// class UploadFileMPULowLevelAPITest
// {
//     private const string bucketName = "*** provide bucket name ***";
//     private const string keyName = "*** provide a name for the uploaded object ***";
//     private const string filePath = "*** provide the full path name of the file to upload ***";
//     // Specify your bucket region (an example region is shown).
//     private static readonly RegionEndpoint bucketRegion = RegionEndpoint.USWest2;
//     private static IAmazonS3 s3Client;

//     public static void Main()
//     {
//         s3Client = new AmazonS3Client(bucketRegion);
//         Console.WriteLine("Uploading an object");
//         UploadObjectAsync().Wait(); 
//     }

//     private static async Task UploadObjectAsync()
//     {
//         // Create list to store upload part responses.
//         List<UploadPartResponse> uploadResponses = new List<UploadPartResponse>();

//         // Setup information required to initiate the multipart upload.
//         InitiateMultipartUploadRequest initiateRequest = new InitiateMultipartUploadRequest
//         {
//             BucketName = bucketName,
//             Key = keyName
//         };

//         // Initiate the upload.
//         InitiateMultipartUploadResponse initResponse =
//             await s3Client.InitiateMultipartUploadAsync(initiateRequest);

//         // Upload parts.
//         long contentLength = new FileInfo(filePath).Length;
//         long partSize = 5 * (long)Math.Pow(2, 20); // 5 MB

//         try
//         {
//             Console.WriteLine("Uploading parts");

//             long filePosition = 0;
//             for (int i = 1; filePosition < contentLength; i++)
//             {
//                 UploadPartRequest uploadRequest = new UploadPartRequest
//                     {
//                         BucketName = bucketName,
//                         Key = keyName,
//                         UploadId = initResponse.UploadId,
//                         PartNumber = i,
//                         PartSize = partSize,
//                         FilePosition = filePosition,
//                         FilePath = filePath
//                     };

//                 // Track upload progress.
//                 uploadRequest.StreamTransferProgress +=
//                     new EventHandler<StreamTransferProgressArgs>(UploadPartProgressEventCallback);

//                 // Upload a part and add the response to our list.
//                 uploadResponses.Add(await s3Client.UploadPartAsync(uploadRequest));

//                 filePosition += partSize;
//             }

//             // Setup to complete the upload.
//             CompleteMultipartUploadRequest completeRequest = new CompleteMultipartUploadRequest
//                 {
//                     BucketName = bucketName,
//                     Key = keyName,
//                     UploadId = initResponse.UploadId
//                  };
//             completeRequest.AddPartETags(uploadResponses);

//             // Complete the upload.
//             CompleteMultipartUploadResponse completeUploadResponse =
//                 await s3Client.CompleteMultipartUploadAsync(completeRequest);
//         }
//         catch (Exception exception)
//         {
//             Console.WriteLine("An AmazonS3Exception was thrown: { 0}", exception.Message);

//             // Abort the upload.
//             AbortMultipartUploadRequest abortMPURequest = new AbortMultipartUploadRequest
//             {
//                 BucketName = bucketName,
//                 Key = keyName,
//                 UploadId = initResponse.UploadId
//             };
//            await s3Client.AbortMultipartUploadAsync(abortMPURequest);
//         }
//     }
//     public static void UploadPartProgressEventCallback(object sender, StreamTransferProgressArgs e)
//     {
//         // Process event. 
//         Console.WriteLine("{0}/{1}", e.TransferredBytes, e.TotalBytes);
//     }
// }

