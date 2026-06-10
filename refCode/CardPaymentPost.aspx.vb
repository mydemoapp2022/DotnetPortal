Imports System.IO
Imports System.Linq
Imports Com.Alacriti.Checkout.Model
Imports DWD.UI.SUITES.BusinessService.BusinessObject
Imports DWD.UI.SUITES.ExternalService

Partial Public Class CardPaymentPost
    Inherits SuitesWebPage

    Public Sub New()
        'This page requires an employer to load
        MyBase.New(False, True)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try

            Dim configProxy As CardPaymentEbillConfigProxy = PortalUtility.GetEbillConfiguration(HttpContext.Current)

            Dim client_key As String
            Dim signature_key As String
            Dim client_api_key As String
            Dim client_private_key As String
            Dim hwf_public_key As String = configProxy.PublicKey
            Dim live_mode As String = configProxy.LiveMode.ToString().ToLower()
            Dim paymentDescription As String
            Dim paymentType As String = "Employer"
            Dim website As String
            Dim customerServiceNumber As String
            Dim amount As String = "0"

            Dim pymtData As CardPaymentData = PortalUtility.GetCardPaymentData(Me.Session)
            If pymtData IsNot Nothing Then
                If pymtData.PaymentAmount.IsNotNull Then
                    amount = pymtData.PaymentAmountString.Value
                End If
                If pymtData.Voluntary Then
                    paymentType = "Voluntary"
                End If
            End If

            client_key = configProxy.TaxClientKey
            signature_key = configProxy.TaxSecretKey
            client_api_key = configProxy.TaxAPIKey
            client_private_key = configProxy.TaxPrivateKey
            paymentDescription = "Employer Portal"

            website = configProxy.EmployerCollectionsWebsite
            customerServiceNumber = configProxy.EmployerCollectionsPhoneNumber

            'Grab the request token
            Dim req As StreamReader = New StreamReader(Request.InputStream, Request.ContentEncoding)
            Dim tokenstring As String = HttpUtility.UrlDecode(req.ReadToEnd())
            Dim rspToken As ResponseToken = GetResponseToken(tokenstring)

            Dim token As String = rspToken.Token
            Dim digi_sign As String = rspToken.DigiSign
            Dim customer_account_reference As String = rspToken.Customer_Account_Reference

            Dim customerAccountCustomFields As Dictionary(Of String, String) = New Dictionary(Of String, String)()
            customerAccountCustomFields.Add("cdf001", Me.Employer.LegalName.Value.First(64))
            customerAccountCustomFields.Add("cdf002", paymentDescription)
            customerAccountCustomFields.Add("cdf003", Me.Employer.UIAccountNumber.Value)
            customerAccountCustomFields.Add("cdf004", Me.Employer.AccountNumber.Value)
            customerAccountCustomFields.Add("cdf005", paymentType)
            customerAccountCustomFields.Add("cdf006", website.First(64))
            customerAccountCustomFields.Add("cdf007", customerServiceNumber.First(64))
            customerAccountCustomFields.Add("cdf008", Me.Employer.AccountNumber.Value.Substring(0, 5))

            Dim invocation_context As Com.Alacriti.Checkout.Api.InvocationContext = New Com.Alacriti.Checkout.Api.InvocationContext(client_api_key, client_private_key, hwf_public_key)

            Dim payment As Payment = New Com.Alacriti.Checkout.Api.Payment(customer_account_reference, amount).withToken(token, digi_sign).forClient(client_key, signature_key, client_api_key).withCustomFields(customerAccountCustomFields).confirm(invocation_context, live_mode)

            If payment IsNot Nothing AndAlso payment.Error Is Nothing Then
                'save card payment And CCDC Confirmation to SUITES
                SaveCardPayment(payment, pymtData.Voluntary)

                pymtData.ConfirmationNumber = payment.ConfirmationNumber
                PortalUtility.SetCardPaymentData(Me.Session, pymtData)

                'route to confirmaton page
                Me.Navigate(PageName.CardPaymentConfirmation)

            Else
                Dim errorString As String = String.Empty
                Dim errorCodes As String = String.Empty
                Dim errorField As String = String.Empty
                Dim phoneNumber As String = String.Empty
                Dim displayErrorCodes As String = configProxy.DisplayErrorCodes
                Dim displayError As Boolean = False

                For Each err As Com.Alacriti.Checkout.Model.Error In payment.Error
                    errorString += err.Message & " "
                    errorString += err.Field & " "
                    errorField += err.Field & " "
                    errorCodes += err.Code & " "

                    If err.Code <> "0" AndAlso displayErrorCodes.Contains(err.Code) Then
                        displayError = True
                    End If
                Next

                phoneNumber = configProxy.EmployerCollectionsPhoneNumber

                'log the error returned
                Dim paymentEx As New ApplicationException(errorString)
                PortalUtility.PublishPortalException(paymentEx)

                'route to error page
                pymtData.ErrorDescription = errorString
                pymtData.PhoneNumber = phoneNumber
                pymtData.DisplayError = displayError

                PortalUtility.SetCardPaymentData(Me.Session, pymtData)
                Me.Navigate(PageName.CardPaymentError)
            End If

        Catch ex As System.Exception
            PortalUtility.HandlePortalException(ex)
        End Try
    End Sub

    Private Function GetResponseToken(ByVal response As String) As ResponseToken
        Dim rtn As ResponseToken = New ResponseToken()
        Dim responseValues As String() = response.Split("&"c)
        Dim switchStrings As String() = {"token", "digisign", "customer_account_reference", "customer_reference"}

        For Each item As String In responseValues

            Select Case switchStrings.FirstOrDefault(Function(s) item.ToLower().Contains(s))
                Case "token"
                    rtn.Token = item.Substring(item.IndexOf("=") + 1)
                Case "digisign"
                    rtn.DigiSign = item.Substring(item.IndexOf("=") + 1)
                Case "customer_account_reference"
                    rtn.Customer_Account_Reference = item.Substring(item.IndexOf("=") + 1)
                Case "customer_reference"
                    rtn.Customer_Reference = item.Substring(item.IndexOf("=") + 1)
                Case Else
            End Select
        Next

        Return rtn
    End Function

    Private Sub SaveCardPayment(ByVal payment As Payment, ByVal isVoluntary As Boolean)
        Try
            Dim paymentProxy As CardPaymentEbillProxy = New CardPaymentEbillProxy()
            Dim reg As CardRegistration = PortalUtility.GetCardPaymentRegisration(Me.Session)

            paymentProxy.ConfirmationID = payment.ConfirmationNumber
            paymentProxy.LastFourAccountNumber = payment.FundingAccount.AccountNumber.Last(4)
            paymentProxy.PaymentMethod = payment.PaymentMethod
            paymentProxy.RegistrationSK = reg.CardRegistrationSK.Value
            paymentProxy.CardType = payment.PaymentMethod
            paymentProxy.PaymentAmount = Decimal.Parse(payment.Amount)

            If Not String.IsNullOrEmpty(payment.Fee.Feeamount) Then
                paymentProxy.ConvenienceFee = Decimal.Parse(payment.Fee.Feeamount)
            End If

            Dim cardFacade As ICardPaymentSystem = ExternalServiceFactory.CardPaymentSystem
            cardFacade.SaveERPortalPayment(paymentProxy, payment.ToJson(), isVoluntary)

        Catch ex As System.Exception
            PortalUtility.HandlePortalException(ex)
        End Try
    End Sub
End Class

Public Class ResponseToken
    Public Property Token As String
    Public Property DigiSign As String
    Public Property Customer_Reference As String
    Public Property Customer_Account_Reference As String
End Class