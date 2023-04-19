using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;
using Post.Query.Infraestructure.DataAccess;

namespace Post.Query.Infraestructure.Repositories;

public class PostRepository : IPostRepository
{
    private DatabaseContextFactory _contextFactory;

    public PostRepository(DatabaseContextFactory contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task CreateAsync(PostEntity post)
    {
        using DatabaseContext context = _contextFactory.CreateDbContext();

        context.Posts.Add(post);

        _ = await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid postId)
    {
        using DatabaseContext context = _contextFactory.CreateDbContext();

        PostEntity? post = await GetByIdAsync(postId);

        if (post == null)
        {
            return;
        }

        context.Posts.Remove(post);

        _ = await context.SaveChangesAsync();
    }

    public async Task<PostEntity?> GetByIdAsync(Guid postId)
    {
        using DatabaseContext context = _contextFactory.CreateDbContext();

        return await context
            .Posts
            .Include(p => p.Comments)
            .FirstOrDefaultAsync(p => p.PostId == postId);
    }

    public async Task<List<PostEntity>> ListAllAsync()
    {
        using DatabaseContext context = _contextFactory.CreateDbContext();

        return await context
            .Posts
            .AsNoTracking()
            .Include(p => p.Comments)
            .ToListAsync();
    }

    public async Task<List<PostEntity>> ListByAuthorAsync(string author)
    {
        using DatabaseContext context = _contextFactory.CreateDbContext();

        return await context
            .Posts
            .AsNoTracking()
            .Include(p => p.Comments)
            .Where(p => p.Author == author)
            .ToListAsync();
    }

    public async Task<List<PostEntity>> ListWithCommentAsync()
    {
        using DatabaseContext context = _contextFactory.CreateDbContext();

        return await context
            .Posts
            .AsNoTracking()
            .Include(p => p.Comments)
            .Where(p => p.Comments != null && !p.Comments.Any())
            .ToListAsync();
    }

    public async Task<List<PostEntity>> ListWithLikesAsync(int numberOfLikes)
    {
        using DatabaseContext context = _contextFactory.CreateDbContext();

        return await context
            .Posts
            .AsNoTracking()
            .Include(p => p.Comments)
            .Where(p => p.Likes >= numberOfLikes)
            .ToListAsync();
    }

    public async Task UpdateAsync(PostEntity post)
    {
        using DatabaseContext context = _contextFactory.CreateDbContext();

        context.Posts.Update(post);

        _ = await context.SaveChangesAsync();
    }
}
