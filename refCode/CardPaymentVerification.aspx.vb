Imports DWD.UI.SUITES.BusinessService.BusinessObject
Imports DWD.UI.SUITES.Utility

Partial Public Class CardPaymentVerification
    Inherits SuitesWebPage

    Public Sub New()
        'This page does require an employer to load
        MyBase.New(True, True)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Me.IsPostBack Then

            Dim pgCd As WebPageCode = SuitesWebPage.ResolvePathToPageCode(Me.Request.AppRelativeCurrentExecutionFilePath())
            lnkHelp.NavigateUrl = SuitesWebPage.GetHelpPath(Me.Request, pgCd)
            lnkLogOut.NavigateUrl = SuitesWebPage.GetURL(PageName.Logout)
            lnkServiceProviderHome.NavigateUrl = SuitesWebPage.GetURL(PageName.ServiceProviderHome)
            lnkContactUs.NavigateUrl = Me.Request.ApplicationPath + "/ContactUs.aspx"

            Dim isServiceProvider As NullableBoolean = PortalUtility.GetIsServiceProvider(Me.Session)
            If isServiceProvider.IsNull Then
                If SuitesWebPage.GetServiceProviderPages.Contains(pgCd) Then
                    isServiceProvider = NullableBoolean.True
                ElseIf SuitesWebPage.GetEmployerPages.Contains(pgCd) Then
                    isServiceProvider = NullableBoolean.False
                Else
                    'there is the also the possibility the page has not been defined or is neither specific to SP or Employer
                    isServiceProvider = NullableBoolean.Null
                End If
                PortalUtility.SetIsServiceProvider(Me.Session, isServiceProvider)
            End If

            lnkServiceProviderHome.Visible = isServiceProvider.IsTrue

            'check eBill configuration
            If PortalUtility.GetEbillConfiguration(HttpContext.Current) Is Nothing Then
                PortalUtility.SetupEbillConfiguration(HttpContext.Current)
            End If

            'get payment data and load verification form
            Dim pymtData As CardPaymentData = PortalUtility.GetCardPaymentData(Me.Session)

            If pymtData IsNot Nothing AndAlso pymtData.PaymentAmount.IsNotNull Then
                lblPaymentAmount.Text = pymtData.PaymentAmountString.Value
            End If

            Dim reg As CardRegistration = PortalUtility.GetCardPaymentRegisration(Me.Session)
            If reg IsNot Nothing Then

                lblContactName.Text = reg.FirstName.ToString() + " " + reg.LastName.ToString()
                lblEmailAddress.Text = reg.EmailAddress.ToString()

                lblLegalName.Text = Me.Employer.LegalName.ToString()
                lblAccount.Text = Me.Employer.UIAccountNumberFormatted

                If reg.CountryCode.ToString() = "US" Then
                    lblAddress.Text = reg.StreetAddressLine1.ToString()
                    lblAddress2.Text = reg.StreetAddressLine2.ToString()
                    lblAddress3.Text = String.Format("{0}, {1}  {2}", reg.CityName.ToString(), reg.State.ToString(), reg.ZipCode.ToString())
                Else
                    lblAddress.Text = reg.CityName.ToString()
                    lblAddress2.Text = reg.CountryCode.ToString()
                    lblAddress3.Text = reg.ZipCode.ToString()
                End If
            End If

        End If

    End Sub

    Protected Friend Function GetHostedForm() As String
        Dim pymtData As CardPaymentData = PortalUtility.GetCardPaymentData(Me.Session)
        Dim amt As String = String.Empty
        If Not pymtData Is Nothing AndAlso pymtData.PaymentAmount.IsNotNull Then
            amt = pymtData.PaymentAmountString.Value
        End If

        Dim reg As CardRegistration = PortalUtility.GetCardPaymentRegisration(Me.Session)
        Dim configProxy As ExternalService.CardPaymentEbillConfigProxy = PortalUtility.GetEbillConfiguration(HttpContext.Current)
        Dim dataclientkey As String = configProxy.TaxClientKey

        Dim formID As String = "orbipay-checkout-form"
        Dim scriptID As String = "orbipay-checkout-script"
        Dim url As String = configProxy.HostedFormURL

        Dim form As New StringBuilder()
        Dim quote As Char = ChrW(34)
        Dim customerID As String = reg.UserIDDecrypted.ToString()
        If customerID = "N/A" Then Throw New ApplicationException("Unable to decrypt eBill User ID.  Value was N/A.")
        Dim customerAccountRef As String = reg.CardRegistrationSK.ToString()

        Dim customerEmail As String = reg.EmailAddress.Value
        Dim lastName As String = reg.LastName.Value
        Dim firstName As String = reg.FirstName.Value
        Dim address1 As String = reg.StreetAddressLine1.Value
        Dim address2 As String = String.Empty
        If reg.StreetAddressLine2.IsNotNull Then address2 = reg.StreetAddressLine2.Value
        Dim city As String = reg.CityName.Value
        Dim state As String = reg.State.Value
        Dim country As String = CountryCode.LoadByShortDescription(reg.CountryCode.Value).ISOCode
        Dim zip As String = reg.ZipCode.Value
        Dim postalCode As String = String.Empty
        If reg.CountryCode.Value <> "US" Then
            postalCode = reg.ZipCode.Value
            zip = ""
        End If
        Dim paymentOption As String = ConfigurationManager.AppSettings("PaymentOption")
        Dim paymentOptionReadOnly As String = ConfigurationManager.AppSettings("PaymentOptionReadOnly")

        Dim apievent As String = "create_payment"
        Dim amount As String = amt

        form.AppendLine($"<form id={quote}{formID}{quote} action={quote}CardPaymentPost.aspx{quote} method={quote}POST{quote}>")
        form.AppendLine($"<script id={quote}{scriptID}{quote} src={quote}{url}{quote}")
        form.AppendLine($"data-id_customer={quote}{customerID}{quote}")
        form.AppendLine($"data-customer_account_reference={quote}{customerAccountRef}{quote}")
        form.AppendLine($"data-customer_email={quote}{customerEmail}{quote}")
        form.AppendLine($"data-customer_last_name={quote}{lastName}{quote}")
        form.AppendLine($"data-customer_first_name={quote}{firstName}{quote}")
        form.AppendLine($"data-customer_address_line1={quote}{address1}{quote}")
        form.AppendLine($"data-customer_address_line2={quote}{address2}{quote}")
        form.AppendLine($"data-customer_city={quote}{city}{quote}")
        form.AppendLine($"data-customer_state={quote}{state}{quote}")
        form.AppendLine($"data-customer_country={quote}{country}{quote}")
        form.AppendLine($"data-customer_zip_code1={quote}{zip}{quote}")
        form.AppendLine($"data-customer_postal_code={quote}{postalCode}{quote}")
        form.AppendLine($"data-payment_option={quote}{paymentOption}{quote}")
        form.AppendLine($"data-payment_option_readonly={quote}{paymentOptionReadOnly}{quote}")
        form.AppendLine($"data-client_key={quote}{dataclientkey}{quote}")
        form.AppendLine($"data-api_event={quote}{apievent}{quote}")
        form.AppendLine($"data-amount={quote}{amount}{quote}>")
        form.AppendLine("</script>")
        form.AppendLine("</form>")

        Return form.ToString()
    End Function

End Class