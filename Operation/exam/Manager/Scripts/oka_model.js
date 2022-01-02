function isOldIE() { //判斷是不是舊IE

    var odd_ie = false, //存判斷結果
        _appName = window.navigator.appName,
        _version = window.navigator.appVersion.split(';')[1] || '';

    _version = _version.replace(/[ ]/g, '');

    if (_appName === 'Microsoft Internet Explorer' && (_version === 'MSIE8.0' || _version === 'MSIE7.0' || _version === 'MSIE6.0')) { //如果是 IE6 - IE8
        odd_ie = true;
    }

    window.oldIE = odd_ie;

    return odd_ie;
}

//相簿輪播
$.fn.module_photo = function () {

    //取所有輪播物件
    var $models = $(this);

    //判斷瀏覽器
    var odd_ie = window.oldIE || isOldIE(); //存判斷結果

    $models.each(function () {

        //取物件 & 參數
        var $that = $(this), //取情境
            _slider = $that.data('slider'), //是否輪播
            _sliderSpeed = parseInt($that.data('sliderspeed'), 10), //切換速度
            _animateSpeed = parseInt($that.data('animatespeed'), 10); //動畫速度

        var $ul = $that.find('ul'), //取播放物件
            $ul_length = $ul.length,
            $ctrl_tools = $that.find('.model_ctrl_tools');

        var _index = 0; //開始播放的順序

        //判斷瀏覽器版本
        if (odd_ie && !$that.hasClass('straight')) { //如果是 IE6 - IE8、沒有直式樣式

            $ul.each(function () { //給每個ul的li寫入行內寬度 

                var $this = $(this),
                    $li = $this.children('li'), //取li
                    $li_length = $li.length; //取length

                if ($li.length < 10) { //如果li的數量小於10個
                    $li.css('width', (100 / $li_length).toFixed(3) - 0.1 + '%'); //寫入寬度 ( -0.1 是因為IE7 滿百不能並排 )
                }
            });
        }

        //如果有一個以上的
        if ($ul_length > 1) {

            //隱藏ul並顯示第一個
            $ul.hide().eq(_index).show();

            //新增左右按鈕
            $ctrl_tools.prepend('<a class="ctrl_left" href="#">&lsaquo;</a>');
            $ctrl_tools.append('<a class="ctrl_right" href="#">&rsaquo;</a>');

            //取物件
            var $ctrl_left = $ctrl_tools.find('.ctrl_left').data('dir', -1), //抓左右按鈕並設定方向參數
                $ctrl_right = $ctrl_tools.find('.ctrl_right').data('dir', 1),
                $btns = $ctrl_left.add($ctrl_right); //取按鈕集

            //按下按鈕
            $btns.click(function (event) {
                event.preventDefault(); //停止預設動作

                var $this = $(this),
                    _dir = $this.data('dir'); //取方向參數

                slider(_dir);
            });

            if (_slider) { //如果要輪播
                var timer; //設定計時器

                //設定輪播涵式
                function auto() {
                    slider(1); //播放下一個
                    timer = setTimeout(auto, _sliderSpeed);
                }

                //設定滑進滑出項目
                $that.mouseover(function () {
                    clearTimeout(timer);
                });

                $that.mouseout(function () {
                    timer = setTimeout(auto, _sliderSpeed);
                });

                //輪播開始
                timer = setTimeout(auto, _sliderSpeed);
            }

            function slider(_away){ //播放核心

                _index = (_index + _away + $ul_length) % $ul_length; //算出第幾個要被撥放
                $ul.eq(_index).fadeIn(_animateSpeed).siblings('ul').hide(); //show出圖片並把其他隱藏
            }
        }
    });
}

