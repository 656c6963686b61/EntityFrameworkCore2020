
namespace ProductShop.DTOs
{
    using System.Collections.Generic;

    public class UserSoldProducts
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        
        public ICollection<ProductSoldProducts> soldProducts { get; set; }
    }
}
