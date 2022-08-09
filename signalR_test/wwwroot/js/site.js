// 顯示Chat視窗
const chatCircle = document.getElementById('chat-circle')
const chatCard = document.getElementById('chat-card')
const chatExitBtn = document.getElementById('chat-exit-btn')

chatCircle.addEventListener('click', event => {
    chatCard.classList.toggle("scaleBox")
    chatCircle.classList.toggle("scaleBox")
})

chatExitBtn.addEventListener('click', event => {
    chatCard.classList.toggle("scaleBox")
    chatCircle.classList.toggle("scaleBox")
})


//渲染訊息

window.addEventListener('load', () => {
    document.querySelector('#send-msg').addEventListener('click', event => {
        event.preventDefault();
        const inputMsg = document.querySelector('#input-msg').value
        if (inputMsg.trim().length === 0) {
            console.log('0')
            return
        }

    })
})
