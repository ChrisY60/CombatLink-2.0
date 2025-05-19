const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .build();

document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (sender, message, utcTimeString) {
    const isOwnerOfMessage = sender.id == currentUserId;

    const msgDiv = document.createElement("div");
    msgDiv.classList.add("chat-message", isOwnerOfMessage ? "sent" : "received");

    const bubble = document.createElement("div");
    bubble.classList.add("bubble");

    const messageText = document.createTextNode(message);
    bubble.appendChild(messageText);

    const timeDiv = document.createElement("div");
    timeDiv.classList.add("message-meta");

    const localTime = new Date(utcTimeString);
    timeDiv.textContent = localTime.toLocaleTimeString([], {
        hour: '2-digit',
        minute: '2-digit'
    });

    bubble.appendChild(timeDiv);
    msgDiv.appendChild(bubble);

    const chatBox = document.getElementById("chatBox");
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
