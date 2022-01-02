//if ('serviceWorker' in navigator) {//透過 'serviceWorker' in navigator 去判斷瀏覽器是否有支援 Service Worker
//    window.addEventListener('load', function () {
//        navigator.serviceWorker.register('/service-worker.js')
//            //有支援、則使用 service-Worker.register 方法傳入 Service Worker 檔案（service-worker.js'）並進行註冊。
//            .then(function () {
//                console.log('Service Worker 註冊成功');
//            }).catch(function (error) {
//                console.log('Service worker 註冊失敗:', error);
//            });
//    })
//} else { console.log('瀏覽器不支援') };

//var enableNotifications = document.querySelectorAll('.enable-notifications');
//if ('Notification' in window) {
//    for (var i = 0; i < enableNotifications.length; i++) {
//        enableNotifications[i].style.display = 'inline-block';
//        enableNotifications[i].addEventListener('click', askForNotificationPermission);
//    }
//}

//function askForNotificationPermission() {
//    Notification.requestPermission(function (status) {
//        console.log('User Choice', status);
//        if (status !== 'granted') {
//            console.log('推播允許被拒絕了!');
//            //允許 屬性為 granted 拒絕denied
//        } else {
//            displayNotification();
//            console.log('允許推播!');
//        }
//    });
//}
////取消訂閱部分
//var noenableNotifications = document.querySelector('.noenable-notifications');//取消訂閱按鈕
//noenableNotifications.addEventListener('click', function () {
//    unsubscribeUser()
//    //if (isSubscribed) {
//    //    unsubscribeUser();
//    //} else {
//    //    displayNotification();
//    //}
//});


//function unsubscribeUser() {
//    swRegistration.pushManager.getSubscription()
//        .then(function (subscription) {
//            if (subscription) {
//                return subscription.unsubscribe();
//            }
//        })
//        .catch(function (error) {
//            console.log('Error unsubscribing', error);
//        })
//        .then(function () {
//            updateSubscriptionOnServer(null);

//            console.log('User is unsubscribed.');
//            isSubscribed = false;

//            //updateBtn();
//        });
//}

////取消訂閱部分



////↓↓↓↓↓下載安裝提示↓↓↓↓↓
//let installPromptEvent;
//window.addEventListener('beforeinstallprompt', event => {
//    // we prevent a banner or infobar to appear
//    event.preventDefault();
//    // we save the event for later usage
//    installPromptEvent = event
//});
//const installAppButton = document.getElementById("btnInstall");
//installAppButton.addEventListener('click', event => {
//    if (installPromptEvent) {
//        installPromptEvent.prompt();
//    } else {
//        // Prompt not available: Display fallback instructions
//    }
//})
////↑↑↑↑↑下載安裝提示↑↑↑↑↑

//function displayNotification() {
//    if ('serviceWorker' in navigator) {
//        var options = {
//            body: 'PWA(內文)',
//            icon: 'https://picsum.photos/300/200/?random1',//網站icon圖示
//            image: 'https://picsum.photos/300/200/?random1',//內容圖片
//            dir: 'ltr',//文字顯示方向
//            lang: 'zh-Hant', //語言
//            vibrate: [100, 50, 200],//裝置的振動模式 [100, 50, 200]指震動100ms，暫停50ms，再振動200ms
//            badge: 'https://picsum.photos/300/200/?random1',//顯示在狀態列的圖示
//            tag: 'confirm-notification',//通知的ID
//            renotify: true,//設定同一組通知更新後，是否要在通知使用者一次
//            actions: [//設定通知上的選項
//                { action: 'confirm', title: '確認', icon: 'https://picsum.photos/300/200/?random1' },
//                { action: 'cancel', title: '取消', icon: 'https://picsum.photos/300/200/?random1' }
//            ]
//        };
//        navigator.serviceWorker.ready
//            .then(function (sw) {
//                sw.showNotification('訂閱成功(標題)！！！', options);
//                console.log(sw)
//                reg = sw;
//                return sw.pushManager.getSubscription();
//            })
//            .then(function (sub) {
//                if (sub === null) {
//                    //建立新的訂閱
//                    var vapidPKey = 'BJnxuvg8nppTG89CByCDyf5gCmlsC-vRW97SEdhCCEvEE2wM6RUvI5_8O09CCR0yIJFG2a1oLaVpWN6e5ChGi3k';
//                    var convertedVapidPKey = urlBase64ToUint8Array(vapidPKey);
//                    return reg.pushManager.subscribe({
//                        userVisibleOnly: true,
//                        applicationServerKey: convertedVapidPKey
//                    }).then(function (newSub) {
//                            fetch('api/Api_PWA/Post/', {
//                                method: 'POST',
//                                body: JSON.stringify(newSub),
//                                headers: {
//                                    'Content-Type': 'application/json',
//                                    'Accept': 'application/json'
//                                },
//                            })
//                            console.warn(JSON.stringify(newSub));
//                            console.log('新增金鑰成功');
//                        })
//                        .catch(function (err) {
//                            console.log('訂閱失敗', err);
//                        });
//                } else {
//                    console.log('用戶已訂閱');
//                }
//            }
//        )
//    }
//}



