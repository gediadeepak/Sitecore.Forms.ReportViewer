using Newtonsoft.Json;
using Sitecore.Forms.Report.Viewer.Models;
using Sitecore.Forms.Report.Viewer.Repository;
using Sitecore.Mvc.Controllers;
using Sitecore.Shell.Framework.Commands;
using Sitecore.XA.Foundation.Mvc.Controllers;
using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace Sitecore.Forms.Report.Viewer.Controllers
{
    public class FormReportController : SitecoreController
    {
        private IFormReportRepository repository;

        public FormReportController(IFormReportRepository _repository)
        {
            repository = _repository;
        }

        [HttpPost]
        public JsonResult GetFormReportData(FormReportRequestModel request)
        {
            FormReportResponseModel response = new FormReportResponseModel();
            if (!ModelState.IsValid)
            {
                response.Data = null;
                response.IsSuccess = false;
                response.ResponseMessage = "";
                foreach (var modelStateKey in ViewData.ModelState.Keys)
                {
                    var value = ViewData.ModelState[modelStateKey];

                    foreach (var error in value.Errors)
                    {
                        if (response.ResponseMessage.Length > 0)
                        {
                            response.ResponseMessage += ", ";
                        }
                        response.ResponseMessage += error.ErrorMessage;
                    }
                }
                return Json(response, JsonRequestBehavior.AllowGet);
            }
            string formJsonData = repository.GetFormReportJsonData(request);


            if (!string.IsNullOrEmpty(formJsonData))
            {
                response.IsSuccess = true;
                response.Data = formJsonData;
                return Json(response, JsonRequestBehavior.AllowGet);
            }

            response.Data = null;
            response.IsSuccess = false;
            response.ResponseMessage = "No Any Records available for given form Id or selected date range.";

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetAllForms()
        {
            try
            {
                return Json(repository.GetAllForms(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                FormReportResponseModel response = new FormReportResponseModel
                {
                    Data = null,
                    IsSuccess = false,
                    ResponseMessage = ex.Message
                };
                return Json(response, JsonRequestBehavior.AllowGet);

            }
        }

        [HttpPost]
        public JsonResult GetFormLanguageById(FormReportRequestModel request)
        {

            try
            {
                return Json(repository.GetFormsLanguage(request.FormId), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                FormReportResponseModel response = new FormReportResponseModel
                {
                    Data = null,
                    IsSuccess = false,
                    ResponseMessage = ex.Message
                };
                return Json(response, JsonRequestBehavior.AllowGet);

            }
        }
    }
}