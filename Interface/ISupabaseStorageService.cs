using System.Threading.Tasks;

namespace TravelDesk.Interface
{
    public interface ISupabaseStorageService
    {
        /// <summary>
        /// Uploads a PDF file to Supabase Storage and returns the download URL
        /// </summary>
        /// <param name="fileBytes">The PDF file bytes</param>
        /// <param name="fileName">The name of the file</param>
        /// <param name="folder">The folder path in Supabase Storage (optional)</param>
        /// <returns>The download URL for the uploaded file</returns>
        Task<string> UploadPdfAsync(byte[] fileBytes, string fileName, string folder = "pdfs");

        /// <summary>
        /// Generates a download URL for a file in Supabase Storage
        /// </summary>
        /// <param name="fileName">The name of the file</param>
        /// <param name="folder">The folder path in Supabase Storage (optional)</param>
        /// <returns>The download URL</returns>
        Task<string> GetDownloadUrlAsync(string fileName, string folder = "pdfs");

        /// <summary>
        /// Deletes a file from Supabase Storage
        /// </summary>
        /// <param name="fileName">The name of the file to delete</param>
        /// <param name="folder">The folder path in Supabase Storage (optional)</param>
        /// <returns>True if deletion was successful</returns>
        Task<bool> DeleteFileAsync(string fileName, string folder = "pdfs");
    }
} 