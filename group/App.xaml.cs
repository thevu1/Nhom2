using System.Windows;

namespace group
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            DangNhap dn = new DangNhap();
            dn.Show();
        }
    }
}