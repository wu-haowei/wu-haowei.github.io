<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MP.master" AutoEventWireup="true" CodeFile="Query.aspx.cs" Inherits="System_Doctor_Query" %>

<%@ Register Assembly="Hamastar.GridViewHead" Namespace="GridViewHead" TagPrefix="asp" %>
<%@ Register Src="~/Common/Date.ascx" TagPrefix="uc1" TagName="Date" %>
<%@ Register Src="~/Common/wucDateTime.ascx" TagPrefix="uc1" TagName="wucDateTime" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="fieldset" style="width: 100%">
        <table class="search-box" style="width: 100%">
            <tr>
                <td colspan="2">
                    <asp:Literal ID="litNavigation" runat="server"></asp:Literal></td>
            </tr>
            <tr>
                <td>服務院所：
                    <asp:DropDownList ID="ddlDeptSN" runat="server" ClientIDMode="Static" AutoPostBack="true" OnSelectedIndexChanged="ddlDeptSN_SelectedIndexChanged" />
                    <asp:HiddenField ID="hdDeptSN" runat="server" />
                </td>
                <td>醫師姓名：<asp:TextBox ID="txtName" runat="server"></asp:TextBox>

                </td>

                <td>狀態：
                     <asp:DropDownList ID="ddlStatus" runat="server" ClientIDMode="Static" AutoPostBack="true" OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged">
                         <asp:ListItem Text="全部" Value="" Selected="True" />
                         <asp:ListItem Text="啟用" Value="1" />
                         <asp:ListItem Text="停用" Value="2" />
                     </asp:DropDownList>
                    <asp:HiddenField ID="hdStatus" runat="server" />
                </td>
                <td style="text-align: right;">
                    <span class="submit">
                        <asp:Button ID="Button1" runat="server" Text="查詢" OnClick="btnSearch_Click" CssClass="btn" />
                    </span>
                    <asp:Button ID="btnClear" runat="server" Text="清除" OnClick="btnClear_Click" CssClass="btn" />
                </td>
            </tr>
            <tr>
                <td style="text-align: right" colspan="8">
                    <asp:Button ID="btnAdd" runat="server" Text="＋新增醫師" CssClass="btn" OnClick="btnAdd_Click" />
                </td>
            </tr>
        </table>
    </div>

    <asp:GridViewHeadText ID="gvIndex" runat="server" CssClass="case-list-table" AutoGenerateColumns="False" DataKeyNames="SN"
        EmptyShowHeader="True" Width="100%" OnRowDataBound="gvIndex_RowDataBound" OnRowCommand="gvIndex_RowCommand"
        GridLines="None" AllowPaging="True" AllowSorting="True" OnDataBound="gvIndex_DataBound"
        DataSourceID="odsIndex" odsSortReverse="False">
        <Columns>
            <asp:TemplateField HeaderText="動作">
                <ItemTemplate>
                    <asp:Button ID="btnEdit" runat="server" Text="修改" CssClass="img-btn edit" title="修改" CommandName="btnEdit" CommandArgument='<%# Eval("SN") %>' />
                    <asp:Button ID="btnRead" runat="server" Text="檢視" CssClass="img-btn ques" title="檢視" CommandName="btnRead" CommandArgument='<%# Eval("SN") %>' />
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="10%"></ItemStyle>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="醫師姓名" SortExpression="Name">
                <ItemTemplate>
                    <asp:HiddenField ID="SN" runat="server" Value='<%# Eval("SN") %>' />
                    <asp:Label ID="Name" runat="server" Text='<%# Eval("Name") %>'></asp:Label>
                </ItemTemplate>
                <ItemStyle Width="10%" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="服務院所" SortExpression="DeptSN">
                <ItemTemplate>
                    <asp:Label ID="DeptSN" runat="server" Text='<%# GetDept(string.Format("{0}",Eval("DeptSN"))) %>'></asp:Label>
                </ItemTemplate>
                <ItemStyle Width="10%" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="狀態" SortExpression="Status">
                <ItemTemplate>
                    <asp:Label ID="Status" runat="server" Text='<%# ("1".Equals(string.Format("{0}",Eval("Status")))?"啟用":"停用") %>'></asp:Label>
                </ItemTemplate>
                <ItemStyle Width="10%" />
            </asp:TemplateField>

        </Columns>

        <PagerSettings Visible="False"></PagerSettings>

    </asp:GridViewHeadText>
    <div class="list_gotopage" style="text-align: center; padding: 10px 0 0 0;">
        <asp:DataPager ID="DataPager1" runat="server" PagedControlID="gvIndex">
            <Fields>
                <asp:TemplatePagerField>
                    <PagerTemplate>
                        共
                                            <%# Container.TotalRowCount %>
                                            筆 |
                                            <%# Container.TotalRowCount > 0 ? Math.Ceiling(((double)(Container.StartRowIndex + Container.MaximumRows) / Container.MaximumRows)) : 0 %>
                                            /
                                            <%# Math.Ceiling((double)Container.TotalRowCount / Container.MaximumRows)%>
                                            頁 |
                    </PagerTemplate>
                </asp:TemplatePagerField>
                <asp:NextPreviousPagerField ShowFirstPageButton="True" ShowLastPageButton="True"
                    FirstPageText="第一頁 | " LastPageText="最後一頁" NextPageText="下一頁 | " PreviousPageText="上一頁 | " />
                <asp:TemplatePagerField>
                    <PagerTemplate>
                        到第
                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtCurrentPage"
                                                ErrorMessage="頁數指定錯誤!" ValidationExpression="\d{1,6}" ValidationGroup="gv">*</asp:RegularExpressionValidator>
                        <asp:TextBox ID="txtCurrentPage" runat="server" MaxLength="6" Width="20"></asp:TextBox>
                        頁 : 每頁
                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="txtPageSize"
                                                ErrorMessage="頁數大小指定錯誤!" ValidationExpression="\d{1,6}" ValidationGroup="gv">*</asp:RegularExpressionValidator>
                        <asp:TextBox ID="txtPageSize" runat="server" MaxLength="6" Width="20" Text='<%# Container.MaximumRows  %>'></asp:TextBox>
                        筆
                    </PagerTemplate>
                </asp:TemplatePagerField>
            </Fields>
        </asp:DataPager>
        <asp:Button ID="btnGo" runat="server" Text=" Go " OnClick="btnGo_Click" ValidationGroup="gv" CssClass="btn" />
    </div>
    <asp:ObjectDataSource ID="odsIndex" runat="server" SelectMethod="GetListData" SelectCountMethod="GetListCount"
        TypeName="Hamastar.BusinessObject.Comm_Doctor" EnablePaging="true" StartRowIndexParameterName="startRowIndex"
        MaximumRowsParameterName="maximumRows" SortParameterName="sortExpression" OnLoad="odsIndex_Load"></asp:ObjectDataSource>
</asp:Content>