//主視覺輪播
$.fn.oka_slider_model = function ($setting) {

    //取所有輪播物件
    var $models = $(this);

    $models.each(function () {

        //取物件 & 參數
        var $this = $(this), //取情境
            _type = parseInt($this.data('type'), 10) || 0, //輪播類型
            _slider = $this.data('slider'), //是否輪播
            _sliderSpeed = $this.data('sliderspeed') || 5000, //切換速度
            _animateSpeed = $this.data('animatespeed') || 300; //動畫速度

        var $slider_box = $this.find('.slider_model_box'), //取播放區域
            $slider_box_c = $slider_box.children(), //取要播放的子物件
            $slider_box_c_length = $slider_box_c.length, //取播放物件的數量
            $slider_box_img = $slider_box.find('img'), //取播放物件的img
            $slider_box_img_length = $slider_box_img.length //取播放物件的數量

        if ($slider_box_c_length != $slider_box_img_length || $slider_box_c_length <= 1) { //如果有物件沒有圖或不是圖就停止程式
            return false;
        }

        // 製作附加模組
        var _dom = ''; //存物件字串

        var _list_box = '<div class="slider_list_box"><ul>'; //建立右播放清單
        for (i = 0; i < $slider_box_c.length; i++) {
            _list_box += '<li><a href="#"><div style="background-image: url(' + $slider_box_img.eq(i).attr('src') + ');"></div><h5>' + $slider_box_img.eq(i).attr('title') + '</h5><p>' + $slider_box_img.eq(i).attr('alt') + '</p></a></li>';
        }
        _list_box += '</ul></div>';

        var _ctrl_img_box = '<div class="ctrl_img_box"><ul>'; //建立影像點點
        for (i = 0; i < $slider_box_c.length; i++) {
            _ctrl_img_box += '<li><a href="#" style="background-image: url(' + $slider_box_img.eq(i).attr('src') + ');"></a></li>\n\r';
        }
        _ctrl_img_box += '</ul></div>';

        var _ctrl_dot_box = '<div class="ctrl_dot_box"><ul>'; //建立點點
        for (i = 0; i < $slider_box_c.length; i++) {
            _ctrl_dot_box += '<li><a href="#" title="'+ $slider_box_img.eq(i).attr('title') +'"></a></li>\n\r';
        }
        _ctrl_dot_box += '</ul></div>';

        var _ctrl_tools = '<div class="model_ctrl_tools"><a href="#" class="ctrl_left">&lsaquo;</a><a href="#" class="ctrl_right">&rsaquo;</a></div>', //建立控制按鈕
            _introduce_box = '<div class="slider_introduce_box"><h5></h5><p></p>' + _ctrl_dot_box + '</div>', //建立左側撥放方框
            _sec_box = '<a href="#" class="slider_sec_model_box"></a>'; //建立第二組輪播視窗

        //依type加入body
        switch (_type) {

            case 1:
                _dom += _ctrl_tools + '<div class="slider_bottom_box">' + _ctrl_dot_box + '</div>';
                break;

            case 2:
                _dom += '<div class="slider_bottom_box">' + _ctrl_dot_box + '<div class="bg_box"><h5></h5></div></div>';
                break;

            case 3:
                _dom += _ctrl_tools + '<div class="slider_bottom_box"><div class="bg_box">' + _ctrl_dot_box + '<p></p></div></div>';
                break;

            case 4:
                _dom += '<div class="slider_bottom_box">' + _ctrl_dot_box + '</div>' + _list_box;
                break;

            case 5:
                _dom += '<div class="slider_bottom_box"><div class="bg_box">' + _ctrl_dot_box + '<p></p></div></div>' + _list_box;
                break;

            case 6:
                _dom += '<div class="slider_bottom_box"><div class="bg_box">' + _ctrl_img_box + '</div></div>';
                break;

            case 7:
                _dom += _ctrl_tools + '<div class="slider_bottom_box"><div class="bg_box">' + _ctrl_img_box + '</div></div>';
                break;

            case 8:
                _dom += _introduce_box;
                break;

            case 9:
                _dom += _introduce_box + _sec_box;
                break;

            case 999:
                _dom += _ctrl_tools + _introduce_box + _sec_box + '<div class="slider_bottom_box">' + _ctrl_dot_box + '<div class="bg_box"><h5></h5>' + _ctrl_dot_box + '<p></p></div></div>' + _list_box;
                break;

            default:
                _dom += '<div class="slider_bottom_box">' + _ctrl_dot_box + '</div>';
        }

        $this.append(_dom); //加入模組

        //控制附加模組
        var $ctrl_tools = $this.find('.model_ctrl_tools'), //取控制按鈕組
            $sec_box = $this.find('.slider_sec_model_box'), //取第二組輪播視窗
            $bottom_box = $this.find('.bg_box'), //取下方外層方框
            $introduce_box = $this.find('.slider_introduce_box'), //取左側播放方框
            $list_box = $this.find('.slider_list_box'), //取右播放清單
            $list_box_a = $list_box.find('a'), //&a
            $ctrl_dot_box = $this.find('.ctrl_dot_box'), //取點點
            $ctrl_dot_box_a = $ctrl_dot_box.find('a'), //&a
            $ctrl_img_box = $this.find('.ctrl_img_box'), //取影像點點
            $ctrl_img_box_a = $ctrl_img_box.find('a'); //&a

        var $ctrl_btns = $list_box_a.add($ctrl_dot_box_a).add($ctrl_img_box_a),
            $ctrl_title = $introduce_box.find('h5').add($bottom_box.find('h5')), //取要更新的標題
            $ctrl_text = $introduce_box.find('p').add($bottom_box.find('p')); //取要更新的內文

        var $ctrl_left = $ctrl_tools.find('.ctrl_left').data('dir', -1), //抓左右按鈕並設定方向參數
            $ctrl_right = $ctrl_tools.find('.ctrl_right').data('dir', 1),
            $away_btns = $ctrl_left.add($ctrl_right); //取按鈕集

        var $list_box_ul = $list_box.find('ul'),  //右側清單ul
            $list_box_li = $list_box.find('li'), //右側清單選項
            $list_box_a = $list_box_li.find('a'); //右側清單連結

        var _index = 0; //紀錄播放的順序

        //如果有第二組輪播視窗就隱藏第一組      
        if ($sec_box.length) { //如果有輪播視窗
            $slider_box_c.css('visibility', 'hidden');
        }

        //滑入點點或影像或右側清單的按鈕
        $ctrl_btns.mouseenter(function (event) {
            event.preventDefault(); //停止預設動作

            var $this = $(this), //取點點
                $this_p = $this.parent('li');

            _index = $this_p.index(); //取點點的順序
            slider(_index); //播放

        }).eq(_index).mouseenter();

        //按下左右
        $away_btns.click(function (event) {
            event.preventDefault(); //停止預設動作

            var $this = $(this),
                _dir = $this.data('dir'); //取方向參數

            change_index(_dir);
        });

        if (_slider) { //如果要輪播
            //設定計時器
            var timer;

            //設定自動撥放涵式
            function auto() {
                change_index(1);

                timer = setTimeout(auto, _sliderSpeed);
            }

            //設定滑進滑出項目
            $this.mouseover(function () {
                clearTimeout(timer);
            });

            $this.mouseout(function () {
                timer = setTimeout(auto, _sliderSpeed);
            });

            //輪播開始
            timer = setTimeout(auto, _sliderSpeed);
        }

        function change_index(_away){ //播放核心

            _index = (_index + _away + $slider_box_c_length) % $slider_box_c_length; //算出第幾個要被撥放
            slider(_index); //播放
        }

        //設定輪播涵式
        function slider(_index) {

            var $slider_dom = $slider_box_c.eq(_index), //存要播放的物件與資料
                $slider_img = $slider_box_img.eq(_index), //存要播放的img
                _href = $slider_dom.attr('href') || null;

            if ($list_box.length) { //如果有$list_box
                var $list_box_h = $list_box.height() - $bottom_box.height(), //播放的眶
                    $list_box_ul_h = $list_box_ul.height(), //清單的高
                    $list_box_slider_h = $list_box_ul_h + $list_box_li.outerHeight(true) - $list_box_h, //右側清單的高
                    $list_box_item = $list_box_li.eq(_index), ///右側清單的播放項目
                    _top = $list_box_item.position().top; //&離父元素的高

                if ($list_box_h > $list_box_ul_h) { //如果框比撥放清單還高
                    _top = 0;
                } else if (_top > $list_box_slider_h) {
                    for (i = 1; $list_box_li.eq(i).position().top <= $list_box_slider_h; i++) //算出最大的_top
                        _top = $list_box_li.eq(i).position().top;
                }

                $list_box_ul.stop().animate({ //讓右側清單選項滑動
                    'margin-top': -1 * _top
                }, _animateSpeed);
            }

            $ctrl_title.text($slider_img.attr('title')); //更新title
            $ctrl_text.text($slider_img.attr('alt')); //更新text
            $sec_box.hide().attr('href', _href).css('background-image', 'url("' + $slider_img.attr('src') + '")').fadeIn(_animateSpeed);

            $slider_dom.fadeIn(_animateSpeed).siblings().hide(); //show出圖片並把其他隱藏

            $list_box_a.removeClass('is_active').eq(_index).addClass('is_active');
            $ctrl_dot_box_a.removeClass('is_active').eq(_index).addClass('is_active');
            $ctrl_img_box_a.removeClass('is_active').eq(_index).addClass('is_active');
        }
    });
}

