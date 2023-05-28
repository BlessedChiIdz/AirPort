using Avalonia.Media.Imaging;
using ReactiveUI;
using System;
using System.Linq;

namespace AirportTimeTable.Models {
    public class TableItem: ReactiveObject {
        public Bitmap Image { get; }
        public string Flight { get; }
        public string Destination { get; }
        public string Time { get; }
        public string TimeCount { get; }
        public string Terminal { get; }
        public string Status { get; private set; }

        public Bitmap BigImage { get; }
        public string Path { get; }
        public bool IsDeparture { get; }
        public string[] Description { get; }
        public bool Visible { get; private set; }

        public TableItem(object[] data, BaseReader br) {
            Image = br.images[(string) data[0]];
            Flight = (string) data[1];
            Destination = (string) data[2];
            Time = (string) data[3];
            TimeCount = (string) data[4];
            Terminal = (string) data[5];
            Status = (string) data[6];

            BigImage = br.images[(string) data[7]];
            Path = (string) data[8];
            IsDeparture = Path.StartsWith("Новосибирск");
            Description = ((object[]) data[9]).Select(x => string.Join("\n", ((object[]) x).Cast<string>())).ToArray();
            Visible = false;
        }

        private static TableItem? last_opened;

        public void Released() {
            Visible = !Visible;
            this.RaisePropertyChanged(nameof(Visible));

            if (last_opened != null && last_opened != this) last_opened.Released();
            last_opened = Visible ? this : null;
        }

        private static readonly Random rand = new();

        public void RecalcTime(int num_day, int mins_offset) {
            var t_arr = TimeCount.Split(':');
            int minutes = (num_day * 24 + int.Parse(t_arr[0])) * 60 + int.Parse(t_arr[1]);
            int delta = minutes - mins_offset;
            if (delta > 2 * 24 * 60) delta -= BaseReader.days_num * 24 * 60;

            

            if (delta < rand.Next(-30, -10)) Status = "Вылетел";
            else if (delta < 0) Status = "Посадка завершена";
            else if (delta <= 30) Status = "Регистр. завершена";
            else Status = rand.Next(3) > 0 ? "Регистрация" : Time == TimeCount ? "По расписанию" : "Задерживается";

            if (delta <= 30 && rand.Next(25) == 0) Status = "Отменён";
        }
    }
}
