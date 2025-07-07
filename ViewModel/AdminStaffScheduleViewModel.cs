using pet_spa_system1.Models;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace pet_spa_system1.ViewModel
{
    public class AdminStaffScheduleViewModel
    {
        public Appointment Appointment { get; set; }

        [ValidateNever]
        public List<User> StaffList { get; set; }

        [ValidateNever]
        public List<StatusAppointment> StatusList { get; set; }

        [ValidateNever]
        public List<Appointment> Appointments { get; set; }

        public int? FilterStaffId { get; set; }
        public DateTime? FilterDate { get; set; }
        public int? FilterStatusId { get; set; }
    }
} 