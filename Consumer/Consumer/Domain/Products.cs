namespace Consumer.Domain;

public class Products
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public decimal Price { get; set; }

    public virtual ICollection<Orders> Order { get; set; }
}
