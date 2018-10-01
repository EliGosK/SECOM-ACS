var app = {
    messages: {
        ajax: {
            "loading": "Please wait while data is loading.."
        },
        validators: {
            "required": "{0} is required."
        },
        confirmation: {
            "proceedButtonText": "PROCEED",
            "cancelButtonText": "CANCEL"
        },
        dataSource: {
            "requestEnd": {
                "readNoRecord": "No record found from request.",
                "updateSuccess": "The data was {0} successfully.",
                "createSuccess": "The data was {0} successfully.",
                "destroySuccess":"The data was deleted successfully."
            },
            "noRecordFound": "No data from search result.",
        },
        grid: {
            popupEditor: {
                create: {
                    title: 'New',
                    button: { text: "Save" }
                },
                edit: {
                    title: 'Edit',
                    button: { text: "Save" }
                },
                cancelButton: {
                    text: "Cancel"
                }
            }
        },
        upload: {
            cancel: "Cancel",
            clearSelectedFiles: "Clear",
            remove: "Remove",
            select: "Select"
        },
        "scrollToTop": "Scroll to top"
    },
    data: {
        toDateString: function (date) {
            if (date && date.toISOString!==undefined)
                return date.toISOString();
            return "";
        },
        getName: function (items, value) {
            var item = $.grep(items, function (n, i) { return n.Value === value });
            return item.length > 0 ? item[0].Name : "";
        },
        getDisplayName: function (items, value) {
            var item = $.grep(items, function (n, i) { return n.Value === value });
            return item.length > 0 ? item[0].DisplayName : "";
        },
    },
    ui: {
        getRequiredMessage: function (input)
        {
            var field = input.data("field");
            if (field)
                return kendo.format(app.messages.validators.required, field);
            return app.messages.validators.required;
        },
        preventKeyEventInPage: function () {
            var selector = $("body");
            selector.on('keypress', function (e) {
                e = e || window.event;
                var key = e.keyCode;
                var srcElement = e.target || e.srcElement;
                var cancelEvent = !(srcElement.type === "submit" || srcElement.type === "button" || srcElement.tagName.toLocaleLowerCase() === "textarea");

                if (key === 13) {
                    e.returnValue = !cancelEvent;
                    e.cancelBubble = cancelEvent;
                    if (cancelEvent) { e.preventDefault(); }
                    return !cancelEvent;
                }
                return true;
            });

            selector.on('keydown', function (e) {
                e = e || window.event;
                var key = e.keyCode;
                var srcElement = e.target || e.srcElement;
                var cancelEvent = (srcElement.type === "submit" || srcElement.type === "button" || (srcElement.type === "text" && srcElement.readOnly));

                if (key === 8) {
                    e.returnValue = !cancelEvent;
                    e.cancelBubble = cancelEvent;
                    if (cancelEvent) { e.preventDefault(); }
                    return !cancelEvent;
                }
                return true;
            });
        },
        initAjax: function () {
            var notification = $("#ajax-notifications").kendoNotification({
                position: {
                    pinned: true,
                    bottom: 30,
                    left: 10
                },
                animation: {
                    open: {
                        effects: "fadeIn"
                    },
                    close: {
                        effects: "fadeOut"
                    }
                },
                width: 400,
                autoHideAfter: 0,
                stacking: "down",
                templates: [
                    {
                        type: "progress",
                        template: "<div class=\"k-notification-container\">" +
                        "<div class=\"content\"><i class=\"fa fa-spinner fa-spin\"></i> #= message #</div>" +
                        "</div>"
                    }
                ]
            }).data("kendoNotification");

            $(document).ajaxStart(function () {
                notification.show({ message: app.messages.ajax.loading }, "progress");
            }).ajaxStop(function () {
                notification.hide();
            });
        },
        clear: function (target) {
            var elems = [];
            if (!$.isArray(target)) {
                elems.push(target);
            } else {
                elems = $.merge([], target);
            }

            $.each(target, function (index, elem) {
                var t = $(elem).data("kendoGrid");
                if (t) {
                    t.dataSource.data([]);
                    return;
                }

                t = $(elem).data("kendoAutoComplete");
                if (t) {
                    var v = t.value();
                    t.value('');
                    if (v !== '') {
                        t.trigger("change");
                    }
                    return;
                }

                t = $(elem).data("kendoMultiSelect");
                if (t) { t.value([]); return; }

                t = $(elem).data("kendoDropDownList");
                if (t) {
                    t.select(0);
                    t.trigger("select");
                    return;
                }

                t = $(elem).data("kendoNumericTextBox");
                if (t) {
                    var d = $(t.element).data("default");
                    if (typeof d !== "undefined") {
                        if (t.value() !== d) {
                            t.value(d);
                            t.trigger("change");
                        }
                    }
                    return;
                }

                t = $(elem).data("kendoDatePicker");
                if (t) {
                    var d = $(t.element).data("default");
                    if (typeof d !== "undefined") {
                        var v = kendo.parseDate(d, t.options.parseFormats);
                        if (v !== t.value()) {
                            t.value(v);
                            t.trigger("change");
                        }
                    }
                    return;
                }

                t = $(elem).data("kendoDateTimePicker");
                if (t) {
                    var d = $(t.element).data("default");
                    if (typeof d !== "undefined") {
                        var v = kendo.parseDate(d, t.options.parseFormats);
                        if (v !== t.value()) {
                            t.value(v);
                            t.trigger("change");
                        }
                    }
                    return;
                }

                t = $(elem).data("kendoTextBox");
                if (t) { t.value(''); return; }

                if ($(elem).is("[type=checkbox]")) {
                    var doChecked = $(elem).is("[data-default-checked]");
                    if (doChecked)
                        $(elem).attr("checked", "checked");
                    else
                        $(elem).removeAttr("checked");
                }

                $(elem).val('');
            });
        },
        handleDatePickerNullValue: function (datePicker) {
            if (datePicker.value() == null) {
                var d = $(datePicker.element).data("default");
                if (typeof d !== "undefined") {
                    var v = kendo.parseDate(d, datePicker.options.format);
                    if (v !== datePicker.value()) {
                        datePicker.value(v);
                        datePicker.trigger("change");
                    }
                } else {
                    datePicker.value(new Date());
                    datePicker.trigger("change");
                }
            }
        },
        datePickerRelate: function (datepickerFrom, datepickerTo) {
            datepickerFrom.bind("change", function (e) {
                if (this.value() > datepickerTo.value()) {
                    if (datepickerTo.value() !== this.value()) {
                        datepickerTo.value(this.value());
                        datepickerTo.trigger("change");
                    }
                }

            });
            datepickerTo.bind("change", function (e) {
                if (this.value() < datepickerFrom.value()) {
                    if (datepickerFrom.value() !== this.value()) {
                        datepickerFrom.value(this.value());
                        datepickerFrom.trigger("change");
                    }
                }
            });
        },
        handleDataSourceRequestEnd: function (e) {
            var handled = false;
            if (typeof e.type !== "undefined") {

                switch (e.type) {
                    case "create":
                        var message = e.response.message !== undefined ? e.response.message : kendo.format(app.messages.dataSource.requestEnd.createSuccess, e.type);
                        $.notify({
                            icon: 'glyphicon glyphicon-info-sign',
                            message: message
                        }, {
                                // settings
                                delay: 3000,
                                type: 'success'
                            });
                        return true;
                    case "update":
                        var message = e.response.message !== undefined ? e.response.message : kendo.format(app.messages.dataSource.requestEnd.updateSuccess, e.type);
                        $.notify({
                            icon: 'glyphicon glyphicon-info-sign',
                            message: message
                        }, {
                                // settings
                                delay: 3000,
                                type: 'success'
                            });
                        return true;
                    case "destroy":
                        var message = e.response.message !== undefined ? e.response.message : app.messages.dataSource.requestEnd.destroySuccess;
                        $.notify({
                            icon: 'glyphicon glyphicon-info-sign',
                            message: message
                        }, {
                                // settings
                                delay: 3000,
                                type: 'success'
                            });
                        return true;
                    case "read":
                        if (e.response !== undefined && e.response.Data !== undefined) {
                            if (e.response.Data.length == 0) {
                                $.notify({
                                    icon: 'glyphicon glyphicon-info-sign',
                                    message: app.messages.dataSource.requestEnd.readNoRecord
                                }, {
                                        // settings
                                        delay: 3000,
                                        type: 'warning'
                                    });
                            }
                        }
                        break;
                }
            }
            return handled;
        },
        handleDataSourceError: function (e, selector) {
            if (e.status === "error") {
                var message = e.xhr.responseJSON !== undefined ? e.xhr.responseJSON.message : e.xhr.statusText;
                var isSessionTimeOut = e.xhr.responseJSON !== undefined ? (e.xhr.responseJSON.sessiontimeOut !== undefined ? e.xhr.responseJSON.sessiontimeOut: false) : false;
                $.notify({
                    // options
                    icon: 'glyphicon glyphicon-exclamation-sign',
                    message: message
                }, {
                    // settings
                    delay: 3000,
                    type: 'danger'
                    });

                if (isSessionTimeOut === true) {
                    window.location = "/";
                }
                return { handled: true, message: message };
            }
            return { handled: false };
        },
        handleAjaxSuccess: function (data, status, xhr) {
            var message = data.message !== undefined ? data.message : "Data request is successfully.";
            $.notify({
                icon: 'glyphicon glyphicon-info-sign',
                message: message
            }, {
                    // settings
                    delay: 3000,
                    type: 'success'
                });
            return message;
        },
        handleAjaxError: function (xhr, status, error) {
            var message = xhr.responseJSON !== undefined && xhr.responseJSON.message !== undefined ? xhr.responseJSON.message : "Internal Server Error";
            var isSessionTimeOut = xhr.responseJSON !== undefined ? (xhr.responseJSON.sessiontimeOut !== undefined ? xhr.responseJSON.sessiontimeOut : false) : false;
            $.notify({
                // options
                icon: 'glyphicon glyphicon-exclamation-sign',
                message: message
            }, {
                    // settings
                    delay: 3000,
                    type: 'danger'
                });

            if (isSessionTimeOut === true) {
                window.location = "/";
            }
            return message;
        },
        applySelectAllTextOnFocus: function (selectors) {
            selectors = selectors || "input[type=text][role!=listbox],input.k-textbox[role!=listbox],textarea";
            $(selectors).on("focus", function () {
                var input = $(this);
                clearTimeout(input.data("selectTimeId")); //stop started time out if any

                var selectTimeId = setTimeout(function () {
                    input.select();
                });

                input.data("selectTimeId", selectTimeId);
            }).blur(function (e) {
                clearTimeout($(this).data("selectTimeId")); //stop started timeout
            });
        },
        applyRegExpTextBox: function (selector, pattern) {
            var j = selector instanceof jQuery ? selector : $(selector);
            j.on("keydown", function (e) {
                if (!pattern.test(e.key)) {
                    e.preventDefault();
                }
                //console.log(e.key);
            });
        },
        applyLetterTextBox: function (selectors) {
            var j = selectors instanceof jQuery ? selectors : $(selectors || "[data-role='lettertextbox']");
            j.on("keydown", function (e) {
                var pattern = /[A-Za-z0-9_-]/i;
                if (!pattern.test(e.key)) {
                    e.preventDefault();
                }
                //console.log(e.key);
            });
        },
        applyConfirmDialog: function (selectors) {
            selectors = selectors || "[data-role='confirm']";
            var dataOptionsMapping = {
                'title': 'title',
                //'title-class': 'titleClass',
                //'type': 'type',
                //'type-animated': 'typeAnimated',
                //'draggable': 'draggable',
                //'align-middle': 'alignMiddle',
                'content': 'content',
                'icon': 'icon',
                //'bg-opacity': 'bgOpacity',
                'theme': 'theme',
                //'animation': 'animation',
                //'close-animation': 'closeAnimation',
                //'animation-speed': 'animationSpeed',
                //'animation-bounce': 'animationBounce',
                'escape-key': 'escapeKey',
                //'rtl': 'rtl',
                //'container': 'container',
                //'container-fluid': 'containerFluid',
                //'background-dismiss': 'backgroundDismiss',
                //'background-dismiss-animation': 'backgroundDismissAnimation',
                //'auto-close': 'autoClose',
                //'close-icon': 'closeIcon',
                //'close-icon-class': 'closeIconClass',
                'use-bootstrap': 'useBootstrap'
            };

            $(selectors).each(function (index, elem) {
                $(elem).on('click', function (e) {
                    e.preventDefault();
                    var dataOptions = {};
                    $.each(dataOptionsMapping, function (attributeName, optionName) {
                        var value = $(elem).data(attributeName);
                        if (typeof value !== "undefined") {
                            dataOptions[optionName] = value;
                        }
                    });

                    var settings = $.extend(dataOptions, {
                        buttons: {
                            yes: {
                                text: "<i class=\"fa fa-check-circle\"></i> " + app.messages.confirmation.proceedButtonText,
                                keys: ['enter'],
                                btnClass: 'btn-primary btn-lg',
                                action: function () {

                                }
                            },
                            no: {
                                text: "<i class=\"fa fa-times-circle\"></i> " + app.messages.confirmation.cancelButtonText,
                                keys: ['esc'],
                                btnClass: 'btn-danger btn-lg',
                                action: function () {

                                }
                            }
                        }
                    });

                    var args = { isValid: true };
                    $(this).trigger("validating", [args, elem]);
                    if (args.isValid) {
                        $(this).trigger("openDialog", [settings,elem]);
                        $.confirm(settings);
                    }
                });
            });
            return $(selectors);
        },
        showValidateSummary: function (errors, selector) {
            selector = selector || ".k-validation-summary";

            if (errors.length > 0) {
                var html = "<ul>";
                for (var idx = 0; idx < errors.length; idx++) {
                    html += "<li>" + errors[idx] + "</li>";
                }
                html += "</ul>";
                $(selector).show().empty().append($(html));
            } else {
                $(selector).hide();
            }
        },
        hideVaidationMessages: function (v, selector) {
            selector = selector || ".k-validation-summary";
            v.hideMessages();
            $(selector).hide();
        },
        initGridPopupEditor: function (e, options) {
            //<span class="k-icon k-i-save"></span> Save
            options = $.extend(true, {
                create: {
                    title: app.messages.grid.popupEditor.create.title,
                    button: {
                        text: app.messages.grid.popupEditor.create.button.text,
                        icon: "save",
                        content: ''
                    }
                },
                edit: {
                    title: app.messages.grid.popupEditor.edit.title,
                    button: {
                        text: app.messages.grid.popupEditor.edit.button.text,
                        icon: "save",
                        content: ''
                    }
                },
                cancelButton: {
                    text: app.messages.grid.popupEditor.cancelButton.text,
                    icon: "cancel",
                    content: ''
                }
            }, options || {});

            var updateButton = $(e.container).parent().find(".k-grid-update");
            var cancelButton = $(e.container).parent().find(".k-grid-cancel");
            if (e.model.isNew()) {
                e.container.kendoWindow("title", options.create.title);
                var content = options.create.button.content === '' ? kendo.format('<span class="k-icon k-i-{0}"></span> {1}', options.create.button.icon, options.create.button.text) : options.create.button.content;
                $(updateButton).html(content);
            } else {
                e.container.kendoWindow("title", options.edit.title);
                var content = options.edit.button.content === '' ? kendo.format('<span class="k-icon k-i-{0}"></span> {1}', options.edit.button.icon, options.edit.button.text) : options.edit.button.content;
                $(updateButton).html(content);
            }
            var content = options.cancelButton.content === '' ? kendo.format('<span class="k-icon k-i-{0}"></span> {1}', options.cancelButton.icon, options.cancelButton.text) : options.cancelButton.content;
            $(cancelButton).html(content);
        },
        handleGridDataSourceRequestEnd: function (e, grid) {
            if (typeof e.type !== "undefined") {
                switch (e.type) {
                    case "create":
                    case "update":
                    case "delete":
                        if (e.response) {
                            grid.dataSource.read();
                        }
                        break;
                }
            }
        },
        applyTableHover: function (grid, options) {
            if (grid.table.tableHover !== undefined) {
                options = $.extend(true, { rowClass: 'k-grid-row-hover', colClass: 'k-grid-column-hover' }, options || {});
                grid.table.tableHover(options);
            }
        },
        initNotification: function () {
            // require call partialview _KendoNotication
            return $("#notifications").kendoNotification({
                position: {
                    pinned: true,
                    top: 100,
                    right: 40
                },
                width: 400,
                autoHideAfter: 3000,
                stacking: "down",
                templates: [
                    {
                        type: "info",
                        template: "<div class=\"k-notification-container\">" +
                        "<div class=\"title\"><i class=\"fa fa-info-circle\"></i> #= title #</div>" +
                        "<div class=\"content\">#= message #</div>" +
                        "</div>"
                    }, {
                        type: "error",
                        template: "<div class=\"k-notification-container\">" +
                        "<div class=\"title\"><i class=\"fa fa-times-circle\"></i> #= title #</div>" +
                        "<div class=\"content\">#= message #</div>" +
                        "</div>"
                    }, {
                        type: "success",
                        template: "<div class=\"k-notification-container\">" +
                        "<div class=\"title\"><i class=\"fa fa-check-circle\"></i> #= title #</div>" +
                        "<div class=\"content\">#= message #</div>" +
                        "</div>"
                    }
                ]

            }).data("kendoNotification");
        },
        setCheckBox: function (selector, checked) {
            if (checked)
                selector.prop("checked", "checked");
            else
                selector.prop("checked", false);
        },
        applyUploadLocalize: function (selector) {
            var upload = $(selector).data("kendoUpload");
            $.extend(upload.options.localization, app.messages.upload);
        },
        handleSessionTimeout: function (e) {

        },
        initGridRowNo: function (grid, rowNoFieldName) {
            var page = grid.dataSource.page() || 1;
            var pageSize = grid.dataSource.pageSize() || grid.dataSource.data().length;
            var start = (page - 1) * pageSize + 1;
            rowNoFieldName = rowNoFieldName || "_rowNo";

            $.each(grid.dataSource.data(), function (index, item) {
                item[rowNoFieldName] = start + index;
            });
        },
        resizeGrid: function (selector) {
            if ($.isArray(selector)) {
                $.each(function (index, item) {
                    var grid = $(item).data("kendoGrid");
                    if (grid) {
                        grid.resize();
                    }
                });
            } else {
                var grid = $(selector).data("kendoGrid");
                if (grid) {
                    grid.resize();
                }
            }
        },
        uiEnable: function (selector, enable) {
            if (enable)
                $(selector).removeAttr('disabled').removeClass('k-state-disabled');
            else
                $(selector).attr('disabled','disabled').addClass('k-state-disabled');
        }

    },
    initPage: function () {
        this.ui.initAjax();
        this.ui.applySelectAllTextOnFocus();
        this.ui.applyConfirmDialog();
        this.ui.preventKeyEventInPage();
        // Apply RegExp TextBox.
        //var regexpTextBoxes = [
        //    { selector: "[data-role='machinetextbox']", pattern: /[A-Za-z0-9_-]/ },
        //    { selector: "[data-role='lettertextbox']", pattern: /[A-Za-z0-9\s_-]/ },
        //    { selector: ".k-machine-no-textbox", pattern: /[A-Za-z0-9_-]/ },
        //    { selector: ".k-machine-name-textbox", pattern: /[A-Za-z0-9\s_-]/ }
        //];
        //$.each(regexpTextBoxes, function (index, item) {
        //    app.ui.applyRegExpTextBox(item.selector, item.pattern);
        //});
    },
    setLocation: function (url) {
        window.location.href = url;
    },
    guid: function () {
        function _p8(s) {
            var p = (Math.random().toString(16) + "000000000").substr(2, 8);
            return s ? "-" + p.substr(0, 4) + "-" + p.substr(4, 4) : p;
        }
        return _p8() + _p8(true) + _p8(true) + _p8();
    },
    addAntiForgeryToken: function (data) {
        //if the object is undefined, create a new one.
        if (!data) {
            data = {};
        }
        //add token
        var tokenInput = $('input[name=__RequestVerificationToken]');
        if (tokenInput.length) {
            data.__RequestVerificationToken = tokenInput.val();
        }
        return data;
    },
    displayFileSize: function (size) {
        var units = ['Bytes', 'KB', 'MB', 'GB'];
        for (var i = 0; i < units.length; i++) {
            if (size < 1024) { return kendo.toString(size, "n2") + " " + units[i]; }
            size = parseFloat(size) / 1024;
        }
        return kendo.toString(size, "n2");
    }
};

