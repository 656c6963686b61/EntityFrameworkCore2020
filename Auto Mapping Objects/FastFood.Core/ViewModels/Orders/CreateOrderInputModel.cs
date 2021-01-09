namespace FastFood.Core.ViewModels.Orders
{
    using System.ComponentModel.DataAnnotations;

    public class CreateOrderInputModel
    {
        public string Customer { get; set; }

        public int ItemId { get; set; }

        public int EmployeeId { get; set; }

        [Range(1, 100)]
        public int Quantity { get; set; }
    }
}
