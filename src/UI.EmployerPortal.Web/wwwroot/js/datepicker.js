window._flatpickrInstances = window._flatpickrInstances || {};

window.initAchDatePicker = (elementId, minDate, maxDate, disabledDates, defaultDate) => {
    if (window._flatpickrInstances[elementId]) {
        window._flatpickrInstances[elementId].destroy();
        delete window._flatpickrInstances[elementId];
    }

    const el = document.getElementById(elementId);
    if (!el) {
        console.warn(`initAchDatePicker: element #${elementId} not found.`);
        return;
    }

    const instance = flatpickr(el, {
        minDate: minDate,
        maxDate: maxDate,
        defaultDate: defaultDate,       // ← fixes the January 1901 issue
        disable: [
            ...disabledDates,
            date => date.getDay() === 0 || date.getDay() === 6
        ],
        dateFormat: "m-d-Y",
        allowInput: false,
        disableMobile: true,
        onChange: (_selectedDates, dateStr) => {
            el.value = dateStr;
            el.dispatchEvent(new Event("change", { bubbles: true }));
        }
    });

    window._flatpickrInstances[elementId] = instance;
};

window.destroyAchDatePicker = (elementId) => {
    if (window._flatpickrInstances[elementId]) {
        window._flatpickrInstances[elementId].destroy();
        delete window._flatpickrInstances[elementId];
    }
};
