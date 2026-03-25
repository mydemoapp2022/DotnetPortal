window.downloadAndOpenPdf = async (fileName, contentStreamReference) => {
    const arrayBuffer = await contentStreamReference.arrayBuffer();
    const blob = new Blob([arrayBuffer], { type: 'application/pdf' });
    const url = URL.createObjectURL(blob);

    // Trigger background download
    const anchor = document.createElement('a');
    anchor.href = url;
    anchor.download = fileName;
    document.body.appendChild(anchor);
    anchor.click();
    document.body.removeChild(anchor);

    // Open in new tab after download triggered
    window.open(url, '_blank');

    // Revoke blob URL after delay to allow both actions to complete
    setTimeout(() => URL.revokeObjectURL(url), 10000);
};

window.downloadFileFromStream = async (fileName, contentStreamReference) => {
    const arrayBuffer = await contentStreamReference.arrayBuffer();
    const blob = new Blob([arrayBuffer], { type: 'application/pdf' });
    const url = URL.createObjectURL(blob);
    const anchor = document.createElement('a');
    anchor.href = url;
    anchor.download = fileName ?? 'download.pdf';
    anchor.target = '_blank';
    document.body.appendChild(anchor);
    anchor.click();
    document.body.removeChild(anchor);
    //URL.revokeObjectURL(url);

    window.open(url, '_blank');
    setTimeout(() => URL.revokeObjectURL(url), 5000);
}

window.openPdfInNewTab = async (contentStreamReference) => {
    const arrayBuffer = await contentStreamReference.arrayBuffer();
    const blob = new Blob([arrayBuffer], { type: 'application/pdf' });
    const url = URL.createObjectURL(blob);

    window.open(url, '_blank');
    setTimeout(() => URL.revokeObjectURL(url), 5000);
}

window.focusElement = function (elementId) {
    const el = document.getElementById(elementId);
    if (el) {
        el.focus();
        el.scrollIntoView({ behavior: 'smooth', block: 'center' });
    }
};