//底部輪播
$.fn.oka_banner_slider_model = function ($setting) {

    //取所有輪播物件
    var $models = $(this);

    $models.each(function(){

        //取物件 & 參數
        var $this = $(this), //取情境
            _slider = $this.data('slider'), //是否輪播
            _all = $this.data('all'), //是否輪播
            _sliderSpeed = $this.data('sliderspeed') || 5000, //切換速度
            _animateSpeed = $this.data('animatespeed'), //動畫速度
            $mask = $this.find('.banner_mask'),
            $ul = $mask.children('ul'); //取播放物件

        $ul.children('li').each(function () {
            var $this = $(this);

            if ($this.css('display') === 'none') {
                $this.remove();
            }
        });

        var $li = $ul.children('li'),
            $li_length = $li.length,
            $ctrl_tools = $this.find('.model_ctrl_tools'),
            $ctrl_left = $ctrl_tools.find('.ctrl_left').data('dir', 0), //抓左右按鈕並設定方向參數
            $ctrl_right = $ctrl_tools.find('.ctrl_right').data('dir', 1),
            $btns = $ctrl_left.add($ctrl_right), //取按鈕集
            $slider_dom = $();

        var $mask_width = $mask.width(), //取遮罩寬度
            $li_width = $li.width(), //取li寬度
            $li_all_width = $li_width * $li_length; //算出所有li總和寬度

        if ($li_all_width - $li_length >= $mask_width ) { //如果播放物件比框框大
            $ctrl_tools.show();
        }else{
            $ctrl_tools.hide();
        }

        $btns.click(function (event) {
            event.preventDefault(); //停止預設動作

            //取物件
            var $this = $(this),
                _dir = $this.data('dir'); //取方向參數

            slider(_dir);
        });

        if (_slider) { //如果要輪播
            var timer; //設定計時器

            //設定輪播涵式
            function auto() {
                slider(1);
                timer = setTimeout(auto, _sliderSpeed);
            }

            //設定滑進滑出項目
            $this.mouseover(function () {
                clearTimeout(timer);
            });

            $this.mouseout(function () {
                timer = setTimeout(auto, _sliderSpeed);
            });

            //輪播開始
            timer = setTimeout(auto, _sliderSpeed);
        }

        function slider(_away){

            $slider_dom = $(); //清空輪播物件

            //取要計算的寬度
            $mask_width = $mask.width(); //取遮罩寬度
            $li_width = $li.width(); //取li寬度
            $li_all_width = $li_width * $li_length; //算出所有li總和寬度

            if ($li_all_width - $li_length >= $mask_width ) { //如果播放物件比框框大

                $ctrl_tools.show();

                var _offset = '-100%',
                    _num = Math.round($mask_width / $li_width);

                if (!_all) { //如果不是撥整塊
                    _offset = '-' + ( 100 / _num ) + '%'; //把li寬度換算成%
                    _num = 1;
                }

                $li = $ul.children('li'); //重取dom

                if (_away) { //如果往右

                    for (i = 0; i < _num; i++) {
                        $slider_dom = $slider_dom.add($li.eq(i));
                    }

                    $ul.stop().animate({
                        'margin-left': _offset //讓ul變成-$li_width
                    }, _animateSpeed, function () {
                        $slider_dom.appendTo($ul); //把最後一個變成第一個
                        $ul.css('margin-left', 0); //調整margin-left為0
                    });

                } else { //如果往左

                    for (i = -1; i >= -1 * _num; i--) {
                        $slider_dom = $slider_dom.add($li.eq(i));
                    }

                    $ul.css('margin-left', _offset); //預先調整margin-left
                    $slider_dom.prependTo($ul); //把最後一個變成第一個

                    $ul.stop().animate({
                        'margin-left': 0 //讓ul變成-$li_width
                    }, _animateSpeed);
                }
            }else{
                $ctrl_tools.hide();
            }
        }
    });
}

