<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MP.master" AutoEventWireup="true" CodeFile="Detail.aspx.cs" Inherits="System_Satisfactionhealth_Detail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div align="right">
        <asp:Button ID="btnGoBack" runat="server" Text="回上一頁" class="btn" CausesValidation="False" />
    </div>

    <asp:Panel ID="Panel1" runat="server">
        <asp:Label ID="Case_information" runat="server" Text="Labe" Width="510px">個案申請資料</asp:Label>
        <table class="Detail-table" width="100%">
            <tr>
                <th class="td-title"><span class="ReqItem"></span>裝置前收件日期</th>
                <td>
                    <asp:TextBox ID="txtBeforDate" runat="server" MaxLength="100" Width="120px"></asp:TextBox>
                </td>
                <th class="td-title"><span class="ReqItem"></span>案件編號</th>
                <td>
                    <asp:TextBox ID="txtCaseNo" runat="server" MaxLength="100" Width="120px"></asp:TextBox>
                </td>
                <th class="td-title"><span class="ReqItem"></span>個案姓名</th>
                <td>
                    <asp:TextBox ID="txtName" runat="server" MaxLength="100" Width="250px"></asp:TextBox>
                </td>
            </tr>

            <tr>
                <th class="td-title"><span class="ReqItem"></span>性別</th>
                <td>
                    <asp:TextBox ID="txtGender" runat="server" MaxLength="120" Width="120px"></asp:TextBox>
                </td>
                <th class="td-title"><span class="ReqItem"></span>出生日期</th>
                <td>
                    <asp:TextBox ID="txtBirthday" runat="server" MaxLength="120" Width="120px"></asp:TextBox>
                </td>
                <th class="td-title"><span class="ReqItem"></span>身分證字號</th>
                <td>
                    <asp:TextBox ID="txtPID" runat="server" MaxLength="200" Width="250px"></asp:TextBox>
                </td>
            </tr>

            <tr>
                <th class="td-title"><span class="ReqItem"></span>電話</th>
                <td>
                    <asp:TextBox ID="txtTel" runat="server" MaxLength="100" Width="120px"></asp:TextBox>
                </td>
                <th class="td-title"><span class="ReqItem"></span>身分別</th>
                <td>
                    <asp:TextBox ID="txtUserType" runat="server" MaxLength="100" Width="120px"></asp:TextBox>
                </td>
                <th class="td-title"><span class="ReqItem"></span>地址</th>
                <td>
                    <asp:TextBox ID="txtAddr" runat="server" MaxLength="100" Width="250px"></asp:TextBox>
                </td>
            </tr>

            <tr>
                <th class="td-title"><span class="ReqItem"></span>合約院所</th>
                <td>
                    <asp:TextBox ID="txtDeptSN" runat="server" MaxLength="100" Width="120px"></asp:TextBox>
                </td>
                <th class="td-title"><span class="ReqItem"></span>負責醫生</th>
                <td>
                    <asp:TextBox ID="txtDoctorSN" runat="server" MaxLength="100" Width="120px"></asp:TextBox>
                </td>
                <th class="td-title"><span class="ReqItem"></span>裝置項目</th>
                <%--申請項目--%>
                <td>
                    <asp:TextBox ID="txtCatregory" runat="server" MaxLength="100" Width="250px"></asp:TextBox>
                </td>
            </tr>

            <tr>
                <th class="td-title">申請費用</th>
                <td>
                    <asp:TextBox ID="txtApplyAmount" runat="server" MaxLength="100" Width="120px"></asp:TextBox>
                </td>
                <th class="td-title"><span class="ReqItem" style="color: red">裝置前<br>
                </span>審查委員會日期</th>
                <td>
                    <asp:TextBox ID="txtProfDate" runat="server" MaxLength="100" Width="120px"></asp:TextBox>
                </td>
                <th class="td-title"><span class="ReqItem" style="color: red">裝置後<br>
                </span>審查委員會日期</th>
                <td>
                    <asp:TextBox ID="txtWriteOffProDate" runat="server" MaxLength="100" Width="250px"></asp:TextBox>
                </td>
            </tr>

        </table>
    </asp:Panel>
    <asp:Label ID="Initial_investigation" runat="server" Text="Labe" Width="510px">裝置完成初次調查</asp:Label>
    <table class="Detail-table" width="100%">
        <tr>
            <td>
                <asp:Label ID="labQuestionAns1" runat="server" Text="Label" Width="510px"><span class="ReqItem" style="color: red">*</span>1.請問您從何處知道本項補助計畫</asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <div class="margecol">
                    <asp:CheckBoxList ID="cblQuestionAns1" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" Enabled="False">
                        <asp:ListItem Value="電視">電視</asp:ListItem>
                        <asp:ListItem Value="報紙">報紙</asp:ListItem>
                        <asp:ListItem Value="廣播電台">廣播電台</asp:ListItem>
                        <asp:ListItem Value="村里廣播">村里廣播</asp:ListItem>
                        <asp:ListItem Value="垃圾巡迴車">垃圾巡迴車</asp:ListItem>
                        <asp:ListItem Value="網路">網路</asp:ListItem>
                        <asp:ListItem Value="縣市政府">縣市政府</asp:ListItem>
                        <asp:ListItem Value="衛生局所">衛生局所</asp:ListItem>
                        <asp:ListItem Value="醫療院所">醫療院所</asp:ListItem>
                        <asp:ListItem Value="鄰里長">鄰里長</asp:ListItem>
                        <asp:ListItem Value="村里幹事">村里幹事</asp:ListItem>
                        <asp:ListItem Value="親朋好友">親朋好友</asp:ListItem>
                        <asp:ListItem Value="其他">其他</asp:ListItem>
                    </asp:CheckBoxList>
                    <asp:TextBox ID="txbQuestionAns1Other" runat="server" Enabled="False" MaxLength="100" Width="250px"></asp:TextBox>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="labQuestionAns2" runat="server" Text="Label" Width="510px"><span class="ReqItem" style="color: red">*</span>2.請問目前您假牙裝置後有無使用？</asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <asp:RadioButtonList ID="rdlQuestionAns2" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" Enabled="False">
                    <asp:ListItem Value="幾乎天天">幾乎天天</asp:ListItem>
                    <asp:ListItem Value="5~6天">5~6天</asp:ListItem>
                    <asp:ListItem Value="3~4天">3~4天</asp:ListItem>
                    <asp:ListItem Value="1~2天">1~2天</asp:ListItem>
                    <asp:ListItem Value="沒再使用">沒再使用</asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="labQuestionAns3" runat="server" Text="Label" Width="510px"><span class="ReqItem" style="color: red">*</span>3.請問您對假牙裝置後是否舒適？</asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <asp:RadioButtonList ID="rdlQuestionAns3" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" AutoPostBack="true" OnSelectedIndexChanged="rdlQuestionAns3_SelectedIndexChanged" Enabled="False">
                    <asp:ListItem Value="非常舒適">非常舒適</asp:ListItem>
                    <asp:ListItem Value="舒適">舒適</asp:ListItem>
                    <asp:ListItem Value="沒意見">沒意見</asp:ListItem>
                    <asp:ListItem Value="不舒適">不舒適</asp:ListItem>
                    <asp:ListItem Value="非常不舒適">非常不舒適</asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>

        <tr runat="server" id="tr_QuestionAns4">
            <td>
                <asp:Label ID="labQuestionAns4" runat="server" Text="Label" Width="510px">承上題，請問您裝置後不舒適的原因是？</asp:Label>
            </td>
        </tr>
        <tr runat="server" id="idRadioButtonList2">
            <td>
                <asp:RadioButtonList ID="rdlQuestionAns4" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" Enabled="False">
                    <asp:ListItem Value="咬合太緊">咬合太緊</asp:ListItem>
                    <asp:ListItem Value="咬合鬆動">咬合鬆動</asp:ListItem>
                    <asp:ListItem Value="口腔潰爛">口腔潰爛</asp:ListItem>
                    <asp:ListItem Value="使用不習慣">使用不習慣</asp:ListItem>
                    <asp:ListItem Value="其他">其他</asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>
        <tr runat="server" id="tr_QuestionAns5">
            <td>
                <asp:Label ID="labQuestionAns5" runat="server" Text="Label" Width="510px">承上題，目前您裝置後不舒適回診狀況？</asp:Label>
            </td>
        </tr>
        <tr runat="server" id="idRadioButtonList3">
            <td>
                <asp:RadioButtonList ID="rdlQuestionAns5" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" Enabled="False">
                    <asp:ListItem Value="持續回診">持續回診</asp:ListItem>
                    <asp:ListItem Value="拒絕回診">拒絕回診</asp:ListItem>
                    <asp:ListItem Value="有空再去">有空再去</asp:ListItem>
                    <asp:ListItem Value="疾病因素無法前往">疾病因素無法前往</asp:ListItem>
                </asp:RadioButtonList>
                <asp:DropDownList ID="ddlQuestionAns5" runat="server" Enabled="False">
                    <asp:ListItem Value="0">請選擇疾病原因</asp:ListItem>
                    <asp:ListItem Value="死亡">死亡</asp:ListItem>
                    <asp:ListItem Value="住養護、長照中心">住養護、長照中心</asp:ListItem>
                    <asp:ListItem Value="身體狀況不佳">身體狀況不佳</asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="labQuestionAns6" runat="server" Text="Label" Width="510px"><span class="ReqItem" style="color: red">*</span>4.您對縣長重要施政「65歲以上長者及55歲以上原住民裝置假牙補助計畫」滿意嗎？</asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <asp:RadioButtonList ID="rdlQuestionAns6" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" AutoPostBack="True" OnSelectedIndexChanged="rdlQuestionAns6_SelectedIndexChanged" Enabled="False">
                    <asp:ListItem Value="非常滿意">非常滿意</asp:ListItem>
                    <asp:ListItem Value="滿意">滿意</asp:ListItem>
                    <asp:ListItem Value="沒意見">沒意見</asp:ListItem>
                    <asp:ListItem Value="不滿意">不滿意</asp:ListItem>
                    <asp:ListItem Value="非常不滿意">非常不滿意</asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>
        <tr runat="server" id="tr_QuestionAns7">
            <td>
                <asp:Label ID="labQuestionAns7" runat="server" Text="Label" Width="510px">承上題，請問您不滿意的原因是？</asp:Label>
            </td>
        </tr>
        <tr runat="server" id="idRadioButtonList5">
            <td>
                <asp:RadioButtonList ID="rdlQuestionAns7" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" AutoPostBack="True" Enabled="False">
                    <asp:ListItem Value="等候裝置時間太久">等候裝置時間太久</asp:ListItem>
                    <asp:ListItem Value="申請程序複雜">申請程序複雜</asp:ListItem>
                    <asp:ListItem Value="政府部門核准作業太久">政府部門核准作業太久</asp:ListItem>
                    <asp:ListItem Value="醫療院所服務態度不佳">醫療院所服務態度不佳</asp:ListItem>
                    <asp:ListItem Value="其他">其他</asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="labMemo" runat="server" Text="Label" Width="510px">請問您對於假牙裝置補助是否有其他建議事項？</asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <asp:TextBox ID="txtMemo" runat="server" Height="80px" Width="100%" TextMode="MultiLine" Enabled="False"></asp:TextBox>
            </td>
        </tr>

    </table>

    <asp:Label ID="labDissatistrac" runat="server" Text="Label" Width="510px">不滿意後續追蹤</asp:Label>
    <table class="Detail-table" width="100%">
        <tr runat="server" id="tr_Dissatistracktopic">
            <td>
                <asp:Label ID="Asterisk" runat="server" Text="*" Style="color: red"></asp:Label>
                <asp:Label ID="labDissatistracktopic" runat="server" Text="Label" Width="510px"></asp:Label>
            </td>
        </tr>
        <tr runat="server" id="tr_Dissatistrackoptions">
            <td>
                <%--不滿意後續追蹤--%>
                <asp:RadioButtonList ID="rblDissatistrackoptions" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" Enabled="False">
                </asp:RadioButtonList>
            </td>
        </tr>
        <tr>
            <td>
                <asp:CheckBoxList ID="cblHealthcenter" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                    <asp:ListItem Value="個案轉衛生所追蹤" Enabled="False">個案轉衛生所追蹤</asp:ListItem>
                </asp:CheckBoxList>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="labRemark" runat="server" Text="Label" Width="510px">備註：</asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <asp:TextBox ID="txtRemark" runat="server" Height="80px" Width="100%" TextMode="MultiLine"></asp:TextBox>
            </td>
        </tr>
    </table>

    <div style="text-align: center; padding-top: 10px">
        <asp:Button ID="btnSave" runat="server" Text="確定" class="edit-btn" OnClick="btnSave_Click" />
        &nbsp;
        <asp:Button ID="btnCancel" runat="server" Text="取消" class="edit-btn" CausesValidation="False" />
    </div>
</asp:Content>

