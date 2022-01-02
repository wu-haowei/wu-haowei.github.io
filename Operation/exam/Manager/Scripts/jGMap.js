var $jMap = (function(MapID, txtID, Modify) {
    var _modify = Modify || false;
    var _mode = ''; //Mark Polyline Polygon
    var _Places_markers = [];
    var _places;
    var _markers = {};
    var _polyline = new google.maps.Polyline();
    var _polygon = new google.maps.Polygon();
    var _infowindow = new google.maps.InfoWindow();
    var _Centerlatlng = new google.maps.LatLng(22.627883, 120.290602);
    var $map = new google.maps.Map(document.getElementById(MapID), {
        center: _Centerlatlng,
        zoom: 11,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    });
    if (txtID != '') {
        var input = document.getElementById(txtID);
        var searchBox = new google.maps.places.SearchBox(input);
        $map.controls[google.maps.ControlPosition.TOP_LEFT].push(input);
        $map.addListener('bounds_changed', function() {
            searchBox.setBounds($map.getBounds());
        });
        searchBox.addListener('places_changed', function() {
            _places = searchBox.getPlaces();
            console.log(_places);
            if (_places.length == 0) {
                return;
            }

            // Clear out the old markers.
            _ClearAllPlacesMarkers();

            // For each place, get the icon, name and location.
            var bounds = new google.maps.LatLngBounds();
            _places.forEach(function(place) {
                var icon = {
                    url: place.icon,
                    size: new google.maps.Size(71, 71),
                    origin: new google.maps.Point(0, 0),
                    anchor: new google.maps.Point(17, 34),
                    scaledSize: new google.maps.Size(25, 25)
                };

                // Create a marker for each place.
                var marker = new google.maps.Marker({
                    map: $map,
                    icon: icon,
                    title: place.name,
                    position: place.geometry.location,
                    description: place.formatted_address
                })

                marker.addListener('click', function() {
                    _SavePlaceInfowindow(place);
                    _infowindow.open($map, marker);

                });


                _Places_markers.push(marker);
                if (place.geometry.viewport) {
                    // Only geocodes have viewport.
                    bounds.union(place.geometry.viewport);
                } else {
                    bounds.extend(place.geometry.location);
                }
            });
            $map.fitBounds(bounds);
        });
    }
    // Bias the SearchBox results towards current map's viewport.

    $(document).on("click", '#ModifyMark', function() {
        var marker = _infowindow.anchor;
        var buttonstring = '';
        buttonstring += '<input id="SaveMark" value="儲存" type="button" />'
        var contentString = '<div id="content">' +
            '<div id="siteNotice">' +
            '</div>' +
            '<h1 id="firstHeading" class="firstHeading"><input type="test" id="title" value="' + marker.title + '" /></h1>' +
            '<div id="bodyContent">' +
            '<p><textarea type="test" id="description" style="margin: 0px; width: 350px; height: 220px" >' + marker.description.replace(/<br\s?\/?>/g, "\n") + '</textarea><p>' +
            buttonstring +
            '</div>' +
            '</div>';
        _infowindow.setContent(contentString);
        console.log(marker);
    });
    $(document).on("click", '#DeleteMark', function() {
        var marker = _infowindow.anchor;
        _DeleteMarker(marker);
    });
    $(document).on("click", '#Place_ToMark', function() {
        var marker = _infowindow.anchor;
        _AddMarker(marker);
        marker.setMap(null);
    });

    google.maps.event.addListener(_infowindow, 'domready', function() {
        var button;
        if (document.getElementById('SaveMark')) {
            button = document.getElementById('SaveMark');
            button.onclick = function() {
                // Get input value and call setMarkerTitle function
                var title = document.getElementById('title').value;
                var description = document.getElementById('description').value;
                var marker = _infowindow.anchor;
                marker.title = title;
                marker.description = description.replace(/(?:\r\n|\r|\n)/g, '<br />');
                _infowindow.close();
                new google.maps.event.trigger(marker, 'click');
            };
        }

    });
    // [START region_getplaces]
    // Listen for the event fired when the user selects a prediction and retrieve
    // more details for that place.




    var _GetMarksPositionArray = function() {
        var arr = [];
        for (var id in _markers) {
            arr.push(_markers[id].getPosition());
        }
        return arr;
    }

    var _ClearPoly = function() {
        _polyline.setMap(null);
        _polygon.setMap(null);
    }

    var _ChangeMode = function(mode) {
        _mode = mode;
        _BuildMode();
    }
    var _BuildMode = function() {
        _ClearPoly();
        if (_mode == 'Mark') {

        }
        if (_mode == 'Polyline') {
            _Marker2Polyline();
        }
        if (_mode == 'Polygon') {
            _Marker2Polygon();
        }
    }

    var _Marker2Polyline = function() {
        _ClearPoly();
        _polyline = new google.maps.Polyline({
            path: _GetMarksPositionArray(),
            strokeColor: '#000000',
            strokeOpacity: 1.0,
            strokeWeight: 3
        });
        _polyline.setMap($map);
    }

    var _Marker2Polygon = function Marker2Polygon() {
        _ClearPoly();
        _polygon = new google.maps.Polygon({
            paths: _GetMarksPositionArray(),
            strokeColor: '#FF0000',
            strokeOpacity: 0.8,
            strokeWeight: 3,
            fillColor: '#FF0000',
            fillOpacity: 0.35
        });
        _polygon.setMap($map);
    }
    var _ClearAllPlacesMarkers = function(oldmarker) {
        _Places_markers.forEach(function(marker) {
            marker.setMap(null);
        });
        _Places_markers = [];
    }
    var _ClearAllMarkers = function(oldmarker) {
        for (var id in _markers) {
            var mark = _markers[id];
            _DeleteMarker(mark);
        }
    }
    var _DeleteMarker = function(marker) {
        marker.setMap(null);
        delete _markers[marker.id];
        _BuildMode();
    }
    var _AddMarker = function(oldmarker) {
        var marker = new google.maps.Marker({
            map: $map,
            draggable: Modify,
            animation: google.maps.Animation.DROP,
            title: oldmarker.title,
            position: oldmarker.position,
            description: oldmarker.description,
            id: _guid()
        });
        _markers[marker.id] = marker; // cache marker in markers object
        marker.addListener('click', function() {
            _SaveInfowindow(marker);
            _infowindow.open($map, marker);

        });
        marker.addListener('drag', function() {
            _BuildMode();

        });
        _BuildMode();
    }

    function _SavePlaceInfowindow(place) {
        var img = '';
        if (place.hasOwnProperty('photos')) {
            var imgUrl = place.photos[0].getUrl({
                maxWidth: 250,
                maxHeight: 250
            });
            img = '<img id="img" src="' + imgUrl + '">';
        }
        var name = place.name;
        var buttonstring = '';
        if (_modify) {
            buttonstring = '<input id="Place_ToMark" value="加入" type="button" />';
        }
        var contentString = '<div id="content">' +
            '<div id="siteNotice">' +
            img +
            '</div>' +
            '<h1 id="firstHeading" class="firstHeading">' + name + '</h1>' +
            '<div id="bodyContent">' +
            '<p>' + place.formatted_address + '<p>' +
            buttonstring +
            '</div>' +
            '</div>';

        _infowindow.setContent(contentString);
    }

    function _SaveInfowindow(marker) {
        var buttonstring = '';
        if (_modify) {
            buttonstring += '<input id="ModifyMark" value="修改" type="button" />'
            buttonstring += '<input id="DeleteMark" value="刪除" type="button" />'
        }
        var contentString = '<div id="content">' +
            '<div id="siteNotice">' +
            '</div>' +
            '<h1 id="firstHeading" class="firstHeading">' + marker.title + '</h1>' +
            '<div id="bodyContent">' +
            '<p>' + marker.description + '<p>' +
            buttonstring +
            '</div>' +
            '</div>';
        _infowindow.setContent(contentString);
    }

    function _guid() {
        function s4() {
            return Math.floor((1 + Math.random()) * 0x10000)
                .toString(16)
                .substring(1);
        }
        return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
            s4() + '-' + s4() + s4() + s4();
    }

    //取得設定 包含目前Mode 與 Markers
    var _GetData = function() {
        var arr = [];
        for (var id in _markers) {
            var mark = _markers[id];
            arr.push({
                lat: mark.position.lat(),
                lng: mark.position.lng(),
                title: mark.title,
                description: mark.description
            });
        }
        return {
            mode: _mode,
            map: {
                lat: $map.getCenter().lat(),
                lng: $map.getCenter().lng(),
                zoom : $map.getZoom()
            },
            datas: arr
        };
    }

    //寫回設定
    var _SaveData = function(obj) {
        _ChangeMode(obj.mode);
        _SaveSaveMarks(obj.datas);
        $map.setCenter(new google.maps.LatLng(obj.map.lat, obj.map.lng));
        $map.setZoom(obj.map.zoom);
    }


    //加入 MarksArray 元Marks不清除
    var _SaveSaveMarks = function(arr) {
        if (arr.length == 0) return false;
        arr.forEach(function(marker) {
            marker.position = new google.maps.LatLng(marker.lat, marker.lng);
            _AddMarker(marker);
        });
    }

    return {
        map: $map,
        ChangeMode: _ChangeMode,
        Marks: _markers,
        GetData: _GetData,
        SaveData: _SaveData,
        SaveMarks: _SaveSaveMarks,
        ClearAllMarkers: _ClearAllMarkers,
        ClearAllPlacesMarkers: _ClearAllPlacesMarkers
    }

});
