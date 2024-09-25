using System;

namespace MiniBlog.ViewModels
{
    public class ShowAllBlogsViewModel
    {
        public PagedList.IPagedList<BlogListViewModel> BlogList { get; set; }

        public string ViewOrEdit { get; set; }
    }

    public class BlogListViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public string BlogOwnerId { get; set; }

        public string Owner { get; set; }
    }
}