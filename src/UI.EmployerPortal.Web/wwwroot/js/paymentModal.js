// PaymentModal.ts - JS interop for Orbipay callback
let paymentCompleteHandler = null;

export function setupPaymentCallback(componentReference) {
    disposePaymentCallback();

    paymentCompleteHandler = async (event) => {
        const isObjectPayload = event && typeof event.data === "object" && event.data !== null;
        const payload = isObjectPayload ? event.data : {};

        const token =
            payload.token ??
            payload.data?.token ??
            "";

        const digisign =
            payload.digisign ??
            payload.digiSign ??
            payload.data?.digisign ??
            payload.data?.digiSign ??
            "";

        const customerAccountReference =
            payload.customer_account_reference ??
            payload.customerAccountReference ??
            payload.data?.customer_account_reference ??
            payload.data?.customerAccountReference ??
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

    window.addEventListener("message", paymentCompleteHandler);
}


// Orbipay hosted form integration
export function initializeOrbipayForm() {
    const form = document.getElementById('orbipay-checkout-form');
    if (!form) return;

    // Setup Orbipay callback for form submission
    window.addEventListener('message', function (event) {
        // Verify origin for security
        if (event.origin !== 'https://secure.orbipay.com') return;

        const { type, data } = event.data;

        if (type === 'orbipay-payment-complete') {
            // Payment completed - trigger confirmation
            console.log('Payment token received:', data.token);
            window.paymentToken = data.token;
            window.digiSign = data.digisign;
            window.customerAccountRef = data.customer_account_reference;

            // Signal back to Blazor component
            if (window.blazorPaymentCallback) {
                window.blazorPaymentCallback(data);
            }
        }
    });
}

export function submitOrbipayForm() {
    const form = document.getElementById('orbipay-checkout-form');
    if (form) {
        form.submit();
    }
}

export function closePaymentModal() {
    const modal = document.getElementById('orbipay-modal');
    if (modal) {
        modal.classList.remove('visible');
        modal.classList.add('hidden');
    }
}
export function disposePaymentCallback() {
    if (paymentCompleteHandler) {
        window.removeEventListener("message", paymentCompleteHandler);
        paymentCompleteHandler = null;
    }
}
