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

window.simsFullscreenInit = (dotNetRef) => {
    const handler = () => {
        dotNetRef.invokeMethodAsync('SetFullscreenState', !!document.fullscreenElement);
    };
    document.addEventListener('fullscreenchange', handler);
};

window.simsToggleDesignerFullscreen = async (element) => {
    if (!element) return;
    try {
        if (document.fullscreenElement === element) {
            await document.exitFullscreen();
        } else if (element.requestFullscreen) {
            await element.requestFullscreen();
        }
    } catch (e) {
        console.warn('fullscreen', e);
    }
};
