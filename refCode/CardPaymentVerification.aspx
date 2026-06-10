<%@ Page Language="vb" AutoEventWireup="false" Codebehind="CardPaymentVerification.aspx.vb" Inherits="DWD.UI.SUITES.UserService.Web.CardPaymentVerification" Title="Card Payment Verification" %>
<html>
<head runat="server">
   <title>Payment Verification</title>
    
   <link rel="STYLESHEET" type="text/css" href="../include/dwdSUITES.css"  media="screen, projection" />
   <link rel="STYLESHEET" type="text/css" href="../include/print.css"  media="print" />
   <%  
      'timeout is in minutes - convert to seconds
      Response.Write("<meta http-equiv=""Refresh"" content=""" & (Me.Page.Session.Timeout * 60).ToString & ";url=" & Me.Request.ApplicationPath & "/SystemTimeout.aspx"" />")
      Response.Write(vbCrLf)
   %>
</head>

<body>
   <div id="divouter" >
         <!--  Header start  -->
     <div class="FixedTotalWidth BannerBackground" >
      <table class="FixedTotalWidth">
         <tr>
            <td rowspan="2" style="text-align:left">
               <img id="Img2" runat="server" src="../images/dwd_logo_2011.png" width="225" height="60"  alt="" />
            </td>
            <td class="HeaderLinkBox">
                  <asp:HyperLink ID="lnkServiceProviderHome" CssClass="HeaderAnchor" runat="server">Service Provider Home</asp:HyperLink>&nbsp;
                  <asp:HyperLink ID="lnkHelp" CssClass="HeaderAnchor" runat="server"  Target="_blank">Help</asp:HyperLink>&nbsp;
                  <asp:HyperLink ID="lnkContactUs" CssClass="HeaderAnchor" runat="server" Target="_blank">Contact Us</asp:HyperLink>&nbsp;
                  <asp:HyperLink ID="lnkLogOut" CssClass="HeaderAnchor" runat="server">Log Out</asp:HyperLink>
            </td>
         </tr>
         <tr>
            <td class="PageHeaderText" style="text-align:right;padding:0px 10px 0px 0px;">
            Department of Workforce Development<br />
            <asp:HyperLink ID="HyperLink1" CssClass="UIAnchor" runat="server">Unemployment Insurance</asp:HyperLink>
            </td>
         </tr>
      </table>      
     </div>

     <asp:Panel ID="pnlSingleAccount" runat="server" CssClass="FixedTotalWidth">
          <br />
          <asp:Label ID="Label1" runat="server" Font-Bold="True" Text="Account:"></asp:Label>
          <asp:Label ID="lblAccount" runat="server"></asp:Label>
          <asp:Label ID="Label2" runat="server" Font-Bold="True" Text="Legal Name:"></asp:Label>
          <asp:Label ID="lblLegalName" runat="server"></asp:Label>
          <br />
      </asp:Panel>
      <br />
      <table class="FixedTotalWidth">
         <tr>
            <td class="bordercell"></td>
            <td class="bordercell"></td>
            <td class="bordercell centered">
               <span><a id="skip_mainmenu"></a></span>
               <h1>Payment Verification</h1>   
            </td>
         </tr>
         <tr>
            <td rowspan="3" class="SideMenuPlaceHolder"></td>
            <td rowspan="3" class="SideMenuSpacer">&nbsp;</td>
            <td class="centered">
               
            </td>
         </tr>
          <tr>
              <td style="vertical-align:top;">
                 <div class="alert alert-secondary">
                     <div class="row">
                           <p>
                              Please verify the payment information is correct. &nbsp;If the information is correct, click "Next" to continue.
                                &nbsp;If the information is not correct, click "Previous" to update the information.
                           </p>
                      </div>
                 </div>
                <table class="FixedWidth">
                   <tr>
                       <td class="RowHeader">Payment Amount:</td>
                       <td><asp:Label ID="lblPaymentAmount" runat="server" Text=""></asp:Label></td>
                   </tr>
               </table>
               <table class="FixedWidth">
                   <tr>
                       <td class="RowHeader">Contact Name:</td>
                       <td><asp:Label ID="lblContactName" runat="server" Text=""></asp:Label></td>
                   </tr>
                   <tr>
                       <td class="RowHeader">Email Address:</td>
                       <td><asp:Label ID="lblEmailAddress" runat="server" Text=""></asp:Label></td>
                   </tr>
                  <tr>
                       <td class="RowHeader">Address:</td>
                       <td><asp:Label ID="lblAddress" runat="server" Text=""></asp:Label></td>
                   </tr>
                  <tr>
                       <td class="RowHeader"></td>
                       <td><asp:Label ID="lblAddress2" runat="server" Text=""></asp:Label></td>
                   </tr>
                  <tr>
                       <td class="RowHeader"></td>
                       <td><asp:Label ID="lblAddress3" runat="server" Text=""></asp:Label></td>
                   </tr>
               </table>
              </td>
          </tr>
         <tr>
            <td class="centered">
                <br />
               <div>
                  <button class="btnHeaderMenu" type="button" id="btnprev" onclick="javascript:history.back()">Previous</button>&nbsp;&nbsp;
                  <button id="orbipay-checkout-button" class="btnHeaderMenu">Next</button>
                  <%=Me.GetHostedForm()%>
               </div>
            </td>
         </tr>
      </table>     
</div>
</body>
   
</html>