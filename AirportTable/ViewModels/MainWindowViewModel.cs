using AirportTimeTable.Models;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Media;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;

namespace AirportTable.ViewModels {
    public class Log {
        static readonly bool use_file = false;

        static readonly List<string> logs = new();
        static readonly string path = "../../../Log.txt";
        static bool first = true;

        public static MainWindowViewModel? Mwvm { private get; set; }
        public static void Write(string message, bool without_update = false) {
            if (!without_update) {
                foreach (var mess in message.Split('\n')) logs.Add(mess);
                while (logs.Count > 55) logs.RemoveAt(0);

                if (Mwvm != null) Mwvm.Logg = string.Join('\n', logs);
            }

            if (use_file) {
                if (first) File.WriteAllText(path, message + "\n");
                else File.AppendAllText(path, message + "\n");
                first = false;
            }
        }
    }

    public class MainWindowViewModel: ViewModelBase {
        private string log = "";
        public string Logg { get => log; set { this.RaiseAndSetIfChanged(ref log, value); } }

        readonly BaseReader br = new();

        public MainWindowViewModel() {
            Log.Mwvm = this;
            SelectA = ReactiveCommand.Create<Unit, Unit>(_ => { FuncSelectA(); return new Unit(); });
            SelectB = ReactiveCommand.Create<Unit, Unit>(_ => { FuncSelectB(); return new Unit(); });
            SelectC = ReactiveCommand.Create<Unit, Unit>(_ => { FuncSelectC(); return new Unit(); });
            SelectD = ReactiveCommand.Create<Unit, Unit>(_ => { FuncSelectD(); return new Unit(); });
            SelectE = ReactiveCommand.Create<Unit, Unit>(_ => { FuncSelectE(); return new Unit(); });
            UpdateItems();
        }

        string column_text = "Назначение";
        public string ColumnText { get => column_text; set { this.RaiseAndSetIfChanged(ref column_text, value); } }

        private TableItem[] items = Array.Empty<TableItem>();
        public TableItem[] Items { get => items; set { this.RaiseAndSetIfChanged(ref items, value); } }
        private void UpdateItems() {
            Items = br.GetItems(selected, selected2);
        }



        Button? button_a, button_b, button_c, button_d, button_e;
        public void AddWindow(Window mw) {
            button_a = mw.Find<Button>("Button_A");
            button_b = mw.Find<Button>("Button_B");
            button_c = mw.Find<Button>("Button_C");
            button_d = mw.Find<Button>("Button_D");
            button_e = mw.Find<Button>("Button_E");
            SetButtonState(0, true);
            SetButtonState2(1, true);
        }

        private void SetButtonState(int num, bool state) {
            var button = num == 0 ? button_a : button_b;
            if (button == null) return;
            button.Background = new SolidColorBrush(Color.Parse(state ? "#EB7501" : "#323B44"));

            var canvas = (Canvas) ((AvaloniaList<ILogical>) button.GetLogicalChildren())[0];
            var app = Application.Current ?? throw new System.Exception("Чё?!");
            var ress = app.Resources;

            var res = ress[num == 0 ? (state ? "departure_B" : "departure_A") : (state ? "landing_B" : "landing_A")];
            var img2 = (Image) (res ?? throw new System.Exception("Чё?!"));
            var img = (Image) canvas.Children[0];
            img.Source = img2.Source;

            var tb = (TextBlock) canvas.Children[1];
            tb.Foreground = new SolidColorBrush(Color.Parse(state ? "#1C242B" : "#6F788B"));
        }
        private void SetButtonState2(int num, bool state) {
            var button = num == 0 ? button_c : num == 1 ? button_d : button_e;
            if (button == null) return;
            button.Background = new SolidColorBrush(Color.Parse(state ? "#8892a5" : "#0000"));
            button.Foreground = new SolidColorBrush(Color.Parse(state ? "#fff" : "#8892a5"));
        }



        bool selected = false;
        void SelectButton(bool newy) {
            if (selected == newy) return;
            selected = newy;
            SetButtonState(0, !newy);
            SetButtonState(1, newy);
            ColumnText = newy ? "Пункт вылета" : "Назначение";
            UpdateItems();
        }
        void FuncSelectA() => SelectButton(false);
        void FuncSelectB() => SelectButton(true);
        public ReactiveCommand<Unit, Unit> SelectA { get; }
        public ReactiveCommand<Unit, Unit> SelectB { get; }

        int selected2 = 1;
        void SelectButton2(int newy) {
            if (selected2 == newy) return;
            selected2 = newy;
            SetButtonState2(0, newy == 0);
            SetButtonState2(1, newy == 1);
            SetButtonState2(2, newy == 2);
            UpdateItems();
        }
        void FuncSelectC() => SelectButton2(0);
        void FuncSelectD() => SelectButton2(1);
        void FuncSelectE() => SelectButton2(2);
        public ReactiveCommand<Unit, Unit> SelectC { get; }
        public ReactiveCommand<Unit, Unit> SelectD { get; }
        public ReactiveCommand<Unit, Unit> SelectE { get; }
    }
}