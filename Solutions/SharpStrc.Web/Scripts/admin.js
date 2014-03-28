(function ($, window, undefined) {
    'use strict';

    $.fn.overlayMask = function (action) {
        var mask = this.find('.overlay-mask');

        if (!mask.length) {
            this.css({
                position: 'relative'
            });
            mask = $('<div class="overlay-mask"><img src="/images/loader.gif" /></div>');
            mask.css({
                position: 'absolute',
                width: '100%',
                height: '100%',
                top: '0px',
                left: '0px',
                zIndex: 100
            }).appendTo(this);
        }

        if (!action || action === 'show') {
            mask.show();
        } else if (action === 'hide') {
            mask.hide();
        }

        return this;
    };

    $.format = (function () {

        var UNDEFINED = 'undefined',
            TRUE = true,
            FALSE = false;

        function isInt(value) {
            if ((parseFloat(value) == parseInt(value)) && !isNaN(value)) {
                return true;
            } else {
                return false;
            }
        }

        return {
            customNumber: function (value, format) {
                //00-00-00 
                if (typeof value == 'string') {
                    return value;
                } else if (isInt(value)) {
                    value = Math.abs(value);
                    var strValue = value.toString();

                    var fNum = format.replace(new RegExp('[^0]', 'g'), '').length;
                    var vNum = strValue.length;

                    if (fNum != vNum) {
                        return 'NaN';
                    }

                    var ret = new Array();
                    var vA = strValue.split('');

                    var j = format.length - 1;
                    for (; j >= 0; j--) {

                        if (format.charAt(j) == '0') {
                            ret.push(vA.pop());
                        } else {
                            ret.push(format.charAt(j));
                        }
                    }

                    return ret.reverse().join('');
                } else {
                    return 'NaN';
                }
            }
        };
    })();

    /*
        * debouncedresize: special jQuery event that happens once after a window resize
        *
        * latest version and complete README available on Github:
        * https://github.com/louisremi/jquery-smartresize/blob/master/jquery.debouncedresize.js
        *
        * Copyright 2011 @louis_remi
        * Licensed under the MIT license.
        */
    var $event = $.event,
	$special,
	resizeTimeout;

    $special = $event.special.debouncedresize = {
        setup: function () {
            $(this).on("resize", $special.handler);
        },
        teardown: function () {
            $(this).off("resize", $special.handler);
        },
        handler: function (event, execAsap) {
            // Save the context
            var context = this,
				args = arguments,
				dispatch = function () {
				    // set correct event type
				    event.type = "debouncedresize";
				    $event.dispatch.apply(context, args);
				};

            if (resizeTimeout) {
                clearTimeout(resizeTimeout);
            }

            execAsap ?
				dispatch() :
				resizeTimeout = setTimeout(dispatch, $special.threshold);
        },
        threshold: 150
    };

    // ======================= imagesLoaded Plugin ===============================
    // https://github.com/desandro/imagesloaded

    // $('#my-container').imagesLoaded(myFunction)
    // execute a callback when all images have loaded.
    // needed because .load() doesn't work on cached images

    // callback function gets image collection as argument
    //  this is the container

    // original: mit license. paul irish. 2010.
    // contributors: Oren Solomianik, David DeSandro, Yiannis Chatzikonstantinou

    // blank image data-uri bypasses webkit log warning (thx doug jones)
    var BLANK = 'data:image/gif;base64,R0lGODlhAQABAIAAAAAAAP///ywAAAAAAQABAAACAUwAOw==';

    $.fn.imagesLoaded = function (callback) {
        var $this = this,
			deferred = $.isFunction($.Deferred) ? $.Deferred() : 0,
			hasNotify = $.isFunction(deferred.notify),
			$images = $this.find('img').add($this.filter('img')),
			loaded = [],
			proper = [],
			broken = [];

        // Register deferred callbacks
        if ($.isPlainObject(callback)) {
            $.each(callback, function (key, value) {
                if (key === 'callback') {
                    callback = value;
                } else if (deferred) {
                    deferred[key](value);
                }
            });
        }

        function doneLoading() {
            var $proper = $(proper),
				$broken = $(broken);

            if (deferred) {
                if (broken.length) {
                    deferred.reject($images, $proper, $broken);
                } else {
                    deferred.resolve($images);
                }
            }

            if ($.isFunction(callback)) {
                callback.call($this, $images, $proper, $broken);
            }
        }

        function imgLoaded(img, isBroken) {
            // don't proceed if BLANK image, or image is already loaded
            if (img.src === BLANK || $.inArray(img, loaded) !== -1) {
                return;
            }

            // store element in loaded images array
            loaded.push(img);

            // keep track of broken and properly loaded images
            if (isBroken) {
                broken.push(img);
            } else {
                proper.push(img);
            }

            // cache image and its state for future calls
            $.data(img, 'imagesLoaded', { isBroken: isBroken, src: img.src });

            // trigger deferred progress method if present
            if (hasNotify) {
                deferred.notifyWith($(img), [isBroken, $images, $(proper), $(broken)]);
            }

            // call doneLoading and clean listeners if all images are loaded
            if ($images.length === loaded.length) {
                setTimeout(doneLoading);
                $images.unbind('.imagesLoaded');
            }
        }

        // if no images, trigger immediately
        if (!$images.length) {
            doneLoading();
        } else {
            $images.bind('load.imagesLoaded error.imagesLoaded', function (event) {
                // trigger imgLoaded
                imgLoaded(event.target, event.type === 'error');
            }).each(function (i, el) {
                var src = el.src;

                // find out if this image has been already checked for status
                // if it was, and src has not changed, call imgLoaded on it
                var cached = $.data(el, 'imagesLoaded');
                if (cached && cached.src === src) {
                    imgLoaded(el, cached.isBroken);
                    return;
                }

                // if complete is true and browser supports natural sizes, try
                // to check for image status manually
                if (el.complete && el.naturalWidth !== undefined) {
                    imgLoaded(el, el.naturalWidth === 0 || el.naturalHeight === 0);
                    return;
                }

                // cached images don't fire load sometimes, so we reset src, but only when
                // dealing with IE, or image is complete (loaded) and failed manual check
                // webkit hack from http://groups.google.com/group/jquery-dev/browse_thread/thread/eee6ab7b2da50e1f
                if (el.readyState || el.complete) {
                    el.src = BLANK;
                    el.src = src;
                }
            });
        }

        return deferred ? deferred.promise($this) : $this;
    };
})(jQuery, window);

