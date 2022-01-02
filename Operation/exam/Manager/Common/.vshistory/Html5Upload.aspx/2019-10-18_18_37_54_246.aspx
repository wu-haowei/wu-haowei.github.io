<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeFile="Html5Upload.aspx.cs" Inherits="Common_RelPic"  %>

  <!-- Css 樣式表 -->
		<link rel="stylesheet" media="screen" href="/css/global.css"> <!-- 全域 -->
	<!-- Css 結束 -->

	<!-- Script -->
    		<script src="/Scripts/jquery-1.12.4.min.js"></script> <!-- 引入 jquery -->
            <script src="/Scripts/fancybox/jquery.fancybox.js"></script> <!-- 引入 jquery -->
            <link href="/Scripts/fancybox/jquery.fancybox.css" rel="stylesheet" />
        <asp:Literal ID="litJS" runat="server"></asp:Literal>


        <script>            //下方表單操作           
            $(function () {
                 $.ajaxSetup({ cache: false });
                if (uploadType == 'RelFile') {
                    $('.base-footer').hide();
                }
                var $document = $(document),
					$body = $('body')
                $form = $('.update-form'),
					$pic = $('.update-pic'),
					$images = $pic.find('.images'),
					_open_class_name = 'is-open',
					_null_class_name = 'is-null',
					_checkbox_01 = '#checkbox_01',
					$show_mark = $(_checkbox_01),
					$default_btn = $('#watermark_01'),
					$customize_btn = $('#watermark_02'),
					$remove_watermark_btn = $pic.find('.remove').find('a'),
					$position = $form.find('.position'),
					$radio = $position.find('[type="radio"]'),
					$opacity = $form.find('.opacity'),
					$range = $opacity.find('[type="range"]'),
					_default_html = '<div class="image"><span style="background-image: url(\'Images/watermark_bg.jpg\')"><img src="Images/watermark_bg.jpg" alt=""></span></div>',
					_default_mark = '<div class="image"><span style="background-image: url(\'Images/watermark.png\')"><img src="Images/watermark.png" alt=""></span></div>';

                //點開加入浮水印，展開選項
                $show_mark.on('change', function (event) {

                    if ($(_checkbox_01 + ':checked').length) {
                        $form.addClass(_open_class_name);
                        $pic.addClass(_open_class_name);
                        $data_area.children('li').each(function () {
                            var ImgPath = $(this).find('img').attr('src');
                            var sn = $(this).find('#hidSN').val();
                            var s = '';
                            s += '<div class="area-figure update-pic is-open" data-sn="' + sn + '">';
                            s += '  <div class="inner">';
                            s += '    <div class="header">';
                            s += '      <div class="inner">';
                            s += '      </div>';
                            s += '    </div>';
                            s += '    <div class="content">';
                            s += '      <div class="inner">';
                            s += '        <div class="image">';
                            s += '          <span style="background-image: url(\'' + ImgPath + '\')"><img src="' + ImgPath + '" alt=""></span>';
                            s += '        </div>';
                            s += '        <div class="image Mark" >';
                            s += '          <span style="opacity: 0.63; background-image: url(\''+MarkwhiteImg+'\');"><img src="'+MarkwhiteImg+'" alt=""></span>';
                            s += '        </div>';
                            s += '      </div>';
                            s += '    </div>';
                            s += '    <div class="footer">';
                            s += '      <div class="inner">';
                            s += '        <ul>';
                            s += '          <li class="remove" onClick="javascrip:RemoveMarkPic(' + sn + ');"><span><a href="javascript:void(0);" title="刪除浮水印">刪除</a></span></li>';
                            s += '        </ul>';
                            s += '      </div>';
                            s += '    </div>';
                            s += '  </div>';
                            s += '</div>';
                            $('.base-footer > .inner > .content > .inner').append(s);
                        })

                    } else {
                        $form.removeClass(_open_class_name);
                        $pic.removeClass(_open_class_name);
                        $('.update-pic').remove();
                    }
                });

                $default_btn.on('click', function () {

                    $pic.removeClass(_null_class_name);
                    $images.html(_default_html + _default_mark);
                    $('#away_01').click();
                    $range.val('100');
                });

                //點自訂
                $customize_btn.on('click', function () {

                    $pic.addClass(_null_class_name);
                    $images.html(_default_html);
                    $('#away_01').click();
                    $range.val('100');
                });

                //點刪除，移除浮水印
                $remove_watermark_btn.on('click', function (event) {
                    event.preventDefault();

                    $customize_btn.click();
                });

                //點位置
                $radio.on('change', function () {
                    var _away = $radio.filter(':checked').data('pos');
                    console.log(_away);
                    $('.update-pic').find('.Mark').each(function () {
                        var $mark = $(this).find('span');
                        $mark.css({
                            'background-position': _away
                        });
                    })
                });

                //移動透明度
                $range.on('input change', function (event) {

                    var	_val = parseInt($range.val(), 10) / 100;

                    $('.update-pic').find('.Mark').each(function () {
                        var $mark = $(this).find('span');
                        $mark.css({
                            'opacity': _val
                        });
                    })
                });
            });
            function RemoveMarkPic(sn){
                $('.update-pic[data-sn="'+sn+'"]').remove();
            }
            
            $(function(){            
                $('#btn_addWaterMarking').click(function () {
                    var Position = $radio.filter(':checked').val();
                    var Transparent = parseInt($range.val(), 10) ;
                    $('.update-pic').each(function () {
                        var SN = $(this).data('sn');
                        var that = $(this);
                        $.ajax({
                            dataType: "text",
                            type: "POST",
                            url: APISPath + "&DataSN=" + SN + "&cmd=WaterMarking" + "&Position=" + Position + "&Transparent=" + Transparent,
                            success: function (msg) {
                                console.log(SN + msg);
                                that.remove();
                                if ($('.update-pic').length == 0) {
                                    Reload();
                                    alert("修改完成!!");
                                }
                            }
                        });
                    })
                })
            })
          
        </script>
        <script>            //拖曳
            $(function () {

                var $document = $(document),
					$root = $('.sys-root'),
					_drop_class_name = 'is-drop',
					_clone_class_name = 'is-clone',
					_hover_class_name = 'is-hover';

                $document.on('mousedown', '.move a', function (event) {

                    var $this = $(this),
						$obj = $this.closest('.obj'),
						$obj_h = $obj.height(),
						$obj_w = $obj.width(),
						$obj_o = $obj.offset(),
						$obj_t = $obj_o.top,
						$obj_l = $obj_o.left,
						_mouse_Y = event.pageY,
						_mouse_X = event.pageX;


                    var $away = $('.before, .after');

                    var $obj_clone = $obj.clone();

                    $root.addClass(_drop_class_name);
                    $obj_clone.addClass(_clone_class_name);

                    $obj_clone.css({
                        'position': 'absolute',
                        'opacity': 0.5,
                        'height': $obj_h,
                        'width': $obj_w,
                        'top': $obj_t,
                        'left': $obj_l
                    });

                    $obj_clone.appendTo($root);

                    //移動
                    $document.on('mousemove', function (event) {

                        var _new_mouse_Y = event.pageY,
							_new_mouse_X = event.pageX;

                        $obj_clone.css({
                            'top': _new_mouse_Y - _mouse_Y + $obj_t,
                            'left': _new_mouse_X - _mouse_X + $obj_l
                        });

                        $away.each(function () {
                            var $this = $(this);

                            if (mou_in_box($this, _new_mouse_Y, _new_mouse_X)) {

                                $away.removeClass(_hover_class_name);
                                $this.addClass(_hover_class_name);

                                return false;
                            }
                        });
                    });

                    //放掉
                    $document.on('mouseup', function (event) {

                        var $this = $('.' + _hover_class_name);

                        if ($this.length) { //如果有對到目標

                            var $target_obj = $this.closest('.obj'),
								_away = $this[0].classList[0]; //方向

                            if ($target_obj.parent('li').find('#hidSN').val() != $obj.parent('li').find('#hidSN').val()) {
                                $target_obj.parent('li')[_away]($obj.parent('li'));
                            }
                        }

                        $root.removeClass(_drop_class_name);
                        $this.removeClass(_hover_class_name);

                        $('.is-clone').remove();

                        $document.unbind('mousemove mouseup');


                        var _SN = $obj.parent('li').find('#hidSN').val();
                        var oldSort = $obj.parent('li').find('#hidSort').val();
                        var newSort = parseInt($target_obj.parent('li').find('#hidSort').val());
                        if (_away == "after" && (oldSort > newSort)) {
                            newSort = newSort + 1;
                        }


                        if (newSort != oldSort) {
                            $.ajax({
                                dataType: "text",
                                type: "POST",
                                url: APISPath + "&DataSN=" + _SN + "&cmd=ChangeSort&oldSort=" + oldSort + "&newSort=" + newSort,
                                success: function (msg) {
                                    Reload();
                                }
                            });
                        }

                        return false;
                    });
                });

                function mou_in_box($node, mou_Y, mou_X) { //判斷游標是否在自己裡面的方法
                    var $node_o = $node.offset(), //求定位
						$node_t = $node_o.top,
						$node_l = $node_o.left,
						$node_w = $node.width(),
						$node_h = $node.height(),
						$node_area_y = $node_t + $node_h, //求範圍
						$node_area_x = $node_l + $node_w;

                    if (mou_Y > $node_t && mou_Y < $node_area_y && mou_X > $node_l && mou_X < $node_area_x) { //檢查上下左右都在範圍內
                        return true; //在容器內就回傳true
                    }
                }
            });
        </script>
		<script>
		    var $upload_area;
		    var $data_area;
		    var FilesSelected = [];
		    function Reload() {
		        $upload_area.find('li').children('.obj').attr("isDelete", "true");
		        $.ajax({
		            dataType: "json",
		            url: APISPath + "&Include=true",
		            success: function (listObj) {
		                $.each(listObj, function (i, l) {
		                    build_albumByjson(listObj[i]);
		                });
		                $upload_area.find('.obj').each(function () {
		                    if ($(this).attr("isDelete") || "false" == "true") {
		                        $(this).parent().remove();
		                    }
		                })
		            }
		        });
		      
		    }


		    //拖曳後 新增 li
		    function build_album(file) {

		        var _title = file.name || '請輸入標題名稱',
						_sn = file.sn || '',
						_introduce = file.name.slice(0, file.name.lastIndexOf('.')) || '請輸入介紹',
						_file_size = (file.size && file.size / 1000 + ' KB') || 'unknow',
						_image_src = URL.createObjectURL(file) || 'Images/update.png';
		        
		        

		        var $node = $('<li><div class="area-figure obj"><div class="inner"><div class="header"><div class="inner"><h4><span><a href="#">項目' + _sn + '</a></span></h4></div></div><div class="content"><div class="inner"><div class="list"><ul><li class="before"></li><li class="after"></li></ul></div><div class="image"><span style="background-image: url(\'' + _image_src + '\')"><img src="' + _image_src + '" alt=""></span></div><i class="mark"><div style="color: #c40;font-size: 1.4em;" class="status">等待上傳</div><span>0%</span></i><div class="caption" >' + _title + '</div><div class="paragraph"><p contenteditable>' + _introduce + '</p></div><div class="label"><ul><li><span>' + _file_size + '</span></li></ul></div></div></div><div class="footer"><div class="inner"><ul><li class="remove"><span><a href="#" title="刪除此項目">刪除</a></span></li><li class="move"><span><a href="#" title="按住以拖曳">拖曳</a></span></li></ul></div></div></div></div></li>');
		        
		        $node.appendTo($data_area);

		        var $Removebtn = $node.find('.remove');
		        $Removebtn.click(function () {

		            $node.remove();
		        })		        
		        return $node.children('.obj');
		    }

		    //status =is-success is-timeout is-failed  is-failed-Size is-failed-Type
		    //Jason 物件轉album
		    function build_albumByjson(Comm_RelFile) {
		        
		        var _Sort = Comm_RelFile.Sort;
		        var _sn = Comm_RelFile.SN;
		        var $node = $('<li><input type="hidden" id="hidSN" value="' + _sn + '"></input><input type="hidden" id="hidSort" value="' + _Sort + '"></input><div class="area-figure obj"><div class="inner"><div class="header"><div class="inner"><h4><span><a href="#">項目' + _sn + '</a></span></h4></div></div><div class="content"><div class="inner"><div class="list"><ul><li class="before"></li><li class="after"></li></ul></div><div class="image"><span id="img_span"><a target="_blank"  href="" id="src"><img  id="img_img"  alt=""></a></span></div><i class="mark"><div style="color: #c40;font-size: 1.4em;" id="msg" class="status"></div><span id="msg_span"></span></i><div class="caption" id="title" ></div><div class="paragraph"><p contenteditable id="ShortDesc"></p></div><div class="label"><ul><li><span id="file_size"></span></li></ul></div></div></div><div class="footer"><div class="inner"><ul><li class="remove"><span><a href="#" title="刪除此項目">刪除</a></span></li><li class="move"><span><a href="#" title="按住以拖曳">拖曳</a></span></li></ul></div></div></div></div></li>');
		        

		        //下載按鈕
		        //var $btnDownload = $('<li class="dowload"><span><a target="_blank" href="' + _src + '" title="下載項目">下載</a></span></li>');
		        //裁切按鈕
		        var $btnResize = $('<li class="resize"><span><a href="#" title="裁切圖片">裁切</a></span></li>');
		        //$node.find('.footer').find('ul').append($btnDownload);
		        if (ShowResize) {
		            $node.find('.footer').find('ul').append($btnResize);
		            $btnResize.click(function () {


		                OpenPhotoEdit(Comm_RelFile.PicName, _sn, NodeID);
		            })
		        }
		        var isrel = false;
		        $data_area.find('li').each(function () {
		            var sn = $(this).find('#hidSN').val() || 0;
		            if (sn == _sn) {
		                isrel = true;
		                $(this).html($node.html());
		                $node = $(this);
		            }
		        })
		        if (!isrel) {
		            $node.appendTo($data_area);
		        }

		        var $Removebtn = $node.find('.remove');
		        $Removebtn.click(function () {
		            $node.album().Deletebutton();
		        })
		        
		        var $ShortDesc = $node.find('#ShortDesc');
		        $ShortDesc.blur(function () {
		            console.log($ShortDesc.html());
		            $.ajax({
		                dataType: "text",
		                type: "POST",
		                url: APISPath + "&DataSN=" + _sn + "&cmd=update",
		                data: { ShortDesc: $ShortDesc.html() },
		                success: function (msg) {
		                    Reload();
		                }
		            });
		        });

		        $node.album().SaveData(Comm_RelFile);
		    }


            
		    function OpenPhotoEdit(SourceImgName, SourceSN, SystemModuleList) {
		          $.blockUI({ css: { border: 'none', 'background-color': '' }, message: '<img src="/images/ajax-loader.gif" />' });
                  
		          
		          var src = 'PhotoEdit/Default.aspx?SourceImgName=' + SourceImgName + '&SourceSN=' + SourceSN + '&NodeID=' + NodeID + "&uploadPath=" + UploadPath;
		          $.ajax({
		              dataType: "text",
		              type: "POST",
		              url: src,
		              success: function (msg) {
		                  $.unblockUI();
		                  var frmHtml = '<iframe style="width: 100%; height:100%;border:0px;" src="' + src + '"></iframe>';
		                  $.blockUI({
		                      css: { width: '96%', height: '96%', top: '2%', left: '2%', border: '' },
		                      message: frmHtml,
		                      baseZ: 2000
		                  });
		              }
		          });
                  
                  }
            function ClosePhotoEdit() {
                      $.unblockUI();
    
                  }
            function Refresh() {
                      location.reload();
              }

              $(function () { //上傳
                  $upload_area = $('.base-content');
                  $data_area = $upload_area.find('.group-list-obj ul');


                  $.event.props.push('dataTransfer');

                  $upload_area.on('dragenter', function (evt) {
                      evt.stopPropagation();
                      evt.preventDefault();
                  });

                  $upload_area.on('dragover', function (evt) {
                      evt.stopPropagation();
                      evt.preventDefault();
                  });


                  $upload_area.on('drop', function (evt) {
                      evt.stopPropagation();
                      evt.preventDefault();

                      $upload_area.removeClass('is-null');

                      var $file_array = evt.dataTransfer.files;
                      if (!MultipleUpload && $file_array.length > 1 ) {
                          alert("只可以上傳單一檔案");
                      }else{
                          UploadFileList($file_array);
                      }
                     

                  });

                  function UploadFileList($file_array) {
                      if (!MultipleUpload) {
                          Reload();
                          if ($upload_area.find('li').children('.obj').length >= 1) {
                              alert("只可以上傳單一檔案");
                              return false;
                          }
                      }
                      //status =is-success is-timeout is-failed  is-failed-Size is-failed-Type
					var $file_array_leng = $file_array.length;
                      for (i = 0; i < $file_array_leng; i++) {
                          var $file = $file_array[i];
                          var ext = "." + $file.name.split('.').pop();
                          var $obj = build_album($file); //建立並取回 dom
                          if ($.inArray(ext.toLowerCase(), FileFilterArray) == -1) { //如果類型不同
                              $obj.addClass('is-failed-Type');
                          } else {
                              if ($file.size > MaxFileSizeKB) {
                                  $obj.addClass('is-failed-Size');
                              } else {
                                  FilesSelected.push({ f: $file, dom: $obj });
                              }
                          }
                      }
                      if (!isLock) {
                          sendNext();
                      }
                  }
                  ///Jason 輪流上傳
                  var isLock = false;
                  function sendNext() {
                      if (!isLock && FilesSelected.length > 0) {
                          var $file = FilesSelected[0].f;
                          var $dom = FilesSelected[0].dom;
                          send_to_server($file, $dom);

                      } else {
                          
                      }
                  }

                  //Jason 先載入資料
                  $(function () {

                      if (!MultipleUpload){
                          $("#f").removeAttr("multiple");
                      }
                      $('.base-content').click(function (event) {                          
                          $("#f").click();
                      });
                      $("#f").change(function () {
                          UploadFileList(this.files);                   
                      });
                      Reload();

                      $('.base-content').on('click','.obj',function (event) {
                          event.stopPropagation();
                      });
                  });

                  //傳送至 HttpUploadHandler
                  function send_to_server(file, dom) {

                      isLock = true;
                      if (FilesSelected.length > 0) {
                          FilesSelected.splice(0, 1);
                      }

                      var data = new FormData(),
					  $percent_bar = dom.find('.mark span');
                      $percent_bar_status = dom.find('.mark .status');
                      $title = dom.find('.caption').html();
                      data.append(file.name, file);                      
                      $.ajax({
                          xhr: function () {
                              var xhr = new window.XMLHttpRequest();
                              xhr.upload.id = i;
                              xhr.upload.addEventListener("progress", function (evt) {

                                  if (evt.lengthComputable) {
                                      var _percentComplete = evt.loaded / evt.total;

                                      $percent_bar.css({
                                          width: _percentComplete * 100 + '%'
                                      });
                                      $percent_bar_status.html("上傳中...." + parseInt(_percentComplete * 100) + "%");
                                      SetInfo("上傳檔案 <font color='#ff0000'>[" + $title + "]</font> "+ parseInt(_percentComplete * 100) + "%");
                                      if (_percentComplete == 1) {
                                          $percent_bar_status.html("處理中....");
                                      }
                                      
                                  } 
                              }, false);

                              xhr.addEventListener("progress", function (evt) {

                                  if (evt.lengthComputable) {
                                     
                                  }
                              }, false);

                              return xhr;
                          },
                          type: 'POST',
                          url: HttpUploadHandlerPath + '&file=' + encodeURI(file.name),
                          contentType: false,
                          processData: false,
                          data: data,
                          success: function (result) {
                              dom.parent().remove();
                              var ResutList = JSON.parse(result);
                              $.each(ResutList, function (i, l) {                                  
                                  build_albumByjson(ResutList[i]);
                                  SetInfo("上傳檔案 <font color='#ff0000'>[" + $title + "]</font> 完成");
                                  if (ResutList[i].EBookUrl) {
                                      var url = (WebStorageURL + "/" + ResutList[i].EBookUrl);
                                      $.fancybox.open({ href: url, type: 'iframe', width: '100%' ,height:'100%' });
                                  }
                              });
                              isLock = false;
                              sendNext();
                          },
                          error: function (e) {
                              dom.addClass('is-failed');
                              isLock = false;
                              sendNext();
                          }
                      });
                  }
              });



		    var getUrlParameter = function getUrlParameter(sParam) {
		        var sPageURL = decodeURIComponent(window.location.search.substring(1)),
                sURLVariables = sPageURL.split('&'),
                sParameterName,
                i;

		        for (i = 0; i < sURLVariables.length; i++) {
		            sParameterName = sURLVariables[i].split('=');

		            if (sParameterName[0] === sParam) {
		                return sParameterName[1] === undefined ? true : sParameterName[1];
		            }
		        }
		    };

		    var $CheckConvert = (function () {
		        var _i = 0;
		        var Timer;
		        var _CheckData = [];
		        var _isRund = false;
		        var _AddCheck = function (Check) {
		            Check.SN = Check.album().SN;
		            var result = $.grep(_CheckData, function (e) { return e.SN == Check.SN; });
		            if (result.length == 0){
		                _CheckData.push(Check);
		            }
		           
		           
		            if (_CheckData.length == 1 && !_isRund) {
		                _Rund();
		            }
		        }
		        var _Rund = function () {
		            _isRund = true;
		            Timer = setInterval(myTimer, 1000);
		        };
		        var _Stop = function () {
		            
		            if (_isRund && _CheckData.length == 0) {
		                clearInterval(Timer);
		                _isRund = false;
		            }
		            _LogInfo();
		        };
		        var myTimer = function () {
		            _LogInfo();
		            $.each(_CheckData, function (k, dom) {		             		               
		                $.ajax({
		                    dataType: "json",
		                    type: "POST",
		                    url: APISPath + "&DataSN=" + dom.find('#hidSN').val() + "&cmd=GetRelFile",
		                    success: function (msg) {
		                        if (msg.oldSN == 0) {
		                            dom.album().SaveData(msg);
		                            _CheckData.splice(k, 1);
		                            _Stop();
		                        }

		                    }
		                });
		            })
		        }
		        var _LogInfo = function () {
		            SetInfo1("等待轉檔數量 <font color='#ff0000'>[" + _CheckData.length + "]</font>");
		        }
		        return {
		            Rund: _Rund,
		            Stop: _Stop,
		            AddCheck: _AddCheck,
		            CheckData: _CheckData
		        }
		    });
		    var oCheckConvert = new $CheckConvert();


		    var ajaxManager = (function () {
		        var requests = [];

		        return {
		            count: function () {
		                return requests.length;
		            },
		            addReq: function (opt) {
		                requests.push(opt);
		            },
		            removeReq: function (opt) {
		                if ($.inArray(opt, requests) > -1)
		                    requests.splice($.inArray(opt, requests), 1);
		            },
		            run: function () {
		                var self = this,
                            oriSuc;

		                if (requests.length) {
		                    oriSuc = requests[0].complete;

		                    requests[0].complete = function () {
		                        if (typeof (oriSuc) === 'function') oriSuc();
		                        requests.shift();
		                        self.run.apply(self, []);
		                    };

		                    $.ajax(requests[0]);
		                } else {
		                    self.tid = setTimeout(function () {
		                        self.run.apply(self, []);
		                    }, 1000);
		                }
		            },
		            stop: function () {
		                requests = [];
		                clearTimeout(this.tid);
		            }
		        };
		    }());
		    ajaxManager.run();
        </script>
        <script>
            $.fn.album = function () {
                var $that = $(this);
                var _SN = $that.find('#hidSN').val();
                var $Removebtn = $that.find('.remove');
                var $Msgbox = $that.find('.mark');
                var $title = $that.find('#title');
                var _ChangStatus = function (val) {
                    $that.find('.mark .status').html(val);
                }
                var _SaveData = function (Comm_RelFile) {
                    var _title = Comm_RelFile.SrcPicName || Comm_RelFile.SrcFileName,
                        _file_size = (Comm_RelFile.FileSize / 1000 + ' KB'),
				        _ShortDesc = Comm_RelFile.ShortDesc                       
                        isConver = false;
                    var _image_src = "";
                    var _src = "";
                    var statusClass = 'is-success';
                    if (uploadType == "RelPic") {
                        _image_src = WebStorageURL + Comm_RelFile.PicName;
                        _src = WebStorageURL + Comm_RelFile.PicName;
                        if (Comm_RelFile.PicName == '') {
                            statusClass = 'is-failed';
                        }
                    }
                    var msg = '';
                    if (uploadType == "RelFile") {
                        if (Comm_RelFile.oldSN == 1) {
                            isConver = true;
                            statusClass = 'is-converting';
                            msg = '轉檔中';
                        }

                        else if (Comm_RelFile.FileName == '' && Comm_RelFile.oldSN != 1) {
                            console.log('failed');
                            statusClass = 'is-failed';
                        } else {
                            _image_src = ContentPath + "/images/icons/Li_" + Comm_RelFile.FileName.split('.').pop() + ".gif";
                            _src = WebStorageURL + Comm_RelFile.FileName;
                        }

                    }
                   
                    $that.find('#msg').html(msg);
                    $that.find('#msg_span').html(msg);

                    $that.find('#ShortDesc').html(_ShortDesc);
                    $title.html(_title);
                    $that.find('#img_span').css('background-image', 'url(\'' + _image_src + '\')');
                    $that.find('#img_img').attr('src', _image_src);
                    $that.find('#src').attr('href', _src);
                    $that.find('#file_size').html(_file_size);
                    
                    //初始化UI
                    $that.find('.obj').removeClass('is-failed');
                    $that.find('.obj').removeClass('is-converting');
                    $that.find('.obj').removeClass('is-success');
                    $Removebtn.show();
                    $Msgbox.show();

                   
                  
                    $that.find('.obj').addClass(statusClass);
                    $Msgbox.hide();
                    if (isConver) {
                        //$Removebtn.hide();
                        oCheckConvert.AddCheck($that);
                    } else {
                       
                    }
                }
                var _Deletebutton = function () {
                    var r = confirm("確定刪除檔案?");
                    if (r == true) {
                        _Delete();
                    }
                }

                var _Delete = function (callback) {
                    ajaxManager.addReq({
                        dataType: "text",
                        type: "POST",
                        url: APISPath + "&DataSN=" + _SN + "&cmd=Delete",
                        success: function (msg) {
                            if (callback) callback();
                            Reload();
                        }
                    });
                }

                

                return {
                    ChangStatus: _ChangStatus,
                    SaveData: _SaveData,
                    Delete: _Delete,
                    Deletebutton: _Deletebutton,
                    Title: $title.html(),
                    SN:_SN
                }
            };

            function SetInfo(MSG) {
                $('#InfoMsg').html(MSG);
            }
            function SetInfo1(MSG) {
                $('#InfoMsg1').html(MSG);
            }

            function DeleteAllFile() {
                var r = confirm("確定刪除所有檔案?");
                if (r == true) {
                    var $list = $('.group-list-obj').find('.content').eq(0).find('ul').eq(0).children('li');                    
                    
                    $list.each(function () {
                        var oAlbum = $(this).album();
                        var callback = function () {
                            SetInfo('刪除檔案 [' + oAlbum.Title + "] 完成");
                        }
                        oAlbum.Delete(callback);
                    })
                }
               
            }
        </script>
	<!-- Script 結束 -->

	<!-- sys-root 拖曳根節點 -->
    <body><form>
        <input type="file" id="f" multiple style="display : none;"/>
		<div class="sys-root"><div class="inner">
			<div class="header"><div class="inner">
			</div></div>
			<div class="content"><div class="inner">

			<!-- base-wrapper 固定佈局 -->
				<div class="base-wrapper"><div class="inner">
					<div class="header"><div class="inner">
					</div></div>
					<div class="content"><div class="inner">

					<!-- base-content 主要內容 -->
						<div class="base-content is-null" id=""><div class="inner">
							<div class="header"><div class="inner">
							</div></div>
							<div class="content"><div class="inner">

							<!-- group-list-obj 物件列表 -->
								<div class="group-list-obj"><div class="inner">
									<div class="header"><div class="inner">
										<h3><span>圖片上傳</span></h3>
									</div></div>
                                    <asp:Literal ID="litTitle" runat="server"></asp:Literal>
                                    <asp:Literal ID="litSubTitle" runat="server"></asp:Literal>									
										<ul>
											
										</ul>
									</div></div>
								</div></div>
							<!-- group-list-obj 結束 -->

							</div></div>
						</div></div>
					<!-- base-content 結束 -->

					<!-- base-footer 頁尾資訊 -->
						<div class="base-footer"><div class="inner">
							<div class="header"><div class="inner">
							</div></div>
							<div class="content"><div class="inner">

							<!-- update-form 上傳表單 -->
								<div class="area-form update-form"><div class="inner">
									<div class="header"><div class="inner">
									</div></div>
									<div class="content"><div class="inner">
										<div class="form">
											<div class="fieldset add" style="display:none;">
												<div class="legend"><span>加入浮水印</span></div>
												<span class="checkbox"><input id="checkbox_01" type="checkbox"><label for="checkbox_01">所有圖檔加入浮水印</label></span>
											</div>
											<!-- <div class="fieldset select">
												<div class="legend"><span>浮水印選擇</span></div>
												<span class="radio"><input name="watermark" id="watermark_01" type="radio" checked><label for="watermark_01">預設</label></span>
												<span class="radio"><input name="watermark" id="watermark_02" type="radio"><label for="watermark_02">自訂</label></span>
												<span class="is-alert">(建議上傳去背圖檔 : *.gif;*.png)</span>
											</div>-->
											<div class="fieldset position">
												<div class="legend"><span>浮水印位置</span></div>
												<span class="radio"><input name="away" id="away_01" type="radio" value="1" data-pos="top right" checked><label for="away_01">右上角</label></span>
												<span class="radio"><input name="away" id="away_02" type="radio" value="2" data-pos="top left"><label for="away_02">左上角</label></span>
												<span class="radio"><input name="away" id="away_03" type="radio" value="0" data-pos="center"><label for="away_03">中間</label></span>
												<span class="radio"><input name="away" id="away_04" type="radio" value="3" data-pos="bottom left"><label for="away_04">左下角</label></span>
												<span class="radio"><input name="away" id="away_05" type="radio" value="4" data-pos="bottom right"><label for="away_05">右下角</label></span>
											</div>
											<div class="fieldset opacity">
												<div class="legend"><span>透明度</span></div>
												<input type="range" value="100" min="0" max="100" step="1">
                                                <input type="button" id="btn_addWaterMarking" value="加入浮水印" />
											</div>
										</div>
									</div></div>
								</div></div>
							<!-- update-form 結束 -->

							<!-- update-pic 上傳圖片
								<div class="area-figure update-pic"><div class="inner">
									<div class="header"><div class="inner">
									</div></div>
									<div class="content"><div class="inner">
										<div class="image">
											<span style="background-image: url('http://ccmsws19.hamastar.com.tw/001/administrator/235/relpic/6642/524/5d3bcbb6-54a2-4577-a034-fe3ecc61973e.jpg')"><img src="http://ccmsws19.hamastar.com.tw/001/administrator/235/relpic/6642/524/5d3bcbb6-54a2-4577-a034-fe3ecc61973e.jpg" alt=""></span>
										</div>
										<div class="image">
											<span style="background-image: url('http://ccmsws19.hamastar.com.tw/001/administrator/235/relpic/6642/524/5d3bcbb6-54a2-4577-a034-fe3ecc61973e.jpg')"><img src="http://ccmsws19.hamastar.com.tw/001/administrator/235/relpic/6642/524/5d3bcbb6-54a2-4577-a034-fe3ecc61973e.jpg" alt=""></span>
										</div>
									</div></div>
									<div class="footer"><div class="inner">
										<ul>
											<li class="remove"><span><a href="#" title="刪除浮水印">刪除</a></span></li>
										</ul>
									</div></div>
								</div></div>
                               -->

							</div></div>
						</div></div>
					<!-- base-footer 結束 -->

					</div></div>
				</div></div>
			<!-- base-wrapper 結束 -->

			</div></div>
		</div></div>
        <link href="../css/pop-up_bar.css" rel="stylesheet" type="text/css" />
        <div class="bar_root"><div class="inner">
	        <div class="content">
                <div class="info" >
                    <p id="InfoMsg"></p>
                    <p id="InfoMsg1"></p>
                </div>
                <div class="inner">
		        <ul>
                    <li><span class="link"><a  onclick="javascript:DeleteAllFile();">刪除所有檔案</a></span></li>
			        <li><span class="link"><a  onclick="top.$.fancybox.close();">關閉</a></span></li>
                    
		        </ul>
	        </div></div>
        </div></div>

        </form></body>
	<!-- sys-root 結束 -->
