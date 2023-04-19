using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;
using Post.Query.Infraestructure.DataAccess;

namespace Post.Query.Infraestructure.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly DatabaseContextFactory _contextFactory;
    public CommentRepository(DatabaseContextFactory contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task CreateAsync(CommentEntity comment)
    {
        using DatabaseContext context = _contextFactory.CreateDbContext();

        await context.AddAsync(comment);

        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid commentId)
    {
        using DatabaseContext context = _contextFactory.CreateDbContext();

        CommentEntity? comment = await GetByIdAsync(commentId);

        if (comment == null)
        {
            return;
        }

        context.Remove(comment);

        await context.SaveChangesAsync();
    }

    public async Task<CommentEntity?> GetByIdAsync(Guid commentId)
    {
        using DatabaseContext context = _contextFactory.CreateDbContext();

        return await context.Comments.FirstOrDefaultAsync(c => c.CommentId == commentId);
    }

    public async Task UpdateAsync(CommentEntity comment)
    {
        using DatabaseContext context = _contextFactory.CreateDbContext();

        context.Update(comment);

        await context.SaveChangesAsync();
    }
}