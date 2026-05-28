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

                /* ── Info card (each section box) ── */
                .ach-info-card { border: 1px solid #ccc; border-radius: 4px; padding: 16px 20px; margin-bottom: 20px; }
                .ach-info-card-header-actions { display: none !important; }

                /* ── Section heading ── */
                h2 { font-size: 20px; margin: 0 0 8px; }
                .ach-section-divider { border: none; border-top: 2px solid #1a2e5a; margin: 10px 0 14px; }

                /* ── Print dl: 2-column grid matching on-screen layout ── */
                .ach-print-dl { display: grid; grid-template-columns: 1fr 1fr; gap: 12px 24px; margin: 0; padding: 0; list-style: none; }
                .ach-print-dl-item { display: flex; flex-direction: column; }
                .ach-print-dl-item--full { grid-column: 1 / -1; }

                /* ── Legacy ach-bank-grid (kept for compatibility) ── */
                .ach-bank-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 12px 24px; margin-bottom: 12px; }

                /* ── Field label / value ── */
                .ach-info-label { display: block; font-size: 14px; color: #000; }
                .ach-info-value { font-weight: bold; font-size: 14px; }
                .ach-info-field { display: flex; flex-direction: column; margin-bottom: 8px; }
                .ach-info-field--full { grid-column: 1 / -1; margin-bottom: 4px; }
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
