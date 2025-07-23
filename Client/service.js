const connection = new signalR.HubConnectionBuilder()
    .withUrl(BACKEND + "/chatHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

async function start() {
    try {
        await connection.start();
        console.log("SignalR Connected.");
    } catch (err) {
        console.log(err);
        setTimeout(start, 5000);
    }
};

const startListeningChatHub = () => {
    connection.onclose(async () => {
        await start();
    });
    start();
};

const joinRoom = async (roomName) => {
    try {
        await connection.invoke("JoinRoom", roomName);
    } catch (error) {
        console.error(error);
    }
};
const leaveRoom = async (roomName) => {
    try {
        connection.invoke("LeaveRoom", roomName);
    } catch (error) {
        console.error(error);
    }
};

const registerUser = (body) => {
    return $.ajax({
        type: "POST",
        url: BACKEND + "/api/auth/register",
        headers: {
            'Content-Type': 'application/json'
        },
        data: JSON.stringify(body),
        dataType: "json"
    });
};

const loginUser = (body) => {
    return $.ajax({
        type: "POST",
        url: BACKEND + "/api/auth/login",
        headers: {
            'Content-Type': 'application/json'
        },
        data: JSON.stringify(body),
        dataType: "json"
    });
};

const getUser = (token) => {
    return $.ajax({
        type: "GET",
        dataType: "json",
        url: BACKEND + "/api/auth/get-user",
        headers: {
            'Authorization': 'Bearer ' + token
        }
    });
};

const getAllUsers = () => {
    return $.ajax({
        type: "GET",
        dataType: "json",
        url: BACKEND + "/api/user/all-users"
    });
};

const getAllMessagesForUsers = (token, userEmail) => {
    return $.ajax({
        type: "GET",
        dataType: "json",
        url: BACKEND + "/api/message/get-messages?receiverEmail=" + userEmail,
        headers: {
            'Authorization': 'Bearer ' + token
        }
    });
};

const createMessage = (token, body) => {
    return $.ajax({
        type: "POST",
        url: BACKEND + "/api/message/create-message",
        headers: {
            'Authorization': 'Bearer ' + token,
            'Content-Type': 'application/json'
        },
        data: JSON.stringify(body),
        dataType: "json"
    });
};


