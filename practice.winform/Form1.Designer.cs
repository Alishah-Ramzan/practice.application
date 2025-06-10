namespace practice.winform
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context?.Dispose();

                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }


        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            textBox1 = new TextBox();
            textBox2 = new TextBox();
            submit_button = new Button();
            label3 = new Label();
            label4 = new Label();
            dataGridView1 = new DataGridView();
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
            textBox3 = new TextBox();
            label2 = new Label();
            label5 = new Label();
            dataGridView2 = new DataGridView();
            button4 = new Button();
            textBox4 = new TextBox();
            button5 = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView2).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Location = new Point(42, 32);
            label1.Name = "label1";
            label1.Size = new Size(96, 25);
            label1.TabIndex = 0;
            label1.Text = "Input Data";
            label1.Click += label1_Click;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(168, 72);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(150, 31);
            textBox1.TabIndex = 2;
            textBox1.TextChanged += textBox1_TextChanged;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(168, 126);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(150, 31);
            textBox2.TabIndex = 3;
            textBox2.TextChanged += textBox2_TextChanged;
            // 
            // submit_button
            // 
            submit_button.BackColor = Color.Lime;
            submit_button.Location = new Point(50, 182);
            submit_button.Name = "submit_button";
            submit_button.Size = new Size(112, 34);
            submit_button.TabIndex = 4;
            submit_button.Text = "Submit";
            submit_button.UseVisualStyleBackColor = false;
            submit_button.Click += button1_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(51, 75);
            label3.Name = "label3";
            label3.Size = new Size(56, 25);
            label3.TabIndex = 5;
            label3.Text = "name";
            label3.Click += label3_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(51, 132);
            label4.Name = "label4";
            label4.Size = new Size(50, 25);
            label4.TabIndex = 6;
            label4.Text = "price";
            label4.Click += label4_Click;
            // 
            // dataGridView1
            // 
            dataGridView1.BackgroundColor = SystemColors.ButtonHighlight;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(424, 32);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 62;
            dataGridView1.Size = new Size(613, 358);
            dataGridView1.TabIndex = 7;
            dataGridView1.CellContentClick += dataGridView1_CellContentClick;
            // 
            // button1
            // 
            button1.BackColor = SystemColors.Info;
            button1.Location = new Point(685, 409);
            button1.Name = "button1";
            button1.Size = new Size(112, 34);
            button1.TabIndex = 8;
            button1.Text = "View All";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click_1;
            // 
            // button2
            // 
            button2.BackColor = SystemColors.MenuHighlight;
            button2.Location = new Point(51, 345);
            button2.Name = "button2";
            button2.Size = new Size(112, 34);
            button2.TabIndex = 9;
            button2.Text = "Edit";
            button2.UseVisualStyleBackColor = false;
            button2.Click += button2_Click;
            // 
            // button3
            // 
            button3.BackColor = Color.Red;
            button3.Location = new Point(192, 345);
            button3.Name = "button3";
            button3.Size = new Size(112, 34);
            button3.TabIndex = 10;
            button3.Text = "Delete";
            button3.UseVisualStyleBackColor = false;
            button3.Click += button3_Click;
            // 
            // textBox3
            // 
            textBox3.Location = new Point(168, 289);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(150, 31);
            textBox3.TabIndex = 11;
            textBox3.TextChanged += textBox3_TextChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(56, 289);
            label2.Name = "label2";
            label2.Size = new Size(27, 25);
            label2.TabIndex = 12;
            label2.Text = "id";
            label2.Click += label2_Click;
            // 
            // label5
            // 
            label5.Font = new Font("Segoe UI", 10F);
            label5.ForeColor = SystemColors.ButtonHighlight;
            label5.Location = new Point(28, 241);
            label5.Name = "label5";
            label5.Size = new Size(390, 45);
            label5.TabIndex = 13;
            label5.Text = "Input ID to Edit/Delete Name and PRICE";
            label5.Click += label5_Click;
            // 
            // dataGridView2
            // 
            dataGridView2.BackgroundColor = SystemColors.ButtonHighlight;
            dataGridView2.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView2.Location = new Point(65, 500);
            dataGridView2.Name = "dataGridView2";
            dataGridView2.RowHeadersWidth = 62;
            dataGridView2.Size = new Size(972, 229);
            dataGridView2.TabIndex = 14;
            dataGridView2.CellContentClick += dataGridView2_CellContentClick;
            // 
            // button4
            // 
            button4.BackColor = SystemColors.Info;
            button4.Location = new Point(444, 815);
            button4.Name = "button4";
            button4.Size = new Size(150, 35);
            button4.TabIndex = 15;
            button4.Text = "Search/Save";
            button4.UseVisualStyleBackColor = false;
            button4.Click += button4_Click;
            // 
            // textBox4
            // 
            textBox4.Location = new Point(444, 757);
            textBox4.Name = "textBox4";
            textBox4.Size = new Size(150, 31);
            textBox4.TabIndex = 16;
            textBox4.TextChanged += textBox4_TextChanged;
            // 
            // button5
            // 
            button5.Location = new Point(790, 802);
            button5.Name = "button5";
            button5.Size = new Size(112, 34);
            button5.TabIndex = 17;
            button5.Text = "form2";
            button5.UseVisualStyleBackColor = true;
            button5.Click += button5_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.SkyBlue;
            ClientSize = new Size(1123, 937);
            Controls.Add(button5);
            Controls.Add(textBox4);
            Controls.Add(button4);
            Controls.Add(dataGridView2);
            Controls.Add(label5);
            Controls.Add(label2);
            Controls.Add(textBox3);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(dataGridView1);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(submit_button);
            Controls.Add(textBox2);
            Controls.Add(textBox1);
            Controls.Add(label1);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView2).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox textBox1;
        private TextBox textBox2;
        private Button submit_button;
        private Label label3;
        private Label label4;
        private DataGridView dataGridView1;
        private Button button1;
        private Button button2;
        private Button button3;
        private TextBox textBox3;
        private Label label2;
        private Label label5;
        private DataGridView dataGridView2;
        private Button button4;
        private TextBox textBox4;
        private Button button5;
    }
}
