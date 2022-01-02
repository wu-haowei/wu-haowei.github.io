using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

//Use by AD 
using System.DirectoryServices; //[加入參考] 對話方塊的 [.NET] 索引標籤，按一下 System.DirectoryServices.dll]
using System.Security.Principal;
using System.Web.Security;

namespace Hamastar.Common.LADP
{
    /// <summary>
    /// LdapAuthentication 的摘要描述
    //string ADServerIP = "", ADUser = "", Domain = "", ADPassword = "";
    //ADServerIP = System.Configuration.ConfigurationSettings.AppSettings["ADServerIP"].ToString();
    //ADUser = System.Configuration.ConfigurationSettings.AppSettings["ADAdminUser"].ToString();
    //Domain = System.Configuration.ConfigurationSettings.AppSettings["Domain"].ToString();
    //ADPassword = System.Configuration.ConfigurationSettings.AppSettings["ADAdminPassword"].ToString();

    //Hamastar.Common.LADP.LdapAuthentication LDAP = new Hamastar.Common.LADP.LdapAuthentication(ADServerIP, ADUser, Domain, ADPassword);
    //txtUserProperty.Text = LDAP.FindUser(txtAccount.Text);
    /// </summary>
    public class LdapAuthentication
    {
        private string ADServerIP = "";      // = System.Configuration.ConfigurationSettings.AppSettings〔"ADServer"〕.ToString();
        private string ADUser = "";          // = System.Configuration.ConfigurationSettings.AppSettings〔"ADAdminUser"〕.ToString();
        private string ADPassword = "";      // = System.Configuration.ConfigurationSettings.AppSettings〔"ADAdminPassword"〕.ToString();
        private string Domain = "";          // = System.Configuration.ConfigurationSettings.AppSettings〔"Domain"〕.ToString();
        private string DC = "";
        private string OU = "";
        private string strLDAP = "";
        private string _filterAttribute = "";
        //private ArrayList PropertiesList = null;

        public LdapAuthentication(string _ADServerIP, string _ADUser, string _Domain, string _ADPassword)
        {
            this.ADServerIP = _ADServerIP;
            this.ADUser = _ADUser;
            this.Domain = _Domain;
            this.ADPassword = _ADPassword;
            this.DC = GetDomainName(Domain);//domain
            this.strLDAP = "LDAP://" + _ADServerIP + "/" + DC; //指向最上層
        }

        private string GetDomainName(string domain)
        {
            string[] SplitStr = null;
            string DomainName = "";
            //Domain
            if (domain.Contains("."))
            {
                SplitStr = domain.Split('.');
                foreach (string item in SplitStr)
                {
                    if (DomainName == "") DomainName += "DC=" + item;
                    else DomainName += "," + "DC=" + item;
                }
            }
            else DomainName = "DC=" + domain;

            return DomainName;
        }

        #region AD 部門目錄 "ou="
        public string SetOU(string Department)
        {
            string MyOU = GetOU(Department);
            return MyOU;
        }
        //取得OU字串
        private string GetOU(string Department)
        {
            Boolean IsSetOU = true;
            string[] SplitStr = null;
            string strRult = "";
            if (Department.Contains("/"))
            {
                SplitStr = Department.Split('/');
                //從後面往前組合
                foreach (string item in SplitStr)
                {
                    if (strRult == "" && IsSetOU) strRult = "OU=" + item.Trim();
                    else strRult = "OU=" + item.Trim() + "," + strRult;

                    if (!CheckOU(strRult)) { if (CreateOU(strRult) == 0) IsSetOU = false; }
                }
            }
            else
            {
                strRult = "OU=" + Department.Trim();
                if (!CheckOU(strRult)) { if (CreateOU(strRult) == 0) IsSetOU = false; }
            }
            if (IsSetOU) return strRult;
            else return "0";
        }
        //檢查OU是否存在
        private bool CheckOU(string DeptName)
        {
            try
            {
                DirectoryEntry de = new DirectoryEntry(strLDAP, ADUser, ADPassword);
                DirectorySearcher ds = new DirectorySearcher(de);
                DirectoryEntry OU = de.Children.Find(DeptName, "organizationalUnit");
                if (OU == null) return false;
                de.Close();
            }
            catch (System.DirectoryServices.DirectoryServicesCOMException ex)
            {
                string err = ex.ToString();
                //throw new Exception(ex.Message);
                return false;
            }
            return true;
        }
        //建立OU目錄
        private int CreateOU(string DeptName)
        {
            try
            {
                DirectoryEntry de = new DirectoryEntry(strLDAP, ADUser, ADPassword);
                DirectorySearcher ds = new DirectorySearcher(de);
                DirectoryEntry OU = de.Children.Add(DeptName, "organizationalUnit");
                OU.CommitChanges();
                de.Close();
                return 1;
            }
            catch (System.DirectoryServices.DirectoryServicesCOMException ex)
            {
                string Msg = ex.Message;
                //Msg += "<br>" + ex.StackTrace.Replace(" ", "").ToString();
                //throw new Exception(Msg);
                return 0;
            }
        }
        //刪除OU目錄
        public int DeleteOU(string DeptName)
        {
            try
            {
                DirectoryEntry de = new DirectoryEntry(strLDAP, ADUser, ADPassword);
                DirectoryEntries objDentry = de.Children;
                OU = SetOU(DeptName);
                DirectoryEntry NewOUEntry = objDentry.Find(OU);
                NewOUEntry.DeleteTree();
                NewOUEntry.CommitChanges();
                de.Close();
                return 1;
            }
            catch (System.DirectoryServices.DirectoryServicesCOMException ex)
            {
                string Msg = ex.Message;
                //Msg += "<br>" + ex.StackTrace.Replace(" ", "").ToString();
                //throw new Exception(Msg);
                return 0;
            }
        }
        #endregion

