using MySql.Data.MySqlClient;
using System.Windows.Forms;
using System;

namespace UserCRUD
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            ControlBox = false;
            radioButton1.Checked = true;

            hideReg();
            feltolt();
        }

        private Connect conn = new Connect();
        public static int userId = 0;

        private void button1_Click(object sender, System.EventArgs e)
        {
            string[] darabol = textBox1.Text.Split(' ');

            // Ellenőrizd, hogy legalább két szó van-e a névben
            if (darabol.Length != 2)
            {
                MessageBox.Show("Kérjük, adjon meg egy teljes nevet (keresztnév és vezetéknév).");
                return;
            }

            if (beleptet(darabol[1], darabol[0], textBox2.Text) == true)
            {
                MessageBox.Show("Regisztrált tag.");
            }
            else
            {
                MessageBox.Show("Nem regisztrált tag.");
                showReg();
                textBox3.Text = darabol[1]; // vezetéknév
                textBox4.Text = darabol[0]; // kereszt név
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Ellenőrizd, hogy a mezők megfelelőek és a jelszavak egyeznek
            if (radioButton1.Checked == true && textBox5.Text == textBox6.Text)
            {
                frissit(userId, textBox4.Text, textBox3.Text, textBox5.Text);
                MessageBox.Show("Sikeres frissítés");
                listBox1.Items.Clear();
                feltolt();
                hideReg();
            }
            // Módosítva: radioButton3 is figyelembe véve
            else if (textBox5.Text == textBox6.Text && (radioButton1.Checked == false || radioButton3.Checked == true))
            {
                MessageBox.Show(regisztral(textBox3.Text, textBox4.Text, textBox5.Text));
                hideReg();
            }
            else
            {
                MessageBox.Show("A jelszavak nem egyeznek meg!");
            }
        }

        private bool beleptet(string firstName, string lastName, string pass)
        {
            conn.Connection.Open();

            string sql = $"SELECT `Id` FROM `data` WHERE `FirstName`= '{firstName}' and `LastName`= '{lastName}' and `Password`= '{pass}'";

            MySqlCommand cmd = new MySqlCommand(sql, conn.Connection);
            MySqlDataReader dr = cmd.ExecuteReader();

            bool van = dr.Read();

            conn.Connection.Close();

            return van;
        }

        private string regisztral(string firstName, string lastName, string pass)
        {
            // Ellenőrizd, hogy a mezők nem üresek
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(pass))
            {
                return "Minden mező kitöltése kötelező!";
            }

            try
            {
                conn.Connection.Open();

                string sql = $"INSERT INTO `data`(`FirstName`, `LastName`, `Password`, `CreatedTime`, `UpdatedTime`) VALUES ('{firstName}','{lastName}','{pass}','{DateTime.Now.ToString("yyyy-MM-dd")}','{DateTime.Now.ToString("yyyy-MM-dd")}')";

                MySqlCommand cmd = new MySqlCommand(sql, conn.Connection);

                var result = cmd.ExecuteNonQuery();

                conn.Connection.Close();

                listBox1.Items.Clear();
                feltolt();

                return result > 0 ? "Sikeres regisztráció" : "Sikertelen Regisztráció.";
            }
            catch (Exception ex)
            {
                return "Hiba történt a regisztráció során: " + ex.Message;
            }
        }

        private void feltolt()
        {
            conn.Connection.Open();

            string sql = $"SELECT `Id`,`LastName`,`FirstName`,`CreatedTime`,`UpdatedTime` FROM `data`; ";

            MySqlCommand cmd = new MySqlCommand(sql, conn.Connection);
            MySqlDataReader dr = cmd.ExecuteReader();

            listBox1.Items.Clear(); // Törölni a listát, hogy friss adatokat jeleníthessük meg

            while (dr.Read())
            {
                listBox1.Items.Add($"{dr.GetInt32(0)}. {dr.GetString(1)} {dr.GetString(2)} {dr.GetDateTime(3).ToString("yyyy-MM-dd")} {dr.GetDateTime(4).ToString("yyyy-MM-dd")}");
            }

            conn.Connection.Close();
        }

        private void hideReg()
        {
            label3.Visible = label4.Visible = label5.Visible = label6.Visible = false;
            textBox3.Visible = textBox4.Visible = textBox5.Visible = textBox6.Visible = false;
            button2.Visible = false;
        }

        private void showReg()
        {
            label3.Visible = label4.Visible = label5.Visible = label6.Visible = true;
            textBox3.Visible = textBox4.Visible = textBox5.Visible = textBox6.Visible = true;
            button2.Visible = true;
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            // Ellenőrizd, hogy egy elem van-e kiválasztva a listából
            if (listBox1.SelectedItem != null)
            {
                string id = listBox1.SelectedItem.ToString();
                string[] idDarabol = id.Split('.');

                // Ellenőrizd, hogy a split eredménye megfelelő-e
                if (idDarabol.Length < 1 || !int.TryParse(idDarabol[0].Trim(), out userId))
                {
                    MessageBox.Show("Hibás ID formátum. Kérjük, ellenőrizze az adatokat.");
                    return;
                }

                // Ha radioButton2 van kiválasztva, törlés
                if (radioButton2.Checked == true)
                {
                    conn.Connection.Open();

                    string sql = $"DELETE FROM `data` WHERE `Id`= '{idDarabol[0]}'";

                    MySqlCommand cmd = new MySqlCommand(sql, conn.Connection);
                    var result = cmd.ExecuteNonQuery();

                    conn.Connection.Close();

                    listBox1.Items.Clear();
                    feltolt();
                }
                else
                {
                    // Ha radioButton1 vagy radioButton3 van kiválasztva, szerkesztés
                    showReg();
                    string[] darabol = id.Split(' ');

                    // Ellenőrizd, hogy legalább 3 elem van-e a darabolt adatban
                    if (darabol.Length >= 3)
                    {
                        textBox4.Text = darabol[1]; // vezetéknév
                        textBox3.Text = darabol[2]; // kereszt név
                    }
                    else
                    {
                        MessageBox.Show("Hibás adat formátum.");
                        return;
                    }

                    // Próbáld meg biztonságosan konvertálni az ID-t
                    string[] darabol2 = id.Split(' ');
                    if (darabol2.Length > 0 && int.TryParse(darabol2[0].TrimEnd(), out userId))
                    {
                        // userId beállítása, ha sikerült a konverzió
                    }
                    else
                    {
                        MessageBox.Show("Hibás ID formátum.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Nincs kiválasztott elem a listában.");
            }
        }


        private void frissit(int Id, string FirstName, string LastName, string Password)
        {
            // Ellenőrizd, hogy minden mező érvényes-e
            if (string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(LastName) || string.IsNullOrEmpty(Password))
            {
                MessageBox.Show("Minden mező kitöltése kötelező!");
                return;
            }

            try
            {
                conn.Connection.Open();

                string sql = $"UPDATE `data` SET `FirstName`='{FirstName}',`LastName`='{LastName}',`Password`='{Password}' WHERE `Id` = '{Id}'";

                MySqlCommand cmd = new MySqlCommand(sql, conn.Connection);

                var result = cmd.ExecuteNonQuery();

                conn.Connection.Close();

                if (result > 0)
                {
                    MessageBox.Show("Sikeres frissítés");
                }
                else
                {
                    MessageBox.Show("Nem történt változtatás.");
                }
            }
            catch (FormatException ex)
            {
                MessageBox.Show("Hibás adatformátum. Kérjük, ellenőrizze a beírt adatokat.");
                Console.WriteLine(ex.Message); // Hibakereséshez hasznos
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba történt: " + ex.Message);
                Console.WriteLine(ex.Message); // Hibakereséshez hasznos
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            // Ha radioButton3 kiválasztva van, akkor az is regisztrációt indít el
            if (radioButton3.Checked)
            {
                // Regisztrációs rész láthatóvá tétele
                showReg();
            }
        }
    }
}
