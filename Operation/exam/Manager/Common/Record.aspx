<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/DealData.master" AutoEventWireup="true" CodeFile="Record.aspx.cs" Inherits="Common_Record" %>

<%@ Register Assembly="Hamastar.GridViewHead" Namespace="GridViewHead" TagPrefix="asp" %>
<%@ Register Src="~/Common/Date.ascx" TagPrefix="uc1" TagName="Date" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="content" runat="Server">
    <div class="fieldset" runat="server" id="fieldset">
        <table class="search-box" style="width: 100%">

            <tr>
                <td class="td-title">
                    <span>修改日期：<uc1:Date runat="server" ID="SDate" DateTimeValidatorEnableFlag="false" />
                        ~
                             <uc1:Date runat="server" ID="EDate" DateTimeValidatorEnableFlag="false" />
                        <br />
                        案號：<asp:TextBox ID="txtCaseNo" runat="server"></asp:TextBox>
                        單元名稱：<asp:DropDownList ID="ddlNodeName" runat="server">
                            <asp:ListItem Value="0" Selected="True">全部</asp:ListItem>
                        </asp:DropDownList>
                        <br />
                        帳號：<asp:TextBox ID="txtID" runat="server"></asp:TextBox>
                        姓名：<asp:TextBox ID="txtName" runat="server"></asp:TextBox>
                        <asp:Button ID="btnSearch" runat="server" Text="查詢" OnClick="btnSearch_Click" CssClass="btn" />
                        <asp:Button ID="btnExcel" runat="server" Text="匯出Excel" OnClick="btnExcel_Click" CssClass="btn" OnClientClick="CCMS_DisableLockUI();" />
                    </span>
                </td>
            </tr>
        </table>
    </div>
    <asp:GridViewHeadText ID="gvIndex" runat="server" CssClass="case-list-table" AutoGenerateColumns="False" DataKeyNames="SN"
        EmptyShowHeader="True" Width="100%" OnRowDataBound="gvIndex_RowDataBound" OnRowCommand="gvIndex_RowCommand"
        GridLines="None" AllowPaging="True" AllowSorting="True" OnDataBound="gvIndex_DataBound"
        DataSourceID="odsIndex" odsSortReverse="False">
        <Columns>
            <asp:BoundField DataField="ModifyAccountID" HeaderText="帳號" SortExpression="ModifyAccountID" HeaderStyle-HorizontalAlign="Left">
                <HeaderStyle HorizontalAlign="Left"></HeaderStyle>

                <ItemStyle Width="10%"></ItemStyle>
            </asp:BoundField>
            <asp:BoundField DataField="Name" HeaderText="姓名" SortExpression="Name" HeaderStyle-HorizontalAlign="Left">
                <HeaderStyle HorizontalAlign="Left"></HeaderStyle>

                <ItemStyle Width="15%"></ItemStyle>
            </asp:BoundField>
            <asp:BoundField DataField="CaseNo" HeaderText="案號" SortExpression="CaseNo" HeaderStyle-HorizontalAlign="Left">
                <HeaderStyle HorizontalAlign="Left"></HeaderStyle>

                <ItemStyle Width="8%"></ItemStyle>
            </asp:BoundField>
            <asp:BoundField DataField="NodeName" HeaderText="單元名稱" SortExpression="NodeName" HeaderStyle-HorizontalAlign="Left">
                <HeaderStyle HorizontalAlign="Left"></HeaderStyle>

                <ItemStyle Width="15%"></ItemStyle>
            </asp:BoundField>
            <asp:BoundField DataField="ModifyDate" HeaderText="修改時間" SortExpression="ModifyDate" DataFormatString="{0:yyyy/MM/dd HH:mm:ss}" HeaderStyle-HorizontalAlign="Left">
                <HeaderStyle HorizontalAlign="Left"></HeaderStyle>

                <ItemStyle Width="20%"></ItemStyle>
            </asp:BoundField>
            <asp:BoundField DataField="IP" HeaderText="IP" SortExpression="IP" HeaderStyle-HorizontalAlign="Left">
                <HeaderStyle HorizontalAlign="Left"></HeaderStyle>

                <ItemStyle Width="10%"></ItemStyle>
            </asp:BoundField>
            <asp:TemplateField HeaderText="操作內容" SortExpression="Record">
                <ItemTemplate>
                    <asp:Label ID="Label1" runat="server" Text='<%# Eval("Record")==null?"":Eval("Record").ToString().IndexOf("@@")>-1?Eval("Record").ToString().Substring(0,Eval("Record").ToString().IndexOf("@@")):Eval("Record").ToString() %>' ToolTip='<%# Eval("Record")==null?"":Eval("Record").ToString().IndexOf("@@")>-1?Eval("Record").ToString().Replace("@@","\n").Replace("<br/>","\n"):"" %>'></asp:Label>
                </ItemTemplate>
                <HeaderStyle HorizontalAlign="Left" />
                <ItemStyle Width="22%" />
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
        <asp:Button ID="btnGo" runat="server" Text=" Go " OnClick="btnGo_Click" ValidationGroup="gv" />
    </div>

    <asp:ObjectDataSource ID="odsIndex" runat="server" SelectMethod="GetListData" SelectCountMethod="GetListCount"
        TypeName="Hamastar.BusinessObject.vw_Record" EnablePaging="True" StartRowIndexParameterName="startRowIndex"
        MaximumRowsParameterName="maximumRows" SortParameterName="sortExpression" OnLoad="odsIndex_Load"></asp:ObjectDataSource>
</asp:Content>

