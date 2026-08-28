const micButton =
    document.getElementById("micButton");

const speechMessageInput =
    document.getElementById("messageInput");


let recorder = null;
let microphoneStream = null;
let audioChunks = [];
let isRecording = false;


if (micButton && speechMessageInput) {

    micButton.addEventListener(
        "click",
        async function () {

            if (!isRecording) {

                try {

                    microphoneStream =
                        await navigator.mediaDevices
                            .getUserMedia({
                                audio: true
                            });

                    recorder =
                        new MediaRecorder(
                            microphoneStream);

                    audioChunks = [];

                    recorder.ondataavailable =
                        function (event) {

                            if (event.data.size > 0) {
                                audioChunks.push(
                                    event.data);
                            }
                        };


                    recorder.onstop =
                        async function () {

                            microphoneStream
                                .getTracks()
                                .forEach(
                                    track =>
                                        track.stop());

                            const audioBlob =
                                new Blob(
                                    audioChunks,
                                    {
                                        type:
                                        recorder.mimeType
                                    });

                            const formData =
                                new FormData();

                            formData.append(
                                "audio",
                                audioBlob,
                                "recording.webm");

                            const token =
                                document.querySelector(
                                    'input[name="__RequestVerificationToken"]'
                                )?.value;

                            if (token) {

                                formData.append(
                                    "__RequestVerificationToken",
                                    token);
                            }

                            const response =
                                await fetch(
                                    "/Speech/Transcribe",
                                    {
                                        method: "POST",
                                        body: formData
                                    });

                            if (!response.ok) {

                                console.error(
                                    "STT işlemi başarısız.");

                                return;
                            }

                            const result =
                                await response.json();

                            speechMessageInput.value =
                                result.text;

                            speechMessageInput.focus();
                        };


                    recorder.start();

                    isRecording = true;

                    micButton.classList.add(
                        "mic-recording");

                    micButton.innerHTML =
                        '<i class="bi bi-stop-fill"></i>';

                    micButton.title =
                        "Kaydı durdur";
                }
                catch (error) {

                    console.error(
                        "Mikrofon açılamadı:",
                        error);
                }

                return;
            }


            if (
                recorder &&
                recorder.state === "recording"
            ) {
                recorder.stop();
            }

            isRecording = false;

            micButton.classList.remove(
                "mic-recording");

            micButton.innerHTML =
                '<i class="bi bi-mic"></i>';

            micButton.title =
                "Sesli mesaj";
        });
}


let currentTtsAudio = null;
let currentTtsUrl = null;
let currentTtsButton = null;


function clearCurrentTts() {

    if (currentTtsAudio) {

        currentTtsAudio.pause();
        currentTtsAudio.currentTime = 0;
    }

    if (currentTtsUrl) {

        URL.revokeObjectURL(
            currentTtsUrl);
    }

    if (currentTtsButton) {

        currentTtsButton.innerHTML =
            '<i class="bi bi-volume-up"></i>';

        currentTtsButton.title =
            "Sesli dinle";
    }

    currentTtsAudio = null;
    currentTtsUrl = null;
    currentTtsButton = null;
}


document.addEventListener(
    "click",
    async function (event) {

        const button =
            event.target.closest(
                ".tts-button");

        if (!button)
            return;


        if (currentTtsAudio) {

            const sameButton =
                currentTtsButton === button;

            clearCurrentTts();

            if (sameButton)
                return;
        }


        const message =
            button.closest(
                ".message.ai");

        const content =
            message?.querySelector(
                ".ai-content");

        const text =
            content?.innerText.trim();

        if (!text)
            return;


        button.disabled = true;

        const token =
            document.querySelector(
                'input[name="__RequestVerificationToken"]'
            )?.value;


        try {

            const response =
                await fetch(
                    "/Speech/Synthesize",
                    {
                        method: "POST",

                        headers: {
                            "Content-Type":
                                "application/json",

                            "RequestVerificationToken":
                            token
                        },

                        body: JSON.stringify({
                            text
                        })
                    });


            if (!response.ok) {

                throw new Error(
                    "TTS işlemi başarısız.");
            }


            const audioBlob =
                await response.blob();

            currentTtsUrl =
                URL.createObjectURL(
                    audioBlob);

            currentTtsAudio =
                new Audio(
                    currentTtsUrl);

            currentTtsButton =
                button;


            button.innerHTML =
                '<i class="bi bi-stop-fill"></i>';

            button.title =
                "Sesi durdur";


            currentTtsAudio.onended =
                function () {

                    clearCurrentTts();
                };


            await currentTtsAudio.play();
        }
        catch (error) {

            clearCurrentTts();

            console.error(
                "TTS hatası:",
                error);
        }
        finally {

            button.disabled = false;
        }
    });