var CheckBoxHelper = function (selector, dataStateId) {
    var checkbox = selector;
    var id = dataStateId;

    this.updateDataState = function () {
        var dataItems = JSON.parse($(id).val() || "[]");
        $(checkbox).each(function (index, item) {
            var value = this.value;
            dataItems = $.grep(dataItems, function (dataItem) {
                return dataItem !== value;
            });

            if (this.checked) {
                dataItems.push(value);
            }
        });
        $(id).val(kendo.stringify(dataItems));
    }

    this.updateUIState = function () {
        var dataItems = JSON.parse($(id).val() || "[]");
        app.ui.setCheckBox($(checkbox), false);
        $.each(dataItems, function (index, item) {
            app.ui.setCheckBox($(checkbox + "[value='" + item + "']"), true);
        });
    }

    this.applyChange = function (cb) {
        $(checkbox).on("change", function (e) {
            var dataItems = JSON.parse($(id).val() || "[]");
            var value = this.value;
            dataItems = $.grep(dataItems, function (dataItem, index) {
                return dataItem !== value;
            });
            if (this.checked) {
                dataItems.push(this.value);
            }
            $(id).val(kendo.stringify(dataItems));

            if (cb !== undefined) {
                cb();
            }
        });
    }

    this.getDataState = function () {
        return JSON.parse($(id).val() || "[]");
    }

    this.clearDataState = function () {
        $(id).val("[]");
    }

    this.validate = function (maxSelectedItems) {
        var dataItems = JSON.parse($(id).val() || "[]");
        if (maxSelectedItems == undefined) { return true; }
        return dataItems.length < maxSelectedItems;
    }
}

