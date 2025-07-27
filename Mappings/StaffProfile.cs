// Mappings/StaffProfile.cs
using AutoMapper;
using ElderlyCareManagement.DTOs;
using ElderlyCareManagement.Models;

namespace ElderlyCareManagement.Mappings
{
    public class StaffProfile : Profile
    {
        public StaffProfile()
        {
            // StaffInfo 到 StaffInfoDTO 的映射
            CreateMap<StaffInfo, StaffInfoDTO>();
            
            // StaffCreateDTO 到 StaffInfo 的映射
            CreateMap<StaffCreateDTO, StaffInfo>();
            
            // StaffUpdateDTO 到 StaffInfo 的映射
            CreateMap<StaffUpdateDTO, StaffInfo>();
            
            // StaffInfo 到 StaffDetailDTO 的映射
            CreateMap<StaffInfo, StaffDetailDTO>();
        }
    }
}