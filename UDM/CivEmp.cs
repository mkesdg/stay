/* Filename:    CivEmp.cs
 * Program:     UDM
 * Version:     7.0 sr13
 * Programmers: Terry Beckhelm
 *              Daniel Flyte (C# revision)
 * Description: This form commands all actions associated with module 2, 
 *              civilian employment processing.  It is called from UDM, after 
 *              the Civilian Employment button has been selected.
 * 
 * Includes procedures
 *  beginCivEmpWork()
 *  applyEmpOvr()
 *  closeEmpFiles()
 *  controlEmpNoOvr()
 *  controlEmpWithOvr()
 *  doEmpTotals1()
 *  empCalc()
 *  empDecrements()
 *  empLand()
 *  empTransactions()
 *  extractEmpLUOvr()
 *  extractImpedAM()
 *  extractRCEmp()
 *  getUDMEmpLU()
 *  loadEmpDecremens()
 *  loadEmpIncrements()
 *  minConstraintEmp()
 *  openFiles()
 *  printAuxEmp()
 *  printEmpOutliers()
 *  printEmpOvr()
 *  printEmpTable4()
 *  printEmpTable5()
 *  printEmpTable6()
 *  printEmpTable7()
 *  printEmpTable8()
 *  printEmpTable9()
 *  printEmpTable10()
 *  printEmpTableSpecial()
 *  processParams()
 *  redistributeEmp()
 *  updateEmpDecrements()
 *  updateEmpIncrements()
 *  writeEmpDecrements()
 *  writeEmpIncrements()
 *  writeEmpParams()
 */

using System;
using System.Collections;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Configuration;


namespace Sandag.TechSvcs.RegionalModels
{
	public class CivEmp : BaseForm
	{
        #region Instance fields
        private bool doEmpOvr;       // Do LUZ employment overrides
        private bool ctrlParmE;      /* Controlling switch.  FALSE to exempt civilian employment overrides from controlling; TRUE to control civilian emp overrides. */

        private int[] zbi;
        private int[,] impedAM;

        private RegionalControls rc;        // Regional controls from defm
        private StreamWriter empChange;     // Employment change output
        private StreamWriter empChange1;    /* Employment change output printed in landscape */
        private StreamWriter empOvr;        /* ASCII output for employment overrides */
        private StreamWriter empOut;        // Employment outliers output
        private StreamWriter empCd;         /* Temp file for employment decrements updates */
        private StreamWriter empCi;         /* Temp file for employment increments updates */
    
        private StreamWriter minConstraints;      /* Minimum constraints output listing file */
        private StreamWriter redisEmp;            /* LUZ employment redistribution listing file */
        private StreamWriter schPart;             /* Partially developed school site records */
           
        private TCapD[,] tcapd;       /* Temporary structure for capacity query in emp decrement routine */
        private TCap[,] tCapI;        /* Temporary structure for capacity query in emp increment routine */
        private TTP[] tt;
        private System.Windows.Forms.Label lblMinConstr;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.TextBox txtMinContraint;
        private System.Windows.Forms.CheckBox chkControlEmpOvr;
        private System.Windows.Forms.CheckBox chkEmpOvr;
        private System.Windows.Forms.CheckBox chkMinConstraints;
        private System.Windows.Forms.Label lblYearSelect;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Label lblScenario;
        private System.Windows.Forms.ComboBox cboYears;
        private System.Windows.Forms.ComboBox cboScenario;
		//private System.ComponentModel.IContainer components = null;

        #endregion Instance fields
		