(function (d) {
    window.__debug || (__debug = !1);
    var b = RegExp("(?:^\\s*)|(?:\\s*$)", "g"),
        e = location.hostname,
        j = /:\/\/(.[^\/]+)/,
        g = {
            util: {
                templ: function (a, d, b, c) {
                    return a.replace(/\{([\w_$]+)\}/g, function (a, h) {
                        var e = d[h];
                        c && typeof e === "string" && (e = argument.callee(e, d, b, c));
                        return e === void 0 || e === null ? "" : b ? encodeURIComponent(e) : e;
                    });
                },
                extend: function (a, d, b) {
                    a || (a = {});
                    if (d) for (var c in d) if (a[c] === void 0 || b) a[c] = d[c];
                    return a;
                },
                trim: function (a) {
                    return a.replace(b, "");
                },
                bind: function (a, d) {
                    return function () {
                        return a.apply(d, arguments);
                    };
                },
                init: function (a) {
                    this.basePath = a;
                    return this;
                },
                basePath: "/"
            }
        };
    var c = g.util;
    if (!d) d = window.Xwb = {};
    d.request = g;
})(window.Xwb);

Xwb.Tpl = {
    Box: '<div class="win-pop {.cs}"><div class="win-con round4">[?.title?{BoxTitlebar}?]<div class="win-box" id="xwb_dlg_ct">{.contentHtml}</div></div>[?.closeable?<a href="#" class="icon-close-btn icon-bg" id="xwb_cls" title="\u5173\u95ed"></a>?]{.boxOutterHtml}</div>',
    DialogContent: '{.dlgContentHtml}<div class="btn-area" id="xwb_dlg_btns">{.buttonHtml}</div>',
    CustomDlgContent: '<div id="xwb_cusdlg_ct"></div>',
    BoxTitlebar: '<h4 class="win-tit x-bg"><span id="xwb_title">{.title}</span></h4>',
    Mask: '<div class="shade-div"></div>',
    MsgDlgContent: '<div class="tips-c"><div class="icon-alert all-bg" id="xwb_msgdlg_icon"></div><p id="xwb_msgdlg_ct"></p></div>',
    Button: '<button  type="button" class="buttonS bDefault  {.cs}" id="xwb_btn_{.id}">{.title}</button>',
    AnchorTipContent: '<div class="tips-c"><div class="icon-correct icon-bg"></div><p id="xwb_title"></p></div>',
    AnchorDlgContent: '<div class="tips-c"><div class="icon-warn icon-bg"></div><p id="xwb_title"></p></div>',
    SpanBoxContent: '<div class="win-box-inner"><p class="desc">\u4e0d\u826f\u4fe1\u606f\u662f\u6307\u542b\u6709\u8272\u60c5\u3001\u66b4\u529b\u3001\u5e7f\u544a\u6216\u5176\u4ed6\u9a9a\u6270\u4f60\u6b63\u5e38\u5fae\u535a\u751f\u6d3b\u7684\u5185\u5bb9\u3002</p><p>\u4f60\u8981\u4e3e\u62a5\u7684\u662f\u201c{.nick}\u201d\u53d1\u7684\u5fae\u535a\uff1a</p><div class="report-box"><div>{.text}</div><img src="{.img}" class="user" width="30px" height="30px" ></div><p>\u4f60\u53ef\u4ee5\u586b\u5199\u66f4\u591a\u4e3e\u62a5\u8bf4\u660e\uff1a\uff08\u9009\u586b\uff09</p><p><textarea rows="" cols="" id="content"></textarea></p><div class="foot-con"><p>\u8bf7\u653e\u5fc3\uff0c\u4f60\u7684\u9690\u79c1\u5c06\u4f1a\u5f97\u5230\u4fdd\u62a4\u3002<br>\u4e3e\u62a5\u7535\u8bdd\uff1a400 690 0000</p><div class="btn-area"><a href="#" class="general-btn highlight" rel="e:ok"><span>\u786e\u8ba4\u4e3e\u62a5</span></a><a href="#" class="general-btn" rel="e:cancel"><span>\u53d6\u6d88</span></a></div></div></div>'
};
(function (c, d) {
    function e(a, d, b) {
        var c, k = [],
            h = [];
        b || (b = "\\");
        for (var u = 0, e = a.length; u < e; u++) c = a.charAt(u), c === d ? (k.push(h.join("")), h.length = 0) : (c === b && a.charAt(u + 1) === d && (c = d, u++), h[h.length] = c);
        h.length && k.push(h.join(""));
        return k;
    }

    function j(a) {
        var d = {}, b, c = e(a, ",");
        try {
            for (var k = 0, u = c.length; k < u; k++) c[k].indexOf(":") === -1 ? d[c[k]] = h : (b = e(c[k], ":"), d[b[0]] = b[1])
        } catch (g) {
            throw __debug && console.trace(), "Syntax " +
                "Error:rel\u5b57\u7b26\u4e32\u683c\u5f0f\u51fa\u9519\u3002" + a;
        }
        return d
    }

    function g(a) {
        this.q = a;
        this.idx = -1;
        this.end = this.q.length - 1;
    }

    function a(a) {
        this.mgr = a;
        this.nextChain = q.bind(this.doNextChain, this)
    }

    function b() {
        return this.view
    }

    var h = !0,
        l = document,
        p = 10001,
        n = c.Tpl,
        s = c.request,
        q = c.util = s.util;

    window.__debug = !0;

    if (window.__debug) {
        if (!window.console) window.console = {};
        (function (a, d) {
            a || (a = {});
            if (d) for (var b in d) a[b] === void 0 && (a[b] = d[b]);
            return a;
        })(console, {
            debug: d.noop,
            trace: d.noop,
            log: d.noop,
            warn: d.noop,
            error: d.noop,
            group: d.noop,
            groupEnd: d.noop
        });
    }
    d.browser.msie && d.trim(d.browser.version) == "6.0" && document.execCommand("BackgroundImageCache", !1, !0);
    var m = /\[\?\.([\w_$]+?)\?([\S\s]*?)\?\]/g,
        o = /\{(\.?[\w_$]+)\}/g;
    n.parse = function (a, d) {
        d || (d = {});
        if (a.charAt(0) !== "<") {
            var b = n[a];
            b && (a = b)
        }
        a = a.replace(m, function (a, b, c) {
            return d[b] === void 0 ? "" : c
        });
        return a.replace(o, function (a, b) {
            var c = b.charAt(0) === "." ? d[b.substr(1)] : n[b];
            return c === void 0 || c === null ? "" : c.toString().charAt(0) === "<" ? n.parse(c, d) : n[c] ? n.parse(n[c], d) : c
        })
    };
    n.forNode = function (a, b) {
        b && (a = this.parse(a, b));
        return d(a).get(0)
    };
    n.get = function (a) {
        return this[a]
    };
    var r = 0,
        k = document.selection;
    d.extend(q, {
        create: function () {
            var a = function () {
                this.init.apply(this, arguments)
            };
            if (arguments.length === 0) return a;
            var b, c, k, h = d.makeArray(arguments);
            typeof h[0] === "string" ? (k = h[0], c = h[1], h.shift()) : c = h[0];
            h.shift();
            if (c) c = c.prototype;
            if (c) b = function () {
            }, b.prototype = c, a.prototype = b = new b;
            if (k) b.type = k, q.ns(k, a);
            k = 0;
            for (var u = h.length; k < u; k++) d.extend(b, typeof h[k] === "function" ? h[k](c) : h[k]);
            return b.constructor = a
        },
        domUp: function (a, b, c) {
            for (var c = c || l.body, k = typeof b === "string"; a;) {
                if (k) {
                    if (d(a).is(b)) break;
                } else if (b(a)) break;
                a = a.parentNode;
                if (a === c) return null;
            }
            return a;
        },
        ns: function (a, d) {
            for (var b = a.split("."), c = window, k, h = 0, u = b.length - 1; h < u; h++) k = b[h], c[k] || (c[k] = {}), c = c[k];
            c[b[h]] = d
        },
        getBind: function (a, d) {
            var b = "__" + d,
                c = a[b];
            c || (c = a[b] = q.bind(a[d], a));
            return c
        },
        uniqueId: function () {
            return ++r;
        }
    });
    var u = c.Cache = {
        MAX_ITEM_SIZE: 3,
        reg: function (a, d, b) {
            this[a] ? this[a][1] = d : this[a] = [null, d];
            b !== void 0 && this.sizeFor(a, b)
        },
        get: function (a) {
            a = this[a];
            if (a === void 0) return null;
            var d = a[1],
                a = a[0];
            return a === null ? d() : a.length > 1 ? a.pop() : d ? d() : null
        },
        put: function (a, d) {
            var b = this[a];
            if (b) {
                var c = b[0];
                c || (b[0] = c = [this.MAX_ITEM_SIZE]);
                c.length - 1 >= c[0] || c.push(d)
            } else this[a] = [
                [this.MAX_ITEM_SIZE, d]
            ]
        },
        remove: function (a) {
            this[a] && delete this[a]
        },
        sizeFor: function (a, d) {
            var b = this[a];
            b || (this[a] = b = [
                []
            ]);
            b[0] || (b[0] = []);
            b[0][0] = d
        }
    };
    u.reg("div", function () {
        return l.createElement("DIV");
    });
    d.extend(c, {
        _cls: {},
        reg: function (a, d, b) {
            if (this._cls[a] !== void 0 && !b) throw __debug && console.trace(), "已定义类" + a;
            return this._cls[a] = d
        },
        use: function (a, d) {
            var b = this._cls[a];
            return b ? typeof b === "object" ? b : typeof d === "function" ? new b(d(b.prototype)) : new b(d) : null
        }
    });
    y = c.ui = {
        Base: q.create()
    }, z = y.Base;
    y.Base.prototype = {
        autoRender: !1,
        titleNode: "#xwb_title",
        init: function (a) {
            this.cacheId = "c" + q.uniqueId();
            a && d.extend(this, a);
            this.initUI();
            this.autoRender && this.getView();
        },
        initUI: d.noop,
        clsNode: "#xwb_cls",
        initClsBehave: function (a) {
            this.jq(this.clsNode).click(q.bind(this.onClsBtnClick, this));
            this.setCloseable(a)
        },
        setCloseable: function (a) {
            this.fly(this.clsNode).display(a);
            this.closeable = a;
        },
        onClsBtnClick: function () {
            this.close(h);
            return !1
        },
        close: function () {
            if (!this.onclose || this.onclose() !== !1) this.destroyOnClose ? this.destroy() : this.display(!1)
        },
        tplData: !1,
        createView: function () {
            var B;
            var a = this.view;
            typeof a === "string" ? (B = this.view = n.forNode(n[a], this.tplData || this, h), a = B) : this.view = a = l.createElement("DIV");
            return a
        },
        innerViewReady: d.noop,
        setTitle: function (a) {
            this.jq(this.titleNode).html(a);
            return this;
        },
        getView: function () {
            var a = this.view;
            if (!a || !a.tagName) a = this.createView();
            this.getView = b;
            if (this.hidden !== void 0) {
                var k = this.hidden;
                this.hidden = void 0;
                this.display(!k)
            }
            this.appendTo && (d(this.appendTo).append(a), delete this.appendTo);
            this.closeable !== void 0 && this.initClsBehave(this.closeable);
            this.innerViewReady(a);
            this.onViewReady && this.onViewReady(a);
            return a;
        },
        beforeShow: d.noop,
        afterShow: d.noop,
        beforeHide: d.noop,
        jqExtra: function (a) {
            for (var d = arguments, b = 0, c = d.length; b < c; b++) {
                var k = d[b],
                    h = this.jq("#" + k);
                h && (k = k.charAt(0).toUpperCase() + k.substring(1), this["jq" + k] = h)
            }
            return this
        },
        display: function (a) {
            var d = this.jq();
            if (a === void 0) return !d.hasClass("hidden");
            a = !a;
            if (this._flied || this.hidden !== a) if (a) {
                if (this.beforeHide() !== !1) this.hidden = a, d.addClass("hidden"), this.afterHide && this.afterHide(), this.contexted && this.setContexted(!1)
            } else this.hidden = a, d.css("visibility", "hidden").removeClass("hidden"), this.beforeShow(), d.css("visibility", ""), this.contextable && !this.contexted && this.setContexted(h), this.afterShow();
            return this;
        },
        onContextRelease: function () {
            this.display(!1);
        },
        appendAt: function (a) {
            d(a).append(this.getView());
            return this
        },
        ancestorOf: function (a, b) {
            var a = a.view || a,
                c = this.view;
            if (c.contains && !d.browser.webkit) return c.contains(a);
            else if (c.compareDocumentPosition) return !!(c.compareDocumentPosition(a) & 16);
            b === void 0 && (b = 65535);
            for (var k = a.parentNode, u = l.body; k != u && b > 0 && k !== null;) {
                if (k == c) return h;
                k = k.parentNode;
                b--;
            }
            return !1;
        },
        jq: function (a) {
            return a === void 0 ? d(this.getView()) : d(this.getView()).find(a);
        },
        fly: function (a) {
            typeof a === "string" && (a = this.jq(a));
            return z.fly(a);
        },
        unfly: function () {
            delete this.view
        },
        destroy: function () {
            this.display(!1);
            this.jq().remove()
        },
        domEvent: function (a, d, b) {
            if (a === "mousedown") {
                var c = this,
                    k = function (a) {
                        c.contexted || A.releaseAll(a);
                        d.apply(c, arguments)
                    };
                if (!this._mousedownFns) this._mousedownFns = {};
                this._mousedownFns[d] = k;
                this.jq(b).bind(a, k)
            } else this.jq(b).bind(a, d)
        },
        unDomEvent: function (a, d, b) {
            if (a === "mousedown") {
                var c = this._mousedownFns[d];
                this.jq(b).unbind(a, c);
                delete this._mousedownFns[d]
            } else this.jq(b).unbind(a, d)
        },
        setContexted: function (a) {
            this.contexted !== a && (a ? A.context(this) : A.release(this));
            return this
        },
        templ: function (a) {
            for (var d in a) this.jq(d).html(a[d]);
            return this
        },
        offset: function () {
            return arguments.length ? (this.jq().css(arguments[0]), this) : this.jq().offset()
        },
        anchor: function (a, b, c) {
            var k = d(a),
                a = this.jq(),
                h = k.offset(),
                u = k.width();
            k.height();
            var k = a.width(),
                e = a.height(),
                l = b.charAt(0),
                g = b.charAt(1),
                b = h.left,
                h = h.top;
            switch (l) {
                case "t":
                    h -= e
            }
            switch (g) {
                case "c":
                    b += Math.floor((u - k) / 2)
            }
            c && (u = u = [b, h], c(u, k, e), b = u[0], h = u[1]);
            a.css("left", b + "px").css("top", h + "px")
        },
        center: function () {
            var a = this.jq(),
                b = [E.width(), E.height()],
                a = [a.width(), a.height()],
                c = (b[1] - a[1]) * 0.8;
            this.view.style.left = Math.max((b[0] - a[0]) / 2 | 0, 0) + d(l).scrollLeft() + "px";
            this.view.style.top = Math.max(c - c / 2 | 0, 0) + d(l).scrollTop() + "px";
            return this
        },
        clip: function () {
            if (!z.CLIP_WRAPPER_CSS) z.CLIP_WRAPPER_CSS = {
                position: "absolute",
                clear: "both",
                overflow: "hidden"
            }, z.CLIPPER_CSS = {
                position: "absolute",
                left: 0,
                top: 0
            };
            if (!this.jqClipWrapper) {
                var a = d(u.get("div")),
                    b = this.getView(),
                    c = this.jq(),
                    k = b.parentNode,
                    h = c.offset();
                a.css(z.CLIP_WRAPPER_CSS).css(h).css("width", c.width() + "px").css("height", c.height() + "px").css("z-index", c.css("z-index")).append(b);
                var h = this._tmpClipedCss = {}, e;
                for (e in z.CLIPPER_CSS) h[e] = b.style[e];
                c.css(z.CLIPPER_CSS);
                k && a.appendTo(k);
                this.jqClipWrapper = a
            }
            return this.jqClipWrapper
        },
        unclip: function () {
            if (this.jqClipWrapper) {
                var a = this.jqClipWrapper[0],
                    d = a.style,
                    b = this.jq(),
                    c = b[0].style,
                    k;
                for (k in z.CLIP_WRAPPER_CSS) d[k] = "";
                this.jqClipWrapper.css("overflow", "").css("width", "").css("height", "");
                d = this._tmpClipedCss;
                for (k in d) c[k] = d[k];
                delete this._tmpClipedCss;
                a.removeChild(b[0]);
                a.parentNode && this.jqClipWrapper.replaceWith(b);
                u.put("div", a);
                delete this.jqClipWrapper
            }
        },
        slide: function (a, d, b, c, k, h) {
            var u = this.jq(),
                e = u.width(),
                l = u.height(),
                g = 0,
                w = 0,
                j = 0,
                p = 0;
            this.clip();
            var v = a.charAt(0),
                a = a.charAt(1);
            switch (v) {
                case "l":
                    g = 0 - e;
                    break;
                case "r":
                    g = 0 + e;
                    break;
                case "t":
                    w = 0 - l;
                    break;
                case "b":
                    w = 0 + l
            }
            switch (a) {
                case "l":
                    j = 0 - e;
                    break;
                case "r":
                    j = 0 + e;
                    break;
                case "t":
                    p = 0 - l;
                    break;
                case "b":
                    p = 0 + l
            }
            u.css("left", g).css("top", w);
            c || (c = {});
            if (j != g) c.left = c.left === void 0 ? j : c.left + j;
            if (p != w) c.top = c.top === void 0 ? p : c.top + p;
            d && u.css("visibility", "");
            var n = this;
            u.animate(c, k || "fast", h, function () {
                d || (n.display(!1), u.css("visibility", ""));
                setTimeout(function () {
                    n.unclip();
                    b && b(n)
                }, 0)
            })
        }
    };
    var L = new y.Base;
    L._flied = h;
    L.unfly = function () {
        this.view = null
    };
    z.fly = function (a) {
        a && (typeof a === "string" ? a = d(a)[0] : a.get && (a = a[0]));
        L.view = a;
        return L;
    };
    c.reg("base", z);
    var J = {
        layers: [],
        hash: {},
        keyListeners: 0,
        add: function (a) {
            var b = a.cacheId;
            this.hash[b] || (this.layers.push(a), a.keyEvent && (this.keyListeners === 0 && (__debug && console.log("bind key listener"), d(l).bind("keydown", this.getEvtHandler())), this.keyListeners++), this.hash[b] = h)
        },
        remove: function (a) {
            var b = a.cacheId;
            if (this.hash[b]) {
                var c = this.layers;
                c[c.length - 1] === a ? c.pop() : q.arrayRemove(c, a);
                this.keyListeners--;
                this.keyListeners === 0 && (__debug && console.log("remove key listener"), d(l).unbind("keydown", this.getEvtHandler()));
                delete this.hash[b]
            }
        },
        getEvtHandler: function () {
            var a = this._onDocKeydown;
            if (!a) a = this._onDocKeydown = q.bind(this.onDocKeydown, this);
            return a
        },
        onDocKeydown: function (a) {
            var d = this.layers[this.layers.length - 1];
            if (d && d.keyEvent) return d.onKeydown(a)
        }
    }, E = d(window),
        G = y.Layer = q.create(z, {
            hidden: h,
            onViewReady: d.noop,
            trackZIndex: function () {
                if (this.z !== p) p += 3, this.mask && d(this.mask).css("z-index", p - 1), this.frameMask && d(this.getFrameMask()).css("z-index", p - 2), this.jq().css("z-index", p), this.z = p
            },
            keyEvent: h,
            onKeydown: function (a) {
                if (a.keyCode === 27 && !this.cancelEscKeyEvent) return this.close(), !1
            },
            beforeShow: function () {
                this.mask && this._applyMask(h);
                var a = this.jq().css("position");
                (a === "absolute" || a === "fixed") && this.trackZIndex();
                J.add(this);
                this.autoCenter && this.center()
            },
            afterHide: function () {
                this.mask && this._applyMask(!1);
                J.remove(this)
            },
            getFrameMask: function () {
                return this.frameMaskEl ? this.frameMaskEl : this.frameMaskEl = n.forNode('<iframe class="shade-div shade-iframe" frameborder="0"></iframe>')
            },
            _applyMask: function (a) {
                var b = this.mask;
                if (!b || b === h) b = this.mask = n.forNode(n.Mask);
                var c = E.height();
                a ? (d(b).height(c).appendTo(l.body), this.frameMask && d(this.getFrameMask()).height(c).appendTo(l.body), d(window).bind("resize", q.getBind(this, "onMaskWinResize"))) : (d(b).remove(), this.frameMask && d(this.getFrameMask()).remove(), d(window).unbind("resize", q.getBind(this, "onMaskWinResize")))
            },
            onMaskWinResize: function () {
                var a = this.mask,
                    b = E.height();
                a && d(a).height(b);
                this.frameMask && d(this.getFrameMask()).height(b);
                this.autoCenter && this.center()
            }
        });
    c.reg("Layer", G);
    y.Switcher = function (a) {
        a && d.extend(this, a);
        this.initUI()
    };
    y.Box = c.reg("Box", q.create(y.Layer, {
        view: "Box"
    }));
    y.Tip = c.reg("Tip", q.create(y.Box, {
        cs: "win-fixed",
        autoHide: h,
        timeoutHide: 500,
        stayHover: !1,
        offX: 25,
        offY: -10,
        innerViewReady: function () {
            var a = this.jq();
            this.stayHover && a.hover(q.bind(this.onMouseover, this), q.bind(this.onMouseout, this))
        },
        onMouseover: function () {
            this.clearHideTimer()
        },
        onMouseout: function () {
            this.setHideTimer()
        },
        clearHideTimer: function () {
            if (this.hideTimerId) clearTimeout(this.hideTimerId), this.hideTimerId = !1;
        },
        beforeShow: function () {
            if (G.prototype.beforeShow.apply(this, arguments) === !1) return !1;
            this.autoHide && this.setHideTimer();
        },
        setHideTimer: function () {
            this.clearHideTimer();
            this.hideTimerId = setTimeout(this._getHideTimerCall(), this.timeoutHide);
        },
        _onTimerHide: function () {
            this.display(!1);
        },
        _getHideTimerCall: function () {
            if (!this._onHideTimer) this._onHideTimer = q.bind(function () {
                this._onTimerHide();
                this.clearHideTimer();
            }, this);
            return this._onHideTimer;
        }
    }));
    y.Dialog = c.reg("Dlg", q.create(y.Box, function (a) {
        return {
            cs: "win-tips win-fixed",
            contentHtml: "DialogContent",
            focusBtnCs: "bGreen",
            mask: h,
            closeable: h,
            initUI: function () {
                if (this.buttons && !this.buttonHtml) {
                    for (var d = [], b = 0, c = this.buttons, k = c.length; b < k; b++) d.push(n.parse(this.buttonTpl || "Button", c[b]));
                    this.buttonHtml = d.join("")
                }
                a.initUI.call(this);
            },
            setFocus: function (a) {
                if (a || this.defBtn) this.jq("#xwb_btn_" + (a || this.defBtn)).focus().addClass(this.focusBtnCs);
            },
            afterShow: function () {
                a.afterShow.call(this);
                this.defBtn && this.setFocus();
            },
            onbuttonclick: function (a) {
                __debug && console.log(a + " clicked");
            },
            setHandler: function (a) {
                this.onbuttonclick = a;
                return this;
            },
            getButton: function (a) {
                return this.jq("#xwb_btn_" + a);
            },
            innerViewReady: function (b) {
                a.innerViewReady.call(this, b);
                var c = this;
                d(b).find("#xwb_dlg_btns").click(function (a) {
                    if (a = q.domUp(a.target, function (a) {
                        return a.id && a.id.indexOf("xwb_btn_") === 0;
                    }, this)) {
                        var b = a.id.substr(8);
                        c.buttons && d.each(c.buttons, function () {
                            if (this.id === b) {
                                var a;
                                this.onclick && (a = this.onclick(c));
                                a !== !1 && c["on" + b] && (a = c["on" + b]());
                                a !== !1 && c.onbuttonclick(b) !== !1 && c.close();
                            }
                        });
                        return !1;
                    }
                })
            }
        }
    }));
    y.MsgBox = c.reg("msgbox", {
        getSysBox: function () {
            var a = this.sysBox;
            if (!a) a = this.sysBox = c.use("Dlg", {
                appendTo: l.body,
                title: "\u63d0\u793a",
                dlgContentHtml: "MsgDlgContent",
                mask: h,
                buttons: [{
                    title: "确&nbsp;定",
                    id: "ok"
                }, {
                    title: "取&nbsp;消",
                    id: "cancel"
                }, {
                    title: "&nbsp;是&nbsp;",
                    id: "yes"
                }, {
                    title: "&nbsp;否&nbsp;",
                    id: "no"
                }, {
                    title: "关&nbsp;闭",
                    id: "close"
                }],
                setContent: function (a) {
                    this.jq("#xwb_msgdlg_ct").html(a);
                },
                setIcon: function (d) {
                    var b = a.jq("#xwb_msgdlg_icon");
                    b.attr("class", b.attr("class").replace(/icon\-\S+/i, "icon-" + d));
                },
                afterHide: function () {
                    y.Dialog.prototype.afterHide.call(this);
                    this.onbuttonclick = y.Dialog.prototype.onbuttonclick;
                }
            });
            return a;
        },
        getTipBox: function () {
            var a = this.tipBox;
            if (!a) a = this.tipBox = c.use("Tip", {
                cs: "win-tips win-fixed",
                contentHtml: "DialogContent",
                appendTo: l.body,
                view: "Box",
                title: "\u63d0\u793a",
                timeoutHide: 1200,
                dlgContentHtml: "MsgDlgContent",
                setContent: function (a) {
                    this.jq("#xwb_msgdlg_ct").html(a);
                },
                setIcon: function (d) {
                    var b = a.jq("#xwb_msgdlg_icon");
                    b.attr("class", b.attr("class").replace(/icon\-\S+/i, "icon-" + d));
                },
                afterHide: function () {
                    y.Tip.prototype.afterHide.call(this);
                    if (this.onhide) this.onhide(), this.onhide = !1;
                }
            });
            return a
        },
        getAnchorDlg: function () {
            var a = this._anchorDlg;
            if (!a) a = this._anchorDlg = c.use("Dlg", {
                cs: "win-tips-ask",
                mask: !1,
                dlgContentHtml: "AnchorDlgContent",
                appendTo: l.body,
                defBtn: "ok",
                buttons: [{
                    title: "\u786e&nbsp;\u5b9a",
                    id: "ok"
                }, {
                    title: "\u53d6&nbsp;\u6d88",
                    id: "cancel"
                }],
                setAnchor: function (a) {
                    this.anchorEl = a;
                    return this;
                },
                beforeShow: function () {
                    y.Dialog.prototype.beforeShow.call(this);
                    if (this.anchorEl) {
                        this.anchor(this.anchorEl, "tc", function (a) {
                            a[1] -= 2;
                        });
                        var a = this;
                        this.slide("bc", h, function () {
                            y.Dialog.prototype.afterShow.call(a);
                        })
                    }
                },
                afterShow: d.noop,
                beforeHide: function () {
                    if (this.anchorEl) return this.slide("cb", !1), delete this.anchorEl, !1;
                    else y.Dialog.prototype.beforeHide.call(this);
                },
                afterHide: function () {
                    y.Dialog.prototype.afterHide.call(this);
                    this.onbuttonclick = y.Dialog.prototype.onbuttonclick;
                }
            });
            return a;
        },
        getAnchorTip: function () {
            var a = this._anchorTip;
            if (!a) a = this._anchorTip = c.use("Tip", {
                view: "Box",
                cs: "operate-success",
                contentHtml: "AnchorTipContent",
                appendTo: l.body,
                timeoutHide: 1800,
                setAnchor: function (a) {
                    this.anchorEl = a;
                    return this;
                },
                beforeShow: function () {
                    this.anchorEl && (this.anchor(this.anchorEl, "tc", function (a) {
                        a[1] -= 8
                    }), this.slide("bc", h));
                    y.Tip.prototype.beforeShow.call(this);
                },
                beforeHide: function () {
                    if (this.anchorEl) return this.slide("cb", !1), delete this.anchorEl, !1;
                    else y.Tip.prototype.beforeHide.call(this);
                }
            });
            return a;
        },
        tipError: function (a, d) {
            this.tip(a, "error", d);
        },
        tipOk: function (a, d) {
            this.tip(a, "success", d);
        },
        tipWarn: function (a, d) {
            this.tip(a, "alert", d);
        },
        anchorTipOk: function (a, d) {
            this.anchorTip(a, d);
        },
        anchorConfirm: function (a, d, b) {
            this.getAnchorDlg().setTitle(d).setHandler(b).setAnchor(a).display(h);
        },
        tip: function (a, d, b) {
            var c = this.getTipBox();
            c.setIcon(d || "alert");
            c.setContent(a || "");
            c.display(h);
            b && (c.onhide = b);
        },
        anchorTip: function (a, d) {
            this.getAnchorTip().setTitle(d).setAnchor(a).display(h)
        },
        alert: function (a, d, b, c, k, u) {
            var e = this.getSysBox(),
            l = e.buttons,
            g = z.fly(e.view);
            c || (u = c = "ok");
            k || (k = "alert");
            for (var w = 0, j = l.length; w < j; w++) g.fly(e.jq("#xwb_btn_" + l[w].id).get(0)).display(c.indexOf(l[w].id) >= 0);
            g.unfly();
            e.defBtn = u;
            a && e.setTitle(a);
            d && e.setContent(d);
            k && e.setIcon(k);
            b && (e.onbuttonclick = b);
            e.display(h);
            return e;
        },
        confirm: function (a, d, b, c) {
            this.alert(a || "提示", d, b, "ok|cancel", "ask", c || "ok");
        },
        success: function (a, d, b, c, k) {
            this.alert(a, d, b, c || "ok", "success", k || "ok");
        },
        error: function (a, d, b, c, k) {
            this.alert(a, d, b, c || "ok", "error", k || "ok");
        }
    });
})(Xwb, $);

