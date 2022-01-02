<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MP.master" AutoEventWireup="true" CodeFile="Query.aspx.cs" Inherits="System_CaseCL_Query" %>

<%@ Register Src="~/Common/Date.ascx" TagPrefix="uc1" TagName="Date" %>
<%@ Register Src="~/Common/wucDateTime.ascx" TagPrefix="uc1" TagName="wucDateTime" %>
<%@ Register Assembly="Hamastar.GridViewHead" Namespace="GridViewHead" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <br />

    <div class="fieldset" style="width: 100%">
        <table class="search-box" style="width: 100%">
            <tr>
                <td colspan="2">
                    <asp:Literal ID="litNavigation" runat="server"></asp:Literal></td>
            </tr>
            <tr>
                <td>收件日期期間：</td>
                <td>
                    <uc1:wucDateTime runat="server" ID="txtBeforDateS" IsShowTime="false" IsShowDefaultDate="false" DateTimeValidatorEnableFlag="false" />
                    至
                    <uc1:wucDateTime runat="server" ID="txtBeforDateE" IsShowTime="false" IsShowDefaultDate="false" DateTimeValidatorEnableFlag="false" />
                </td>
                <td>案件狀態：</td>
                <td>
                    <asp:DropDownList ID="ddlStatus" runat="server" ClientIDMode="Static" AutoPostBack="true" OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged">
                        <asp:ListItem Text="請選擇" Value="" />
                        <asp:ListItem Text="專業審查通過" Value="專業審查通過" />
                        <asp:ListItem Text="核銷審查退件" Value="核銷審查退件" />
                    </asp:DropDownList>
                    <asp:HiddenField ID="hdStatus" runat="server" />
                </td>
            </tr>
            <tr>
                <td>醫療院所名稱：</td>
                <td>
                    <asp:DropDownList ID="ddlDeptSN" runat="server" ClientIDMode="Static" AutoPostBack="true" OnSelectedIndexChanged="ddlDeptSN_SelectedIndexChanged" />
                    <asp:HiddenField ID="hdDeptSN" runat="server" />
                </td>
                <td>個案身分證字號：</td>
                <td>
                    <asp:TextBox runat="server" ID="txtPID" />
                </td>
            </tr>
            <tr>
                <td>是否移送社會局：</td>
                <td colspan="3">
                    <asp:RadioButtonList ID="rdbWriteOffTransfer" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" AutoPostBack="true">
                        <asp:ListItem Value="是">是</asp:ListItem>
                        <asp:ListItem Value="否" Selected="True">否</asp:ListItem>
                    </asp:RadioButtonList>
                </td>
            </tr>
            <tr>
                <td style="text-align: center;" colspan="4">
                    <span class="submit">
                        <asp:Button ID="btnSearch" runat="server" Text="查詢" CssClass="btn" OnClick="btnSearch_Click" />
                    </span>
                    <asp:Button ID="btnClear" runat="server" Text="清除" CssClass="btn" OnClick="btnClear_Click" />
                </td>
            </tr>

            <tr>
                <td style="text-align: right" colspan="4">
                    <asp:Button ID="btnExport" runat="server" Text="匯出" CssClass="btn" />
                    <asp:Button ID="btnCertNo" runat="server" Text="核銷憑證輸入" CssClass="btn" />
                </td>
            </tr>
        </table>
    </div>

    <asp:GridViewHeadText ID="gvIndex" runat="server" CssClass="case-list-table" AutoGenerateColumns="False" DataKeyNames="CaseNo"
        EmptyShowHeader="True" Width="100%" OnRowDataBound="gvIndex_RowDataBound" OnRowCommand="gvIndex_RowCommand"
        GridLines="None" AllowPaging="True" AllowSorting="True" OnDataBound="gvIndex_DataBound"
        DataSourceID="odsIndex" odsSortReverse="False">
        <Columns>
            <asp:TemplateField HeaderText="勾選">
                <ItemTemplate>
                    <asp:CheckBox runat="server" ID="cbxSelect" />
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="10%"></ItemStyle>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="功能">
                <ItemTemplate>
                    <asp:Button ID="btnRead" runat="server" Text="檢視" CssClass="img-btn ques" title="檢視" CommandName="btnRead" CommandArgument='<%# Eval("CaseNo") %>' />
                    <asp:Button ID="btnLog" runat="server" CssClass="img-btn clock" title="歷程" CommandName="btnLog" CommandArgument='<%# Eval("CaseNo") %>' />
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="10%"></ItemStyle>
            </asp:TemplateField>
            <asp:BoundField DataField="CaseNo" HeaderText="案件編號" SortExpression="CaseNo" HeaderStyle-HorizontalAlign="Center">
                <HeaderStyle HorizontalAlign="Center" Width="10%" />
                <ItemStyle HorizontalAlign="Center" />
            </asp:BoundField>
            <asp:BoundField DataField="Name" HeaderText="個案姓名" SortExpression="Name" HeaderStyle-HorizontalAlign="Center">
                <HeaderStyle HorizontalAlign="Center" Width="10%" />
                <ItemStyle HorizontalAlign="Center" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="收件日期" SortExpression="BeforDate">
                <ItemTemplate>
                    <asp:Label ID="BeforDate" runat="server" Text='<%# string.Format("{0:yyy-MM-dd}",Eval("BeforDate")) %>'></asp:Label>
                </ItemTemplate>
                <ItemStyle Width="10%" HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="院所名稱" SortExpression="DeptSN">
                <ItemTemplate>
                    <asp:Label ID="DeptSN" runat="server" Text='<%#  GetDept(string.Format("{0}",Eval("DeptSN"))) %>'></asp:Label>
                </ItemTemplate>
                <ItemStyle Width="10%" HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:BoundField DataField="申請項目" HeaderText="申請項目" SortExpression="申請項目" HeaderStyle-HorizontalAlign="Center">
                <HeaderStyle HorizontalAlign="Center" Width="20%" />
                <ItemStyle HorizontalAlign="Left" />
            </asp:BoundField>
            <asp:BoundField DataField="Status" HeaderText="結案類型" SortExpression="Status" HeaderStyle-HorizontalAlign="Center">
                <HeaderStyle HorizontalAlign="Center" Width="10%" />
                <ItemStyle HorizontalAlign="Center" />
            </asp:BoundField>
            <asp:BoundField DataField="CertNo" HeaderText="憑證編號" SortExpression="Status" HeaderStyle-HorizontalAlign="Center">
                <HeaderStyle HorizontalAlign="Center" Width="10%" />
                <ItemStyle HorizontalAlign="Center" />
            </asp:BoundField>
            <asp:BoundField DataField="WriteOffTransfer" HeaderText="移送社會局" SortExpression="Status" HeaderStyle-HorizontalAlign="Center">
                <HeaderStyle HorizontalAlign="Center" Width="10%" />
                <ItemStyle HorizontalAlign="Center" />
            </asp:BoundField>
        </Columns>
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
        TypeName="Hamastar.BusinessObject.vw_Case" EnablePaging="True" StartRowIndexParameterName="startRowIndex"
        MaximumRowsParameterName="maximumRows" SortParameterName="sortExpression" OnLoad="odsIndex_Load"></asp:ObjectDataSource>

</asp:Content>

