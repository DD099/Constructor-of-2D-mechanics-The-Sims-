window.simsDownloadText = (fileName, text) => {
    const blob = new Blob([text], { type: 'application/json' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = fileName;
    document.body.appendChild(a);
    a.click();
    a.remove();
    URL.revokeObjectURL(url);
};

window.simsGetRelativePos = (element, clientX, clientY) => {
    const rect = element.getBoundingClientRect();
    return {
        x: clientX - rect.left,
        y: clientY - rect.top
    };
};
