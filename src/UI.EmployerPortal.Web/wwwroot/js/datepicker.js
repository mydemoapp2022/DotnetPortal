window.initAchDatePicker = (elementId, minDate, maxDate, disabledDates) => {
    flatpickr(`#${elementId}`, {
        minDate: minDate,
        maxDate: maxDate,
        disable: [
            ...disabledDates,
            date => date.getDay() === 0 || date.getDay() === 6  // weekends
        ],
        dateFormat: "Y-m-d",
        allowInput: true
    });
};


window._flatpickrInstances = window._flatpickrInstances || {};

window.initAchDatePicker = (elementId, minDate, maxDate, disabledDates) => {
    if (window._flatpickrInstances[elementId]) {
        window._flatpickrInstances[elementId].destroy();
    }

    const el = document.getElementById(elementId);

    const instance = flatpickr(`#${elementId}`, {
        minDate: minDate,
        maxDate: maxDate,
        disable: [
            ...disabledDates,
            date => date.getDay() === 0 || date.getDay() === 6  // weekends
        ],
        dateFormat: "Y-m-d",
        allowInput: false,
        disableMobile: true,
        onChange: (_selectedDates, dateStr) => {
            // Dispatch a native 'change' event so Blazor's @onchange picks it up
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
