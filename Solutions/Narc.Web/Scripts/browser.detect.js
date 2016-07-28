$(function() {
    var bro = browser();
    if (bro.Browser === "IE" && parseInt(bro.Version, 10) === 6) {
        window.location.href = "/NotSupported";
        return false;
    }    
    
    if (bro.Browser === "IE" && parseInt(bro.Version, 10) === 7) {
        testBrowser();
    }
    
    if (bro.Browser === "IE" && parseInt(bro.Version, 10) === 8) {
        testBrowser();
    }
});

var tpl = { BroDlgContent: '<div class="tips-c"><p id="xwb_msgdlg_ct"></p></div>' };
var win_tpl = $.extend(Xwb.Tpl, tpl);

function page(title, text) {
    var panel = "<div>" + title + "</div>"
        + "<div> " + text + "</div><br/>"
        + "<div>下次不再显示：<input type=\"checkbox\" name =\"IsShowBrowserInfo\" id=\"IsShowBrowserInfo\" /></div><br/>"
        + "<div>推荐使用以下浏览器：</div><br/>"
        + "<div class='browser'>"
        + "<ul>"
        + "<li><a class='chrome' href='https://www.google.com/chrome/' target='_blank'></a></li>"
        + "<li><a class='firefox' href='http://firefox.com.cn/download/' target='_blank'></a></li>"
        + "<li><a class='ie9' href='http://windows.microsoft.com/en-US/internet-explorer/downloads/ie/' target='_blank'></a></li>"
        + "<li><a class='safari' href='http://www.apple.com/safari/download/' target='_blank'></a></li>"
        + "<li><a class='opera' href='http://www.opera.com/download/' target='_blank'></a></li>"
        + "<ul>"
        + "</div>";

    return panel;
}

function browser() {
    $.browser.chrome = /chrome/.test(navigator.userAgent.toLowerCase());

    if ($.browser.chrome) {
        return { Browser: "Chrome", Version: $.browser.version };
    }

    if ($.browser.msie) {
        return { Browser: "IE", Version: $.browser.version };
    }

    if ($.browser.mozilla) {
        return { Browser: "Mozilla Firfox", Version: $.browser.version };
    }

    if ($.browser.safari) {
        return { Browser: "Safari", Version: $.browser.version };
    }

    if ($.browser.opera) {
        return { Browser: "Opera", Version: $.browser.version };
    }
    return "";
}

function testBrowser() {
    var bro = browser();
    var isShow = $.cookie("IsShowBrowserInfo");
    
    if (isShow == null || isShow == "true") {
        var mb = Xwb.use("box");
        var html = page("您当前使用的浏览器为：" + bro.Browser + " " + bro.Version, "");
        mb.setContent(html);
        mb.display(true);
    }
}

Xwb.ui.box = Xwb.reg('box', function () {
    var inst = Xwb.use('Dlg', {
        cs: 'win-wtm-browser',
        appendTo: document.body,
        autoCenter: true,
        closeable: true,
        dlgContentHtml: win_tpl.BroDlgContent,
        title: '温馨提示',
        buttons: [{ title: '确认', id: 'ok' }, { title: '取消', id: 'cancel' }],
        defBtn: 'ok',
        mask: true,
        destroyOnClose: true,
        onViewReady: function (v) {
            this.fly(this.getButton('close').get(0)).display(false);
            this.setIcon('ask');
        },
        setIcon: function (d) {
        },
        setContent: function (a) {
            this.jq("#xwb_msgdlg_ct").html(a);
        },
        onbuttonclick: function (bid) {
            if (bid == 'ok') {

                if ($("#IsShowBrowserInfo").val()) {
                    $.cookie("IsShowBrowserInfo", false, { expires: 7 });
                }
            }
            return true;
        }
    });
    return inst;
});