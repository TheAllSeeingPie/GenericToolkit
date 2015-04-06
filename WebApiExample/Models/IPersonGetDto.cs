using GenericToolkit.Core.EntityFramework;

namespace WebApiExample.Models
{
    public interface IPersonGetDto : IEntity
    {
        string Name { get; set; }
    }
}