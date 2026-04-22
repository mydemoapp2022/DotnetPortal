export function attachNumericOnlyFilter(element) {
    if (!element) return;

    element.addEventListener('keydown', function (e) {
        if ([46, 8, 9, 27, 13, 110].indexOf(e.keyCode) !== -1 ||
            (e.keyCode === 65 && e.ctrlKey === true) ||
            (e.keyCode === 67 && e.ctrlKey === true) ||
            (e.keyCode === 86 && e.ctrlKey === true) ||
            (e.keyCode === 88 && e.ctrlKey === true) ||
            (e.keyCode >= 35 && e.keyCode <= 39)) {
            return;
        }
        if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) &&
            (e.keyCode < 96 || e.keyCode > 105)) {
            e.preventDefault();
        }
    });

    element.addEventListener('paste', function (e) {
        const pastedData = e.clipboardData?.getData('Text') || '';
        if (!/^[0-9-]*$/.test(pastedData)) {
            e.preventDefault();
        }
    });
}

export function attachMaskedSSNInput(element, dotNetRef, initialDigits) {
    element._ssnDigits = initialDigits || '';
    element._locked = false;
    element.value = element._ssnDigits.length > 0 ? formatMask(element._ssnDigits.length) : '';

    element.addEventListener('beforeinput', (e) => {
        e.preventDefault();

        if (e.inputType === 'insertText') {
            const digits = (e.data || '').replace(/\D/g, '');
            if (digits && element._ssnDigits.length < 9) {
                element._ssnDigits = (element._ssnDigits + digits).slice(0, 9);
                syncDisplay(element, dotNetRef);
            }
        } else if (e.inputType.startsWith('delete')) {
            if (element._ssnDigits.length > 0) {
                element._ssnDigits = element._ssnDigits.slice(0, -1);
                syncDisplay(element, dotNetRef);
            }
        }
    });

    element.addEventListener('paste', (e) => {
        e.preventDefault();
        const text = (e.clipboardData || window.clipboardData).getData('text');
        const digits = text.replace(/\D/g, '');
        if (digits) {
            element._ssnDigits = (element._ssnDigits + digits).slice(0, 9);
            syncDisplay(element, dotNetRef);
        }
    });

    element.addEventListener('drop', (e) => e.preventDefault());

    // On first blur: lock permanently — never show digits again
    element.addEventListener('blur', () => {
        element._locked = true;
        element.value = formatMask(element._ssnDigits.length);
    });
}

export function setSSNDigits(element, digits) {
    element._ssnDigits = digits || '';
    // If already locked (was blurred before), keep stars
    element.value = element._locked
        ? formatMask(element._ssnDigits.length)
        : formatMask(element._ssnDigits.length); // also stars on programmatic set
}

/* ── helpers ─────────────────────────────────────────────── */

/**
 * While _locked: keeps stars (digits update silently).
 * Before first blur: shows full formatted digits.
 */
function syncDisplay(element, dotNetRef) {
    if (element._locked) {
        // Digits change internally but display stays as stars
        element.value = formatMask(element._ssnDigits.length);
    } else {
        // Before first blur — show real digits while typing
        element.value = formatSSN(element._ssnDigits);
    }
    const len = element.value.length;
    element.setSelectionRange(len, len);
    dotNetRef.invokeMethodAsync('OnSSNDigitsChanged', element._ssnDigits);
}

/** Formats raw digits as 999-99-9999 (real characters). */
function formatSSN(digits) {
    if (!digits || digits.length === 0) return '';
    if (digits.length > 5) return `${digits.slice(0, 3)}-${digits.slice(3, 5)}-${digits.slice(5)}`;
    if (digits.length > 3) return `${digits.slice(0, 3)}-${digits.slice(3)}`;
    return digits;
}

/** Formats digit count as ***-**-**** (full stars). */
function formatMask(digitCount) {
    if (digitCount <= 0) return '';
    if (digitCount <= 3) return '*'.repeat(digitCount);
    if (digitCount <= 5) return '***-' + '*'.repeat(digitCount - 3);
    return '***-**-' + '*'.repeat(digitCount - 5);
}
