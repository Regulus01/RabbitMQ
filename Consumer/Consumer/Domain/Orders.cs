namespace Consumer.Domain;

public class Orders
{
    public Guid Id { get; set; }

    public string Customername { get; set; }

    public DateTimeOffset Orderdate { get; set; }

    public string Orderstatus { get; set; }

    public virtual ICollection<Products> Product { get; set; }
    
    public Orders(Guid id, string customername, DateTimeOffset orderdate, string orderstatus)
    {
        Id = id;
        Customername = customername;
        Orderdate = orderdate;
        Orderstatus = orderstatus;
    }
}
