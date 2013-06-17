/* Filename:    UDM.cs
 * Program:     UDM
 * Version:     7.0 SR13
 * Programmers: Terry Beckhelm
 *              Daniel Flyte (C# revision)
 * Description: This program..............
 *                       
 * 
 * Revision History
 * STR             Date       By    Description
 * --------------------------------------------------------------------------
 *                 04/08/97   tb    Initial coding
 *                 07/16/03   df    C# revision
 *                 02/06/09   tb    sr13 updates - resorting back to sr13
 *                 07/17/12   tb    Series 13 changes
 * --------------------------------------------------------------------------
 */
 

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading;

namespace Sandag.TechSvcs.RegionalModels
{
    public class UDM : System.Windows.Forms.Form
    {
    
        #region Instance Fields
   
        private System.Data.SqlClient.SqlConnection sqlConnection;
        private System.Data.SqlClient.SqlCommand sqlCommand;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.StatusBar txtStatus;
        private System.Windows.Forms.Label lblModuleSelect;
        private System.Windows.Forms.MainMenu mainMenu1;
        private System.Windows.Forms.RadioButton radModule1;
        private System.Windows.Forms.RadioButton radModule2;
        private System.Windows.Forms.RadioButton radModule4;
        private System.Windows.Forms.RadioButton radModule3;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.StatusBarPanel statusBarPanel;
        private System.Windows.Forms.StatusBarPanel statusBarPanel1;
        private System.ComponentModel.IContainer components;
        #endregion Fields

