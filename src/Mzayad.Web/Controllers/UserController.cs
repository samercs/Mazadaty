﻿using System;
using System.Threading.Tasks;
using Mzayad.Models;
using Mzayad.Services;
using Mzayad.Web.Core.Configuration;
using Mzayad.Web.Core.Services;
using System.Web.Mvc;
using Mzayad.Models.Enum;
using Mzayad.Web.Extensions;
using Mzayad.Web.Models.Account;
using Mzayad.Web.Models.Shared;
using Mzayad.Web.Models.User;
using Mzayad.Web.Resources;
using OrangeJetpack.Base.Web;
using OrangeJetpack.Localization;
using OrangeJetpack.Services.Models;

namespace Mzayad.Web.Controllers
{
    [RoutePrefix("{language}/user")]
    public class UserController : ApplicationController
    {
        private readonly AddressService _addressService;
        public UserController(IControllerServices controllerServices) : base(controllerServices)
        {
            _addressService=new AddressService(DataContextFactory);
        }

        [Route("my-account")]
        public ActionResult MyAccount()
        {
            return View();
        }

        [Route("change-password")]
        public ActionResult ChangePassword()
        {
            var viewModel = new ChangePasswordViewModel();

            return View(viewModel);
        }

        [Route("change-password")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await AuthService.ChangePassword(User.Identity, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                SetStatusMessage(Global.PasswordChangeFailureMessage, StatusMessageType.Error);

                return View(model);
            }

            await SendPasswordChangedEmail();

            SetStatusMessage(Global.PasswordSuccessfullyChanged);

            return RedirectToAction("MyAccount");
        }

        private async Task SendPasswordChangedEmail()
        {
            var user = await AuthService.CurrentUser();
            var emailTeamplet = await _EmailTemplateService.GetByTemplateType(EmailTemplateType.PasswordChanged);
            var email = new Email
            {
                ToAddress = user.Email,
                Subject = emailTeamplet.Localize(Language, i => i.Subject).Subject,
                Message = string.Format(emailTeamplet.Localize(Language, i => i.Message).Message, user.FirstName)
            };

            await MessageService.SendMessage(email.WithTemplate(this));
        }

        [Route("editaccount")]
        public async Task<ActionResult> EditAccount()
        {
            var user = await AuthService.CurrentUser();
            Address address=null;
            if (user.AddressId.HasValue)
            {
                address = await _addressService.GetAddress(user.AddressId.Value);    
            }
            
            var model = new RegisterViewModel()
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = new AddressViewModel(address).Hydrate(),
                PhoneCountryCode = user.PhoneCountryCode,
                PhoneNumber = user.PhoneNumber,
                UserName = user.UserName,
                IsEdit = true
            };
            return View(model);
        }

        [Route("editaccount")]
        [HttpPost,ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAccount(RegisterViewModel model)
        {
            
            var user = await AuthService.CurrentUser();
            var userNameChanged = user.Email != model.Email;
            if (!TryUpdateModel(user))
            {
                RedirectToAction("EditAccount");
            }
            
            
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.PhoneCountryCode = model.PhoneCountryCode;
            user.PhoneNumber = model.PhoneNumber;
            await AuthService.UpdateUser(user);

            if (user.AddressId.HasValue)
            {
                var address = await _addressService.GetAddress(user.AddressId.Value);
                address.AddressLine1 = model.Address.AddressLine1;
                address.AddressLine2 = model.Address.AddressLine2;
                address.AddressLine3 = model.Address.AddressLine3;
                address.AddressLine4 = model.Address.AddressLine4;
                address.CityArea = model.Address.CityArea;
                address.CountryCode = model.Address.CountryCode;
                address.PostalCode = model.Address.PostalCode;
                address.StateProvince = model.Address.StateProvince;
                await _addressService.Update(address);
            }
            

            CookieService.Add(CookieKeys.DisplayName, user.FirstName, DateTime.MaxValue);
            CookieService.Add(CookieKeys.LastSignInEmail, user.Email, DateTime.MaxValue);


            if (userNameChanged)
            {
                await SendEmailChangedEmail();

                AuthService.SignOut();

                SetStatusMessage(Global.EditAccountNameAndEmailSuccessMessage);

                return RedirectToAction("SignIn", "Account");
            }


            SetStatusMessage(Global.EditAccountNameSuccessMessage + " " +userNameChanged);

            return RedirectToAction("MyAccount");
        }


        private async Task SendEmailChangedEmail()
        {
            var user = await AuthService.CurrentUser();
            var emailTeamplet = await _EmailTemplateService.GetByTemplateType(EmailTemplateType.EmailChanged);
            var email = new Email
            {
                ToAddress = user.Email,
                Subject = emailTeamplet.Localize(Language, i => i.Subject).Subject,
                Message = string.Format(emailTeamplet.Localize(Language, i => i.Message).Message, user.FirstName,AppSettings.SiteName)
            };

            await MessageService.SendMessage(email.WithTemplate(this));
        }


    }
}