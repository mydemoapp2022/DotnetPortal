using System;
using System.Collections.Generic;
using System.Text;

namespace Test.UI.EmployerPortal.Web.BillingPayments;

public static class BillingPaymentsMainTestExtensions
{
    public static void HandleContinueForTest(this BillingPaymentsMain component)
        => component.GetType()
            .GetMethod("HandleContinue", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .Invoke(component, null);

    public static void GoBackForTest(this BillingPaymentsMain component)
        => component.GetType()
            .GetMethod("GoBack", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .Invoke(component, null);

    public static void HandleCancelForTest(this BillingPaymentsMain component)
        => component.GetType()
            .GetMethod("HandleCancel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .Invoke(component, null);
}
