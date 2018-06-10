using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        OleDbConnection conexiune;
        string Connectiontext = @"Provider=Microsoft.Jet.OLEDB.4.0;
                                  Data Source=db1.mdb";
        string plus = "++PLUS++";
        string Minus = "--MINUS--";
        bool a_scris_nume = false;//testeaza daca utilizatorul a introdus un nume
        int ScorMax = 60;//Introdu scorul maxim
        int rez = 0, level = 1, score = 0;
        int puncte = 0;
        int sec = 0, secr = 0, milisecr = 0;
        string Ecuatia;
        int milisec = -1, minute, ora;
        Random r = new Random();
        bool Admin = false;
        bool level3 = false;
        int nrgres = 0;
        string[] greseala = new string[65];
        int[] rezultat = new int[60];
        string[] semn_bun = new string[65];
        string[] semn_rau = new string[65];
        string text_scunde = " secunde";
        string text_milisecunde = " milisecunde";
        string Win_text = "Jocul s-a sfarsit.Ai ";
        string text_puncte = " puncte";
        string Help_text = @" 
                                                            Bine ai venit la jocul ''Plus şi Minus''! 
                                                      Scopul acestui Joc este să raspunzi la o intrebare simplă:  
                                                                    Care este Semnul Operaţiei?  
Click pe primul buton de sus şi pe submeniul Joc te va duce la meniul jocului.Aici iţi vei introduce numele  
şi vei apăsa pe butonul 'Începe'.Vor apărea 2 butoane +(Plus) şi respectiv -(Minus)  
Deasupra acestora va apărea o formulă matematică.Trebuie să răspunzi la intrebarea  
jocului apăsând unul din cele 2 butoane.Operaţiile pot conţine şi modul.
După terminarea jocului vor apărea 2 butoane care iţi vor permite fie inceperea unui joc nou   
sau să iti examinezi greşelile făcute pe parcurs.De asemenea în submeniul clasament vei putea  
vedea şi scorurile celorlalţi jucători.În submeniul Performanţă poţi vedea ce performanţă ai avut 
dealungul mai multor jocuri.Alegeţi limba din meniul Limba.Meniul Ajutor te va aduce aici";
        private void Rememberstring(string ec, int rez, string rasp, string gres)
        {
            nrgres++;
            greseala[nrgres] = ec + "";
            rezultat[nrgres] = rez;
            semn_bun[nrgres] = rasp + "";
            semn_rau[nrgres] = gres + "";
        }

        private void Remember()
        {
            for (int i = 1; i <= nrgres; i++)
            {

                string sql = "Insert Into Greseli(Ecuatia,Rezultatul,Semnul,Raspunsul_tau) Values( ";
                sql += "'" + greseala[i] + "'," + rezultat[i] + ",'" + semn_rau[i] + "','" + semn_bun[i] + "')";
                oleDbCommand1.CommandText = sql;
                oleDbConnection1.Open();

                oleDbCommand1.ExecuteNonQuery();
                oleDbConnection1.Close();
            }
        }
        private void updatebd()
        {
            string ordonare = "Nume";
            if (radioButton1.Checked == true) ordonare = "Nume";
            else if (radioButton2.Checked == true)
            {

                if (checkBox1.Checked == true) ordonare = "Timps Asc,Timpms";
                else if (checkBox2.Checked == true) ordonare = "Timps Desc,Timpms";
            }
            else if (radioButton3.Checked == true) ordonare = "Scor";
            if (checkBox1.Checked == true) ordonare += " ASC";
            else if (checkBox2.Checked == true) ordonare += " DESC";

            string sql = "SELECT nume, data,Timps & ':' & Timpms AS Timp,scor FROM Highscore ORDER BY " + ordonare;
            int linie = 0;
            try
            {
                conexiune = new OleDbConnection(Connectiontext);
                conexiune.Open();
                ds = new DataSet();
                ds.Tables.Add("Highscore");
                da = new OleDbDataAdapter(sql, conexiune);
                da.Fill(ds, "Highscore");
                bs = new BindingSource(ds, "Highscore");
                dataGridView1.DataSource = bs;
                cb = new OleDbCommandBuilder(da);
                OleDbCommand comanda = new OleDbCommand(sql, conexiune);
                OleDbDataReader dr = comanda.ExecuteReader();
                conexiune.Close(); linie++;
            }
            catch (OleDbException exc)
            {
                MessageBox.Show("Error! " + exc.Message);
            }

        }
        string Text_eroare = "Te rog sa-ti introduci Numele";
        private void button3_Click(object sender, EventArgs e)
        {
            sterge_greseli();
            timer2.Enabled = false;
            timer3.Enabled = false;
            button5.Visible = false;
            if (a_scris_nume == false) No_name.SetError(Nume, Text_eroare);
            else
            {
                if (Nume.Text == "Phenom23") Admin = true;
                if (Admin == false) button3.Visible = false;
                Nume.Visible = false;
                button1.Visible = true;
                button2.Visible = true;
                Lab4Score.Visible = true;
                label2.Visible = true;
                Hint.Visible = true;
                timp.Visible = true;
                Lab4Timp_total.Visible = true;
                Rezfin.Visible = true;
                timer1.Enabled = true;
                //---------Level 1---------
                int a, b, co;
                string c = "+-*";
                a = r.Next(10);
                b = r.Next(10);
                co = r.Next(3);
                switch (co)
                {
                    case 0: rez = a + b; break;
                    case 1: rez = a - b; break;
                    case 2: rez = a * b; break;
                }
                label1.Text = a + " " + c[co] + " " + b;
                //------------------------
            }
        }

        bool level2 = false;

        private void AfisGres()
        {
            string sql = "Select Ecuatia,Rezultatul,Raspunsul_tau,Semnul From Greseli";
            OleDbConnection connection;
            try
            {
                connection = new OleDbConnection(Connectiontext);
                connection.Open();
                ds = new DataSet();
                ds.Tables.Add("Greseli");
                da = new OleDbDataAdapter(sql, connection);
                da.Fill(ds, "greseli");
                bs = new BindingSource(ds, "greseli");
                dataGridView2.DataSource = bs;
                cb = new OleDbCommandBuilder(da);
            }
            catch (OleDbException exc)
            {
                MessageBox.Show("Error! " + exc.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (puncte >= 40 && puncte <= 44)
            {
                Random poz = new Random();
                int x = poz.Next(101);
                if (x % 3 == 0) x = 1;
                else x = 0;
                if (x == 1)
                {
                    int swap = button1.Left;
                    button1.Left = button2.Left;
                    button2.Left = swap;
                }
            }
            if (puncte >= 45 && puncte <= 49)
            {
                Random poz = new Random();
                int x = poz.Next(101);
                if (x % 2 == 0) x = 1;
                else x = 0;
                if (x == 1)
                {
                    int swap = button1.Left;
                    button1.Left = button2.Left;
                    button2.Left = swap;
                }
            }
            if (puncte >= 50)//Kill Screen
            {
                timer4.Enabled = true;
                timer2.Enabled = true;
                timer3.Enabled = true;
            }
            if (score < puncte) puncte = score;
            try
            {

                score++;
                //---------------Level 1----------
                if (level == 1)
                {
                    if (rez >= 0) puncte++;
                    else { Rememberstring(Ecuatia, rez, plus, Minus); puncte--; }
                    int abs = -1; int absran;
                    label2.Text = puncte + "/" + score;
                    if (puncte > score) puncte = score;
                    int a, b, co;
                    string c = "+-*";
                    a = r.Next(10);
                    b = r.Next(10);
                    co = r.Next(3);
                    absran = r.Next(100);
                    if (absran % 2 == 0) a *= abs;
                    absran = r.Next(100);
                    if (absran % 3 == 0) b *= abs;
                    switch (co)
                    {
                        case 0: rez = a + b; break;
                        case 1: rez = a - b; break;
                        case 2: rez = a * b; break;
                    }
                    if (a >= 0 && b >= 0) label1.Text = a + " " + c[co] + " " + b;
                    else if (a <= 0 && b >= 0) label1.Text = "(" + a + ")" + " " + c[co] + " " + b;
                    else if (a >= 0 && b <= 0) label1.Text = a + " " + c[co] + " " + "(" + b + ")";
                    else label1.Text = "(" + a + ")" + " " + c[co] + " " + "(" + b + ")";
                    if (puncte % 10 == 0 && puncte > 9) level++;
                    Ecuatia = label1.Text;
                    Rezfin.Text = secr + text_scunde + " , " + milisecr + text_milisecunde;
                }
                //----------End of level 1-------------------
                //---------------Level 2--------------------
                else if (level == 2)
                {
                    if (level2 == false)
                    {
                        if (rez >= 0) puncte++;
                        else { Rememberstring(Ecuatia, rez, plus, Minus); puncte--; }
                        level2 = true;
                    }
                    if (rez >= 0) { puncte++; }
                    else { Rememberstring(Ecuatia, rez, plus, Minus); puncte--; }
                    int abs = -1; int absran;
                    label2.Text = puncte + "/" + score; if (puncte > score) puncte = score;
                    int a, b, co;
                    string c = "+-*";
                    a = r.Next(10);
                    b = r.Next(10);
                    co = r.Next(3);
                    while (co == 3) co = r.Next(3);
                    absran = r.Next(100);
                    if (absran % 2 == 0) a *= abs;
                    absran = r.Next(100);
                    if (absran % 3 == 0) b *= abs;
                    int modulec = r.Next(2);
                    int modula = r.Next(2);
                    int modulb = r.Next(2);
                    while (modulec == 2) modulec = r.Next(2);
                    while (modula == 2) modula = r.Next(2);
                    while (modulb == 2) modulb = r.Next(2);
                    char[] semn_modul = { ' ', '|' };
                    if (a >= 0 && b >= 0)
                    {
                        label1.Text = a + " " + c[co] + " " + b;
                        switch (co)
                        {
                            case 0: rez = a + b; break;
                            case 1: rez = a - b; break;
                            case 2: rez = a * b; break;
                        }
                    }
                    else if (a <= 0 && b >= 0)
                    {
                        label1.Text = semn_modul[modulec] + " " + semn_modul[modula] + "" + "(" + "" + a + "" + ")" + "" + semn_modul[modula] + " " + c[co] + " " + b + " " + semn_modul[modulec];
                        switch (co)
                        {
                            case 0:
                                {
                                    rez = a + b;
                                    if (modulec == 1)
                                    {
                                        rez = Math.Abs(rez);
                                    }
                                    else if (modula == 1)
                                    {
                                        rez = Math.Abs(a) + b;
                                    }

                                } break;
                            case 1:
                                {
                                    rez = a - b;
                                    if (modulec == 1)
                                    {
                                        rez = Math.Abs(rez);
                                    }
                                    else if (modula == 1)
                                    {
                                        rez = Math.Abs(a) - b;
                                    }

                                } break;
                            case 2:
                                {
                                    rez = a * b;
                                    if (modulec == 1)
                                    {
                                        rez = Math.Abs(rez);
                                    }
                                    else if (modula == 1)
                                    {
                                        rez = Math.Abs(a) * b;
                                    }

                                } break;
                        }
                    }
                    else if (a >= 0 && b <= 0)
                    {
                        label1.Text = semn_modul[modulec] + " " + a + " " + c[co] + " " + semn_modul[modulb] + "" + "(" + "" + b + "" + ")" + "" + semn_modul[modulb] + " " + semn_modul[modulec];
                        switch (co)
                        {
                            case 0:
                                {
                                    rez = a + b;
                                    if (modulec == 1)
                                    {
                                        rez = Math.Abs(rez);
                                    }
                                    else if (modulb == 1)
                                    {
                                        rez = a + Math.Abs(b);
                                    }

                                } break;
                            case 1:
                                {
                                    rez = a - b;
                                    if (modulec == 1)
                                    {
                                        rez = Math.Abs(rez);
                                    }
                                    else if (modulb == 1)
                                    {
                                        rez = a - Math.Abs(b);
                                    }

                                } break;
                            case 2:
                                {
                                    rez = a * b;
                                    if (modulec == 1)
                                    {
                                        rez = Math.Abs(rez);
                                    }
                                    else if (modulb == 1)
                                    {
                                        rez = a * Math.Abs(b);
                                    }

                                } break;
                        }
                    }
                    else if (a <= 0 && b <= 0)
                    {
                        label1.Text = semn_modul[modulec] + " " + semn_modul[modula] + "" + "(" + "" + a + "" + ")" + "" + semn_modul[modula] + " " + c[co] + " " + semn_modul[modulb] + "" + "(" + "" + b + "" + ")" + "" + semn_modul[modulb] + " " + semn_modul[modulec];
                        switch (co)
                        {
                            case 0:
                                {
                                    rez = a + b;
                                    if (modulec == 1)
                                    {
                                        rez = Math.Abs(rez);
                                    }
                                    else if (modulb == 1 && modula == 1)
                                    {
                                        rez = Math.Abs(a) + Math.Abs(b);
                                    }
                                    else
                                    {
                                        if (modula == 1)
                                        {
                                            rez = Math.Abs(a) + b;
                                        }
                                        if (modulb == 1)
                                        {
                                            rez = a + Math.Abs(b);
                                        }
                                    }
                                } break;
                            case 1:
                                {
                                    rez = a - b;
                                    if (modulec == 1)
                                    {
                                        rez = Math.Abs(rez);
                                    }
                                    else if (modula == 1 && modulb == 1)
                                    {
                                        rez = Math.Abs(a) - Math.Abs(b);
                                    }
                                    else
                                    {
                                        if (modula == 1)
                                        {
                                            rez = Math.Abs(a) - b;
                                        }
                                        if (modulb == 1)
                                        {
                                            rez = a - Math.Abs(b);
                                        }
                                    }

                                } break;
                            case 2:
                                {
                                    rez = a * b;
                                    if (modulec == 1)
                                    {
                                        rez = Math.Abs(rez);
                                    }
                                    else if (modulb == 1 && modula == 1)
                                    {
                                        rez = Math.Abs(a) * Math.Abs(b);
                                    }
                                    else
                                    {
                                        if (modula == 1)
                                        {
                                            rez = Math.Abs(a) * b;
                                        }
                                        if (modulb == 1)
                                        {
                                            rez = a * Math.Abs(b);
                                        }
                                    }

                                } break;
                        }
                    }
                    Ecuatia = label1.Text;
                    if (puncte % 10 == 0 && puncte > 19) level++;
                    Rezfin.Text = secr + text_scunde + " , " + milisecr + text_milisecunde;
                }
                else if (level >= 3)
                {

                    if (level3 == false)
                    {
                        if (rez >= 0) puncte++;
                        else { Rememberstring(Ecuatia, rez, plus, Minus); puncte--; }
                        level3 = true;
                    }

                    if (rez >= 0) { puncte++; }
                    else { Rememberstring(Ecuatia, rez, plus, Minus); puncte--; }
                    int abs = -1; int absran;
                    label2.Text = puncte + "/" + score; if (puncte > score) puncte = score;
                    int a, b, co;
                    string c = "+-*";
                    a = r.Next(10);
                    b = r.Next(10);
                    co = r.Next(3);
                    while (co == 3) co = r.Next(3);
                    absran = r.Next(100);
                    if (absran % 2 == 0) a *= abs;
                    absran = r.Next(100);
                    if (absran % 3 == 0) b *= abs;
                    int modulec = r.Next(999);
                    int modula = r.Next(2);
                    int modulb = r.Next(2);
                    int minus = r.Next(100);
                    while (modulec == 0) modulec = r.Next(999);
                    if (modulec % 72 == 0) modulec = 1;
                    else modulec = 0;
                    while (minus == 0) minus = r.Next(100);
                    if (minus % 5 == 0) minus = 2;
                    else minus = 0;
                    while (modula == 2) modula = r.Next(2);
                    while (modulb == 2) modulb = r.Next(2);
                    char[] semn_modul = { ' ', '|' };
                    string[] smn_minus = { " ", " ", "-(", ")" };
                    if (a >= 0 && b >= 0)
                    {
                        label1.Text = smn_minus[minus] + a + " " + c[co] + " " + b + smn_minus[minus + 1];
                        switch (co)
                        {
                            case 0:
                                {
                                    rez = a + b;
                                    if (minus == 2) rez *= (-1);
                                } break;
                            case 1:
                                {
                                    rez = a - b;
                                    if (minus == 2) rez *= (-1);
                                } break;
                            case 2:
                                {
                                    rez = a * b;
                                    if (minus == 2) rez *= (-1);
                                } break;
                        }
                    }
                    else if (a <= 0 && b >= 0)
                    {
                        label1.Text = smn_minus[minus] + semn_modul[modulec] + " " + semn_modul[modula] + "" + "(" + "" + a + "" + ")" + "" + semn_modul[modula] + " " + c[co] + " " + b + " " + semn_modul[modulec] + smn_minus[minus + 1];
                        switch (co)
                        {
                            case 0:
                                {
                                    rez = a + b;
                                    if (modulec == 1)
                                    {
                                        rez = Math.Abs(rez);
                                    }
                                    else if (modula == 1)
                                    {
                                        rez = Math.Abs(a) + b;
                                    }

                                } break;
                            case 1:
                                {
                                    rez = a - b;
                                    if (modulec == 1)
                                    {
                                        rez = Math.Abs(rez);
                                    }
                                    else if (modula == 1)
                                    {
                                        rez = Math.Abs(a) - b;
                                    }

                                } break;
                            case 2:
                                {
                                    rez = a * b;
                                    if (modulec == 1)
                                    {
                                        rez = Math.Abs(rez);
                                    }
                                    else if (modula == 1)
                                    {
                                        rez = Math.Abs(a) * b;
                                    }

                                } break;
                        }
                        if (minus == 2) rez *= (-1);
                    }
                    else if (a >= 0 && b <= 0)
                    {
                        label1.Text = smn_minus[minus] + semn_modul[modulec] + " " + a + " " + c[co] + " " + semn_modul[modulb] + "" + "(" + "" + b + "" + ")" + "" + semn_modul[modulb] + " " + semn_modul[modulec] + smn_minus[minus + 1];
                        switch (co)
                        {
                            case 0:
                                {
                                    rez = a + b;
                                    if (modulec == 1)
                                    {
                                        rez = Math.Abs(rez);
                                    }
                                    else if (modulb == 1)
                                    {
                                        rez = a + Math.Abs(b);
                                    }

                                } break;
                            case 1:
                                {
                                    rez = a - b;
                                    if (modulec == 1)
                                    {
                                        rez = Math.Abs(rez);
                                    }
                                    else if (modulb == 1)
                                    {
                                        rez = a - Math.Abs(b);
                                    }

                                } break;
                            case 2:
                                {
                                    rez = a * b;
                                    if (modulec == 1)
                                    {
                                        rez = Math.Abs(rez);
                                    }
                                    else if (modulb == 1)
                                    {
                                        rez = a * Math.Abs(b);
                                    }

                                } break;
                        }
                        if (minus == 2) rez *= (-1);
                    }
                    else if (a <= 0 && b <= 0)
                    {
                        label1.Text = smn_minus[minus] + semn_modul[modulec] + " " + semn_modul[modula] + "" + "(" + "" + a + "" + ")" + "" + semn_modul[modula] + " " + c[co] + " " + semn_modul[modulb] + "" + "(" + "" + b + "" + ")" + "" + semn_modul[modulb] + " " + semn_modul[modulec] + smn_minus[minus + 1];
                        switch (co)
                        {
                            case 0:
                                {
                                    rez = a + b;
                                    if (modulec == 1)
                                    {
                                        rez = Math.Abs(rez);
                                    }
                                    else if (modulb == 1 && modula == 1)
                                    {
                                        rez = Math.Abs(a) + Math.Abs(b);
                                    }
                                    else
                                    {
                                        if (modula == 1)
                                        {
                                            rez = Math.Abs(a) + b;
                                        }
                                        if (modulb == 1)
                                        {
                                            rez = a + Math.Abs(b);
                                        }
                                    }
                                } break;
                            case 1:
                                {
                                    rez = a - b;
                                    if (modulec == 1)
                                    {
                                        rez = Math.Abs(rez);
                                    }
                                    else if (modula == 1 && modulb == 1)
                                    {
                                        rez = Math.Abs(a) - Math.Abs(b);
                                    }
                                    else
                                    {
                                        if (modula == 1)
                                        {
                                            rez = Math.Abs(a) - b;
                                        }
                                        if (modulb == 1)
                                        {
                                            rez = a - Math.Abs(b);
                                        }
                                    }

                                } break;
                            case 2:
                                {
                                    rez = a * b;
                                    if (modulec == 1)
                                    {
                                        rez = Math.Abs(rez);
                                    }
                                    else if (modulb == 1 && modula == 1)
                                    {
                                        rez = Math.Abs(a) * Math.Abs(b);
                                    }
                                    else
                                    {
                                        if (modula == 1)
                                        {
                                            rez = Math.Abs(a) * b;
                                        }
                                        if (modulb == 1)
                                        {
                                            rez = a * Math.Abs(b);
                                        }
                                    }

                                } break;
                        }
                        if (minus == 2) rez *= (-1);
                    }
                    Ecuatia = label1.Text;
                    if (puncte % 10 == 0 && puncte > 29) level++;
                    Rezfin.Text = secr + text_scunde + " , " + milisecr + text_milisecunde;
                }
                //------------Level 3--------------------

                //---------End of level 3-----------------
                secr += sec;
                milisecr += milisec;
                if (milisecr >= 100) { milisecr = 0; secr++; }
                sec = 0;
                milisec = 0;
                minute = 0;
                ora = 0;
                Rezfin.Text = secr + text_scunde + " , " + milisecr + text_milisecunde;
                if (Admin == false)
                    if (score >= ScorMax)
                    {
                        timer4.Enabled = false;
                        button1.Visible = false;
                        button2.Visible = false;

                        Remember();
                        timer2.Enabled = false;
                        timer1.Enabled = false;
                        timer3.Enabled = false;
                        label5.Visible = false;
                        label6.Visible = false;
                        label7.Visible = false;
                        label8.Visible = false;
                        label9.Visible = false;

                        button5.Visible = true;
                        MessageBox.Show(Win_text + " " + puncte + text_puncte);
                        label1.Visible = false;
                        Hint.Visible = false;
                        timp.Visible = false;
                        string dataDeAzi = DateTime.Today.ToShortDateString();

                        string cmd = "INSERT INTO highscore(Nume,data,Timps,Timpms,Scor) VALUES(";
                        try
                        {
                            cmd = cmd + "'" + Nume.Text + "',"; //numele jucatorului
                            cmd = cmd + "'" + dataDeAzi + "',";
                            cmd = cmd + secr + "," + milisecr + ",";  //Timp 
                            cmd = cmd + puncte + ")";//Scor                 
                        }

                        catch (System.FormatException ex)
                        {
                            MessageBox.Show("Error " + ex.Message);
                        }
                        oleDbCommand1.CommandText = cmd;
                        oleDbConnection1.Open();
                        oleDbCommand1.ExecuteNonQuery();
                        oleDbConnection1.Close();
                        updatebd();
                        button4.Visible = true;
                    }
            }
            catch (Exception exp)
            {
                MessageBox.Show("Error " + exp.Message);
            }
            if (score < puncte) puncte = score;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (puncte >= 40 && puncte <= 44)
            {
                Random poz = new Random();
                int x = poz.Next(101);
                if (x % 3 == 0) x = 1;
                else x = 0;
                if (x == 1)
                {
                    int swap = button1.Left;
                    button1.Left = button2.Left;
                    button2.Left = swap;
                }
            }
            if (puncte >= 45 && puncte <= 49)
            {
                Random poz = new Random();
                int x = poz.Next(101);
                if (x % 2 == 0) x = 1;
                else x = 0;
                if (x == 1)
                {
                    int swap = button1.Left;
                    button1.Left = button2.Left;
                    button2.Left = swap;
                }
            }
            if (puncte >= 50)//kill screen
            {
                timer4.Enabled = true;

                timer2.Enabled = true;
                timer3.Enabled = true;
            }

            if (score < puncte) puncte = score;
            try
            {
                score++;
                //---------------Level 1----------
                if (level == 1)
                {
                    if (rez <= 0) puncte++;
                    else { Rememberstring(Ecuatia, rez, Minus, plus); puncte--; }
                    int abs = -1; int absran;
                    label2.Text = puncte + "/" + score; if (puncte > score) puncte = score;
                    int a, b, co;
                    string c = "+-*";
                    a = r.Next(10);
                    b = r.Next(10);
                    co = r.Next(3);
                    absran = r.Next(100);
                    if (absran % 2 == 0) a *= abs;
                    absran = r.Next(100);
                    if (absran % 3 == 0) b *= abs;
                    switch (co)
                    {
                        case 0: rez = a + b; break;
                        case 1: rez = a - b; break;
                        case 2: rez = a * b; break;
                    }
                    if (a >= 0 && b >= 0) label1.Text = a + " " + c[co] + " " + b;
                    else if (a <= 0 && b >= 0) label1.Text = "(" + a + ")" + " " + c[co] + " " + b;
                    else if (a >= 0 && b <= 0) label1.Text = a + " " + c[co] + " " + "(" + b + ")";
                    else label1.Text = "(" + a + ")" + " " + c[co] + " " + "(" + b + ")";
                    Ecuatia = label1.Text;
                    if (puncte % 10 == 0 && puncte > 9) level++;
                    Rezfin.Text = secr + text_scunde + " , " + milisecr + text_milisecunde;
                }
                //-----------End of level 1-------------------
                //---------------Level 2--------------------
                else if (level == 2)
                {
                    if (level2 == false)
                    {
                        if (rez <= 0) puncte++;
                        else { Rememberstring(Ecuatia, rez, Minus, plus); puncte--; }
                        level2 = true;
                    }
                    if (rez <= 0) { puncte++; }
                    else { Rememberstring(Ecuatia, rez, Minus, plus); puncte--; }
                    int abs = -1; int absran;
                    label2.Text = puncte + "/" + score; if (puncte > score) puncte = score;
                    int a, b, co;
                    string c = "+-*";
                    a = r.Next(10);
                    b = r.Next(10);
                    co = r.Next(3);
                    while (co == 3) co = r.Next(3);
                    absran = r.Next(100);
                    if (absran % 2 == 0) a *= abs;
                    absran = r.Next(100);
                    if (absran % 3 == 0) b *= abs;
                    int modulec = r.Next(2);
                    int modula = r.Next(2);
                    int modulb = r.Next(2);
                    while (modulec == 2) modulec = r.Next(2);
                    while (modula == 2) modula = r.Next(2);

                    while (modulb == 2) modulb = r.Next(2);
                    char[] semn_modul = { ' ', '|' };
                    if (a >= 0 && b >= 0)
                    {
                        label1.Text = a + " " + c[co] + " " + b;
                        switch (co)
                        {
                            case 0: rez = a + b; break;
                            case 1: rez = a - b; break;
                            case 2: rez = a * b; break;
                        }
                    }
                    else if (a <= 0 && b >= 0)
                    {
                        label1.Text = semn_modul[modulec] + " " + semn_modul[modula] + "" + "(" + "" + a + "" + ")" + "" + semn_modul[modula] + " " + c[co] + " " + b + " " + semn_modul[modulec];
                        switch (co)
                        {
                            case 0:
                                {
                                    rez = a + b;
                                    if (modulec == 1)
                                    {
                                        rez = Math.Abs(rez);
                                    }
                                    else if (modula == 1)
                                    {
                                        rez = Math.Abs(a) + b;
                                    }

                                } break;
                            case 1:
                                {
                                    rez = a - b;
                                    if (modulec == 1)
                                    {
                                        rez = Math.Abs(rez);
                                    }
                                    else if (modula == 1)
                                    {
                                        rez = Math.Abs(a) - b;
                                    }

                                } break;
                            case 2:
                                {
                                    rez = a * b;
                                    if (modulec == 1)
                                    {
                                        rez = Math.Abs(rez);
                                    }
                                    else if (modula == 1)
                                    {
                                        rez = Math.Abs(a) * b;
                                    }

                                } break;
                        }
                    }
                    else if (a >= 0 && b <= 0)
                    {
                        label1.Text = semn_modul[modulec] + " " + a + " " + c[co] + " " + semn_modul[modulb] + "" + "(" + "" + b + "" + ")" + "" + semn_modul[modulb] + " " + semn_modul[modulec];
                        switch (co)
                        {
                            case 0:
                                {
                                    rez = a + b;
                                    if (modulec == 1)
                                    {
                                        rez = Math.Abs(rez);
                                    }
                                    else if (modulb == 1)
                                    {
                                        rez = a + Math.Abs(b);
                                    }

                                } break;
                            case 1:
                                {
                                    rez = a - b;
                                    if (modulec == 1)
                                    {
                                        rez = Math.Abs(rez);
                                    }
                                    else if (modulb == 1)
                                    {
                                        rez = a - Math.Abs(b);
                                    }

                                } break;
                            case 2:
                                {
                                    rez = a * b;
                                    if (modulec == 1)
                                    {
                                        rez = Math.Abs(rez);
                                    }
                                    else if (modulb == 1)
                                    {
                                        rez = a * Math.Abs(b);
                                    }

                                } break;
                        }
                    }
                    else if (a <= 0 && b <= 0)
                    {
                        label1.Text = semn_modul[modulec] + " " + semn_modul[modula] + "" + "(" + "" + a + "" + ")" + "" + semn_modul[modula] + " " + c[co] + " " + semn_modul[modulb] + "" + "(" + b + ")" + "" + semn_modul[modulb] + " " + semn_modul[modulec];
                        switch (co)
                        {
                            case 0:
                                {
                                    rez = a + b;
                                    if (modulec == 1)
                                    {
                                        rez = Math.Abs(rez);
                                    }
                                    else if (modulb == 1 && modula == 1)
                                    {
                                        rez = Math.Abs(a) + Math.Abs(b);
                                    }
                                    else
                                    {
                                        if (modula == 1)
                                        {
                                            rez = Math.Abs(a) + b;
                                        }
                                        if (modulb == 1)
                                        {
                                            rez = a + Math.Abs(b);
                                        }
                                    }
                                } break;
                            case 1:
                                {
                                    rez = a - b;
                                    if (modulec == 1)
                                    {
                                        rez = Math.Abs(rez);
                                    }
                                    else if (modula == 1 && modulb == 1)
                                    {
                                        rez = Math.Abs(a) - Math.Abs(b);
                                    }
                                    else
                                    {
                                        if (modula == 1)
                                        {
                                            rez = Math.Abs(a) - b;
                                        }
                                        if (modulb == 1)
                                        {
                                            rez = a - Math.Abs(b);
                                        }
                                    }

                                } break;
                            case 2:
                                {
                                    rez = a * b;
                                    if (modulec == 1)
                                    {
                                        rez = Math.Abs(rez);
                                    }
                                    else if (modulb == 1 && modula == 1)
                                    {
                                        rez = Math.Abs(a) * Math.Abs(b);
                                    }
                                    else
                                    {
                                        if (modula == 1)
                                        {
                                            rez = Math.Abs(a) * b;
                                        }
                                        if (modulb == 1)
                                        {
                                            rez = a * Math.Abs(b);
                                        }
                                    }

                                } break;
                        }
                    }
                    Ecuatia = label1.Text;
                    if (puncte % 10 == 0 && puncte > 19) level++;
                    Rezfin.Text = secr + text_scunde + " , " + milisecr + text_milisecunde;
                }
                //------------Level 3--------------------
                else if (level >= 3)
                {
                    if (level3 == false)
                    {

                        if (rez <= 0) puncte++;
                        else { Rememberstring(Ecuatia, rez, Minus, plus); puncte--; }
                        level3 = true;
                    }
                    if (rez <= 0) { puncte++; }
                    else { Rememberstring(Ecuatia, rez, Minus, plus); puncte--; }
                    int abs = -1; int absran;
                    label2.Text = puncte + "/" + score; if (puncte > score) puncte = score;
                    int a, b, co;
                    string c = "+-*";
                    a = r.Next(10);
                    b = r.Next(10);
                    co = r.Next(3);
                    while (co == 3) co = r.Next(3);
                    absran = r.Next(100);
                    if (absran % 2 == 0) a *= abs;
                    absran = r.Next(100);
                    if (absran % 3 == 0) b *= abs;
                    int modulec = r.Next(999);
                    int modula = r.Next(2);
                    int modulb = r.Next(2);
                    int minus = r.Next(100);
                    while (modulec == 0) modulec = r.Next(999);
                    if (modulec % 7 == 0) modulec = 1;
                    else modulec = 0;
                    while (minus == 0) minus = r.Next(100);
                    if (minus % 5 == 0) minus = 2;
                    else minus = 0;
                    while (modula == 2) modula = r.Next(2);
                    while (modulb == 2) modulb = r.Next(2);
                    char[] semn_modul = { ' ', '|' };
                    string[] smn_minus = { " ", " ", "-(", ")" };
                    if (a >= 0 && b >= 0)
                    {
                        label1.Text = smn_minus[minus] + a + " " + c[co] + " " + b + smn_minus[minus + 1];
                        switch (co)
                        {
                            case 0: rez = a + b; break;
                            case 1: rez = a - b; break;
                            case 2: rez = a * b; break;
                        }
                        if (minus == 2) rez *= (-1);
                    }
                    else if (a <= 0 && b >= 0)
                    {
                        label1.Text = smn_minus[minus] + semn_modul[modulec] + " " + semn_modul[modula] + "" + "(" + "" + a + "" + ")" + "" + semn_modul[modula] + " " + c[co] + " " + b + " " + semn_modul[modulec] + smn_minus[minus + 1];
                        switch (co)
                        {
                            case 0:
                                {
                                    rez = a + b;
                                    if (modulec == 1)
                                    {
                                        rez = Math.Abs(rez);
                                    }
                                    else if (modula == 1)
                                    {
                                        rez = Math.Abs(a) + b;
                                    }

                                } break;
                            case 1:
                                {
                                    rez = a - b;
                                    if (modulec == 1)
                                    {
                                        rez = Math.Abs(rez);
                                    }
                                    else if (modula == 1)
                                    {
                                        rez = Math.Abs(a) - b;
                                    }

                                } break;
                            case 2:
                                {
                                    rez = a * b;
                                    if (modulec == 1)
                                    {
                                        rez = Math.Abs(rez);
                                    }
                                    else if (modula == 1)
                                    {
                                        rez = Math.Abs(a) * b;
                                    }

                                } break;
                        }
                        if (minus == 2) rez *= (-1);
                    }
                    else if (a >= 0 && b <= 0)
                    {
                        label1.Text = smn_minus[minus] + semn_modul[modulec] + " " + a + " " + c[co] + " " + semn_modul[modulb] + "" + "(" + "" + b + "" + ")" + "" + semn_modul[modulb] + " " + semn_modul[modulec] + smn_minus[minus + 1];
                        switch (co)
                        {
                            case 0:
                                {
                                    rez = a + b;
                                    if (modulec == 1)
                                    {
                                        rez = Math.Abs(rez);
                                    }
                                    else if (modulb == 1)
                                    {
                                        rez = a + Math.Abs(b);
                                    }

                                } break;
                            case 1:
                                {
                                    rez = a - b;
                                    if (modulec == 1)
                                    {
                                        rez = Math.Abs(rez);
                                    }
                                    else if (modulb == 1)
                                    {
                                        rez = a - Math.Abs(b);
                                    }

                                } break;
                            case 2:
                                {
                                    rez = a * b;
                                    if (modulec == 1)
                                    {
                                        rez = Math.Abs(rez);
                                    }
                                    else if (modulb == 1)
                                    {
                                        rez = a * Math.Abs(b);
                                    }

                                } break;
                        }
                        if (minus == 2) rez *= (-1);
                    }
                    else if (a <= 0 && b <= 0)
                    {
                        label1.Text = smn_minus[minus] + semn_modul[modulec] + " " + semn_modul[modula] + "" + "(" + "" + a + "" + ")" + "" + semn_modul[modula] + " " + c[co] + " " + semn_modul[modulb] + "" + "(" + b + ")" + "" + semn_modul[modulb] + " " + semn_modul[modulec] + smn_minus[minus + 1];
                        switch (co)
                        {
                            case 0:
                                {
                                    rez = a + b;
                                    if (modulec == 1)
                                    {
                                        rez = Math.Abs(rez);
                                    }
                                    else if (modulb == 1 && modula == 1)
                                    {
                                        rez = Math.Abs(a) + Math.Abs(b);
                                    }
                                    else
                                    {
                                        if (modula == 1)
                                        {
                                            rez = Math.Abs(a) + b;
                                        }
                                        if (modulb == 1)
                                        {
                                            rez = a + Math.Abs(b);
                                        }
                                    }
                                } break;
                            case 1:
                                {
                                    rez = a - b;
                                    if (modulec == 1)
                                    {
                                        rez = Math.Abs(rez);
                                    }
                                    else if (modula == 1 && modulb == 1)
                                    {
                                        rez = Math.Abs(a) - Math.Abs(b);
                                    }
                                    else
                                    {
                                        if (modula == 1)
                                        {
                                            rez = Math.Abs(a) - b;
                                        }
                                        if (modulb == 1)
                                        {
                                            rez = a - Math.Abs(b);
                                        }
                                    }

                                } break;
                            case 2:
                                {
                                    rez = a * b;
                                    if (modulec == 1)
                                    {
                                        rez = Math.Abs(rez);
                                    }
                                    else if (modulb == 1 && modula == 1)
                                    {
                                        rez = Math.Abs(a) * Math.Abs(b);
                                    }
                                    else
                                    {
                                        if (modula == 1)
                                        {
                                            rez = Math.Abs(a) * b;
                                        }
                                        if (modulb == 1)
                                        {
                                            rez = a * Math.Abs(b);
                                        }
                                    }

                                } break;
                        }
                        if (minus == 2) rez *= (-1);
                    }
                    Ecuatia = label1.Text;
                    if (puncte % 10 == 0 && puncte > 29) level++;
                    Rezfin.Text = secr + text_scunde + " , " + milisecr + text_milisecunde;
                }

                //---------End of level 3-----------------
                secr += sec;
                milisecr += milisec;
                if (milisecr >= 100) { secr++; milisecr = 0; }
                sec = 0;
                milisec = 0;
                minute = 0;
                ora = 0;
                Rezfin.Text = secr + text_scunde + " , " + milisecr + text_milisecunde;
                if (Admin == false)
                    if (score >= ScorMax)
                    {
                        timer4.Enabled = false;
                        Remember();
                        button2.Visible = false;
                        button1.Visible = false;
                        timer4.Enabled = false;//may god have mercy
                        timer2.Enabled = false;
                        timer1.Enabled = false;
                        timer3.Enabled = false;
                        label5.Visible = false;
                        label6.Visible = false;
                        label7.Visible = false;
                        label8.Visible = false;
                        label9.Visible = false;
                        button5.Visible = true;
                        MessageBox.Show(Win_text + " " + puncte + text_puncte);
                        label1.Visible = false;
                        Hint.Visible = false;
                        timp.Visible = false;
                        string dataDeAzi = DateTime.Today.ToShortDateString();
                        string cmd = "INSERT INTO highscore(Nume,Data,Timps,Timpms,Scor) VALUES(";
                        try
                        {
                            cmd = cmd + "'" + Nume.Text + "',"; //numele jucatorului
                            cmd = cmd + "'" + dataDeAzi + "',";//DAta
                            cmd = cmd + secr + "," + milisecr + ",";//Timp                      
                            cmd = cmd + puncte + ")";//Scor                 
                        }

                        catch (System.FormatException ex)
                        {
                            MessageBox.Show("Error! " + ex.Message);
                        }

                        oleDbCommand1.CommandText = cmd;
                        oleDbConnection1.Open();
                        oleDbCommand1.ExecuteNonQuery();
                        oleDbConnection1.Close();
                        updatebd();
                        button4.Visible = true;
                    }
            }
            catch (Exception exp)
            {
                MessageBox.Show("Error " + exp.Message);
            }
            if (score < puncte) puncte = score;
        }

        bool effect = true;
        Random cul = new Random();
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Interval = 25;
            // int x=cul.Next(3);
            if (milisec < 100)
            {
                milisec += 2;
                if (milisec >= 100)
                {
                    milisec = -1;
                    sec++;
                    if (sec >= 60)
                    {
                        sec = 0;
                        minute++;
                        if (minute >= 60)
                            ora++;
                    }
                }
            }
            timp.Text = ora + ":" + minute + ":" + sec + ":" + milisec;
        }

        DataSet ds;
        OleDbDataAdapter da;
        BindingSource bs;
        OleDbCommandBuilder cb;

        private void Nume_TextChanged(object sender, EventArgs e)
        {
            a_scris_nume = true;
        }

        private void Nume_Click(object sender, EventArgs e)
        {
            Nume.Clear();
        }
        int b1t, b1l, b2t, b2l,fw,fh;
        private void Form1_Load(object sender, EventArgs e)
        {
            b1l = button1.Left;
            b1t = button1.Top;
            b2l = button2.Left;
            b2t = button2.Top;
            fw = this.Width;
            fh = this.Height;

            textBox2.Text = Help_text;
            checkBox1.Checked = true;
            updatebd();
        }

        private void highscoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panel2.Height =fh;
            panel2.Width =fw;
           
            panel1.Height = 10;
            panel1.Width = 10; 
            panel3.Height = 10;
            panel3.Width = 10; 
            panel4.Height = 10;
            panel4.Width = 10; 
            panel5.Height = 10;
            panel5.Width = 10;

            panel1.Visible = false;
            panel2.Visible = true;
            panel3.Visible = false;
            panel4.Visible = false; panel5.Visible = false;
            dataGridView1.Visible = true;
        }

        private void gameToolStripMenuItem_Click(object sender, EventArgs e)
        {

            panel1.Height = fh;
            panel1.Width = fw;

            panel4.Height = 10;
            panel4.Width = 10;
            panel3.Height = 10;
            panel3.Width = 10;
            panel2.Height = 10;
            panel2.Width = 10;
            panel5.Height = 10;
            panel5.Width = 10;
            panel1.Visible = true;
            panel2.Visible = false;
            panel3.Visible = false;
            panel4.Visible = false;
            panel5.Visible = false;
        }

        private void performantaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panel3.Height =fh;
            panel3.Width = fw;

            panel1.Height = 10;
            panel1.Width = 10;
            panel2.Height = 10;
            panel2.Width = 10;
            panel4.Height = 10;
            panel4.Width = 10;
            panel5.Height = 10;
            panel5.Width = 10;
            panel1.Visible = false;
            panel5.Visible = false;
            panel2.Visible = false;
            panel3.Visible = true;
            panel4.Visible = false;
            listBox1.Items.Clear();
            nrNote = 0;


            string sql = "SELECT DISTINCT nume FROM Highscore ORDER BY nume";

            try
            {
                conexiune = new OleDbConnection(Connectiontext);
                conexiune.Open();

                OleDbCommand comanda = new OleDbCommand(sql, conexiune);
                OleDbDataReader dr = comanda.ExecuteReader();

                while (dr.Read())
                {
                    string n = dr[0].ToString();
                    listBox1.Items.Add(n);
                }

                conexiune.Close();
            }
            catch (OleDbException exc)
            {
                MessageBox.Show("Error! " + exc.Message);
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            updatebd();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            updatebd();
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            updatebd();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            checkBox2.Checked = false;
            for (int i = 1; i <= 900; i++) ;
            updatebd();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

            checkBox1.Checked = false;
            for (int i = 1; i <= 900; i++) ;
            updatebd();
        }

        int nrNote;
        int[] note;
        string[] date;
        string peCineAmSelectat;
        string cresterepnc = " a avut o crestere in punctaj";
        string scaderepnc = " a avut oscadere in punctaj";
        bool stare;
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            nrNote = 0;
            peCineAmSelectat = listBox1.SelectedItem.ToString();
            label1.Text = "Rezultatele elevului " + peCineAmSelectat;

            string sql = @"SELECT data, scor
                          FROM Highscore 
                          WHERE nume='" + peCineAmSelectat + "' ORDER BY data,scor";

            try
            {
                conexiune = new OleDbConnection(Connectiontext);
                conexiune.Open();

                OleDbCommand comanda = new OleDbCommand(sql, conexiune);
                OleDbDataReader dr = comanda.ExecuteReader();

                nrNote = 0;
                note = new int[30];
                date = new string[30];
                while (dr.Read())
                {
                    nrNote++;
                    note[nrNote] = int.Parse(dr[1].ToString());
                    int pozSpatiu = dr[0].ToString().IndexOf(' ');
                    date[nrNote] = dr[0].ToString().Substring(0, pozSpatiu);
                }

                pictureBox1.Invalidate();

                if (nrNote > 1 && note[nrNote] > note[nrNote - 1])
                {
                    Perform.Text = peCineAmSelectat + cresterepnc;
                    stare = true;

                }
                if (nrNote > 1 && note[nrNote] < note[nrNote - 1])
                {
                    Perform.Text = peCineAmSelectat + scaderepnc;
                    stare = false;
                }
                conexiune.Close();
            }
            catch (OleDbException exc)
            {
                MessageBox.Show("Eroare!" + exc.Message);
            }
        }
        Graphics g;

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (nrNote != 0)
            {
                g = e.Graphics;
                g.FillRectangle(Brushes.White, 0, 0, pictureBox1.Width, pictureBox1.Height);
                Pen p = new Pen(Color.Black);
                Font f1 = new Font("Arial", 8, FontStyle.Bold);
                Brush b1 = new SolidBrush(Color.Black);
                Brush b2 = new SolidBrush(Color.Coral);

                int x0 = 30;
                int y0 = pictureBox1.Height - 70;

                g.DrawLine(p, x0, y0, pictureBox1.Width - 20, y0); //Ox
                g.DrawLine(p, x0, 5, x0, y0);                     //Oy  

                int L = (pictureBox1.Width - 50) / nrNote;
                int H = y0 / 61;

                for (int i = 1; i <= nrNote; i++)
                {
                    g.DrawLine(p, i * L, y0 - 2, i * L, y0 + 2);
                    g.DrawString(date[i], f1, b1, i * L - 20, y0 + 5);
                    g.FillEllipse(b2, i * L - 3, y0 - note[i] * H - 3, 5, 5);
                }
                g.DrawString((0).ToString(), f1, b1, x0 - 20, 60 * H - 5);
                for (int i = 1; i <= 60; i += 5)
                {
                    g.DrawLine(p, x0 - 2, i * H, x0 + 2, i * H);
                    g.DrawString((60 - i + 1).ToString(), f1, b1, x0 - 20, i * H - 5);
                }

                for (int i = 1; i < nrNote; i++)
                {
                    g.DrawLine(p, i * L, y0 - note[i] * H, (i + 1) * L, y0 - note[i + 1] * H);
                }
            }

        }
        private void sterge_greseli()
        {
            string sql = "Delete from Greseli";
            conexiune = new OleDbConnection(Connectiontext);
            conexiune.Open();
            OleDbCommand cmd = new OleDbCommand(sql, conexiune);
            cmd.ExecuteNonQuery();
            conexiune.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            button1.Visible = true;
            button2.Visible = true;
            button1.Left = b1l;
            button1.Top = b1t;
            button2.Left = b2l;
            button2.Top = b2t;
            button5.Visible = false;
            dataGridView2.Visible = false;
            button3.Visible = true;
            sterge_greseli();
            score = 0;
            puncte = 0;
            sec = 0;
            secr = 0;
            milisec = 0;
            milisecr = 0;
            label2.Text = "";
            Rezfin.Text = "";
            label1.Visible = true;
            Hint.Visible = true;
            timp.Visible = true;
            button4.Visible = false;
            label5.Visible = false;
            label6.Visible = false;
            label7.Visible = false;
            label8.Visible = false;
            label9.Visible = false;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            dataGridView2.Visible = true;
            AfisGres();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            //Kill SCREEN
            Random r = new Random();
            int xs = r.Next(panel1.Size.Width);
            int ys = r.Next(panel1.Size.Height);
            while (xs == 0 || xs + button1.Width >= panel1.Size.Width) xs = r.Next(panel1.Size.Width);
            while (ys == 0 || ys + button1.Height >= panel1.Size.Height) ys = r.Next(panel1.Size.Height);
            button2.Location = new Point(xs, ys);

        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            //Kill SCREEN

            Random r = new Random();
            int xs = r.Next(panel1.Size.Width);
            int ys = r.Next(panel1.Size.Height);
            while (xs == 0 || xs + button1.Width >= panel1.Size.Width) xs = r.Next(panel1.Size.Width);
            while (ys == 0 || ys + button1.Height >= panel1.Size.Height) ys = r.Next(panel1.Size.Height);
            button1.Location = new Point(xs, ys);
        }

        private void button7_Click(object sender, EventArgs e)
        {

            if (textBox1.Text == "parolamea")
            {
                string sql = "Delete From Highscore";
                oleDbCommand1.CommandText = sql;
                oleDbConnection1.Open();
                oleDbCommand1.ExecuteNonQuery();
                oleDbConnection1.Close();



            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            button7.Visible = true;
            textBox1.Visible = true;
        }
        Random re = new Random();
        private void timer4_Tick(object sender, EventArgs e)
        {

            if (puncte >= 50)
            {
                int rez = 456;
                if (effect == true)
                {

                    label5.Visible = effect;
                    while (rez == 0) rez = re.Next(800);
                    timer4.Interval = rez;

                    label6.Visible = effect;
                    while (rez == 0) rez = re.Next(800);
                    timer4.Interval = rez;

                    label7.Visible = effect;
                    while (rez == 0) rez = re.Next(800);
                    timer4.Interval = rez;

                    label8.Visible = effect;
                    while (rez == 0) rez = re.Next(800);
                    timer4.Interval = rez;

                    label9.Visible = effect;
                    while (rez == 0) rez = re.Next(800);
                    timer4.Interval = rez;

                    effect = false;
                    while (rez == 0) rez = re.Next(800);
                    timer4.Interval = rez;
                }
                else
                {
                    label5.Visible = effect; while (rez == 0) rez = re.Next(800);
                    timer4.Interval = rez;
                    label6.Visible = effect; while (rez == 0) rez = re.Next(800);
                    timer4.Interval = rez;
                    label7.Visible = effect; while (rez == 0) rez = re.Next(800);
                    timer4.Interval = rez;
                    label8.Visible = effect; while (rez == 0) rez = re.Next(800);
                    timer4.Interval = rez;
                    label9.Visible = effect; while (rez == 0) rez = re.Next(800);
                    timer4.Interval = rez;
                    effect = true;
                    timer4.Interval = re.Next(800);
                }
            }
            label5.Location = new Point(re.Next(panel1.Width), re.Next(panel1.Height));
            label6.Location = new Point(re.Next(panel1.Width), re.Next(panel1.Height));
            label7.Location = new Point(re.Next(panel1.Width), re.Next(panel1.Height));
            label8.Location = new Point(re.Next(panel1.Width), re.Next(panel1.Height));
            label9.Location = new Point(re.Next(panel1.Width), re.Next(panel1.Height));
            int x = re.Next(4);
            switch (x)
            {
                case 0:
                    {
                        label5.ForeColor = System.Drawing.Color.Red;
                        label6.ForeColor = System.Drawing.Color.Indigo;
                        label7.ForeColor = System.Drawing.Color.LightGray;
                        label8.ForeColor = System.Drawing.Color.OldLace;
                        label9.ForeColor = System.Drawing.Color.WhiteSmoke;
                    } break;
                case 1:
                    {
                        label5.ForeColor = System.Drawing.Color.Green;
                        label6.ForeColor = System.Drawing.Color.MediumSlateBlue;
                        label7.ForeColor = System.Drawing.Color.LightGreen;
                        label8.ForeColor = System.Drawing.Color.DarkSlateBlue;
                        label9.ForeColor = System.Drawing.Color.Gainsboro;
                    } break;
                case 3:
                    {
                        label5.ForeColor = System.Drawing.Color.Gold;
                        label6.ForeColor = System.Drawing.Color.LightSalmon;
                        label7.ForeColor = System.Drawing.Color.DarkTurquoise;
                        label8.ForeColor = System.Drawing.Color.Honeydew;
                        label9.ForeColor = System.Drawing.Color.HotPink;
                    } break;
            }
        }

        private void button6_MouseHover(object sender, EventArgs e)
        {
            button6.Visible = false;
            button8.Visible = true;

        }

        private void button6_MouseLeave(object sender, EventArgs e)
        {

        }

        private void panel3_MouseHover(object sender, EventArgs e)
        {
            button6.Visible = true;
            button8.Visible = false;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            button7.Visible = true;
            textBox1.Visible = true;
        }

        private void românăToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Hint.Text = "Semnul operaţiei este";
            label5.Text = "Ecranul morții";
            label6.Text = "Ecranul morţii";
            label7.Text = "Ecranul morţii";
            label8.Text = "Ecranul morţii";
            label9.Text = "Ecranul morţii";
            button3.Text = "Începe";
            button4.Text = "Joc Nou";
            if (stare == true) Perform.Text = " a avut o crestere in punctaj";
            else
            {
                Perform.Text = " a avut o scadere in punctaj";
            }
            button5.Text = "Greşeli";
            Lab4Score.Text = "Scorul tău este";
            Nume.Text = "Introdu-ţi numele";
            radioButton1.Text = "Nume";
            radioButton2.Text = "Timp";
            radioButton3.Text = "Scor";
            checkBox1.Text = "Ascendent";
            checkBox2.Text = "Descendent";
            checkBox3.Text = "Ajutor în timpul jocului";
            groupBox1.Text = "Alege metoda de ordonare";
            toolStripDropDownButton2.Text = "Limbă";
            gameToolStripMenuItem.Text = "Joc";
            highscoreToolStripMenuItem.Text = "Clasament";
            performantaToolStripMenuItem.Text = "Performanta";
            button6.Text = "Resetează";
            button8.Text = "Resetează";
            Text_eroare = "Te rog să-ţi introduci numele";
            this.Text = "Plus și Minus";
            text_milisecunde = " milisecunde";
            label3.Text = "Autor";
            toolStripButton3.Text = "Autor";
            text_scunde = " secunde";
            Lab4Timp_total.Text = " Timpul tău total este de";
            cresterepnc = " a avut o creştere în punctaj";
            scaderepnc = " a avut o scădere în punctaj";
            Win_text = "Jocul s-a sfârşit.Ai";
            text_puncte = " puncte";
            textBox3.Text = @"
                                    Bine ai venit la jocul ''Plus si Minus''! 
                                    Scopul acestui Joc este sa raspunzi la o intrebare simpla:  
                                               Care este Semnul Operatiei? 
Click pe primul buton de sus şi pe submeniul Joc te va duce la meniul jocului.Aici iţi vei introduce numele  
şi vei apăsa pe butonul 'Începe'.Vor apărea 2 butoane +(Plus) şi respectiv -(Minus)  
Deasupra acestora va apărea o formulă matematică.Trebuie să răspunzi la intrebarea  
jocului apăsând unul din cele 2 butoane.ATENTIE!In cazul in care rezultatul este 0 poti apasa si pe
Plus si pe Minus.Formulele pot conţine şi modul.
După terminarea jocului vor apărea 2 butoane care iţi vor permite fie inceperea unui joc nou   
sau să iti examinezi greşelile făcute pe parcurs.De asemenea în submeniul clasament vei putea  
vedea şi scorurile celorlalţi jucători.În submeniul Performanţă poţi vedea ce performanţă ai avut 
dealungul mai multor jocuri.Alegeţi limba din meniul Limba.Meniul Ajutor te va aduce aici
";
        }

        private void englishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Hint.Text = "The operation sign is";
            if (stare == true) Perform.Text = " had an increse in score";
            else
            {
                Perform.Text = " had a decrease in score";
            }
            label5.Text = "Kill screen";
            label6.Text = "Kill screen";
            label7.Text = "Kill screen";
            label8.Text = "Kill screen";
            label9.Text = "Kill screen";
            button3.Text = "Start";
            button4.Text = "New Game";
            button5.Text = "Mistakes";
            Lab4Score.Text = "Your score is:";
            Nume.Text = "Enter your name";
            radioButton1.Text = "Name";
            radioButton2.Text = "Time";
            radioButton3.Text = "Score";
            label3.Text = "Author: Stroescu Bogdan Theodor";
            checkBox1.Text = "Ascending";
            checkBox2.Text = "Descending";
            groupBox1.Text = "Choose ordering method";
            button6.Text = "Reset";
            button8.Text = "Reset";
            checkBox3.Text = "Help during the game";
            gameToolStripMenuItem.Text = "Game";
            highscoreToolStripMenuItem.Text = "HighScore";
            performantaToolStripMenuItem.Text = "Performance";
            Text_eroare = "Please enter your name";
            toolStripDropDownButton2.Text = "Language";
            toolStripButton3.Text = "Author";
            this.Text = "Plus and minus";
            text_milisecunde = " milliseconds";
            text_scunde = " seconds";
            Lab4Timp_total.Text = " Your total time is:";
            cresterepnc = " had an increase in score";
            scaderepnc = " had a decrease in score";
            Win_text = "The game ended. You have: ";
            text_puncte = " points";
            textBox2.Text=@"
                                Welcome to the ''Plus and Minus'' Game !
                                The purpose of the game is answering one question:
                                        What’s the sign of the operation?
