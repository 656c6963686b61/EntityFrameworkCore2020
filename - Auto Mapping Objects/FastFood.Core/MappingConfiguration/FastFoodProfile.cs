namespace FastFood.Core.MappingConfiguration
{
    using System.Linq;
    using AutoMapper;
    using FastFood.Models;
    using ViewModels.Categories;
    using ViewModels.Employees;
    using ViewModels.Items;
    using ViewModels.Orders;
    using ViewModels.Positions;

    public class FastFoodProfile : Profile
    {
        public FastFoodProfile()
        {
            //Positions
            this.CreateMap<CreatePositionInputModel, Position>()
                .ForMember(x => x.Name, y => y.MapFrom(s => s.PositionName));

            this.CreateMap<Position, PositionsAllViewModel>()
                .ForMember(x => x.Name, y => y.MapFrom(s => s.Name));

            //Categories
            this.CreateMap<CreateCategoryInputModel, Category>()
                .ForMember(x => x.Name, opt => opt.MapFrom(x => x.CategoryName));

            this.CreateMap<Category, CategoryAllViewModel>()
                .ForMember(x => x.Name, opt => opt.MapFrom(x => x.Name));

            //Items
            this.CreateMap<CreateItemInputModel, Item>();

            this.CreateMap<Item, ItemsAllViewModels>()
                .ForMember(x => x.Category, opt => opt.MapFrom(x => x.Category.Name));

            this.CreateMap<Category, CreateItemViewModel>()
                .ForMember(x => x.CategoryId, opt => opt.MapFrom(x => x.Id));

            //Orders
            this.CreateMap<CreateOrderInputModel, Order>();

            this.CreateMap<Order, OrderAllViewModel>()
                .ForMember(x => x.OrderId, y => y.MapFrom(s => s.Id))
                .ForMember(x => x.Employee, y => y.MapFrom(s => s.Employee.Name));

            //Employees
            this.CreateMap<RegisterEmployeeInputModel, Employee>();

            this.CreateMap<Position, RegisterEmployeeViewModel>()
                .ForMember(x => x.PositionId, opt => opt.MapFrom(x => x.Id))
                .ForMember(x => x.PositionName, opt => opt.MapFrom(x => x.Name));

            this.CreateMap<Employee, EmployeesAllViewModel>()
                .ForMember(x => x.Position, opt => opt.MapFrom(x => x.Position.Name));
        }
    }
}