        // Constructor
        public UDM()
	    {
		    InitializeComponent();
        }

		
		#region Windows Form Designer generated code
    /// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UDM));
            this.sqlConnection = new System.Data.SqlClient.SqlConnection();
            this.sqlCommand = new System.Data.SqlClient.SqlCommand();
            this.btnRun = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.txtStatus = new System.Windows.Forms.StatusBar();
            this.statusBarPanel = new System.Windows.Forms.StatusBarPanel();
            this.statusBarPanel1 = new System.Windows.Forms.StatusBarPanel();
            this.lblModuleSelect = new System.Windows.Forms.Label();
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.radModule1 = new System.Windows.Forms.RadioButton();
            this.radModule2 = new System.Windows.Forms.RadioButton();
            this.radModule4 = new System.Windows.Forms.RadioButton();
            this.radModule3 = new System.Windows.Forms.RadioButton();
            this.lblTitle = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel1)).BeginInit();
            this.SuspendLayout();
            // 
            // sqlConnection
            // 
            this.sqlConnection.ConnectionString = "workstation id=TBE;packet size=4096;data source=PILA\\SdgIntDb;user = forecast;pas" +
                "sword = forecast;persist security info=False;initial catalog=sr13";
            this.sqlConnection.FireInfoMessageEventOnUserErrors = false;
            // 
            // sqlCommand
            // 
            this.sqlCommand.Connection = this.sqlConnection;
            // 
            // btnRun
            // 
            this.btnRun.BackColor = System.Drawing.Color.LightGreen;
            this.btnRun.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRun.Location = new System.Drawing.Point(40, 305);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(72, 40);
            this.btnRun.TabIndex = 1;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = false;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // btnExit
            // 
            this.btnExit.BackColor = System.Drawing.Color.Red;
            this.btnExit.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExit.Location = new System.Drawing.Point(120, 305);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(72, 40);
            this.btnExit.TabIndex = 2;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = false;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // txtStatus
            // 
            this.txtStatus.Location = new System.Drawing.Point(0, 384);
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
            this.statusBarPanel,
            this.statusBarPanel1});
            this.txtStatus.Size = new System.Drawing.Size(449, 10);
            this.txtStatus.TabIndex = 3;
            // 
            // statusBarPanel
            // 
            this.statusBarPanel.MinWidth = 50;
            this.statusBarPanel.Name = "statusBarPanel";
            this.statusBarPanel.Text = "statusBarPanel";
            this.statusBarPanel.Width = 50;
            // 
            // statusBarPanel1
            // 
            this.statusBarPanel1.Name = "statusBarPanel1";
            this.statusBarPanel1.Text = "statusBarPanel1";
            // 
            // lblModuleSelect
            // 
            this.lblModuleSelect.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblModuleSelect.Location = new System.Drawing.Point(24, 64);
            this.lblModuleSelect.Name = "lblModuleSelect";
            this.lblModuleSelect.Size = new System.Drawing.Size(128, 16);
            this.lblModuleSelect.TabIndex = 6;
            this.lblModuleSelect.Text = "Module";
            // 
            // radModule1
            // 
            this.radModule1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radModule1.Location = new System.Drawing.Point(32, 83);
            this.radModule1.Name = "radModule1";
            this.radModule1.Size = new System.Drawing.Size(358, 23);
            this.radModule1.TabIndex = 10;
            this.radModule1.Text = "1 -  Preprocessor (Includes Access Weights)";
            // 
            // radModule2
            // 
            this.radModule2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radModule2.Location = new System.Drawing.Point(32, 112);
            this.radModule2.Name = "radModule2";
            this.radModule2.Size = new System.Drawing.Size(208, 24);
            this.radModule2.TabIndex = 11;
            this.radModule2.Text = "2 -  Employment Allocation";
            // 
            // radModule4
            // 
            this.radModule4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radModule4.Location = new System.Drawing.Point(32, 176);
            this.radModule4.Name = "radModule4";
            this.radModule4.Size = new System.Drawing.Size(317, 26);
            this.radModule4.TabIndex = 13;
            this.radModule4.Text = "4 -  Detailed MGRA Characteristics";
            // 
            // radModule3
            // 
            this.radModule3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radModule3.Location = new System.Drawing.Point(32, 144);
            this.radModule3.Name = "radModule3";
            this.radModule3.Size = new System.Drawing.Size(216, 26);
            this.radModule3.TabIndex = 12;
            this.radModule3.Text = "3 -  Housing Stock Allocation";
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.Navy;
            this.lblTitle.Location = new System.Drawing.Point(8, 16);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(136, 32);
            this.lblTitle.TabIndex = 14;
            this.lblTitle.Text = "UDM";
            // 
            // UDM
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(449, 394);
            this.Controls.Add(this.radModule4);
            this.Controls.Add(this.radModule3);
            this.Controls.Add(this.radModule2);
            this.Controls.Add(this.radModule1);
            this.Controls.Add(this.lblModuleSelect);
            this.Controls.Add(this.txtStatus);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Menu = this.mainMenu1;
            this.Name = "UDM";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "UDM Version 7 (SR13)";
            ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel1)).EndInit();
            this.ResumeLayout(false);

    }
		#endregion

	[STAThread]

	    static void Main() 
        {
		    Application.Run( new UDM() );
	    }

        private void btnRun_Click( object sender, System.EventArgs e )
        {
          if( !radModule1.Checked && !radModule2.Checked && !radModule3.Checked && !radModule4.Checked )
          {
            MessageBox.Show( "Please select a valid module to run.", "Invalid Selection" );
            return;
          }

          if( radModule1.Checked )
          {
            PreprocessorForm pre = new PreprocessorForm( this );
            pre.Show();
          }  // end if

          else if( radModule2.Checked )
          {
            CivEmp emp = new CivEmp( this );
            emp.Show();
          }  // end else if

          else if( radModule3.Checked )
          {
            HousingStock hs = new HousingStock( this );
            hs.Show();
          } // end else if

          else if (radModule4.Checked)
          {
            Detailed dc = new Detailed( this );
            dc.Show();
          } // end else if

        }     // End main


        private void btnExit_Click(object sender, System.EventArgs e)
        {
          Close();
        }


        private void mnuItemExit_Click(object sender, System.EventArgs e)
        {
          Close();
        }
    }  // end class UDM
}  // end namespace