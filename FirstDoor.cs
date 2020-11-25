using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using Engine;
using System.Drawing;
using System.IO;
using NAudio.Wave;
using System.Collections.Generic;
using EStudio.Service;

namespace EStudio
{
    public partial class FirstDoor : Form
    {
        //加入错词
        int flag = 0;
        //定义在单词和翻译显示的索引
        int wordindex = 1;
        int wrongwordindex = 2;
        //定义一个单词表显示单词和意思
        DataTable dt = new DataTable();
        //定义一个错误单词表
        DataTable ds = new DataTable();
        //book
        static string tableName;

        public FirstDoor()
        {
            InitializeComponent();
            tabControl1.TabPages.Remove(tabPage2);
            tabControl1.TabPages.Remove(tabPage6);
           
        }

        private void FirstDoor_Load(object sender, EventArgs e)
        {
            this.listBox1.Visible = false;
            this.label_Word.Visible = false;

        }

        //播放单词声音
        public void Play(string dir)
        {
            string path = "c://bike//wave//"+dir+".wav";
            List<float> data = Filereader.generate(path);
            Console.WriteLine(data.Count);
            Bitmap imp = Filereader.getImg(pictureBox1.Width, pictureBox1.Height, data);
            pictureBox1.Image = imp;
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(path);
            player.Play();
            
        }

        ///获取表名称
        /// </summary>
        /// <param name="excelFilename">表名</param>
        /// <returns></returns>
        public static DataTable GetExcelTable(int index)
        {
           
            DataSet ds = new DataSet();

            //string tableName;
            int t = 0;
            MySqlConnection connection1 = new MySqlConnection(DBhelper.ConnStr);
            
                connection1.Open();

                DataTable table = connection1.GetSchema("Tables");
                tableName = table.Rows[index]["Table_Name"].ToString();

                string strExcel = "SELECT * FROM "  + tableName + " WHERE flag = '" + t + "'";
                MySqlDataAdapter adapter1 = new MySqlDataAdapter(strExcel,connection1);
                adapter1.Fill(ds);
                connection1.Close();

            Console.WriteLine(tableName);
            DataTable dt = ds.Tables[0];
            return dt;
        }

        public static DataTable GetreviewTable()
        {

            DataSet ds = new DataSet();

            string tableName;

            MySqlConnection connection1 = new MySqlConnection(DBhelper.wrongconnstr);

            connection1.Open();

            DataTable table = connection1.GetSchema("Tables");
            tableName = table.Rows[0]["Table_Name"].ToString();

            string strExcel = "SELECT * FROM " + tableName;
            MySqlDataAdapter adapter1 = new MySqlDataAdapter(strExcel, connection1);
            adapter1.Fill(ds);
            connection1.Close();

            Console.WriteLine(tableName);
            DataTable dt = ds.Tables[0];
            return dt;
        }
        /// <summary>
        ///随机从词库中取题目
        /// </summary>
        public void get_question(int index)
        {

            //加载题
            wordindex = 1;
            dt = GetExcelTable(index);
            
           
        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
           
            int i = listBox1.SelectedIndex;
            Console.WriteLine(i);
            //获取题目
            DialogResult result = MessageBox.Show("确定选择" + listBox1.SelectedItem.ToString() + "进行学习吗?", "提示：", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No)
                return;
            
            get_question(i);
            startbook.Visible = true;
            reviewbook.Visible = true;
            choosebook.Visible = true;
            listBox1.Visible = false;
            button2.Visible = false;

            progressBar1.Visible = true;
            jindu.Visible = true;
            MySqlConnection Conn = new MySqlConnection(DBhelper.ConnStr);
            Conn.Open();
            string counts = "select count(*) from "+tableName;
            MySqlCommand cmd = new MySqlCommand(counts, Conn);
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            
            string counts1 = "select count(*) from "+ tableName +" where flag = 1";
            MySqlCommand cmd1 = new MySqlCommand(counts1, Conn);
            int count1 = Convert.ToInt32(cmd1.ExecuteScalar());progressBar1.Maximum = count;
            Conn.Close();
            progressBar1.Value = count1+800;
            string s1 = count.ToString();
            string s2 = (count1+800).ToString();
            jindu.Text = s2+"/"+s1;
        }

