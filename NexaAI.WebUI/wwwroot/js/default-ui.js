const html =
    document.documentElement;

const themeButton =
    document.getElementById("themeButton");

const themeIcon =
    document.getElementById("themeIcon");

const profileThemeButton =
    document.getElementById("profileThemeButton");

const profileButton =
    document.getElementById("profileButton");

const profileMenu =
    document.getElementById("profileMenu");

const assistantButton =
    document.getElementById("assistantButton");

const assistantMenu =
    document.getElementById("assistantMenu");

const newChatButton =
    document.getElementById("newChatButton");

const newChatPanel =
    document.getElementById("newChatPanel");

const cancelChatButton =
    document.getElementById("cancelChatButton");

const conversationTitleInput =
    document.getElementById("conversationTitle");


function updateThemeIcon() {

    if (!themeIcon)
        return;

    const isDark =
        html.dataset.theme === "dark";

    themeIcon.className =
        isDark
            ? "bi bi-sun"
            : "bi bi-moon";
}


function toggleTheme() {

    const newTheme =
        html.dataset.theme === "dark"
            ? "light"
            : "dark";

    html.dataset.theme =
        newTheme;

    localStorage.setItem(
        "nexa-theme",
        newTheme);

    updateThemeIcon();
}


themeButton?.addEventListener(
    "click",
    toggleTheme);

profileThemeButton?.addEventListener(
    "click",
    toggleTheme);

updateThemeIcon();


profileButton?.addEventListener(
    "click",
    function (event) {

        event.stopPropagation();

        profileMenu?.classList.toggle("show");
        assistantMenu?.classList.remove("show");
    });


assistantButton?.addEventListener(
    "click",
    function (event) {

        event.stopPropagation();

        assistantMenu?.classList.toggle("show");
        profileMenu?.classList.remove("show");
    });


document.addEventListener(
    "click",
    function () {

        profileMenu?.classList.remove("show");
        assistantMenu?.classList.remove("show");
    });


newChatButton?.addEventListener(
    "click",
    function () {

        newChatPanel?.classList.toggle("show");

        if (
            newChatPanel?.classList.contains("show")
        ) {
            conversationTitleInput?.focus();
        }
    });


cancelChatButton?.addEventListener(
    "click",
    function () {

        newChatPanel?.classList.remove("show");

        if (conversationTitleInput)
            conversationTitleInput.value = "";
    });