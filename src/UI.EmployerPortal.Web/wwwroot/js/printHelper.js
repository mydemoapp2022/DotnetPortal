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
                body { font-family: Verdana, sans-serif; padding: 20px; margin: 0; }

                /* ── Outer container: single border wrapping all sections ── */
                .print-section {
                    max-width: 684px;
                    border: 2px solid #000000;
                    padding: 0;
                }

                /* ── Each section: no individual card border ── */
                .ach-print-section { display: block; }
                .ach-info-card {
                    border: none !important;
                    background-color: #fff !important;
                    padding: 16px 20px;
                    margin-bottom: 0;
                }

                /* ── Hide collapse buttons ── */
                .ach-info-card-header-actions { display: none !important; }

                /* ── Section heading ── */
                h2 {
                    margin: 0 0 8px;
                    color: #000;
                    font-family: Verdana;
                    font-weight: 700;
                    font-size: 25px;
                    line-height: 116%;
                }

                /* ── Divider under each heading ── */
                .ach-section-divider {
                    border: none;
                    border-top: 2px solid #1a2e5a;
                    margin: 10px 0 14px;
                }

                /* ── Print dl: 2-column grid ── */
                .ach-print-dl {
                    display: grid;
                    grid-template-columns: 1fr 1fr;
                    gap: 12px 24px;
                    margin: 0;
                    padding: 0;
                    list-style: none;
                }
                .ach-print-dl-item { display: flex; flex-direction: column; }
                .ach-print-dl-item--full { grid-column: 1 / -1; margin-bottom: 4px; }
                .ach-print-dl dt { font-weight: 400; margin: 0; font-size: 16px; line-height: 160%; }
                .ach-print-dl dd { font-weight: 700; margin: 0; font-size: 16px; line-height: 160%; }

                /* ── Legacy ach-bank-grid (kept for compatibility) ── */
                .ach-bank-grid {
                    display: grid;
                    grid-template-columns: 1fr 1fr;
                    gap: 12px 24px;
                    margin-bottom: 12px;
                }

                /* ── Field label / value ── */
                .ach-info-label {
                    display: block;
                    font-size: 16px;
                    font-weight: 400;
                    color: #000;
                    margin-bottom: 2px;
                    line-height: 160%;
                }
                .ach-info-value {
                    font-weight: 700;
                    font-size: 16px;
                    line-height: 160%;
                }
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
