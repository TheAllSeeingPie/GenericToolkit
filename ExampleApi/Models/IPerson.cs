using GenericToolkit.Core.EntityFramework;

namespace ExampleApi.Models
{
    public interface IPerson : IEntity
    {
        string Name { get; set; }
        int Age { get; set; }
    }
}
