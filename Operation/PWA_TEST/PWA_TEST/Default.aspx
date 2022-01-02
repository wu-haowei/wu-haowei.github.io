<!DOCTYPE html>
<html lang="zh-Hant-TW">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta http-equiv="X-UA-Compatible" content="ie=edge">
    <title>PWA</title>
    <link href="https://fonts.googleapis.com/css?family=Roboto:400,700" rel="stylesheet">
    <link rel="stylesheet" href="https://fonts.googleapis.com/icon?family=Material+Icons">
    <link rel="icon" sizes="48x48" href="favicon.ico">
    <link rel="manifest" href="manifest.json">
    <style>
        pre {
  white-space: pre-wrap;
  background-color: #EEEEEE;
  padding: 16px;
}
    </style>
</head>
<body>
    <img src="icon-512.png" />

      <main>
          <p>
              1.至Push Companion 產生Public Key(若通知有清除需重新設定) 放置 main.js的(211行)applicationServerPublicKey 後即可PUSH
          </p>
    <p>
      <button disabled class="js-push-btn mdl-button mdl-js-button mdl-button--raised mdl-js-ripple-effect">
        Enable Push Messaging
      </button>
    </p>
    <section class="subscription-details js-subscription-details is-invisible">

      <pre><code class="js-subscription-json"></code></pre>
        <a href="https://web-push-codelab.glitch.me/" target="_blank" >Push Companion</a>
    </section>
  </main>




    <script src="js/main.js"></script>
</body>
</html>