(function (d, b) {
    d.fn.extend({
        zIndex: function (e) {
            if (e !== b) return this.css("zIndex", e);
            if (this.length) for (var e = d(this[0]), g; e.length && e[0] !== document;) {
                g = e.css("position");
                if (g === "absolute" || g === "relative" || g === "fixed") if (g = parseInt(e.css("zIndex"), 10), !isNaN(g) && g !== 0) return g;
                e = e.parent();
            }
            return 0;
        }
    });
})(jQuery);



Date.prototype.to_s = function () {
    var d = this.getMonth() + 1,
        b = this.getDate();
    d < 10 && (d = "0" + d);
    b < 10 && (b = "0" + b);
    return this.getFullYear() + "-" + d + "-" + b
};

Date.prototype.to_longs = function () {
    var d = this.getMonth() + 1,
        b = this.getDate();
    d < 10 && (d = "0" + d);
    b < 10 && (b = "0" + b);

    var m = this.getHours() > 9 ? this.getHours().toString() : '0' + this.getHours();
    var s = this.getMinutes() > 9 ? this.getMinutes().toString() : '0' + this.getMinutes();
    return this.getFullYear() + "-" + d + "-" + b + " " + m + ":" + s;
};


; (function (d) {
    if (!d) d = window.iv = {};

    window.Xwb || (Xwb = {});

    $.extend(!0, d, {
        inited: !1,
        bind: function () {
            $('#right-menu > ul > li > a').click(function () {
                $(this).hasClass("cur") || $("#right-menu > ul > li > dl").slideUp();
                $(this).next("dl").length && ($(this).toggleClass("cur"), $(this).next("dl").slideToggle(350));

                $('#right-menu > ul > li > a').last().removeClass('noBorderB');
                
                //$(this).addClass("activeA");
            });

            var cmid = $('input[name="cmid"]').val();

            var e = $("#" + cmid);
            if ((""+cmid+"").length === 3) {
                e.addClass("pcur");
                //e.parent().parent().parent().children().eq(0).addClass("activeA");
            } else {
                e.addClass("cur"), e.closest("dl").show(), e.closest("dl").prev("a").addClass("cur");
                //e.parent().parent().parent().children().eq(0).addClass("activeA");
            }


            $("#setting").click(function (b) {
                b.stopPropagation();
                b.preventDefault();
                $(this).toggleClass("cur");
                $("#modify-box").slideToggle(300);
            });

            $(document).click(function (d) {
                d.currentTarget != $("#setting")[0] && ($("#setting").removeClass("cur"), $("#modify-box").hide());
            });

        },
        init: function () {
            if (this.inited == !1) {
                this.bind();
                $.isFunction(this.childInit) && this.childInit();
            };
            this.inited = !0;
        }
    });

})(window.iv);


