using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Commands;

namespace Post.Cmd.Api.Commands;

public class AddCommentCommand : BaseCommand
{
    public string Comment { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
}