using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Post.Query.Domain.Entities;

[Table("Comment")]
public class CommentEntity
{
    [Key]
    public Guid CommentId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public DateTime CommentDate { get; set; }
    public string Comment { get; set; } = string.Empty;
    public bool Edited { get; set; }
    public Guid PostId { get; set; }

    [JsonIgnore]
    public virtual PostEntity Post { get; set; } = new PostEntity();
}
