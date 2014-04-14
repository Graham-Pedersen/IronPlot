using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CompilerLib;
using Microsoft.VisualBasic.PowerPacks;

namespace _2048GUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

       ObjBox Board;
       CompilerLib.Environment env;

        private void Form1_Load(object sender, EventArgs e)
        {
            Console.WriteLine("TEST");
            this.BringToFront();
            this.Focus();
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(Form1_KeyDown);
            env = DLL2048.init();
            Board = (ObjBox) DLL2048.init_board(env, new List<object>());
            redraw_gui(Board);
        }



        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Right:
                case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                    return true;
            }
            return base.IsInputKey(keyData);
        }
      
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            List<object> obj = new List<object>();
            switch (e.KeyCode)
            {
                case Keys.W://up
                    obj.Add((object)Board);
                    Board = (ObjBox) DLL2048.move_board_up_helper(env, obj);
                   // DLL2048.debug_display(env, obj);
                    redraw_gui(Board);
                    break;
                case Keys.A://left
                    obj.Add((object)Board);
                    Board = (ObjBox) DLL2048.move_board_left_helper(env, obj);
                  //  DLL2048.debug_display(env, obj);
                    redraw_gui(Board);
                    break;
                case Keys.S://down
                    obj.Add((object)Board);
                    Board = (ObjBox) DLL2048.move_board_down_helper(env, obj);
                   // DLL2048.net_debug_display(env, obj);
                    redraw_gui(Board);
                    break;
                case Keys.D://right
                    obj.Add((object)Board);
                    Board = (ObjBox) DLL2048.move_board_right_helper(env, obj);
                  //  DLL2048.debug_display(env, obj);
                    redraw_gui(Board);
                    break;
            }
            
        }


        private void redraw_gui(ObjBox board)
        {
            RacketPair rp = getRP(board);
            int temp = 0;
            int holder = 0;
            System.Drawing.Color c;

            for (int i = 0; i < 4; i++)
            {
                RacketPair row = getRP(rp.car());
                rp = getRP(rp.cdr());
                
                for (int j = 0; j < 4; j++)
                {
                    holder = getVal(row.car());
                    c = num2Color(holder);
                    ((RectangleShape) this.shapeContainer1.Shapes.get_Item(temp+j)).BackColor = c;
                    labelGrid[temp + j].Text = (holder == 0) ? "" : (holder + "");
                    if (holder > 4)
                    {
                        labelGrid[temp + j].ForeColor = System.Drawing.ColorTranslator.FromHtml("#F9F6F2");
                    }
                    else
                    {
                        labelGrid[temp + j].ForeColor = System.Drawing.ColorTranslator.FromHtml("#776E65");
                    }
                    labelGrid[temp+j].BackColor = c;
                    row = getRP(row.cdr());
                }
                temp+=4;
            }
        }


        static int getVal(ObjBox val)
        {
            RacketInt rint = (RacketInt) val.getObj();
            return rint.value;
        }

        static RacketPair getRP(ObjBox val)
        {
            return (RacketPair)val.getObj();
        }
    }





}
