(() => {
    const previousOnLoad = window.onload;
    window.onload = (ev) => {
        if (previousOnLoad) {
            previousOnLoad(ev);
        }
        alert("Loaded!");
    };
})();