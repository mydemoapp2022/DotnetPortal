window.printSection = function (sectionClass) {
    const section = document.querySelector('.' + sectionClass);
    if (!section) return;

    const printWindow = window.open('', '_blank', 'width=800,height=600');
    printWindow.document.write(`
        <!DOCTYPE html>
        <html>
        <head>
            <title>Print</title>
            <style>
                body { font-family: Verdana, sans-serif; padding: 20px; }
                .ach-bank-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 12px 24px; margin-bottom: 12px; }
                .ach-info-label { display: block; font-size: 14px; color: #000; }
                .ach-info-value { font-weight: bold; font-size: 14px; }
                .ach-info-card { border: 1px solid #ccc; border-radius: 4px; padding: 16px 20px; margin-bottom: 20px; }
                .ach-info-card-header-actions { display: none !important; }
                .ach-section-divider { border: none; border-top: 1px solid #ccc; margin: 10px 0 14px; }
                h2 { font-size: 20px; margin: 0 0 8px; }
                .ach-info-field--full { margin-bottom: 4px; }
            </style>
        </head>
        <body>${section.innerHTML}</body>
        </html>
    `);
    printWindow.document.close();
    printWindow.focus();
    printWindow.print();
    printWindow.close();
};