//跑馬燈
$.fn.marquee = function ($setting) {

    //取所有輪播物件
    var $models = $(this);

    $models.each(function () {

        //取物件 & 參數
        var $this = $(this), //取情境
            _sliderSpeed = $this.data('sliderspeed'), //切換速度
            _animateSpeed = $this.data('animatespeed'), //動畫速度
            $box = $this.find('.marquee_box'),
            $ul = $box.find('ul'),
            $li = $box.find('li'),
            $li_length = $li.length,
            $li_h = $li.eq(0).outerHeight(true);

        $box.css({
            'height': $li_h
        });

        $li.css({
            'display': 'block'
        })

        function marquee() {

            $ul.animate({
                'margin-top': -1 * $li_h
            }, _animateSpeed, function () {
                $li.eq(0).appendTo($ul); //把第一個變成最後一個
                $ul.css('margin-top', 0); //調整margin-left為0
            });

            $li = $box.find('li'); //重取dom
        }

        var timer; //設定計時器

        function auto() { //輪播
            marquee();
            timer = setTimeout(auto, _sliderSpeed);
        }

        $this.mouseover(function () { //設定滑進滑出輪播區塊
            clearTimeout(timer);
        });

        $this.mouseout(function () {
            timer = setTimeout(auto, _sliderSpeed);
        });

        timer = setTimeout(auto, _sliderSpeed); //輪播開始
    });
}