If you click the first button above and on the submenu Game it will bring up the game menu. Here you will enter 
your name and press on the Start button. Two buttons will appear +(Plus) and –(Minus).Above them, there will be
a mathematical operation. You have to answer the games question by pressing one of the two buttons. ATTENTION! 
In case of a zero you can press either buttons. The operations can contain modules. After the game ends two more 
buttons will appear that will allow you to start a new game or see you mistakes. Also in the submenu Highscore you 
can see the scores of the other players. In the Performance submenu you can see your performance during several games. 
Pick your language from the Language menu. The Help button will bring you back here.";
        }

        private void francaisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Hint.Text = "Le signe de operation est";
            label5.Text = "L'écran de la mort";
            label6.Text = "L'écran de la mort";
            label7.Text = "L'écran de la mort";
            label8.Text = "L'écran de la mort";
            if (stare == true) Perform.Text = " On a eu une evolution ";
            else
            {
                Perform.Text = " On a eu une diminution";
            }
            label9.Text = "L'écran de morte";
            checkBox3.Text = "Aider pendant le jeu";
            button3.Text = "Début";
            button4.Text = "Nouveau jeu";
            label3.Text = "Auteur: Stroescu Bogdan Theodor";
            button5.Text = "Fautes";
            Lab4Score.Text = "Ton score est:";
            Nume.Text = "Introduis ton nom";
            radioButton1.Text = "Nom";
            radioButton2.Text = "Temps";
            radioButton3.Text = "Score";
            gameToolStripMenuItem.Text = "Jeu";
            highscoreToolStripMenuItem.Text = "Classament";
            performantaToolStripMenuItem.Text = "Performence";
            checkBox1.Text = "Ascendant";
            checkBox2.Text = "Descendant";
            groupBox1.Text = "Choisir la méthode de commande";
            toolStripDropDownButton2.Text = "Langue";
            button6.Text = "Réinitialise";
            button8.Text = "Réinitialise";
            Text_eroare = "S’il te plait, introduis ton nom";
            toolStripButton3.Text = "Auteur";
            this.Text = "Plus et moins";
            text_milisecunde = " millisecondes";
            text_scunde = " secondes";
            Lab4Timp_total.Text = " Ton temps total est:";
            cresterepnc = " On a eu une evolution";
            scaderepnc = " On a eu une diminution";
            Win_text = " Le jeu s’est fini. Tu as";
            text_puncte = " points";
         
            textBox2.Text =
            @" 
                                    Soyez les bienvenus au jeu ,,Plus et moins’’!
                                    Le but de ce jeu est de répondre à une question simple :
                                             Quel est le Signe de l’operation ?
