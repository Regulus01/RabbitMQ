namespace Publisher.Domain;

public class Payments
{
    public Guid Paymentid { get; set; }

    public Guid Relatedid { get; set; }

    public decimal Paymentamount { get; set; }

    public DateTimeOffset Paymentdate { get; set; }

    public string Paymentstatus { get; set; }
    
    public Payments(Guid relatedid, decimal paymentamount, DateTimeOffset paymentdate, string paymentstatus)
    {
        Paymentid = Guid.NewGuid();
        Relatedid = relatedid;
        Paymentamount = paymentamount;
        Paymentdate = paymentdate;
        Paymentstatus = paymentstatus;
    }
}
