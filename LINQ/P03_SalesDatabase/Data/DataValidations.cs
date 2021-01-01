namespace P03_SalesDatabase.Data
{
    using System.Dynamic;

    public class DataValidations
    {
        public class Product
        {
            public const int NameMaxLength = 50;
            public const int DescriptionMaxLength = 250;
        }

        public class Store
        {
            public const int NameMaxLength = 80;
        }

        public class Customer
        {
            public const int NameMaxLength = 100;
            public const int EmailMaxLength = 80;
        }
    }
}
