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
        defaultDate: defaultDate,
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
        },
        onDayCreate: (_dObj, _dStr, _fp, dayElem) => {
            // currentMonth is the month currently displayed in the calendar (0-based)
            const displayedMonth = _fp.currentMonth;
            const dayMonth = dayElem.dateObj.getMonth();

            if (dayMonth > displayedMonth || (displayedMonth === 11 && dayMonth === 0)) {
                // Next month overflow dates
                dayElem.classList.add("flatpickr-next-month-day");
            } else if (dayMonth < displayedMonth || (displayedMonth === 0 && dayMonth === 11)) {
                // Prev month overflow dates (optional — keep consistent)
                dayElem.classList.add("flatpickr-prev-month-day");
            }
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
