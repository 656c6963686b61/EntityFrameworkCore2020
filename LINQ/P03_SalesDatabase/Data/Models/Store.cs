namespace P03_SalesDatabase.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using static DataValidations.Store;

    public class Store
    {
        public Store()
        {
            this.Sales = new List<Sale>();
        }

        [Key]
        public int StoreId { get; set; }

        [MaxLength(NameMaxLength)]
        [Required]
        public string Name { get; set; }

        public ICollection<Sale> Sales { get; set; }
    }
}