function convertCurrency(currencyDigits) {
    // Constants:
    var MAXIMUM_NUMBER = 99999999999.99;
    // Predefine the radix characters and currency symbols for output:
    var CN_ZERO = "零";
    var CN_ONE = "壹";
    var CN_TWO = "贰";
    var CN_THREE = "叁";
    var CN_FOUR = "肆";
    var CN_FIVE = "伍";
    var CN_SIX = "陆";
    var CN_SEVEN = "柒";
    var CN_EIGHT = "捌";
    var CN_NINE = "玖";
    var CN_TEN = "拾";
    var CN_HUNDRED = "佰";
    var CN_THOUSAND = "仟";
    var CN_TEN_THOUSAND = "万";
    var CN_HUNDRED_MILLION = "亿";
    var CN_SYMBOL = "";
    var CN_DOLLAR = "元";
    var CN_TEN_CENT = "角";
    var CN_CENT = "分";
    var CN_INTEGER = "整";
    // Variables:
    var integral; // Represent integral part of digit number.
    var decimal; // Represent decimal part of digit number.
    var outputCharacters; // The output result.
    var parts;
    var digits, radices, bigRadices, decimals;
    var zeroCount;
    var i, p, d;
    var quotient, modulus;
    // Validate input string:
    currencyDigits = currencyDigits.toString();
    if (currencyDigits == "") {
        alert("Empty input!");
        return "";
    }
    if (currencyDigits.match(/[^,.\d]/) != null) {
        alert("Invalid characters in the input string!");
        return "";
    }
    if ((currencyDigits).match(/^((\d{1,3}(,\d{3})*(.((\d{3},)*\d{1,3}))?)|(\d+(.\d+)?))$/) == null) {
        alert("Illegal format of digit number!");
        return "";
    }
    // Normalize the format of input digits:
    currencyDigits = currencyDigits.replace(/,/g, ""); // Remove comma delimiters.
    currencyDigits = currencyDigits.replace(/^0+/, ""); // Trim zeros at the beginning.
    // Assert the number is not greater than the maximum number.
    if (Number(currencyDigits) > MAXIMUM_NUMBER) {
        alert("Too large a number to convert!");
        return "";
    }
    // Process the coversion from currency digits to characters:
    // Separate integral and decimal parts before processing coversion:
    parts = currencyDigits.split(".");
    if (parts.length > 1) {
        integral = parts[0];
        decimal = parts[1];
        // Cut down redundant decimal digits that are after the second.
        decimal = decimal.substr(0, 2);
    } else {
        integral = parts[0];
        decimal = "";
    }
    // Prepare the characters corresponding to the digits:
    digits = new Array(CN_ZERO, CN_ONE, CN_TWO, CN_THREE, CN_FOUR, CN_FIVE, CN_SIX, CN_SEVEN, CN_EIGHT, CN_NINE);
    radices = new Array("", CN_TEN, CN_HUNDRED, CN_THOUSAND);
    bigRadices = new Array("", CN_TEN_THOUSAND, CN_HUNDRED_MILLION);
    decimals = new Array(CN_TEN_CENT, CN_CENT);
    // Start processing:
    outputCharacters = "";
    // Process integral part if it is larger than 0:
    if (Number(integral) > 0) {
        zeroCount = 0;
        for (i = 0; i < integral.length; i++) {
            p = integral.length - i - 1;
            d = integral.substr(i, 1);
            quotient = p / 4;
            modulus = p % 4;
            if (d == "0") {
                zeroCount++;
            } else {
                if (zeroCount > 0) {
                    outputCharacters += digits[0];
                }
                zeroCount = 0;
                outputCharacters += digits[Number(d)] + radices[modulus];
            }
            if (modulus == 0 && zeroCount < 4) {
                outputCharacters += bigRadices[quotient];
            }
        }
        outputCharacters += CN_DOLLAR;
    }
    // Process decimal part if there is:
    if (decimal != "") {
        for (i = 0; i < decimal.length; i++) {
            d = decimal.substr(i, 1);
            if (d != "0") {
                outputCharacters += digits[Number(d)] + decimals[i];
            }
        }
    }
    // Confirm and return the final output string:
    if (outputCharacters == "") {
        outputCharacters = CN_ZERO + CN_DOLLAR;
    }
    if (decimal == "") {
        outputCharacters += CN_INTEGER;
    }
    outputCharacters = CN_SYMBOL + outputCharacters;
    return outputCharacters;
}

