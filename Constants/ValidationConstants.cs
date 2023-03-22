using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Configuration;

namespace Sitecore.Forms.Report.Viewer.Constants
{
    public class ValidationConstants
    {
        public static string StartDateGreaterThanEndDateMsg => Settings.GetSetting("StartDateGreaterThanEndDateMsg");
        public static string EndDateLessThnaStartDate => Settings.GetSetting("EndDateLessThnaStartDate");
        public static string EndateOrStartDateGreaterThanCurrentDate => Settings.GetSetting("EndateOrStartDateGreaterThanCurrentDate");
        public static string DateDiffMessage => Settings.GetSetting("DateDiffMessage");
        public static int AllowedNumberOfDays => System.Convert.ToInt32(Settings.GetSetting("AllowedNumberOfDays") ?? "365");
    }
}