Click sur le premier bouton d’en haut et sur le sousmenu Jeu et on arrive au menu du jeu. Ici on introduit le nom 
et on appuye sur le bouton ,,Commence’’. Deux boutons vont apparaître :  +(plus),  respectivement –(moins).
Au dessus on va apparaître une operation mathématique. Il faut répondre à la question du jeu en appuyant un 
des deux boutons. ATTENTION ! au cas où le résultat est 0, on peut appuyer aussi Plus ou Moins. Les formules 
peuvent contenir aussi le modul.Après la fin du jeu, deux boutons vont apparaître ; ceux-ci permettront soit 
le début d’un nouveau jeu, soit l’examen des fautes faites au cours du jeu. Dans le sousmenu classement on peut 
voir les scores des autres joueurs. Dans le menu Performence on peut voir le meilleur résultat au long de plusieurs 
jeux. On choisit la langue du menu Langue. Le menu Aide montrera comment faire.";
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            panel4.Height =fh;;
            panel4.Width =fw;

            panel1.Height = 10;
            panel1.Width = 10;
            panel3.Height = 10;
            panel3.Width = 10;
            panel2.Height = 10;
            panel2.Width = 10;
            panel5.Height = 10;
            panel5.Width = 10;

            panel1.Visible = false;
            panel5.Visible = false;
            panel2.Visible = false;
            panel3.Visible = false;
            panel4.Visible = true;

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toolStripLabel1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            panel5.Height =fh;;
            panel5.Width =fw;

            panel1.Height = 10;
            panel1.Width = 10;
            panel3.Height = 10;
            panel3.Width = 10;
            panel2.Height = 10;
            panel2.Width = 10;
            panel4.Height = 10;
            panel4.Width = 10;

            panel1.Visible = false;
            panel2.Visible = false;
            panel3.Visible = false;
            panel4.Visible = false;
            panel5.Visible = true;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            /*
             * if (checkBox3.Checked == true)
            {
                scrie.SetToolTip(button1, "");
                scrie.SetToolTip(button2, "");
                scrie.SetToolTip(button3, "");
                scrie.SetToolTip(button4, "");
                scrie.SetToolTip(button5, "");
                scrie.SetToolTip(button6, "");
                scrie.SetToolTip(button7, "");
                scrie.SetToolTip(button8, "");
                scrie.SetToolTip(checkBox1, "");
                scrie.SetToolTip(checkBox2, "");
                scrie.SetToolTip(checkBox3, "");
                scrie.SetToolTip(dataGridView2, "");
                scrie.SetToolTip(dataGridView1, "");
                scrie.SetToolTip(Lab4Score, "");
                scrie.SetToolTip(Lab4Timp_total, "");
                scrie.SetToolTip(label1, "");
                scrie.SetToolTip(label2, "");
                scrie.SetToolTip(label5, "");
                scrie.SetToolTip(label6, "");
                scrie.SetToolTip(label7, "");
                scrie.SetToolTip(label8, "");
                scrie.SetToolTip(label9, "");
                scrie.SetToolTip(listBox1, "");
                scrie.SetToolTip(Nume, "");
                scrie.SetToolTip(Perform, "");
                scrie.SetToolTip(pictureBox1, "");
                scrie.SetToolTip(radioButton1, "");
                scrie.SetToolTip(radioButton2, "");
                scrie.SetToolTip(radioButton3, "");
                scrie.SetToolTip(Rezfin, "");
                scrie.SetToolTip(textBox1, "");
                scrie.SetToolTip(textBox2, "");
                scrie.SetToolTip(timp, "");

            }
              */
        }
        
        private void toolStripButton2_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }








    }

}