function handleAuth(e) {
    if (typeof e.unAuth != 'undefined') {
        window.location.href = '/AdminUser/Login';
    }
}

function getErrorMessage(e) {
    if (typeof e.errors != 'undefined') {
        var message = "错误信息:\n";
        $.each(e.errors, function (key, value) {
            if ('errors' in value) {
                $.each(value.errors, function () {
                    message += this + "\n";
                });
            }
        });
        return message;
    } else if (typeof e.statusText != 'undefined') {
        return e.statusText;
    } else if (typeof e.message != 'undefined') {
        return e.message;
    } else {
        return e;
    }
}
function errors(e) {
    if (typeof e.errors != 'undefined') {
        var message = "错误信息:\n";
        $.each(e.errors, function (key, value) {
            if ('errors' in value) {
                $.each(value.errors, function () {
                    message += this + "\n";
                });
            }
        });
        alert(message);
    } else if (typeof e.statusText != 'undefined') {
        alert(e.statusText);
    } else if (typeof e.unAuth != 'undefined') {
        window.location.href = '/AdminUser/Login';
    } else if (typeof e.message != 'undefined') {
        alert(e.message);
    } else {
        alert(e);
    }
}

kendo.dataviz.ui.Chart.fn.options.autoBind = false;
kendo.culture("zh-CN");


