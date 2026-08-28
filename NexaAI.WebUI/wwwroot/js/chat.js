const signalRToken =
    document.body.dataset.signalrToken;

const connection =
    new signalR.HubConnectionBuilder()
        .withUrl(
            "http://localhost:5015/hubs/chat",
            {
                accessTokenFactory: () => signalRToken
            })
        .withAutomaticReconnect()
        .build();


const startConversationForm =
    document.getElementById("startConversationForm");

const messageForm =
    document.getElementById("messageForm");

const messageInput =
    document.getElementById("messageInput");

const sendButton =
    document.getElementById("sendButton");

const messages =
    document.getElementById("messages");

const chatContent =
    document.getElementById("chatContent");

const emptyConversation =
    document.getElementById("emptyConversation");


let streamingAIContent = null;

let thinkingElement = null;

let streamFinished = false;

let requestFinished = false;

let isSending = false;


function scrollChatToBottom() {
    if (!chatContent)
        return;

    chatContent.scrollTop =
        chatContent.scrollHeight;
}


function getCurrentConversationId() {
    if (!messages)
        return null;

    return messages.dataset.conversationId;
}


function createUserMessage(content) {
    if (!messages)
        return;

    const message =
        document.createElement("div");

    message.className =
        "message user";

    const messageContent =
        document.createElement("div");

    messageContent.className =
        "user-message-content";

    messageContent.textContent =
        content;

    message.appendChild(
        messageContent
    );

    messages.appendChild(
        message
    );

    if (emptyConversation) {
        emptyConversation.remove();
    }

    scrollChatToBottom();
}


function showThinking() {
    if (!messages)
        return;

    if (thinkingElement)
        return;

    thinkingElement =
        document.createElement("div");

    thinkingElement.className =
        "message ai thinking-message";

    thinkingElement.textContent =
        "Düşünüyor...";

    messages.appendChild(
        thinkingElement
    );

    scrollChatToBottom();
}


function hideThinking() {
    if (!thinkingElement)
        return;

    thinkingElement.remove();

    thinkingElement = null;
}


function createAIMessage() {
    if (!messages)
        return null;

    const message =
        document.createElement("div");

    message.className =
        "message ai";

    const content =
        document.createElement("div");

    content.className =
        "ai-streaming-content";

    message.appendChild(
        content
    );

    messages.appendChild(
        message
    );

    scrollChatToBottom();

    return content;
}


function finishRequestIfReady() {
    if (
        streamFinished &&
        requestFinished
    ) {
        window.location.reload();
    }
}


connection.on(
    "ReceiveAIChunk",
    function (
        conversationId,
        chunk
    ) {
        const currentConversationId =
            getCurrentConversationId();

        if (!currentConversationId)
            return;

        if (
            currentConversationId.toLowerCase()
            !==
            conversationId.toLowerCase()
        ) {
            return;
        }

        hideThinking();

        if (!streamingAIContent) {
            streamingAIContent =
                createAIMessage();
        }

        if (!streamingAIContent)
            return;

        streamingAIContent.textContent +=
            chunk;

        scrollChatToBottom();
    });


connection.on(
    "ReceiveAICompleted",
    function (conversationId) {
        const currentConversationId =
            getCurrentConversationId();

        if (!currentConversationId)
            return;

        if (
            currentConversationId.toLowerCase()
            !==
            conversationId.toLowerCase()
        ) {
            return;
        }

        hideThinking();

        streamFinished = true;

        finishRequestIfReady();
    });


connection.onreconnecting(
    error => {
        console.log(
            "SignalR yeniden bağlanıyor...",
            error
        );
    });


connection.onreconnected(
    connectionId => {
        console.log(
            "SignalR tekrar bağlandı.",
            connectionId
        );
    });


connection.onclose(
    error => {
        console.log(
            "SignalR bağlantısı kapandı.",
            error
        );
    });


if (startConversationForm) {
    startConversationForm.addEventListener(
        "submit",
        async function (event) {
            event.preventDefault();

            const content =
                messageInput.value.trim();

            if (!content)
                return;

            const formData =
                new FormData(
                    startConversationForm
                );

            sendButton.disabled = true;
            messageInput.disabled = true;

            try {
                const response =
                    await fetch(
                        startConversationForm.action,
                        {
                            method: "POST",
                            body: formData
                        });

                if (!response.ok) {
                    throw new Error(
                        "Sohbet oluşturulamadı."
                    );
                }

                const result =
                    await response.json();

                sessionStorage.setItem(
                    "nexa-initial-message",
                    content
                );

                window.location.href =
                    `/?conversationId=${result.conversationId}`;
            }
            catch (error) {
                console.error(
                    "Sohbet oluşturma hatası:",
                    error
                );

                sendButton.disabled = false;
                messageInput.disabled = false;
            }
        });
}


if (messageForm) {
    messageForm.addEventListener(
        "submit",
        async function (event) {
            event.preventDefault();

            if (isSending)
                return;

            const content =
                messageInput.value.trim();

            if (!content)
                return;

            createUserMessage(
                content
            );

            showThinking();

            const formData =
                new FormData(
                    messageForm
                );

            isSending = true;

            streamFinished = false;
            requestFinished = false;
            streamingAIContent = null;

            sendButton.disabled = true;
            messageInput.disabled = true;

            messageInput.value = "";
            messageInput.style.height =
                "auto";

            try {
                const response =
                    await fetch(
                        messageForm.action,
                        {
                            method: "POST",
                            body: formData
                        });

                if (!response.ok) {
                    throw new Error(
                        "Mesaj gönderilemedi."
                    );
                }

                requestFinished = true;

                finishRequestIfReady();
            }
            catch (error) {
                hideThinking();

                console.error(
                    "Mesaj gönderme hatası:",
                    error
                );

                requestFinished = true;
            }
            finally {
                isSending = false;

                sendButton.disabled = false;
                messageInput.disabled = false;

                messageInput.focus();
            }
        });
}


if (messageInput) {
    messageInput.addEventListener(
        "input",
        function () {
            this.style.height =
                "auto";

            this.style.height =
                Math.min(
                    this.scrollHeight,
                    150
                ) + "px";
        });
}


async function startSignalR() {
    try {
        await connection.start();

        console.log(
            "SignalR bağlantısı kuruldu."
        );

        if (!messageForm)
            return;

        const initialMessage =
            sessionStorage.getItem(
                "nexa-initial-message"
            );

        if (!initialMessage)
            return;

        messageInput.value =
            initialMessage;

        sessionStorage.removeItem(
            "nexa-initial-message"
        );

        messageForm.requestSubmit();
    }
    catch (error) {
        console.error(
            "SignalR bağlantı hatası:",
            error
        );
    }
}


scrollChatToBottom();

startSignalR();