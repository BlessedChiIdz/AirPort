using AirportTable.ViewModels;
using AirportTimeTable.Models;
using Avalonia.Controls;
using Avalonia.Input;

namespace AirportTable.Views {
    public partial class MainWindow: Window {
        public MainWindow() {
            InitializeComponent();
            var mwvm = new MainWindowViewModel();
            DataContext = mwvm;
            mwvm.AddWindow(this);
        }

        public void Released(object? sender, PointerReleasedEventArgs e) {
            var src = (Control) (e.Source ?? throw new System.Exception("׸?!"));
            while (src.Parent != null) {
                if (src is ListBoxItem) {
                    var item = (TableItem?) src.DataContext;
                    item?.Released();
                    return;
                }
                src = (Control) src.Parent;
            }
        }
    }
}