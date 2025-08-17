using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class Employee : Entity
{
    [Column("name")]
    public string Name { get; set; } = null!;

    public override string ToString()
    {
        return $"Employee: Id - {Id} Name - {Name}";
    }
}