kendo.ui.FilterMenu.prototype.options.messages = 
  $.extend(kendo.ui.FilterMenu.prototype.options.messages, {
      info: "条件",
      filter: "过滤",
      clear: "清除",
      isTrue: "为真",
      isFalse: "为假",
      and: "并且",
      or: "或者",
      selectValue: "-请选择-"
});
         
kendo.ui.FilterMenu.prototype.options.operators =           
  $.extend(kendo.ui.FilterMenu.prototype.options.operators, {
      string: {
          eq: "等于",
          neq: "不等于",
          startswith: "开始于",
          contains: "包含",
          doesnotcontain: "不包含",
          endswith: "结束于"
      },
      number: {
          eq: "等于",
          neq: "不等于",
          gte: "大于等于",
          gt: "大于",
          lte: "小于等于",
          lt: "小于"
      },
      date: {
          eq: "等于",
          neq: "不等于",
          gte: "大于等于",
          gt: "大于",
          lte: "小于等于",
          lt: "小于"
      },
      enums: {
          eq: "等于",
          neq: "不等于"
      }
  });

kendo.ui.Pager.prototype.options.messages = 
  $.extend(kendo.ui.Pager.prototype.options.messages, {
      display: "{0} - {1} 条，共 {2} 条记录",
      empty: "没有记录",
      page: "第",
      of: "页，共 {0} 页",
      itemsPerPage: "每页记录",
      first: "首页",
      previous: "上一页",
      next: "下一页",
      last: "尾页",
      refresh: "刷新"
  });