$.fn.main_menu = function ($setting) {

    //取物件
    var $menus = $(this);

    $menus.each(function () {

        var $this = $(this),
            $li = $this.children('li'), //第一層li
            $li_length = $li.length,
            odd_ie = window.oldIE || isOldIE(), //存判斷結果
            $a = $('.menu').children('a'),
            $all_a = $this.find('a'),
            $submenuarea = $('.submenuarea,.submenuarea2'),
            $submenuarea_a = $submenuarea.find('a');

        $all_a.each(function () { //檢查所有的a
            var $this = $(this);

            if ($this.data('pic')) { //如果有圖片屬性
                var _src = $this.data('pic'); //取出路徑

                $this.html('<img src="' + _src + '">');

                var $img = $this.find('img');

                if ($this.data('hover-pic')) { //如果有hover圖片路徑
                    var _hover_src = $this.data('hover-pic'); //就取出來

                    $this.mouseenter(function () { //進入時套用
                        $img.attr('src', _hover_src);
                    });

                    $this.mouseleave(function () { //離開時還原
                        $img.attr('src', _src);
                    });
                }
            }
        });

        //判斷瀏覽器版本&是否大於6個項目
        if (odd_ie && $li.css('float') && $li.css('float') !== 'none' && $li_length > 6) { //如果是 IE6 - IE8，有寫浮動，大於六個選單
            $li.css('width', 100 / $li.length + '%'); //指定寬度
        }

        $submenuarea.stop().hide(0);

        $a.focus(function () {
            $submenuarea.hide(0);
            $(this).closest('li').find('.submenuarea,.submenuarea2').stop().fadeIn(150);
        });

        $all_a.eq(-1).blur(function () {
            $submenuarea.stop().fadeOut(150, function () {
                $submenuarea.css('overflow', 'visible');
            });
        });

        $li.mouseenter(function () {
            $(this).find('.submenuarea,.submenuarea2').stop().fadeIn(150);
        });

        $li.mouseleave(function () {
            $submenuarea.stop().fadeOut(150, function () {
                $submenuarea.css('overflow', 'visible');
            });
        });
    });
}

