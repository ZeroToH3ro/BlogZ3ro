using Blog.Models.Photo;

namespace Blog.Repository;

public interface IPhotoRepository
{
    public Task<Photo> InsertAsync(PhotoCreate photoCreate, int applicationUserId);

    public Task<Photo> GetAsync(int photoId);

    public Task<List<Photo>> GetAllByUserIdAsync (int applicationUserId);

    public Task<int> DeletetAsync(int photoId);
}