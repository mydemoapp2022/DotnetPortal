// PaymentModal.ts - JS interop for Orbipay callback
export function setupPaymentCallback(componentReference: any) {
    window.paymentModalInstance = componentReference;

    // Listen for Orbipay completion event
    document.addEventListener('orbipay-payment-complete', async (event: CustomEvent) => {
        const { token, digisign, customer_account_reference } = event.detail;
        await componentReference.invokeMethodAsync(
            'OnPaymentTokenReceived',
            token,
            digisign,
            customer_account_reference
        );
    });
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
