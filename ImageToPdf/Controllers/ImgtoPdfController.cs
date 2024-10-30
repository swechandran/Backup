using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.IO.Image;
using iText.Kernel.Pdf.Xobject;

[ApiController]
[Route("api/[controller]")]
public class ImageToPdfController : ControllerBase
{
    [HttpPost("convert")]
    public async Task<IActionResult> ConvertImageToPdf(IFormFile imageFile)
    {
        // Check if the file is not null
        if (imageFile == null || imageFile.Length == 0)
        {
            return BadRequest("No file was provided.");
        }

        // Create a MemoryStream to hold the PDF data
        using (var pdfStream = new MemoryStream())
        {
            // Create a PDF document
            using (PdfWriter writer = new PdfWriter(pdfStream))
            using (PdfDocument pdfDoc = new PdfDocument(writer))
            {
                Document document = new Document(pdfDoc);

                // Read the image file into a byte array
                using (var ms = new MemoryStream())
                {
                    await imageFile.CopyToAsync(ms);
                    var imageBytes = ms.ToArray();

                    // Convert byte array to ImageData
                    var imageData = ImageDataFactory.Create(imageBytes);

                    // Create a PdfImageXObject from ImageData
                    PdfImageXObject pdfImageXObject = new PdfImageXObject(imageData);

                    // Create an Image object from the PdfImageXObject
                    Image image = new Image(pdfImageXObject);

                    // Add the image to the document
                    document.Add(image);
                }

                // Close the document
                document.Close();
            }

            // Return the PDF as a file result
            return File(pdfStream.ToArray(), "application/pdf", "converted.pdf");
        }
    }
}