function formatStr(str, format) {
    str = str.toString();
    var list = format.split("");

    for (var i = 0; i < list.length; i++) {
        if (list[i] == "0") {
            continue;
        }

        str = str.replace(eval("/(.{" + i + "})/"), "$1" + list[i]);
    }

    return str.substring(0, list.length);
}

function childHeight(s,h) {
    $(s).css('height', h);
}

function getFilePath(p) {
    var serverPath = $('#FileServerPath').val();
    
    if (typeof p == 'undefined' || p == null || p == '') return '';
    
    if (p.charAt(0) == '~' ) {
        return serverPath + p.substr(2);
    }else if (p.charAt(0) == '/') {
        return serverPath + p.substr(1);
    }else {
        return serverPath + p;
    }
}

function getLocalFilePath(p) {
    if (typeof p == 'undefined' || p == null || p == '') return '';

    if (p.charAt(0) == '~') {
        return p.substr(2);
    } else if (p.charAt(0) == '/') {
        return p.substr(1);
    } else {
        return p;
    }
}

function getThumbPath(p) {
    if (typeof p == 'undefined' || !p) {
        p = '~/Avatars/default.jpg';
    }
    return getFilePath(p.replace(/(.*\/)(.*)\.jpg$/, '$1$2_thumb.jpg'));
}

var categoryType = new Array("未知", "创意图片", "编辑图片");

$.ajaxSetup({ cache: false });
