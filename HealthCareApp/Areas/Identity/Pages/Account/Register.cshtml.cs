// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using HealthCareApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthCareApp.Data;

namespace HealthCareApp.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;

        private readonly RoleManager<IdentityRole> _roleManager;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,

            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;

            _roleManager = roleManager;
            _context = context;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

          
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Required]
            [Display(Name = "Date of Birth")]
            [DataType(DataType.Date)]
            public DateOnly DateOfBirth { get; set; }

            [Required]
            [Display(Name = "Gender")]
            public Gender Gender { get; set; }

            // Selection for user type
            [Required]
            [Display(Name = "Register as")]
            public string UserType { get; set; } // This will be "Patient" or "Doctor"

            // Patient-specific fields
            [RegularExpression(@"^\d{11}$", ErrorMessage = "Mobile Number must be exactly 11 digits.")]
            [Display(Name = "Phone Number")]
            public string PhoneNumber { get; set; }


            [RegularExpression(@"^\d{11}$", ErrorMessage = "Mobile Number must be exactly 11 digits.")]
            [Display(Name = "Emergency Contact")]
            public string EmergencyContact { get; set; }

            [Display(Name = "Medical History")]
            [DataType(DataType.MultilineText)]
            public string MedicalHistory { get; set; }

            // Doctor-specific fields
            [Display(Name = "Professional Title")]
            public Title? Title { get; set; }

            [Display(Name = "Description")]
            [DataType(DataType.MultilineText)]
            public string Description { get; set; }

            [Display(Name = "Years of Experience")]
            public int? ExperienceYears { get; set; }

            [Display(Name = "Specialization")]
            public int? SpecializationId { get; set; }
        }


        public SelectList RoleList { get; set; }
        public List<SelectListItem> Specializations { get; set; }
        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            var roles = new List<string> { "Patient", "Doctor" };
            RoleList = new SelectList(roles);

            Specializations = await _context.Specializations
           .Select(s => new SelectListItem
           {
               Value = s.Id.ToString(),
               Text = s.Name
           })
           .ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                //var user = CreateUser();

                //await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                //await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                //var result = await _userManager.CreateAsync(user, Input.Password);

                if (Input.UserType == "Patient")
                {
                    var user = new Patient
                    {
                        UserName = Input.Email,
                        Email = Input.Email,
                        FirstName = Input.FirstName,
                        LastName = Input.LastName,
                        DateOfBirth = Input.DateOfBirth,
                        gender = Input.Gender,
                        CreatedAt = DateTime.Now,
                        PhoneNumber = Input.PhoneNumber,
                        EmergencyContact = Input.EmergencyContact,
                        MedicalHistory = Input.MedicalHistory
                    };

                    var result = await _userManager.CreateAsync(user, Input.Password);

                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "Patient");

                        _logger.LogInformation("User created a new account with password.");

                        var userId = await _userManager.GetUserIdAsync(user);
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                            protocol: Request.Scheme);

                        await _emailSender.SendEmailAsync(
                             Input.Email,
                             "Confirm your HealthCareApp account",
                             $@"
                            <html>
                            <body style='font-family: Arial, sans-serif; color: #333; line-height: 1.6;'>
                                <div style='max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 5px;'>
                                    <div style='text-align: center; margin-bottom: 20px;'>
                                        <h2 style='color: #4285f4;'>Welcome to HealthCareApp!</h2>
                                    </div>
                                    <p>Thank you for registering with HealthCareApp. We're excited to have you join us!</p>
                                    <p>Please confirm your email address by clicking the button below:</p>
                                    <div style='text-align: center; margin: 25px 0;'>
                                        <a href='{HtmlEncoder.Default.Encode(callbackUrl)}' 
                                           style='background-color: #4285f4; color: white; padding: 10px 20px; 
                                                  text-decoration: none; border-radius: 4px; font-weight: bold;'>
                                            Confirm Email
                                        </a>
                                    </div>
                                    <p>If you didn't create an account with HealthCareApp, you can safely ignore this email.</p>
                                    <hr style='border: 0; border-top: 1px solid #ddd; margin: 20px 0;'>
                                    <p style='font-size: 12px; color: #777; text-align: center;'>
                                        &copy; ${DateTime.Now.Year} HealthCareApp. All rights reserved.
                                    </p>
                                </div>
                            </body>
                            </html>
                            ");
                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                        }
                        else
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            return LocalRedirect(returnUrl);
                        }
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    
                }
                else if (Input.UserType == "Doctor")
                {
                    var user = new Doctor
                    {
                        UserName = Input.Email,
                        Email = Input.Email,
                        FirstName = Input.FirstName,
                        LastName = Input.LastName,
                        DateOfBirth = Input.DateOfBirth,
                        gender = Input.Gender,
                        CreatedAt = DateTime.Now,
                        Title = Input.Title,
                        Description = Input.Description,
                        ExperienceYears = Input.ExperienceYears ?? 0,
                        SpecializationId = Input.SpecializationId ?? 0, 
                        verificationStatus = VerificationStatus.Pinding,
                        ProfilePicture= Input.Gender == Gender.Male ? "DefaultMale.jpg" :"DefaultFemale.png",
                    };

                    var result = await _userManager.CreateAsync(user, Input.Password);
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "Doctor");
                        _logger.LogInformation("User created a new account with password.");

                        var userId = await _userManager.GetUserIdAsync(user);

                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                            protocol: Request.Scheme);

                          await _emailSender.SendEmailAsync(
                          Input.Email,
                          "Confirm your HealthCareApp account",
                          $@"
                        <html>
                        <body style='font-family: Arial, sans-serif; color: #333; line-height: 1.6;'>
                            <div style='max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 5px;'>
                                <div style='text-align: center; margin-bottom: 20px;'>
                                    <h2 style='color: #4285f4;'>Welcome to HealthCareApp!</h2>
                                </div>
                                <p>Thank you for registering with HealthCareApp. We're excited to have you join us!</p>
                                <p>Please confirm your email address by clicking the button below:</p>
                                <div style='text-align: center; margin: 25px 0;'>
                                    <a href='{HtmlEncoder.Default.Encode(callbackUrl)}' 
                                       style='background-color: #4285f4; color: white; padding: 10px 20px; 
                                              text-decoration: none; border-radius: 4px; font-weight: bold;'>
                                        Confirm Email
                                    </a>
                                </div>
                                <p>If you didn't create an account with HealthCareApp, you can safely ignore this email.</p>
                                <hr style='border: 0; border-top: 1px solid #ddd; margin: 20px 0;'>
                                <p style='font-size: 12px; color: #777; text-align: center;'>
                                    &copy; ${DateTime.Now.Year} HealthCareApp. All rights reserved.
                                </p>
                            </div>
                        </body>
                        </html>
                        ");

                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                        }
                        else
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            return LocalRedirect(returnUrl);
                        }
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    
                }

            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}
