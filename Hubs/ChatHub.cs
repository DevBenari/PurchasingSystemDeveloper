using Microsoft.AspNetCore.SignalR;

namespace PurchasingSystemDeveloper.Hubs
{
    public class ChatHub:Hub
    {        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        //Method untuk mengirim data karyawan baru
        //public async Task NotifyKaryawanAdded(Karyawan karyawan)
        //{
        //    await Clients.All.SendAsync("ReceiveKaryawan", karyawan);
        //}

        public async Task UpdateDataCount(string totalKaryawan)
        {
            await Clients.All.SendAsync("UpdateDataCount", totalKaryawan);
        }

        //// Method untuk mengirim data log baru
        public async Task UpdateDataLogger(string loggerData)
        {
            await Clients.All.SendAsync("UpdateDataLogger", loggerData);
        }
    }
}
