using Business.Dtos.AppointmentDto;
using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IAppointmentService
    {
        Task CreateAsync(AppointmentCreateDto createAppointmentDto, string patientId);
        Task DeleteAsync(string id);
        Task ChangeStatusAsync(string appointmentId, Status status);
        Task<IEnumerable<AppointmentReturnDto>> GetAppointmentsAsync(string userId);
        Task<List<Appointment>> GetAppointmentsStartingWithinOneHour();
        Task UpdateExpiredAppointmentsAsync();
    }
}
