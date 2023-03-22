using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Sitecore.Forms.Report.Viewer.Constants;

namespace Sitecore.Forms.Report.Viewer.Attribute
{
    public class DateValidation : ValidationAttribute
    {

        private string _propertyNameToCompare;

        public DateValidation(string compareWith = "")
        {   
            _propertyNameToCompare = compareWith;
        }
        
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime endDate= DateTime.MinValue;
            DateTime startDate = DateTime.MinValue;
            DateTime currentDate = DateTime.Now;
            string errorMessage = string.Empty;
            if (_propertyNameToCompare.ToLower().Equals("startdate"))
            {
                endDate = DateTime.Parse(value.ToString());
                startDate = DateTime.Parse(GetPropertyValue(_propertyNameToCompare, validationContext));
                if (endDate > currentDate)
                {
                    var errorMsg = FormatErrorMessage(string.Format(ValidationConstants.EndateOrStartDateGreaterThanCurrentDate, "End Date"));
                    return new ValidationResult(errorMsg);
                }
                if (endDate < startDate)
                {
                    //create error
                    var errorMsg = FormatErrorMessage(string.Format(ValidationConstants.EndDateLessThnaStartDate, "End Date"));
                    return new ValidationResult(errorMsg);
                }
            }

            if (_propertyNameToCompare.ToLower().Equals("enddate"))
            {
                startDate = DateTime.Parse(value.ToString());
                endDate = DateTime.Parse(GetPropertyValue(_propertyNameToCompare, validationContext));
                if (startDate > currentDate)
                {
                    var errorMsg = FormatErrorMessage(string.Format(ValidationConstants.EndateOrStartDateGreaterThanCurrentDate, "Start Date"));
                    return new ValidationResult(errorMsg);
                }
                if (startDate > endDate)
                {
                    //create error
                    var errorMsg = FormatErrorMessage(string.Format(ValidationConstants.StartDateGreaterThanEndDateMsg, "Start Date"));
                    return new ValidationResult(errorMsg);
                }
            }

            var dateDiff = (endDate - startDate).TotalDays;

            if (dateDiff > ValidationConstants.AllowedNumberOfDays)
            {
                //create error
                var errorMsg = FormatErrorMessage(string.Format(ValidationConstants.DateDiffMessage, ValidationConstants.AllowedNumberOfDays));
                return new ValidationResult(errorMsg);
            }

            return ValidationResult.Success;

        }
        private string GetPropertyValue(string _propertyNameToCompare, ValidationContext validationContext)
        {
            string propValue = string.Empty;
            var model = validationContext.ObjectInstance;
            var displayName = validationContext.DisplayName;
            var propertyName = model.GetType().GetProperties()
                .FirstOrDefault(p => p.Name == _propertyNameToCompare);
            if (propertyName != null)
            {
                propValue = propertyName.GetValue(model, null).ToString();

            }

            return propValue;
        }

    }

}