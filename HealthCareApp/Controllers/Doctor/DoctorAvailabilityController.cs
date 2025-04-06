using HealthCareApp.Models;
using HealthCareApp.RepositoryServices;
using HealthCareApp.ViewModel.Doctor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace HealthCareApp.Controllers.Doctor
{
    public class DoctorAvailabilityViewModel
    {
        public List<DrAvailability> DrAvailability { get; set; }
    }

    public class DrAvailability
    {
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }
    public class DoctorAvailabilityController : Controller
    {
        public IGenericRepoServices<Availability> AvailabilityRepository { get; }
        public IGenericRepoServices<Clinic> ClinicRepository { get; }
        public DoctorAvailabilityController( IGenericRepoServices<Availability> availabilityRepository, IGenericRepoServices<Clinic> ClinicRepository) {
            this.AvailabilityRepository = availabilityRepository;
            this.ClinicRepository = ClinicRepository;
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
         
        [HttpGet]
        public IActionResult ViewSlots(int id)
        {
            return View();
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
            ,c=>new Item<int> { Id = c.Id , Name = c.Name });
            return View(drAvailabilityVM);
        }


        [HttpPost]
        public IActionResult EditAvailability(int id, DrAvailabilityVM drAvailabilityVM)
        {
            return View(drAvailabilityVM);
        }
        [HttpGet]
        public IActionResult AddAvailability(string id)
        {
            //DateTime.Today.AddDays(i)
            return View(SetAddAvailabilityVM(id));
        }

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
        //   // if (EndTime <= StartTime)
        //        return Json(true);
        //}
        private AddAvailabilityVM SetAddAvailabilityVM(string id)
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            AddAvailabilityVM addAvailability = new AddAvailabilityVM();
            addAvailability.DrAvailability = new List<DrAvailabilityVM>();

            IEnumerable<Availability> availabilities = AvailabilityRepository.FindAll(a => a.DoctorId == id && a.Date >= today);
            Availability tempAvail;
            DateOnly tempDate;
            for (int i = 0; i < 7; i++)
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
                .Select(c => new Item<int> { Id = c.Id, Name = $"{c.Name} ({c.ClinicRegion})" }
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
                .Select(c => new Item<int> { Id = c.Id, Name = $"{c.Name} ({c.ClinicRegion})" }
            ).ToList();
            return View(addAvailability);

        }

    }
}
