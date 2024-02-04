namespace Publisher.ViewModels;

public class GetOrderViewModel
{
    public string Customername { get; set; }

    public DateTimeOffset Orderdate { get; set; }

    public string Orderstatus { get; set; }

    public virtual ICollection<GetProductOrderViewModel> Product { get; set; }
}