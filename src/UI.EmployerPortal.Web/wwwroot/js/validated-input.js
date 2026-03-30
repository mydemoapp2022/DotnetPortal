export function enforceNumberMaxLength(element, maxLength) {
    if (!element || !maxLength) return;
    element.addEventListener('input', function () {
        if (this.value.length > maxLength) {
            this.value = this.value.slice(0, maxLength);
        }
    });
}
