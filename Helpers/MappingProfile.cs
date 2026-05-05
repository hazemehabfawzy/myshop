using AutoMapper;
using System.Text.Json;
using TechVault.API.DTOs.Auth;
using TechVault.API.DTOs.Category;
using TechVault.API.DTOs.Customer;
using TechVault.API.DTOs.Order;
using TechVault.API.DTOs.Product;
using TechVault.API.DTOs.ServiceRequest;
using TechVault.API.DTOs.Tag;
using TechVault.API.Models.Entities;

namespace TechVault.API.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Auth
            CreateMap<RegisterDto, User>();
            CreateMap<User, AuthResponseDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.CustomerProfile != null ? src.CustomerProfile.FullName : ""));

            // Customer
            CreateMap<User, CustomerResponseDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.CustomerProfile != null ? src.CustomerProfile.FullName : ""))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.CustomerProfile != null ? src.CustomerProfile.PhoneNumber : ""))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.CustomerProfile != null ? src.CustomerProfile.Address : ""))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CustomerProfile != null ? src.CustomerProfile.CreatedAt : DateTime.MinValue));

            // Category
            CreateMap<Category, CategoryResponseDto>()
                .ForMember(dest => dest.ParentCategoryName, opt => opt.MapFrom(src => src.ParentCategory != null ? src.ParentCategory.Name : null))
                .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.Products.Count));
            CreateMap<CreateCategoryDto, Category>();
            CreateMap<UpdateCategoryDto, Category>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Product
            CreateMap<Product, ProductResponseDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.ProductTags.Select(pt => pt.Tag.Name)));
            CreateMap<CreateProductDto, Product>();
            CreateMap<UpdateProductDto, Product>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Tag
            CreateMap<Tag, TagResponseDto>()
                .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.ProductTags.Count));
            CreateMap<TagDto, Tag>();

            // Order
            CreateMap<Order, OrderResponseDto>();
            CreateMap<OrderItem, OrderItemResponseDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : ""));

            // ServiceRequest
            CreateMap<ServiceRequest, ServiceRequestResponseDto>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.User != null && src.User.CustomerProfile != null ? src.User.CustomerProfile.FullName : ""))
                .ForMember(dest => dest.TechnicianName, opt => opt.MapFrom(src => src.Technician != null && src.Technician.CustomerProfile != null ? src.Technician.CustomerProfile.FullName : ""));
            CreateMap<CreateServiceRequestDto, ServiceRequest>();
            CreateMap<UpdateServiceRequestDto, ServiceRequest>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
