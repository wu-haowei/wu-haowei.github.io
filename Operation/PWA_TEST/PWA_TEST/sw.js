


//////↓↓↓↓↓預緩存↓↓↓↓↓
////var cacheName = 'hello'; //緩存名稱
////self.addEventListener('install', event => {//進入SW的安裝事件
////    console.log('[SW] 安裝(Install) Service Worker!', event);//安裝(Install)
////    event.waitUntil(
////        caches.open(cacheName)//使用指定的緩存名稱來打開緩存
////            .then(cache => cache.addAll([//把JS和hello.png添加到緩存中
////                '/src/js/appA.js',
////                '/icon-512.png',
////                '/icon-513.png',
////                'Default.aspx'

////            ]))
////    );
////});

//////利用fetch事件，讀取緩存。fetch事件會監聽URL請求，
//////如果在SW緩存中，就從SW中取；如果不在，就通過網絡從服務器中取。
////self.addEventListener('fetch', function (event) {
////    event.respondWith(
////        caches.match(event.request)//檢查傳入的請求URL是否匹配當前緩存中存在的任何內容
////            .then(function (response) {
////                if (response) { return response; }//SW有，則返回
////                return fetch(event.request);//SW沒有，通過網絡從服務器中取
////            })
////	);
////});
//////↑↑↑↑↑預緩存↑↑↑↑↑


////↓↓↓↓↓攔截並緩存/資源是靜態/動態↓↓↓↓↓


const cacheName = '暫存'; //離線緩存的名稱
const staticAssets = [//離線緩存的檔案
    '/src/js/main.js',
    '/icon-512.png',
    '/icon-513.png',
    'fallback.json',
    '/images/fetch-dog.jpg',
];
self.addEventListener('install', async function () {//進入Service Worker的安裝事件
    console.log('[SW] 安裝(Install) Service Worker!');
    const cache = await caches.open(cacheName);//使用指定的緩存名稱來打開緩存
    cache.addAll(staticAssets);
});

self.addEventListener('activate', event => {//功能性的需求(如:擺脫舊的緩存)
    console.log('[SW] 觸發(Activate) Service Worker!');
    event.waitUntil(self.clients.claim()); //使用Promise來知曉安裝所需的時間以及是否安裝成功。
});
//利用fetch事件，讀取緩存。fetch事件會監聽URL請求，
//如果在Service Worker緩存中，就從SW中取；如果不在，就通過網絡從服務器中取。
self.addEventListener('fetch', event => {
    const request = event.request;
    const url = new URL(request.url);
    if (url.origin === location.origin) {
        event.respondWith(cacheFirst(request));//緩存優先
    } else {
        event.respondWith(networkFirst(request));//用網路上
    }
});

async function cacheFirst(request) {
    const cachedResponse = await caches.match(request);//檢查是否有緩存
    return cachedResponse || fetch(request);//調用或使用網路上來保存
}

async function networkFirst(request) {
    const dynamicCache = await caches.open('動態暫存');
    try {
        const networkResponse = await fetch(request);//嘗試獲取網路
        dynamicCache.put(request, networkResponse.clone());//儲存起來
        return networkResponse;
    } catch (err) {
        const cachedResponse = await dynamicCache.match(request);//查看有無暫存資料
        return cachedResponse || await caches.match('./fallback.json');
    }
}

////↑↑↑↑↑攔截並緩存/資源是靜態/動態↑↑↑↑↑



//self.addEventListener('notificationclick', function (event) {
//    var notification = event.notification;
//    var action = event.action;

//    console.log(notification);
//    if (action === 'confirm') {
//        console.log('使用者點選確認');
//        notification.close();
//    } else {
//        console.log(action);
//    }
//});
//self.addEventListener('notificationclose', function (event) {
//    console.log('使用者沒興趣', event);
//});










//////↓↓↓↓↓接收通知↓↓↓↓↓
////self.addEventListener('push', function (event) {
////    //檢查服務器端是否發送了任何有效載荷數據
////    let payload = event.data ? JSON.parse(event.data.text()) : 'no payload';
////    let title = 'Progressive';
////    //使用提供的信息來顯示Web推送通知
////    event.waitUntil(
////        self.registration.showNotification(title, {
////            body: payload.msg,
////            url: payload.url,
////            icon: payload.icon
////        })
////    )
////})
//////↑↑↑↑↑接收通知SW↑↑↑↑↑

