using System;

namespace Playbook.Service.Dtos
{
    public record ItemDto(Guid Id, string Name, string Description, decimal Price, DateTimeOffset CreatedDate);
    public record CreateItemDto(string Name, string Description, decimal Price);
    public record UpdateItemDto(string Name, string Reception, decimal Price);
}