//function urlBase64ToUint8Array(base64String) {
//    var padding = '='.repeat((4 - base64String.length % 4) % 4);
//    var base64 = (base64String + padding)
//        .replace(/\-/g, '+')
//        .replace(/_/g, '/');
//    var rawData = window.atob(base64);
//    var outputArr = new Uint8Array(rawData.length);

//    for (var i = 0; i < rawData.length; ++i) {
//        outputArr[i] = rawData.charCodeAt(i);
//    }
//    return outputArr;
//}



//function AAAA() {

//    self.addEventListener('notificationclick', function (event) {
//        var notification = event.notification;
//        var action = event.action;

//        console.log(notification);
//        if (action === 'confirm') {
//            console.log('使用者點選確認');
//            notification.close();
//        } else {
//            console.log(action);
//        }
//    });
//    self.addEventListener('notificationclose', function (event) {
//        console.log('使用者沒興趣', event);
//    });
//}




////////////////////////////test///////////////////////////////

///////////////////////////////////////////////////////////////////developers///



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

/* eslint-env browser, es6 */

'use strict';

const applicationServerPublicKey = 'BCi4n4X3llgTReE0qPBUiDxLn_IHf_9frv08WgTVJiU7Us1GhwtsOMGWmMcBLyBji8KpmO8H8735zZYuwiv40Ac';

const pushButton = document.querySelector('.js-push-btn');

let isSubscribed = false;
let swRegistration = null;

function urlB64ToUint8Array(base64String) {
    const padding = '='.repeat((4 - base64String.length % 4) % 4);
    const base64 = (base64String + padding)
        .replace(/\-/g, '+')
        .replace(/_/g, '/');

    const rawData = window.atob(base64);
    const outputArray = new Uint8Array(rawData.length);

    for (let i = 0; i < rawData.length; ++i) {
        outputArray[i] = rawData.charCodeAt(i);
    }
    return outputArray;
}

if ('serviceWorker' in navigator && 'PushManager' in window) {
    console.log('Service Worker and Push is supported');

    navigator.serviceWorker.register('sw.js')
        .then(function (swReg) {
            console.log('Service Worker 已註冊', swReg);

            swRegistration = swReg;
        })
        .catch(function (error) {
            console.error('Service Worker 錯誤', error);
        });
} else {
    console.warn('不支持推送消息');
    pushButton.textContent = 'Push Not Supported';
}

// function initialiseUI() {
//   // Set the initial subscription value
//   swRegistration.pushManager.getSubscription()
//   .then(function(subscription) {
//     isSubscribed = !(subscription === null);

//     if (isSubscribed) {
//       console.log('User IS subscribed.');
//     } else {
//       console.log('User is NOT subscribed.');
//     }

