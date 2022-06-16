using MoviePro.Services.Interfaces;

namespace MoviePro.Services
{
    public class BasicImageService : IImageService
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly ApplicationDbContext _context;

        public BasicImageService(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient;
        }

        public string DecodeImage(byte[] poster, string contentType)
        {
            if (poster == null) return null;
            var posterImage = Convert.ToBase64String(poster);
            return $"data:{contentType};base64,{posterImage}"; //returns data able to be used by the img src attribute
        }

        public async Task<byte[]> EncodeImageAsync(IFormFile poster)
        {
            if (poster == null) return null;

            using var ms = new MemoryStream();
            await poster.CopyToAsync(ms);
            return ms.ToArray();
        }

        public async Task<byte[]> EncodeImageURLAsync(string imageURL)
        {
            var client = _httpClient.CreateClient(); //creates client
            var request = await client.GetAsync(imageURL); //fire request to assembled imageURL
            using Stream stream = await request.Content.ReadAsStreamAsync();//using statement allows
            //us to more aggressively reclaim the memory used by the stream object

            var ms = new MemoryStream();
            await stream.CopyToAsync(ms); //copy to MemoryStream - allows us to return the ms as an array

            return ms.ToArray();//info can now be stored as a byte[]
        }
    }
}
