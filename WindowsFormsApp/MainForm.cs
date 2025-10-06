using System;
using System.Drawing;
using System.Windows.Forms;

namespace ConfigManagementApp
{
    public partial class MainForm : Form
    {
        private Button buttonHello;
        private Label labelWelcome;
        private TextBox textBoxName;
        private Button buttonClear;

        public MainForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Form properties
            this.Text = "Config Management Application";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Welcome label
            labelWelcome = new Label();
            labelWelcome.Text = "Добро пожаловать в приложение управления конфигурацией!";
            labelWelcome.Size = new Size(360, 40);
            labelWelcome.Location = new Point(20, 20);
            labelWelcome.TextAlign = ContentAlignment.MiddleCenter;
            labelWelcome.Font = new Font("Arial", 10, FontStyle.Bold);
            this.Controls.Add(labelWelcome);

            // Name input label
            Label labelName = new Label();
            labelName.Text = "Введите ваше имя:";
            labelName.Size = new Size(120, 20);
            labelName.Location = new Point(20, 80);
            this.Controls.Add(labelName);

            // Text box for name input
            textBoxName = new TextBox();
            textBoxName.Size = new Size(200, 25);
            textBoxName.Location = new Point(150, 78);
            this.Controls.Add(textBoxName);

            // Hello button
            buttonHello = new Button();
            buttonHello.Text = "Поздороваться";
            buttonHello.Size = new Size(100, 30);
            buttonHello.Location = new Point(20, 120);
            buttonHello.Click += ButtonHello_Click;
            this.Controls.Add(buttonHello);

            // Clear button
            buttonClear = new Button();
            buttonClear.Text = "Очистить";
            buttonClear.Size = new Size(100, 30);
            buttonClear.Location = new Point(130, 120);
            buttonClear.Click += ButtonClear_Click;
            this.Controls.Add(buttonClear);
        }

        private void ButtonHello_Click(object sender, EventArgs e)
        {
            string name = textBoxName.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Пожалуйста, введите ваше имя!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show($"Привет, {name}! Добро пожаловать в приложение!", "Приветствие", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ButtonClear_Click(object sender, EventArgs e)
        {
            textBoxName.Clear();
            textBoxName.Focus();
        }
    }
}