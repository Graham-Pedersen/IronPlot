using System.Drawing;
using System.Windows.Input;
namespace _2048GUI
{


    // 2 and 4 text color: #776E65
    //#F9F6F2 rest
    //

    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private static System.Windows.Forms.Label[] labelGrid;





        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code



        private System.Drawing.Color num2Color(int num)
        {
            switch (num)
            {
                case 0:
                    return System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(238)))), ((int)(((byte)(228)))), ((int)(((byte)(218)))));
                case 2:
                    return System.Drawing.ColorTranslator.FromHtml("#EEE4DA");
                case 4:
                    return System.Drawing.ColorTranslator.FromHtml("#EDE0C8");
                case 8:
                    return System.Drawing.ColorTranslator.FromHtml("#F2B179");
                case 16:
                    return System.Drawing.ColorTranslator.FromHtml("#F59563");
                case 32:
                    return System.Drawing.ColorTranslator.FromHtml("#F67C5F");
                case 64:
                    return System.Drawing.ColorTranslator.FromHtml("#F65E3B");
                case 128:
                    return System.Drawing.ColorTranslator.FromHtml("#EDCF72");
                case 256:
                    return System.Drawing.ColorTranslator.FromHtml("#EDCC61");
                case 512:
                    return System.Drawing.ColorTranslator.FromHtml("#EDC850");
                case 1024:
                    return System.Drawing.ColorTranslator.FromHtml("#EDC53F");
                case 2048:
                    return System.Drawing.ColorTranslator.FromHtml("#EDC22E");
                default:
                    return System.Drawing.ColorTranslator.FromHtml("#3C3A32");
            }
        }



        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.shapeContainer1 = new Microsoft.VisualBasic.PowerPacks.ShapeContainer();
            this.rectangleShape16 = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.rectangleShape15 = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.rectangleShape14 = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.rectangleShape13 = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.rectangleShape12 = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.rectangleShape11 = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.rectangleShape10 = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.rectangleShape9 = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.rectangleShape8 = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.rectangleShape7 = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.rectangleShape6 = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.rectangleShape5 = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.rectangleShape4 = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.rectangleShape3 = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.rectangleShape2 = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.rectangleShape1 = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            labelGrid = new System.Windows.Forms.Label[] {
                this.label1,
                this.label2,
                this.label3,
                this.label4,
                this.label5,
                this.label6,
                this.label7,
                this.label8,
                this.label9,
                this.label10,
                this.label11,
                this.label12,
                this.label13,
                this.label14,
                this.label15,
                this.label16,
            };




            this.SuspendLayout();
            // 
            // shapeContainer1
            // 
            this.shapeContainer1.Location = new System.Drawing.Point(0, 0);
            this.shapeContainer1.Margin = new System.Windows.Forms.Padding(0);
            this.shapeContainer1.Name = "shapeContainer1";
            this.shapeContainer1.Shapes.AddRange(new Microsoft.VisualBasic.PowerPacks.Shape[] {
            this.rectangleShape1,
            this.rectangleShape2,
            this.rectangleShape3,
            this.rectangleShape4,
            this.rectangleShape5,
            this.rectangleShape6,
            this.rectangleShape7,
            this.rectangleShape8,
            this.rectangleShape9,
            this.rectangleShape10,
            this.rectangleShape11,
            this.rectangleShape12,
            this.rectangleShape13,
            this.rectangleShape14,
            this.rectangleShape15,
            this.rectangleShape16});
            this.shapeContainer1.Size = new System.Drawing.Size(419, 372);
            this.shapeContainer1.TabIndex = 0;
            this.shapeContainer1.TabStop = false;
            // 
            // rectangleShape16
            // 
            this.rectangleShape16.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(238)))), ((int)(((byte)(228)))), ((int)(((byte)(218)))));
            this.rectangleShape16.BackStyle = Microsoft.VisualBasic.PowerPacks.BackStyle.Opaque;
            this.rectangleShape16.Location = new System.Drawing.Point(306, 278);
            this.rectangleShape16.Name = "rectangleShape16";
            this.rectangleShape16.Size = new System.Drawing.Size(87, 75);
            // 
            // rectangleShape15
            // 
            this.rectangleShape15.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(238)))), ((int)(((byte)(228)))), ((int)(((byte)(218)))));
            this.rectangleShape15.BackStyle = Microsoft.VisualBasic.PowerPacks.BackStyle.Opaque;
            this.rectangleShape15.Location = new System.Drawing.Point(212, 278);
            this.rectangleShape15.Name = "rectangleShape15";
            this.rectangleShape15.Size = new System.Drawing.Size(87, 75);
            // 
            // rectangleShape14
            // 
            this.rectangleShape14.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(238)))), ((int)(((byte)(228)))), ((int)(((byte)(218)))));
            this.rectangleShape14.BackStyle = Microsoft.VisualBasic.PowerPacks.BackStyle.Opaque;
            this.rectangleShape14.Location = new System.Drawing.Point(118, 278);
            this.rectangleShape14.Name = "rectangleShape14";
            this.rectangleShape14.Size = new System.Drawing.Size(87, 75);
            // 
            // rectangleShape13
            // 
            this.rectangleShape13.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(238)))), ((int)(((byte)(228)))), ((int)(((byte)(218)))));
            this.rectangleShape13.BackStyle = Microsoft.VisualBasic.PowerPacks.BackStyle.Opaque;
            this.rectangleShape13.Location = new System.Drawing.Point(25, 278);
            this.rectangleShape13.Name = "rectangleShape13";
            this.rectangleShape13.Size = new System.Drawing.Size(87, 75);
            // 
            // rectangleShape12
            // 
            this.rectangleShape12.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(238)))), ((int)(((byte)(228)))), ((int)(((byte)(218)))));
            this.rectangleShape12.BackStyle = Microsoft.VisualBasic.PowerPacks.BackStyle.Opaque;
            this.rectangleShape12.Location = new System.Drawing.Point(306, 197);
            this.rectangleShape12.Name = "rectangleShape12";
            this.rectangleShape12.Size = new System.Drawing.Size(87, 75);
            // 
            // rectangleShape11
            // 
            this.rectangleShape11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(238)))), ((int)(((byte)(228)))), ((int)(((byte)(218)))));
            this.rectangleShape11.BackStyle = Microsoft.VisualBasic.PowerPacks.BackStyle.Opaque;
            this.rectangleShape11.Location = new System.Drawing.Point(212, 197);
            this.rectangleShape11.Name = "rectangleShape11";
            this.rectangleShape11.Size = new System.Drawing.Size(87, 75);
            // 
            // rectangleShape10
            // 
            this.rectangleShape10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(238)))), ((int)(((byte)(228)))), ((int)(((byte)(218)))));
            this.rectangleShape10.BackStyle = Microsoft.VisualBasic.PowerPacks.BackStyle.Opaque;
            this.rectangleShape10.Location = new System.Drawing.Point(118, 197);
            this.rectangleShape10.Name = "rectangleShape10";
            this.rectangleShape10.Size = new System.Drawing.Size(87, 75);
            // 
            // rectangleShape9
            // 
            this.rectangleShape9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(238)))), ((int)(((byte)(228)))), ((int)(((byte)(218)))));
            this.rectangleShape9.BackStyle = Microsoft.VisualBasic.PowerPacks.BackStyle.Opaque;
            this.rectangleShape9.Location = new System.Drawing.Point(25, 197);
            this.rectangleShape9.Name = "rectangleShape9";
            this.rectangleShape9.Size = new System.Drawing.Size(87, 75);
            // 
            // rectangleShape8
            // 
            this.rectangleShape8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(238)))), ((int)(((byte)(228)))), ((int)(((byte)(218)))));
            this.rectangleShape8.BackStyle = Microsoft.VisualBasic.PowerPacks.BackStyle.Opaque;
            this.rectangleShape8.Location = new System.Drawing.Point(306, 116);
            this.rectangleShape8.Name = "rectangleShape8";
            this.rectangleShape8.Size = new System.Drawing.Size(87, 75);
            // 
            // rectangleShape7
            // 
            this.rectangleShape7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(238)))), ((int)(((byte)(228)))), ((int)(((byte)(218)))));
            this.rectangleShape7.BackStyle = Microsoft.VisualBasic.PowerPacks.BackStyle.Opaque;
            this.rectangleShape7.Location = new System.Drawing.Point(212, 116);
            this.rectangleShape7.Name = "rectangleShape7";
            this.rectangleShape7.Size = new System.Drawing.Size(87, 75);
            // 
            // rectangleShape6
            // 
            this.rectangleShape6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(238)))), ((int)(((byte)(228)))), ((int)(((byte)(218)))));
            this.rectangleShape6.BackStyle = Microsoft.VisualBasic.PowerPacks.BackStyle.Opaque;
            this.rectangleShape6.Location = new System.Drawing.Point(118, 116);
            this.rectangleShape6.Name = "rectangleShape6";
            this.rectangleShape6.Size = new System.Drawing.Size(87, 75);
            // 
            // rectangleShape5
            // 
            this.rectangleShape5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(238)))), ((int)(((byte)(228)))), ((int)(((byte)(218)))));
            this.rectangleShape5.BackStyle = Microsoft.VisualBasic.PowerPacks.BackStyle.Opaque;
            this.rectangleShape5.Location = new System.Drawing.Point(25, 116);
            this.rectangleShape5.Name = "rectangleShape5";
            this.rectangleShape5.Size = new System.Drawing.Size(87, 75);
            // 
            // rectangleShape4
            // 
            this.rectangleShape4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(238)))), ((int)(((byte)(228)))), ((int)(((byte)(218)))));
            this.rectangleShape4.BackStyle = Microsoft.VisualBasic.PowerPacks.BackStyle.Opaque;
            this.rectangleShape4.Location = new System.Drawing.Point(306, 35);
            this.rectangleShape4.Name = "rectangleShape4";
            this.rectangleShape4.Size = new System.Drawing.Size(87, 75);
            // 
            // rectangleShape3
            // 
            this.rectangleShape3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(238)))), ((int)(((byte)(228)))), ((int)(((byte)(218)))));
            this.rectangleShape3.BackStyle = Microsoft.VisualBasic.PowerPacks.BackStyle.Opaque;
            this.rectangleShape3.Location = new System.Drawing.Point(212, 35);
            this.rectangleShape3.Name = "rectangleShape3";
            this.rectangleShape3.Size = new System.Drawing.Size(87, 75);
            // 
            // rectangleShape2
            // 
            this.rectangleShape2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(238)))), ((int)(((byte)(228)))), ((int)(((byte)(218)))));
            this.rectangleShape2.BackStyle = Microsoft.VisualBasic.PowerPacks.BackStyle.Opaque;
            this.rectangleShape2.Location = new System.Drawing.Point(118, 35);
            this.rectangleShape2.Name = "rectangleShape2";
            this.rectangleShape2.Size = new System.Drawing.Size(87, 75);
            // 
            // rectangleShape1
            // 
            this.rectangleShape1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(238)))), ((int)(((byte)(228)))), ((int)(((byte)(218)))));
            this.rectangleShape1.BackStyle = Microsoft.VisualBasic.PowerPacks.BackStyle.Opaque;
            this.rectangleShape1.Location = new System.Drawing.Point(25, 35);
            this.rectangleShape1.Name = "rectangleShape1";
            this.rectangleShape1.Size = new System.Drawing.Size(87, 75);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 18F);
            this.label1.Location = new System.Drawing.Point(47, 59);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 27);
            this.label1.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 18F);
            this.label2.Location = new System.Drawing.Point(144, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 27);
            this.label2.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 18F);
            this.label3.Location = new System.Drawing.Point(230, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(0, 27);
            this.label3.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial", 18F);
            this.label4.Location = new System.Drawing.Point(329, 59);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(0, 27);
            this.label4.TabIndex = 4;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial", 18F);
            this.label5.Location = new System.Drawing.Point(47, 143);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(0, 27);
            this.label5.TabIndex = 5;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Arial", 18F);
            this.label6.Location = new System.Drawing.Point(144, 143);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(0, 27);
            this.label6.TabIndex = 6;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Arial", 18F);
            this.label7.Location = new System.Drawing.Point(230, 143);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(0, 27);
            this.label7.TabIndex = 7;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Arial", 18F);
            this.label8.Location = new System.Drawing.Point(329, 143);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(0, 27);
            this.label8.TabIndex = 8;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Arial", 18F);
            this.label9.Location = new System.Drawing.Point(47, 224);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(0, 27);
            this.label9.TabIndex = 9;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Arial", 18F);
            this.label10.Location = new System.Drawing.Point(144, 224);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(0, 27);
            this.label10.TabIndex = 10;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Arial", 18F);
            this.label11.Location = new System.Drawing.Point(230, 224);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(0, 27);
            this.label11.TabIndex = 11;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Arial", 18F);
            this.label12.Location = new System.Drawing.Point(329, 224);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(0, 27);
            this.label12.TabIndex = 12;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Arial", 18F);
            this.label13.Location = new System.Drawing.Point(47, 301);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(0, 27);
            this.label13.TabIndex = 13;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Arial", 18F);
            this.label14.Location = new System.Drawing.Point(144, 301);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(0, 27);
            this.label14.TabIndex = 14;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Arial", 18F);
            this.label15.Location = new System.Drawing.Point(230, 301);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(0, 27);
            this.label15.TabIndex = 15;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Arial", 18F);
            this.label16.Location = new System.Drawing.Point(329, 301);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(0, 27);
            this.label16.TabIndex = 16;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(187)))), ((int)(((byte)(173)))), ((int)(((byte)(160)))));
            this.ClientSize = new System.Drawing.Size(419, 372);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.shapeContainer1);
            this.Name = "Form1";
            this.Text = "2048";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Microsoft.VisualBasic.PowerPacks.ShapeContainer shapeContainer1;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape rectangleShape1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape rectangleShape16;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape rectangleShape15;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape rectangleShape14;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape rectangleShape13;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape rectangleShape12;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape rectangleShape11;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape rectangleShape10;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape rectangleShape9;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape rectangleShape8;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape rectangleShape7;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape rectangleShape6;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape rectangleShape5;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape rectangleShape4;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape rectangleShape3;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape rectangleShape2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
    }
}