//     updateBtn();
//   });
// }
function initialiseUI() {
    pushButton.addEventListener('click', function () {
        pushButton.disabled = true;
        if (isSubscribed) {
            unsubscribeUser();
        } else {
            subscribeUser();
        }
    });

    // Set the initial subscription value
    swRegistration.pushManager.getSubscription()
        .then(function (subscription) {
            isSubscribed = !(subscription === null);

            updateSubscriptionOnServer(subscription);

            if (isSubscribed) {
                console.log('用戶已訂閱。');
            } else {
                console.log('用戶未訂閱。');
            }

            updateBtn();
        });
}

// function updateBtn() {
//   if (isSubscribed) {
//     pushButton.textContent = 'Disable Push Messaging';
//   } else {
//     pushButton.textContent = 'Enable Push Messaging';
//   }

//   pushButton.disabled = false;
// }
var _swReg;
function updateBtn() {
    if (Notification.permission === 'denied') {
        pushButton.textContent = 'Push Messaging Blocked.';
        pushButton.disabled = true;
        updateSubscriptionOnServer(null);
        return;
    }

    if (isSubscribed) {
        pushButton.textContent = '取消訂閱';
    } else {
        pushButton.textContent = '訂閱';
    }

    pushButton.disabled = false;
}

navigator.serviceWorker.register('sw.js')
    .then(function (swReg) {
        console.log('Service Worker is registered', swReg);

        swRegistration = swReg;
        initialiseUI();
    })

function subscribeUser() {
    const applicationServerKey = urlB64ToUint8Array(applicationServerPublicKey);
    swRegistration.pushManager.subscribe({
        userVisibleOnly: true,
        applicationServerKey: applicationServerKey
    })
        //訂閱API
        .then(function (subscription) {
            fetch('api/Api_PWA/Add_Post/', {
                method: 'POST',
                body: JSON.stringify(subscription),
                headers: {
                    'Content-Type': 'application/json',
                    'Accept': 'application/json'
                },
            })
            console.log('User is subscribed:', subscription);
            _swReg = subscription
            updateSubscriptionOnServer(subscription);

            isSubscribed = true;

            updateBtn();
            console.log('新增金鑰成功');
        })
        .catch(function (err) {
            console.log('訂閱用戶失敗：', err);
            updateBtn();
        });
}

swRegistration.pushManager.subscribe({
    userVisibleOnly: true,
    applicationServerKey: applicationServerKey
})
    .then(function (subscription) {
        console.log('用戶已訂閱：', subscription);

        updateSubscriptionOnServer(subscription);

        isSubscribed = true;

        updateBtn();

    })
    .catch(function (err) {
        console.log('訂閱用戶失敗：', err);
        updateBtn();
    });
function updateSubscriptionOnServer(subscription) {
    // TODO: Send subscription to application server

    const subscriptionJson = document.querySelector('.js-subscription-json');
    const subscriptionDetails =
        document.querySelector('.js-subscription-details');


    if (subscription) {
        subscriptionJson.textContent = JSON.stringify(subscription);
        subscriptionDetails.classList.remove('is-invisible');
    } else {
        subscriptionDetails.classList.add('is-invisible');
    }
}



//取消訂閱用戶
function unsubscribeUser() {
    swRegistration.pushManager.getSubscription()
        .then(function (subscription) {      ///取消訂閱API
            if (subscription) {

                fetch('api/Api_PWA/POST_Delete/', {
                method: 'POST',
                body: JSON.stringify(_swReg),
                headers: {
                    'Content-Type': 'application/json',
                    'Accept': 'application/json'
                },
            })
            console.log('用戶已退訂', _swReg);


                return subscription.unsubscribe();
            }
        })
        .catch(function (error) {
            console.log('取消訂閱時出錯：', error);
        })
        .then(function () {
            updateSubscriptionOnServer(null);
            isSubscribed = false;
            let subscriptionJson = document.querySelector('.js-subscription-json');
            subscriptionJson.textContent = '用戶已退訂';
            updateBtn();
        });
}


//取消訂閱用戶