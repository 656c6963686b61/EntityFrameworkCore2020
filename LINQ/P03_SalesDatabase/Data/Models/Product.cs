namespace P03_SalesDatabase.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using static DataValidations.Product;
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        [MaxLength(DescriptionMaxLength)]
        public string Description { get; set; }
    }
}
