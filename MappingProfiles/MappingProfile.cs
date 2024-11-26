using AutoMapper;
using BookingService.Models;
using BookingService.DTOs;
using BCrypt.Net;

namespace BookingService.MappingProfiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => BCrypt.Net.BCrypt.HashPassword(src.Password)))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => Roles.User));

            CreateMap<User, UserDto>();
            CreateMap<UserUpdateDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

            CreateMap<BusinessCreateDto, Business>();
            CreateMap<BusinessUpdateDto, Business>();

            CreateMap<Business, BusinessDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.Nip, opt => opt.MapFrom(src => src.Nip))
                .ForMember(dest => dest.Regon, opt => opt.MapFrom(src => src.Regon))
                .ForMember(dest => dest.Krs, opt => opt.MapFrom(src => src.Krs))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.PrimaryImage, opt => opt.MapFrom(src => src.Images != null ? src.Images.FirstOrDefault(i => i.IsPrimary) : null));

            CreateMap<Business, BusinessListDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.PrimaryImage, opt => opt.MapFrom(src => src.Images.FirstOrDefault(i => i.IsPrimary)))
                .ForMember(dest => dest.FeaturedServices, opt => opt.MapFrom(src => src.Services.Where(s => s.IsFeatured)));

            CreateMap<Business, BusinessDetailDto>();

            CreateMap<BusinessImage, BusinessImageDto>();

            CreateMap<WorkingHoursUpdateDto, WorkingHours>();
            CreateMap<WorkingHours, WorkingHoursDto>();

            CreateMap<AddressDto, Address>();
            CreateMap<Address, AddressDto>();

            CreateMap<EmployeeCreateDto, Employee>();
            CreateMap<EmployeeUpdateDto, Employee>();
            CreateMap<Employee, EmployeeDto>();

            CreateMap<Service, ServiceDto>();
            CreateMap<ServiceCreateDto, Service>();
            CreateMap<ServiceUpdateDto, Service>();

            CreateMap<Booking, BookingListDto>()
                .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Service.Name))
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Service.Employee.Name))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Service.Price))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Service.Duration))
                .ForMember(dest => dest.DateTime, opt => opt.MapFrom(src => src.DateTime));

            CreateMap<Booking, BookingDetailDto>()
                .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Service.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Service.Description))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Service.Price))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Service.Duration))
                .ForMember(dest => dest.BusinessId, opt => opt.MapFrom(src => src.Service.Business.Id))
                .ForMember(dest => dest.BusinessName, opt => opt.MapFrom(src => src.Service.Business.Name))
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Service.Employee.Name))
                .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.DateTime, opt => opt.MapFrom(src => src.DateTime))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

            CreateMap<BookingCreateDto, Booking>();
            CreateMap<BookingUpdateDto, Booking>();
            
        }
    }
}
