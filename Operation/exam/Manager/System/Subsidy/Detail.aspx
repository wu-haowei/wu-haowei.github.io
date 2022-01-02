<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MP.master" AutoEventWireup="true" CodeFile="Detail.aspx.cs" Inherits="System_Subsidy_Detail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div align="right">
        <asp:Button ID="btnGoBack" runat="server" Text="回上一頁" class="btn" CausesValidation="False" />
    </div>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
        </ContentTemplate>
    </asp:UpdatePanel>
    <table class="Detail-table" width="100%">

        <tr>
            <th class="td-title"><span class="ReqItem" style="color: red">*</span>補助類型</th>
            <td>

                <asp:RadioButtonList ID="Catregory" runat="server" OnSelectedIndexChanged="Catregory_SelectedIndexChanged" RepeatDirection="Horizontal" RepeatLayout="Flow" AutoPostBack="True">
                    <asp:ListItem Selected="True" Value="1">診治項目</asp:ListItem>
                    <asp:ListItem Value="2">維修費項目</asp:ListItem>
                </asp:RadioButtonList>

            </td>
        </tr>
        <tr>
            <th class="td-title"><span class="ReqItem" style="color: red">*</span>項目名稱</th>
            <td>
                <asp:TextBox ID="txtName" runat="server" MaxLength="100"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtName" ErrorMessage="必填" ForeColor="#FF3300"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr runat="server" id="TrUnit">
            <th class="td-title">維修項目單位</th>
            <td>
                <%--輸入框--%>
                <asp:TextBox ID="txtUnit" runat="server" MaxLength="50"></asp:TextBox>
            </td>
        </tr>

        <tr>
            <th class="td-title"><span class="ReqItem" style="color: red">*</span>補助金額</th>
            <td>
                <asp:TextBox ID="txtAmount" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtAmount" ErrorMessage="必填" ForeColor="#FF3300"></asp:RequiredFieldValidator>
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

    <%--UpdatePanel補助類型 維修費項目? 顯示 維修項目單位 : 關閉 維修項目單位--%>
</asp:Content>