        #region 驗證帳號
        public int IsAuthenticated(string account, string pwd)
        {
            int vchk = 1;//0:未通過認證 1:通過認證, 2:AD主機發生問題,3:無此帳號
            DirectoryEntry de_Admin = new DirectoryEntry(strLDAP, ADUser, ADPassword);
            try
            {
                DirectorySearcher ds = new DirectorySearcher(de_Admin);
                ds.Filter = "(SAMAccountName=" + account + ")";
                SearchResult sr = ds.FindOne();
                if (sr == null) vchk = 3;
                else
                {
                    DirectoryEntry de = new DirectoryEntry(strLDAP, account, pwd);
                    try
                    {
                        DirectorySearcher search = new DirectorySearcher(de);
                        search.Filter = "(samAccountName=" + account + ")";
                        search.PropertiesToLoad.Add("cn");
                        SearchResult result = search.FindOne();

                        if (null == result) vchk = 0;
                        _filterAttribute = (String)result.Properties["cn"][0];
                    }
                    catch (Exception ex)
                    {
                        vchk = 0;
                        string Msg = ex.Message;
                        //Msg += "<br>" + ex.StackTrace.Replace(" ", "").ToString();
                        //throw new Exception(Msg);
                    }
                    de.Close();
                }
            }
            catch (Exception ex)
            {
                vchk = 2;
                string Msg = ex.Message;
                //Msg += "<br>" + ex.StackTrace.Replace(" ", "").ToString();
                //throw new Exception(Msg);
            }

            return vchk;
        }
        #endregion

