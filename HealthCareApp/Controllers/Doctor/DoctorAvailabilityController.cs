using HealthCareApp.Models;
using HealthCareApp.RepositoryServices;
using HealthCareApp.ViewModel.Doctor;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;
using System.Linq.Expressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HealthCareApp.Controllers.Doctor
{
    public class DoctorAvailabilityController : Controller
    {
       public IGenericRepoServices<Notification> NotificationRepository {  get; }
        public IAvailabilityRepository AvailabilityRepository { get; }
        public IGenericRepoServices<Clinic> ClinicRepository { get; }
        public IGenericRepoServices<AvailabilitySlots> SlotRepository { get; }
        int AvailabilityDays = 14;
        private readonly IGenericRepoServices<Models.Doctor> doctorRepository;

        public DoctorAvailabilityController( IAvailabilityRepository availabilityRepository, IGenericRepoServices<Clinic> ClinicRepository, IGenericRepoServices<AvailabilitySlots> SlotRepository,IGenericRepoServices<Notification>NotificationRepository , IGenericRepoServices<HealthCareApp.Models.Doctor> doctorRepository) {
            this.AvailabilityRepository = availabilityRepository;
            this.ClinicRepository = ClinicRepository;
            this.SlotRepository = SlotRepository;
            this.NotificationRepository = NotificationRepository;
            this.doctorRepository = doctorRepository;
        }

        public IActionResult DisplayDaysSlots(string DrId="1")
        {
            IEnumerable<AvailabilityWithSlotVM> drAvailabilities = AvailabilityRepository.FindAllWithSelect(v => v.DoctorId == DrId, v => new AvailabilityWithSlotVM()
            {
                AvailabilityDate = v.Date,
                AvailabilityId = v.Id,
                AvailabilityType = v.type,
                ClinicName = $"{v.Clinic.Name} ({v.Clinic.ClinicRegion})",
                Slots = v.AvailableSlots.Select(s => new Slot()
                {
                   EndTime = s.EndTime,
                   IsBooked = s.IsBooked,
                   SlotId=s.Id,
                   StartTime = s.StartTime,    
                })
            }).OrderBy(v=>v.AvailabilityDate);
            return View(drAvailabilities);
        }
        
        public IActionResult GetAllAvailabilitiesForAllDr()
        {
            IEnumerable<DrWithAvailabilityVM> drWithAvailabilities = GetDrWithAvailabilities();

            ViewBag.drNames = doctorRepository.FindAllWithSelect(null,d => $"{d.FirstName} {d.LastName}").Distinct();
            return View(drWithAvailabilities);
        }
        private IEnumerable<DrWithAvailabilityVM> GetDrWithAvailabilities(string drname="")
        {
            Expression<Func<Availability, bool>> criteria=null;
            if (drname != "")
            {
                var nameParts = drname.Split(" ");
                var firstName = nameParts[0];
                var lastName = nameParts.Length > 1 ? nameParts[1] : "";
                criteria = v => v.Doctor.FirstName == firstName && v.Doctor.LastName == lastName;
            }
            
           var datat= AvailabilityRepository.FindAllWithSelect(criteria,v => new DrWithAvailabilityVM()
                {
                    DrId = v.DoctorId,
                    name = $"{v.Doctor.FirstName} {v.Doctor.LastName}",
                    SpecializationName = v.Doctor.Specialization.Name,
                    GetAvailabilityForDrVM = new GetAvailabilityForDrVM()
                    {
                        AvailableSlotsCnt = v.AvailableSlots.Count(v => !v.IsBooked),
                        AppointmentCnt = v.AvailableSlots.Count(v => v.IsBooked),
                        ClinicName = $"{v.Clinic.Name} ({v.Clinic.ClinicRegion}) ",
                        Date = v.Date,
                        dayOfWeek = v.dayOfWeek,
                        DoctorId = v.DoctorId,
                        Duration = v.Duration,
                        StartTime = v.StartTime,
                        EndTime = v.EndTime,
                        TimeRange = $"{v.StartTime} - {v.EndTime}",
                        Id = v.Id,
                        type = v.type,
                    }
                });
            return datat;
       }


        
        [HttpPost]
        public IActionResult GetAllAvailabilitiesForAllDr(string drNames)
        {
            ViewBag.drNames = doctorRepository.FindAllWithSelect(null,d => $"{d.FirstName} {d.LastName}").Distinct();
            ViewBag.selectName = drNames;
            return View(GetDrWithAvailabilities(drNames));
        }
        [HttpGet]
        public IActionResult GetAvailabilitiesForDr(string id)
        {
            //??isDeleted in AVibility?
            //??diplay avaibiliy of today and future? not past ?or doctor need data for past
            var drAvailabilities = AvailabilityRepository.FindAllWithSelect(v => v.DoctorId == id
            , v => new GetAvailabilityForDrVM
            {
                AvailableSlotsCnt = v.AvailableSlots.Count(v=>!v.IsBooked),
                AppointmentCnt = v.AvailableSlots.Count(v=>v.IsBooked),
                ClinicName = $"{v.Clinic.Name} ({v.Clinic.ClinicRegion}) ",
                Date = v.Date,
                dayOfWeek = v.dayOfWeek,
                DoctorId = id,
                Duration = v.Duration,
                StartTime = v.StartTime,
                EndTime = v.EndTime,
                TimeRange = $"{v.StartTime} - {v.EndTime}",  
                 Id = v.Id,
                type = v.type,     
            });

            ViewBag.DoctorId = id;
            return View(drAvailabilities);
        }
       /////////////////////////////////////////////////////////
       

        private GetAvailabilityForDrVM GetAvailabilityForDrVM(int availabilityId)
        {
            var drAvailabilities = AvailabilityRepository.FindWithSelect(v => v.Id == availabilityId, v => new GetAvailabilityForDrVM
            {
                AvailableSlotsCnt = v.AvailableSlots.Count(v => !v.IsBooked),
                AppointmentCnt = v.AvailableSlots.Count(v => v.IsBooked),
                ClinicName = $"{v.Clinic.Name} ({v.Clinic.ClinicRegion}) ",
                Date = v.Date,
                dayOfWeek = v.dayOfWeek,
                DoctorId = v.DoctorId,
                Duration = v.Duration,
                StartTime = v.StartTime,
                EndTime = v.EndTime,
                TimeRange = $"{v.StartTime} - {v.EndTime}",
                Id = v.Id,
                type = v.type,
            });
            return drAvailabilities;
        }
         public IActionResult DeleteAvailability(int availabilityId)
         {
            var drAvailabilities = GetAvailabilityForDrVM(availabilityId);
            return View(drAvailabilities);

         }


        public IActionResult CancelDay(int availabilityId)
        {
            Availability availability = AvailabilityRepository.Find(v => v.Id == availabilityId, v => v.AvailableSlots, v => v.AvailableSlots);
            string drId = availability.DoctorId;
            SlotRepository.HardDeleteRange(availability.AvailableSlots);
            AvailabilityRepository.HardDelete(availability);
            return RedirectToAction(nameof(GetAvailabilitiesForDr), new { id = drId });

        }

        public IActionResult RescheduleAvailability(int availabilityId)
        {
            DateOnly dateToday = DateOnly.FromDateTime(DateTime.Now);
            IEnumerable<DateOnly> currentAvailabilities = AvailabilityRepository.FindAllWithSelect(
                v=>v.Date>=dateToday,
                v=>v.Date
            );

            IEnumerable<Item<DateOnly, string>> ScheduleDays = GetScheduleDays(currentAvailabilities).Select(d =>
                new Item<DateOnly, string>()
                {
                    Id = d,
                    Name = $"{d.DayOfWeek.ToString()}, {d:MMMM dd, yyyy}"
                }
            );
            ViewBag.ScheduleDays= ScheduleDays;
            ViewBag.availabilityId = availabilityId;
            ViewBag.AvailabilityInfo = GetAvailabilityForDrVM(availabilityId);

            return View();

        }
        [HttpPost]
        public IActionResult RescheduleAvailability(Item<DateOnly,int> RescheduleAvailabilityDate, int previousAvailabilityId)
        {
            Availability previousAvailability = AvailabilityRepository.GetAvailabilitySlotsAppointment(previousAvailabilityId);
            
            List<Notification>notifications = new List<Notification>();
            Availability availability = new Availability()
            {
                AvailableSlots = new List<AvailabilitySlots>(),
                ClinicId = previousAvailability.ClinicId,
                Date = RescheduleAvailabilityDate.Id, //id is date 
                dayOfWeek = previousAvailability.dayOfWeek,
                Duration = previousAvailability.Duration,
                DoctorId = previousAvailability.DoctorId,
                EndTime = previousAvailability.EndTime,
                StartTime = previousAvailability.StartTime,
                type = previousAvailability.type,

            };
            DateTime todayTime = DateTime.Now;////////////////////////////////////////////////////
            foreach (var slot in previousAvailability.AvailableSlots) {
                availability.AvailableSlots.Add(slot);
                if (slot.IsBooked) {
                    notifications.Add(new Notification()
                    {
                        CreatedDate = todayTime,
                        Message = "Doctor Reschedule Appointment",
                        UserId = slot.Appointment.PatientId
                    });
                }
            }
            AvailabilityRepository.Add(availability);
            previousAvailability.AvailableSlots.Clear();
            AvailabilityRepository.HardDelete(previousAvailability);

            //NotificationRepository.(notifications)
            NotificationRepository.AddRange(notifications);

            return RedirectToAction(nameof(GetAvailabilitiesForDr), new { id = availability.DoctorId });

        }
        private IEnumerable<DateOnly> GetScheduleDays(IEnumerable<DateOnly> currentAvailabilities)
        {
            List<DateOnly> ScheduleDays = new List<DateOnly>();
            DateOnly dateToday = DateOnly.FromDateTime(DateTime.Now);

            for (int i = 0; i < 14; i++)
            {
                DateOnly currentDay = dateToday.AddDays(i);
                if (!currentAvailabilities.Contains(currentDay))
                    ScheduleDays.Add(currentDay);
            }
            return ScheduleDays;
        }
        [HttpGet]
        public IActionResult ViewSlots( string drId,int availabilityId )
        {
            List<ViewSlotVM> Slots = SlotRepository.FindAllWithSelect(
                s => s.AvailabilityId == availabilityId,
                s => new ViewSlotVM()
                {
                    TimeRange = $"{s.StartTime} - {s.EndTime}",
                    PatientName = (s.Appointment==null)?"-": s.Appointment.PatientName,
                    PatientNumber = (s.Appointment == null) ? "-" : s.Appointment.PatientPhone,
                    IsBooked = s.IsBooked,
                    Status = (s.IsBooked) ? "Booked" : "Available",
                    AppointmentId = (s.Appointment == null) ? null : s.Appointment.Id,
                    SlotId = s.Id,
                    AvailabilityId = s.AvailabilityId
                }
            ).ToList();
            ViewBag.drId = drId;
            return View(Slots);
        }

        //url of ajax call
        [HttpPost]
        public IActionResult GetSlots(int AvailableId, int OldSlotId)
        {
           var avail =AvailabilityRepository.GetById(AvailableId);
            TimeOnly timeNow = TimeOnly.FromDateTime(DateTime.Now);
            DateOnly dateToday = DateOnly.FromDateTime(DateTime.Now);
            List<Item<int, string>> Slots;
            if (avail.Date==dateToday)
            {
                Slots= SlotRepository.FindAllWithSelect(
                  s => s.AvailabilityId == AvailableId && s.Id != OldSlotId&&s.StartTime>timeNow&&!s.IsBooked,
                  s => new Item<int, string>
                  {
                      Name = $"{s.StartTime} - {s.EndTime}",
                      Id = s.Id,
                  }
              ).ToList();
            }
            else
            {
                Slots = SlotRepository.FindAllWithSelect(
                  s => s.AvailabilityId == AvailableId && s.Id != OldSlotId&&!s.IsBooked,
                  s => new Item<int, string>
                  {
                      Name = $"{s.StartTime} - {s.EndTime}",
                      Id = s.Id,
                  }
              ).ToList();
            }

            return Json(Slots);
        }
        public IActionResult CancelSlot(int slotId )
        {
            ViewSlotVM Slot = SlotRepository.FindWithSelect(s => s.Id == slotId,s => new ViewSlotVM()
            {
                TimeRange = $"{s.StartTime} - {s.EndTime}",
                PatientName = (s.Appointment == null) ? "-" : s.Appointment.PatientName,
                PatientNumber = (s.Appointment == null) ? "-" : s.Appointment.PatientPhone,
                IsBooked = s.IsBooked,
                Status = (s.IsBooked) ? "Booked" : "Available",
                AppointmentId = (s.Appointment == null) ? null : s.Appointment.Id,
                SlotId = s.Id,
                AvailabilityId = s.AvailabilityId

            });

            return View(Slot);
        }


        public IActionResult CancelSlotPost(int slotId)
        {
            AvailabilitySlots slot = SlotRepository.Find(s => s.Id == slotId, s => s.Appointment);
            /***************************************** Notification for patient about the cancelation ********************************************/
            /***************************************** how dealing with digital payment ********************************************/
            if(slot != null)
                SlotRepository.HardDelete(slot); //delete slot with appoiment 

            return RedirectToAction(nameof(ViewSlots), new { availabilityId  =slot.AvailabilityId });
        }
        public IActionResult RescheduleAppointment(int slotId)
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            AvailabilitySlots oldSlot = SlotRepository.Find(s => s.Id == slotId);
            List<Item<int,DateOnly>> AvailabeDays = AvailabilityRepository.FindAllWithSelect(
                v => v.Date >= today && v.AvailableSlots.Any(s => !s.IsBooked),
                v => new Item<int, DateOnly>() { Id = v.Id , Name = v.Date}
            ).OrderBy(t=>t.Name).ToList();

            List<Item<int, string>> DrAvailabeDays = new List<Item<int, string>>();
            for (int i = 0; i < AvailabeDays.Count; i++)
                DrAvailabeDays.Add(new Item<int, string>()
                {
                    Id = AvailabeDays[i].Id,
                    Name = $"{AvailabeDays[i].Name.DayOfWeek.ToString()}, {AvailabeDays[i].Name:MMMM dd, yyyy}",
                });
            ViewBag.DrAvailabeDays = DrAvailabeDays;
            Slot_AvailbilityVM slot_AvailbilityVM = new Slot_AvailbilityVM()
            {
                OldSlotId = slotId
            };
            return View(slot_AvailbilityVM);
        }

        [HttpPost]
        public IActionResult RescheduleAppointment(Slot_AvailbilityVM slot_Availbility )
        {
            var OldSlot = SlotRepository.Find(s => s.Id == slot_Availbility.OldSlotId,s=>s.Appointment,s=>s.Availability);
            var NewSlot = SlotRepository.Find(s => s.Id == slot_Availbility.NewSlotId);

            OldSlot.Appointment.AvailableSlot = NewSlot;
            NewSlot.IsBooked = true;

            int oldAvailbility = OldSlot.AvailabilityId;
            string drId = OldSlot.Availability.DoctorId;
            int CntSlots = SlotRepository.Count(s => s.AvailabilityId == oldAvailbility && s.Id!=OldSlot.Id);
            SlotRepository.HardDelete(OldSlot);

            if (CntSlots == 0)
                AvailabilityRepository.HardDelete( AvailabilityRepository.Find(v=>v.Id== oldAvailbility));

            /***************************************** Notification for patient about Reschedule Appointment ********************************************/

            return RedirectToAction(nameof(GetAvailabilitiesForDr), new { id = drId });
        }


        [HttpGet]
        public IActionResult EditAvailability(int id )
        {
            Availability drAvailability = AvailabilityRepository.Find(v=>v.Id==id);
            DrAvailabilityVM drAvailabilityVM = new DrAvailabilityVM(drAvailability);
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);

            IEnumerable<DateOnly> BookeddrAvailabilityDates = AvailabilityRepository
                .FindAllWithSelect(
                    v => v.Date >= today && v.AvailableSlots.Any(s => s.IsBooked),
                    v => v.Date
                );

           List<DateOnly> FreeAvailabilityDates = new List<DateOnly>();

            for (int i = 0; i < 7; i++)
            {
               DateOnly tempDate = today.AddDays(i);
                if (!BookeddrAvailabilityDates.Contains(tempDate))
                    FreeAvailabilityDates.Add(tempDate);
            }
            if(!FreeAvailabilityDates.Contains(drAvailability.Date))
                FreeAvailabilityDates.Add(drAvailability.Date);

            ViewBag.FreeAvailabilityDates = FreeAvailabilityDates.Order();
            ViewBag.DrClinics = ClinicRepository.FindAllWithSelect(c=>c.DoctorId==drAvailability.DoctorId
            ,c=>new Item<int,string> { Id = c.Id , Name = c.Name });
            return View(drAvailabilityVM);
        }


        //[HttpPost]
        //public IActionResult EditAvailability(int id, DrAvailabilityVM drAvailabilityVM)
        //{
        //    return View(drAvailabilityVM);
        //}


        //[HttpPost]
        //public IActionResult EditAvailability(int id, DrAvailabilityVM drAvailabilityVM)
        //{
        //    return View(drAvailabilityVM);
        //}
        //[HttpGet]
        //public IActionResult AddAvailability(string id)        
        //{
        //    //DateTime.Today.AddDays(i)
        //    return View(SetAddAvailabilityVM(id));
        //}

        //public IActionResult ValidateEndTime(DrAvailability DrAvailability)
        //{
        //   // if (EndTime <= StartTime)k
        //        return Json(true);
        //}

        [HttpGet]
        public IActionResult AddAvailability(string id)
        {
            //DateTime.Today.AddDays(i)
            return View(SetAddAvailabilityVM(id));
        }
        private AddAvailabilityVM SetAddAvailabilityVM(string id)
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            AddAvailabilityVM addAvailability = new AddAvailabilityVM();
            addAvailability.DrAvailability = new List<DrAvailabilityVM>();

            IEnumerable<Availability> availabilities = AvailabilityRepository.FindAll(a => a.DoctorId == id && a.Date >= today);
            Availability tempAvail;
            DateOnly tempDate;
            for (int i = 0; i < AvailabilityDays; i++)
            {
                tempDate = today.AddDays(i);
                tempAvail = availabilities.FirstOrDefault(a => a.Date == tempDate);
                if (tempAvail != null)
                    addAvailability.DrAvailability.Add(new DrAvailabilityVM(tempAvail));
                else
                    addAvailability.DrAvailability.Add(new DrAvailabilityVM() { dayOfWeek = tempDate.DayOfWeek, Date = tempDate });

            }

            addAvailability.id = id;
            addAvailability.DrClincs = ClinicRepository.FindAll
            (
                c => c.DoctorId == id)
                .Select(c => new Item<int,string> { Id = c.Id, Name = $"{c.Name} ({c.ClinicRegion})" }
            ).ToList();
            return addAvailability;

        }
        [HttpPost]
        public IActionResult AddAvailability(string id,List<DrAvailabilityVM> DrAvailability)
        {

            if (ModelState.IsValid)
            {

                foreach (DrAvailabilityVM drAvailability in DrAvailability)
                {
                    if (drAvailability.IsAvailable)
                    {
                        Availability availability = drAvailability.GetAvailability();
                        availability.DoctorId = id;
                        if (drAvailability.id == null)
                        {
                            List<AvailabilitySlots> availabilitySlots = new List<AvailabilitySlots>();
                            while (drAvailability.StartTime < drAvailability.EndTime)
                            {
                                availabilitySlots.Add(new AvailabilitySlots()
                                {

                                    StartTime = drAvailability.StartTime,
                                    EndTime = drAvailability.StartTime.AddMinutes(drAvailability.Duration)

                                });
                                drAvailability.StartTime = availabilitySlots[availabilitySlots.Count - 1].EndTime;
                            }
                            availability.AvailableSlots = availabilitySlots;
                            AvailabilityRepository.Add(availability);
                        }
                        else
                        {


                        }
                        AvailabilityRepository.SaveChanges();
                    }

                }
                return RedirectToAction(nameof(GetAvailabilitiesForDr), new {id = id});
            }

            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            AddAvailabilityVM addAvailability = new AddAvailabilityVM();
            addAvailability.DrAvailability = DrAvailability;

            IEnumerable<Availability> availabilities = AvailabilityRepository.FindAll(a => a.DoctorId == id && a.Date >= today);
            Availability tempAvail;
            DateOnly tempDate;
            for (int i = 0; i < 7; i++)
            {
                tempDate = today.AddDays(i);
                tempAvail = availabilities.FirstOrDefault(a => a.Date == tempDate);
                if (tempAvail != null)
                    addAvailability.DrAvailability[i]=new DrAvailabilityVM(tempAvail);
                else if (!DrAvailability[i].IsAvailable)
                    addAvailability.DrAvailability[i] = new DrAvailabilityVM() { dayOfWeek = tempDate.DayOfWeek, Date = tempDate };

            }

            addAvailability.id = id;
            addAvailability.DrClincs = ClinicRepository.FindAll
            (
                c => c.DoctorId == id)
                .Select(c => new Item<int, string> { Id = c.Id, Name = $"{c.Name} ({c.ClinicRegion})" }
            ).ToList();
            return View(addAvailability);

        }

    }
}