//tab切換
$.fn.oka_tab_model = function (setting) {
    var $this = $(this),
        $set = setting || {};

    $this.each(function () { //頁面中每一個頁面區塊都run一次
        var $this = $(this),
            $a = $this.find('a'),
            $hrefs = $('');

        if ($a.length <= 1) { //如果不到一個項目，就只幫 $a 加 .active

            $a.addClass('tabs_yes');
        } else {

            $a.each(function () { //把每一個頁籤內頁隱藏
                var $href = $($(this).attr('href'));

                $hrefs = $hrefs.add($href); //把連結目標加入 $hrefs
                $href.hide(); //隱藏連結目標
            });

            $a.click(function (event) {
                event.preventDefault(); //停止預設動作

                var $this = $(this),
                    $href = $($this.attr('href')); //取出連結目標

                $a.removeClass('tabs_yes'); //加入與取走active
                $this.addClass('tabs_yes');

                $a.parent().removeClass('tabs_yes'); //加入與取走active
                $this.parent().addClass('tabs_yes');

                $hrefs.hide(); //顯示與隱藏
                $href.show();

            }).eq(0).click(); //點第一個
        }
    });
}

//全文檢索關鍵字
$.fn.oka_keyword_model = function (setting) {
    var $this = $(this),
        $set = setting || {},
        $txt = $(this).find('input[type="text"]'),
        $ul = $(this).find('ul'),
        $Words = $ul.find('a');
    var isOverUl = false;

    $txt.keyup(function () {
        $ul.show();
        doSearch($txt.val());
    });
    $Words.click(function (e) {
        e.preventDefault();
        $txt.val($(this).html());
        $ul.hide();
    });
    $ul.mouseover(function () {
        isOverUl = true;
    });
    $ul.mouseleave(function () {
        isOverUl = false;
    });

    $txt.focusout(function () {
        if (!isOverUl) {
            $ul.hide();
        }
    });

    function doSearch(keyWord) {
        keyWord = keyWord.toLowerCase();
        $ul.find('li').each(function () {
            var Word = $(this).find('a').html().toLowerCase()
            $(this).hide();
            if (Word.indexOf(keyWord) > -1) {
                $(this).show();
            }
        })
    }
}

$(function () { //調整字體大小
    var $body = $('body'),
        $font_size_btn = $('.fontlevel').find('a'), //抓a
        _font_size = 'm';

    if ($.cookie && $.cookie('font_size')) { _font_size = $.cookie('font_size') } //找cookie，沒有就給預設值


    $font_size_btn.click(function (evt) {
        evt.preventDefault();

        var $this = $(this),
            _font_size = $this.data('font-size');

        $font_size_btn.removeClass('is_active');
        $this.addClass('is_active');

        $.cookie('font_size', _font_size); //存cookie

        $body.attr('class', 'font-size-' + _font_size); //改class值
    });

    $font_size_btn.filter('[data-font-size="' + _font_size + '"]').click();
});

//網頁load後執行
$(function (i) {
    $('[data-function]').each(function(){
        $(this)[$(this).data('function')]();
    });
});