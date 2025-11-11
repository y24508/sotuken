using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ICP
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                //runを開始
                Application.Run(new MainForm());
            }
            //エラーだったら終了
            catch (System.Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "UserTracker", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
