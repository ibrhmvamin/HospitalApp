﻿using Business.Dtos.UserDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
        public interface IUserService
    {
        Task<IEnumerable<GetDoctorDto>> GetAllDoctorsAsync();
        Task<IEnumerable<GetUserDto>> GetAllPatientAsync();
        Task<IEnumerable<UserReturnDto>> GetAllUsersAsync();
        Task<UserReturnDto> GetPatientAsync(string id);
        Task<object> GetUserProfileAsync(string userId);
        Task<DoctorReturnDto> GetDoctorAsync(string id);
        Task<string> CreateDoctorAsync(DoctorCreateDto dto);
        Task<IEnumerable<DoctorSchdelueReturnDto>> GetDoctorSchedule(string doctorId);
        Task DeleteDoctorAsync(string id);

    }
}
