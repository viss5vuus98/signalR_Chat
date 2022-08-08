using System;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace signalR_test.Hubs
{ 
    public class ChatHub : Hub
    {
        public static List<string> ConnIDList = new List<string>();
        public async Task SendMessage(string user, string message)
        {
            var id = Context.ConnectionId;
            Console.WriteLine(id + "--------------------------------");
            await Clients.All.SendAsync("ReceiveMessage", user, message);
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
    }
}