        private void getbook()
        {
            listBox1.Items.Clear();
            this.listBox1.Visible = true;
            listBox1.Enabled = true;



            DirectoryInfo mydir = new DirectoryInfo(@"C:\bike\WORDlibrary");

            FileInfo[] file_name = mydir.GetFiles("*.csv");

            string[] temp_name = new string[file_name.Length];

            for (int i = 0; i < file_name.Length; i++)
            {
                temp_name[i] = file_name[i].Name.Substring(0, file_name[i].Name.Length - 4);
                //获取词库中的excel文件名
                listBox1.Items.Add(temp_name[i]);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            getbook();
            tabPage1.Parent = tabControl1;
            tabPage2.Parent = null;
            tabPage6.Parent = null;
            //button2.Visible = false;
           // get_question();

        }

        private void wrongword_Click(object sender, EventArgs e)
        {

        }

        private void startreview_Click(object sender, EventArgs e)
        {
            result.Visible = true;
            nextword.Visible = true;
            befor.Visible = true;
            btnStart.Visible = true;
            voice.Visible = true;
            //dt = GetExcelTable(1);
            
            word.Text = dt.Rows[wordindex][1].ToString();
            trans.Text = dt.Rows[wordindex][2].ToString();
         

            startreview.Visible = false;
        }
        
        //录音

        private IWaveIn waveIn;
        private WaveFileWriter writer;
        List<float> sampleAggregator = new List<float>();//用来储存波形数据


        private void StartRecording()
        {
            if (waveIn != null) return;
            Text = "start recording....";
            btnStart.Text = "正在录制....";
            btnStart.Enabled = false;

            waveIn = new WaveIn { WaveFormat = new WaveFormat(8000, 1) };//设置码率
            writer = new WaveFileWriter("C://bike//test.wav", waveIn.WaveFormat);
            waveIn.DataAvailable += waveIn_DataAvailable;
            waveIn.RecordingStopped += OnRecordingStopped;
            waveIn.StartRecording();


        }
        private void StartRecording2()
        {
            if (waveIn != null) return;
            Text = "start recording....";
            btnStart.Text = "正在录制....";
            btnStart.Enabled = false;

            waveIn = new WaveIn { WaveFormat = new WaveFormat(8000, 1) };//设置码率
            writer = new WaveFileWriter("C://bike//test.wav", waveIn.WaveFormat);
            waveIn.DataAvailable += waveIn_DataAvailable;
            waveIn.RecordingStopped += OnRecordingStopped2;
            waveIn.StartRecording();


        }

        private void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {

            writer.Write(e.Buffer, 0, e.BytesRecorded);
            byte[] buffer = e.Buffer;
            int bytesRecorded = e.BytesRecorded;


            for (int index = 0; index < e.BytesRecorded; index += 2)
            {
                short sample = (short)((buffer[index + 1] << 8) |
                                        buffer[index + 0]);
                float sample32 = sample / 32768f;
                sampleAggregator.Add(sample32);
            }



            int secondsRecorded = (int)(writer.Length / writer.WaveFormat.AverageBytesPerSecond);//录音时间获取
            if (secondsRecorded >= 3)//只录制6秒
            {
                waveIn.StopRecording();
                Text = "complete";
                btnStart.Text = "开始录制";
                btnStart.Enabled = true;


            }
   

        }
        public static List<float> generate(string path)
        {
            List<float> data = new List<float>();
            byte[] length = new byte[4];
            BinaryReader bI = new BinaryReader(new FileStream(path, FileMode.Open));
            bI.BaseStream.Position = 34;
            int bps = bI.ReadInt16();
            bI.BaseStream.Position = 40;

            while (bI.BaseStream.Position < bI.BaseStream.Length - 16)
            {
                int j = bI.ReadInt16();
                data.Add(j);
            }
            bI.Close();
            return data;
        }
        private void rec_and_score(string path, Label wordlabel, Label resultlabel)
        {
            WordRec.label = resultlabel;
            WordRec.doRec(path);
            Console.WriteLine(WordRec.word);
            if (WordRec.word == wordlabel.Text)
            {
                string str = "UPDATE " +tableName+" SET flag = 1 where words = '" + WordRec.word + "'";
                MySqlConnection con = DBhelper.Open_Conn(DBhelper.ConnStr);
                DBhelper.InsertData(word.Text, trans.Text);
                MySqlCommand cmd = new MySqlCommand(str,con);
                cmd.ExecuteNonQuery();

                DBhelper.Close_Conn(con);
                string path2 = "C://bike//wave//" + WordRec.word + ".wav";
                List<float> data1 = generate(path);
                List<float> data2 = generate(path2);
                data1 = MathLib.expand(data1);
                data2 = MathLib.expand(data2);

                List<float> data3 = MathLib.FFT(data1);
                List<float> data4 = MathLib.FFT(data2);
                float _score = MathLib.ShapeBaseDis(data3, data4);
                _score = 70 + 3 * (float)Math.Pow(100 * _score, 0.5);
                resultlabel.Text = "正确    得分:" + string.Format("{0:f2}", _score);
         
            }

            else
            {

                resultlabel.Text = "错误，请重新录音或查看提示";
               
            }

        }

        private void reviewrec_and_score(string path, Label wordlabel, Label resultlabel)
        {
            WordRec.label = resultlabel;
            WordRec.doRec(path);
            if (WordRec.word == wrongword.Text)
            {
                string path2 = "C://bike//wave//" + WordRec.word + ".wav";
                List<float> data1 = generate(path);
                List<float> data2 = generate(path2);
                data1 = MathLib.expand(data1);
                data2 = MathLib.expand(data2);

                List<float> data3 = MathLib.FFT(data1);
                List<float> data4 = MathLib.FFT(data2);
                float _score = MathLib.ShapeBaseDis(data3, data4);
                _score = 70 + 3 * (float)Math.Pow(100 * _score, 0.5);
                reviewread.Text = "正确    得分:" + string.Format("{0:f2}", _score);
            }

            else
                reviewread.Text = "错误，请重新录音或查看提示";

        }


        private void OnRecordingStopped(object sender, StoppedEventArgs e)
        {
            
            if (waveIn != null) // 关闭录音对象
            {
                waveIn.Dispose();
                waveIn = null;
            }
            if (writer != null)//关闭文件流
            {
                writer.Close();
                writer = null;
            }
            if (e.Exception != null)
            {
                MessageBox.Show(String.Format("出现问题 " + e.Exception.Message));
            }
            List<float> data = Filereader.generate("C://bike//test.wav");
            Bitmap imp = Filereader.getImg(pictureBox2.Width, pictureBox2.Height, data);
            pictureBox2.Image = imp;
            pictureBox2.Visible = true;
            rec_and_score("C://bike//test.wav", word, result);
        }
        private void OnRecordingStopped2(object sender, StoppedEventArgs e)
        {
     
            if (waveIn != null) // 关闭录音对象
            {
                waveIn.Dispose();
                waveIn = null;
            }
            if (writer != null)//关闭文件流
            {
                writer.Close();
                writer = null;
            }
            if (e.Exception != null)
            {
                MessageBox.Show(String.Format("出现问题 " + e.Exception.Message));
            }
            List<float> data = Filereader.generate("C://bike//test.wav");
            Bitmap imp = Filereader.getImg(pictureBox2.Width, pictureBox2.Height, data);
            pictureBox2.Image = imp;
            pictureBox2.Visible = true;
            reviewrec_and_score("C://bike//test.wav", wrongword, reviewread);
        }
        private WaveFileReader reader;
        private void btnStart_Click_1(object sender, EventArgs e)
        {
            
           
            string path1 = "C://bike//Speech_EN//" + word.Text + ".mp3";
            string path2 = "C://bike//wave//" + word.Text + ".wav";
            MP3convert.ConvertMp3ToWav(path1, path2);
            Play(word.Text);
            StartRecording();
            pictureBox1.Visible = true;
            timer1.Interval = 2500;
            timer1.Enabled = true;
            //rec_and_score("C://bike//test.wav", word, result);



        }


        private void flowLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void nextword_Click(object sender, EventArgs e)
        {
            pictureBox1.Visible = false;
            pictureBox2.Visible = false;
            wordindex++;
            word.Text = dt.Rows[wordindex][1].ToString();
            trans.Text = dt.Rows[wordindex][2].ToString();
          
            result.Text = "跟读结果";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (timer1.Enabled == true)
            {
                StartRecording();
            }
            timer1.Enabled = false;
            

            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            pictureBox1.Visible = false;
            pictureBox2.Visible = false;
            wordindex--;
            word.Text = dt.Rows[wordindex][1].ToString();
            trans.Text = dt.Rows[wordindex][2].ToString();
           
            result.Text = "跟读结果";
        }

        private void voice_Click(object sender, EventArgs e)
        {
            pictureBox1.Visible = false;
            Play(word.Text);
        }

        private void tabPage6_Click(object sender, EventArgs e)
        {

        }

        private void stdrev_Click(object sender, EventArgs e)
        {
            showword.Visible = true;
            nextwrongword.Visible = true;
            befowrongword.Visible = true;
            reviewreadword.Visible = true;
            ds = GetreviewTable();
            wrongword.Text = ds.Rows[wrongwordindex][1].ToString();
            wrongtrans.Text = ds.Rows[wrongwordindex][2].ToString();
            stdrev.Visible = false;
        }

        private void nextwrongword_Click(object sender, EventArgs e)
        {
            wrongword.Visible = false;
            wrongwordindex++;
            wrongword.Text = ds.Rows[wrongwordindex][1].ToString();
            wrongtrans.Text = ds.Rows[wrongwordindex][2].ToString();
        }

        private void befowrongword_Click(object sender, EventArgs e)
        {
            wrongword.Visible = false;
            wrongwordindex--;
            wrongword.Text = ds.Rows[wrongwordindex][1].ToString();
            wrongtrans.Text = ds.Rows[wrongwordindex][2].ToString();
        }

        private void showword_Click(object sender, EventArgs e)
        {
            wrongword.Visible = true;
        }

        private void startbook_Click(object sender, EventArgs e)
        {
            tabControl1.TabPages.Remove(tabPage1);
            tabControl1.TabPages.Add(tabPage2);
            tabControl1.TabPages.Add(tabPage6);
            tabControl1.SelectedIndex = 0;
            tabPage1.Parent = null;
            tabPage2.Parent = tabControl1;
            tabPage6.Parent = null;

        }

        private void reviewbook_Click(object sender, EventArgs e)
        {
            tabControl1.TabPages.Remove(tabPage1);
            tabControl1.TabPages.Add(tabPage2);
            tabControl1.TabPages.Add(tabPage6);
            tabControl1.SelectedIndex = 1;
            tabPage1.Parent = null;
            tabPage2.Parent = null;
            tabPage6.Parent = tabControl1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            tabControl1.TabPages.Add(tabPage1);
            tabControl1.TabPages.Remove(tabPage2);
            tabControl1.TabPages.Remove(tabPage6);
            tabControl1.SelectedIndex = 0;
            tabPage1.Parent = tabControl1;
            tabPage2.Parent = null;
            tabPage6.Parent = null;
            startreview.Visible = true;
            word.Text = "";
            trans.Text = "";
        }

        private void choosebook_Click(object sender, EventArgs e)
        {
            dt.Clear();
            getbook();
            progressBar1.Visible = false;
            jindu.Visible = false;
            startbook.Visible = false;
            reviewbook.Visible = false;
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 2;

            tabPage1.Parent = null;
            tabPage2.Parent = null;
            tabPage6.Parent = tabControl1;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            tabControl1.TabPages.Add(tabPage1);
            tabControl1.TabPages.Remove(tabPage2);
            tabControl1.TabPages.Remove(tabPage6);
            tabControl1.SelectedIndex = 0;

            tabPage1.Parent = tabControl1;
            tabPage2.Parent = null;
            tabPage6.Parent = null;
        }

        private void reviewreadword_Click(object sender, EventArgs e)
        {
            StartRecording2();
            string path1 = "C://bike//test.wav";
            
            reviewread.Visible = true;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void progressBar1_Click(object sender, EventArgs e)
        {
            
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }
    }
}
