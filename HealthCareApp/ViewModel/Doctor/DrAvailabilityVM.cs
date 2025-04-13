using HealthCareApp.CustomValidation;
using HealthCareApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace HealthCareApp.ViewModel.Doctor
{
    public class DrAvailabilityVM
    {
        public int? id {  get; set; } = null;
        public DayOfWeek dayOfWeek { get; set; }
        [DataType(DataType.Time)]
        public TimeOnly StartTime { get; set; }

        //[Remote("ValidateEndTime" , "DoctorAvailability",AdditionalFields =nameof(StartTime) ,ErrorMessage = "End Time must be after Start Time.")]
        [ValidateEndTime(nameof(StartTime),nameof(IsAvailable))]
        [DataType(DataType.Time)]
        public TimeOnly EndTime { get; set; }

        [DataType(DataType.Date)]
        public DateOnly Date { get; set; }

        [Range(0, int.MaxValue  , ErrorMessage ="Waiting Time must be positive ")]
        public int Duration { get; set; }
        public Models.Type mode { get; set; }
        public int ClinicId { get; set; }
        public string ?DrId {  get; set; } 
    
        public bool IsAvailable { get; set; }

        public DrAvailabilityVM(Availability availability)
        {
            SetDrAvailabilityVM(availability);
        }
        public DrAvailabilityVM()
        { }
        
        public Availability GetAvailability()
        {
            return new Availability()
            {
                ClinicId = ClinicId,
                Date = Date,
                Duration = Duration,
                dayOfWeek =   dayOfWeek,
                type = mode,
                StartTime = StartTime,
                EndTime = EndTime,
                DoctorId = DrId

            };
        }
        public void SetDrAvailabilityVM(Availability availability)
        { 
            id= availability.Id;
            dayOfWeek = availability.dayOfWeek;
            StartTime = availability.StartTime;
            EndTime = availability.EndTime;
            Date = availability.Date;
            Duration = availability.Duration;
            mode = availability.type;
            ClinicId = availability.ClinicId;
            IsAvailable = true;
            DrId = availability.DoctorId;
        }
    }
}
