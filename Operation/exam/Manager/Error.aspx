﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Error.aspx.cs" Inherits="Error" %>

<!DOCTYPE html>

<html lang="zh-TW">
<head>
    <title>
        <asp:Literal ID="Literal2" runat="server"></asp:Literal></title>
    <style>
        html {
            position: fixed;
            width: 100%;
            height: 100%;
        }

        body {
            position: absolute;
            top: 0;
            right: 0;
            bottom: 0;
            left: 0;
            background-color: #f7ef83;
            background-image: linear-gradient(to bottom, #fdfdfd, #dfdfdf);
        }

        .box {
            width: 80%;
            max-width: 600px;
            min-width: 300px;
            padding: 30px;
            margin: 40px auto 0;
            border: 1px solid #ddd;
            background-color: #f7ef83;
            box-shadow: 0 0 20px #ddd;
            border-radius: 4px;
        }

        .inner {
        }

        h1 {
            padding-bottom: 10px;
            margin-top: 0;
            margin-bottom: 6px;
            border-bottom: 1px solid #333;
        }

        p {
            margin-bottom: 0;
            font-family: '微軟正黑體', Arial, 'Helvetica Neue', Helvetica, sans-serif, '新細明體';
            line-height: 1.5;
        }

        a {
            display: inline-block;
            padding: 3px 6px;
            margin: 0 4px;
            text-decoration: none;
            color: #fff;
            background-color: #bd362f;
            background-image: linear-gradient(to bottom, #ee5f5b, #bd362f);
            border-radius: 4px;
            font-size: .8em;
        }

            a:hover {
                background-image: none;
            }
    </style>
</head>
<body>

    <div class="box">
        <div class="inner">
            <h1>系統發生錯誤
            </h1>
            <p>
                抱歉！請您再重新檢視輸入的網址是否正確。<br />
                您也可以回到<a href="javascript:history.back();">上一頁</a>或<a href="login.aspx">回登入頁</a>。
            </p>


        </div>
    </div>

</body>
</html>
