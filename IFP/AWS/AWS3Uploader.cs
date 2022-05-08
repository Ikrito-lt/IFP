using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using IFP.Utils;
using System.Threading.Tasks;

namespace IFP.AWS
{
    internal static class AWS3Uploader
    {

        public static async Task<string> UploadFileAsync(string bucketName, string keyName, string filePath, string contentType = "")
        {
            var creds = new BasicAWSCredentials(Globals.AWSAccessKeyID, Globals.AWSSecretAccessKey);
            var client = new AmazonS3Client(creds, Amazon.RegionEndpoint.EUCentral1);

            try
            {
                PutObjectRequest putRequest = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = keyName,
                    FilePath = filePath,
                    ContentType = contentType
                };

                var response = await client.PutObjectAsync(putRequest);
                var message = $"HttP {response.HttpStatusCode}\n {response.ResponseMetadata}\n{response.ContentLength}";
                return message;
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
                    ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    return "Check the provided AWS Credentials.";
                }
                else
                {
                    return "Error occurred: " + amazonS3Exception.Message;
                }
            }
        }
    }
}
