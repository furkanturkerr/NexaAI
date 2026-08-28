try {

    const savedTheme =
        localStorage.getItem("nexa-theme");

    document.documentElement.dataset.theme =
        savedTheme === "dark"
            ? "dark"
            : "light";
}
catch (_) {
}