<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MP.master" AutoEventWireup="true" CodeFile="Detail.aspx.cs" Inherits="System_Examiner_Detail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div align="right">
        <asp:Button ID="btnGoBack" runat="server" Text="回上一頁" class="btn" CausesValidation="False" />
    </div>
    <table class="Detail-table" width="100%">
        <tr>
            <th class="td-title"><span class="ReqItem" style="color: red">*</span>委員姓名</th>
            <td>
                <asp:TextBox ID="txtName" runat="server" MaxLength="100" Width="200px"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtName" ErrorMessage="姓名沒有輸入" ForeColor="Red"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <th class="td-title"><span class="ReqItem" style="color: red">*</span>身分證字號</th>
            <td>
                <asp:TextBox ID="identityNumber" runat="server" Width="200px" MaxLength="10"></asp:TextBox>
                <%--<font color="#FF0000">(若不填寫密碼則只能用單一窗口登入)</font>--%>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="identityNumber" ErrorMessage="身份證字號格式錯誤" ForeColor="Red" ValidationExpression="^[A-Za-z]{1}[1-2]{1}[0-9]{8}$"></asp:RegularExpressionValidator>
            </td>
        </tr>
        <tr>
            <th class="td-title"><span class="ReqItem" style="color: red">*</span>地址</th>
            <td>
                <asp:TextBox ID="txtaddress" runat="server" Width="350px" MaxLength="255"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtaddress" ErrorMessage="地址沒有輸入" ForeColor="Red"></asp:RequiredFieldValidator>
            </td>
        </tr>



        <tr>
            <th class="td-title"><span class="ReqItem" style="color: red">*</span>狀態</th>
            <td>
                <asp:RadioButtonList ID="rdbEnabel" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                    <asp:ListItem Value="1" Selected="True">啟用</asp:ListItem>
                    <asp:ListItem Value="2">停用</asp:ListItem>
                    <%--<asp:ListItem Value="0">鎖定</asp:ListItem>--%>
                </asp:RadioButtonList>
            </td>
        </tr>
        <tr>
            <th class="td-title">停用說明</th>
            <td>
                <asp:TextBox ID="Deactivateillustrate" runat="server" Width="500px" MaxLength="255"></asp:TextBox>
            </td>
        </tr>

    </table>
    <div style="text-align: center; padding-top: 10px">
        <asp:Button ID="btnSave" runat="server" Text="確定" class="edit-btn" OnClick="btnSave_Click" />
        &nbsp;
        <asp:Button ID="btnCancel" runat="server" Text="取消" class="edit-btn" CausesValidation="False" />
    </div>





</asp:Content>

