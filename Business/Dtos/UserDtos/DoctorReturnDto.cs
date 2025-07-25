﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Dtos.UserDtos
{
    public class DoctorReturnDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Profile { get; set; }
        public DateTime BirthDate { get; set; }
        public IEnumerable<string> Statuses { get; set; }
        public bool IsBanned { get; set; }
        public DateTime? BannedUntil { get; set; }
    }
}
