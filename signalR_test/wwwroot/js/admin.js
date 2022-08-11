
const connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build()
const connectIDList = []

//管理者＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝//
//管理者加入ID//

connection.start().then(function () {
    console.log("Hub 連線完成")
    connection.invoke("updAdminID")
    .catch(function (err) {
        return console.error(err.toString())
    })
    }).catch(function (err) {
        console.error(err.toString())
    })


//send Message

window.addEventListener('load', () => {
    document.querySelector('#send-Admsg').addEventListener('click', sendMsg)
    document.querySelector('#input-Admsg').addEventListener('keydown', function (e) {
        if (e.keyCode === 13) {
            sendMsg(e)
        }
    }, false)

    document.querySelector('#user-list').addEventListener('click', (event) => {
        if (event.target.classList.contains("clearfix")) {
            //轉換視窗

            const index = event.target.dataset.index
            let contentList = document.querySelectorAll('.chat-content')
            contentList.forEach(item => item.classList.add("d-none"))
            contentList[index].classList.remove("d-none")

            //按鈕加入sendID 
            document.getElementById('send-Admsg').setAttribute('data-sendto', contentList[index].dataset.sendid)
        }
        
    })
})

function sendMsg(event) {
    event.preventDefault();
    let sendToID;
    if (event.target.classList.contains("fa-paper-plane")) {
        sendToID = event.target.parentElement.dataset.sendto
    } else {
        sendToID = event.target.dataset.sendto
    }
    
    const message = document.querySelector('#input-Admsg').value
    console.log(sendToID)
    console.log(message)
    if (message.trim().length === 0) {
        return
    }
    
    //送出訊息至server
    //start存入ID
    connection.invoke("AdminSendMessage", message, sendToID).catch(function (err) {
        console.log("錯誤" + err.toString())
    })
    document.querySelector('#input-Admsg').value = ''
}


//管理者接收//
connection.on("takeover", function (user, message) {    
    if (!connectIDList.includes(user)) {
        connectIDList.push(user)
        document.getElementById('user-list').innerHTML += `
            <li class="clearfix" data-index="${connectIDList.indexOf(user)}">
                <img src="#" alt="avatar" class="me-2">
                <p>Vincent Porter</p>
            </li>
            `
        const newChatbody = document.createElement("div")
        const name = 'chat-content' + connectIDList.indexOf(user).toString()
        document.getElementById('admin-chat').appendChild(newChatbody)
        newChatbody.setAttribute('id', name)
        newChatbody.setAttribute('data-sendid', user)
        newChatbody.classList.add("d-none", "chat-content")
    }
    const ChatDiv = document.createElement("div")
    ChatDiv.classList.add("d-flex", "align-items-baseline", "mb-4", "chatbox")
    ChatDiv.innerHTML += `
            <div class="position-relative avatar">
                <img src="#" alt="Logo" class="img-fluid rounded-circle">
                <span class="position-absolute bottom-0 start-100 translate-middle p-1 bg-success border border-light rounded-circle">
                    <span class="visually-hidden">New alerts</span>
                </span>
            </div>
                <!-- 對話框 -->
            <div class="pe-2">
                <div>
                    <div class="card card-text d-inline-block p-2 m-1">${message}</div>
                    <div class="small">01:13PM</div>
                </div>
            </div>
        `
    const name = 'chat-content' + connectIDList.indexOf(user).toString()
    document.querySelector(`#${name}`).appendChild(ChatDiv)
})

//管理者發送

connection.on("postover", function (message, user) {
    const ChatDiv = document.createElement("div")
    ChatDiv.classList.add("d-flex", "align-items-baseline", "mb-4")
    ChatDiv.innerHTML = `
            <div class="position-relative avatar">
                <img src="#" alt="Logo" class="img-fluid rounded-circle">
                <span class="position-absolute bottom-0 start-100 translate-middle p-1 bg-success border border-light rounded-circle">
                    <span class="visually-hidden">New alerts</span>
                </span>
            </div>
                <!-- 對話框 -->
            <div class="pe-2">
                <div>
                    <div class="card card-text d-inline-block p-2 m-1">${message}</div>
                    <div class="small">01:13PM</div>
                </div>
            </div>
        `
    const name = 'chat-content' + connectIDList.indexOf(user).toString()
    document.querySelector(`#${name}`).appendChild(ChatDiv)
})
