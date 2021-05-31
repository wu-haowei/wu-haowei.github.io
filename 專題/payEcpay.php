try {
    $obj = new ECPay_AllInOne();
    $obj->ServiceURL  = Config::ECPAY_API_URL;
    $obj->HashKey     = Config::ECPAY_HASH_KEY;
    $obj->HashIV      = Config::ECPAY_HASH_IV;
    $obj->MerchantID  = Config::ECPAY_MERCHANT_ID;
    $obj->Send['ReturnURL'] = Config::ECPAY_CALLBACK_URL; //付款完成通知回傳的網址
    $obj->Send['MerchantTradeNo']   = $order_id;
    $obj->Send['MerchantTradeDate'] = date('Y/m/d H:i:s'); //交易時間
    $obj->Send['TotalAmount']       = (int)$order_total;   //交易金額
    $obj->Send['TradeDesc']         = "商店訂購商品，訂單編號：".$order_id;                
    $obj->Send['NeedExtraPaidInfo'] = 'Y'; //額外的付款資訊(消費者信用卡末四碼)
    if ($order_payment_option== 'credit_card') 
    { 
        // CREDIT CARD
        $obj->Send['OrderResultURL']    = $returnUrl;//付款完成導回平台的網址
        $obj->Send['ChoosePayment'] = ECPay_PaymentMethod::Credit;
    } else if ($order_payment_option == 'atm') {
        // ATM
        $ClientRedirectURL = Config::BASE_URL;
        $obj->SendExtend['ExpireDate'] = 7; // 最短 3 天 最長 60 天
        $obj->SendExtend['ClientRedirectURL'] = $ClientRedirectURL;
        $obj->Send['ChoosePayment'] = ECPay_PaymentMethod::ATM;
    } else {
        return null;
    }
    /*  當 付款方式 [ChoosePayment] 為 ALL 時，可隱藏不需要的付款方式，多筆請以井號分隔(#)。
        可用的參數值：
        Credit:信用卡
        WebATM:網路 ATM
        ATM:自動櫃員機
        CVS:超商代碼
        BARCODE:超商條碼 */
    $obj->Send['EncryptType'] = 1;

    //訂單的商品資料
    array_push($obj->Send['Items'], array(
        'Name'  => "商品名稱",
        'Price'  => (int)1000,
        'Currency'  => "元",
        'Quantity'  => (int) "1",
        'URL'  => ""));

    //產生訂單(auto submit至ECPay)
    $obj->CheckOut();
} catch (Exception $e) {
    echo $e->getMessage();
}