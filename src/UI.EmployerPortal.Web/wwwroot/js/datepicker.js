window._flatpickrInstances = window._flatpickrInstances || {};

/**
 * Initialises a Flatpickr date picker on the given element.
 * @param {string}   elementId      - The id of the <input> element.
 * @param {string}   minDate        - ISO date string for the earliest selectable date.
 * @param {string}   maxDate        - ISO date string for the latest selectable date.
 * @param {string[]} disabledDates  - Array of ISO date strings to disable (Fed holidays).
 * @param {object}   dotNetRef      - DotNetObjectReference for Blazor callback.
 */
window.initAchDatePicker = (elementId, minDate, maxDate, disabledDates, dotNetRef) => {
    // Destroy any previous instance on this element before re-initialising
    if (window._flatpickrInstances[elementId]) {
        window._flatpickrInstances[elementId].destroy();
    }

    const instance = flatpickr(`#${elementId}`, {
        minDate: minDate,
        maxDate: maxDate,
        disable: [
            // Specific Fed holiday dates
            ...disabledDates,
            // Weekends
            date => date.getDay() === 0 || date.getDay() === 6
        ],
        dateFormat: "Y-m-d",
        allowInput: false,          // force selection through the picker only
        disableMobile: true,        // keep Flatpickr on mobile too
        onChange: (selectedDates, dateStr) => {
            if (dotNetRef && dateStr) {
                dotNetRef.invokeMethodAsync("OnFlatpickrDateSelected", dateStr);
            }
        }
    });

    window._flatpickrInstances[elementId] = instance;
};

/**
 * Destroys the Flatpickr instance attached to the given element id.
 * @param {string} elementId
 */
window.destroyAchDatePicker = (elementId) => {
    if (window._flatpickrInstances[elementId]) {
        window._flatpickrInstances[elementId].destroy();
        delete window._flatpickrInstances[elementId];
    }
};