var KendoGridHelper = function (grid) {
    var widget = grid;
    var rowNo = 0;

    this.resetRowNo = function () {
        rowNo = 0;
    }

    this.grid = function () {
        return widget;
    }

    this.renderRowNo = function (model) {
        var page = widget.dataSource.page() || 1;
        var pageSize = widget.dataSource.pageSize() || 20;
        var start = (page - 1) * pageSize;

        if (model) {
            if (model._rowno !== undefined) { return model._rowno; }
            rowNo = rowNo + 1;
            model._rowno = Math.max(start + rowNo, 1)
            return model._rowno;
        } else {
            rowNo = rowNo + 1;
            return Math.max(start + rowNo, 1);
        }
    }

    this.notifyNoData = function (message) {
        if (widget.dataSource.data().length == 0) {
            message = message || app.messages.dataSource.noRecordFound;
            $.notify({
                icon: 'glyphicon glyphicon-info-sign',
                message: message
            }, {
                    // settings
                    delay: 3000,
                    type: 'info'
                });
        }
    }


};

// ui.register("search",".k-button");
// ui.register("search",[{ elem: ".k-button-search", busyContent: ""},{elem: ".k-button"}]);
// ui.register("search",[".k-button-search",".k-button"]);
var AppUIState = function () {
    var uiStates = [];
    this.register = function (name, options) {

        for (var i = 0; i < uiStates.length; i++) {
            if (uiStates[i].name == name) {
                uiStates.splice(i, 1);
                break;
            }
        }

        if ($.isArray(options)) {
            var data = { 'name': name, 'targets': [] };
            $.each(options, function (i, item) {
                if (item.elem !== undefined) {
                    var selector = item.elem instanceof jQuery ? item.elem : $(item.elem);
                    data.targets.push({
                        'elem': selector,
                        'content': $(item.elem).html(),
                        'busyContent': item.busyContent
                    });
                } else {
                    var selector = item instanceof jQuery ? item : $(item);
                    data.targets.push({ 'elem': selector });
                }

            });
            uiStates.push(data);
        } else {
            var selector = item instanceof jQuery ? item : $(item);
            uiStates.push({
                'name': name,
                'targets': [{ 'elem': selector }]
            });
        }
    }
    this.clear = function () {
        uiStates = [];
    }

    this.busy = function (name, isBusy) {
        if (isBusy) {
            var ui = $.grep(uiStates, function (item, i) {
                return item.name == name;
            });

            $.each(ui, function (i, item) {
                $.each(item.targets, function (index, target) {
                    setTimeout(function () {
                        //$.each(target.elem, function (i, d) {
                        //    $(d).attr("ui-state-disabled", $(d).hasClass('k-state-disabled')||$(d).is(":disabled"));
                        //    $(d).attr('disabled', 'disabled').addClass('k-state-disabled');                            
                        //});
                        target.elem.attr('disabled', 'disabled').addClass('k-state-disabled');
                        if (target.busyContent !== undefined) {
                            target.elem.html(target.busyContent);
                        }
                    }, 100);
                });
            });
        } else {
            var ui = $.grep(uiStates, function (item, i) {
                return item.name == name;
            });

            $.each(ui, function (i, item) {
                $.each(item.targets, function (index, target) {
                    setTimeout(function () {
                        //$.each(target.elem, function (i, d) {
                        //    console.log(d);
                        //    if ($(d).attr('ui-state-disabled')!=="true") {
                        //        $(d).removeAttr('disabled').removeClass('k-state-disabled');
                        //    }
                        //});
                        //target.elem.removeAttr('ui-state-disabled');
                        target.elem.removeAttr('disabled').removeClass('k-state-disabled');
                        if (target.content !== undefined) {
                            target.elem.html(target.content);
                        }
                    }, 100);
                });
            });
        }
    }
};

(function ($, kendo) {
    $.extend(true, kendo.ui.validator, {
        rules: {
            whitespaceorempty: function (input) {
                return !(input.filter("input[required]").length && $.trim(input.val()) === "");
            },
            mvcdate: function (input) {
                if (input.is("[data-val-date]") && input.val() != "") {
                    var date = kendo.parseDate(input.val(), "d/MM/yyyy");
                    return date !== null;
                }
                return true;
            }
        },
        messages: {
            required: function (input) {
                var field = input.data("field");
                if (field)
                    return kendo.format(app.messages.validators.required, field);
                return app.messages.validators.required;
            },
            whitespaceorempty: function (input) {
                var field = input.data("field");
                if (field)
                    return kendo.format(app.messages.validators.required, field);
                return app.messages.validators.required;
            }
        }
    });
})(jQuery, kendo);


$(function () {
    if ($.scrollUp !== "undefined") {
        $.scrollUp({
            scrollName: 'scroll-up',
            animationSpeed: '600',
            scrollText: '<i class="fa fa-4x fa-chevron-circle-up"></i>',
            scrollTitle: app.messages.scrollToTop
        });
    }
});