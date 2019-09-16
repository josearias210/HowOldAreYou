namespace HowOldAreyou.Services
{
    using HowOldAreyou.Models;
    using Newtonsoft.Json;
    using System.IO;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    public class FaceService
    {
        private static readonly string subscriptionKey = "YOUR_KEY";
        private static readonly string uriBase = "YOUR_URL_ENDPOINT";
        public async Task<double> DetectAge(string imageFilePath)
        {
            double age = 0;
            try
            {
                var client = new HttpClient();

                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

                string requestParameters = "returnFaceId=true&returnFaceLandmarks=false" +
                    "&returnFaceAttributes=age,gender,headPose,smile,facialHair,glasses," +
                    "emotion,hair,makeup,occlusion,accessories,blur,exposure,noise";

                string uri = uriBase + "?" + requestParameters;

                HttpResponseMessage response;
                byte[] byteData = GetImageAsByteArray(imageFilePath);

                using (ByteArrayContent content = new ByteArrayContent(byteData))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                    response = await client.PostAsync(uri, content);
                    string json = await response.Content.ReadAsStringAsync();
                    var faceResult = JsonConvert.DeserializeObject<FaceResult[]>(json);
                    if (faceResult != null && faceResult.Length > 0)
                    {
                        age = faceResult[0].faceAttributes.age;
                    }
                }
            }
            catch
            {
            }

            return age;
        }

        private byte[] GetImageAsByteArray(string imageFilePath)
        {
            using (FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader binaryReader = new BinaryReader(fileStream))
                {
                    return binaryReader.ReadBytes((int)fileStream.Length);
                }
            }
        }
    }
}
