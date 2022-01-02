function openDialog(stitle, Content, callback, callbackNo) {
    var Status = {};
    Status.Step1 = {
        title: stitle,
        html: Content,
        buttons: { 確定: "yes", 取消: "on" },
        position: { width: 550 },
        focus: 1,
        submit: function (a, v, m, f) {
            if (v == 'yes') if (callback) callback();
            if (v == 'on') if (callbackNo) callbackNo();
            return true;
        }
    }
    $.prompt(Status);
}


$.fn.JcheckboxListTODialog = function () {
    var $that = $(this),
        _thisID = $(this).attr('ID');
    var _BtnTitle = $(this).attr('data-BtnTitle') || "請選擇";
    var _Title = $(this).attr('data-Title') || "請選擇";

    kernel_UI_init($that); //初始化 ui

    $that.find('li').find('input[type="checkbox"]').each(function () { //如果原本 li chanage 時候
        $(this).change(function () {
            kernel_UI_init($that);
        })
    })
    //把button 產出
    var $button = $("<input type='button' class='Button_add_2' value='" + _BtnTitle + "' ></b>");
    $that.before($button);

    var Subs = $('[data-JType="JSelectTODialog"]').data('sublists') || '';
    if (Subs != '') {
        Subs = JSON.parse(Subs.replace(/'/g, '"'));
    }

    //打開 dialog 的時候
    $button.click(function () {
        var islevel2Class = ''; //是否為多階層Class
        if (Subs != '') {
            islevel2Class = 'Class="HasSub"';
        }
        var html = $('#' + _thisID).html();
        openDialog(_Title, "<ul id='Dialog_ul' " + islevel2Class + " >" + html + "</ul>", null, null);
        $('#Dialog_ul').find('li').each(function () {
            var $li = $(this),
                $input = $(this).find('input'),
                $label = $(this).find('label'),
                inputID = $input.attr("id"),
                forID = $label.attr("for");

            $(this).show(); //打開所有項目
            $input.show(); //開啟選擇項
            $input.attr("id", inputID + "_Temp");
            $label.attr("for", inputID + "_Temp");

            //把子類別加進去html
            if (Subs != '') {
                var SubListstring = (Subs['s' + $input.val()] || '').split(',');
                var $SubUL = $('<ul class="subC"></ul>')
                $.each(SubListstring, function (number) {
                    $SubUL.append('<li>' + SubListstring[number] + '</li>');
                });
                $li.append($SubUL);
            }


            $input.click(function () {
                $('#' + inputID).prop("disabled", false); //啟用項目          
                $('#' + inputID).click();
            })
        })
    })

    //初始化li
    function kernel_UI_init() {
        $that.find('li').find('input[type="checkbox"]').each(function () {
            var $input = $(this), //原本UI input
                $li = $(this).parent(), //原本UI LI
                $label = $li.find('label'); //原本label
            $input.hide(); //把原本 input 隱藏
            $label.attr('for', '');
            if ($input.attr('checked')) {
                $input.attr("checked", true);
                $li.show();
            }
            else {
                $input.attr("checked", false);
                $li.hide();
            }
        })
    }
}

$(function () {
    $('[data-JType="JSelectTODialog"]').JcheckboxListTODialog();
})

$.fn.CCMS_PageReload = function () {
    window.location = window.location;
}

$.fn.CCMS_OpenDialog = function (url, iFrameName, width, height) {
    var _iFrameName = iFrameName || 'CCMS_iFrameName';
    var iwidth = width || screen.width - (screen.width * 0.4);
    if (width == 0) {
        iwidth = '100%';
    }
    var windowheight = $(window).height();
    var iheight = windowheight - (windowheight * 0.2);
    if (height != 0)
        iheight = height + 'px';
    var _buttons = { 儲存: "btnPost", 列印: "btnHtml2PD", 派案: "btnSetCase", 分案: "btnSplit", 送審: "btnVerify", 修改: "btnEdit", 確定: "btnOK", 發送: "btnSend", 刪除: "btnDraft", 取消: "btnCancel" };
    var _buttonsfunction = function (a, v, m, f) {
        if (v != 'btnCancel') {
            $('#' + _iFrameName).contents().find('input[type="submit"][id$="' + v + '"]').click();
        } else {
            return true;
        }
        return false;
    }
    var _iframe = '<iframe class="popiframe" style="border-width:0px;" id="' + _iFrameName + '" src="' + url + '" width="99%" height="' + iheight + '"></iframe>';

    var Status = {}; Status.Step1 = {
        title: ' ', html: _iframe, buttons: _buttons, position: { width: iwidth }, focus: 2,
        submit: _buttonsfunction
    };

    var CCMS_Impromptu = $.prompt(Status);
    $('.jqibuttons ').hide();
    $('#' + _iFrameName).on("load", function () {
        $('.jqibuttons ').show();
        $(this).contents().find('input[id="btnCancel"]').hide();

        //標題置換
        var Title = $(this).contents().find('[id^="lblTitleName"]').html();
        $(this).contents().find('[id^="lblTitleName"]').hide();
        $('.jqititle').html('<h4><img src="/images/popup_info.png" />' + Title + '</h4>')
        $('.jqibutton').show();

        var stemp = [];
        for (var key in _buttons) { stemp.push(new button(_buttons[key], key)); }
        var btns = $(this).contents().find('input[type="submit"]');
        btns.each(function () {
            var ID = $(this).attr("ID") || '';
            var value = $(this).val();
            for (var index in stemp) {
                if (ID.indexOf("_" + stemp[index].key) > -1 && stemp[index].value == value) {
                    $(this).hide();
                    if (index > -1) { stemp.splice(index, 1); }
                }
            }
        })
        for (var index in stemp) {
            var value = stemp[index].key;
            if (value == "btnCancel") return;
            $('.jqibutton[value="' + value + '"]').each(function () {
                if ($(this).html() == stemp[index].value) $(this).hide();
            })
        }

    });
    function button(k, v) {
        this.key = k;
        this.value = v;
    }
}

//.ccms-datetime
$(function () {
    $('[data-ccms_datepicker]').each(function () {
        var $that = $(this).ccms_datepicker();
    })
})
$.fn.ccms_datepicker = function () {
    var $obj = JSON.parse($(this).data('ccms_datepicker').replace(/\'/g, '"'));
    var IsShowTime = $obj.IsShowTime || false;
    var IsShowClearButton = $obj.IsShowClearButton || true;
    var IsRequired = $obj.IsRequired || false;
    var Setting = {
        language: 'zh',
        autoClose: true,
        clearButton: IsShowClearButton,
        timepicker: IsShowTime,
        firstDay: 0,
        navTitles: { days: '<i>民國 rrr 年</i>  MM', months: '民國 rrr 年', years: '民國 rrr1 至 rrr2 年' }
    };



    var datepicker = $(this).datepicker(Setting)
    $(this).addClass('Wdate');
    $(this).prop('required', IsRequired);
    $(this).css("width", "130px");

    $(this).keydown(function (e) {
        e.preventDefault();
    });

    try {
        $(this).val($(this).val().replace('T', ' '));
        var s = $(this).val().split(/[-: ]/);
        console.log(s);
        if (s.length >= 5) {
            var date = new Date(s[0], s[1] - 1, s[2], s[3], s[4], 0);
            var myDatepicker = $(this).datepicker().data('datepicker');
            console.log(date);
            myDatepicker.selectDate(date);
        } else if (s.length == 3) {
            var date = new Date(s[0], s[1] - 1, s[2], 0, 0, 0);
            date.setFullYear(1911 + parseInt(s[0]));
            var myDatepicker = $(this).datepicker().data('datepicker');
            console.log(date);
            myDatepicker.selectDate(date);
        } else {
        }
    } catch (e) {
        console.log("error" + e);
    }

};
//.data-deptAndAccountSelect
$(function () {
    $('[data-deptandaccountselect]').each(function () {
        var $that = $(this).deptandaccountselect();
    })
})

$.fn.deptandaccountselect = function () {
    var $that = $(this);
    $that.hide();
    var $obj = JSON.parse($that.data('deptandaccountselect').replace(/\'/g, '"'));
    var IsRequired = $obj.IsRequired || false;

    var ddl1 = $('<select></select>');
    var ddl2 = $('<select></select>');
    var ddl3 = $('<select></select>');
    $that.after(ddl3);
    $that.after(ddl2);
    $that.after(ddl1);
    ddl3.prop('required', IsRequired);

    var _Bindddl = function (ddl, ParentID, func) {
        ddl.find('option').each(function () {
            $(this).remove();
        })
        ddl.append("<option value=''>請選擇</option>");

        var _CallBack = function (Datas) {
            if (Datas.length == 0) {
                ddl.prop('disabled', true);
            } else {
                ddl.prop('disabled', false);
            }
            $.each(Datas, function (k, v) {
                var Name = '';
                if ('Name' in v) Name = v.Name;
                if ('ChineseFullName' in v) Name = v.ChineseFullName;
                ddl.append("<option value='" + v.ID + "'>" + Name + "</option>")
            });
        }


        func(ParentID, _CallBack);
    }
    var _GetDept = function (ParentID, CallBack) {
        $.ajax({
            type: "get",
            async: false,
            url: "/common/WebAPI/GetDept.ashx?ParentID=" + ParentID,
            dataType: "json",
            success: function (res) {
                CallBack(res);
            }
        });
    }
    var _GetAccount = function (DepartmentID, CallBack) {
        $.ajax({
            type: "get",
            async: false,
            url: "/common/WebAPI/GetAccount.ashx?DepartmentID=" + DepartmentID,
            dataType: "json",
            success: function (res) {
                CallBack(res);
            }
        });
    }
    var _Set_ddl1 = function (value) {
        ddl1.val(value);
        ddl1.change();
    }

    var _Set_ddl2 = function (value) {
        ddl2.val(value);
        ddl2.change();
    }
    var _Set_ddl3 = function (value) {
        ddl3.val(value);
    }

    _Bindddl(ddl1, "0", _GetDept);
    ddl1.change(function () {
        _Bindddl(ddl2, ddl1.val(), _GetDept);
        ddl2.change();
    })
    ddl2.change(function () {
        if (ddl2.val() != '')
            _Bindddl(ddl3, ddl2.val(), _GetAccount);
        else
            _Bindddl(ddl3, ddl1.val(), _GetAccount);
        console.log("ddl2 change");
    })
    ddl3.change(function () {
        $that.val(ddl3.val());
    })

    if ($that.val() != '') {
        $.ajax({
            type: "get",
            async: false,
            url: "/common/WebAPI/GetAccount.ashx?AccountID=" + $that.val(),
            dataType: "json",
            success: function (res) {
                if (res.Dept_L1 != '') {
                    _Set_ddl1(res.Dept_L1);
                }
                if (res.Dept_L2 != '') {
                    _Set_ddl2(res.Dept_L2);
                }
                if (res.Account != '') {
                    _Set_ddl3(res.Account);
                }
            }
        });
    }
};