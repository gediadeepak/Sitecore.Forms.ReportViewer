using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Sitecore.Forms.Report.Viewer.Attribute;

namespace Sitecore.Forms.Report.Viewer.Models
{
    public class FormReportRequestModel
    {
        [Required(ErrorMessage = "Form id not selected.")]
        public string FormId { get; set; }

        [Required(ErrorMessage = "Start date is not selected.")]
        [DateValidation(compareWith: "EndDate")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is not selected.")]
        [DateValidation(compareWith: "StartDate")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Language not selected.")]
        public string Language { get; set; }
    }
}