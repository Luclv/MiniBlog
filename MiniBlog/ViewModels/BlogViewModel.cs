using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MiniBlog.ViewModels
{
    public class BlogViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        [Required]
        [AllowHtml]
        public string Content { get; set; }

        public string Status { get; set; }

        public byte[] Banner { get; set; }

        public SelectList StatusSelectList =>
            new SelectList(
                new List<SelectListItem>
                {
                    new SelectListItem { Text = "Draft", Value = ((int)StatusEnum.Draft).ToString()},
                    new SelectListItem { Text = "Published", Value = ((int) StatusEnum.Published).ToString()},
                    new SelectListItem { Text = "Rejected", Value = ((int) StatusEnum.Rejected).ToString()}
                }, "Value", "Text");
    }

    public enum StatusEnum
    {
        Draft,
        Published,
        Rejected
    }
}