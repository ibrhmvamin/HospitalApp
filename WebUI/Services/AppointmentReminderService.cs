using Business.Abstract;

namespace WebUI.Services
{
    public class AppointmentReminderService
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IEmailService _emailService;
        //private readonly IRecurringJobManager _recurringJobManager;

        public AppointmentReminderService(IAppointmentService appointmentService, IEmailService emailService/*, IRecurringJobManager recurringJobManager*/)
        {
            _appointmentService = appointmentService;
            _emailService = emailService;
            //_recurringJobManager = recurringJobManager;
        }

        //public void ScheduleAppointmentReminders()
        //{
        //    _recurringJobManager.AddOrUpdate(
        //        "SendAppointmentReminders",
        //        () => CheckAndSendReminders()
        //    );
        //}

        public async Task CheckAndSendReminders()
        {
            var upcomingAppointments = await _appointmentService.GetAppointmentsStartingWithinOneHour();

            foreach (var appointment in upcomingAppointments)
            {
                string body = "";
                using (StreamReader stream = new("wwwroot/templates/reminder.html"))
                {
                    body = stream.ReadToEnd();
                };
                body = body.Replace("{{doctor}}", $"Dr. {appointment.Doctor.Name}");
                body = body.Replace("{{username}}", $"{appointment.Patient.Name} {appointment.Patient.Surname}");
                await _emailService.SendReminderEmailAsync(appointment.Patient.Email, "Appointment Reminder", body);
            }
        }
    }
}
