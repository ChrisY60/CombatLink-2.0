const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .build();

document.getElementById("sendButton").disabled = true;

let lastRenderedDate = null;

connection.on("ReceiveMessage", function (sender, message, utcTimeString) {
    const isOwnerOfMessage = sender.id == currentUserId;

    const localDate = new Date(utcTimeString);
    const localDateString = localDate.toLocaleDateString('en-GB');

    const chatBox = document.getElementById("chatBox");

    if (lastRenderedDate !== localDateString) {
        const dateDiv = document.createElement("div");
        dateDiv.classList.add("date-separator");
        dateDiv.textContent = localDate.toLocaleDateString('bg-BG', {
            day: '2-digit',
            month: '2-digit',
            year: 'numeric'
        });
        chatBox.appendChild(dateDiv);
        lastRenderedDate = localDateString;
    }

    const msgDiv = document.createElement("div");
    msgDiv.classList.add("chat-message", isOwnerOfMessage ? "sent" : "received");

    const bubble = document.createElement("div");
    bubble.classList.add("bubble");

    const messageText = document.createTextNode(message);
    bubble.appendChild(messageText);

    const timeDiv = document.createElement("div");
    timeDiv.classList.add("message-meta");
    timeDiv.textContent = localDate.toLocaleTimeString([], {
        hour: '2-digit',
        minute: '2-digit'
    });

    bubble.appendChild(timeDiv);
    msgDiv.appendChild(bubble);

    chatBox.appendChild(msgDiv);
    chatBox.scrollTop = chatBox.scrollHeight;
});



connection.start().then(() => {
    connection.invoke("JoinMatchGroup", matchId.toString()).catch(err => console.error(err.toString()));
    document.getElementById("sendButton").disabled = false;

    document.getElementById("sendButton").addEventListener("click", () => {
        const input = document.getElementById("messageInput");
        const message = input.value.trim();
        if (message !== "") {
            connection.invoke("SendMessageToMatch", matchId.toString(), currentUserId.toString(), message)
                .catch(err => console.error(err.toString()));
            input.value = "";
        }
    });
});
