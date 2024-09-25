using System;
using System.ComponentModel.DataAnnotations;

namespace MiniBlog.Models
{
    public class Blog
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime ModifiedAt { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        [StringLength(255)]
        public string Status { get; set; }

        public byte[] Banner { get; set; }

        public string BlogOwnerId { get; set; }
        public ApplicationUser BlogOwner { get; set; }
    }
}