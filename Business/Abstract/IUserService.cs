using Business.Dtos.UserDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
        public interface IUserService
    {
        Task<IEnumerable<UserReturnDto>> GetAllDoctorsAsync();
        Task<IEnumerable<UserReturnDto>> GetAllPatientAsync();
        Task<IEnumerable<UserReturnDto>> GetAllUsersAsync();
        Task<DoctorReturnDto> GetDoctorAsync(string id);
        Task<string> CreateDoctorAsync(DoctorCreateDto dto);
        Task<IEnumerable<DoctorSchdelueReturnDto>> GetDoctorSchedule(string doctorId);
    }
}
