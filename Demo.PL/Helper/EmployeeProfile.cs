
using AutoMapper;
using Demo.PL.ViewModels;
using Demo.DAL.Models;

namespace Demo.PL.Helper
{
    public class EmployeeProfile : Profile
    {
        public EmployeeProfile()
        {
            CreateMap<Employee, EmployeeViewModel>()
                .ReverseMap();
        }
    }
}
