using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileSystemWatcherSample
{
    public partial class Form1 : Form
    {
        private string m_targetPath = "";
        private System.IO.FileSystemWatcher watcher = null;

        public Form1()
        {
            InitializeComponent();
        }

        ~Form1()
        {
            if ( watcher != null )
            {
                //監視を終了
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
                watcher = null;
            }
        }

        private void Initialie()
        {
            if ( watcher != null )
            {
                return;
            }

            watcher = new System.IO.FileSystemWatcher();
            //監視するディレクトリを指定
            watcher.Path = Path.GetDirectoryName( m_targetPath );
            watcher.Filter = Path.GetFileName( m_targetPath );
            //最終アクセス日時、最終更新日時、ファイル、フォルダ名の変更を監視する
            watcher.NotifyFilter =
                ( System.IO.NotifyFilters.LastAccess
                | System.IO.NotifyFilters.LastWrite
                | System.IO.NotifyFilters.FileName
                | System.IO.NotifyFilters.DirectoryName );
            //すべてのファイルを監視
            watcher.Filter = "";
            //UIのスレッドにマーシャリングする
            //コンソールアプリケーションでの使用では必要ない
            watcher.SynchronizingObject = this;

            //イベントハンドラの追加
            watcher.Changed += new System.IO.FileSystemEventHandler( Watcher_Changed );
            watcher.Created += new System.IO.FileSystemEventHandler( Watcher_Changed );
            watcher.Deleted += new System.IO.FileSystemEventHandler( Watcher_Changed );
            watcher.Renamed += new System.IO.RenamedEventHandler( Watcher_Renamed );
        }

        //イベントハンドラ
        private void Watcher_Changed( System.Object source,
            System.IO.FileSystemEventArgs e )
        {
            switch ( e.ChangeType )
            {
                case System.IO.WatcherChangeTypes.Changed:
                    richTextBox1.AppendText( "ファイル 「" + e.FullPath + "」が変更されました。\r\n" );
                    break;
                case System.IO.WatcherChangeTypes.Created:
                    richTextBox1.AppendText( "ファイル 「" + e.FullPath + "」が作成されました。\r\n" );
                    break;
                case System.IO.WatcherChangeTypes.Deleted:
                    richTextBox1.AppendText( "ファイル 「" + e.FullPath + "」が削除されました。\r\n" );
                    break;
            }
        }

        private void Watcher_Renamed( System.Object source,
            System.IO.RenamedEventArgs e )
        {
            richTextBox1.AppendText( "ファイル 「" + e.FullPath + "」の名前が変更されました。\r\n" );
        }

        private void button_path_Click( object sender, EventArgs e )
        {
            //テキストボックスをクリア
            textBox_path.Text = "";

            //ファイルダイアログを生成する
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "ファイルを開く";
            op.InitialDirectory = @"C:\";
            op.Filter = "すべてのファイル(*.*)|*.*";
            op.FilterIndex = 1;

            //オープンファイルダイアログを表示する
            DialogResult dialog = op.ShowDialog();

            //「開く」ボタンが選択された時の処理
            if ( dialog == DialogResult.OK )
            {
                m_targetPath = op.FileName;
                //テキストボックスにパスを表示
                textBox_path.Text = m_targetPath;
            }
        }

        private void button_start_Click( object sender, EventArgs e )
        {
            if ( m_targetPath.Length <= 0 )
            {
                return;
            }

            Initialie();

            //監視を開始する
            watcher.EnableRaisingEvents = true;
            richTextBox1.AppendText( "監視を開始しました。\r\n" );
        }
        private void button_stop_Click( object sender, EventArgs e )
        {
            if ( watcher != null )
            {
                //監視を終了
                watcher.EnableRaisingEvents = false;
                richTextBox1.AppendText( "監視を終了しました。\r\n" );
            }
        }
    }
}
