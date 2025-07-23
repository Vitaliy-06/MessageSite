$(document).ready(async function () {
    let user = null;
    let userList = null;
    let chosenUser = null;


    $("#btn-login").on("click", async function () {
        const email = $("#login-email").val();
        const password = $("#login-password").val();

        if (!email || !password) {
            alert("Please enter email and password");
            return;
        }

        try {
            const response = await loginUser({ email, password });
            localStorage.setItem("token", response.token);
            alert("Login successful!");
            user = await displayUserHeader();

            userList = await displayListUsers();
            chosenUser = null;
        } catch (error) {
            console.error(error);
            alert("Login failed");
        }
    });

    $("#btn-register").on("click", async function () {
        const email = $("#register-email").val();
        const password = $("#register-password").val();

        if (!email || !password) {
            alert("Please enter email and password");
            return;
        }

        try {
            const response = await registerUser({ email, password });
            alert("Registered successfully! Check your email to confirm.");
        } catch (error) {
            console.error(error);
            alert("Registration failed");
        }
    });

    startListeningChatHub();

    connection.on("ReceiveMessage", (data) => {
        const currentRoom = [user.email, chosenUser].sort().join("-");
        const incomingRoom = [data.sender.email, data.receiver.email].sort().join("-");

        if (currentRoom !== incomingRoom) {
            return;
        }

        const newMessageDiv = $(` 
            <div class="message-main-${data.sender.email === user.email ? "you" : "another"}">
                <div class="message-content">${escapeHtml(data.content)}</div>
                <div class="message-timestamp">${formatTimestamp(data.timestamp)}</div>
            </div>
        `);
        $('.message-main').first().append(newMessageDiv);

    })

    async function displayUserHeader() {
        const token = localStorage.getItem("token");
        if (!token) {
            alert("You have to authorize")
            return;
        }

        try {
            const data = await getUser(token);
            $('#header-user').append(`
                    Welcome ${data.user.email}
            `);
            return data.user;
        } catch (error) {
            console.log(error);
            alert(error.status);
        }
    }

    async function displayListUsers() {
        try {
            const data = await getAllUsers();

            for (const el of data.users) {
                if (el === user.email)
                    continue;
                $('#user-list').append(`
                        <li class="li-user">
                            <img src="https://plus.unsplash.com/premium_photo-1677094310956-7f88ae5f5c6b?q=80&w=880&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D"
                                alt="avatar">
                            <span>${el}</span>
                        </li>
                    `);
            }
            $('#user-list').on('click', '.li-user', function () {
                const email = $(this).find('span').text();
                chooseUser(email);
            });
            return data.users.filter((u) => u !== user.email);
        } catch (error) {
            alert(error);
        }
    }

    async function chooseUser(email) {
        $('.message-header').first().text(email);

        if (chosenUser) {
            const room = [chosenUser, user.email].sort().join("-");
            console.log("Leave room " + room);
            leaveRoom(room);
        }

        const token = localStorage.getItem("token");
        if (!token) return;

        try {
            const response = await getAllMessagesForUsers(token, email);
            displayMessages(response.data, user.email);

            $('#textBox').prop('disabled', false);
            $('#btnSend').off('click').on('click', function () {
                sendMessage(email);
            });

            const room = [email, user.email].sort().join("-");
            console.log("Join to room " + room);
            joinRoom(room);

            chosenUser = email;
        } catch (error) {
            alert(error.status)
        }
    }

    function displayMessages(messages, mainUserEmail) {
        const messagesBox = $('.message-main').first();
        messagesBox.html('');
        if (messages.length > 0) {
            for (const el of messages) {
                const messageDiv = $(`
                    <div class="message-main-${el.sender.email === mainUserEmail ? "you" : "another"}">
                        <div class="message-content">${escapeHtml(el.content)}</div>
                        <div class="message-timestamp">${formatTimestamp(el.timestamp)}</div>
                    </div>
                `);
                messagesBox.append(messageDiv);
            }
        }
    }

    async function sendMessage(receiverEmail) {
        const text = $('#textBox').val();

        const token = localStorage.getItem("token");
        if (!token) return;

        try {
            const body = {
                senderId: " ",
                receiverEmail: receiverEmail,
                content: text,
                isRead: true
            };
            const response = await createMessage(token, body);

            $('#textBox').val('');
        } catch (error) {
            console.error(error)
        }
    }

    function formatTimestamp(timestamp) {
        const date = new Date(timestamp);
        const hours = date.getHours().toString().padStart(2, '0');
        const minutes = date.getMinutes().toString().padStart(2, '0');
        const day = date.getDate().toString().padStart(2, '0');
        const month = (date.getMonth() + 1).toString().padStart(2, '0');
        return `${day}.${month} ${hours}:${minutes}`;
    }

    function escapeHtml(text) {
        return text
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;");
    }


});