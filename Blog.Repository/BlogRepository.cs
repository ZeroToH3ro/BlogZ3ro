using System.Data;
using Blog.Models.Blog;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Blog.Repository;

public class BlogRepository : IBlogRepository
{
    private readonly IConfiguration _configuration;

    public BlogRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public async Task<Models.Blog.Blog?> UpsertAsync(BlogCreate blogCreate, int applicationUserId)
    {
        var dataTable = new DataTable();
        dataTable.Columns.Add("BlogId", typeof(int));
        dataTable.Columns.Add("Title", typeof(string));
        dataTable.Columns.Add("Content", typeof(string));
        dataTable.Columns.Add("PhotoId", typeof(int));

        dataTable.Rows.Add(blogCreate.BlogId, blogCreate.Title, blogCreate.Content, blogCreate.PhotoId);

        int? newBlogId;

        await using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        {
            await connection.OpenAsync().ConfigureAwait(false);

            newBlogId = await connection.ExecuteScalarAsync<int>(
                "Blog_Upsert",
                new { Blog = dataTable.AsTableValuedParameter("dbo.BlogType"), ApplicationUserId = applicationUserId },
                commandType: CommandType.StoredProcedure
            );
        }
        newBlogId ??= blogCreate.BlogId;
            
        Models.Blog.Blog? blog = await GetAsync(newBlogId.Value).ConfigureAwait(false);
        
        return blog;
    }

    public async Task<PagedResults<Models.Blog.Blog>> GetAllAsync(BlogPaging blogPaging)
    {
        var results = new PagedResults<Models.Blog.Blog>();

        await using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        {
            await connection.OpenAsync();

            await using (var multi = await connection.QueryMultipleAsync("Blog_GetAll",
                       new { 
                           Offset = (blogPaging.Page - 1) * blogPaging.PageSize,
                           PageSize = blogPaging.PageSize
                       }, 
                       commandType: CommandType.StoredProcedure))
            {
                results.Items = multi.Read<Models.Blog.Blog>();

                results.TotalCount = multi.ReadFirst<int>();
            }
        }

        return results;
    }

    public async Task<Models.Blog.Blog?> GetAsync(int blogId)
    {
        Models.Blog.Blog? blog = null;

        using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        {
            await connection.OpenAsync();
            blog = await connection.QuerySingleOrDefaultAsync<Models.Blog.Blog>("Blog_Get", new { BlogId = blogId }, commandType: CommandType.StoredProcedure);
        } 
        
        return blog;
    }

    public async Task<List<Models.Blog.Blog>> GetAllByUserIdAsync(int applicationUserId)
    {
        IEnumerable<Models.Blog.Blog> blogs;

        await using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        {
            await connection.OpenAsync();

            blogs = await connection.QueryAsync<Models.Blog.Blog>(
                "Blog_GetByUserId",
                new {ApplicationUserId = applicationUserId},
                commandType: CommandType.StoredProcedure 
                );
        }

        return blogs.ToList();
    }

    public async Task<List<Models.Blog.Blog>> GetAllFamousAsync()
    {
        IEnumerable<Models.Blog.Blog> blogFamous;

        await using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        {
            await connection.OpenAsync();

            blogFamous = await connection.QueryAsync<Models.Blog.Blog>(
                "Blog_GetAllFamous",
                new { },
                commandType: CommandType.StoredProcedure
            );
        }

        return blogFamous.ToList();
    }

    public async Task<int> DeleteAsync(int blogId)
    {
        int affectedRows = 0;
        
        await using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        {
            await connection.OpenAsync();

            affectedRows = await connection.ExecuteAsync(
                "Blog_Delete",
                new {BlogId = blogId},
                commandType: CommandType.StoredProcedure);
        }
        
        return affectedRows;
    }
}