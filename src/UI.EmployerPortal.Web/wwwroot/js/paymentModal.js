let paymentCompleteHandler = null;
let orbipayContainer = null;

window.paymentModal = {
    setupPaymentCallback(componentReference) {
        this.disposePaymentCallback();

        paymentCompleteHandler = async (event) => {
            const raw = event?.detail ?? event?.data ?? {};
            const payload = raw?.data ?? raw;

            const token = payload?.token ?? "";
            const digisign = payload?.digiSign ?? payload?.digisign ?? "";
            const customerAccountReference =
                payload?.customer_account_reference ??
                payload?.customerAccountReference ??
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
    },

    openOrbipayOverlay(hostedFormHtml) {
        cleanupOrbipayDom();

        const parser = new DOMParser();
        const doc = parser.parseFromString(hostedFormHtml, "text/html");
        const sourceForm = doc.querySelector("form");
        const sourceScript = doc.querySelector("script");

        if (!sourceForm || !sourceScript) {
            throw new Error("Invalid Orbipay hosted form markup.");
        }

        orbipayContainer = document.createElement("div");
        orbipayContainer.id = "orbipay-runtime-container";
        orbipayContainer.style.position = "fixed";
        orbipayContainer.style.left = "-10000px";
        orbipayContainer.style.top = "-10000px";
        orbipayContainer.style.width = "1px";
        orbipayContainer.style.height = "1px";
        orbipayContainer.style.overflow = "hidden";

        const checkoutButton = document.createElement("button");
        checkoutButton.id = "orbipay-checkout-button";
        checkoutButton.type = "button";
        checkoutButton.textContent = "Pay";
        orbipayContainer.appendChild(checkoutButton);

        const form = document.createElement("form");
        for (const attr of sourceForm.attributes) {
            form.setAttribute(attr.name, attr.value);
        }

        const script = document.createElement("script");
        for (const attr of sourceScript.attributes) {
            script.setAttribute(attr.name, attr.value);
        }

        form.appendChild(script);
        orbipayContainer.appendChild(form);
        document.body.appendChild(orbipayContainer);

        setTimeout(() => {
            checkoutButton.click();
        }, 0);
    },

    disposePaymentCallback() {
        if (paymentCompleteHandler) {
            window.removeEventListener("checkout-event", paymentCompleteHandler);
            window.removeEventListener("message", paymentCompleteHandler);
            paymentCompleteHandler = null;
        }

        cleanupOrbipayDom();
    }
};

function cleanupOrbipayDom() {
    if (orbipayContainer?.parentNode) {
        orbipayContainer.parentNode.removeChild(orbipayContainer);
    }
    orbipayContainer = null;
}
