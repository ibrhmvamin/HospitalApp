using AutoMapper;
using Business.Abstract;
using Business.Dtos.AppointmentDto;
using Business.Exceptions;
using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly UserManager<AppUser> _userManager;

        public AppointmentService(IMapper mapper, DataContext context, UserManager<AppUser> userManager)
        {
            _mapper = mapper;
            _context = context;
            _userManager = userManager;
        }

        public async Task ChangeStatusAsync(string appointmentId, Status status)
        {
            Appointment? appointment = await _context.Appointments.SingleOrDefaultAsync(a => a.Id.ToString() == appointmentId);
            if (appointment == null) throw new CustomException(404, "Appointment does not exist");
            if (appointment.StartTime < DateTime.Now) throw new CustomException(400, "Appointment is inactive");
            appointment.Status = status;
            var patientId = appointment.PatientId;
            await _context.SaveChangesAsync();
        }

        public async Task CreateAsync(AppointmentCreateDto createAppointmentDto, string patientId)
        {
            AppUser? patient = await _context.Users.SingleOrDefaultAsync(u => u.Id == patientId);
            if (patient is null || !await _userManager.IsInRoleAsync(patient, "member")) throw new CustomException(404, "Patient does not exist");

            DateTime StartTime = createAppointmentDto.StartTime;
            DateTime EndTime = StartTime + TimeSpan.FromMinutes(30);

            AppUser? doctor = await _context.Users.SingleOrDefaultAsync(u => u.Id == createAppointmentDto.DoctorId);
            if (doctor is null || !await _userManager.IsInRoleAsync(doctor, "doctor")) throw new CustomException(404, "Doctor does not exist");

            if (StartTime < DateTime.Now || EndTime <= DateTime.Now) throw new CustomException(400, "Invalid date");

            if (await _context.Appointments.AnyAsync(a => a.StartTime < EndTime && a.EndTime > StartTime)) throw new CustomException(400, "Busy schedule");

            Appointment appointment = _mapper.Map<Appointment>(createAppointmentDto);
            appointment.Status = Status.PENDING;
            appointment.StartTime = StartTime;
            appointment.EndTime = EndTime;
            appointment.PatientId = patientId;

            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            Appointment? appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) throw new CustomException(404, "Appointment not found");
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<AppointmentReturnDto>> GetAppointmentsAsync(string userId)
        {
            IEnumerable<Appointment> appointments = await _context.Appointments.Where(a => a.PatientId == userId || a.DoctorId == userId).Include(a => a.Doctor).Include(a => a.Patient).ToListAsync();
            IEnumerable<AppointmentReturnDto> returnDto = _mapper.Map<IEnumerable<AppointmentReturnDto>>(appointments);
            return returnDto;
        }

        public async Task<List<Appointment>> GetAppointmentsStartingWithinOneHour()
        {
            var oneHourFromNow = DateTime.Now.AddHours(1);

            return await _context.Appointments
                .Where(a => a.StartTime <= oneHourFromNow && a.StartTime > DateTime.Now)
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task UpdateExpiredAppointmentsAsync()
        {
            var now = DateTime.Now;
            var pendingAppointments = await _context.Appointments
                .Where(a => a.Status == Status.PENDING && a.StartTime < now)
                .ToListAsync();

            foreach (var appointment in pendingAppointments) appointment.Status = Status.REJECTED;

            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<AppointmentReturnDto>> GetAllAppointmentsAsync()
        {
            var appointments = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .OrderByDescending(a => a.StartTime)
                .ToListAsync();

            return _mapper.Map<IEnumerable<AppointmentReturnDto>>(appointments);
        }
    }
}

