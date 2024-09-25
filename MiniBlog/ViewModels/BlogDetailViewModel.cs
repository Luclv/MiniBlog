using System.ComponentModel.DataAnnotations;

namespace MiniBlog.ViewModels
{
    public class BlogDetailViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public string Status { get; set; }

        public byte[] Banner { get; set; }
    }
}