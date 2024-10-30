using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.IO.Image;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace ImageToPDFApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageToPDFController : ControllerBase
    {
        [HttpPost("convert")]
        public async Task<IActionResult> ConvertImageToPDF([FromBody] IFormFile image)
        {
            // Check if the image is provided and has the correct content type
            if (image == null)
            {
                return BadRequest(new { error = "The image field is required." });
            }

            if (!(image.ContentType == "image/jpeg" || image.ContentType == "image/png"))
            {
                return BadRequest(new { error = "Invalid file format. Only JPG and PNG are supported." });
            }

            try
            {
                using var stream = new MemoryStream();
                await image.CopyToAsync(stream);
                stream.Position = 0;

                byte[] pdfBytes;

                // Create a PDF document with the image
                using (var pdfStream = new MemoryStream())
                using (var pdfWriter = new PdfWriter(pdfStream))
                {
                    var pdf = new PdfDocument(pdfWriter);
                    var document = new Document(pdf);

                    // Convert the image to iText's ImageData object
                    ImageData imageData = ImageDataFactory.Create(stream.ToArray());
                    var img = new Image(imageData);

                    // Fit the image to the PDF page
                    img.SetAutoScale(true);
                    document.Add(img);
                    document.Close();

                    pdfBytes = pdfStream.ToArray();
                }

                // Return the PDF as a file response
                return File(pdfBytes, "application/pdf", "ConvertedImage.pdf");
            }
            catch (Exception ex)
            {
                // Handle unexpected errors gracefully
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { error = $"An error occurred: {ex.Message}" });
            }
        }
    }
}
