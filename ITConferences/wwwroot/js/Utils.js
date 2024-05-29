(function ($) {
    let S4 = function () {
        return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
    };

    let alertifyNotify = function (style, position, message) {
        let msgtitle = "";
        let icon = 'fa fa-adjust';

        switch (position) {
            case "tc":
                pos = "top center";
                break;
            case "tl":
                pos = "top left";
                break;
            case "tr":
                pos = "top right";
                break;
            case "bl":
                pos = "bottom left";
                break;
            case "br":
                pos = "bottom right";
                break;
        }


        switch (style) {
            case "default":
                msgtitle = "Thông báo";
                icon = "fa fa-adjust";
                break;
            case "info":
                msgtitle = "Thông tin";
                icon = "fa fa-question";
                break;
            case "warning":
                msgtitle = "Cảnh báo";
                icon = "fas fa-exclamation-circle";
                break;
            case "error":
                msgtitle = "Không thành công";
                icon = "fa fa-exclamation";
                break;
            case "success":
                msgtitle = "Thành công";
                icon = "fa fa-check";
                break;
        }

        $.notify({
            title: msgtitle,
            text: message,
            image: "<i class='" + icon + "'></i>"
        }, {
            style: 'metro',
            className: style,
            globalPosition: position,
            showAnimation: "show",
            showDuration: 0,
            hideDuration: 0,
            autoHide: true,
            clickToHide: true
        });
    }

    const Utils = {
        Msg: {
            OK: "OK",
            YESNO: "YESNO",
            YESIGNORE: "YESIGNORE",
            SENDBUGTOSERVER: "SENDBUGTOSERVER",
            BUG: "bug",
            WARNING: "exclamation-circle",
            INFO: "info-circle",
            QUESTION: "question-circle",
            ERROR: "times-circle",
            SUCCESS: "check-circle",
            UNSUCCESS: "meh",
            show: function (opts) {
                var defaultOpts = {
                    title: "Thông báo",
                    msg: "",
                    errorMsg: "",
                    icon: "info-circle",
                    buttons: "OK",
                    btnConfig: null,
                    fn: null,
                    ajaxErr: false,
                };
                opts = $.extend({}, defaultOpts, opts);

                var type = "default";
                switch (opts.icon) {
                    case Utils.Msg.WARNING:
                    case Utils.Msg.QUESTION:
                        type = "orange";
                        break;
                    case Utils.Msg.INFO:
                        type = "blue";
                        break;
                    case Utils.Msg.BUG:
                        type = "dark";
                        break;
                    case Utils.Msg.ERROR:
                        type = "red";
                        break;
                    case Utils.Msg.SUCCESS:
                        type = "green";
                        break;
                    case Utils.Msg.UNSUCCESS:
                        type = "purple";
                        break;
                }

                var maxWidth = 400;
                var contentWidth = 0;

                var el = $('<span>').html(opts.msg);
                $('body').append(el);
                contentWidth = el.width() > contentWidth ? el.width() : contentWidth;
                el.remove();

                el = $('<span>').html(opts.errorMsg);
                $('body').append(el);
                contentWidth = el.width() > contentWidth ? el.width() : contentWidth;
                el.remove();

                var content = [
                    '<div class="jconfirm-icon">',
                    '<i class="fal fa-' + opts.icon + '"></i>',
                    '</div>',
                    '<div class="jconfirm-message">',
                    '<p>' + opts.msg + '</p>',
                    '<p>' + opts.errorMsg + '</p>',
                    '</div>'
                ];

                var buttons = {};
                switch (opts.buttons) {
                    case Utils.Msg.OK:
                        buttons = {
                            ok: {
                                text: 'Đồng ý',
                                btnClass: 'btn-' + type
                            }
                        }
                        break;
                    case Utils.Msg.YESNO:
                        buttons = {
                            yes: {
                                text: 'Đồng ý',
                                btnClass: 'btn-' + type
                            },
                            no: {
                                text: 'Không',
                                btnClass: 'btn-default'
                            }
                        }
                        break;
                    case Utils.Msg.YESIGNORE:
                        buttons = {
                            yes: {
                                text: 'Đồng ý',
                                btnClass: 'btn-' + type
                            },
                            ignore: {
                                text: 'Bỏ qua',
                                btnClass: 'btn-default'
                            }
                        }
                        break;
                }

                if (opts.btnConfig) {
                    for (var prop in buttons) {
                        if (opts.btnConfig[prop]) {
                            buttons[prop] = $.extend(buttons[prop], opts.btnConfig[prop]);
                        }
                    }
                }

                for (var prop in buttons) {
                    buttons[prop].action = (function (btn) {
                        return function () { if (typeof (opts.fn) == "function") opts.fn(btn); }
                    })(prop.toUpperCase());
                }

                $.confirm({
                    title: opts.title,
                    type: type,
                    theme: 'pitriti-theme',
                    animationSpeed: 300,
                    boxWidth: (contentWidth == 0 || contentWidth > maxWidth ? maxWidth : contentWidth) + 100,
                    useBootstrap: false,
                    containerFluid: true,
                    content: content.join(""),
                    buttons: buttons
                });
            },
        },
        Alertify: {
            default: function (pos, mes) {
                alertifyNotify("white", pos, mes);
            },
            info: function (pos, mes) {
                alertifyNotify("info", pos, mes);
            },
            warning: function (pos, mes) {
                alertifyNotify("warning", pos, mes);
            },
            error: function (pos, mes) {
                alertifyNotify("error", pos, mes);
            },
            success: function (pos, mes) {
                alertifyNotify("success", pos, mes);
            }
        },
        DateTime: {
            Parser: {
                fromDateTimeString: function (dateTimeString) {
                    var date;

                    if (typeof dateTimeString === 'string' && !$.isNumeric(dateTimeString)) {
                        let aspnetDate = /\/Date\((-*\d*)\)\//.exec(dateTimeString);
                        let isoDate = /^(\d{4})\D?(0[1-9]|1[0-2])\D?([12]\d|0[1-9]|3[01])(\D?([01]\d|2[0-3])\D?([0-5]\d)\D?([0-5]\d)?\D?(\d{3})?([zZ]|([\+-])([01]\d|2[0-3])\D?([0-5]\d)?)?)?$/.exec(dateTimeString);
                        let strDate = /^(0?[1-9]|[12][0-9]|3[01])[\/\-](0?[1-9]|1[012])[\/\-]\d{4}$/.exec(dateTimeString);

                        if (aspnetDate) {
                            date = new Date(+aspnetDate[1]);
                        } else if (isoDate) {
                            date = new Date(dateTimeString);
                        } else if (strDate) {
                            date = new Date(dateTimeString.replace(/(\d{2})\/(\d{2})\/(\d{4})/, '$3-$2-$1'));
                        }
                    }
                    else if (dateTimeString instanceof Date) {
                        date = dateTimeString;
                    }

                    if (date instanceof Date) {
                        date.toString = Utils.DateTime.Converter.toString;

                        return date;
                    }

                    return dateTimeString;
                }
            },
            Converter: {
                toString: function (includeTime) {
                    var strDateTime = ('0' + this.getDate()).slice(-2) + '/' + ('0' + (this.getMonth() + 1)).slice(-2) + '/' + ("000" + this.getFullYear()).slice(-4);

                    if (includeTime) {
                        strDateTime += (" " + ('0' + this.getHours()).slice(-2) + ":" + ('0' + this.getMinutes()).slice(-2));
                    }

                    return strDateTime;
                }
            }
        },
        Form: {
            getFormValue: function (form) {
                if (typeof (form) === 'string') {
                    var elementId = (form[0] !== "#") ? "#" + form : form
                    form = $(elementId);
                }
                else if (typeof (form) === 'object' && !(form instanceof jQuery)) {
                    form = $(form);
                }
                if (!form) return null;
                var frm = {};
                var listModuleNames = [];

                form.find('[name]').each(function (index, item) {
                    var closestVModule = $(item).closest('.VModule');

                    if (closestVModule.length == 0 || listModuleNames.indexOf(closestVModule.attr('vmodule-id')) == -1) {
                        var name = item.getAttribute('name');

                        if (name) {
                            if (item.type === 'checkbox') {
                                frm[name] = item.checked;
                            }
                            else if (item.type === 'radio') {
                                if (item.checked) {
                                    frm[name] = item.value;
                                }
                            }
                            else {
                                if (item.value || item.value === '') {
                                    let jinput = $(item);
                                    if ((jinput.hasClass('number') || jinput.attr('data-type') === "int") && item.value !== '') {
                                        let value = parseInt(item.value);
                                        if (isNaN(value)) {
                                            frm[name] = 0;
                                        }
                                        else
                                            frm[name] = value;
                                    }
                                    else if ((jinput.hasClass('area') || jinput.hasClass('area2d') || jinput.hasClass('money') || jinput.attr('data-type') === "decimal") && item.value != '') {
                                        let value = parseFloat(item.value);
                                        if (isNaN(value)) {
                                            frm[name] = 0;
                                        }
                                        else
                                            frm[name] = value;
                                    }
                                    else if (jinput.hasClass('datepicker') || jinput.hasClass('datepicker-from') || jinput.hasClass('datepicker-to') || jinput.hasClass('datetimepicker') || jinput.hasClass('datetimepicker-from') || jinput.hasClass('datetimepicker-from')) {
                                        frm[name] = jinput.datepicker("getDate");
                                    }
                                    else if (jinput.attr('data-datepicker') == "true" && item.value) {
                                        frm[name] = new Date(item.value.replace(/(\d{2})\/(\d{2})\/(\d{4})/, "$2/$1/$3"))
                                    }
                                    else {
                                        if (typeof (item.value) === 'string') {
                                            frm[name] = item.value.trim();
                                        }
                                        else {
                                            frm[name] = item.value;
                                        }

                                    }

                                    if (jinput.hasClass('VModule')) {
                                        listModuleNames.push(item.getAttribute('vmodule-id'));
                                    }
                                }
                            }
                        }
                    }
                });

                let hiddenData = form.data('hiddenData');

                if (hiddenData) {
                    frm = $.extend({}, frm, hiddenData);
                }
                return frm;
            },
            clearFormData: function (form) {

                if (typeof (form) != 'object') {
                    if (form[0] != "#") {
                        form = "#" + form;
                    }

                    form = $(form);
                }
                else if (typeof (form) == 'object' && !(form instanceof jQuery)) {
                    form = $(form);
                }

                if (!form) return null;

                var frm = {};

                form.find('[name]').each(function (index, item) {
                    var type = item.getAttribute("type");
                    if (type == "text" || type == "email" || type == "password" || type == "hidden") {
                        if (item.name !== "__RequestVerificationToken") {
                            $(item).val('').trigger('change');
                        }
                    }
                    else if (type == "checkbox") {
                        item.checked = false;
                    }
                    else if (type == "file") {
                        $(item).wrap('<form>').closest('form').get(0).reset();
                        $(item).unwrap();
                    }
                    else if (item.className.indexOf('VModule') != -1) {
                        item.value = null;
                        $(item).data('hiddenData', null);
                        $(item).trigger('initview');
                    }
                    else if (item.tagName == "SELECT") {
                        //$(item).val($(item).find('option:first').val()).change();
                        //if (item.selectedIndex == -1)
                        {
                            var selectItem = $(item).find('option:first').val();
                            $(item).val(selectItem).trigger('change');
                        }
                    }
                    else if (item.tagName == "TEXTAREA") {
                        item.value = '';
                    }
                });

                form.data('hiddenData', null);
            },
            bindFormData: function (form, data) {
                if (typeof (form) != 'object') {
                    if (form[0] != "#") {
                        form = "#" + form;
                    }

                    form = $(form);
                }
                else if (typeof (form) == 'object' && !(form instanceof jQuery)) {
                    form = $(form);
                }

                if (form) {
                    Utils.Form.clearFormData(form);
                    if (data) {
                        for (var prop in data) {
                            var input = form.find('[name="' + prop + '"]');
                            var type = input.attr('type');

                            if (input && input[0]) {
                                if (input.hasClass('datepicker') || input.hasClass('datepicker-from') || input.hasClass('datepicker-to') || input.hasClass('datetimepicker') || input.hasClass('datetimepicker-from') || input.hasClass('datetimepicker-from')) {
                                    if (data[prop]) {
                                        var date = Utils.DateTime.Parser.fromDateTimeString(data[prop]);
                                        input.val(date).trigger('change');
                                    }
                                }
                                else if (type == 'radio') {
                                    if (input.length > 1) {
                                        for (var i = 0; i < input.length; i++) {
                                            input[i].checked = input[i].value == data[prop] + "";
                                        }
                                    }

                                }
                                else if (type == 'checkbox') {
                                    input.prop('checked', (data[prop] == 1 || input.val() == true));
                                }
                                else if (type === "text" || type === "email" || type === "pasword" || type === "hidden" || input[0].tagName == "TEXTAREA") {
                                    input.val(data[prop]).trigger('change');
                                }
                                else if (input[0].tagName == "SELECT") {
                                    input.val(data[prop]).trigger('change');
                                    input.attr('data-default-select', data[prop]);
                                }
                                else if (type === 'file') {
                                    //do nothing
                                }
                                else { //VModule
                                    input[0].value = data[prop];
                                    input.trigger('initview');
                                }
                            }
                            else {
                                var exceptProps = ["InId", "OutId"];
                                if (typeof (data[prop]) !== 'object' && data[prop] && exceptProps.indexOf(prop) === -1) {
                                    //form.append('<input type="hidden" name=' + prop + ' value="' + data[prop] + '"></input>');
                                    let hiddenData = form.data('hiddenData') || {};
                                    hiddenData[prop] = data[prop];
                                    form.data('hiddenData', hiddenData);
                                }
                                var input = form.find('[value="' + prop + '"]');
                                var type = input.attr('type');
                                if (type === 'radio') {
                                    if (input.length > 0) {
                                        input[0].checked = data[prop];
                                    }
                                }
                            }
                        }
                    }
                }
            },
            bindRequiredSignal: function (jObject) {
                jObject.find('[data-rule-required="true"], [data-rule-date="true"]').closest('.form-group').find('label:not(:has(span))').each((index, label) => {
                    label.innerHTML += '<span class="required-signal">*</span>';
                });
            },
            formValidationErrorPlacement: function (error, element) {

                $(element).popover({
                    trigger: "manual",
                    placement: "top",
                    container: "body",
                    animation: false,
                    content: $(error).text(),
                    offset: '50',
                    template: `<div class="popover" role="tooltip"><div class="arrow"></div><div class="popover-body">${$(error).text()}</div></div>`
                });

                $(element).popover("show");

                return setTimeout(function () { $(element).popover("hide"); }, 2000);
            },
            Control: {
                initDataSelect2: function (dom) {
                    //select2 select2-autobind Config:
                    //data-auto-bind: Name of property in collection DanhMuc
                    //data-value-field: Value field
                    //data-display-field: Display field
                    //data-display-default: --Chọn abc---
                    //data-default-select: default select
                    //data-current-user: current user
                    var me = this;
                    var maxInterval = 200; //20s
                    var countInterval = 0;
                    if (dom && dom.getAttribute('data-auto-bind')) {
                        var interval = setInterval(function () { //<= set tạm vì không biết khi nào load xong DanhMuc
                            var danhMucs = localDB.getCollection("DanhMuc");
                            var domAttribute = dom.getAttribute('data-auto-bind');
                            var domType = dom.getAttribute('data-type');
                            var options;
                            if (danhMucs != null) {
                                if (danhMucs.data && danhMucs.data.length > 0 && danhMucs.data[0] && danhMucs.data[0].value) {
                                    options = danhMucs.data[0].value[domAttribute];
                                }
                                else {
                                    console.log(dom.getAttribute('data-auto-bind') + " not found in collection DanhMuc")
                                }
                                clearInterval(interval);
                                if (options != null) {
                                    if (GLOBAL.CurrentUser && domAttribute === 'huyens') {
                                        options = options.filter(function (item) {
                                            if (GLOBAL.CurrentUser && GLOBAL.CurrentUser.Tinh) {
                                                return item.tinhId == GLOBAL.CurrentUser.Tinh.tinhId;
                                            }
                                            else {
                                                return false;
                                            }
                                        }).map(function (item) {
                                            return {
                                                id: item[dom.getAttribute('data-value-field')],
                                                text: item[dom.getAttribute('data-display-field')]
                                            }
                                        });
                                    }
                                    else if (domAttribute === 'loaiBienDongs' && domType === 'chuyenQuyen') {
                                        var optionsChuyenQuyen = options.filter(chuyenquyen => chuyenquyen.laBienDongChuyenQuyen === true);
                                        options = optionsChuyenQuyen.map(function (item) {
                                            return {
                                                id: item[dom.getAttribute('data-value-field')],
                                                text: item[dom.getAttribute('data-display-field')]
                                            }
                                        });
                                    }
                                    else if (domAttribute === 'noiDungGhiChus') {
                                        var trangGhiChu = dom.getAttribute("data-trangGhiChu");
                                        if (!trangGhiChu) {
                                            trangGhiChu = 1;
                                        }
                                        var optionsNoiDungGhiChu = options.filter(noiDungGhiChu => noiDungGhiChu.trangGhiChu === parseInt(trangGhiChu));
                                        if (GLOBAL.CurrentUser.Huyen && GLOBAL.CurrentUser.Huyen.huyenId > 0) {
                                            optionsNoiDungGhiChu = optionsNoiDungGhiChu.filter(noiDungGhiChu => noiDungGhiChu.huyenId === GLOBAL.CurrentUser.Huyen.huyenId);
                                        }
                                        optionsNoiDungGhiChu.unshift({
                                            Id: "",
                                            noiDung: '--- Chọn nội dung ghi chú ---'
                                        });
                                        options = optionsNoiDungGhiChu.map(function (item) {
                                            return {
                                                id: item[dom.getAttribute('data-value-field')],
                                                text: item[dom.getAttribute('data-display-field')]
                                            }
                                        });
                                    }
                                    else {
                                        options = options.map(function (item) {
                                            return {
                                                id: item[dom.getAttribute('data-value-field')],
                                                text: item[dom.getAttribute('data-display-field')]
                                            }
                                        });
                                    }
                                }
                                else {
                                    options = [];
                                }

                                var defaultDispay = dom.getAttribute('data-display-default');
                                if (defaultDispay != null && defaultDispay != "") {
                                    options.unshift({
                                        id: 0,
                                        text: defaultDispay
                                    })
                                }
                                //$(dom).select2('destroy');
                                $(dom).select2({
                                    dropdownCssClass: "dynamic-select2", allowClear: false, width: "100%",
                                    data: options
                                });

                                var defaultValue = dom.getAttribute("data-default-select");
                                if (GLOBAL.CurrentUser && (domAttribute === 'tinhs' || domAttribute === 'huyens' || domAttribute === 'xas') && dom.getAttribute("data-allow-default-select") === "1") {
                                    if (domAttribute === 'tinhs' && GLOBAL.CurrentUser.Tinh && GLOBAL.CurrentUser.Tinh.tinhId > 0) {
                                        defaultValue = GLOBAL.CurrentUser.Tinh.tinhId;
                                    }
                                    else if (domAttribute === 'huyens' && GLOBAL.CurrentUser.Huyen && GLOBAL.CurrentUser.Huyen.huyenId > 0) {
                                        defaultValue = GLOBAL.CurrentUser.Huyen.huyenId;
                                    }
                                }
                                if (defaultValue) {
                                    $(dom).val(defaultValue).trigger('change');
                                }
                                else {
                                    $(dom).trigger('change');
                                }
                            }
                            else {
                                countInterval++;

                                if (countInterval === maxInterval) {
                                    clearInterval(interval);

                                    console.log("DanhMuc colection could't load");
                                }
                            }
                        }, 100);

                    }
                },
                bindCbbDonViHanhChinhTraCuu: function (ddlTinhThanh, ddlQuanHuyen, ddlPhuongXa, tinhId, huyenId, xaId, bindDVHCByUser) {
                    if (!ddlTinhThanh || !ddlQuanHuyen || !ddlPhuongXa) return;

                    let maTinh = tinhId;
                    let maHuyen = huyenId;
                    let maXa = xaId;

                    if (bindDVHCByUser) {
                        maTinh = GLOBAL.CurrentUser.TinhId || 0;
                        maHuyen = GLOBAL.CurrentUser.HuyenId || 0;
                        maXa = GLOBAL.CurrentUser.XaId || 0;
                    }

                    ddlTinhThanh.on("change", function () {
                        let tinhId = this.value;
                        if (tinhId) {
                            if (bindDVHCByUser) {
                                if (GLOBAL.CurrentUser.TinhId != 0 && GLOBAL.CurrentUser.TinhId != tinhId) {
                                    $.confirm({
                                        title: 'CẢNH BÁO!',
                                        content: 'Bạn không có quyền truy cập thông tin của đơn vị hành chính này',
                                        theme: 'modern',
                                        icon: 'fa fa-exclamation-circle',
                                        type: 'orange',
                                        buttons: {
                                            ok: {
                                                text: 'Đóng',
                                                btnClass: 'btn-default',
                                            }
                                        }
                                    });
                                    return;
                                }
                            }

                            if (maHuyen > 0) {
                                let vmHuyen = DanhMucAjax.GetDistrictById(maHuyen.toString());
                                if (vmHuyen && vmHuyen.huyenId) {
                                    ddlQuanHuyen.attr('disabled', true);
                                    ddlQuanHuyen.html('');
                                    ddlQuanHuyen.append(`<option value="${vmHuyen.huyenId}" selected>${vmHuyen.ten}</option>`);
                                    ddlQuanHuyen.trigger('change');
                                } else {
                                    Utils.Msg.show({
                                        msg: 'Lấy danh sách quận/huyện không thành công',
                                        icon: Utils.Msg.UNSUCCESS,
                                        buttons: Utils.Msg.OK
                                    });
                                }
                            }
                            else {
                                DanhMucAjax.GetDistrictByProvinceId(tinhId, function (data) {
                                    if (data && data.value) {
                                        let options = data.value.map(t => `<option value="${t.huyenId}">${t.ten}</option>`);

                                        options.unshift('<option value="">-- Chọn quận/huyện --</option>');

                                        ddlQuanHuyen.html('');
                                        ddlQuanHuyen.append(options);

                                        if (bindDVHCByUser && huyenId > 0) {
                                            ddlQuanHuyen.val(huyenId).change();
                                        }
                                    }
                                    else {
                                        Utils.Msg.show({
                                            msg: 'Lấy danh sách quận/huyện không thành công',
                                            icon: Utils.Msg.UNSUCCESS,
                                            buttons: Utils.Msg.OK
                                        });
                                    }
                                });
                            }
                        } else {
                            ddlQuanHuyen.html('');
                            ddlQuanHuyen.append(`<option value="">-- Chọn quận/huyện --</option>`);
                            ddlQuanHuyen.trigger('change');
                        }
                    });

                    ddlQuanHuyen.on("change", function () {
                        let huyenId = this.value;
                        if (huyenId) {
                            if (bindDVHCByUser) {
                                if (GLOBAL.CurrentUser.HuyenId != 0 && GLOBAL.CurrentUser.HuyenId != huyenId) {
                                    $.confirm({
                                        title: 'CẢNH BÁO!',
                                        content: 'Bạn không có quyền truy cập thông tin của đơn vị hành chính này',
                                        theme: 'modern',
                                        icon: 'fa fa-exclamation-circle',
                                        type: 'orange',
                                        buttons: {
                                            ok: {
                                                text: 'Đóng',
                                                btnClass: 'btn-default',
                                            }
                                        }
                                    });
                                    return;
                                }
                            }

                            if (maXa > 0) {
                                let vmXa = DanhMucAjax.GetWardById(maXa.toString());
                                if (vmXa && vmXa.xaId) {
                                    ddlPhuongXa.attr('disabled', true);
                                    ddlPhuongXa.html('');
                                    ddlPhuongXa.append(`<option value="${vmXa.xaId}" selected>${vmXa.ten}</option>`);
                                    ddlPhuongXa.trigger('change');
                                } else {
                                    Utils.Msg.show({
                                        msg: 'Lấy danh sách phường/xã không thành công',
                                        icon: Utils.Msg.UNSUCCESS,
                                        buttons: Utils.Msg.OK
                                    });
                                }
                            }
                            else {
                                DanhMucAjax.GetWardByDistrictId(huyenId, function (data) {
                                    if (data && data.value) {
                                        let options = data.value.map(t => `<option value="${t.xaId}">${t.ten}</option>`);

                                        options.unshift('<option value="">-- Chọn phường/xã --</option>');

                                        ddlPhuongXa.html('');
                                        ddlPhuongXa.append(options);

                                        if (bindDVHCByUser && xaId > 0) {
                                            ddlPhuongXa.val(xaId).trigger('change');
                                        }
                                    }
                                    else {
                                        Utils.Msg.show({
                                            msg: 'Lấy danh sách phường/xã không thành công',
                                            icon: Utils.Msg.UNSUCCESS,
                                            buttons: Utils.Msg.OK
                                        });
                                    }
                                });
                            }
                        } else {
                            ddlPhuongXa.html('');
                            ddlPhuongXa.append(`<option value="">-- Chọn phường/xã --</option>`);
                        }
                    });

                    if (bindDVHCByUser) {
                        if (maTinh && (!GLOBAL.CurrentUser.TinhId || GLOBAL.CurrentUser.TinhId != maTinh)) {
                            $.confirm({
                                title: 'CẢNH BÁO!',
                                content: 'Bạn không có quyền truy cập thông tin của đơn vị hành chính này',
                                theme: 'modern',
                                icon: 'fa fa-exclamation-circle',
                                type: 'orange',
                                buttons: {
                                    ok: {
                                        text: 'Đóng',
                                        btnClass: 'btn-default',
                                    }
                                }
                            });
                            return;
                        }
                    }

                    if (maTinh && maTinh > 0) {
                        let vmTinh = DanhMucAjax.GetProvinceById(maTinh.toString());
                        if (vmTinh && vmTinh.tinhId) {
                            ddlTinhThanh.attr('disabled', true);
                            ddlTinhThanh.html('');
                            ddlTinhThanh.append(`<option value="${vmTinh.tinhId}" selected>${vmTinh.ten}</option>`);
                            ddlTinhThanh.trigger('change');
                        } else {
                            Utils.Msg.show({
                                msg: 'Lấy danh sách tỉnh/thành phố không thành công',
                                icon: Utils.Msg.UNSUCCESS,
                                buttons: Utils.Msg.OK
                            });
                        }
                    }
                    else {
                        DanhMucAjax.GetAllProvince(function (data) {
                            if (data && data.value) {
                                let options = data.value.map(t => `<option value="${t.tinhId}">${t.ten}</option>`);

                                options.unshift('<option value="">-- Chọn tỉnh/thành phố --</option>');

                                ddlTinhThanh.html('');
                                ddlTinhThanh.append(options);

                                if (bindDVHCByUser && tinhId > 0) {
                                    ddlTinhThanh.val(tinhId).trigger('change');
                                }
                            }
                            else {
                                Utils.Msg.show({
                                    msg: 'Lấy danh sách tỉnh/thành phố không thành công',
                                    icon: Utils.Msg.UNSUCCESS,
                                    buttons: Utils.Msg.OK
                                });
                            }
                        });
                    }
                }
            }
        },
        Ajax: {
            callAjax: function (requestURL, type, data, callback, showLoadingMask) {
                let header = {
                    '__RequestVerificationToken': $('input[name="__RequestVerificationToken"]:eq(0)').val(),
                    'X-Requested-With': "XMLHttpRequest"
                };

                $.ajax({
                    url: requestURL,
                    type: type,
                    global: showLoadingMask === false ? showLoadingMask : true,
                    headers: header,
                    data: data,
                    success: function (data) {
                        Utils.Object.tryParseDateInObject(data);

                        callback && callback(data);
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        console.log("Lỗi", jqXHR, textStatus, errorThrown);

                        let data = {
                            success: false,
                            errorMessage: "Gửi yêu cầu không thành công"
                        }

                        callback && callback(data);
                    }
                });
            },
            post: function (requestURL, data, callback, showLoadingMask) {
                this.callAjax(requestURL, "POST", data, callback, showLoadingMask);
            },
            postFormData: function (requestURL, formData, callback, showLoadingMask) {
                let header = {
                    '__RequestVerificationToken': $('input[name="__RequestVerificationToken"]:eq(0)').val(),
                    'X-Requested-With': "XMLHttpRequest"
                };

                $.ajax({
                    url: requestURL,
                    type: 'POST',
                    contentType: false,
                    processData: false,
                    headers: header,
                    global: showLoadingMask === false ? showLoadingMask : true,
                    data: formData,
                    success: function (data) {
                        Utils.Object.tryParseDateInObject(data);

                        callback && callback(data);
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        console.log("Lỗi", jqXHR, textStatus, errorThrown);

                        let data = {
                            success: false,
                            errorMessage: "Gửi yêu cầu không thành công"
                        }

                        callback && callback(data);
                    }
                });

            }
        },
        Object: {
            tryParseDateInObject: function (obj) {
                if (typeof obj === "object") {
                    for (var prop in obj) {
                        obj[prop] = Utils.Object.tryParseDateInObject(obj[prop]);
                    }
                    return obj;
                } else {
                    return Utils.DateTime.Parser.fromDateTimeString(obj);
                }
            },
            cloneObject: function (obj) {
                let cloneObject = $.extend(true, $.isArray(obj) ? [] : {}, obj);

                return Utils.Object.tryParseDateInObject(cloneObject);
            }
        },
        String: {
            isJson: function (item) {
                item = typeof item !== "string"
                    ? JSON.stringify(item)
                    : item;
                try {
                    item = JSON.parse(item);
                } catch (e) {
                    return false;
                }
                if (typeof item === "object" && item !== null) {
                    return true;
                }
                return false;
            },
            firstCharToUpper: function (value) {
                if (!value) return "";

                return value.split(" ").map(t => t.charAt(0).toUpperCase() + t.substring(1).toLowerCase()).join(" ");
            }
        },
        Guid: {
            new: function () {
                return (S4() + S4() + "-" + S4() + "-4" + S4().substr(0, 3) + "-" + S4() + "-" + S4() + S4() + S4()).toLowerCase();
            }
        },
        LocalStorage: {
            checkLocalExist: function (collectionName) {
                if (localDB && localDB.getCollection(collectionName) === null) {
                    return false;
                } else {
                    return true;
                }
            },
            saveToLocal: function (collectionName, docData) {
                var collection = localDB.getCollection(collectionName);
                if (collection === null) {
                    collection = localDB.addCollection(collectionName);
                }
                collection.clear({ removeIndices: true });
                var doc = collection.insert(docData);
                try {
                    collection.update(doc);
                } catch (err) {
                    Utils.Alertify.warning("tl", err);
                }
            },
            getData: function () {
                return VBDLISLocalDB.getData();
            }
        }
    };

    window.Utils = Utils;
})(jQuery);