<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MP.master" AutoEventWireup="true" CodeFile="Detail.aspx.cs" Inherits="System_CasePR_Detail" %>

<%@ Register Src="~/Common/Date.ascx" TagPrefix="uc1" TagName="Date" %>
<%@ Register Src="~/Common/wucDateTime.ascx" TagPrefix="uc1" TagName="wucDateTime" %>

<%@ Register Assembly="Hamastar.GridViewHead" Namespace="GridViewHead" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <div align="right">
        <asp:Button ID="btnGoBack" runat="server" Text="回上一頁" class="ButtonStyle_H1" CausesValidation="False" CssClass="btn" />
    </div>
    <br />

    <%--新增個案--%>
    <asp:Panel runat="server" ID="plAddCase">
        <table class="Detail-table" width="100%">
            <tr>
                <th class="td-title" style="width: 15%"><span class="ReqItem"></span>案件編號</th>
                <td>
                    <asp:Label runat="server" ID="lblCaseNo" /></td>
            </tr>
            <tr>
                <th class="td-title"><span class="ReqItem" style="color: red">*</span>裝置前收件日期</th>
                <td>
                    <uc1:wucDateTime runat="server" ID="dtBeforDate" IsShowTime="false" IsShowDefaultDate="false" DateTimeValidatorEnableFlag="false" />
                </td>
            </tr>
        </table>

        <br />
        <br />
        <div class="Detail-table-title"><span>院所基本資料</span></div>
        <table class="Detail-table" width="100%">
            <tr>
                <th class="td-title" style="width: 15%"><span class="ReqItem" style="color: red">*</span>醫療院所名稱</th>
                <td style="width: 35%">
                    <asp:DropDownList ID="ddlDeptSN" runat="server" ClientIDMode="Static" AutoPostBack="true" OnSelectedIndexChanged="ddlDeptSN_SelectedIndexChanged" />
                    <asp:HiddenField ID="hdDeptSN" runat="server" />
                </td>
                <th class="td-title" style="width: 15%"><span class="ReqItem"></span>醫療院所統編</th>
                <td style="width: 35%">
                    <asp:Label ID="lbDID" runat="server" ClientIDMode="Static"></asp:Label>
                </td>
            </tr>
            <tr>
                <th class="td-title"><span class="ReqItem" style="color: red">*</span>負責醫師姓名</th>
                <td>
                    <asp:DropDownList ID="ddlDoctorSN" runat="server" ClientIDMode="Static" AutoPostBack="true" OnSelectedIndexChanged="ddlDoctorSN_SelectedIndexChanged" Enabled="false" />
                    <asp:HiddenField ID="hdDoctorSN" runat="server" />
                </td>
                <th class="td-title"><span class="ReqItem"></span>院所電話</th>
                <td>
                    <asp:Label ID="lbTel" runat="server" ClientIDMode="Static"></asp:Label>
                </td>
            </tr>
            <tr>
                <th class="td-title"><span class="ReqItem"></span>院所傳真</th>
                <td>
                    <asp:Label ID="lbFax" runat="server" ClientIDMode="Static"></asp:Label>
                </td>
                <th class="td-title"><span class="ReqItem"></span>院所地址</th>
                <td>
                    <asp:Label ID="lbAddr" runat="server" ClientIDMode="Static"></asp:Label>
                </td>
            </tr>
        </table>

        <br />
        <br />
        <div class="Detail-table-title"><span>個案基本資料</span></div>
        <table class="Detail-table" width="100%">
            <tr>
                <th class="td-title" style="width: 15%"><span class="ReqItem" style="color: red">*</span>身分證字號</th>
                <td style="width: 35%">
                    <asp:TextBox ID="txtPID" runat="server" Width="75%"></asp:TextBox>
                    <asp:Button runat="server" Text="檢核" class="edit-btn" ID="btnAudit" OnClick="btnAudit_Click" />
                </td>
                <th class="td-title" style="width: 15%"><span class="ReqItem" style="color: red">*</span>本縣設籍滿一年</th>
                <td style="width: 35%">
                    <asp:RadioButtonList ID="rdbIsLocalUser" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                        <asp:ListItem Value="1" Selected="True">是</asp:ListItem>
                        <asp:ListItem Value="0">否</asp:ListItem>
                    </asp:RadioButtonList>
                </td>
            </tr>
            <tr>
                <th class="td-title"><span class="ReqItem" style="color: red">*</span>姓名</th>
                <td>
                    <asp:TextBox ID="txtName" runat="server" Width="95%"></asp:TextBox>
                </td>
                <th class="td-title"><span class="ReqItem"></span>身分別</th>
                <td>
                    <asp:RadioButtonList ID="rdbUserType" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                        <asp:ListItem Value="一般" Selected="True">一般</asp:ListItem>
                        <asp:ListItem Value="中低收入">中低收入</asp:ListItem>
                        <asp:ListItem Value="身心障礙">身心障礙</asp:ListItem>
                        <asp:ListItem Value="原住民">原住民</asp:ListItem>
                    </asp:RadioButtonList>
                </td>
            </tr>
            <tr>
                <th class="td-title"><span class="ReqItem" style="color: red">*</span>生日</th>
                <td>
                    <uc1:wucDateTime runat="server" ID="dtBirthday" IsShowTime="false" IsShowDefaultDate="false" DateTimeValidatorEnableFlag="false" />
                </td>
                <th class="td-title"><span class="ReqItem" style="color: red">*</span>電話</th>
                <td>
                    <asp:TextBox ID="txtTel" runat="server" Width="95%"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <th class="td-title"><span class="ReqItem" style="color: red">*</span>性別</th>
                <td>
                    <asp:RadioButtonList ID="rdbGender" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                        <asp:ListItem Value="男" Selected="True">男</asp:ListItem>
                        <asp:ListItem Value="女">女</asp:ListItem>
                    </asp:RadioButtonList>
                </td>
                <th class="td-title"><span class="ReqItem" style="color: red">*</span>地址</th>
                <td>
                    <asp:TextBox ID="txtAddr" runat="server" Width="95%"></asp:TextBox>
                </td>
            </tr>
        </table>

    </asp:Panel>
    <br />
    <br />
    <div class="Detail-table-title"><span>診治紀錄</span></div>
    <asp:Panel runat="server" ID="plSubsidy">
        <table class="Detail-table" width="100%">
            <tr>
                <th class="td-title" style="width: 15%"><span class="ReqItem" style="color: red">*</span>診治項目</th>
                <td style="width: 85%">
                    <div class="check-list">
                        <asp:CheckBoxList runat="server" ID="ItemType1" RepeatLayout="Flow" OnSelectedIndexChanged="ItemType1_SelectedIndexChanged" RepeatDirection="Horizontal" AutoPostBack="true" />
                    </div>
                </td>
            </tr>
            <tr>
                <th class="td-title"><span class="ReqItem" style="color: red">*</span>維修費項目</th>
                <td>
                    <div class="check-list-gp">
                        <asp:DataList runat="server" ID="ItemType2" RepeatLayout="Flow" RepeatDirection="Horizontal" Width="100%">
                            <ItemTemplate>
                                <asp:HiddenField runat="server" ID="hdItem" Value='<%# DataBinder.Eval(Container,"DataItem.SN") %>' />
                                <asp:CheckBox runat="server" ID="cbxItem" AutoPostBack="true" OnCheckedChanged="ItemType2CheckBox_changed" />
                                <asp:Label runat="server" Text='<%# DataBinder.Eval(Container,"DataItem.Name") %>' />
                                <asp:TextBox runat="server" ID="txtItem" Width="20px" OnTextChanged="ItemType2TextBox_changed" AutoPostBack="true" />
                                <asp:Label runat="server" Text='<%# DataBinder.Eval(Container,"DataItem.Unit") %>' />
                            </ItemTemplate>
                        </asp:DataList>
                    </div>
                </td>
            </tr>
            <tr>
                <th class="td-title"><span class="ReqItem" style="color: red">*</span>申請補助金額</th>
                <td>
                    <asp:Label runat="server" Width="95%" ID="lbApplyAmount" Text="0元" />
                    <asp:HiddenField runat="server" ID="hdApplyAmount" Value="0" />
                </td>
            </tr>
            <tr>
                <th class="td-title"><span class="ReqItem" style="color: red">*</span>預定完成日期</th>
                <td>
                    <uc1:wucDateTime runat="server" ID="dtFinishDate" IsShowTime="false" IsShowDefaultDate="false" DateTimeValidatorEnableFlag="false" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <br />
    <br />
    <div class="Detail-table-title"><span>附件上傳</span></div>
    <table class="Detail-table" width="100%">
        <tr>
            <td class="td-title" style="width: 90%">
                <asp:FileUpload runat="server" ID="fuFileUpload" Enabled="true" AllowMultiple="true" /><br />
                <asp:Label runat="server" ID="lblFileUpload" />
            </td>
            <td style="width: 10%">
                <asp:Button runat="server" Text="📤" ID="btnFileUpload" OnClick="btnFileUpload_Click" />
            </td>
        </tr>
    </table>

    <br />
    <br />
    <asp:Panel runat="server" ID="plAdmin">
        <div class="Detail-table-title"><span>裝置前行政檢查</span></div>
        <table class="Detail-table" width="100%">
            <tr>
                <th class="td-title" style="width: 15%"><span class="ReqItem" style="color: red">*</span>行政審查日期</th>
                <td style="width: 35%">
                    <uc1:wucDateTime runat="server" ID="dtAdminDate" IsShowTime="false" IsShowDefaultDate="false" DateTimeValidatorEnableFlag="false" />
                </td>
                <th class="td-title" style="width: 15%"><span class="ReqItem" style="color: red">*</span>行政審查結果</th>
                <td style="width: 35%">
                    <asp:RadioButtonList ID="rdbAdminStatus" runat="server" RepeatDirection="Vertical" RepeatLayout="Flow" OnSelectedIndexChanged="rdbAdminStatus_SelectedIndexChanged" AutoPostBack="true">
                        <asp:ListItem Value="通過" Selected="True">通過</asp:ListItem>
                        <asp:ListItem Value="退件">退件</asp:ListItem>
                    </asp:RadioButtonList>
                    <asp:DropDownList ID="ddlAdminRejectSN" runat="server" ClientIDMode="Static" AutoPostBack="true" OnSelectedIndexChanged="ddlAdminRejectSN_SelectedIndexChanged" />
                    <asp:HiddenField ID="hdAdminRejectSN" runat="server" />
                </td>
            </tr>
        </table>
    </asp:Panel>

    <br />
    <br />
    <asp:Panel runat="server" ID="plProf">
        <div class="Detail-table-title"><span>裝置前專業審查</span></div>
        <table class="Detail-table" width="100%">
            <tr>
                <th class="td-title" style="width: 15%"><span class="ReqItem" style="color: red">*</span>專業審查日期</th>
                <td style="width: 35%">
                    <uc1:wucDateTime runat="server" ID="dtProfDate" IsShowTime="false" IsShowDefaultDate="false" DateTimeValidatorEnableFlag="false" />
                </td>
                <th class="td-title" style="width: 15%"><span class="ReqItem" style="color: red">*</span>專業審查結果</th>
                <td style="width: 35%">
                    <asp:RadioButtonList ID="rdbProfStatus" runat="server" RepeatDirection="Vertical" RepeatLayout="Flow" OnSelectedIndexChanged="rdbProfStatus_SelectedIndexChanged" AutoPostBack="true">
                        <asp:ListItem Value="通過" Selected="True">通過</asp:ListItem>
                        <asp:ListItem Value="退件">退件</asp:ListItem>
                    </asp:RadioButtonList>
                    <asp:DropDownList ID="ddlProfRejectSN" runat="server" ClientIDMode="Static" AutoPostBack="true" OnSelectedIndexChanged="ddlProfRejectSN_SelectedIndexChanged" />
                    <asp:HiddenField ID="hdProfRejectSN" runat="server" />
                </td>
            </tr>
            <tr>
                <th class="td-title" style="width: 15%"><span class="ReqItem" style="color: red">*</span>專業審查委員</th>
                <td colspan="3" style="width: 85%">
                    <asp:DropDownList ID="ddlProfUser" runat="server" ClientIDMode="Static" AutoPostBack="true" OnSelectedIndexChanged="ddlProfUser_SelectedIndexChanged" />
                    <asp:HiddenField ID="hdProfUser" runat="server" />
                </td>
            </tr>
        </table>
    </asp:Panel>

    <%--核銷專業審查 or 特殊個案結案--%>
    <br />
    <br />
    <asp:Panel runat="server" ID="plWriteOff">
        <div class="Detail-table-title"><span>請選擇 特殊個案結案 或 核銷專業審查</span></div>
        <asp:RadioButtonList ID="rdbWriteOffType" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" AutoPostBack="true" OnSelectedIndexChanged="rdbWriteOffType_SelectedIndexChanged">
            <asp:ListItem Value="特殊個案結案" Selected="True">特殊個案結案</asp:ListItem>
            <asp:ListItem Value="核銷專業審查">核銷專業審查</asp:ListItem>
        </asp:RadioButtonList>

        <div class="Detail-table-title">
            <span>
                <asp:Label runat="server" ID="lblWriteTitle" Text="特殊個案結案" /></span>
        </div>
        <table class="Detail-table" width="100%">
            <tr>
                <th class="td-title" style="width: 15%"><span class="ReqItem" style="color: red">*</span>裝置前<br />
                    審查通過發文日期</th>
                <td style="width: 35%">
                    <uc1:wucDateTime runat="server" ID="dtWriteOffBefDate" IsShowTime="false" IsShowDefaultDate="false" DateTimeValidatorEnableFlag="false" />
                </td>
                <th class="td-title" style="width: 15%"><span class="ReqItem" style="color: red">*</span>裝置後收件日期</th>
                <td style="width: 35%">
                    <uc1:wucDateTime runat="server" ID="dtWriteOffAftDate" IsShowTime="false" IsShowDefaultDate="false" DateTimeValidatorEnableFlag="false" />
                </td>
            </tr>
            <tr>
                <th class="td-title" style="width: 15%"><span class="ReqItem" style="color: red">*</span>撥付費用</th>
                <td style="width: 35%">
                    <asp:TextBox runat="server" ID="txtWriteOffAmout" TextMode="Number" />
                </td>
                <th class="td-title" style="width: 15%"><span class="ReqItem" style="color: red">*</span>篩檢費</th>
                <td style="width: 35%">
                    <asp:RadioButtonList ID="rdbWriteOffFee" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" AutoPostBack="true">
                        <asp:ListItem Value="是" Selected="True">是</asp:ListItem>
                        <asp:ListItem Value="否">否</asp:ListItem>
                    </asp:RadioButtonList>
                </td>
            </tr>


            <%--核銷專業審查Start--%>
            <asp:Panel runat="server" ID="plAudit">
                <tr>
                    <th class="td-title" style="width: 15%"><span class="ReqItem" style="color: red">*</span>專業審查日期</th>
                    <td style="width: 35%">
                        <uc1:wucDateTime runat="server" ID="dtWriteOffProDate" IsShowTime="false" IsShowDefaultDate="false" DateTimeValidatorEnableFlag="false" />
                    </td>
                    <th class="td-title" style="width: 15%"><span class="ReqItem" style="color: red">*</span>專業審查委員</th>
                    <td style="width: 35%">
                        <asp:DropDownList ID="ddlWriteOffUser" runat="server" ClientIDMode="Static" AutoPostBack="true" OnSelectedIndexChanged="ddlWriteOffUser_SelectedIndexChanged" />
                        <asp:HiddenField ID="hdWriteOffUser" runat="server" />
                    </td>
                </tr>
            </asp:Panel>
            <%--核銷專業審查End--%>


            <tr>
                <th class="td-title" style="width: 15%"><span class="ReqItem" style="color: red">*</span><asp:Label runat="server" ID="lblWriteOffStatus" Text="特殊結案結果" /></th>
                <td style="width: 35%">
                    <asp:RadioButtonList ID="rdbWriteOffStatus" runat="server" RepeatDirection="Vertical" RepeatLayout="Flow" AutoPostBack="true" OnSelectedIndexChanged="rdbWriteOffStatus_SelectedIndexChanged">
                    </asp:RadioButtonList>
                    <asp:DropDownList ID="ddlWriteOffRejectSN" runat="server" ClientIDMode="Static" AutoPostBack="true" OnSelectedIndexChanged="ddlWriteOffRejectSN_SelectedIndexChanged" />
                    <asp:HiddenField ID="hdWriteOffRejectSN" runat="server" />
                </td>
                <th class="td-title" style="width: 15%"><span class="ReqItem" style="color: red">*</span>是否移交社會局</th>
                <td style="width: 35%">
                    <asp:RadioButtonList ID="rdbWriteOffTransfer" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" AutoPostBack="true">
                        <asp:ListItem Value="是">是</asp:ListItem>
                        <asp:ListItem Value="否" Selected="True">否</asp:ListItem>
                    </asp:RadioButtonList>
                </td>
            </tr>



            <tr>
                <th class="td-title" style="width: 15%"><span class="ReqItem"></span>備註</th>
                <td colspan="3" style="width: 85%">
                    <asp:TextBox runat="server" ID="txtWriteOffMemo" TextMode="MultiLine" Height="50px" Width="95%" />
                </td>
            </tr>
        </table>
    </asp:Panel>



    <div style="text-align: center; padding-top: 10px">
        <asp:Button ID="btnSave" runat="server" Text="確定" class="edit-btn" OnClick="btnSave_Click" />
        &nbsp;
        <asp:Button ID="btnCancel" runat="server" Text="取消" class="edit-btn" CausesValidation="False" />
    </div>
</asp:Content>

