namespace MoviePro.Services.Interfaces
{
    public interface IImageService
    {
        Task<byte[]> EncodeImageAsync(IFormFile poster); //return task of type byte[], accepts argument
                                                         //IFormFile
        Task<byte[]> EncodeImageURLAsync(string imageURL); //will be responsible for taking in image path

        string DecodeImage(byte[] poster, string contentType); //return type string and accept byte[] and string
                                                               //responsible for pulling byte[] out of DB and turn
                                                               //into a string to be displayed inside of img tags
    }
}
