// PaymentModal.ts - JS interop for Orbipay callback
let paymentCompleteHandler = null;

export function setupPaymentCallback(componentReference) {
    disposePaymentCallback();

    paymentCompleteHandler = async (event) => {
        const detail = event?.detail ?? event?.data ?? {};
        const payload = detail?.data ?? detail;

        const token = payload.token ?? "";
        const digisign = payload.digiSign ?? payload.digisign ?? "";
        const customerAccountReference =
            payload.customer_account_reference ??
            payload.customerAccountReference ??
            "";

        if (!token || !digisign || !customerAccountReference) {
            return;
        }

        await componentReference.invokeMethodAsync(
            "OnPaymentTokenReceived",
            token,
            digisign,
            customerAccountReference
        );
    };

    window.addEventListener("checkout-event", paymentCompleteHandler);
    window.addEventListener("message", paymentCompleteHandler);
}

export function initializeOrbipayForm() {
    return;
}

export function disposePaymentCallback() {
    if (paymentCompleteHandler) {
        window.removeEventListener("checkout-event", paymentCompleteHandler);
        window.removeEventListener("message", paymentCompleteHandler);
        paymentCompleteHandler = null;
    }
}

