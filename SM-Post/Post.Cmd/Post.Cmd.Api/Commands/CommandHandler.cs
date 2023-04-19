using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Handlers;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Api.Commands;

public class CommandHandler : ICommandHandler
{
    private readonly IEventSourcingHandler<PostAggregate> _eventSourcingHandler;

    public CommandHandler(IEventSourcingHandler<PostAggregate> eventSourcingHandler)
    {
        _eventSourcingHandler = eventSourcingHandler;
    }

    public async Task HandleAsync(NewPostCommand command)
    {
        PostAggregate aggregate = new PostAggregate(command.Id, command.Author, command.Message);

        await _eventSourcingHandler.SaveAsync(aggregate);
    }

    public async Task HandleAsync(EditMessageCommand command)
    {
        PostAggregate aggregate = await _eventSourcingHandler.GetByIdAsync(command.Id);

        aggregate.EditMessage(command.Message);

        await _eventSourcingHandler.SaveAsync(aggregate);
    }

    public async Task HandleAsync(LikePostCommand command)
    {
        PostAggregate aggregate = await _eventSourcingHandler.GetByIdAsync(command.Id);

        aggregate.LikePost();

        await _eventSourcingHandler.SaveAsync(aggregate);
    }

    public async Task HandleAsync(AddCommentCommand command)
    {
        PostAggregate aggregate = await _eventSourcingHandler.GetByIdAsync(command.Id);

        aggregate.AddComment(command.Comment, command.UserName);

        await _eventSourcingHandler.SaveAsync(aggregate);
    }

    public async Task HandleAsync(EditCommentCommand command)
    {
        PostAggregate aggregate = await _eventSourcingHandler.GetByIdAsync(command.Id);

        aggregate.EditComment(command.CommentId, command.Comment, command.UserName);

        await _eventSourcingHandler.SaveAsync(aggregate);
    }

    public async Task HandleAsync(RemoveCommentCommand command)
    {
        PostAggregate aggregate = await _eventSourcingHandler.GetByIdAsync(command.Id);

        aggregate.RemoveComment(command.CommentId, command.UserName);

        await _eventSourcingHandler.SaveAsync(aggregate);
    }

    public async Task HandleAsync(DeletePostCommand command)
    {
        PostAggregate aggregate = await _eventSourcingHandler.GetByIdAsync(command.Id);

        aggregate.DeletePost(command.UserName);

        await _eventSourcingHandler.SaveAsync(aggregate);
    }
}