		#region Designer generated code
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CivEmp));
            this.lblMinConstr = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.txtMinContraint = new System.Windows.Forms.TextBox();
            this.chkControlEmpOvr = new System.Windows.Forms.CheckBox();
            this.chkEmpOvr = new System.Windows.Forms.CheckBox();
            this.chkMinConstraints = new System.Windows.Forms.CheckBox();
            this.lblYearSelect = new System.Windows.Forms.Label();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnRun = new System.Windows.Forms.Button();
            this.lblScenario = new System.Windows.Forms.Label();
            this.cboScenario = new System.Windows.Forms.ComboBox();
            this.cboYears = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // txtStatus
            // 
            this.txtStatus.Location = new System.Drawing.Point(24, 224);
            this.txtStatus.Size = new System.Drawing.Size(384, 72);
            // 
            // lblMinConstr
            // 
            this.lblMinConstr.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMinConstr.Location = new System.Drawing.Point(213, 195);
            this.lblMinConstr.Name = "lblMinConstr";
            this.lblMinConstr.Size = new System.Drawing.Size(136, 22);
            this.lblMinConstr.TabIndex = 29;
            this.lblMinConstr.Text = "Minimum Constraint";
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Garamond", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.Navy;
            this.lblTitle.Location = new System.Drawing.Point(8, 16);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(440, 32);
            this.lblTitle.TabIndex = 28;
            this.lblTitle.Text = "Civilian Employment Assignment";
            // 
            // txtMinContraint
            // 
            this.txtMinContraint.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMinContraint.Location = new System.Drawing.Point(165, 192);
            this.txtMinContraint.Name = "txtMinContraint";
            this.txtMinContraint.Size = new System.Drawing.Size(40, 22);
            this.txtMinContraint.TabIndex = 6;
            this.txtMinContraint.Text = "0";
            // 
            // chkControlEmpOvr
            // 
            this.chkControlEmpOvr.Checked = true;
            this.chkControlEmpOvr.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkControlEmpOvr.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkControlEmpOvr.Location = new System.Drawing.Point(24, 160);
            this.chkControlEmpOvr.Name = "chkControlEmpOvr";
            this.chkControlEmpOvr.Size = new System.Drawing.Size(325, 24);
            this.chkControlEmpOvr.TabIndex = 3;
            this.chkControlEmpOvr.Text = "Include Overrides in Controlling";
            // 
            // chkEmpOvr
            // 
            this.chkEmpOvr.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkEmpOvr.Location = new System.Drawing.Point(24, 128);
            this.chkEmpOvr.Name = "chkEmpOvr";
            this.chkEmpOvr.Size = new System.Drawing.Size(168, 24);
            this.chkEmpOvr.TabIndex = 2;
            this.chkEmpOvr.Text = "Use Emp Overrides";
            // 
            // chkMinConstraints
            // 
            this.chkMinConstraints.Checked = true;
            this.chkMinConstraints.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkMinConstraints.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkMinConstraints.Location = new System.Drawing.Point(24, 192);
            this.chkMinConstraints.Name = "chkMinConstraints";
            this.chkMinConstraints.Size = new System.Drawing.Size(148, 24);
            this.chkMinConstraints.TabIndex = 4;
            this.chkMinConstraints.Text = "Constrain Emp ";
            this.chkMinConstraints.CheckedChanged += new System.EventHandler(this.chkMinConstraints_CheckedChanged);
            // 
            // lblYearSelect
            // 
            this.lblYearSelect.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblYearSelect.Location = new System.Drawing.Point(24, 64);
            this.lblYearSelect.Name = "lblYearSelect";
            this.lblYearSelect.Size = new System.Drawing.Size(120, 16);
            this.lblYearSelect.TabIndex = 23;
            this.lblYearSelect.Text = "Increment";
            // 
            // btnExit
            // 
            this.btnExit.BackColor = System.Drawing.Color.Red;
            this.btnExit.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExit.Location = new System.Drawing.Point(168, 304);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(80, 40);
            this.btnExit.TabIndex = 8;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = false;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnRun
            // 
            this.btnRun.BackColor = System.Drawing.Color.LightGreen;
            this.btnRun.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRun.Location = new System.Drawing.Point(72, 304);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(80, 40);
            this.btnRun.TabIndex = 7;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = false;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // lblScenario
            // 
            this.lblScenario.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblScenario.Location = new System.Drawing.Point(176, 64);
            this.lblScenario.Name = "lblScenario";
            this.lblScenario.Size = new System.Drawing.Size(72, 16);
            this.lblScenario.TabIndex = 19;
            this.lblScenario.Text = "Scenario";
            // 
            // cboScenario
            // 
            this.cboScenario.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboScenario.Items.AddRange(new object[] {
            "0 - EP"});
            this.cboScenario.Location = new System.Drawing.Point(184, 88);
            this.cboScenario.Name = "cboScenario";
            this.cboScenario.Size = new System.Drawing.Size(96, 24);
            this.cboScenario.TabIndex = 1;
            // 
            // cboYears
            // 
            this.cboYears.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboYears.Items.AddRange(new object[] {
            "2012 - 2020",
            "2020 - 2035",
            "2035 - 2050"});
            this.cboYears.Location = new System.Drawing.Point(24, 88);
            this.cboYears.Name = "cboYears";
            this.cboYears.Size = new System.Drawing.Size(120, 24);
            this.cboYears.TabIndex = 30;
            // 
            // CivEmp
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(458, 360);
            this.Controls.Add(this.cboYears);
            this.Controls.Add(this.lblMinConstr);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.txtMinContraint);
            this.Controls.Add(this.chkControlEmpOvr);
            this.Controls.Add(this.chkEmpOvr);
            this.Controls.Add(this.chkMinConstraints);
            this.Controls.Add(this.lblYearSelect);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.lblScenario);
            this.Controls.Add(this.cboScenario);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CivEmp";
            this.Text = "UDM - Civilian Employment Assignment";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.CivEmp_Closing);
            this.Controls.SetChildIndex(this.cboScenario, 0);
            this.Controls.SetChildIndex(this.lblScenario, 0);
            this.Controls.SetChildIndex(this.btnRun, 0);
            this.Controls.SetChildIndex(this.btnExit, 0);
            this.Controls.SetChildIndex(this.lblYearSelect, 0);
            this.Controls.SetChildIndex(this.chkMinConstraints, 0);
            this.Controls.SetChildIndex(this.chkEmpOvr, 0);
            this.Controls.SetChildIndex(this.chkControlEmpOvr, 0);
            this.Controls.SetChildIndex(this.txtMinContraint, 0);
            this.Controls.SetChildIndex(this.lblTitle, 0);
            this.Controls.SetChildIndex(this.lblMinConstr, 0);
            this.Controls.SetChildIndex(this.txtStatus, 0);
            this.Controls.SetChildIndex(this.cboYears, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

    }
		#endregion


	    public CivEmp( Form form )
	    {
          InitializeComponent();
          caller = form;
        }

        #region Run button processing

        /*****************************************************************************/

        /* method btnRun_Click() */
        /// <summary>
        /// Main run button handling.
        /// </summary>

        /* Revision History
        * 
        * STR             Date       By    Description
        * --------------------------------------------------------------------------
        *                 06/17/97   tb    Initial coding
        *                 01/22/01   tb    Put in check for employment capacity to
        *                                  continue
        *                 07/22/03   df    C# revision of previous emp_main()
        * --------------------------------------------------------------------------
        */
        private void btnRun_Click( object sender, System.EventArgs e )
        {
            if( !processParams() )
                return;
            MethodInvoker mi = new MethodInvoker( beginCivEmpWork );
            mi.BeginInvoke( null, null );
        }
        // *********************************************************************

        /* method beginCivEmpWork() */
        /// <summary>
        /// Main run button handling.
        /// </summary>

        /* Revision History
        * 
        * STR             Date       By    Description
        * --------------------------------------------------------------------------
        *                 06/17/97   tb    Initial coding
        *                 01/22/01   tb    Put in check for employment capacity to
        *                                  continue
        *                 07/22/03   df    C# revision of previous emp_main()
        * --------------------------------------------------------------------------
        */
        private void beginCivEmpWork()
        {
            impedAM = new int[NUM_LUZS, NUM_LUZS];
            extractImpedAM();
            if( !UDMUtils.checkCapacity( this, 1, scenarioID, bYear ) )
            {
                MessageBox.Show( "FATAL ERROR - Employment forecast exceeds capacity","Error" );
            }  // end if
            try
            {
                openFiles();     // Open all of the input and output files.
                // Assign travel time parmeters from global defines
                // Initialize the array

                
                tt[0] = new TTP();
                tt[1] = new TTP();
                tt[2] = new TTP();

                // Assign travel time parms from global defines
                tt[0].med = AUTO_MED;
                tt[1].med = TRAN_MED;
                tt[2].med = CBD_MED;
                tt[0].asd = AUTO_STD;
                tt[1].asd = TRAN_STD;
                tt[2].asd = CBD_STD;
                tt[0].nla = AUTO_NLA;
                tt[1].nla = TRAN_NLA;
                tt[2].nla = CBD_NLA;
         
                // Load the LUZ emp and hs history array
                UDMUtils.extractHistory( this, scenarioID, bYear );
                UDMUtils.extractLUZTemp( this, scenarioID, bYear );
                UDMUtils.copyCapacity( this, 2, scenarioID,bYear,fYear );

                // Load the base data
                UDMUtils.extractLUZBase( this, zbi, scenarioID, bYear );

                UDMUtils.buildBaseRatios(this, z);

                // Load regional controls
                extractRCEmp();           

                // Load the overrides
                if( doEmpOvr )
                    extractEmpLUOvr();

                // Do the calculations
                empCalc();

                // Apply any overrides
                if (doEmpOvr)
                  applyEmpOvr();

                // Control to forecast
                if (ctrlParmE)     // If overrides are controlled
                  controlEmpWithOvr();
                else      // Otherwise
                  controlEmpNoOvr();

                // Redistribute emp subject to minimum constraints if switch is set.
                if( minSwitch )
                  minConstraintEmp();

                // Redistribute for emp over capacity
                redistributeEmp();

                // LUZ and regional totals
                doEmpTotals1();

                // Distribute emp forecast to land uses
                empLand();
      
                // Process outliers
                getEmpOutliers();
                printEmpTable4();
                printEmpTable5();
                printEmpTable6();

                // Distribute employment to transactions
                empTransactions();

                flags.empChange = true;
                printAuxEmp();
                closeEmpFiles();
                writeToStatusBox("Completed Employment Allocation");
                MessageBox.Show("Completed UDM Employment Allocation" );
          }   // end try
          catch (Exception e)
          {
                MessageBox.Show(e.ToString(),e.GetType().ToString());
          }

        }     // End method beginCivEmpWork()

        // **********************************************************************

        #endregion Run button processing


        #region Employment utilities methods

        /*****************************************************************************/

        /* method applyEmpOvr() */
        /// <summary>
        /// Method to load any employment overrides.
        /// </summary>

        /* Revision History
        * 
        * STR             Date       By    Description
        * --------------------------------------------------------------------------
        *                 05/27/97   tb    Initial coding
        *                 07/25/03   df    C# revision
        * --------------------------------------------------------------------------
        */
        private void applyEmpOvr()
        {
          int i;
          int tempsum = 0;
          // ---------------------------------------------------------------------
          for(i = 0; i < NUM_LUZS; i++ )
          {
            // Are there any overrides
            if( !z[i].eOvr )      // If not, do next i
              continue;
            
            // Otherwise check override not exceed capacity */
            if( z[i].eo.adj > z[i].capacity.totalEmp )
              z[i].eo.adj = z[i].capacity.totalEmp;
        
            // Set increment to override
            z[i].fcst.ei.adj = z[i].eo.adj;
            tempsum += z[i].eo.adj;
          }   // end for
        }     // End method applyEmpOvr()

        /*****************************************************************************/

        /* method closeEmpFiles() */
        /// <summary>
        /// Method to close all ASCII files associated with employment module.
        /// </summary>

        /* Revision History
        * 
        * STR             Date       By    Description
        * --------------------------------------------------------------------------
        *                 04/08/97   tb    Initial coding
        *                 07/16/03   df    C# revision
        * --------------------------------------------------------------------------
        */
        private void closeEmpFiles()
        {
          try
          {
            empChange.Close();
            empChange1.Close();
            empOut.Close();
            empOvr.Close();
            minConstraints.Close();
            redisEmp.Close();
            schPart.Close();
          }   // end try
          catch( IOException e )
          {
            MessageBox.Show( e.ToString(), "IO Exception" );
            Close();
          }   // end catch
        }     // End method closeEmpFiles()

        /*****************************************************************************/

        /* method controlEmpNoOvr() */
        /// <summary>
        /// Method to control the emp forecast excluding overrides.
        /// </summary>

        /* Revision History
        * 
        * STR             Date       By    Description
        * --------------------------------------------------------------------------
        *                 05/27/97   tb    Initial coding
        *                 07/25/03   df    C# revision
        * --------------------------------------------------------------------------
        */
        private void controlEmpNoOvr()
        {
          bool trans = false;     // Translation flag
          bool[] oFlag = new bool[NUM_LUZS];
          int i, diff = 0, ovrTot = 0, adjRegTot, ret;
          int[] pt = new int[NUM_LUZS];      
          int[] ubound = new int[NUM_LUZS];
          double abSum = 0, summ = 0, posAdj = 0, negAdj = 0;
          // ---------------------------------------------------------------------

          // Build accumulators
          for( i = 0; i < NUM_LUZS; i++ )
          {
            if( !z[i].eOvr )      // If not overridden
            {
              abSum += Math.Abs( z[i].fcst.ei.adj );
              summ += z[i].fcst.ei.adj;
            }   // end if
            else      // Compute the total of the overriden LUZs
            {
              ovrTot += z[i].eo.adj;

            }   // end else
          }   // end for

          // Compute regional total adjusted for overrides
          adjRegTot = rc.fcst.ei.adj - ovrTot;
          if( adjRegTot == 0 )
            return;

          if( abSum > 0 )
          {
            // Adjustment for LUZs with employment growth
            posAdj = ( abSum + ( adjRegTot - summ ) ) / abSum;
            // Adjustment for LUZs with negative growth
            negAdj = ( abSum - ( adjRegTot - summ ) ) / abSum;
          }   // end if

          /* Check negAdj for < 0 and compute translation to get it to be at least 0.10 */
          if( negAdj < 0 )
          {
            trans = true;
            diff = ( int )( 0.5 + ( ( double )( 0.9 * abSum + summ - adjRegTot ) ) / NUM_LUZS );
            abSum = 0;
            summ = 0;
            for( i = 0; i < NUM_LUZS; i++ )
            {
              if( !z[i].eOvr )
              {
                z[i].fcst.ei.adj += diff;
                abSum += Math.Abs( z[i].fcst.ei.adj );
                summ += z[i].fcst.ei.adj;
              }   // end if
            }   // end for
            // Recompute adjustments         
            if( abSum > 0 )
            {
              // Adjustment for LUZs with employment growth
              posAdj = ( abSum + ( adjRegTot - summ ) ) / abSum;
              // Adjustment for LUZs with negative growth
              negAdj = ( abSum - ( adjRegTot - summ ) ) / abSum;
            }   // end if
          }     // End if

          for( i = 0; i < NUM_LUZS; i++ )
          {
            if( z[i].fcst.ei.adj > 0 && !z[i].eOvr )
              z[i].fcst.ei.adj *= ( int )posAdj;
            else if( z[i].fcst.ei.adj < 0 && !z[i].eOvr )
              z[i].fcst.ei.adj *= ( int )negAdj;

            if( trans && !z[i].eOvr )
              z[i].fcst.ei.adj -= diff;     // Adjust for translation if any
          }   // end for i
    
          // Now use the + - roundoff to match the regional totals.
          for( i = 0; i < NUM_LUZS; i++ )
          {
            pt[i] = z[i].fcst.ei.adj;     // Pass the increment
            if( !ctrlParmE )
              oFlag[i] = z[i].eOvr;       // Pass the override flag for control
            else
              oFlag[i] = false;
            ubound[i] = z[i].capacity.totalEmp;     // Pass capacity as control
          }  // end for i

          ret = UDMUtils.roundItUpperLimit( pt, oFlag, ubound, adjRegTot, NUM_LUZS );
          if( ret > 0 )
            MessageBox.Show( "controlEmpNoOvr roundItUpperLimit didn't converge, difference = " + ret );
    
          // Restore the rounded values
          for( i = 0; i < NUM_LUZS; i++ )
            z[i].fcst.ei.adj = pt[i];

        }     // End method controlEmpNoOvr()

        /*****************************************************************************/

        /* method controlEmpWithOvr() */
        /// <summary>
        /// Method to control the emp forecast with overrides included.
        /// </summary>

        /* Revision History
        * 
        * STR             Date       By    Description
        * --------------------------------------------------------------------------
        *                 05/27/97   tb    Initial coding
        *                 07/25/03   df    C# revision
        * --------------------------------------------------------------------------
        */
        private void controlEmpWithOvr()
        {
          bool trans = false;     // Translation flag
          int diff = 0, i, ret;
          int[] pt = new int[NUM_LUZS];
          int[] ubound = new int[NUM_LUZS];

          int[] saveAdj = new int[NUM_LUZS];
          double abSum = 0, summ = 0, posAdj = 0, negAdj = 0;
          StreamWriter nc = null;
          // ---------------------------------------------------------------------

          if( EMP_CALC_DEBUG )
          {
            nc = new StreamWriter( new FileStream( networkPath + String.Format(appSettings["empControlCheck"].Value),FileMode.Create ) );
            nc.WriteLine( "EMPLOYMENT CONTROLLING CHECK" );
          }  // end if 

          abSum = 0;      // Zero accumulators

          // Build accumulators
          for( i = 0; i < NUM_LUZS; i++ )
          {
            abSum += Math.Abs( z[i].fcst.ei.adj );
            summ += z[i].fcst.ei.adj;
          }   // end for i
          
          if( abSum > 0 )
          {
            // Adjustment for LUZS with employment growth
            posAdj = ( abSum + ( rc.fcst.ei.adj - summ ) ) / abSum;
            // Adjustment for LUZS with negative growth
            negAdj = ( abSum - ( rc.fcst.ei.adj - summ ) ) / abSum;
          }   // end if

          if( EMP_CALC_DEBUG )
            nc.WriteLine( "BEFORE TRANSLATION POSADJ = {0,10:F4} NEGADJ = " + "{1,10:F4}", posAdj, negAdj );

          // Check negAdj for < 0 and compute translation to get it to be at least 0.10
          if( negAdj < 0 )
          {
            trans = true;
            diff = ( int )( 0.5 + ( ( double )( 0.9 * abSum + summ - rc.fcst.ei.adj ) ) / NUM_LUZS );
            abSum = 0;
            summ = 0;

            for( i = 0; i < NUM_LUZS; i++ )
            {
              z[i].fcst.ei.adj += diff;
              abSum += Math.Abs( z[i].fcst.ei.adj );
              summ += z[i].fcst.ei.adj;
            }   // end for i  

            // Recompute adjustments
            if( abSum > 0 )
            {
              // Adjustment for LUZS with employment growth
              posAdj = ( abSum + ( rc.fcst.ei.adj - summ ) ) / abSum;
              // Adjustment for LUZS with negative growth
              negAdj = ( abSum - ( rc.fcst.ei.adj - summ ) ) / abSum;
            }   // end if
          }   // end if

          if( EMP_CALC_DEBUG )
          {
            nc.WriteLine( "AFTER TRANSLATION POSADJ = {0,10:F4} NEGADJ = " + "{1,10:F4}", posAdj, negAdj );
            nc.WriteLine();
            nc.WriteLine();
            nc.WriteLine( "LUZ DATA BEFORE AND AFTER ADJUSTMENT and TRANSLATION");
          }   // end if

          for( i = 0; i < NUM_LUZS; i++ )
          {
            // Store the old adj value for later comparison
            saveAdj[i] = z[i].fcst.ei.adj;
            if( z[i].fcst.ei.adj > 0 )
              z[i].fcst.ei.adj = ( int )( ( double )z[i].fcst.ei.adj * posAdj );
            else if( z[i].fcst.ei.adj < 0 )
              z[i].fcst.ei.adj = ( int )( ( double )z[i].fcst.ei.adj * negAdj );

            if( trans )
              z[i].fcst.ei.adj -= diff;     // Adjust for translation if any
            if( EMP_CALC_DEBUG )
            {
              nc.WriteLine( "{0,4}{1,10}{2,10}", i + 1, saveAdj[i],z[i].fcst.ei.adj );
              nc.Flush();
            }   // end if
          }   // end for
    
          // Now use the + - roundoff to match the regional totals
          for( i = 0; i < NUM_LUZS; i++ )
          {
            pt[i] = z[i].fcst.ei.adj;     /* Save the increment in array for passing */
            ubound[i] = z[i].capacity.totalEmp;     // Pass capacity as control
          }   // end for

          ret = UDMUtils.roundIt( pt, ubound, rc.fcst.ei.adj, NUM_LUZS, 0 );
          if( ret > 0 )
          {
            txtStatus.Text += "controlEmpWithOvr roundit didn't converge, difference = " + ret + Environment.NewLine;  
            txtStatus.Select( txtStatus.Text.Length - 1, 0 );
            txtStatus.ScrollToCaret();
            Refresh();
          }   // end if

          if( EMP_CALC_DEBUG )
            nc.WriteLine( "AFTER ADJUSTED AND ROUNDED" );

              // Restore the rounded values
          for( i = 0; i < NUM_LUZS; i++ )
          {
            z[i].fcst.ei.adj = pt[i];
            saveAdj[i] = z[i].fcst.ei.adj;
            if( EMP_CALC_DEBUG )
            {
              nc.WriteLine( "{0,4}{1,10}", i + 1, z[i].fcst.ei.adj );
              nc.Flush();
            }   // end if
          }   // end for i
          if( EMP_CALC_DEBUG )
              nc.Close();
          }     // End procedure controlEmpWithOvr()

        /*****************************************************************************/

        /* method doEmpTotals1() */
        /// <summary>
        /// Method to perform LUZ totaling after redistribution and building regional
        /// totals.
        /// </summary>

        /* Revision History
        * 
        * STR             Date       By    Description
        * --------------------------------------------------------------------------
        *                 05/29/97   tb    Initial coding
        *                 07/25/03   df    C# revision
        * --------------------------------------------------------------------------
        */
        private void doEmpTotals1()
        {
          int i;

          reg.gain = 0;     // Initialize the regional gain and loss totals.
          reg.loss = 0;
      
          for(i = 0; i < NUM_LUZS; i++ )
          {
            // Add site spec to increments for civ and mil
            z[i].fcst.ei.civ = z[i].fcst.ei.adj + z[i].site.civ;
            z[i].fcst.ei.total = z[i].fcst.ei.civ;

            // Build total civ emp forecast (levels)
            z[i].fcst.e.civ = z[i].fcst.ei.civ + z[i].baseData.e.civ;
            z[i].fcst.e.adj = z[i].fcst.ei.adj + z[i].baseData.e.adj;
            z[i].fcst.e.total = z[i].fcst.ei.total + z[i].baseData.e.total;
            reg.fcst.e.civ += z[i].fcst.e.civ;
            reg.fcst.e.adj += z[i].fcst.e.adj;
            reg.fcst.e.total += z[i].fcst.e.total;

            // % change
            if( z[i].baseData.e.civ != 0 )
              z[i].fcst.pct.civ = ( double )z[i].fcst.ei.civ / ( double )z[i].baseData.e.civ * 100;
        
            if( z[i].baseData.e.total != 0 )
              z[i].fcst.pct.total = ( double )z[i].fcst.ei.total / ( double )z[i].baseData.e.total * 100;
        
            if( z[i].fcst.ei.adj >= 0 )
            {
              reg.gain += z[i].fcst.ei.adj;
              z[i].gain += z[i].fcst.ei.adj;
            }   // end if
            else
            {
              reg.loss += z[i].fcst.ei.adj;
              z[i].loss += z[i].fcst.ei.adj;
            }   // end else
          }     // End for i

              // Notify if net change != control increment
          if( reg.gain + reg.loss != rc.fcst.ei.adj )
          {
            MessageBox.Show( "WARNING ! ! ! Regional net gain and loss not equal to civ control increment!" +
                "  GAIN = " + reg.gain + "  LOSS = " + reg.loss + "  CONTROL = " + rc.fcst.ei.adj);
          }
        }     // End method doEmpTotals1()

        /*****************************************************************************/

        /* method empCalc() */
        /// <summary>
        /// Method to compute employment shares.
        /// </summary>

        /* Revision History
        * 
        * STR             Date       By    Description
        * --------------------------------------------------------------------------
        *                 05/27/97   tb    Initial coding
        *                 07/25/03   df    C# revision
        * --------------------------------------------------------------------------
        */
        private void empCalc()
        {
          StreamWriter etOut;
          int i, j;      
          double dl, dd, summ, temp1, temp2;
          double[] zat = new double[NUM_LUZS];
          double[] ai = new double[NUM_LUZS];
          int[] savEadj = new int[NUM_LUZS];

          string ehead1 = " LUZ   cap_emp     temp1    lagged     temp2     zat";

          writeToStatusBox( "Employment Forecast Allocation" );
      
          // Open a temp ASCII file for empcalc output
          etOut= new StreamWriter( new FileStream( networkPath + String.Format(appSettings["empCalcOut"].Value),FileMode.Create ) );
          
          if( EMP_CALC_DEBUG )
          {
            etOut.WriteLine( "ECALC COMPUTATION 1 ZAT" );
            etOut.WriteLine();
            etOut.WriteLine( ehead1 );
          }   // end if
        
          for( i = 0; i < NUM_LUZS; i++ )
          {
            // Capacity attractor
            if( z[i].capacity.totalEmp > 0 )
              temp1 = ( double )Math.Log( ( double )z[i].capacity.totalEmp );
            else
              temp1 = 0;

            if( z[i].baseData.e.civ > 0 )
              temp2 = ( double )Math.Log( ( double )z[i].baseData.e.civ );      
            else
              temp2 = 0;

            zat[i] = ( CAP_EMP ) * temp1 + ( LAGGED_EMP  ) *  temp2;
      
            if( EMP_CALC_DEBUG )
            {
              etOut.WriteLine( "{0,4}{1,10:F4}{2,10:F4}{3,10:F4}{4,10:F4}{5,10:F4}", i + 1, CAP_EMP, temp1, LAGGED_EMP, temp2, zat[i] );
                etOut.Flush();
            }   // end if
          }     // End for i
        
          if( EMP_CALC_DEBUG )
          {
            etOut.WriteLine( "ECALC COMPUTATION 2 ZAT" );
            etOut.WriteLine();
          }   // end if
        
          for( i = 0; i < NUM_LUZS; i++ )
          {
            summ = 0;
            for( j = 0; j < NUM_LUZS; j++ )
            {
              // Impedence matrix parameters
              dd = ( double )impedAM[i,j] / 10.0;
              dl = Math.Log( dd );

              ai[j] = ALPHA * dl + BETA * dd + zat[j];
              summ += ai[j];
        
              if( EMP_CALC_DEBUG && i == 0 )
              {
                etOut.WriteLine( "j = {0,4} dd = {1,10:F4} dl = {2,10:F4} alpha = {3,10:F4} beta = {4,10:F4} zat = {5,10:F4} ai = {6,10:F4}" +
                              "summ = {7,10:F4}", j, dd, dl, ALPHA, BETA, zat[j], ai[j], summ );
                etOut.Flush();
              }   // end if
            }     // End for j

            for( j = 0; j < NUM_LUZS; j++ )
            {
              if( summ > 0 )
                z[j].fcst.share += ai[j] / summ * z[j].fcst.hhR;
            }   // end for j
          
          }     // End for i

          for( i = 0; i < NUM_LUZS; i++ )
          {
            z[i].fcst.share = LAMBDA * z[i].fcst.share + ( 1 - LAMBDA ) * z[i].fcst.civR;
            if( EMP_CALC_DEBUG )
              etOut.WriteLine( "LUZ = {0,4} raw share = {1,10:F5}", i + 1, z[i].fcst.share );
          }   // end for i

          // Do the k-Factor normalization
          summ = 0;
        
          // Apply the k-Factors
          for( i = 0; i < NUM_LUZS; i++ )
          {
            z[i].fcst.ashare = z[i].fcst.share;
            if( z[i].fcst.ashare < 0 )
              z[i].fcst.ashare = z[i].baseRatio;
            summ += z[i].fcst.ashare;
            if( EMP_CALC_DEBUG )
              etOut.WriteLine( "i = {0,3} raw share = {1,10:F5}ashare = {3,10:F5}", i + 1,z[i].fcst.share, z[i].fcst.ashare );
          }   // end for i

          // Normalize the ashares
          for( i = 0; i < NUM_LUZS; i++ )
          {
            if( summ > 0 )
              z[i].fcst.ashare = z[i].fcst.ashare / summ;

            // Multiply proportions by zonal controls
            z[i].fcst.e.adj = ( int )( z[i].fcst.ashare * rc.fcst.e.adj );
            if( EMP_CALC_DEBUG )
              etOut.WriteLine( "i = {0,3} norm ashare = {1,10:F5} adj = {2,6}", ( i + 1 ), z[i].fcst.ashare, z[i].fcst.e.adj );
          }   // end for i
        
          // Convert to increments
          for( i = 0; i < NUM_LUZS; i++ )
          {
              savEadj[i] = new int();
            z[i].fcst.ei.adj = z[i].fcst.e.adj - z[i].baseData.e.adj;
            savEadj[i] = z[i].fcst.ei.adj;
            if( EMP_CALC_DEBUG )
              etOut.WriteLine( "i = {0,3} eadj = {1,6} base = {2,6} eiadj = {3,6} cap = {4,6}", i + 1, z[i].fcst.e.adj, z[i].baseData.e.adj, z[i].fcst.ei.adj, z[i].capacity.totalEmp );
          }   // end for i    
          etOut.Close();
        }     // End method empCalc()

        /*****************************************************************************/

        /* method empDecrements() */
        /// <summary>
        /// Method to distribute LUZ employment decrements to transcations.
        /// </summary>

        /* Revision History
        * 
        * STR             Date       By    Description
        * --------------------------------------------------------------------------
        *                 06/16/97   tb    Initial coding
        *                 07/30/03   df    C# revision
         *                10/14/09   tb    changed qualifying records for consideration - eliminated any constraints on devcode
         *                                 for decrements we'll use any records that have existing emp
        * --------------------------------------------------------------------------
        */
        private void empDecrements( int[] list, int counter )
        {
        
          SqlDataReader rdr;
          int i, luzControl, temp, k, zid, lz, nt, kk;
          int luz, mgra, LCKey, lu, devCode, eluu, civ, capCiv;
          int[] nTCap = new int[NUM_LUZS];
          double weight;
      
          empCd = new StreamWriter( new FileStream( networkPath + String.Format(appSettings["empCd"].Value), FileMode.Create ) );
          sqlCommand.CommandText = String.Format(appSettings["select04"].Value, TN.capacity2, TN.accessWeights, fYear, scenarioID, bYear);
          
          try
          { 
		    sqlConnection.Open();
            rdr = sqlCommand.ExecuteReader();
            while( rdr.Read() )
            {
              luz = rdr.GetInt16( 0 );
          
              mgra = rdr.GetInt16( 1 );
              weight = rdr.GetDouble( 2 );
              LCKey = rdr.GetInt32( 3 );
              civ = rdr.GetInt32( 4 );
              devCode = rdr.GetByte( 5 );
              lu = rdr.GetInt16( 6 );
              capCiv = rdr.GetInt32( 7 );
              zid = 999;      // Set the default for not found

              // Is the luz from query response in this list
              for( k = 0; k < counter; k++ )
              {
                if( list[k] == luz )
                {
                  zid = k;      // Save the list position as index
                  break;
                }   // end if
              }   // end for k

              if( zid != 999 )      // Make sure the index is legitimate
              {
                nt = nTCap[zid];      // Store index position
            
                // Save developed only if land use is correct
                eluu = 1;     /* set eluu for default */
                if( devCode == 1 )
                  eluu = getUDMEmpLU( lu );

                if( eluu > 0 )      /* This gets done for devcode = 1 and correct land use or devcode = 4 */
                {
                  tcapd[zid,nt] = new TCapD();
                  tcapd[zid,nt].done = false;         // Initialize the used flag
                  tcapd[zid,nt].mgra = mgra;          // MGRA ID
                  tcapd[zid,nt].LCKey = LCKey;            // LCKey
                  tcapd[zid,nt].civ = civ;            // Base year emp
                  tcapd[zid,nt].capCiv = capCiv;      // Civ emp cap
                  tcapd[zid, nt].devCode = devCode;   // devcode
                  nTCap[zid]++;

                  // Check for max array size
                  //if( nTCap[zid] == NUM_LUZS )
                  //{
                    //MessageBox.Show( "FATAL ERROR!!! - emp_dec tcap array index exceeds array bound of NUM_LUZS", "EmpDecrement Error" );
                    //Close();
                  //}   // end if
                }  // end if eluu
              }     // End if zid
            }     // End while
            rdr.Close();
          }     // End try

          catch( SqlException s )
          {
            MessageBox.Show( s.Message, "SQL Error" );
            Close();
          }
          finally
          {
            sqlConnection.Close();
          }

          for( i = 0; i < counter; i++ )
          {
       
            zid = i;
            lz = list[i] - 1;     // Get the LUZ ID from the list 
        
            if (lz == 110)
              lz = 110;
            k = 0;
            luzControl = -z[lz].fcst.ei.adj;      /* Set the luzControl value to the adjusted employment */
            // While the luzControl > 0 and the list is unfinished
            while( k < nTCap[i] && luzControl > 0 )
            {
              // Get the minimum of base year emp and luz_control
              temp = Math.Min( luzControl, tcapd[zid,k].civ );
              tcapd[zid,k].civ -= temp;         // Adjust base year emp
              tcapd[zid,k].chgCiv = -temp;      // Adjust the increment holder
              tcapd[zid,k].capCiv += temp;      // Adjust the capacity
              luzControl -= temp;                // Adjust the loop luzControl
              tcapd[zid,k].done = true;          // Mark this entry as adjusted
              if (tcapd[zid,k].devCode == 1)     //reset any developed emp land to emp infill
                tcapd[zid,k].devCode = 4;          // All   of these become redev
              tcapd[zid,k].pCap_hs = 0;             // Reset emp and hs pCap to 0
              tcapd[zid,k].pCap_emp = 0;
              k++;                               // Increment the loop list counter
            }   // end while

            for( kk = 1; kk < NUM_EMP_LAND; kk++ )
            {
              // Add site spec to acreage used
              z[lz].useCap.ac.ae[kk] += z[lz].site.ac.ae[kk];
                  // Compute remaining acreage
              z[lz].remCap.ac.ae[kk] = z[lz].capacity.ac.ae[kk] - z[lz].useCap.ac.ae[kk];
              z[lz].useCap.ac.totalEmpAcres += z[lz].useCap.ac.ae[kk];
              z[lz].remCap.ac.totalEmpAcres += z[lz].remCap.ac.ae[kk];
              reg.useCap.ac.ae[kk] += z[lz].useCap.ac.ae[kk];     /* Used acreage by category */

              // Remaining acreage by category
              reg.remCap.ac.ae[kk] += z[lz].remCap.ac.ae[kk];
              // Total used
              reg.useCap.ac.totalEmpAcres += z[lz].useCap.ac.ae[kk];
              // Total remaining
              reg.remCap.ac.totalEmpAcres += z[lz].remCap.ac.ae[kk];
            }   // end for kk

            if( luzControl > 0 )      // Is there employment left to allocate
              MessageBox.Show( "EXCEPTION -- Cannot allocate LUZ decrement to transactions, LUZ " + ( lz + 1 ) + " CONTROL = " + luzControl );
            // Update the capacity records with new data
            for( k = 0; k < nTCap[i]; k++ )
            {
              if( tcapd[zid,k].done )
                writeEmpDecrements( tcapd[zid,k]);      /* Write the update data to ASCII for bulk load */
            }   // end for k
          }     // End for i

          // Close the output file
          try
          {
            empCd.Close();
          }
          catch( IOException io )
          {
            MessageBox.Show( io.Message );
            Close();
          }
        
          loadEmpDecrements();        /* Bulk load the ASCII data to the emp_dec table */
          updateEmpDecrements();
        }     // End method empDecrements()

        /*****************************************************************************/

        /* method empIncrements() */
        /// <summary>
        /// Method to distribute LUZ employment increments to transactions.
        /// </summary>

        /* Revision History
        * 
        * STR             Date       By    Description
        * --------------------------------------------------------------------------
        *                 06/16/97   tb    Initial coding
        *                 03/13/98   jt    Changed pcap comp for dev_code = 4 (set 
        *                                  to 0)
        *                 06/22/99   tb    Fixed error in pcap calculation - base 
        *                                  year pcap was not included in query to 
        *                                  fill tcapi arrays
        *                 07/31/03   df    C# revision
        * --------------------------------------------------------------------------
        */
        private void empIncrements( int[] list, int counter )
        {
        
          SqlDataReader rdr;

          int i, j, k, kk, zid, nt;
          int luz, mgra, LCKey, lu, plu, devCode, eluu, civ, capCiv, sf, mf, 
              mh, csf, cmf, cmh;
          int msf, mmf, mmh, temp, lz;
          int tempSF, tempMF, tempmh;
          int masterControl;
          int[] control = new int[NUM_EMP_LAND];      // Land use control totals
          int[] nTCap = new int[NUM_LUZS];

          double acres, capRatio, pCap, temp1, weight;
          double capDiff;
          //-------------------------------------------------------------------------------
          empCi = new StreamWriter( new FileStream( networkPath + String.Format(appSettings["empCi"].Value),FileMode.Create ) );
          sqlCommand.CommandText = String.Format(appSettings["select05"].Value, TN.capacity2, TN.accessWeights, fYear, scenarioID, bYear);
         
	        try
	        {
		        sqlConnection.Open();
		        rdr = sqlCommand.ExecuteReader();
		        while( rdr.Read() )
		        {
			        luz = rdr.GetInt16( 0 );
    	            
			        mgra = rdr.GetInt16( 1 );
    	            
			        weight = rdr.GetDouble( 2 );
			        LCKey = rdr.GetInt32( 3 );
			        lu = rdr.GetInt16( 4 );
			        plu = rdr.GetInt16( 5 );
			        devCode = rdr.GetByte( 6 );
			        acres = rdr.GetDouble( 7 );
			        eluu = rdr.GetByte( 8 );
			        civ = rdr.GetInt32( 9 );
			        capCiv = rdr.GetInt32( 10 );
			        sf = rdr.GetInt32( 11 );
			        mf = rdr.GetInt32( 12 );
			        mh = rdr.GetInt32( 13 );
			        csf = rdr.GetInt32( 14 );
			        cmf = rdr.GetInt32( 15 );
			        cmh = rdr.GetInt32( 16 );
			        pCap = rdr.GetDouble( 17 );

			        zid = 999;      // Set the default for not found
			        // Is the LUZ in the query in this list
			        for( k = 0; k < counter; k++ )
			        {
				        if( list[k] == luz )
				        {
					        zid = k;      // Save the list position as index.
					        break;
				        }   // end if
			        }   // end for k

			        // Build the list of allocated LUZs.
			        if( zid != 999 )      // Do this only for good indexes
			        {
				        nt = nTCap[zid];              // Get the second dimension index
				        tCapI[zid,nt] = new TCap();
                        tCapI[zid,nt].luz = luz;
				        tCapI[zid,nt].mgra = mgra;
				        tCapI[zid,nt].LCKey = LCKey;
				        tCapI[zid,nt].lu = lu;
				        tCapI[zid,nt].plu = plu;
				        tCapI[zid,nt].devCode = devCode;
				        tCapI[zid,nt].acres = acres;
				        tCapI[zid,nt].udmEmpLU = eluu;
				        tCapI[zid,nt].civ = civ;
				        tCapI[zid,nt].capCiv = capCiv;
				        tCapI[zid,nt].sf = sf;
				        tCapI[zid,nt].mf = mf;
				        tCapI[zid,nt].mh = mh;
				        tCapI[zid,nt].capSF = csf;
				        tCapI[zid,nt].capMF = cmf;
				        tCapI[zid,nt].capmh = cmh;
				        tCapI[zid,nt].done = false;
				        tCapI[zid,nt].pCap_emp = pCap;
				        nTCap[zid]++;     // Increment the second dimension index

				        // Check for max array size, MAXTRANS is somewhat arbitrary
				        if( nTCap[zid] == MAX_TRANS )
				        {
					        MessageBox.Show( "FATAL ERROR!!! - In empIncrement tcapi array index exceeds array bound = " + MAX_TRANS, "EmpIncrement Error" );
				        }   // end if
			        }     // End if zid
		        }     // End while
		        rdr.Close();
	        }   // end try
	        catch(Exception e)
	        {
		        MessageBox.Show(e.ToString(),e.GetType().ToString());
    		
	        }  // end catch
            finally
            {
            sqlConnection.Close();
            }

          // Cycle though the transactions
          for( i = 0; i < counter; i++ )
          {
            zid = i;
            lz = list[i] - 1;           // Get LUZ ID
            if (lz == 71)
                lz = 71;
            for( j = 1; j < NUM_EMP_LAND; j++ )
            {
              control[j] = z[lz].fcst.ei.empLand[j];      /* Store empLand distribution as controls */          
              // These are the incremental emps allocated to land use.

              z[lz].useCap.ac.ae[j] = 0;      /* Initialize the dev acres used totals. */
              z[lz].remCap.ac.ae[j] = 0;      /* Initialize the dev acres remaining totals. */
            }   // end for j
            masterControl = 0;

            // Build master control with control values.  Master control will be used as loop control as long as it is > 0 meaning
            // there is still capacity to fill.

            // Build the loop control from the land use totals.
            for( j = 1; j < NUM_EMP_LAND; j++ )
              masterControl += control[j];

            k = 0;      // Start the counter

            // Allocation control loop
            while( k < nTCap[i] && masterControl > 0 )   
            {
              tCapI[zid,k].done = true;       /* mark as used */
              tCapI[zid,k].oldCapCiv = tCapI[zid,k].capCiv;     /* Save the original capacities. */
              tCapI[zid,k].oldCapSF = tCapI[zid,k].capSF;
              tCapI[zid,k].oldCapMF = tCapI[zid,k].capMF;
              tCapI[zid,k].oldCapmh = tCapI[zid,k].capmh;

              // Get UDM land array index 1 - 6
              kk = tCapI[zid,k].udmEmpLU;

              /* Get the minimum of emp capacity and control(empLand) for this land use. */
              temp = Math.Min( control[kk], tCapI[zid,k].capCiv );

              // Allocate to this land use if temp > 0
              if( temp > 0 )
              {
                tCapI[zid,k].civ += temp;         // Adjust base year employment
                tCapI[zid,k].chgCiv = temp;       // Store increment
                tCapI[zid,k].capCiv -= temp;      // Adjust capacity
                control[kk] -= temp;              // Adjust this control value

                /* Things to do if the transaction capacity has been reduced to zero */
                if( tCapI[zid,k].capCiv == 0 )
                {
                  tCapI[zid,k].pCap_emp = 1.0;                // Set the pcap to 100%
                  tCapI[zid,k].lu = tCapI[zid,k].plu;     /* Reset the base land use to the planned. */
                  if (tCapI[zid,k].plu != 9700)
                    tCapI[zid,k].devCode = 1;               // Mark as developed only if it's not mixed use
                  // Keep track of the acres
                  z[lz].useCap.ac.ae[kk] += tCapI[zid,k].acres;
                }   // end if
          
                // This is a partialy developed record - compute some residuals
                else
                {
                  tCapI[zid,k].oldpCap_emp = tCapI[zid,k].pCap_emp;     /* Save the original pCap */
                  // Estimate original (2004) capacity

                  if( tCapI[zid,k].pCap_emp != 1 )
                    temp1 = ( double )tCapI[zid,k].oldCapCiv / ( 1 - tCapI[zid,k].pCap_emp );
                  else
                    temp1 = 0;
                  // Recompute pCap
                  // New computation for devCode 4 pCap added per JT 03/13/98
                  if( tCapI[zid,k].devCode == 4 )
                    tCapI[zid,k].pCap_emp = 0;
                  else 
                    if (temp1 != 0)
                      tCapI[zid,k].pCap_emp = 1 - ( ( double )tCapI[zid,k].capCiv / temp1 );
                  // Allocate acreage
                  z[lz].useCap.ac.ae[kk] += tCapI[zid,k].acres * ( tCapI[zid,k].pCap_emp - tCapI[zid,k].oldpCap_emp );
                  /* Warning if this is a partially developed school site.  We may want to avoid these with overrides (tuning). */
                  if( kk == 6 )
                  {
                    flags.schPart = true;      /* Mark the partial school records exists flag */
                    // Write some data to ASCII
                    schPart.WriteLine( "LUZ {0,4} LCKey {1,6} PCAP = {2,7:F2} EMP = {3,6} OLDCAP = {4,6}",
                            lz + 1, tCapI[zid,k].LCKey, tCapI[zid,k].pCap_emp,temp, tCapI[zid,k].oldCapCiv );
                    schPart.Flush();
                  }   // end if
                }     // End else

                /* If this is a residential to emp redev - there are some units to address */
                if( kk == 1 )     // This is a res to emp redev code
                {
                  //  Capacity difference
                  capDiff = tCapI[zid,k].pCap_emp - tCapI[zid,k].oldpCap_emp;
                  capRatio = 1 - tCapI[zid,k].oldpCap_emp;      // Ratio
                  msf = ( int )( 0.5 + ( double )Math.Abs( tCapI[zid,k].capSF ) / capRatio * ( double )capDiff );
                  tempSF = Math.Min( msf, tCapI[zid,k].sf );
                  mmf = ( int )( 0.5 + ( double )Math.Abs( tCapI[zid,k].capMF ) / capRatio * ( double )capDiff );
                  tempMF = Math.Min( mmf, tCapI[zid,k].mf );
                  mmh = ( int )( 0.5 + ( double )Math.Abs( tCapI[zid,k].capmh ) / capRatio * ( double )capDiff );
                  tempmh = Math.Min( mmh, tCapI[zid,k].mh );

                  tCapI[zid,k].sf -= tempSF;      // Adjust base year sf
                  tCapI[zid,k].mf -= tempMF;      // Adjust base year mf
                  tCapI[zid,k].mh -= tempmh;      // Adjust base year mh

                  tCapI[zid,k].chgSF = -tempSF;     // Set value for sf increment
                  tCapI[zid,k].chgMF = -tempMF;     // Set value for mf increment
                  tCapI[zid,k].chgmh = -tempmh;     // Set value for mh increment

                  tCapI[zid,k].capSF += tempSF;     // Add to capacity sf
                  tCapI[zid,k].capMF += tempMF;     // Add to capacity mf
                  tCapI[zid,k].capmh += tempmh;     // Add to capacity mh
                }   // end if

                // Rebuild master control for checking loop
                masterControl = 0;
                for( j = 1; j < NUM_EMP_LAND; j++ )
                  masterControl += control[j];
              }     // End if temp > 0
              k++;
            }     // End while

            for( kk = 1; kk < NUM_EMP_LAND; kk++ )
            {
              z[lz].useCap.ac.ae[kk] += z[lz].site.ac.ae[kk];     /* Add site spec to acreage used */
              // Compute remaining acreage
              z[lz].remCap.ac.ae[kk] = z[lz].capacity.ac.ae[kk] - z[lz].useCap.ac.ae[kk];
              z[lz].useCap.ac.totalEmpAcres += z[lz].useCap.ac.ae[kk];
              z[lz].remCap.ac.totalEmpAcres += z[lz].remCap.ac.ae[kk];
              reg.useCap.ac.ae[kk] += z[lz].useCap.ac.ae[kk];     /* Used acreage by category */
              reg.remCap.ac.ae[kk] += z[lz].remCap.ac.ae[kk];     /* Remaining acreage by category */
              // Total used
              reg.useCap.ac.totalEmpAcres += z[lz].useCap.ac.ae[kk];
              // Total remaining
              reg.remCap.ac.totalEmpAcres += z[lz].remCap.ac.ae[kk];
            }   // end for kk

            if( masterControl > 0 )     // Is there capacity remaining
            {
              MessageBox.Show( "EXCEPTION -- Cannot allocate LUZ increment to transactions, LUZ "  + ( lz + 1 ) );
            
            }   // end if

            // Update the capacity records with new data
            for( k = 0; k < nTCap[i]; k++ )
            {
              if( tCapI[zid,k].done )
                writeEmpIncrements( tCapI[zid,k] );     /* Write the update date to ASCII for bulk loading */
            }   // end for k
          }     // End for i

          try
          {
            empCi.Close();
          }
          catch( IOException io )
          {
            MessageBox.Show( io.Message );
            Close();
          }
        
          loadEmpIncrements();       // Bulk load the ASCII empIncrement file
          updateEmpIncrements();     // Update capacity from the empInc table
        }     // End method empIncrement()

        /*****************************************************************************/

        /* method empLand() */
        /// <summary>
        /// Method to distribute the LUZ emp forecast to the six land use types.
        /// </summary>

        /* Revision History
        * 
        * STR             Date       By    Description
        * --------------------------------------------------------------------------
        *                 05/29/97   tb    Initial coding
        *                 07/25/03   df    C# revision
        * --------------------------------------------------------------------------
        */
        private void empLand()
        {
          int zt, zdiff, adj, loopCount,i,j;
          bool eluOvrOk = false;      // Are land use overrides valid
          bool[] eluOverrides = new bool[7];
          double ratio;
          
          writeToStatusBox( "Distributing employment to land use categories.." );
          // Distribute employent increment to emp land uses   
          // Emp land use arrays start at 1, 0 is a dummy      
          for(i = 0; i < NUM_LUZS; i++ )
          {
            writeToStatusBox( "   processing LUZ " + ( i + 1 ) );        
            // This gets done only for positive increments 
            if( z[i].fcst.ei.adj > 0 )
            {
              zt = 0;
              if( z[i].elOvr )
              {
                eluOvrOk = true;      // Start with a good land use override flag
              
                /* Check override flags and replace with overrides if overrides <= capacity. */
                for(j = 1; j < NUM_EMP_LAND; j++ )
                {
                  if( z[i].eo.elu[j] <= z[i].capacity.e[j] )
                  {
                    z[i].fcst.ei.empLand[j] = z[i].eo.elu[j];
                    zt += z[i].eo.elu[j];     // Keep a running sum
                    eluOverrides[j] = true;
                  }   // end if
                }   // end for j
            
                // Rebuild eluOvr as AND
                for(j = 1; j < NUM_EMP_LAND; j++ )
                eluOvrOk &= eluOverrides[j];

                if( eluOvrOk && ( zt != z[i].fcst.ei.adj ) )
                {
                  writeToStatusBox( "Land use overrides sum " + zt + " != INCREMENT " + z[i].eo.adj + " LUZ " + ( i + 1 ) );
                  eluOvrOk = false;
                }   // end if
                
                // If the elu_ovr_ok flag is not good - report

                else if( !eluOvrOk )
                {
                  writeToStatusBox( "Land use overrides exceed capacity for LUZ" + ( i + 1 ) );
                  for(j = 1; j < NUM_EMP_LAND; j++ )
                  {
                    writeToStatusBox( "   j = " + j + " capacity = " + z[i].capacity.e[j] + "override = " + z[i].eo.elu[j] );
                  }   // end for j
                }   // end else if
              }     // End if

              // If no overrides or overides are bad, use the ratio method.
              if( !z[i].elOvr || !eluOvrOk )
              {
                  zt = 0;
                  // Allocate to emp and keep sum of computed distributions
                  for(j = 1; j < NUM_EMP_LAND; j++ )     // 6 emp land cats
                  {
                    if( z[i].capacity.totalEmp != 0 )
                      ratio = ( double )z[i].capacity.e[j] /  ( double )z[i].capacity.totalEmp;
                    else
                      ratio = 0;
                    z[i].fcst.ei.empLand[j] = ( int )( ( double )z[i].fcst.ei.adj * ratio );
                    zt += z[i].fcst.ei.empLand[j];
                  }   // end for j
              }   // end if
                      
              zdiff = z[i].fcst.ei.adj - zt;      // Get difference due to rounding
              if( zdiff > 0 )
                adj = 1;
              else 
                adj = -1;
          
              loopCount = 0;
              while( zdiff != 0 )     // While there is still a difference
              {
                for(j = 1; j < NUM_EMP_LAND; j++ )
                {
                  if( z[i].fcst.ei.empLand[j] + adj <= z[i].capacity.e[j] )
                  {
                    z[i].fcst.ei.empLand[j] += adj;
                    zdiff -= adj;
                  }   // end if
                  // Check is the difference gone
                  if( zdiff == 0 )
                    break;     // Get out of for loop
                }   // end for j
                if( zdiff == 0 )
                  break;      // Get out of while loop
                loopCount++;
              }   // end while
            }     // End if

            // Compute regional totals and remaining capacity for emp land cats.
            for(j = 1; j < NUM_EMP_LAND; j++ )
            {
              z[i].useCap.e[j] = z[i].fcst.ei.empLand[j];
              z[i].useCap.totalEmp += z[i].useCap.e[j];
        
              // Regional increment by land use type
              reg.fcst.ei.empLand[j] += z[i].fcst.ei.empLand[j];
              reg.useCap.e[j] += z[i].useCap.e[j];

              // LUZ remaining capacity by land use and total
              z[i].remCap.e[j] = z[i].capacity.e[j] - z[i].useCap.e[j];
              z[i].remCap.totalEmp += z[i].remCap.e[j];

              // Pct capacity used
              if( z[i].capacity.e[j] > 0 )
                z[i].useCap.pe[j] = ( double )z[i].useCap.e[j] / ( double )z[i].capacity.e[j] * 100;

              // Regional total remaining capacity by land use type *
              reg.remCap.e[j] += z[i].remCap.e[j];
            }   // end for j

            // Pct capacity used for total emp
            if( z[i].capacity.totalEmp > 0 )
              z[i].useCap.pTotalEmp = ( double )z[i].useCap.totalEmp / ( double )z[i].capacity.totalEmp * 100;

            // Regional total remaining capacity all emp land cats
            reg.remCap.totalEmp += z[i].remCap.totalEmp;
          }   // end for
          
          for(j = 1; j < NUM_EMP_LAND; j++ )
          {
            if( reg.capacity.e[j] > 0 )
              reg.useCap.pe[j] = reg.useCap.e[j] / reg.capacity.e[j] * 100;
            if( reg.capacity.totalEmp > 0)
              reg.useCap.pTotalEmp = reg.useCap.totalEmp / reg.capacity.totalEmp * 100;
          }   // end for j
        }     /* end procedure emp_land()*/

        /*****************************************************************************/

        /* method empTransactions() */
        /// <summary>
        /// Method to distribute LUZ employment to transcations - master control 
        /// routine.
        /// </summary>

        /* Revision History
        * 
        * STR             Date       By    Description
        * --------------------------------------------------------------------------
        *                 06/16/97   tb    Initial coding
        *                 07/31/03   df    C# revision
        * --------------------------------------------------------------------------
        */
        private void empTransactions()
        {
          int ki = 0, kd = 0, i;
          int[] ilist = new int[NUM_LUZS], dlist = new int[NUM_LUZS];
          int[] oldAdj = new int[NUM_LUZS];
          writeToStatusBox( "Distributing employment to transactions.." );
          for(i = 0; i < NUM_LUZS; i++ )
          {
              oldAdj[i] = new int();
              oldAdj[i] = z[i].fcst.ei.adj;
            // Build increment and decrement list of LUZs
            if( z[i].fcst.ei.adj >= 0 )
              ilist[ki++] = i + 1;
            else  
              dlist[kd++] = i + 1;
          }   // end for i

              // Call decrement and increment if applicable
          if( kd > 0 )
            empDecrements( dlist, kd );
          if( ki > 0 )
            empIncrements( ilist, ki );
        }     // End method empTransactions()

        /*****************************************************************************/

        /* method extractEmpLUOvr() */
        /// <summary>
        /// Method to extract emp overrides.
        /// </summary>

        /* Revision History
        * 
        * STR             Date       By    Description
        * --------------------------------------------------------------------------
        *                 05/21/97   tb    Initial coding
        *                 07/25/03   df    C# revision
        * --------------------------------------------------------------------------
        */
        private void extractEmpLUOvr()
        {
          int t = 0, zi,i,sc,inc;
          int[] ov = new int[9];
          int tempsum = 0;
          //-------------------------------------------------------------------------
        
          // LUZ employment override struct is eo.  
      
          /* Employment overrides
          *    0:     civilain emp increment; 
          *    1 - 7: emp increment by land use redev, infill, vac-indus, 
          *           vac-comm, vac-office, vac- schools
          */
          
          //writeToStatusBox( "Extracting LUZ employment - land use overrides.." );
          SqlDataReader rdr;
          sqlCommand.CommandText = String.Format(appSettings["selectAll"].Value, TN.luzEmpOvr, scenarioID, bYear);
          try
          {
		    sqlConnection.Open();  
            rdr = sqlCommand.ExecuteReader();

            while( rdr.Read() )
            {
              sc = rdr.GetByte(0);
              inc = rdr.GetInt16(1);
              luzB = rdr.GetInt16( 2 );

              for(i = 0; i < 7; i++ )
                ov[i] = rdr.GetInt32( i + 3 );

              zi = luzB - 1;
              z[zi].eOvr = true;      // Mark this LUZ as having overrides

              //if( ov[0] > 0 )         // If total increment > 0 set total
              z[zi].eo.adj = ov[0];
              tempsum += ov[0];
              t = 0;

              for(i = 1; i < NUM_EMP_LAND; i++ )
              {
                z[zi].eo.elu[i] = ov[i];      // Store any land use override
                t += ov[i];                   // Sum the land use overrides if any
              }   // end for i

              flags.eOvr = true;      // Mark overrides exist
            
              // If the land use override sum > 0 set flag and replace total
              if( t > 0 )
              {
                z[zi].elOvr = true;
                z[zi].eo.adj = t;
              }   // end if
            }     // End while 
            rdr.Close();
          }   // end try
          catch(Exception e)
          {
            MessageBox.Show(e.ToString(),e.GetType().ToString());

          }  // end catch
          finally
          {
            sqlConnection.Close();
          }     
        }     // End method extractEmpLUOvr()*/

        /*****************************************************************************/

        /* method extractImpedAM() */
        /// <summary>
        /// Method to extract am travel time.
        /// </summary>

        /* Revision History
        * 
        * STR             Date       By    Description
        * --------------------------------------------------------------------------
        *                 05/22/97   tb    Initial coding
        *                 07/25/03   df    C# revision
        * --------------------------------------------------------------------------
        */
        private void extractImpedAM()
        {
          int imp, z1, z2;
          SqlDataReader rdr;
          writeToStatusBox( "Extracting AM Impedence Matrix" );
          
          sqlCommand.CommandText = String.Format(appSettings["selectAll"].Value,TN.impedAM,scenarioID,bYear);
          try
          {
		    sqlConnection.Open();
            rdr = sqlCommand.ExecuteReader();
            while( rdr.Read() )
            {
              z1 = rdr.GetInt16( 2 );  // skip scenario and increment
              z2 = rdr.GetInt16( 3 );
              imp = rdr.GetInt16( 4 );
              impedAM[z1 - 1, z2 - 1] = imp;
            }   // end while
            rdr.Close();
          }   // end try
          catch(Exception e)
          {
            MessageBox.Show(e.ToString(),e.GetType().ToString());

          }  // end catch
          finally
          {
            sqlConnection.Close();
          }
        }     // End method extractImpedAM()

        /*****************************************************************************/

   
        /* method extractRCEmp() */
        /// <summary>
        /// Method to get regional controls for the employment model.
        /// </summary>

        /* Revision History
        * 
        * STR             Date       By    Description
        * --------------------------------------------------------------------------
        *                 05/21/97   tb    Initial coding
        *                 07/09/97   tb    Removed the rcn2 array and the query for 
        *                                  the rc.be data because they are not used 
        *                                  anywhere.  Also, changes this routine to 
        *                                  extract_civ_emp for the employment model 
        *                                  and eliminated the store_rc call -- the 
        *                                  emp model only uses the civ and mil data 
        *                                  from the regional controls.
        *                 07/25/03   df    C# revision
         *                10/14/09   tb    changed origin of base data.  now get it from the adjusted capacity table _2
         *                                 because we have applied negative employment capacities and the total no longer matches what is 
         *                                 in the reg_fcst table
         *                11/17/2009 tb    restored reg_fcst as source for base year data because we did away with the negative employment overrides
        * --------------------------------------------------------------------------
        */
        private void extractRCEmp()
        {
          StreamWriter rce = null;
          int i,year;
          int[] rcn = new int[42];
        
          if( RC_DEBUG )
          {
            rce = new StreamWriter( new FileStream( networkPath+String.Format(appSettings["rcEmpCheck"].Value), FileMode.Create ) );
            rce.WriteLine( "REGIONAL CONTROL INPUT CHECK" );
            rce.WriteLine();
          }

          // Employment level struct is e.
      
          SqlDataReader rdr;
          writeToStatusBox("Extracting base year employment ..");
          sqlCommand.CommandText = String.Format(appSettings["select18"].Value,TN.regfcst,scenarioID,bYear,fYear); // get base year year only from this table";
          try
          {
            sqlConnection.Open();
            rdr = sqlCommand.ExecuteReader();

            while (rdr.Read())
            {
              year = rdr.GetInt16(0);
              if (year == bYear)
              {
                  rc.baseData.e.civ = rdr.GetInt32(1);
                  rc.baseData.e.mil = rdr.GetInt32(2);
                  rc.baseData.e.total = rc.baseData.e.civ + rc.baseData.e.mil;
              }  // end if
              else
              {
                  rc.fcst.e.civ = rdr.GetInt32(1);
                  rc.fcst.e.mil = rdr.GetInt32(2);
                  rc.fcst.e.total = rc.fcst.e.civ + rc.fcst.e.mil;
              }  // end else

            }   // end while
            rdr.Close();
          }   // end try
          catch (Exception e)
          {
            MessageBox.Show(e.ToString(), e.GetType().ToString());

          }  // end catch
          finally
          {
            sqlConnection.Close();
          }

          // Build regional controls from LUZ ss data
          for(i = 0; i < NUM_LUZS; i++ )
          {
            rc.site.civ += z[i].site.civ;     // Civilian ss
            rc.capacity.totalEmp += z[i].capacity.totalEmp;     // Total emp cap
          }   // end for i

          // Some totals
          rc.fcst.e.adj = rc.fcst.e.civ - reg.site.civ;
          if( rc.fcst.e.adj < 0 )
          {
            writeToStatusBox( "Warning! - Regional Adjusted Civilian (for SS)< 0" );
            rc.fcst.e.adj = 0;
          }   // end if

          // Employent increment struct is ei. Regional control increments.
          rc.fcst.ei.total = rc.fcst.e.total - rc.baseData.e.total;
          rc.fcst.ei.civ = rc.fcst.e.civ - rc.baseData.e.civ;
          rc.fcst.ei.adj = rc.fcst.ei.civ - reg.site.civ;

          if( RC_DEBUG )
          {
            rce.WriteLine( "rc.base.e.civ = {0,8} rc.fcst.e.civ = {1,8} " + "rc.fcst.ei.civ = {2,0}", rc.baseData.e.civ, rc.fcst.e.civ, rc.fcst.ei.civ );
            rce.WriteLine( "rc.base.e.mil = {0,8} rc.fcst.e.mil = {1,8} " + "rc.fcst.ei.mil = {2,0}", rc.baseData.e.mil, rc.fcst.e.mil, rc.fcst.ei.mil );
            rce.WriteLine( "rc.base.e.total = {0,8} rc.fcst.e.total = {1,8} " + "rc.fcst.ei.total = {2,0}", rc.baseData.e.total, rc.fcst.e.total, rc.fcst.ei.total );
            rce.WriteLine( "rc.fcst.ei.civ = {0,8} reg.site.civ = {1,8} " + "rc.fcst.ei.adj = {2,8}", rc.fcst.ei.civ, reg.site.civ, rc.fcst.ei.adj );
            rce.Flush();
            rce.Close();
          }   // end if
        }     // End method extractRCEmp()

        /*****************************************************************************/

        /* method getEmpOutliers() */
        /// <summary>
        /// Method to build values to determine emp outliers.
        /// </summary>

        /* Revision History
        * 
        * STR             Date       By    Description
        * --------------------------------------------------------------------------
        *                 05/30/97   tb    Initial coding
        *                 03/03/98   tb    Removed constraint on >= 200 for outlier
        *                 07/27/03   df    C# revision
        * --------------------------------------------------------------------------
        */
        private void getEmpOutliers()
        {
          int i;
          // --------------------------------------------------------------------
          writeToStatusBox( "Building employment outliers.." );
          if( reg.histEmp.c5 > 0 )
            reg.histEmp.r5 = ( double )rc.fcst.ei.civ / ( double )reg.histEmp.c5;
          if( reg.histEmp.L5 > 0 )
            reg.histEmp.pct5 = ( double )reg.histEmp.c5 / ( double )reg.histEmp.L5 * 100;

          for (i = 0; i < NUM_LUZS; i++ )
          {
            empOut.WriteLine( "LUZ " + ( i + 1 ) );
            if( z[i].histEmp.L5 > 0 )
              z[i].histEmp.pct5 = ( double )z[i].histEmp.c5 /( double )z[i].histEmp.L5 * 100;

            // Compute LUZ change ratios
            if( z[i].histEmp.c5 > 0 )
              z[i].histEmp.r5 = ( double )z[i].fcst.ei.civ / ( double )z[i].histEmp.c5;     // 5 year ratio
        
            empOut.WriteLine( "he.r5 = " + z[i].histEmp.r5 + " ei.civ = " + z[i].fcst.ei.civ + " he.c5 = " + z[i].histEmp.c5 );
            if( reg.histEmp.r5 > 0 )
              z[i].eOut.r5 = z[i].histEmp.r5 / reg.histEmp.r5;
            else
              z[i].eOut.r5 = 0;

            empOut.WriteLine( "eout.r5 = " + z[i].eOut.r5 + " he.r5 = " + z[i].histEmp.r5 + " reg.hist_e.r5 = " + reg.histEmp.r5 );

            empOut.WriteLine( "ei.civ = " + z[i].fcst.ei.civ + " he.c5 = " + z[i].histEmp.c5 );

            if( z[i].fcst.ei.civ * z[i].histEmp.c5 < 0 )      /* This means signs are different */
              z[i].eOut.outCode = 1;
        
            else if( Math.Abs( z[i].eOut.r5 ) > 1.2 || Math.Abs( z[i].eOut.r5 ) < 0.70 )
              z[i].eOut.outCode = 2;

            // If any of the outlier codes are > 0 set the flag.
            if( z[i].eOut.outCode > 0 )
              flags.eOut = true;
          
            empOut.WriteLine( "outcode = " + z[i].eOut.outCode );

            if( z[i].eOut.outCode > 1 )
            {
              if( Math.Abs( z[i].eOut.r5 ) < 0.70 )
                  z[i].eOut.inc5 = ( int )( z[i].eOut.r5 * 0.70 *( double )z[i].histEmp.c5 );
              else if( Math.Abs( z[i].eOut.r5 ) > 1.2 )
                  z[i].eOut.inc5 = ( int )( z[i].eOut.r5 * 1.20 * ( double )z[i].histEmp.c5 );
              z[i].eOut.diff5 = z[i].fcst.ei.civ - z[i].eOut.inc5;
          
              empOut.WriteLine( "eout.inc5 = " + z[i].eOut.inc5 + " eout.r5*0.70 = " + z[i].eOut.r5 + " he.c5 = " + z[i].histEmp.c5 );
            }   // end if
            empOut.Flush();
          }     // End for i
        }     // End method getEmpOutliers()

        /*****************************************************************************/

        /* method getUDMEmpLU() */
        /// <summary>
        /// Method to assign UDM emp land use for neg emp increments.
        /// </summary>

        /* Revision History
        * 
        * STR             Date       By    Description
        * --------------------------------------------------------------------------
        *                 06/16/97   tb    Initial coding
        *                 03/09/98   jt    Added ag and comercial rec to land 
        *                                  categories for emp decrements
        *                 07/30/03   df    C# revision
        * --------------------------------------------------------------------------
        */
        private int getUDMEmpLU( int lu )
        {      
          // Industrial
          if( UDMUtils.inIndustrial( lu ) )
            return 1;

          // Commercial
          else if( UDMUtils.inCommercial( lu ) )
            return 1;

          // Office
          else if( UDMUtils.inOffice( lu ) )
            return 1;
          
          // Agricultural added per JT 03/08/98
          else if( UDMUtils.inAg( lu ) )
            return 1;
          
          // Everything else
          else
            return 0;
        }     // End method getUDMEmpLU()

        /*****************************************************************************/

        /* method loadEmpDecrements() */
        /// <summary>
        /// Method to bulk load SQL table from temporary ASCII table.
        /// </summary>

        /* Revision History
        * 
        * STR             Date       By    Description
        * --------------------------------------------------------------------------
        *                 06/24/97   tb    Initial coding
        *                 07/30/03   df    C# revision
        * --------------------------------------------------------------------------
        */
        private void loadEmpDecrements()
        {
          //writeToStatusBox( "Bulk loading employment decrements table.." );
          //writeToStatusBox( "   truncating emp_dec table.." );
          sqlCommand.CommandText = String.Format(appSettings["truncate"].Value, TN.empIncrements);
          try
          {
		    sqlConnection.Open();
            sqlCommand.ExecuteNonQuery();
		  }
          catch(Exception e)
          {
            MessageBox.Show(e.ToString(),e.GetType().ToString());
 
          }  // end catch
          finally
          {
            sqlConnection.Close();       

          }

          writeToStatusBox( "   bulk loading emp_dec table.." );
          string tempInc = String.Format(networkPath + appSettings["empCd"].Value);
          sqlCommand.CommandText = String.Format(appSettings["bulkInsert"].Value, TN.empDecrements, tempInc);
          
          try
          {
            sqlConnection.Open();
            sqlCommand.ExecuteNonQuery();

          }
          catch (Exception e)
          {
            MessageBox.Show(e.ToString(), e.GetType().ToString());

          }  // end catch
          finally
          {
            sqlConnection.Close();
          }

        }     // End method loadEmpDecrements()

        /*****************************************************************************/

        /* method loadEmpIncrements() */
        /// <summary>
        /// Method to load updated employment increments data to SQL capacity
        /// table.
        /// </summary>

        /* Revision History
        * 
        * STR             Date       By    Description
        * --------------------------------------------------------------------------
        *                 06/24/97   tb    Initial coding
        *                 07/30/03   df    C# revision
        * --------------------------------------------------------------------------
        */
        private void loadEmpIncrements()
        {
          //writeToStatusBox( "Bulk loading employments increments table.." );
          //writeToStatusBox( "   truncating emp_inc table.." );
          sqlCommand.CommandText = String.Format(appSettings["truncate"].Value,TN.empIncrements);
          try
          {
		    sqlConnection.Open();
            sqlCommand.ExecuteNonQuery();
	      }
          catch(Exception e)
          {
            MessageBox.Show(e.ToString(),e.GetType().ToString());
          }  // end catch
          finally
          {
            sqlConnection.Close();
          }

          writeToStatusBox( "   bulk loading emp_inc table.." );
          string tempInc = String.Format(networkPath + appSettings["empCi"].Value);
          sqlCommand.CommandText = String.Format(appSettings["bulkInsert"].Value, TN.empIncrements,tempInc);
          
          try
          {
		    sqlConnection.Open();
		    sqlCommand.ExecuteNonQuery();
          }
          catch(Exception e)
          {
            MessageBox.Show(e.ToString(),e.GetType().ToString());

          }  // end catch
          finally
          {
            sqlConnection.Close();
          }
        }     // End method loadEmpIncrements()

        /*****************************************************************************/

        /* method minConstraintEmp() */
        /// <summary>
        /// Method to redistribute LUZ employment for minimum constraints.
        /// </summary>

        /* Revision History
        * 
        * STR             Date       By    Description
        * --------------------------------------------------------------------------
        *                 04/10/97   tb    Initial coding
        *                 07/25/03   df    C# revision
        * --------------------------------------------------------------------------
        */
        private void minConstraintEmp()
        {
          int i, j, k, shortage,loopCount;
          int ot, nt, lt, gt, st, rn, rp, adjustment, imp1, dest;

          int[] iAM = new int[NUM_LUZS], reals = new int[NUM_LUZS], 
          realt = new int[NUM_LUZS], oldAdj = new int[NUM_LUZS];
          int [] minflags = new Int32[NUM_LUZS];
          SqlDataReader rdr;
          StreamWriter mc = null;
          // ---------------------------------------------------------------------
          lt = gt = 0;
          if( EMP_CALC_DEBUG )
          {
            mc = new StreamWriter( new FileStream( networkPath + String.Format(appSettings["minConstraintsCheck"].Value),FileMode.Create ) );
            mc.WriteLine( "MINIMUM CONSTRAINTS REDISTRIBUTION CHECK" );

            // Save the old adj for comparison.
            
            for( i = 0; i < NUM_LUZS; ++i)
            {
              oldAdj[i] = z[i].fcst.ei.adj;
              if( z[i].fcst.ei.adj < 0 )
                lt += z[i].fcst.ei.adj;
              else
                gt += z[i].fcst.ei.adj;
            }   // end for i
          }   // end if

          writeToStatusBox( "Redistributing employment for minimum constraints.." );
          for( i = 0; i < NUM_LUZS; i++ )
          {
            if( z[i].fcst.ei.adj < z[i].minEmp )
                z[i].minFlag = 2;      // Set flag marking under minimum
      
            /* Set flag marking not in cordon area or overrides used and ctrlparm = 0.  Also exclude any LUZs with no forecast adj change; we don't want to redistribute from the base emp. */
            else if( !z[i].cordon || ( z[i].eOvr && !ctrlParmE ) || ( z[i].fcst.ei.adj == 0 ) )
              z[i].minFlag = 1;
            else
              z[i].minFlag = 0;
            minflags[i] = z[i].minFlag;
          }   // end for i

          st = rn = rp = 0;
          for( i = 0; i < NUM_LUZS; i++ )
          {
            writeToStatusBox("In Minimum redist , Processing LUZ # " + i.ToString());

            /* Redistribute from closest LUZs over min to LUZ under minimum constraints */
            if( z[i].minFlag == 2 )
            {
              k = 0;
              shortage = z[i].minEmp - z[i].fcst.ei.adj;
              st += shortage;
              sqlCommand.CommandText = String.Format(appSettings["select06"].Value, TN.impedAM, (i+1),scenarioID, bYear);
              try
              {
                sqlConnection.Open();
                rdr = sqlCommand.ExecuteReader();          
                /* Don't actually use impedence, just here for sorting the query 
                  * results. */
                while( rdr.Read() )
                {
                 
                  dest = rdr.GetInt16( 0 );
                  imp1 = rdr.GetInt16( 1 );
                  iAM[k++] = dest;
                }   // end while
                rdr.Close();
           
              }   // end try
              catch( SqlException sq )
              {
                MessageBox.Show( sq.ToString(), "SQL Exception" );
                Close();
              }
              finally
              {
                sqlConnection.Close();
              }

              loopCount = 0;
              while( shortage > 0 && loopCount < 10000)
              {
                // Do the redistribution
                for( k = 0; k < NUM_LUZS - 1; k++ )
                {
                  // Index into imped array for closest LUZ
                  j = iAM[k] - 1;
                  if( z[j].minFlag > 0 )      /* If this target is at or over cap, get another. */
                    continue;
                  else
                  {
                    adjustment = Math.Min( shortage, ( int )( 0.10 * 
                                ( double )z[j].fcst.ei.adj ) );
                    if( adjustment != 0 )
                    {
                      reals[j] += adjustment;     /* Increment counter for source LUZ */
                      realt[i] += adjustment;     /* Increment counter for target sum */
                      shortage -= adjustment;     // Decrement excess
        
                      z[i].fcst.ei.adj += adjustment;     // Increment target LUZ
                      rn += adjustment;
                      z[j].fcst.ei.adj -= adjustment;     // Decrement source LUZ
                      rp += adjustment;
                    }   // end if
                  }   // end else       

                  // Out of while loop, check shortage
                  if( shortage == 0 )
                      break;      // Shortage has been filled, get out of for loop
                }     // End for k
                ++loopCount;
              }     // End while
            }     // End if
      
            if( realt[i] > 0 )
            {
              flags.minCons = true;
              minConstraints.WriteLine( "MINIMUM REDISTRIBUTED " + realt[i] + " TO LUZ " + ( i + 1 ) );
            }   // end if

            for( k = 0; k < NUM_LUZS; k++ )
            {
                oldAdj[k] = z[k].fcst.ei.adj;
              if( reals[k] > 0 )
              {
                minConstraints.WriteLine( "    REDISTRIBUTED " + reals[k] + " FROM LUZ " + ( k + 1 ) );
                minConstraints.Flush();
              }   // end if
            }   // end for k
          }     // End for i
                
          if( EMP_CALC_DEBUG )     // Print old and adjusted if debug is on.
          {
            ot = nt = 0;
            for( i = 0; i < NUM_LUZS; i++ )
            {
              ot += oldAdj[i];
              nt += z[i].fcst.ei.adj;
              mc.WriteLine( "LUZ = {0,4} OLD_ADJ = {1,8} MIN_ADJ = {2,8} " + "NEW_ADJ = {3,8}", i + 1, oldAdj[i], z[i].minEmp, z[i].fcst.ei.adj );
              mc.Flush();
            }   // end for i

            mc.WriteLine( " TOT OLD_NEG = {0,8} OLD_POS = {1,8} OLD_TOT = {2,8} " + "NEW_NEG = {3,8} NEW_POS = {4,8} NEW_TOT = {5,8}", lt, gt, ot, rn, rp, nt );
            mc.Close();
          } // end if

        }     // End method minConstraintEmp()

        /*****************************************************************************/

        /* method openFiles() */
        /// <summary>
        /// Method to open all of the ASCII output files for employment.
        /// </summary>

        /* Revision History
        * 
        * STR             Date       By    Description
        * --------------------------------------------------------------------------
        *                 04/08/97   tb    Initial coding
        *                 07/22/03   df    C# revision
        * --------------------------------------------------------------------------
        */
        private void openFiles()
        {          
          // Open formatted files
          try
          {
            empChange = new StreamWriter( new FileStream(networkPath + String.Format(appSettings["empChange"].Value), FileMode.Create ) );
            empChange1 = new StreamWriter(new FileStream(networkPath + String.Format(appSettings["empChange1"].Value), FileMode.Create));
            empOut = new StreamWriter(new FileStream(networkPath + String.Format(appSettings["empOut"].Value), FileMode.Create));
            empOvr = new StreamWriter(new FileStream(networkPath + String.Format(appSettings["empOvr"].Value), FileMode.Create));
            minConstraints = new StreamWriter(new FileStream(networkPath + String.Format(appSettings["minConstraints"].Value), FileMode.Create));
            redisEmp = new StreamWriter(new FileStream(networkPath + String.Format(appSettings["redisEmp"].Value), FileMode.Create));
            schPart = new StreamWriter(new FileStream(networkPath + String.Format(appSettings["schPart"].Value), FileMode.Create));
          }

          catch( IOException e )
          {
            MessageBox.Show( "Error opening file for writing.  " + e.Message );

          }

          minConstraints.WriteLine( "MINIMUM CONSTRAINTS REDISTRIBUTION FILE" );
               
          redisEmp.WriteLine( "LUZ EMPLOYMENT REDISTRIBUTION FILE" );
          
          schPart.WriteLine( "PARTIALLY DEVELOPED SCHOOL SITES" );
          
        }     // End method openFiles()

        /*****************************************************************************/

        /* method printAuxEmp() */
        /// <summary>
        /// Method to write auxillary employment reports to ASCII.
        /// </summary>

        /* Revision History
        * 
        * STR             Date       By    Description
        * --------------------------------------------------------------------------
        *                 06/12/97   tb    Initial coding
        *                 07/30/03   df    C# revision
        * --------------------------------------------------------------------------
        */
        private void printAuxEmp()
        {   
          // Overrides file
          if( flags.eOvr )
            printEmpOvr();

          // Outliers file
          if( flags.eOut )
            printEmpOutliers();

          // Emp change file
          if( flags.empChange )
          {
            printEmpTableSpecial();
            printEmpTable7();
            printEmpTable8();
            printEmpTable9();
            printEmpTable10();
          }   // end if
        }     // End method printAuxEmp()

        /*****************************************************************************/

        /* method printEmpOutliers() */
        /// <summary>
        /// Method to write employment outliers reports to ASCII.
        /// </summary>

        /* Revision History
        * 
        * STR             Date       By    Description
        * --------------------------------------------------------------------------
        *                 06/12/97   tb    Initial coding
        *                 07/31/03   df    C# revision
        * --------------------------------------------------------------------------
        */
        private void printEmpOutliers()
        {
          int i;
          empOut.WriteLine( outputLabel );
          empOut.WriteLine( " LUZ      Code     Ratio       Inc      Inc5     Diff5" );
          empOut.WriteLine( "-----------------------------------------------------------" );
      
          for(i = 0; i < NUM_LUZS; i++ )
          {
            if( z[i].eOut.outCode > 0 )
              empOut.WriteLine( "{0,4}{1,10}{2,10:F4}{3,10}{4,10}{5,10}", 
                    ( i + 1 ), z[i].eOut.outCode, z[i].eOut.r5, 
                    z[i].fcst.ei.civ, z[i].eOut.inc5, 
                    z[i].eOut.diff5 );
          }   // end for i
        }   // end printEmpOutliers()

        /*****************************************************************************/

        /* method printEmpOvr() */
        /// <summary>
        /// Method to write employment overrides reports to ASCII.
        /// </summary>

        /* Revision History
        * 
        * STR             Date       By    Description
        * --------------------------------------------------------------------------
        *                 06/12/97   tb    Initial coding
        *                 07/30/03   df    C# revision
        * --------------------------------------------------------------------------
        */
        private void printEmpOvr()
        {
          string title1 = " LUZ       Civ     Redev    Infill   Vac-Ind   Vac-Com   Vac-Off   Vac-Sch Total-ELU";
          string title2 = "------------------------------------------------------------------------------------";
          int eluTotal,i,j;
          int regCiv = 0, regELUTotal = 0;
          int[] regELU = new int[7];
          // ---------------------------------------------------------------------

          empOvr.WriteLine( "LUZ EMPLOYMENT OVERRIDES " + outputLabel );
          empOvr.WriteLine();
          empOvr.WriteLine();
          empOvr.WriteLine( title1 );
          empOvr.WriteLine( title2 );

          for(i = 0; i < NUM_LUZS; i++ )
          {
            if( z[i].eOvr )     // Skip LUZs with no overrides
            {     
              eluTotal = 0;     // Initialize the land use overrides total
              empOvr.Write( "{0,4}{1,10}", ( i + 1 ), z[i].eo.adj );
              regCiv += z[i].eo.adj;      // Add to regional civ total

              for(j = 1; j < NUM_EMP_LAND; j++ )
              {
                empOvr.Write( "{0,10}", z[i].eo.elu[j] );     /* Write the LUZ emp land overrides */
                eluTotal += z[i].eo.elu[j];       // Get LUZ total for emp land
                regELU[j] += z[i].eo.elu[j];      /* Accumulate emp land overrides for region */
              }   // end for j

              regELUTotal += eluTotal;                    /* Accumulate total emp land overrides for region */
              empOvr.WriteLine( "{0,10}", eluTotal );     /* LUZ total emp land overrides */
            }   // end if
          }     // End for i

          // Write the regional totals
          empOvr.Write( "Reg{0,10}", regCiv );
          for(j = 1; j < NUM_EMP_LAND; j++ )
            empOvr.Write( "{0,10}", regELU[j] );          /* Write the reg emp land overrides total */
          empOvr.WriteLine( "{0,10}", regELUTotal );      /* Regional total emp land overrides */
        }     // End method printEmpOvr()

        /*****************************************************************************/

        /* method printEmpTable4() */
        /// <summary>
        /// Method to write civilian employment change reports to ASCII.
        /// </summary>

        /* Revision History
        * 
        * STR             Date       By    Description
        * --------------------------------------------------------------------------
        *                 06/12/97   tb    Initial coding
        *                 03/03/98   tb    Added 0.5 to computation of e_cap_share 
        *                                  to get closer to total
        *                 06/08/98   tb    Split from print_echange()
        *                 07/27/03   df    C# revision
        * --------------------------------------------------------------------------
        */
        private void printEmpTable4()
        {
          string title1 = " LUZ    Base    Fcst     Chg    %Chg    Chg5   %Chg5  CapShr  OutRat";
          string title2 = "--------------------------------------------------------------------";
          int lineCount = 0;
          int r1, r2, r3, r4, r5,i;
          double chg1, r8;

          // Civilian Employment Change
          empChange.WriteLine( "Table 2-4" );
          empChange.WriteLine( "CIVILIAN EMPLOYMENT "  + outputLabel );
          empChange.WriteLine( title1 );
          empChange.WriteLine( title2 );
          
          r8 = r1 = r2 = r3 = r4 = r5 = 0;

          for(i = 0; i < NUM_LUZS; i++ )
          {
            if( z[i].baseData.e.civ > 0 )
              chg1 = ( double )z[i].fcst.ei.civ / ( double )z[i].baseData.e.civ * 100;
            else
              chg1 = 0;
            if( reg.capacity.totalEmp > 0 )
              z[i].fcst.eCapShare = ( int )( 0.5 + rc.fcst.ei.adj * ( ( double )z[i].capacity.totalEmp / 
                                    ( double )reg.capacity.totalEmp ) );
            empChange.WriteLine( "{0,4}{1,8}{2,8}{3,8}{4,8:F1}{5,8}{6,8:F1}{7,8}{8,8:F4}", ( i + 1 ), z[i].baseData.e.civ, 
                                z[i].fcst.e.civ, z[i].fcst.ei.civ,chg1, z[i].histEmp.c5, z[i].histEmp.pct5, z[i].fcst.eCapShare, z[i].eOut.r5 );
            r1 += z[i].baseData.e.civ;
            r2 += z[i].fcst.e.civ;
            r3 += z[i].fcst.ei.civ;
            r4 += z[i].histEmp.c5;
            r5 += z[i].fcst.eCapShare;
            empChange.Flush();
            lineCount++;

            if( lineCount >= 57 )
            {
              empChange.WriteLine();
              empChange.WriteLine();
              lineCount = 0;      // Reset line count
              empChange.WriteLine( "Table 2-4" );
              empChange.WriteLine( "CIVILIAN EMPLOYMENT " + outputLabel );
              empChange.WriteLine( title1 );
              empChange.WriteLine( title2 );
            }   // end if
          }     // End for

          if( r1 > 0 )
            r8 = ( double )r3 / ( double ) r1 * 100;
          else
            r8 = 0;

          if( reg.baseData.e.civ > 0 )
            chg1 = ( double )reg.fcst.ei.civ / ( double )reg.baseData.e.civ * 100;
          else
            chg1 = 0;
          empChange.WriteLine( title2 );
          empChange.WriteLine();
          empChange.WriteLine( "Sum{0,8}{1,8}{2,8}{3,8:F1}{4,8}     " +  "N/A{5,8}     N/A", r1, r2, r3, r8, r4, r5 );
          empChange.WriteLine( "Reg{0,8}{1,8}{2,8}{3,8:F1}{4,8}{5,8:F1}     " +
                              "N/A     N/A", reg.baseData.e.civ, reg.fcst.e.civ,reg.fcst.ei.civ, chg1, reg.histEmp.c5,reg.histEmp.pct5 );
          empChange.Flush();
        }     // End method printEmpTable4()

        /*****************************************************************************/

        /* method printEmpTable5() */
        /// <summary>
        /// Method to write components of employment change reports to ASCII.
        /// </summary>

        /* Revision History
        * 
        * STR             Date       By    Description
        * --------------------------------------------------------------------------
        *                 06/12/97   tb    Initial coding
        *                 03/03/98   tb    Added 0.5 to computation of e_cap_share 
        *                                  to get closer to total
        *                 06/08/98   tb    Split from print_echange()
        *                 07/31/03   df    C# revision
        * --------------------------------------------------------------------------
        */
        private void printEmpTable5()
        {
          string title30 = "                       Prog    Prog";
          string title3 = " LUZ    Base   SSCiv    Loss    Gain    Fcst     Chg    %Chg";
          string title4 = "------------------------------------------------------------";
          int lineCount = 0;
          int r1, r2, r3, r4, r5, r6, r7,i;
          double chg1;

          // Components of employment total change

          r1 = r2 = r3 = r4 = r5 = r6 = r7 = 0;

          empChange.WriteLine( "Table 2-5" );
          empChange.WriteLine( "COMPONENTS OF TOTAL EMPLOYMENT CHANGE" + outputLabel );
          empChange.WriteLine( title30 );
          empChange.WriteLine( title3 );
          empChange.WriteLine( title4 );

          for(i = 0; i < NUM_LUZS; i++ )
          {
            if( z[i].baseData.e.total > 0 )
              chg1 = ( double )z[i].fcst.ei.total /( double )z[i].baseData.e.total * 100;
            else
              chg1 = 0;

            empChange.WriteLine( "{0,4}{1,8}{2,8}{3,8}{4,8}{5,8}{6,8}{7,8:F1}", ( i + 1 ), z[i].baseData.e.total, 
                                z[i].site.civ, z[i].loss, z[i].gain, z[i].fcst.e.total, z[i].fcst.ei.total, chg1 );        
            r1 += z[i].baseData.e.total;
            r2 += z[i].site.civ;
            r3 += z[i].loss;
            r4 += z[i].gain;
            r5 += z[i].fcst.e.total;
            r6 += z[i].fcst.ei.total;
            empChange.Flush();

            lineCount++;

            if( lineCount >= 57 )
            {
              lineCount = 0;
              empChange.WriteLine( "Table 2-5" );
              empChange.WriteLine( "COMPONENTS OF TOTAL EMPLOYMENT CHANGE" + outputLabel );
              empChange.WriteLine( title30 );
              empChange.WriteLine( title3 );
              empChange.WriteLine( title4 );
            }   // end if
          }     // End for
        
          if( r1 > 0 )
            chg1 = ( double )r7 / ( double )r1 * 100;
          else
            chg1 = 0;
          empChange.WriteLine();
          empChange.WriteLine( "Sum{0,8}{1,8}{2,8}{3,8}{4,8}{5,8}{6,8:F1}", r1, r2, r3, r4, r5, r6, chg1 );
          if( reg.baseData.e.total > 0 )
            chg1 = ( double )reg.fcst.ei.total / ( double )reg.baseData.e.total * 100;
          else
            chg1 = 0;
          empChange.WriteLine( title4 );
          empChange.WriteLine( "Reg{0,8}{1,8}{2,8}{3,8}{4,8}{5,8}{6,8:F1}",reg.baseData.e.total, reg.site.civ, reg.loss, reg.gain, reg.fcst.e.total,  reg.fcst.ei.total, chg1 );
          empChange.Flush();
        }     // End procedure printEmpTable5()

        /*****************************************************************************/

        /* method printEmpTable6() */
        /// <summary>
        /// Method to write employment change by land use reports to ASCII.
        /// </summary>

        /* Revision History
        * 
        * STR             Date       By    Description
        * --------------------------------------------------------------------------
        *                 06/12/97   tb    Initial coding
        *                 03/03/98   tb    Added 0.5 to computation of e_cap_share 
        *                                  to get closer to total
        *                 06/08/98   tb    Split from print_echange()
        *                 07/31/03   df    C# revision
        * --------------------------------------------------------------------------
        */
        private void printEmpTable6()
        {
          string title5 = " LUZ    Redev   Infill  Vac-Ind  Vac-Com  Vac-Off  Vac-Sch      Sum    Total";
          string title6 = "----------------------------------------------------------------------------";
          int i, j, lt;
          int lineCount;
          int[] ra = new int[9];
        
          // Employment change by land use
        
          lineCount = 0;
          empChange.WriteLine( "Table 2-6" );
          empChange.WriteLine( "CIVILIAN EMPLOYMENT CHANGE (LESS SITE SPEC) BY LAND USE " + outputLabel );
          empChange.WriteLine( title5 );
          empChange.WriteLine( title6 );
      
          for( i = 0; i < NUM_LUZS; i++ )
          {
            lt = 0;
            empChange.WriteLine( "{0,4}", ( i + 1 ) );
            for( j = 1; j < NUM_EMP_LAND; j++ )
            {
              empChange.Write( "{0,9}", z[i].fcst.ei.empLand[j] );
              ra[j] += z[i].fcst.ei.empLand[j];
              lt += z[i].fcst.ei.empLand[j];
            }   // end for j
            ra[7] += lt;
            ra[8] += z[i].fcst.ei.adj;
            empChange.WriteLine( "{0,9}{1,9}", lt, z[i].fcst.ei.adj );
            empChange.Flush();
            lineCount++;

            if( lineCount >= 57 )
            {
              lineCount = 0;
              empChange.WriteLine( "Table 2-6" );
              empChange.WriteLine( "CIVILIAN EMPLOYMENT CHANGE (LESS SITE SPEC) BY LAND USE " + outputLabel );
              empChange.WriteLine( title5 );
              empChange.WriteLine( title6 );
            }   // end if
          }       // End for i

          empChange.WriteLine( title6 );
          empChange.WriteLine();
          empChange.WriteLine( "Sum" );
          for( j = 1; j < 9; j++ )
            empChange.Write( "{0,9}", ra[j] );
          empChange.WriteLine();

          empChange.WriteLine( " Reg" );
          for( j = 1; j < NUM_EMP_LAND; j++ )
            empChange.Write( "{0,9}", reg.fcst.ei.empLand[j] );
          empChange.WriteLine( "{0,9}{1,9}", reg.fcst.ei.adj, reg.fcst.ei.adj );
          empChange.Flush();
        }     // End method printEmpTable6()

        /*****************************************************************************/

        /* method printEmpTable7() */
        /// <summary>
        /// Method to write employment change reports to ASCII.
        /// </summary>

        /* Revision History
        * 
        * STR             Date       By    Description
        * --------------------------------------------------------------------------
        *                 06/12/97   tb    Initial coding
        *                 03/03/98   tb    Added 0.5 to computation of e_cap_share 
        *                                  to get closer to total
        *                 06/08/98   tb    Split from print_echange()
        *                 08/01/03   df    C# revision
        * --------------------------------------------------------------------------
        */
        private void printEmpTable7()
        {
          string title5 = " LUZ    Redev   Infill  Vac-Ind  Vac-Com  Vac-Off  Vac-Sch      Sum    Total";
          string title6 = "----------------------------------------------------------------------------";

          int i, j, lt;
          int lineCount = 0;
          int[] ra = new int[9];

          /****** Starting Capacity By Land Use ******/
          empChange.WriteLine( "Table 2-7a" );
          empChange.WriteLine( "STARTING CIVILIAN EMPLOYMENT CAPACITY BY LAND USE " + outputLabel );
          empChange.WriteLine( title5 );
          empChange.WriteLine( title6 );
      
          for( i = 0; i < NUM_LUZS; i++ )
          {
            lt = 0;
            empChange.Write( "{0,4}", ( i + 1 ) );
            for( j = 1; j < NUM_EMP_LAND; j++ )
            {
              empChange.Write( "{0,9}", z[i].capacity.e[j] );
              ra[j] += z[i].capacity.e[j];
              lt += z[i].capacity.e[j];
            }   // end for j
            ra[7] += lt;
            ra[8] += z[i].capacity.totalEmp;
            empChange.WriteLine( "{0,9}{1,9}", lt, z[i].capacity.totalEmp );
            empChange.Flush();
            lineCount++;

            if( lineCount >= 57 )
            {
              lineCount = 0;
              empChange.WriteLine( "Table 2-7a" );
              empChange.WriteLine( "STARTING CIVILIAN EMPLOYMENT CAPACITY BY " + "LAND USE  " + outputLabel );
              empChange.WriteLine( title5 );
              empChange.WriteLine( title6 );
            }   // end if
          }     // End for i
      
          empChange.WriteLine( title6 );
          empChange.WriteLine();
          empChange.WriteLine( "Sum" );
          for( j = 1; j < 9; j++ )
            empChange.Write( "{0,9}", ra[j] );
          empChange.WriteLine();

          empChange.Write( "Reg" );
          for( j = 1; j < NUM_EMP_LAND; j++ )
            empChange.Write( "{0,9}", reg.capacity.e[j] );
          empChange.WriteLine( "      N/A{0,9}", reg.capacity.totalEmp );
          empChange.Flush();


          /****** Capacity Used By Land Use ******/
          lineCount = 0;
          empChange.WriteLine( "Table 2-7b" );
          empChange.WriteLine( "USED CIVILIAN EMPLOYMENT CAPACITY BY LAND USE " + outputLabel );
          empChange.WriteLine( title5 );
          empChange.WriteLine( title6 );
      
          for( i = 0; i < NUM_LUZS; i++ )
          {
            lt = 0;
            empChange.Write( "{0,4}", ( i + 1 ) );
            for( j = 1; j < NUM_EMP_LAND; j++ )
            {
              empChange.Write( "{0,9}", z[i].useCap.e[j] );
              ra[j] += z[i].useCap.e[j];
              lt += z[i].useCap.e[j];
            }   // end for j
            ra[7] += lt;
            ra[8] += z[i].useCap.totalEmp;
            empChange.WriteLine( "{0,9}{1,9}", lt, z[i].useCap.totalEmp );

            empChange.Flush();
            lineCount++;
            if( lineCount >= 57 )
            {
              lineCount = 0;
              empChange.WriteLine( "Table 2-7b" );
              empChange.WriteLine( "USED CIVILIAN EMPLOYMENT CAPACITY BY LAND USE " + outputLabel );
              empChange.WriteLine( title5 );
              empChange.WriteLine( title6 );
            }   // end if
          }     // End for i
      
          empChange.WriteLine( title6 );
          empChange.WriteLine();
          empChange.WriteLine( "Sum" );
          for( j = 1; j < 9; j++ )
            empChange.Write( "{0,9}", ra[j] );
          empChange.WriteLine();

          empChange.Write( "Reg" );
          for( j = 1; j < NUM_EMP_LAND; j++ )
          {
            empChange.Write( "{0,9}", reg.useCap.e[j] );
            reg.useCap.totalEmp += reg.useCap.e[j];
          }
          empChange.WriteLine( "      N/A{0,9}", reg.useCap.totalEmp );
          empChange.Flush();

          /****** Remaining Capacity By Land Use ******/
          lineCount = 0;
          empChange.WriteLine( "Table 2-7c" );
          empChange.WriteLine( "REMAINING CIVILIAN EMPLOYMENT CAPACITY BY LAND USE " + outputLabel );
          empChange.WriteLine( title5 );
          empChange.WriteLine( title6 );
      
          for( i = 0; i < NUM_LUZS; i++ )
          {
            lt = 0;
            empChange.Write( "{0,4}", ( i + 1 ) );
            for( j = 1; j < NUM_EMP_LAND; j++ )
            {
              empChange.Write( "{0,9}", z[i].remCap.e[j] );
              ra[j] += z[i].remCap.e[j];
              lt += z[i].remCap.e[j];
            }   // end for j
            ra[7] += lt;
            ra[8] += z[i].remCap.totalEmp;
            empChange.WriteLine( "{0,9}{1,9}", lt, z[i].remCap.totalEmp );
            empChange.Flush();
            lineCount++;

            if( lineCount >= 57 )
            {
              lineCount = 0;
              empChange.WriteLine( "Table 2-7c" );
              empChange.WriteLine( "REMAINING CIVILIAN EMPLOYMENT CAPACITY BY LAND USE " + outputLabel );
              empChange.WriteLine( title5 );
              empChange.WriteLine( title6 );
            }   // end if
          }     // End for i

          empChange.WriteLine( title6 );
          empChange.WriteLine();
          empChange.WriteLine( "Sum" );
          for( j = 1; j < 9; j++ )
            empChange.Write( "{0,9}", ra[j] );
          empChange.WriteLine();
          empChange.Write( "Reg" );
          for( j = 1; j < NUM_EMP_LAND; j++ )
            empChange.Write( "{0,9}", reg.remCap.e[j] );
          empChange.WriteLine( "      N/A{0,9}", reg.remCap.totalEmp );
          empChange.Flush();
        }     // End method printEmpTable7()

        /*****************************************************************************/

        /* method printEmpTable8() */
        /// <summary>
        /// Method to write employment change reports to ASCII.
        /// </summary>

        /* Revision History
        * 
        * STR             Date       By    Description
        * --------------------------------------------------------------------------
        *                 06/12/97   tb    Initial coding
        *                 03/03/98   tb    Added 0.5 to computation of e_cap_share 
        *                                  to get closer to total
        *                 06/08/98   tb    Split from print_echange()
        *                 08/01/03   df    C# revision
        * --------------------------------------------------------------------------
        */
        private void printEmpTable8()
        {
          string title51 = " LUZ    Redev   Infill  Vac-Ind  Vac-Com  Vac-Off  Vac-Sch      Sum";
          string title61 = "-------------------------------------------------------------------";
          int i, j, lineCount = 0;
          int tc, te;
          double pct;
          double[] rba = new double[8], rca = new double[8];

          /****** Percent Of Capacity By Land Use ******/
          empChange.WriteLine( "Table 2-8" );
          empChange.WriteLine( "PERCENT OF USED CIVILIAN EMPLOYMENT CAPACITY BY LAND USE " + outputLabel );
          empChange.WriteLine( title51 );
          empChange.WriteLine( title61 );
        
          for( i = 0; i < NUM_LUZS; i++ )
          {
            te = tc = 0;
            empChange.Write( "{0,4}", ( i + 1 ) );
            for( j = 1; j < NUM_EMP_LAND; j++ )
            {
              te += z[i].useCap.e[j];
              tc += z[i].capacity.e[j];
              rba[j] += z[i].useCap.e[j];
              rca[j] += z[i].capacity.e[j];

              if( z[i].capacity.e[j] > 0 )
                pct = ( double )z[i].useCap.e[j] / ( double )z[i].capacity.e[j] * 100;
              else
                pct = 0;

              empChange.Write( "{0,9:F1}", pct );
            }   // end for j
            rba[7] += te;
            rca[7] += tc;

            if( tc > 0 )
              empChange.WriteLine( "{0,9:F1}", ( double )te / ( double )tc * 100 );
            else
              empChange.WriteLine( "     0.00" );
            empChange.Flush();
            lineCount++;

            if( lineCount >= 57 )
            {
              lineCount = 0;
              empChange.WriteLine( "Table 2-8" );
              empChange.WriteLine( "PERCENT OF USED CIVILIAN EMPLOYMENT CAPACITY BY LAND USE ", outputLabel );
              empChange.WriteLine( title51 );
              empChange.WriteLine( title61 );
            }   // end if
          }     // End for i
        
          empChange.WriteLine( title61 );
          empChange.Write( "Sum" );
          for( j = 1; j < 8; j++ )
          {
              if( rca[j] > 0 )
                pct = ( double )rba[j] / ( double )rca[j] * 100;
              else
                pct = 0;
              empChange.Write( "{0,9:F1}", pct );
          }   // end for j
          empChange.WriteLine();
          empChange.Flush();
        
          te = tc = 0;
          empChange.Write( "Reg" );
          for( j = 1; j < NUM_EMP_LAND; j++ )
          {
            te += reg.fcst.ei.empLand[j];
            tc += reg.capacity.e[j];
            if( reg.capacity.e[j] > 0 )
              pct = ( double )reg.useCap.e[j] / ( double )reg.capacity.e[j] * 100;
            else
              pct = 0;
            empChange.Write( "{0,9:F1}", pct );
          }   // end for j

          if( tc > 0 )
            empChange.WriteLine( "{0,9:F1}", ( double )te / ( double )tc * 100 );
          empChange.Flush();
        }     // End method printEmpTable8()

        /*****************************************************************************/

        /* method printEmpTable9() */
        /// <summary>
        /// Method to write employment change reports to ASCII.
        /// </summary>

        /* Revision History
        * 
        * STR             Date       By    Description
        * --------------------------------------------------------------------------
        *                 06/12/97   tb    Initial coding
        *                 03/03/98   tb    Added 0.5 to computation of e_cap_share 
        *                                  to get closer to total
        *                 08/01/03   df    C# revision
        * --------------------------------------------------------------------------
        */
        private void printEmpTable9()
        {
          string title51 = " LUZ    Redev   Infill  Vac-Ind  Vac-Com  Vac-Off  Vac-Sch      Sum";
          string title61 = "-------------------------------------------------------------------";
          int i, j, lineCount = 0;
          double tc, te, pct;
          double[] rba = new double[8], rca = new double[8];

          /****** Percent of Developable Acreage By Land Use ******/
          empChange.WriteLine( "Table 2-9" );
          empChange.WriteLine( "PERCENT OF DEVELOPABLE CIVILIAN EMPLOYMENT ACREAGE BY LAND USE " + outputLabel );
          empChange.WriteLine( title51 );
          empChange.WriteLine( title61 );
          for( i = 0; i < NUM_LUZS; i++ )
          {
            te = tc = 0;
            empChange.Write( "{0,4}", ( i + 1 ) );
            for( j = 1; j < NUM_EMP_LAND; j++ )
            {
              te += z[i].useCap.ac.ae[j];
              tc += z[i].capacity.ac.ae[j];
              rba[j] += z[i].useCap.ac.ae[j];
              rca[j] += z[i].capacity.ac.ae[j];
              if( z[i].capacity.ac.ae[j] > 0 )
                pct = ( double )z[i].useCap.ac.ae[j] / ( double )z[i].capacity.ac.ae[j] * 100;
              else
                pct = 0;
              empChange.Write( "{0,9:F1}", pct );
            }   // end for j

            rba[7] += te;
            rca[7] += tc;
            if( tc > 0 )
              empChange.WriteLine( "{0,9:F1}", ( double )te / ( double )tc * 100 );
            else
              empChange.WriteLine( "     0.00" );
            empChange.Flush();

            lineCount++;
            if( lineCount >= 57 )
            {
              lineCount = 0;
              empChange.WriteLine( "Table 2-9" );
              empChange.WriteLine( "PERCENT OF DEVELOPABLE CIVILIAN EMPLOYMENT ACREAGE BY LAND USE " + outputLabel );
              empChange.WriteLine( title51 );
              empChange.WriteLine( title61 );
            }   // end if
          }     // End for i

          empChange.WriteLine( title61 );
          empChange.WriteLine( "Sum" );
        
          for( j = 1; j < 8; j++ )
          {
            if( rca[j] > 0 )
              pct = ( double )rba[j] / ( double )rca[j] * 100;
            else
              pct = 0;
            empChange.Write( "{0,9}", pct );
          }   // end for j
          empChange.WriteLine();
          empChange.Flush();
          te = tc = 0;
          empChange.Write( "Reg" );
          for( j = 1; j < NUM_EMP_LAND; j++ )
          {
            te += reg.useCap.ac.ae[j];
            tc += reg.capacity.ac.ae[j];
            if( reg.capacity.ac.ae[j] > 0 )
              pct = ( double )reg.useCap.ac.ae[j] / ( double )reg.capacity.ac.ae[j] * 100;
            else
              pct = 0;
            empChange.Write( "{0,9}", pct );
          }   // end for j

          if( tc > 0 )
            empChange.WriteLine( "{0,9:F1}", ( double )te / ( double )tc * 100 );
          empChange.Flush();
        }     // End method printEmpTable9()

        /*****************************************************************************/

        /* method printEmpTable10() */
        /// <summary>
        /// Method to write employment change reports to ASCII.
        /// </summary>

        /* Revision History
        * 
        * STR             Date       By    Description
        * --------------------------------------------------------------------------
        *                 06/12/97   tb    Initial coding
        *                 03/03/98   tb    Added 0.5 to computation of e_cap_share 
        *                                  to get closer to total
        *                 08/01/03   df    C# revision
        * --------------------------------------------------------------------------
        */
        private void printEmpTable10()
        {
          string title70 = "                  Developable Acres Capacity        " + "                    Developable Acres Used";      
          string title71 = "                                  Developable Acres " +"Remaining";
          string title72 = " LUZ   Redev  Infill Vac-Ind Vac-Com Vac-Off Vac-Sch   Total";
          string title73 = "   Redev  Infill Vac-Ind Vac-Com Vac-Off Vac-Sch   " +
                          "Total   Redev  Infill Vac-Ind Vac-Com Vac-Off Vac-Sch   Total";
          string title74 = "----------------------------------------------------" +
                          "----------------------------------------------------------------";
          string title75 = "------------------------------------------------------------";
          int i, j;
          int lineCount = 0;
          double[] rba = new double[8], rca = new double[8], rda = new double[8];
        
          /****** Land Consumption ******/
          lineCount = 0;
          empChange1.WriteLine( "Table 2-10" );
          empChange1.WriteLine( "CIVILIAN EMPLOYMENT LAND CONSUMPTION " + outputLabel );
          empChange1.Write( title70 );
          empChange1.WriteLine( title71 );
          empChange1.Write( title72 );
          empChange1.WriteLine( title73 );
          empChange1.Write( title74 );
          empChange1.WriteLine( title75 );
      
          for( i = 0; i < NUM_LUZS; i++ )
          {
            empChange1.Write( "{0,4}", ( i + 1 ) );
            for( j = 1; j < NUM_EMP_LAND; j++ )
            {
              empChange1.Write( "{0,8:F1}", z[i].capacity.ac.ae[j] );
              rba[j] += z[i].capacity.ac.ae[j];
            }   // end for j
            empChange1.Write( "{0,8:F1}", z[i].capacity.ac.totalEmpAcres );
            rba[7] += z[i].capacity.ac.totalEmpAcres;

            for( j = 1; j < NUM_EMP_LAND; j++ )
            {
              empChange1.Write( "{0,8:F1}", z[i].useCap.ac.ae[j] );
              rca[j] += z[i].useCap.ac.ae[j];
            }   // end for j
            empChange1.Write( "{0,8:F1}", z[i].useCap.ac.totalEmpAcres );
            rca[7] += z[i].useCap.ac.totalEmpAcres;

            for( j = 1; j < NUM_EMP_LAND; j++ )
            {
              empChange1.Write( "{0,8:F1}", z[i].remCap.ac.ae[j] );
              rda[j] += z[i].remCap.ac.ae[j];
            }   // end for j
            empChange1.WriteLine( "{0,8:F1}", z[i].remCap.ac.totalEmpAcres );
            rda[7] += z[i].remCap.ac.totalEmpAcres;
            empChange1.Flush();
            lineCount++;

            if( lineCount >= 30 )
            {
              lineCount = 0;
              empChange1.WriteLine( "Table 2-10" );
              empChange1.WriteLine( "CIVILIAN EMPLOYMENT LAND CONSUMPTION " + outputLabel );
              empChange1.Write( title70 );
              empChange1.WriteLine( title71 );
              empChange1.Write( title72 );
              empChange1.WriteLine( title73 );
              empChange1.Write( title74 );
              empChange1.WriteLine( title75 );
            }   // end if
          }     // End for i
        
          empChange1.WriteLine();
          empChange1.Write( "Sum" );
          for( j = 1; j < 8; j++ )
            empChange1.Write( "{0,8:F1}",rba[j] );
          for( j = 1; j < 8; j++ )
            empChange1.Write( "{0,8:F1}", rca[j] );
          for( j = 1; j < 8; j++ )
            empChange1.Write( "{0,8:F1}", rda[j] );
          empChange1.WriteLine();
            
          empChange1.Write( " Reg" );
          for( j = 1; j < NUM_EMP_LAND; j++ )
            empChange1.Write( "{0,8:F1}", reg.capacity.ac.ae[j] );
          empChange1.Write( "{0,8:F1}", reg.capacity.ac.totalEmpAcres );
          for( j = 1; j < NUM_EMP_LAND; j++ )
            empChange1.Write( "{0,8:F1}", reg.useCap.ac.ae[j] );
          empChange1.Write( "{0,8:F1}", reg.useCap.ac.totalEmpAcres );
          for( j = 1; j < NUM_EMP_LAND; j++ )
            empChange1.Write( "{0,8:F1}", reg.remCap.ac.ae[j] );
          empChange1.WriteLine( "{0,8:F1}", reg.remCap.ac.totalEmpAcres );
        }     // End method printEmpTable10()

        /*****************************************************************************/

        /* method printEmpTableSpecial() */
        /// <summary>
        /// Method to combine some data from 2-4, 2-7, and 2-8 for tuning.
        /// </summary>

        /* Revision History
        * 
        * STR             Date       By    Description
        * --------------------------------------------------------------------------
        *                 07/29/98   tb    Initial coding
        *                 08/01/03   df    C# revision
        * --------------------------------------------------------------------------
        */
        private void printEmpTableSpecial()
        {
          string title1 = " LUZ   Base   Fcst    Chg   %Chg  %Chg5 CapShr  SiteC    Cap  %Used";
          string title2 = "-------------------------------------------------------------------";
          int lineCount = 0,i;
          double chg1;
          StreamWriter empDataOnly = new StreamWriter( new FileStream(networkPath + String.Format(appSettings["empOnly"].Value),FileMode.Create ) );
          /****** Civilian Employment Change ******/
          empChange.WriteLine( "Table 2-SP" );
          empChange.WriteLine( "CIVILIAN EMPLOYMENT VARS  " + outputLabel );
          empChange.WriteLine( title1 );
          empChange.WriteLine( title2 );

          for(i = 0; i < NUM_LUZS; i++ )
          {
            if( z[i].baseData.e.civ > 0 )
              chg1 = ( double )z[i].fcst.ei.civ / ( double )z[i].baseData.e.civ * 100;
            else
              chg1 = 0;
            empChange.WriteLine( "{0,4}{1,7}{2,7}{3,7}{4,7:F1}{5,7:F1}{6,7}{7,7}{8,7}{9,7:F1}", ( i + 1 ),
                                z[i].baseData.e.civ, z[i].fcst.e.civ, z[i].fcst.ei.civ, chg1, z[i].histEmp.pct5, 
                                z[i].fcst.eCapShare, z[i].site.civ, z[i].capacity.totalEmp, z[i].useCap.pTotalEmp );
            empChange.Flush();
            empDataOnly.WriteLine( "{0,4}{1,7}{2,7}{3,7}{4,7:F1}{5,7:F1}{6,7}{7,7}{8,7}{9,7:F1}", ( i + 1 ),
                                z[i].baseData.e.civ, z[i].fcst.e.civ, z[i].fcst.ei.civ, chg1, z[i].histEmp.pct5, 
                                z[i].fcst.eCapShare, z[i].site.civ, z[i].capacity.totalEmp, z[i].useCap.pTotalEmp );
            empDataOnly.Flush();
            lineCount++;
            if( lineCount >= 57 )
            {
              lineCount = 0;     // Reset line count
              empChange.WriteLine( "Table 2-SP" );
              empChange.WriteLine( "CIVILIAN EMPLOYMENT VARS  " + outputLabel );
              empChange.WriteLine( title1 );
              empChange.WriteLine( title2 );
            }   // end if
          }     // End for i

          empChange.Flush();
          empDataOnly.Close();
        }     // End method printEmpTableSpecial()

        /*****************************************************************************/

        /* method processParams() */
        /// <summary>
        /// Method to process employment model input parameters and build table
        /// names.
        /// </summary>

        /* Revision History
        * 
        * STR             Date       By    Description
        * --------------------------------------------------------------------------
        *                 04/08/97   tb    Initial coding
        *                 02/02/99   tb    Changed name of access_wts because of 
        *                                  system errors
        *                 01/11/02   tb    Changes for Jan, 2002 sr10
        *                 07/22/03   df    C# revision
        * --------------------------------------------------------------------------
        */
        public bool processParams()
        {
            if( cboYears.SelectedIndex == -1 )
            { 
                MessageBox.Show( "Invalid selection of years!  Please try again.", "Invalid Years Range" );
                return false;
            }   // end if

            /* Decided to use compareTo method for text of cboScenario because users should be able to enter scenario by keyboard if they wish. */
            if( cboScenario.SelectedIndex == -1 )
            {
                MessageBox.Show( "Invalid scenario! Please try again." );
                return false;
            }   // end  if

            else if( txtMinContraint.Text.ToString() == "" )
            {
                MessageBox.Show( "You have entered an invalid minimum constraint! Please try again.", "Invalid Minimum Constraint" );
                return false;
            }   // end else if
        
            // In case the user entered a scenario via the keyboard, set the index.
      
            scenarioID = cboScenario.SelectedIndex;
            ctrlParmE = chkControlEmpOvr.Checked;
            doEmpOvr = chkEmpOvr.Checked;
            minSwitch = chkMinConstraints.Checked;

            try
            {
                minParam = double.Parse( txtMinContraint.Text.ToString() );
            }   // end try
            catch( FormatException e )
            {
                MessageBox.Show( "You have entered an invalid number! Please try again.  " + e.Message, "Invalid Number Entered" );
                return false;
            }   // end catch
            if( minParam > 0 )
            {
                MessageBox.Show( "You have selected an invalid minimum constraints parameter.  Please try again.", "Invalid Minimum Constraint" );
                return false;
            }   // end if

            bYear = incrementLabels[cboYears.SelectedIndex];
            fYear = incrementLabels[cboYears.SelectedIndex + 1];

            
            try
            {
                sqlConnection = new System.Data.SqlClient.SqlConnection();
                sqlCommand = new System.Data.SqlClient.SqlCommand();
                config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                appSettings = config.AppSettings.Settings;

                connectionStrings = config.ConnectionStrings.ConnectionStrings;

                sqlConnection.ConnectionString = connectionStrings["SR13DBConnectionString"].ConnectionString;
                networkPath = String.Format(appSettings["networkPath"].Value);
                this.sqlCommand.Connection = this.sqlConnection;

                EMP_CALC_DEBUG = bool.Parse(appSettings["EMP_CALC_DEBUG"].Value);   // write emp calcs to file
                RC_DEBUG = bool.Parse(appSettings["RC_DEBUG"].Value);        // write regional controls to file
                SF_DEBUG = bool.Parse(appSettings["SF_DEBUG"].Value);         // Write SF distribution to file
                ZB_DEBUG = bool.Parse(appSettings["ZB_DEBUG"].Value);        // Write luzBase to file
                ZH_DEBUG = bool.Parse(appSettings["ZH_DEBUG"].Value);        // Write luzHistory to file
                ZT_DEBUG = bool.Parse(appSettings["ZT_DEBUG"].Value);        // Write luzTemp to file

                // travel time model parms
                AUTO_MED = int.Parse(appSettings["AUTO_MED"].Value);            // median auto travel time
                TRAN_MED = int.Parse(appSettings["TRAN_MED"].Value);            // median tran travel time
                CBD_MED = int.Parse(appSettings["CBD_MED"].Value);              // median cbd travel time
                AUTO_STD = double.Parse(appSettings["AUTO_STD"].Value);         // std dev auto travel time
                TRAN_STD = double.Parse(appSettings["TRAN_STD"].Value);         // std dev tran travel time
                CBD_STD = double.Parse(appSettings["CBD_STD"].Value);           // std dev cbd travel time
                AUTO_NLA = double.Parse(appSettings["AUTO_NLA"].Value);         // auto nonlinear adj travel time
                TRAN_NLA = double.Parse(appSettings["AUTO_MED"].Value);         // tran nonliner adj travel time
                CBD_NLA = double.Parse(appSettings["AUTO_MED"].Value);          // cbd nonlinear adj travel time

                // empal equsation parms
                LAGGED_EMP = double.Parse(appSettings["LAGGED_EMP"].Value);          // exponent "b" in the EMPAL estimation equation */
                ALPHA = double.Parse(appSettings["ALPHA"].Value);
                BETA = double.Parse(appSettings["BETA"].Value);
                LAMBDA = double.Parse(appSettings["LAMBDA"].Value);
                CAP_EMP = double.Parse(appSettings["CAP_EMP"].Value);

                MAX_TRANS = int.Parse(appSettings["MAX_TRANS"].Value);
                NUM_EMP_LAND = int.Parse(appSettings["NUM_EMP_LAND"].Value);
                NUM_EMP_SECTORS = int.Parse(appSettings["NUM_EMP_SECTORS"].Value);
                NUM_HH_INCOME = int.Parse(appSettings["NUM_HH_INCOME"].Value);
                NUM_LUZS = int.Parse(appSettings["NUM_LUZS"].Value);
                NUM_MGRAS = int.Parse(appSettings["NUM_MGRAS"].Value);

                TN.accessWeights = String.Format(appSettings["accessWeightsTable"].Value);
                TN.capacity = String.Format(appSettings["capacity"].Value);
                TN.capacity1 = String.Format(appSettings["capacity1"].Value);
                TN.capacity2 = String.Format(appSettings["capacity2"].Value);
                TN.empDecrements = String.Format(appSettings["empDecrements"].Value);
                TN.empIncrements = String.Format(appSettings["empIncrements"].Value);
                TN.impedAM = String.Format(appSettings["impedAM"].Value);
                TN.luzbase = String.Format(appSettings["luzbase"].Value);
                TN.luzEmpOvr = String.Format(appSettings["luzEmpOvr"].Value);
                TN.luzhist = String.Format(appSettings["luzhist"].Value);
                TN.luzIncomeParms = String.Format(appSettings["luzIncomeParms"].Value);
                TN.luztemp = String.Format(appSettings["luztemp"].Value);
                TN.regfcst = String.Format(appSettings["regfcst"].Value);
                TN.homePrices = String.Format(appSettings["homePrices"].Value);
                TN.mgrabase = String.Format(appSettings["mgrabase"].Value);
                
                TN.milSiteSpec = String.Format(appSettings["milSiteSpec"].Value);
                
                TN.xref = String.Format(appSettings["xrefMGRA"].Value);
               
            }
            catch (ConfigurationErrorsException err)
            {
                string msg = err.Message;
                Console.WriteLine("Message: {0}", msg);

                string fileName = err.Filename;
                Console.WriteLine("Filename: {0}", fileName);

                int lineNumber = err.Line;
                Console.WriteLine("Line: {0}", lineNumber.ToString());

                string bmsg = err.BareMessage;
                Console.WriteLine("BareMessage: {0}", bmsg);

                string source = err.Source;
                Console.WriteLine("Source: {0}", source);

                string st = err.StackTrace;
                Console.WriteLine("StackTrace: {0}", st);
            }
            catch (NullReferenceException n)
            {
                MessageBox.Show("ERROR." + n.ToString());
                Application.Exit();
            }
          
            z = new Master[NUM_LUZS];
            reg = new Master();
            outputLabel = scenLabels[scenarioID] + " " + incrementLabels[cboYears.SelectedIndex] + " - " +
                            incrementLabels[cboYears.SelectedIndex + 1] + " " + DateTime.Now;
           
            flags = new Flags();
            rc = new RegionalControls();
            tcapd = new TCapD[NUM_LUZS,MAX_TRANS];
            tCapI = new TCap[NUM_LUZS,MAX_TRANS];
            tt = new TTP[3];
            zbi = new int[70];   // this is the size of the luzbase data set
            return true;
        }     // End method processParams()

        /*****************************************************************************/

        /* method redistributeEmp() */
        /// <summary>
        /// Method to redistribute any LUZ employment that exceeds capacity.
        /// </summary>

        /* Revision History
        * 
        * STR             Date       By    Description
        * --------------------------------------------------------------------------
        *                 04/10/97   tb    Initial coding
        *                 07/25/03   df    C# revision
        * --------------------------------------------------------------------------
        */
        private void redistributeEmp()
        {
        
          SqlDataReader rdr;

          int i, j, k, excess, imp1, dest, regTotal = 0;
          int[] iAM = new int[NUM_LUZS], reals = new int[NUM_LUZS], 
          realt = new int[NUM_LUZS], excapFlags = new int[NUM_LUZS];
          int[] oldAdj = new int[NUM_LUZS];
          // --------------------------------------------------------------------
      
          writeToStatusBox( "Redistributing Employment That Exceeds Capacity.." );

          for( i = 0; i < NUM_LUZS; i++ )
          {
            regTotal += z[i].fcst.ei.adj;
            if( z[i].fcst.ei.adj > z[i].capacity.totalEmp )
              excapFlags[i] = z[i].exCap = 2;      /* Set flag marking over capacity */     
            /* Set flag marking at capacity or not in cordon area or overrides used and ctrlparm = 0 */
            else if( z[i].fcst.ei.adj == z[i].capacity.totalEmp ||!z[i].cordon || ( z[i].eOvr && !ctrlParmE ) )
              excapFlags[i] = z[i].exCap = 1;
            else 
              excapFlags[i] = z[i].exCap = 0;
          }   // end for i

          for( i = 0; i < NUM_LUZS; i++ )
          {
            // Redistribute from LUZs over capacity to closest LUZ under capacity
            if (z[i].exCap == 2 )
            {
              k = 0;
              excess = z[i].fcst.ei.adj - z[i].capacity.totalEmp;
              sqlCommand.CommandText = String.Format(appSettings["select06"].Value, TN.impedAM, (i + 1), scenarioID, bYear);
              
              /* Don't actually use impedence, just here for sorting the query results. */
              try
              {
                sqlConnection.Open();
                rdr = sqlCommand.ExecuteReader();
                while( rdr.Read() )
                {
                  dest = rdr.GetInt16( 0 );
                  imp1 = rdr.GetInt16( 1 );
                  iAM[k++] = dest - 1;      // Convert LUZ id into array index
                }   // end while
                rdr.Close();
              }
              catch(Exception e)
              {
                MessageBox.Show(e.ToString(),e.GetType().ToString());

              }  // end catch
              finally
              {
                sqlConnection.Close();
              }

              // Do the redistribution
              for( k = 0; k < NUM_LUZS; k++ )
              {
                // Index into imped array for closest LUZ
                j = iAM[k];
                if( z[j].exCap > 0 )
                  continue;               /* If this target is at or over cap, get another */

                /* Stay in this loop until either the excess id gone or the 
                * target is maxed out. */
                while( excess > 0 && z[j].exCap == 0 )
                {
                  reals[i]++;             // Increment counter for source LUZ
                  realt[j]++;             // Increment counter for target LUZ
                  excess--;               // Decrement excess
                  z[i].fcst.ei.adj--;     // Decrement source LUZ
                  z[j].fcst.ei.adj++;     // Increment target LUZ

                  if( z[j].fcst.ei.adj == z[j].capacity.totalEmp )
                    z[j].exCap = 1;
                  if( excess == 0 || z[j].exCap > 0 )
                    break;
                }   // end while
        
                // Out of while loop, check excess
                if( excess == 0 )
                  break;                /* Excess has been redistributed, get out of for loop */
              }     // End for k
            }     // End if
            else   // else z[i].excap != 2 - go to next index
              continue;

            if( reals[i] > 0 )
            {
              flags.redisEmp = true;
              redisEmp.WriteLine( "EXCESS REDISTRIBUTED " + reals[i] + " FROM LUZ " + ( i + 1 ) );
              redisEmp.Flush();
            }   // end if

            for( k = 0; k < NUM_LUZS; k++ )
            {
              if( realt[k] > 0 )
              {
                redisEmp.WriteLine( "   REDISTRIBUTED " + realt[k] + " TO LUZ " + ( k + 1 ) );
                redisEmp.Flush();
              }   // end if
            }   // end for
          }     // End for i

          regTotal = 0;
          for (i = 0; i < NUM_LUZS; i++)
          {
              oldAdj[i] = new int();
              regTotal += z[i].fcst.ei.adj;
              oldAdj[i] = z[i].fcst.ei.adj;
          } // end for
     
        }     // End method redistributeEmp()

        /*****************************************************************************/

        /* method update05() */
        /// <summary>
        /// Method to update the capacity table from the emp_dec table.
        /// </summary>

        /* Revision History
        * 
        * STR             Date       By    Description
        * --------------------------------------------------------------------------
        *                 06/25/97   tb    Initial coding
        *                 07/30/03   df    C# revision
        * --------------------------------------------------------------------------
        */
        private void updateEmpDecrements()
        {
          writeToStatusBox( "Updating capacityNext table for employment decrements.." );
          sqlCommand.CommandText = String.Format(appSettings["update05"].Value, TN.capacity2, TN.empDecrements, scenarioID, bYear);
          try
          { 
            sqlConnection.Open();
            sqlCommand.ExecuteNonQuery(); 
          }
          catch(Exception e)
          {
            MessageBox.Show(e.ToString(),e.GetType().ToString());

          }  // end catch
          finally
          {
            sqlConnection.Close();
          }
        }     // End method updateEmpDecrements()

        /*****************************************************************************/

        /* method updateEmpIncrements() */
        /// <summary>
        /// Method to update the capacity table from the emp_inc table.
        /// </summary>

        /* Revision History
        * 
        * STR             Date       By    Description
        * --------------------------------------------------------------------------
        *                 06/25/97   tb    Initial coding
        *                 07/30/03   df    C# revision
        * --------------------------------------------------------------------------
        */
        private void updateEmpIncrements()
        {
          writeToStatusBox( "Updating capactityNext table for employment increments.." );
          sqlCommand.CommandText = String.Format(appSettings["update06"].Value, TN.capacity2, TN.empIncrements, scenarioID, bYear);
          
          try
          {
            sqlConnection.Open();
            sqlCommand.ExecuteNonQuery();
          }
          catch(Exception e)
          {
            MessageBox.Show(e.ToString(),e.GetType().ToString());

          }  // end catch
          finally
          {
            sqlConnection.Close();
          }

          if (fYear == 2020)
          {
            writeToStatusBox("Updating capactityNext table for mil site spec employment");
            sqlCommand.CommandText = String.Format(appSettings["update19"].Value, TN.capacity2, TN.milSiteSpec, scenarioID, bYear);
            try
            {
              sqlConnection.Open();
              sqlCommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
              MessageBox.Show(e.ToString(), e.GetType().ToString());

            }  // end catch
            finally
            {
              sqlConnection.Close();
            }
          }
        }     // End method updateEmpIncrements()

        /*****************************************************************************/

        /* method writeEmpDecrements() */
        /// <summary>
        /// Method to write updated emp decrements data to temporary capacity ASCII 
        /// table.
        /// </summary>

        /* Revision History
        * 
        * STR             Date       By    Description
        * --------------------------------------------------------------------------
        *                 06/23/97   tb    Initial coding
        *                 07/30/03   df    C# revision
        * --------------------------------------------------------------------------
        */
        private void writeEmpDecrements( TCapD t )
        {
          empCd.WriteLine(t.luz + "," + t.LCKey + "," + t.devCode + "," + t.civ + "," + t.chgCiv +"," + t.capCiv + "," + t.pCap_emp);
          empCd.Flush();
        }     // End method writeEmpDecrements()

        /*****************************************************************************/

        /* method writeEmpIncrements() */
        /// <summary>
        /// Method to write updated employment increments data to temporary capacity
        /// file.
        /// </summary>

        /* Revision History
        * 
        * STR             Date       By    Description
        * --------------------------------------------------------------------------
        *                 06/23/97   tb    Initial coding
        *                 07/30/03   df    C# revision
        * --------------------------------------------------------------------------
        */
        private void writeEmpIncrements( TCap t )
        {
          empCi.WriteLine(t.luz + "," + t.LCKey + "," + t.devCode + "," + t.lu + "," + t.civ + "," + t.capCiv + "," + t.pCap_emp + "," + t.chgCiv + "," +
              t.sf + "," + t.mf + "," + t.mh + "," + t.capSF + "," + t.capMF + "," + t.capmh + "," + t.chgSF + "," + t.chgMF + "," + t.chgmh );
          empCi.Flush();
        }     // End method writeEmpIncrements()

        /*****************************************************************************/

        #endregion Employment utilities

        private void btnExit_Click(object sender, System.EventArgs e)
        {
          Close();
        }

        private void CivEmp_Closing( object sender, System.ComponentModel.CancelEventArgs e )
        {
          caller.Visible = true;    
        }

        private void chkMinConstraints_CheckedChanged(object sender, System.EventArgs e)
        {
    
        }

    }     // End class CivEmp()
}     // End namespace udm.civemp