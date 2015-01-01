using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace MobilePro
{
    public partial class Form1 : Form
    {
        private double collision_count;
        private double success_count;
        private bool Aloha;
        private Dictionary<int, List<int>> Collisions;

        public Form1()
        {
            InitializeComponent();
            this.collision_count = this.success_count = 0;
            Collisions = new Dictionary<int, List<int>>();
            Aloha = true;
        }

        //========== delete all entries of any table ==============//
        private void delete_all(string pro)
        {
            SqlConnection con = new SqlConnection("Data Source=.\\SQLEXPRESS;AttachDbFilename=\"C:\\Program Files (x86)\\Microsoft SQL Server\\MSSQL10_50.SQLEXPRESS\\MSSQL\\DATA\\MobileNetwork.mdf\";Integrated Security=True;Connect Timeout=30;User Instance=True");
            con.Open();

            SqlCommand cmd = new SqlCommand(pro, con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.ExecuteNonQuery();

            con.Close();
        }

        //========== delete specific entry from sending_channel table ==========//
        private void delete_entry(int id)
        {
            SqlConnection con = new SqlConnection("Data Source=.\\SQLEXPRESS;AttachDbFilename=\"C:\\Program Files (x86)\\Microsoft SQL Server\\MSSQL10_50.SQLEXPRESS\\MSSQL\\DATA\\MobileNetwork.mdf\";Integrated Security=True;Connect Timeout=30;User Instance=True");
            con.Open();

            SqlCommand cmd = new SqlCommand("delete_entry", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@id", id));
            cmd.ExecuteNonQuery();

            con.Close();
        }

        // ====== update random generator of node ========//
        private void update_random(int id, int rand)
        {

            SqlConnection con = new SqlConnection("Data Source=.\\SQLEXPRESS;AttachDbFilename=\"C:\\Program Files (x86)\\Microsoft SQL Server\\MSSQL10_50.SQLEXPRESS\\MSSQL\\DATA\\MobileNetwork.mdf\";Integrated Security=True;Connect Timeout=30;User Instance=True");
            con.Open();

            SqlCommand cmd = new SqlCommand("update_node_collision", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@collisions", rand));
            cmd.Parameters.Add(new SqlParameter("@id", id));
            cmd.ExecuteNonQuery();

            con.Close();
        }

        //========== inserting in node table =================//
        private void insert_nodes(int id)
        {
            SqlConnection con = new SqlConnection("Data Source=.\\SQLEXPRESS;AttachDbFilename=\"C:\\Program Files (x86)\\Microsoft SQL Server\\MSSQL10_50.SQLEXPRESS\\MSSQL\\DATA\\MobileNetwork.mdf\";Integrated Security=True;Connect Timeout=30;User Instance=True");
            con.Open();
            SqlCommand cmmd = new SqlCommand("insert_node", con);
            cmmd.CommandType = CommandType.StoredProcedure;
            cmmd.Parameters.Add(new SqlParameter("@id", id));
            cmmd.Parameters.Add(new SqlParameter("@rnd", 15));
            cmmd.ExecuteNonQuery();
            con.Close();

        }

        //========== inserting in sending_channel table =================//
        private void insert_sent(int id, int from, int to, int start, int cnt)
        {
            SqlConnection con = new SqlConnection("Data Source=.\\SQLEXPRESS;AttachDbFilename=\"C:\\Program Files (x86)\\Microsoft SQL Server\\MSSQL10_50.SQLEXPRESS\\MSSQL\\DATA\\MobileNetwork.mdf\";Integrated Security=True;Connect Timeout=30;User Instance=True");
            con.Open();
            SqlCommand cmd = new SqlCommand("insert_sent", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@id", id));
            cmd.Parameters.Add(new SqlParameter("@from", from));
            cmd.Parameters.Add(new SqlParameter("@to", to));
            if (!this.Aloha && start%2 == 1 )
                start++;
            cmd.Parameters.Add(new SqlParameter("@start", start));
            cmd.Parameters.Add(new SqlParameter("@cnt", cnt));
            cmd.ExecuteNonQuery();
            con.Close();
        }

        //========== inserting in sent_successfuly table =================//
        private void insert_recieved(Packet P)
        {
            SqlConnection con = new SqlConnection("Data Source=.\\SQLEXPRESS;AttachDbFilename=\"C:\\Program Files (x86)\\Microsoft SQL Server\\MSSQL10_50.SQLEXPRESS\\MSSQL\\DATA\\MobileNetwork.mdf\";Integrated Security=True;Connect Timeout=30;User Instance=True");
            con.Open();
            SqlCommand cmd = new SqlCommand("insert_recieved", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@id", P.id));
            cmd.Parameters.Add(new SqlParameter("@from", P.source));
            cmd.Parameters.Add(new SqlParameter("@to", P.dest));
            cmd.Parameters.Add(new SqlParameter("@end", P.start+2));
            cmd.Parameters.Add(new SqlParameter("@cnt", P.cnt));
            cmd.ExecuteNonQuery();
            con.Close();
        }

        //========== inserting in sent table =================//
        private void insert_initial(bool aloha)
        {

            for (int i = 1; i < 4; i++)
                insert_nodes(i);

            insert_sent(1, 1, 2, 0, 0);
            insert_sent(2, 2, 3, 2, 0);
            insert_sent(3, 3, 1, 2, 0);
            insert_sent(4, 1, 3, 13, 0);
            insert_sent(5, 3, 2, 30, 0);
        }

        //========== selecting max start value from sending_channel table =========//
        private string select_Max()
        {
            SqlConnection con = new SqlConnection("Data Source=.\\SQLEXPRESS;AttachDbFilename=\"C:\\Program Files (x86)\\Microsoft SQL Server\\MSSQL10_50.SQLEXPRESS\\MSSQL\\DATA\\MobileNetwork.mdf\";Integrated Security=True;Connect Timeout=30;User Instance=True");
            con.Open();

            SqlCommand cmd = new SqlCommand("max_time", con);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlParameter output = new SqlParameter("@@Max", SqlDbType.Int);
            output.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(output);
            cmd.ExecuteScalar();
            con.Close();
            return output.Value.ToString();
        }

        //==== collision random of a node ========//
        private string select_collision(int id)
        {
            SqlConnection con = new SqlConnection("Data Source=.\\SQLEXPRESS;AttachDbFilename=\"C:\\Program Files (x86)\\Microsoft SQL Server\\MSSQL10_50.SQLEXPRESS\\MSSQL\\DATA\\MobileNetwork.mdf\";Integrated Security=True;Connect Timeout=30;User Instance=True");
            con.Open();

            SqlCommand cmd = new SqlCommand("get_rand_col", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@id", id));
            SqlParameter output = new SqlParameter("@@collision", SqlDbType.Int);
            output.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(output);
            cmd.ExecuteScalar();
            con.Close();
            return output.Value.ToString();
        }

        //========== getting packets sent at time n and time n+1 ==========//
        private List<Packet> current_packets(int n)
        {
            List<Packet> L = new List<Packet>();
            

            SqlConnection con = new SqlConnection("Data Source=.\\SQLEXPRESS;AttachDbFilename=\"C:\\Program Files (x86)\\Microsoft SQL Server\\MSSQL10_50.SQLEXPRESS\\MSSQL\\DATA\\MobileNetwork.mdf\";Integrated Security=True;Connect Timeout=30;User Instance=True");
            con.Open();
            SqlCommand cmd = new SqlCommand("getPackets", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@T1", n));
            if( this.Aloha )
            cmd.Parameters.Add(new SqlParameter("@T2", n + 1));
            else
                cmd.Parameters.Add(new SqlParameter("@T2", n));
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Packet P = new Packet();
                P.id = (int)reader["Id"];
                P.source = (int)reader["source"];
                P.dest = (int)reader["dest"];
                P.start = (int)reader["start_time"];
                P.cnt = (int)reader["collision_count"];
                L.Add(P);
            }
            reader.Close();
            con.Close();
            return L;
        }

        //====== get final table =======//
        private string all_packets()
        {
            string line = "";

            SqlConnection con = new SqlConnection("Data Source=.\\SQLEXPRESS;AttachDbFilename=\"C:\\Program Files (x86)\\Microsoft SQL Server\\MSSQL10_50.SQLEXPRESS\\MSSQL\\DATA\\MobileNetwork.mdf\";Integrated Security=True;Connect Timeout=30;User Instance=True");
            con.Open();
            SqlCommand cmd = new SqlCommand("get_sent", con);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                line += "Packet number = ";
                line += reader["Id"].ToString();
                line += " Sent from node ";
                line += reader["source"].ToString();
                line += " To node ";
                line += reader["dest"].ToString();
                line += " Reaches the destination at time = ";
                line += reader["end_time"].ToString();
                line += " Number of collisions = ";
                line += reader["count"].ToString();
                line += "\r\n";
            }
            reader.Close();
            con.Close();
            return line;
        }

        //====== get final collisions =======//
        private string all_collisions()
        {
            string line="";
            foreach (int key in Collisions.Keys)
            {
                int n = key;
                if (n % 2 == 1 && !Aloha)
                    n++;
                line += "At time: " + n.ToString() + ", " + (n+1).ToString() + " nodes";
                List<int> L = Collisions[key];
                for (int i = 0; i < L.Count(); i++)
                    line += " " + L[i].ToString();
                line += " collide.\n";
            }
            return line;
        }

        private void start(object sender, EventArgs e)
        {
            int maxi = Int32.Parse(select_Max()), count = 0;
            while (count <= maxi)
            {           
                List<Packet> L = new List<Packet>();
                L = current_packets(count);
                for (int i = 0; i < L.Count(); i++)
                    delete_entry(L[i].id);

                if (L.Count() > 1)
                {
                    List<int> Li = new List<int>();
                    for (int i = 0; i < L.Count(); i++)
                    {
                        Li.Add(L[i].id);
                        int col = Int32.Parse(select_collision(L[i].source));
                        Random rand = new Random();
                        int r = L[i].start + rand.Next(1, col);
                        if (!Aloha && r % 2 == 1)
                            r++;
                        L[i].cnt++;
                        insert_sent(L[i].id, L[i].source, L[i].dest, r, L[i].cnt);
                        update_random(L[i].source, col * 2);
                    }
                    Collisions[count] = Li;
                }
                else if (L.Count() == 1)
                {
                    if (L[0].cnt == 0)
                        this.success_count++;
                    else
                    {
                        this.collision_count++;
                        int col = Int32.Parse(select_collision(L[0].source));
                        update_random(L[0].source, col / 2);
                    }
                    insert_recieved( L[0] );
                }
                count++;
                if( select_Max() != "" )
                maxi = Int32.Parse(select_Max());
            }

            double c = this.collision_count / (this.collision_count + this.success_count);
            double s = this.success_count / (this.collision_count + this.success_count);
            this.collision_r.Text = c.ToString();
            this.success_rl.Text = s.ToString();
            this.textBox1.Text = all_packets();
            MessageBox.Show(all_collisions());
        }

        private void config(object sender, EventArgs e)
        {
            this.Aloha = (this.comboBox1.Text == "Aloha" ? true : false);
            delete_all("delete_Node");
            delete_all("delete_recieved");
            delete_all("delete_Sent");
            insert_initial(Aloha);
            this.collision_count = this.success_count = 0;
            this.collision_r.Text = this.success_rl.Text = this.textBox1.Text = "";
            Collisions.Clear();
            MessageBox.Show("Initial Config. done.");
        }


    }
}