        #region 列出帳號屬性
        public string GetUserProperties(string UserID, string Properties)
        {
            DirectoryEntry de = new DirectoryEntry(strLDAP, ADUser, ADPassword);
            string value = string.Empty;
            System.Text.StringBuilder sbValue = new System.Text.StringBuilder();
            try
            {
                //1.定義 DirectoryEntry
                //2.定義 DirectorySearcher
                using (DirectorySearcher ds = new DirectorySearcher(de))
                {
                    //3.定義查詢
                    ds.Filter = "(SAMAccountName=" + UserID + ")";
                    ds.PropertiesToLoad.Add(Properties);
                    string adspath = "";
                    //找一筆
                    SearchResult sr = ds.FindOne();
                    if (sr != null)
                    {
                        foreach (string key in sr.Properties.PropertyNames)
                        {
                            foreach (Object propValue in sr.Properties[key])
                            {
                                if (key.ToLower() == "objectsid")//SID
                                {
                                    byte[] SIDs = propValue as byte[];
                                    SecurityIdentifier si = new SecurityIdentifier(SIDs, 0);
                                    value = key + " = " + si.ToString();
                                    sbValue.Append(key + " = " + propValue);
                                }
                                else
                                {
                                    if (key != "adspath")
                                    {
                                        adspath = propValue.ToString();//取出該帳號的問句
                                        if (key != "accountexpires")
                                            value = key + " = " + propValue + Environment.NewLine;
                                        else
                                            value = propValue + Environment.NewLine;
                                        sbValue.Append(value);
                                    }
                                    else if (key == "adspath" && Properties == "adspath")
                                    {
                                        sbValue.Append(propValue);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        sbValue.Append("User not found");
                    }
                }
            }
            catch (Exception ex)
            {
                string Msg = ex.Message;
                //Msg += "<br>" + ex.StackTrace.Replace(" ", "").ToString();
                sbValue.Append(Msg);
            }
            de.Close();
            return sbValue.ToString();
        }
        public string FindUser(string UserID)
        {
            DirectoryEntry de = new DirectoryEntry(strLDAP, ADUser, ADPassword);
            string value = string.Empty;
            System.Text.StringBuilder sbValue = new System.Text.StringBuilder();
            try
            {
                //1.定義 DirectoryEntry
                //2.定義 DirectorySearcher
                using (DirectorySearcher ds = new DirectorySearcher(de))
                {
                    //3.定義查詢
                    ds.Filter = "(SAMAccountName=" + UserID + ")";
                    ds.PropertiesToLoad.Add("SAMAccountName");//account
                    ds.PropertiesToLoad.Add("Name");//full name
                    ds.PropertiesToLoad.Add("displayName");
                    ds.PropertiesToLoad.Add("mail");
                    ds.PropertiesToLoad.Add("description");
                    ds.PropertiesToLoad.Add("phsicalDeliveryOfficeName");
                    ds.PropertiesToLoad.Add("department");
                    ds.PropertiesToLoad.Add("userPrincipalName");//user logon name,xxx@cccc.com
                    ds.PropertiesToLoad.Add("telephoneNumber");
                    ds.PropertiesToLoad.Add("givenName");//first name
                    ds.PropertiesToLoad.Add("objectsid");//SID
                    ds.PropertiesToLoad.Add("memberOf");// 群組
                    ds.PropertiesToLoad.Add("userWrokstations");//登入權限
                    string adspath = "";
                    //找一筆
                    SearchResult sr = ds.FindOne();
                    if (sr != null)
                    {
                        foreach (string key in sr.Properties.PropertyNames)
                        {
                            foreach (Object propValue in sr.Properties[key])
                            {
                                if (key.ToLower() == "objectsid")//SID
                                {
                                    byte[] SIDs = propValue as byte[];
                                    SecurityIdentifier si = new SecurityIdentifier(SIDs, 0);
                                    value = key + " = " + si.ToString();
                                    sbValue.Append(key + " = " + propValue);
                                }
                                else
                                {
                                    if (key == "adspath")
                                    {
                                        adspath = propValue.ToString();//取出該帳號的問句
                                    }
                                    value = key + " = " + propValue + Environment.NewLine;
                                    sbValue.Append(value);
                                }
                            }
                        }
                    }
                    else
                    {
                        sbValue.Append("User not found");
                    }
                }
            }
            catch (Exception ex)
            {
                string Msg = ex.Message;
                //Msg += "<br>" + ex.StackTrace.Replace(" ", "").ToString();
                sbValue.Append(Msg);
            }
            de.Close();
            return sbValue.ToString();
        }
        #endregion

        #region 自動新增or修改帳號
        public int Append_ADUser(string userPrincipalName, string Pwd, string Givenname, string Sn, string DisplayName, string physicalDeliveryOfficeName, string telephoneNumber, string mail, string title, string department, string company, string manager, string mobile, string facsimileTelephoneNumber, string ipPhone, string description, bool IsAccountExpires, DateTime accountExpires, bool userAccountControl)
        {
            if (userPrincipalName == "") return 0; //防止空白帳號
            #region 檢查OU是否存在
            string[] SplitStr = null; bool chkOU = true; string userOU = department;
            string strRult = "";
            SplitStr = department.Split('/');
            foreach (string item in SplitStr)
            {
                if (strRult == "") strRult = "OU=" + item.Trim();
                else strRult = "OU=" + item.Trim() + "," + strRult;

                if (!CheckOU(strRult)) chkOU = false;
            }
            if (!chkOU) { userOU = "暫存帳號部門"; userAccountControl = false; }
            #endregion
            int vRult = 1;
            DirectoryEntry de = new DirectoryEntry(strLDAP, ADUser, ADPassword);
            using (DirectorySearcher ds = new DirectorySearcher(de))
            {
                ds.Filter = "(SAMAccountName=" + userPrincipalName + ")";
                SearchResult sr = ds.FindOne();
                if (sr != null)
                    vRult = Edit_ADUser(userOU.Trim(), userPrincipalName, Pwd.Trim(), Givenname, Sn, DisplayName, physicalDeliveryOfficeName, telephoneNumber, mail.Trim(), title, department, company, manager, mobile, facsimileTelephoneNumber, ipPhone, description, IsAccountExpires, accountExpires, userAccountControl);
                else
                {
                    vRult = Add_ADUser(userOU.Trim(), userPrincipalName, Pwd.Trim(), Givenname, Sn, DisplayName, physicalDeliveryOfficeName, telephoneNumber, mail.Trim(), title, department, company, manager, mobile, facsimileTelephoneNumber, ipPhone, description, IsAccountExpires, accountExpires, userAccountControl);
                }
            }
            de.Close();

            return vRult;
        }
        #endregion

        #region 新增帳號
        public int Add_ADUser(string userOU, string userPrincipalName, string Pwd, string Givenname, string Sn, string DisplayName, string physicalDeliveryOfficeName, string telephoneNumber, string mail, string title, string department, string company, string manager, string mobile, string facsimileTelephoneNumber, string ipPhone, string description, bool IsAccountExpires, DateTime accountExpires, bool userAccountControl)
        {
            if (userPrincipalName == "") return 0; //防止空白帳號
            try
            {
                OU = SetOU(userOU);
                string strCN = userPrincipalName; //DisplayName + "(" + userPrincipalName + ")";
                //1.定義 DirectoryEntry
                DirectoryEntry de = new DirectoryEntry(strLDAP, ADUser, ADPassword);
                //2.定義 新增 DirectoryEntry
                using (DirectoryEntry objUser = de.Children.Add("CN=" + strCN + "," + OU, "user"))
                {
                    objUser.UsePropertyCache = true;
                    objUser.Properties["samAccountName"].Add(userPrincipalName);    //AD ID
                    objUser.Properties["userPrincipalName"].Add(userPrincipalName); //帳號
                    objUser.Properties["DisplayName"].Add(DisplayName);             //顯示名稱
                    if (Givenname != "") objUser.Properties["Givenname"].Add(Givenname); //名                        
                    if (Sn != "") objUser.Properties["Sn"].Add(Sn); //姓                        
                    if (physicalDeliveryOfficeName != "") //辦公室
                        objUser.Properties["physicalDeliveryOfficeName"].Add(physicalDeliveryOfficeName);
                    if (mail != "") objUser.Properties["mail"].Add(mail); //電子郵件                        
                    if (title != "") objUser.Properties["title"].Add(title); //職稱                        
                    if (department != "") objUser.Properties["department"].Add(department); //部門                        
                    if (company != "") objUser.Properties["company"].Add(company);   //公司                        

                    if (manager != "") //長官
                    {
                        DirectorySearcher dsManager = new DirectorySearcher(de);
                        dsManager.Filter = "(SAMAccountName=" + manager + ")";
                        SearchResult ManagerResult = dsManager.FindOne();
                        string ManagerName = (string)ManagerResult.Properties["DistinguishedName"][0];
                        objUser.Properties["Manager"].Value = ManagerName;
                    }
                    if (telephoneNumber != "") objUser.Properties["telephonenumber"].Add(telephoneNumber); //電話
                    if (mobile != "") objUser.Properties["mobile"].Add(mobile); //行動電話
                    if (facsimileTelephoneNumber != "") objUser.Properties["facsimiletelephonenumber"].Add(facsimileTelephoneNumber); //傳真
                    if (ipPhone != "") objUser.Properties["ipPhone"].Add(ipPhone); //IP電話
                    if (description != "") objUser.Properties["description"].Add(description); //業務職掌

                    if (IsAccountExpires) objUser.Properties["accountExpires"].Add(accountExpires.ToFileTime().ToString()); //帳號到期日
                    objUser.CommitChanges();
                    //設定密碼
                    objUser.Invoke("SetPassword", new object[] { Pwd });
                    objUser.CommitChanges();

                    //啟用帳號
                    //int val = (int)objUser.Properties["userAccountControl"].Value;
                    if (userAccountControl) objUser.Properties["userAccountControl"].Value = 0x2 ^ 0x2;
                    else objUser.Properties["userAccountControl"].Value = 0x2;
                    objUser.CommitChanges();
                }
                de.Close();
                return 1;
            }
            catch (Exception ex)
            {
                string Msg = ex.Message;
                //Msg += "<br>" + ex.StackTrace.Replace(" ", "").ToString();
                //throw new Exception(Msg);
                return 0;
            }
        }
        #endregion

        #region 修改帳號
        public int Edit_ADUser(string userOU, string userPrincipalName, string Pwd, string Givenname, string Sn, string DisplayName, string physicalDeliveryOfficeName, string telephoneNumber, string mail, string title, string department, string company, string manager, string mobile, string facsimileTelephoneNumber, string ipPhone, string description, bool IsAccountExpires, DateTime accountExpires, bool userAccountControl)
        {
            if (userPrincipalName == "") return 0; //防止空白帳號
            try
            {
                OU = SetOU(userOU); //因Windows版本不同,所以最好先手動建立OU
                string strCN = userPrincipalName; //DisplayName + "(" + userPrincipalName + ")";
                DirectoryEntry de = new DirectoryEntry(strLDAP, ADUser, ADPassword);
                using (DirectorySearcher ds = new DirectorySearcher(de))
                {
                    //取得原LDAP資訊
                    string Src_LDAP = "";
                    ds.Filter = "(SAMAccountName=" + userPrincipalName + ")";
                    SearchResult result = ds.FindOne();
                    if (result != null) Src_LDAP = result.GetDirectoryEntry().Path;
                    else return 0;

                    string New_LDAP = "LDAP://" + ADServerIP + "/CN=" + strCN + "," + OU + "," + DC;
                    //若OU不同時需利用DirectoryEntry.MoveTo變更
                    if (Src_LDAP != New_LDAP)
                    {
                        New_LDAP = "LDAP://" + ADServerIP + "/" + OU + "," + DC; //需移除CN參數
                        DirectoryEntry eLocation = new DirectoryEntry(Src_LDAP, ADUser, ADPassword);
                        DirectoryEntry nLocation = new DirectoryEntry(New_LDAP, ADUser, ADPassword);
                        eLocation.UsePropertyCache = true;
                        eLocation.MoveTo(nLocation);
                        nLocation.Close();
                        eLocation.Close();
                    }

                    //找一筆
                    ds.Filter = "(SAMAccountName=" + userPrincipalName + ")";
                    SearchResult sr = ds.FindOne();
                    if (sr != null)
                    {
                        try
                        {
                            using (DirectoryEntry objUser = de.Children.Find("CN=" + strCN + "," + OU, "user"))
                            {
                                objUser.UsePropertyCache = true;
                                objUser.Properties["DisplayName"].Value = (DisplayName);            //顯示名稱
                                objUser.Properties["Givenname"].Value = (Givenname);                //名
                                objUser.Properties["Sn"].Value = (Sn);                              //姓
                                objUser.Properties["physicalDeliveryOfficeName"].Value = (physicalDeliveryOfficeName); //辦公室
                                objUser.Properties["mail"].Value = (mail);                          //電子郵件
                                if (title != "") objUser.Properties["title"].Value = (title);       //職稱
                                objUser.Properties["department"].Value = (department);              //部門
                                objUser.Properties["company"].Value = (company);                    //公司

                                if (manager != "") //長官
                                {
                                    DirectorySearcher dsManager = new DirectorySearcher(de);
                                    dsManager.Filter = "(SAMAccountName=" + manager + ")";
                                    SearchResult ManagerResult = dsManager.FindOne();
                                    string ManagerName = (string)ManagerResult.Properties["DistinguishedName"][0];
                                    objUser.Properties["Manager"].Value = ManagerName;
                                }
                                else if (objUser.Properties.Contains("Manager")) objUser.Properties["Manager"].Clear();

                                if (objUser.Properties.Contains("telephonenumber")) //電話
                                {
                                    if (telephoneNumber != "") objUser.Properties["telephonenumber"][0] = telephoneNumber;
                                    else objUser.Properties["telephonenumber"].Clear();
                                }
                                else if (telephoneNumber != "") objUser.Properties["telephonenumber"].Add(telephoneNumber);
                                if (objUser.Properties.Contains("mobile"))//行動電話
                                {
                                    if (mobile != "") objUser.Properties["mobile"][0] = mobile;
                                    else objUser.Properties["mobile"].Clear();
                                }
                                else if (mobile != "") objUser.Properties["mobile"].Add(mobile);
                                if (objUser.Properties.Contains("facsimiletelephonenumber")) //傳真
                                {
                                    if (facsimileTelephoneNumber != "") objUser.Properties["facsimiletelephonenumber"][0] = facsimileTelephoneNumber;
                                    else objUser.Properties["facsimiletelephonenumber"].Clear();
                                }
                                else if (facsimileTelephoneNumber != "") objUser.Properties["facsimiletelephonenumber"].Add(facsimileTelephoneNumber);
                                if (objUser.Properties.Contains("ipPhone")) //IP電話
                                {
                                    if (ipPhone != "") objUser.Properties["ipPhone"][0] = ipPhone;
                                    else objUser.Properties["ipPhone"].Clear();
                                }
                                else if (ipPhone != "") objUser.Properties["ipPhone"].Add(ipPhone);
                                if (objUser.Properties.Contains("description")) //業務職掌
                                {
                                    if (description != "") objUser.Properties["description"][0] = description;
                                    else objUser.Properties["description"].Clear();
                                }
                                else if (description != "") objUser.Properties["description"].Add(description);

                                if (IsAccountExpires) objUser.Properties["accountExpires"].Value = (accountExpires.ToFileTime().ToString()); //帳號到期日
                                objUser.CommitChanges();
                                //重設密碼
                                if (Pwd != "") ResetPassword(objUser, Pwd);
                                //啟用帳號
                                //int val = (int)objUser.Properties["userAccountControl"].Value;
                                if (userAccountControl) objUser.Properties["userAccountControl"].Value = 0x2 ^ 0x2;
                                else objUser.Properties["userAccountControl"].Value = 0x2;
                                objUser.CommitChanges();
                            }
                        }
                        catch (Exception ex1)
                        {
                            string err = ex1.ToString();
                            de.Close();
                            return 0;
                        }
                    }
                    else
                    {
                        de.Close();
                        return 0;
                        //throw new Exception("User not found");
                    }
                }
                de.Close();
                return 1;
            }
            catch (Exception ex)
            {
                string Msg = ex.Message;
                //Msg += "<br>" + ex.StackTrace.Replace(" ", "").ToString();
                //throw new Exception(Msg);
                return 0;
            }
        }
        #endregion

        #region 修改CN 值
        public int Chang_CN(string userPrincipalName, string DisplayName, string department)
        {
            string userOU = department;
            try
            {
                OU = SetOU(userOU); //因Windows版本不同,所以最好先手動建立OU
                string strCN = userPrincipalName; //DisplayName + "(" + userPrincipalName + ")";
                DirectoryEntry de = new DirectoryEntry(strLDAP, ADUser, ADPassword);
                using (DirectorySearcher ds = new DirectorySearcher(de))
                {
                    //取得原LDAP資訊
                    string Src_LDAP = "";
                    ds.Filter = "(SAMAccountName=" + userPrincipalName + ")";
                    SearchResult result = ds.FindOne();
                    if (result != null) Src_LDAP = result.GetDirectoryEntry().Path;
                    else return 0;

                    string New_LDAP = "LDAP://" + ADServerIP + "/CN=" + strCN + "," + OU + "," + DC;
                    //若OU不同時需利用DirectoryEntry.MoveTo變更
                    if (Src_LDAP != New_LDAP)
                    {
                        New_LDAP = "LDAP://" + ADServerIP + "/" + OU + "," + DC; //需移除CN參數
                        DirectoryEntry eLocation = new DirectoryEntry(Src_LDAP, ADUser, ADPassword);
                        DirectoryEntry nLocation = new DirectoryEntry(New_LDAP, ADUser, ADPassword);
                        eLocation.UsePropertyCache = true;
                        eLocation.MoveTo(nLocation);
                        nLocation.Close();
                        eLocation.Close();
                    }
                }
                de.Close();
                return 1;
            }
            catch (Exception ex)
            {
                string Msg = ex.Message;
                //Msg += "<br>" + ex.StackTrace.Replace(" ", "").ToString();
                //throw new Exception(Msg);
                return 0;
            }
        }
        #endregion

        #region 重設密碼
        public int RestPW(string userPrincipalName, string DisplayName, string strPassword, string strDepartment)
        {
            try
            {
                DirectoryEntry de = new DirectoryEntry(strLDAP, ADUser, ADPassword);
                SetOU(strDepartment);
                string strCN = userPrincipalName; //DisplayName + "(" + userPrincipalName + ")";
                using (DirectorySearcher ds = new DirectorySearcher(de))
                {
                    ds.Filter = "(SAMAccountName=" + userPrincipalName + ")";
                    //找一筆
                    SearchResult sr = ds.FindOne();
                    if (sr != null)
                    {
                        using (DirectoryEntry objUser = de.Children.Find("CN=" + strCN + "," + OU, "user"))
                        {
                            ResetPassword(objUser, strPassword);
                        }
                    }
                    else
                    {
                        //throw new Exception("User not found");
                        return 0;
                    }
                }
                de.Close();
                return 1;
            }
            catch (Exception ex)
            {
                string Msg = ex.Message;
                //Msg += "<br>" + ex.StackTrace.Replace(" ", "").ToString();
                //throw new Exception(Msg);
                return 0;
            }
        }
        private void ResetPassword(DirectoryEntry objUser, string strPassword)
        {
            objUser.Invoke("SetPassword", new object[] { strPassword });
            objUser.Properties["LockOutTime"].Value = 0; //unlock account
            objUser.CommitChanges();
            objUser.Close();
        }
        #endregion

        #region 刪除帳號
        public int Del_ADUser(string userPrincipalName, string DisplayName, string strDepartment)
        {
            try
            {
                #region 防止OU與資料庫紀錄不符,所已先搬移到系統指定OU下再做刪除動作
                //SetOU(department); //因Windows版本不同,所以最好先手動建立OU
                //Edit_ADUser(userPrincipalName, "", "", "", DisplayName, "", "", "", "", department, "", "", "", "", "", "" , DateTime.Now, false);
                #endregion

                DirectoryEntry de = new DirectoryEntry(strLDAP, ADUser, ADPassword);
                SetOU(strDepartment);
                string strCN = userPrincipalName; //DisplayName + "(" + userPrincipalName + ")";
                using (DirectorySearcher ds = new DirectorySearcher(de))
                {
                    ds.Filter = "(SAMAccountName=" + userPrincipalName + ")";
                    //找一筆
                    SearchResult sr = ds.FindOne();
                    if (sr != null)
                    {
                        using (DirectoryEntry objUser = de.Children.Find("CN=" + strCN + "," + OU, "user"))
                        {
                            objUser.DeleteTree();
                        }
                    }
                    else
                    {
                        //throw new Exception("User not found");
                        return 0;
                    }
                }
                de.Close();
                return 1;
            }
            catch (System.DirectoryServices.DirectoryServicesCOMException ex)
            {
                string Msg = ex.Message;
                //Msg += "<br>" + ex.StackTrace.Replace(" ", "").ToString();
                //throw new Exception(Msg);
                return 0;
            }
        }
        #endregion

        #region 啟用帳號
        public int Enable_ADUser(string userPrincipalName, string DisplayName, string department)
        {
            try
            {
                DirectoryEntry de = new DirectoryEntry(strLDAP, ADUser, ADPassword);
                SetOU(department);
                string strCN = userPrincipalName; //DisplayName + "(" + userPrincipalName + ")";
                using (DirectorySearcher ds = new DirectorySearcher(de))
                {
                    ds.Filter = "(SAMAccountName=" + userPrincipalName + ")";
                    //找一筆
                    SearchResult sr = ds.FindOne();
                    if (sr != null)
                    {
                        using (DirectoryEntry objUser = de.Children.Find("CN=" + strCN + "," + OU, "user"))
                        {
                            objUser.Properties["userAccountControl"].Value = 0x2 ^ 0x2;
                            objUser.CommitChanges();
                        }
                    }
                    else
                    {
                        //throw new Exception("User not found");
                        return 0;
                    }
                }
                de.Close();
                return 1;
            }
            catch (Exception ex)
            {
                string Msg = ex.Message;
                //Msg += "<br>" + ex.StackTrace.Replace(" ", "").ToString();
                //throw new Exception(Msg);
                return 0;
            }
        }
        #endregion

        #region 停用帳號
        public int Disable_ADUser(string userPrincipalName, string DisplayName, string department)
        {
            try
            {
                DirectoryEntry de = new DirectoryEntry(strLDAP, ADUser, ADPassword);
                SetOU(department);
                string strCN = userPrincipalName; //DisplayName + "(" + userPrincipalName + ")";
                using (DirectorySearcher ds = new DirectorySearcher(de))
                {
                    ds.Filter = "(SAMAccountName=" + userPrincipalName + ")";
                    //找一筆
                    SearchResult sr = ds.FindOne();
                    if (sr != null)
                    {
                        using (DirectoryEntry objUser = de.Children.Find("CN=" + strCN + "," + OU, "user"))
                        {
                            objUser.Properties["userAccountControl"].Value = 0x2;
                            objUser.CommitChanges();
                        }
                    }
                    else
                    {
                        //throw new Exception("User not found");
                        return 0;
                    }
                }
                de.Close();
                return 1;
            }
            catch (Exception ex)
            {
                string Msg = ex.Message;
                //Msg += "<br>" + ex.StackTrace.Replace(" ", "").ToString();
                //throw new Exception(Msg);
                return 0;
            }
        }
        #endregion

        #region 將使用者從group中移除
        public int RemoveUserFromGroup(string UserID, string GroupID)
        {
            try
            {
                DirectoryEntry de = new DirectoryEntry(strLDAP, ADUser, ADPassword);
                DirectoryEntry obj = de.Children.Find(GroupID);
                //GroupID 格式(CN, OU 不包含DC) CN=XX,OU=YY
                //Ex: CN=TestGroup2,OU=第二課,OU=人事室,OU=金門縣政府
                obj.Properties["member"].Remove(UserID);
                //UserID 格式(CN, OU, DC完整) CN=XX,OU=YY,DC=ZZ
                //Ex: CN=per45,OU=第二課,OU=人事室,OU=金門縣政府,DC=klad,DC=domain
                obj.CommitChanges();
                obj.Close();
                return 1;
            }
            catch (Exception ex)
            {
                string Msg = ex.Message;
                //return Msg;
                return 0;
            }
        }
        #endregion

        private string WorkStationParser(string CurrentStation, string TargetStation, int method)
        {
            HashSet<string> SplitedStation = new HashSet<string>(CurrentStation.Split(','));
            if (method == 1)    // method = 1 => insert Target Station
                SplitedStation.Add(TargetStation);
            else if (method == 2)   // method = 2 => remove Target Station
                SplitedStation.Remove(TargetStation);

            if (SplitedStation.Count != 0)  // 理論上不會有 = 0的情況  = 0要在外面用.remove處理
                return string.Join(",", SplitedStation);
            else
                return string.Empty;
        }

        #region 使用者登入權限新增
        public int AddUserWorkstation(string userPrincipalName, string Workstation, string strDepartment)
        {
            try
            {
                DirectoryEntry de = new DirectoryEntry(strLDAP, ADUser, ADPassword);
                SetOU(strDepartment);
                string strCN = userPrincipalName; //DisplayName + "(" + userPrincipalName + ")";
                using (DirectorySearcher ds = new DirectorySearcher(de))
                {
                    ds.Filter = "(SAMAccountName=" + userPrincipalName + ")";
                    //找一筆
                    SearchResult sr = ds.FindOne();
                    if (sr != null)
                    {
                        // OU正確才找的到人
                        using (DirectoryEntry objUser = de.Children.Find("CN=" + strCN + "," + OU, "user"))
                        {
                            if (objUser == null)
                                return 0;
                            // .add()只能在本來沒東西的情況下使用 至少一台的情況得改.value
                            if (objUser.Properties["userWorkStations"].Value != null)
                                objUser.Properties["userWorkStations"].Value = WorkStationParser(objUser.Properties["userWorkStations"].Value.ToString(), Workstation, 1);
                            else
                                objUser.Properties["userWorkStations"].Add(Workstation);

                            // 雖然說是 Properties["userWorkStations"]
                            // but目前值是存在 Properties["userWorkStations"][0] ex: "windows7-001, windows7-002" 的字串
                            // .add() 會新增到 Properties["userWorkStations"][1] CommitChange()就exception了.
                            objUser.CommitChanges();
                        }
                    }
                    else
                    {
                        //throw new Exception("User not found");
                        return 0;
                    }
                }
                de.Close();
                return 1;
            }
            catch (Exception ex)
            {
                string Msg = ex.Message;
                //Msg += "<br>" + ex.StackTrace.Replace(" ", "").ToString();
                //throw new Exception(Msg);
                return 0;
            }
        }
        #endregion

        #region 使用者登入權限刪除
        public int DelUserWorkstation(string userPrincipalName, string Workstation, string strDepartment)
        {
            try
            {
                DirectoryEntry de = new DirectoryEntry(strLDAP, ADUser, ADPassword);
                SetOU(strDepartment);
                string strCN = userPrincipalName; //DisplayName + "(" + userPrincipalName + ")";
                using (DirectorySearcher ds = new DirectorySearcher(de))
                {
                    ds.Filter = "(SAMAccountName=" + userPrincipalName + ")";
                    //找一筆
                    SearchResult sr = ds.FindOne();
                    if (sr != null)
                    {
                        using (DirectoryEntry objUser = de.Children.Find("CN=" + strCN + "," + OU, "user"))
                        {
                            // 本來就沒權限的話就不用刪了  (注意: 全部刪掉 = 全部電腦都可以登入！)
                            if (objUser.Properties["userWorkStations"].Value != null)
                            {
                                // .remove()只能刪掉一台的情況  有兩台以上要直接改.value (偉大的儲存方式 字串=  =)
                                if (objUser.Properties["userWorkStations"].Value.ToString() == Workstation)
                                    objUser.Properties["userWorkStations"].Remove(Workstation);
                                else
                                    objUser.Properties["userWorkStations"].Value = WorkStationParser(objUser.Properties["userWorkStations"].Value.ToString(), Workstation, 2);

                                objUser.CommitChanges();
                            }
                        }
                    }
                    else
                    {
                        //throw new Exception("User not found");
                        return 0;
                    }
                }
                de.Close();
                return 1;
            }
            catch (Exception ex)
            {
                string Msg = ex.Message;
                //Msg += "<br>" + ex.StackTrace.Replace(" ", "").ToString();
                //throw new Exception(Msg);
                return 0;
            }
        }
        #endregion
    }
}
