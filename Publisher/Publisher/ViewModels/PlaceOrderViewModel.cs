namespace Publisher.ViewModels;

public class PlaceOrderViewModel
{
    public string Customername { get; set; }
    
    public ICollection<Guid> ProductsIds { get; set; }
}