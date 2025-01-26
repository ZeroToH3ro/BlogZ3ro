namespace Blog.Models.Blog;

public class PagedResults<T>
{
    public IEnumerable<T> Items { get; set; }
    public int TotalCount { get; set; }
}