using System;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace signalR_test.Hubs
{ 
    public class ChatHub : Hub
    {
        public static List<string> ConnIDList = new List<string>();
        //管理者ID列表
        public static List<string> ConnAdminIDList = new List<string>();
        public async Task SendMessage(string user, string message)
        {
            var id = Context.ConnectionId;
            Console.WriteLine(id + "--------------------------------");
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        //管理者登入 儲存管理者ID
        public async Task updAdminID()
        {
            if (ConnAdminIDList.Where(p => p == Context.ConnectionId).FirstOrDefault() == null)
            {
                ConnAdminIDList.Add(Context.ConnectionId);
            }
            Console.WriteLine(ConnAdminIDList[0] + "----------------------------------------");
        }


        public override async Task OnConnectedAsync()
        {
            if(ConnIDList.Where(p => p == Context.ConnectionId).FirstOrDefault() == null)
            {
                ConnIDList.Add(Context.ConnectionId);
            }

            string jsonString = JsonConvert.SerializeObject(ConnIDList);
            await Clients.All.SendAsync("UpdList", jsonString);

            await Clients.Client(Context.ConnectionId).SendAsync("UpdSelfID", Context.ConnectionId);

            await Clients.All.SendAsync("UpdContent", "新連線 ID:" + Context.ConnectionId);

            //方法 OnConnectedAsync() 是每一個用戶連線後，都會觸發的事件，在這事件裡面可以更新用戶列表。
            await base.OnConnectedAsync(); 
        }
        //方法 OnDisconnectedAsync() 是每一個用戶離線後，都會觸發的事件，在這事件裡面可以更新用戶列表。

        //public override async Task OnDisconnectedAsync(Exception ex)
        //{
        //    string id = ConnIDList.Where(p => p == Context.ConnectionId).FirstOrDefault();
        //    if (id != null)
        //    {
        //        ConnIDList.Remove(id);
        //    }
        //    // 更新連線 ID 列表
        //    string jsonString = JsonConvert.SerializeObject(ConnIDList);
        //    await Clients.All.SendAsync("UpdList", jsonString);

        //    // 更新聊天內容
        //    await Clients.All.SendAsync("UpdContent", "已離線 ID: " + Context.ConnectionId);

        //    await base.OnDisconnectedAsync(ex);
        //}

        public async Task Message(string selfID, string message, string sendToID)
        {
            if (string.IsNullOrEmpty(sendToID))
            {
                await Clients.All.SendAsync("UpdContent", selfID + " 説: " + message);
            }
            else
            {
                await Clients.Client(sendToID).SendAsync("UpdContent", selfID + " 向你説: " + message);
                await Clients.Client(Context.ConnectionId).SendAsync("UpdContent", "你向: " + sendToID + " 説: " + message);
            }
        }

        //使用者發話
        public async Task UserSendMessage(string message)
        {
            if (ConnAdminIDList.Count > 0)
            {
                await Clients.Client(ConnAdminIDList[0]).SendAsync("takeover", Context.ConnectionId, message);
                await Clients.Client(Context.ConnectionId).SendAsync("userPostover", message);
            }
            else
            {
                await Clients.Client(Context.ConnectionId).SendAsync("userPostover", message);
            }
        }

        //管理者發話
        public async Task AdminSendMessage(string message, string sendToID)
        {
            if (string.IsNullOrEmpty(sendToID))
            {
                await Clients.Client(Context.ConnectionId).SendAsync("postover", message);
            }
            else
            {
                await Clients.Client(sendToID).SendAsync("userTakeover", message);
                await Clients.Client(Context.ConnectionId).SendAsync("postover", message, sendToID);
            }
        }
    }
}