//////↓↓↓↓↓處理用戶與推送通知的交互↓↓↓↓↓
////self.addEventListener('notificationclick', function (event) {
////    event.notification.close();//一旦單擊了通知提示，便會關閉
////    //檢查當前窗口是否已經打開，如果打開，則切換至當前窗口
////    event.waitUntil(
////        clients.matchAll({
////            type: "window"
////        })
////            .then(function (clientList) {
////                for (let i = 0; i < clientList.length; i++) {
////                    let client = clientList[i];
////                    if (client.url == '/' && 'focus' in client)
////                        return client.focus();
////                }
////                if (clients.openWindow) {
////                    //單擊後開發URL
////                    return clients.openWindow('http://localhost:8081');
////                }
////            })
////    )
////})
////////↑↑↑↑↑處理用戶與推送通知的交互//↑↑↑↑↑

//////↓↓↓↓↓推播↓↓↓↓↓

//self.addEventListener('push', function (event) {
//    //檢查服務器端是否發送了任何有效載荷數據
//    let payload = event.data ? JSON.parse(event.data.text()) : 'no payload';
//    let title = '新訊息(推播)';
//    //使用提供的信息來顯示Web推送通知
//    event.waitUntil(
//        self.registration.showNotification(title, {
//            body: 'PWA(內文)',
//            click_action:'https://stackoverflow.com/',
//            //body: Body.value,
//            icon: 'https://picsum.photos/300/200/?random1',//網站icon圖示
//            image: 'https://picsum.photos/300/200/?random1',//內容圖片
//            dir: 'ltr',//文字顯示方向
//            lang: 'zh-Hant', //BCP 47//語言代碼
//            vibrate: [100, 50, 200],//裝置的振動模式 [100, 50, 200]指震動100ms，暫停50ms，再振動200ms
//            badge: 'https://picsum.photos/300/200/?random1',//顯示在狀態列的圖示
//            tag: 'confirm-notification',//通知的ID
//            renotify: true,//設定同一組通知更新後，是否要在通知使用者一次
//            actions: [//設定通知上的選項
//                { action: 'confirm', title: '確認', icon: 'https://picsum.photos/300/200/?random1' },
//                { action: 'cancel', title: '取消', icon: 'https://picsum.photos/300/200/?random1' }
//            ],
//            vibrate: [300, 100, 400] //振動300ms，暫停100ms，再暫停400ms
//        })

//    )
//})





/////////////////////////////test////////////////////


/*
*
*  Push Notifications codelab
*  Copyright 2015 Google Inc. All rights reserved.
*
*  Licensed under the Apache License, Version 2.0 (the "License");
*  you may not use this file except in compliance with the License.
*  You may obtain a copy of the License at
*
*      https://www.apache.org/licenses/LICENSE-2.0
*
*  Unless required by applicable law or agreed to in writing, software
*  distributed under the License is distributed on an "AS IS" BASIS,
*  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
*  See the License for the specific language governing permissions and
*  limitations under the License
*
*/

/* eslint-env browser, serviceworker, es6 */

'use strict';

self.addEventListener('push', function (event) {
    console.log('[Service Worker] 收到推送。');
    console.log(`[Service Worker] Push 推播資料："${event.data.text()}"`);

    const title = '推播標題';
    const options = {
        body: event.data.text(),
        icon: 'https://picsum.photos/300/200/?random1',
        badge: 'https://picsum.photos/300/200/?random1',
        data: {
            link: 'https://www.google.com.tw/',
            link_ok: 'https://developers.google.com/web/fundamentals/codelabs/payment-request-api',
            link_ng: 'https://tw.yahoo.com/'
        },
        requireInteraction: true,
        actions: [//設定通知上的選項
            { action: 'confirm', title: '確認', icon: 'https://picsum.photos/300/200/?random1' },
            { action: 'cancel', title: '取消', icon: 'https://picsum.photos/300/200/?random1' }
        ]
    };

    event.waitUntil(self.registration.showNotification(title, options));
});

self.addEventListener('notificationclick', function (event) {
    console.log('[Service Worker] 通知點擊收到');

    const notification = event.notification;
    const action = event.action;
    const link = notification.data.link;
    const link_ok = notification.data.link_ok;
    const link_ng = notification.data.link_ng;
    console.log(action);

    switch (action) {
        case 'confirm':
            clients.openWindow(link_ok);
            break;
        case 'cancel':
            clients.openWindow(link_ng);
            break;
        default:
            if (link) {
                clients.openWindow(link);
            }
            break;
    }
    notification.close();
    console.log('notificationclick action is', action);
});

