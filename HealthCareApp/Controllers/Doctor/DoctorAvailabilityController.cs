
using HealthCareApp.ViewModel.Doctor;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HealthCareApp.Controllers.Doctor
{
    public class DoctorAvailabilityController : Controller
    {
       public IGenericRepo<Notification> NotificationRepository {  get; }
        public IAvailabilityRepository AvailabilityRepository { get; }
        public IGenericRepo<Clinic> ClinicRepository { get; }
        public IGenericRepo<AvailabilitySlots> SlotRepository { get; }
        int AvailabilityDays = 7;
        private readonly IGenericRepo<HealthCare.DAL.Models.Doctor> doctorRepository;
        NotificationService notificationService;

        public DoctorAvailabilityController( IAvailabilityRepository availabilityRepository, IGenericRepo<Clinic> ClinicRepository, IGenericRepo<AvailabilitySlots> SlotRepository,IGenericRepo<Notification>NotificationRepository , IGenericRepo<HealthCare.DAL.Models.Doctor> doctorRepository, NotificationService notificationService) {
            this.AvailabilityRepository = availabilityRepository;
            this.ClinicRepository = ClinicRepository;
            this.SlotRepository = SlotRepository;
            this.NotificationRepository = NotificationRepository;
            this.doctorRepository = doctorRepository;
            this.notificationService = notificationService;
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
                        ClinicName = $"{v.Clinic.Region.City.CityNameEn} ({v.Clinic.Region.RegionNameEn})",
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
        public IActionResult GetAvailabilitiesForDr(string doctorId=null)
        {
            if(doctorId==null)
                doctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //??isDeleted in AVibility?
            //??diplay avaibiliy of today and future? not past ?or doctor need data for past
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);

            var drAvailabilities = AvailabilityRepository.FindAllWithSelect(v => v.DoctorId == doctorId && (v.Date == DateOnly.FromDateTime(DateTime.Today) || v.Date > today)
            , v => new GetAvailabilityForDrVM
            {
                AvailableSlotsCnt = v.AvailableSlots.Count(v=>!v.IsBooked),
                AppointmentCnt = v.AvailableSlots.Count(v=>v.IsBooked),
                ClinicName = $"{v.Clinic.Region.City.CityNameEn} ({v.Clinic.Region.RegionNameEn})",
                Date = v.Date,
                dayOfWeek = v.dayOfWeek,
                DoctorId = doctorId,
                Duration = v.Duration,
                StartTime = v.StartTime,
                EndTime = v.EndTime,
                TimeRange = $"{v.StartTime} - {v.EndTime}",  
                 Id = v.Id,
                type = v.type,     
            }).OrderBy(v=>v.Date);

            ViewBag.DoctorId = doctorId;
            return View(drAvailabilities);
        }
        /////////////////////////////////////////////////////////
        private GetAvailabilityForDrVM GetAvailabilityForDrVM(int availabilityId)
        {
            var drAvailabilities = AvailabilityRepository.FindWithSelect(v => v.Id == availabilityId, v => new GetAvailabilityForDrVM
            {
                AvailableSlotsCnt = v.AvailableSlots.Count(v => !v.IsBooked),
                AppointmentCnt = v.AvailableSlots.Count(v => v.IsBooked),
                ClinicName = $"{v.Clinic.Region.City.CityNameEn} ({v.Clinic.Region.RegionNameEn})",
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
            HealthCare.DAL.Models.Doctor doctor = new HealthCare.DAL.Models.Doctor();
            return drAvailabilities;
        }

        ////////////////////////////////////CancelDay for Notification/////////////////////////////////////////////
        public IActionResult CancelDay(int oldAvailabilityId)
        {
            Availability oldAvailability = AvailabilityRepository.GetAvailabilitySlotsAppointment(oldAvailabilityId);
            string drId = oldAvailability.DoctorId;

            foreach (var slot in oldAvailability.AvailableSlots)
            {
                if (slot.IsBooked)
                {
                    /////////////////////////////notification for Cancel Appointment/////////////////////////////////////////
                    var notificationDr = new Notification
                    {
                        UserId = slot.Appointment.PatientId,
                        Message = $"Dr {slot.Availability.Doctor.FirstName} {slot.Availability.Doctor.LastName} Canceled your Appointment that was in {slot.Availability.Date} from {slot.Availability.StartTime} to {slot.Availability.EndTime}.",
                        CreatedDate = DateTime.Now,
                        notificationType = NotificationType.ApproveAccount,
                    };
                    notificationService.Notify(notificationDr);
                  
                    slot.Appointment.Status = Status.Cancelled;
                    slot.Appointment.IsDeleted = true;
                    if (slot.Appointment.PaymentMethod == PaymentMethod.Visa)
                        slot.Appointment.PaymentStatus = PaymentStatus.Refunded;

                    //string msg =slot.Appointment.patientId.ToString();
                }
                slot.IsDeleted = true;
            }

            oldAvailability.IsDeleted=true;
            SlotRepository.SaveChanges();
            return RedirectToAction(nameof(GetAvailabilitiesForDr), new { doctorId = drId });
        }

        public IActionResult RescheduleAvailability(int availabilityId)
        {
            DateOnly dateToday = DateOnly.FromDateTime(DateTime.Now);
            IEnumerable<DateOnly> currentAvailabilities = AvailabilityRepository.FindAllWithSelect(
                v=>v.Date>=dateToday,
                v=>v.Date
            ).Order();

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


        //////////////////////////////////RescheduleAvailability For Notification///////////////////////////////////////////
        [HttpPost]
        public IActionResult RescheduleAvailability(Item<DateOnly,int> RescheduleAvailabilityDate, int previousAvailabilityId)
        {
            Availability previousAvailability = AvailabilityRepository.GetAvailabilitySlotsAppointment(previousAvailabilityId);
            
          //  List<Notification>notifications = new List<Notification>();
            Availability newAvailability = new Availability()
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
            DateTime todayTime = DateTime.Now;

            foreach (var slot in previousAvailability.AvailableSlots) {
                newAvailability.AvailableSlots.Add(slot);
                if (slot.IsBooked) {
                    /////////////////////////////notification for Reschdule Appointment/////////////////////////////////////////
                    var notificationDr = new Notification
                    {
                        UserId = slot.Appointment.PatientId,
                        Message = $"Dr {slot.Availability.Doctor.FirstName} {slot.Availability.Doctor.LastName} Reschduled your Appointment from day {previousAvailability.Date} to Day {newAvailability.Date} from time: {slot.Availability.StartTime} to {slot.Availability.EndTime}.",
                        CreatedDate = DateTime.Now,
                        notificationType = NotificationType.ApproveAccount,
                    };
                    notificationService.Notify(notificationDr);

                    slot.Appointment.Status = Status.Rescheduled;
                    
                }
            }
            AvailabilityRepository.Add(newAvailability);
            previousAvailability.AvailableSlots.Clear();
            previousAvailability.IsDeleted = true;
            AvailabilityRepository.SaveChanges();

            //NotificationRepository.(notifications)
            //NotificationRepository.AddRange(notifications);

            return RedirectToAction(nameof(GetAvailabilitiesForDr), new { doctorId = newAvailability.DoctorId });

        }
        private IEnumerable<DateOnly> GetScheduleDays(IEnumerable<DateOnly> currentAvailabilities)
        {
            List<DateOnly> ScheduleDays = new List<DateOnly>();
            DateOnly dateToday = DateOnly.FromDateTime(DateTime.Now);

            for (int i = 0; i < AvailabilityDays; i++)
            {
                DateOnly currentDay = dateToday.AddDays(i);
                if (!currentAvailabilities.Contains(currentDay))
                    ScheduleDays.Add(currentDay);
            }
            return ScheduleDays;
        }
        [HttpGet]
        public IActionResult ViewSlots(string drId, int availabilityId)
        {
            List<ViewSlotVM> Slots = SlotRepository.FindAllWithSelect(
                s => s.AvailabilityId == availabilityId,
                s => new ViewSlotVM()
                {
                    TimeRange = $"{s.StartTime} - {s.EndTime}",
                    PatientName = (s.Appointment == null) ? "-" : $"{s.Appointment.Patient.FirstName} {s.Appointment.Patient.LastName}",
                    PatientNumber = (s.Appointment == null) ? "-" : s.Appointment.Patient.PhoneNumber,
                    IsBooked = s.IsBooked,
                    Status = (s.IsBooked) ? "Booked" : "Available",
                    AppointmentId = (s.Appointment == null) ? null : s.Appointment.Id,
                    SlotId = s.Id,
                    AvailabilityId = s.AvailabilityId,
                     startTime = s.StartTime,
                    
                }
            ).OrderBy(s=>s.startTime).ToList();
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
            if(avail==null)
                return Json(null);
            if (avail.Date==dateToday)
            {
                Slots = SlotRepository.FindAll
                (s => s.AvailabilityId == AvailableId && s.Id != OldSlotId && s.StartTime > timeNow && !s.IsBooked)
                .OrderBy(s => s.StartTime).Select(s => new Item<int, string>()
                 {
                      Id = s.Id,
                      Name = $"{s.StartTime} - {s.EndTime}"
                 }).ToList();

            }
            else
            {
                Slots = SlotRepository.FindAll(
                  s => s.AvailabilityId == AvailableId && s.Id != OldSlotId && !s.IsBooked).OrderBy(s=>s.StartTime)
                        .Select(s => new Item<int, string>
                        {
                            Name = $"{s.StartTime} - {s.EndTime}",
                            Id = s.Id,
                        }).ToList();
            }

            return Json(Slots);
        }

        //////////////////////////// Notification for patient about the cancelation ///////////////////
        public IActionResult CancelSlotPost(int slotId)
        {
            AvailabilitySlots cancelSlot=AvailabilityRepository.GetSlot(slotId);
            Notification notificationDr= null;
            /***************************************** Notification for patient about the cancelation ********************************************/
            if (cancelSlot.IsBooked)
            {
                notificationDr = new Notification
                {
                    UserId = cancelSlot.Appointment.PatientId,
                    Message = $"Dr {cancelSlot.Availability.Doctor.FirstName} {cancelSlot.Availability.Doctor.LastName} Canceled your Appointment that was in {cancelSlot.Availability.Date} from {cancelSlot.Availability.StartTime} to {cancelSlot.Availability.EndTime}.",
                    CreatedDate = DateTime.Now,
                    notificationType = NotificationType.ApproveAccount,
                };
                notificationService.Notify(notificationDr);

            }
            /***************************************** how dealing with digital payment ********************************************/
            if (cancelSlot != null)
            {
                if (cancelSlot.IsBooked) {
                    cancelSlot.Appointment.Status = Status.Cancelled;
                    cancelSlot.Appointment.IsDeleted= true;
                    if (cancelSlot.Appointment.PaymentMethod == PaymentMethod.Visa)
                        cancelSlot.Appointment.PaymentStatus = PaymentStatus.Refunded;
                   
                }
                int cnt = SlotRepository.FindAll(s=>s.AvailabilityId==cancelSlot.AvailabilityId).Count();
                if(cnt==1)
                    cancelSlot.Availability.IsDeleted = true;

                cancelSlot.IsDeleted=true;
                SlotRepository.SaveChanges();
               // SlotRepository.HardDelete(cancelSlot); //delete slot with appoiment 

            }

            return RedirectToAction(nameof(ViewSlots), new { drId = cancelSlot.Availability.DoctorId ,availabilityId = cancelSlot.AvailabilityId });
        }
        public IActionResult RescheduleAppointment(int slotId)
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            AvailabilitySlots oldSlot = SlotRepository.Find(s => s.Id == slotId,s=>s.Availability);
            List<Item<int,DateOnly>> AvailabeDays = AvailabilityRepository.FindAllWithSelect(
                v => v.Date >= today && v.AvailableSlots.Any(s => !s.IsBooked) && v.DoctorId==oldSlot.Availability.DoctorId,
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
                OldSlotId = slotId, AvailableId = oldSlot.AvailabilityId
            };
            return View(slot_AvailbilityVM);
        }

        ////////////////////////Notification for patient about Reschedule Appointment ////////////////

        [HttpPost]
        public IActionResult RescheduleAppointment(Slot_AvailbilityVM slot_Availbility )
        {
            var OldSlot = AvailabilityRepository.GetSlot( slot_Availbility.OldSlotId);
            var NewSlot = AvailabilityRepository.GetSlot(slot_Availbility.NewSlotId);

           //////////////////// //dr notification
            var notificationDr = new Notification
            {
                UserId = OldSlot.Appointment.PatientId,
                Message = $"Dr {OldSlot.Availability.Doctor.FirstName} {OldSlot.Availability.Doctor.LastName} Reschduled your Appointment from day {OldSlot.Availability.Date} to Day {NewSlot.Availability.Date} from time: {NewSlot.Availability.StartTime} to {NewSlot.Availability.EndTime}.",
                CreatedDate = DateTime.Now,
                notificationType = NotificationType.ApproveAccount,
            };
            notificationService.Notify(notificationDr);

            NewSlot.IsBooked = true;
            OldSlot.Appointment.AvailableSlot = NewSlot;
            OldSlot.Appointment.Status = Status.Rescheduled;

            OldSlot.IsDeleted = true;

            int oldAvailbility = OldSlot.AvailabilityId;
            int CntSlots = SlotRepository.Count(s => s.AvailabilityId == oldAvailbility && s.Id!=OldSlot.Id);
            if (CntSlots == 0)
                OldSlot.Availability.IsDeleted=true;

            AvailabilityRepository.SaveChanges();
            /***************************************** Notification for patient about Reschedule Appointment ********************************************/

            return RedirectToAction(nameof(GetAvailabilitiesForDr), new { doctorId = OldSlot.Availability.DoctorId });
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
        public IActionResult AddAvailability(string id=null)
        {
            //DateTime.Today.AddDays(i)
            if (id == null)
                  id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

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
            addAvailability.DrClincs = ClinicRepository.FindAllWithSelect
            (
                c => c.DoctorId == id,
                c => new Item<int,string> { Id = c.Id, Name =$"{c.Region.City.CityNameEn} ({c.Region.RegionNameEn})" }
            ).ToList();
            return addAvailability;

        }
        [HttpPost]
        public IActionResult AddAvailability(List<DrAvailabilityVM> DrAvailability, string id = null)
        {
            if (id == null)
                id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

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
                return RedirectToAction(nameof(GetAvailabilitiesForDr), new { doctorId = id});
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
            addAvailability.DrClincs = ClinicRepository.FindAllWithSelect
            (
                c => c.DoctorId == id,
                c => new Item<int, string> { Id = c.Id, Name = $"{c.Region.City.CityNameEn} ({c.Region.RegionNameEn})" }
            ).ToList();
            return View(addAvailability);

        }

    }
}
