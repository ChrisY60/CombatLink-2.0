const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .build();

document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (sender, message) {
    let isOwnerOfMessage = false;
    if (sender.id == currentUserId) {
        isOwnerOfMessage = true; 
    }

    const msgDiv = document.createElement("div");
    msgDiv.classList.add("chat-message", isOwnerOfMessage ? "sent" : "received");
    msgDiv.textContent = message;
    console.log(message);

    const chatBox = document.getElementById("chatBox"); 
    chatBox.appendChild(msgDiv);
    chatBox.scrollTop = chatBox.scrollHeight;
});

connection.start().then(() => {
    connection.invoke("JoinMatchGroup", matchId.toString()).catch(err => console.error(err.toString()));
    document.getElementById("sendButton").disabled = false;

    document.getElementById("sendButton").addEventListener("click", () => {
        const input = document.getElementById("messageInput");
        let message = input.value.trim();
        if (message !== "") {
            connection.invoke("SendMessageToMatch", matchId.toString(), currentUserId.toString(), message.toString())
                .catch(err => console.error(err.toString()));
            input.value = "";
        }
    });
});
