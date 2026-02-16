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
