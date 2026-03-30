export function attachNumericOnlyFilter(element) {
    if (!element) return;

    element.addEventListener('keydown', function (e) {
        // Allow: backspace, delete, tab, escape, enter, and arrow keys
        if ([46, 8, 9, 27, 13, 110].indexOf(e.keyCode) !== -1 ||
            // Allow: Ctrl+A, Ctrl+C, Ctrl+V, Ctrl+X
            (e.keyCode === 65 && e.ctrlKey === true) ||
            (e.keyCode === 67 && e.ctrlKey === true) ||
            (e.keyCode === 86 && e.ctrlKey === true) ||
            (e.keyCode === 88 && e.ctrlKey === true) ||
            // Allow: home, end, left, right
            (e.keyCode >= 35 && e.keyCode <= 39)) {
            return;
        }

        // Ensure that it is a number and stop the keypress if not
        if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) &&
            (e.keyCode < 96 || e.keyCode > 105)) {
            e.preventDefault();
        }
    });

    element.addEventListener('paste', function (e) {
        // Get pasted data via clipboard API
        const pastedData = e.clipboardData?.getData('Text') || '';
        
        // Check if pasted data contains only numbers and dashes
        if (!/^[0-9-]*$/.test(pastedData)) {
            e.preventDefault();
        }
    });
}




/**
 * Attaches a fully-masked SSN input handler.
 * Digits are captured but the field always displays '*' characters.
 * @param {HTMLInputElement} element - The input element.
 * @param {DotNetObjectReference} dotNetRef - .NET reference for callbacks.
 * @param {string} initialDigits - Pre-existing raw digits (no dashes).
 */
export function attachMaskedSSNInput(element, dotNetRef, initialDigits) {
    element._ssnDigits = initialDigits || '';
    element.value = formatMask(element._ssnDigits.length);

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

    // Block drag-and-drop text into the field
    element.addEventListener('drop', (e) => e.preventDefault());
}

/**
 * Pushes new digits into the element (e.g. when the parent resets the value).
 */
export function setSSNDigits(element, digits) {
    element._ssnDigits = digits || '';
    element.value = formatMask(element._ssnDigits.length);
}

/* ── helpers ─────────────────────────────────────────────── */

function syncDisplay(element, dotNetRef) {
    element.value = formatMask(element._ssnDigits.length);
    const len = element.value.length;
    element.setSelectionRange(len, len);
    dotNetRef.invokeMethodAsync('OnSSNDigitsChanged', element._ssnDigits);
}

function formatMask(digitCount) {
    if (digitCount <= 0) return '';
    if (digitCount <= 3) return '*'.repeat(digitCount);
    if (digitCount <= 5) return '***-' + '*'.repeat(digitCount - 3);
    return '***-**-' + '*'.repeat(digitCount - 5);
}
