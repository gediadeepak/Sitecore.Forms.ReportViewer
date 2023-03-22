(function (speak) {

    var showMessageBar = function (context, type, text, isTemporary, isClosable, isVisible = true) {
        context.errorMessageBar.IsVisible = true;
        if (!isVisible) {
            context.errorMessageBar.IsVisible = false;
        }
        context.errorMessageBar.reset([{ Type: type, Text: text, IsTemporary: isTemporary, IsClosable: isClosable }]);
    }
    const notificationType = {
        Notify: "Notification", Error: "Error"
    }
    var isMoreThanDays = function (dt1, dt2, days = 0) {
        const date1 = new Date(dt1.FormattedDate);
        const date2 = new Date(dt2.FormattedDate);
        const diffTime = Math.abs(date2 - date1);
        const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
        if (diffDays > days) {
            return false;
        }
        return true;
    }
    speak.pageCode(["knockout",
        "jquery.dataTables.min",
        "dataTables.buttons.min",
        "jszip.min",
        "buttons.html5.min",
        "buttons.print.min"
    ], function (ko) {
        var reserved = ["initialize", "initialized", "get", "set", "render", "serialize"];
        var context = {};
        window.onerror = function (errorMsg, url, lineNumber) {
            if (!window.jsErrors) { window.jsErrors = new Array(); }
            window.jsErrors.push(errorMsg + "\n" + url + ":" + lineNumber);
        };
        var bindFormsName = function (context) {
            var formsApiUrl = "/api/v1/forms/";
            $.ajax({
                type: "POST",
                processData: false,
                contentType: 'application/json',
                url: formsApiUrl,
                success: function (data) {

                    if (data && data.Data && data.IsSuccess) {
                        var formsNameData = JSON.parse(data.Data);
                        context.Form1.SelectForm.DynamicData = formsNameData;
                    }
                }
            });
        }
        var bindLanguage = function (selectedFormId) {
            var formsApiUrl = "/api/v1/forms/langByFormId";
            $.ajax({
                type: "POST",
                processData: false,
                contentType: 'application/json',
                data: '{"FormId":"' + selectedFormId + '","StartDate":"","EndDate":""}',
                url: formsApiUrl,
                success: function (data) {
                    if (data && data.Data && data.IsSuccess) {
                        var formsNameData = JSON.parse(data.Data);
                        context.Form1.SelectLanguage.DynamicData = formsNameData;
                    }
                }
            });
        }
        var buttonState = function (display) {
            var button = $('[data-sc-id="btnReport"]');
            if (button) {
                if (display == true) {
                    $(button).removeAttr("disabled");
                } else {
                    $(button).attr("disabled", "true");
                    
                }
            }
        }
        var spinnerState = function (display) {
            var spinner = $("#ribbonPreLoadingIndicator");
            if (spinner) {
                if (display == true) {
                    $(spinner).attr("style","display:block");
                    
                } else {
                    $(spinner).attr("style", "display:none");
                }
            }
        }
            var setState = function (isButtonEnabled, isSpinnerVisible) {
                window.setTimeout(function () {
                    buttonState(isButtonEnabled);
                    spinnerState(isSpinnerVisible)
                },500);
                
        }
        var validateFields = function (context) {
            var errorJson = "";
            if (context.Form1.SelectForm.SelectedItem === '' || context.Form1.SelectForm.SelectedItem.Id == "0") {
                errorJson = errorJson + 'Form name';
            }
            if (context.Form1.StartDate.getDate() == null) {
                if (errorJson.length > 0) {
                    errorJson = errorJson + ", "
                }
                errorJson = errorJson + 'start date';
            }
            if (context.Form1.EndDate.getDate() == null) {
                if (errorJson.length > 0) {
                    errorJson = errorJson + ", "
                }
                errorJson = errorJson + 'end date';
            }

            return errorJson;
        }
        return {
            initialized: function () {
                context = this;
                context.btnReport.on("click", this.btnReportClick, this);
                bindFormsName(context);
                setState(true, false);
            },
            formSelectionChangedEvent: function () {
                if (context.Form1.SelectForm.SelectedItem) {
                    bindLanguage(context.Form1.SelectForm.SelectedItem.Id);
                }
            },
            btnReportClick: function () {
                var reportData = context.Form1.getFormData();
                setState(false, true);
                errorJson = validateFields(context);
                if (errorJson.length > 0) {
                    setState(true, false);
                    showMessage(context, notificationType.Error, "There is some Error before submitting the details. " + errorJson + " details are not supplied to generate the report.", true, true, true);
                    return;
                } else {
                    if (!isMoreThanDays(context.Form1.StartDate, context.Form1.EndDate, 365)) {
                        errorJson = 'You can extract report only for 365 days.';
                        showMessage(context, notificationType.Error, errorJson, true, true, true);
                        setState(true, false);
                        return;
                    }
                    setState(true, false);
                }
                var selectedForm = context.Form1.SelectForm.SelectedItem.Id;
                var selectedStartDate = this.generateDate(context.Form1.StartDate);
                var selectedEndDate = this.generateDate(context.Form1.EndDate);
                var apiData = '{"Language":"en","FormId":"' + selectedForm + '","StartDate":"' + selectedStartDate + '","EndDate":"' + selectedEndDate + '"}';
                var formsReportApi = "/api/v1/forms/report"
                $.ajax({
                    type: "POST",
                    processData: false,
                    contentType: 'application/json',
                    url: formsReportApi,
                    data: apiData,
                    success: function (data) {
                        if (data && data.Data && data.IsSuccess) {
                            var responseJson = JSON.parse(data.Data);
                            var reportData = responseJson.ReportData;
                            var reportColumn = responseJson.ReportColumn;
                            var tblPlaceholderbyId = $("#tblPlaceholder");
                            tblPlaceholderbyId.html('');
                            var tblPlaceholder = '<table id="rptGrid" width="100%"><thead><tr></tr></thead></table>';
                            tblPlaceholderbyId.html(tblPlaceholder);
                            var oTblReport = $('#rptGrid');
                            var dttblColumn = '';
                            dttblColumn = reportColumn.replace("'", '"');
                            var tblHdRow = $('#rptGrid >thead tr');
                            $(tblHdRow).append(responseJson.TableHead);
                            showMessage(context, "Notification", "Total " + reportData.length + " records found.", true, true, true);
                            //tblbody.html('');
                            var reportTbl = oTblReport.DataTable({
                                "data": reportData,
                                "columns": JSON.parse("[" + dttblColumn + "]"),
                                dom: 'Bfrtip',
                                "dom": 'Bfrtip',
                                "buttons": [
                                    { extend: 'copy', text: 'Copy Report' },
                                    { extend: 'csv', text: 'Export to CSV' },
                                    { extend: 'excel', text: 'Export to Excel' },
                                ]
                            });
                            var column = reportTbl.column(0);
                            column.visible(false);
                            setState(true, false);
                            var rows = '<table class="table">' +
                                '<thead class="thead-dark">' +
                                '<tr>' +
                                '<th scope="col">Field Name</th>' +
                                '<th scope="col">Field Value</th>' +
                                '</tr>' +
                                '</thead>' +
                                '<tbody>$tbody' +
                                '</tbody>' +
                                '</table>';
                            $('#rptGrid tbody').on('click', 'tr', function () {
                                $(context.Section2.el).html('');
                                $('#rptGrid tbody tr').removeClass('selected');
                                $(this).addClass('selected');
                                var selectedData = reportTbl.rows('.selected').data()
                                if (selectedData.length > 0) {
                                    var selectedRow = selectedData[0]
                                    var colValue = "<tr><td>$colname</td><td>$colvalue</td></tr>";
                                    var dynamicCol = "";
                                    Object.getOwnPropertyNames(selectedRow).forEach((val, idx, array) => {

                                        var temprow = colValue.replace("$colname", val).replace("$colvalue", selectedRow[val]);
                                        dynamicCol += temprow;
                                    });

                                    $(context.Section2.el).html(rows.replace('$tbody', dynamicCol));
                                    context.formItem.show();
                                }


                            });
                        } else {
                            showMessage(context, "Warning", data.ResponseMessage, true, true, true);
                        }
                    }
                });
                function showMessage(context, type, text, isTemporary, isClosable, isVisible = true) {
                    showMessageBar(context, type, text, isTemporary, isClosable, isVisible);
                }
            },
            generateDate: function (dt) {
                // change string to "2017-04-01T07:30:00Z";
                var dateString = dt.Date;
                var formattedDateString = dateString.substr(0, 4)
                    + "-" + dateString.substr(4, 2)
                    + "-" + dateString.substr(6, 5)
                    + ":" + dateString.substr(11, 2)
                    + ":" + dateString.substr(13, 3);

                // create date obj with Timezone info
                //var dateObj = new Date();
                return formattedDateString;
            },
            btnExcelClick: function () {
                var table = $('#rptGrid').DataTable();
                var data = table.buttons.exportData();
            },
            btnCsvClick: function () {
                alert("btncsv clicked")
            }
        };
    });
}(Sitecore.Speak));