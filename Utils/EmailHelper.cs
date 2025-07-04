using System;
using System.IO;
using System.Web;
using Microsoft.AspNetCore.Hosting;

namespace pet_spa_system1.Utils
{
    public class EmailHelper
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EmailHelper(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// Gets the CSS content from file and returns it as string
        /// </summary>
        /// <param name="cssRelativePath">The relative path to the CSS file</param>
        /// <returns>The CSS content as string or empty string if file not found</returns>
        public string GetCssContent(string cssRelativePath)
        {
            try
            {
                var cssFullPath = Path.Combine(_webHostEnvironment.WebRootPath, cssRelativePath.TrimStart('~', '/'));
                if (File.Exists(cssFullPath))
                {
                    return File.ReadAllText(cssFullPath);
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                // Log exception
                Console.WriteLine($"Error reading CSS file: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// Embeds CSS content into an HTML email template
        /// </summary>
        /// <param name="htmlContent">The HTML content of the email</param>
        /// <param name="cssContent">The CSS content to embed</param>
        /// <returns>HTML with embedded CSS</returns>
        public string EmbedCssInHtml(string htmlContent, string cssContent)
        {
            if (string.IsNullOrEmpty(cssContent))
                return htmlContent;

            // Add the CSS to a style tag in the head
            var styleTag = $"<style type=\"text/css\">{cssContent}</style>";
            
            // Insert the style tag into the head
            return htmlContent.Replace("<link rel=\"stylesheet\" href=\"@Url.Content(\"~/cssjsAppointment/email-confirmation.css\")\" />", styleTag);
        }
    }
}
