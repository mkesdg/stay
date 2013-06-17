/* Filename:    HousingStock.cs
 * Program:     UDM
 * Version:     7.0 sr13
 * Programmers: Terry Beckhelm
 *              Daniel Flyte (C# revision)
 * Description: This form commands all actions associated with module 3, 
 *              housing stock processing.  It is called from UDM, after 
 *              the Housing Stock button has been selected from the main form.
 * 
 * Includes procedures
 *  beginHSWork()
 *  buildBaseRatios()
 *  controlMFNoOvr()
 *  controlMFOvr()
 *  controlmhNoOvr()
 *  controlmhOvr()
 *  controlSFNoOvr()
 *  controlSFOvr()
 *  closeFiles()
 *  computeHSRates()
 *  doHSTotals1()
 *  extractHSOvr()
 *  extractImpedPM()
 *  extractLost()
 *  extractRegionalControls()
 *  getMFOutliers()
 *  getSFOutliers()
 *  getmhCap()
 *  loadDecrements()
 *  loadIncrements()
 *  mfCalc()
 *  MFDecrements()
 *  MFIncrements()
 *  mfLand()
 *  mfTansactions()
 *  mhCalc()
 *  mhDecrements()
 *  mhIncrements()
 *  mhTransactions()
 *  openFiles()
 *  printAuxHS()
 *  printHS1()
 *  printHS2()
 *  printHS3()
 *  printHS4()
 *  printHS5()
 *  printHS6()
 *  printHSOutliers()
 *  printHSOvr()
 *  printHSRates()
 *  printHSTableSpecial()
 *  prob1()
 *  prob2()
 *  processParams()
 *  redistHS()
 *  sfCalc()
 *  SFDecrements
 *  SFIncrements()
 *  sfLand()
 *  sfTransactions()
 *  updateGQ()
 *  updateMhDecrements()
 *  updateMHIncrements()
 *  updateMFDecrements() 
 *  updateNonMHIncrements()
 *  updateSFDecrements()
 *  updatecapacity()
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
  public class HousingStock : BaseForm
  {
    #region Instance fields

    private bool doHSsfOvr;           // Use HSsf overrides
    private bool doHSmfOvr;           // Use HSmf overrides
    private bool controlHSOverrides;
    private bool[] mfOvrFlags;     /* mf override flags - used only for debug printing */
    private bool[] mhOvrFlags;     /* mh override flags - used only for debug printing */
    private bool[] sfOvrFlags;     /* sf override flags - used only for debug printing */
    private int[] zbi;
    private int[] rcn;

    private double[,] allocProb;
    private int[,] impedPM;         // Impedence matrix pm
    private int[,] impedTran;       // Impedence matrix transit
    private double[,] allocAll;
    private double[,] intProb;      /* Interval probabilities for HS allocations */
    private double[] fractees;      // Percent using transit
    private double[,] cumProb;      /* Cumulative  probabilities for HS allocations */

    private StreamWriter hsOvr;           // HS overrides
    private StreamWriter hsOut;           // HS outliers output
    private StreamWriter hsChange;        // Portrait format hs allocations
    private StreamWriter hsChange1;       // Landscape format hs allocations
    private StreamWriter hsRates;         // hs vac rates and exceptions
    private StreamWriter mfCi;            /* Temp file for mf increments updates */
    private StreamWriter mfCd;
    private StreamWriter mfOut;           // Debugging file for MF calcs
    private StreamWriter mhCi;            /* Temp file for mh increments updates */
    private StreamWriter mhCd;
    private StreamWriter redisHS;         /* LUZ HS redistribution listing file */
    private StreamWriter sfCi;            /* Temp file for SF increments updates */
    private StreamWriter sfCd;
    private StreamWriter sfOut;           // Debugging file for SF calculations
    private StreamWriter hoe;

    private RegionalControls rc;
    private TCapD[,] tcapd;       /* Temporary structure for capacity query in  decrement routine */
    private TCap[,] tCapI;        /* Temporary structure for capacity query in increment routine */
    private TTP[] tt;             // Travel time parameter data structures

    private System.Windows.Forms.Button btnExit;
    private System.Windows.Forms.Button btnRun;
    private System.Windows.Forms.CheckBox chkCtrlOvr;
    private System.Windows.Forms.Label lblScenario;
    private System.Windows.Forms.ComboBox cboScenario;
    private System.Windows.Forms.Label lblYearsIncrement;
    private System.Windows.Forms.ComboBox cboYears;
    private System.Windows.Forms.CheckBox chkUseSFOvr;
    private System.Windows.Forms.CheckBox chkUseMFOvr;
    private System.Windows.Forms.Label label1;

    #endregion Instance fields

    #region Designer generated code
    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (components != null)
        {
          components.Dispose();
        }
      }
      base.Dispose(disposing);
    }

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HousingStock));
            this.btnExit = new System.Windows.Forms.Button();
            this.btnRun = new System.Windows.Forms.Button();
            this.chkCtrlOvr = new System.Windows.Forms.CheckBox();
            this.chkUseSFOvr = new System.Windows.Forms.CheckBox();
            this.lblScenario = new System.Windows.Forms.Label();
            this.cboScenario = new System.Windows.Forms.ComboBox();
            this.lblYearsIncrement = new System.Windows.Forms.Label();
            this.cboYears = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chkUseMFOvr = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // txtStatus
            // 
            this.txtStatus.Location = new System.Drawing.Point(24, 200);
            this.txtStatus.Size = new System.Drawing.Size(360, 72);
            // 
            // btnExit
            // 
            this.btnExit.BackColor = System.Drawing.Color.Red;
            this.btnExit.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExit.Location = new System.Drawing.Point(176, 288);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(96, 40);
            this.btnExit.TabIndex = 18;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = false;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnRun
            // 
            this.btnRun.BackColor = System.Drawing.Color.LightGreen;
            this.btnRun.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRun.Location = new System.Drawing.Point(56, 288);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(96, 40);
            this.btnRun.TabIndex = 17;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = false;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // chkCtrlOvr
            // 
            this.chkCtrlOvr.Checked = true;
            this.chkCtrlOvr.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCtrlOvr.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkCtrlOvr.Location = new System.Drawing.Point(24, 168);
            this.chkCtrlOvr.Name = "chkCtrlOvr";
            this.chkCtrlOvr.Size = new System.Drawing.Size(272, 24);
            this.chkCtrlOvr.TabIndex = 16;
            this.chkCtrlOvr.Text = "Include Overrides in Controlling";
            // 
            // chkUseSFOvr
            // 
            this.chkUseSFOvr.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkUseSFOvr.Location = new System.Drawing.Point(195, 138);
            this.chkUseSFOvr.Name = "chkUseSFOvr";
            this.chkUseSFOvr.Size = new System.Drawing.Size(189, 24);
            this.chkUseSFOvr.TabIndex = 15;
            this.chkUseSFOvr.Text = "Use SF overrides";
            // 
            // lblScenario
            // 
            this.lblScenario.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblScenario.Location = new System.Drawing.Point(168, 72);
            this.lblScenario.Name = "lblScenario";
            this.lblScenario.Size = new System.Drawing.Size(72, 16);
            this.lblScenario.TabIndex = 14;
            this.lblScenario.Text = "Scenario";
            // 
            // cboScenario
            // 
            this.cboScenario.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboScenario.Items.AddRange(new object[] {
            "0 - EP"});
            this.cboScenario.Location = new System.Drawing.Point(168, 96);
            this.cboScenario.Name = "cboScenario";
            this.cboScenario.Size = new System.Drawing.Size(64, 24);
            this.cboScenario.TabIndex = 13;
            // 
            // lblYearsIncrement
            // 
            this.lblYearsIncrement.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblYearsIncrement.Location = new System.Drawing.Point(24, 72);
            this.lblYearsIncrement.Name = "lblYearsIncrement";
            this.lblYearsIncrement.Size = new System.Drawing.Size(120, 16);
            this.lblYearsIncrement.TabIndex = 12;
            this.lblYearsIncrement.Text = "Increment";
            // 
            // cboYears
            // 
            this.cboYears.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboYears.Items.AddRange(new object[] {
            "2012 - 2020",
            "2020 - 2035",
            "2035 - 2050"});
            this.cboYears.Location = new System.Drawing.Point(24, 96);
            this.cboYears.Name = "cboYears";
            this.cboYears.Size = new System.Drawing.Size(120, 24);
            this.cboYears.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Garamond", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Navy;
            this.label1.Location = new System.Drawing.Point(8, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(192, 32);
            this.label1.TabIndex = 10;
            this.label1.Text = "HS Allocation";
            // 
            // chkUseMFOvr
            // 
            this.chkUseMFOvr.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkUseMFOvr.Location = new System.Drawing.Point(24, 138);
            this.chkUseMFOvr.Name = "chkUseMFOvr";
            this.chkUseMFOvr.Size = new System.Drawing.Size(165, 24);
            this.chkUseMFOvr.TabIndex = 19;
            this.chkUseMFOvr.Text = "Use MF overrides";
            // 
            // HousingStock
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(458, 352);
            this.Controls.Add(this.chkUseMFOvr);
            this.Controls.Add(this.chkCtrlOvr);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.chkUseSFOvr);
            this.Controls.Add(this.lblScenario);
            this.Controls.Add(this.cboScenario);
            this.Controls.Add(this.lblYearsIncrement);
            this.Controls.Add(this.cboYears);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "HousingStock";
            this.Text = "HS Allocation";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.HousingStock_Closing);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.cboYears, 0);
            this.Controls.SetChildIndex(this.lblYearsIncrement, 0);
            this.Controls.SetChildIndex(this.cboScenario, 0);
            this.Controls.SetChildIndex(this.lblScenario, 0);
            this.Controls.SetChildIndex(this.chkUseSFOvr, 0);
            this.Controls.SetChildIndex(this.btnRun, 0);
            this.Controls.SetChildIndex(this.btnExit, 0);
            this.Controls.SetChildIndex(this.txtStatus, 0);
            this.Controls.SetChildIndex(this.chkCtrlOvr, 0);
            this.Controls.SetChildIndex(this.chkUseMFOvr, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

    }
    #endregion

    public HousingStock(Form form)
    {
      InitializeComponent();
      //writeToStatusBox( "Awaiting user input.." );
      caller = form;
    }
    /*****************************************************************************/

    /* method btnRun_Click() */
    /// <summary>
    /// Method to begin run button processing.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 07/02/97   tb    Initial coding
    *                 08/01/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    private void btnRun_Click(object sender, System.EventArgs e)
    {
      if (!processParams())
        return;
      MethodInvoker mi = new MethodInvoker(beginHSWork);
      mi.BeginInvoke(null, null);
    }     // End method btnRun_Click()


    // Slow(er) method that will run on its own thread from the thread pool.
    private void beginHSWork()
    {
        int i;
        // -----------------------------------------------------------------

        if (!UDMUtils.checkCapacity(this, 2, scenarioID, bYear))
        {
            MessageBox.Show("Fatal Error! HS_SF forecast exceeds capacity.", "Fatal Error");
        }   // end if

        if (!UDMUtils.checkCapacity(this, 3, scenarioID, bYear))
        {
            MessageBox.Show("Fatal Error! HS_MF forecast exceeds capacity.", "Fatal Error");
        }   // end if

        openFiles();          // Open all of the input and output files

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
     
        // Load the LUZ emp and HS history array
        if (!UDMUtils.extractHistory(this, scenarioID, bYear))
            Close();

        if (!UDMUtils.extractLUZTemp(this, scenarioID, bYear))
            Close();

        if (!UDMUtils.copyCapacity(this, 3,scenarioID, bYear,fYear))
            Close();

        // Load the base data
        UDMUtils.extractLUZBase(this, zbi, scenarioID, bYear);
        buildBaseRatios();

        // Extract lost units
        extractLost();

        // Load regional controls 
        extractRegionalControls();

        // Download pm impedence matrix 
        extractImpedPM();

        // Load overrides 
        extractHSOvr(doHSsfOvr, doHSmfOvr);

        // Build HS rates
        computeHSRates();

        // Do the calculations
        prob1();

        // MF first
        mfCalc();

        for (i = 0; i < NUM_LUZS; i++)
        {
            if (z[i].mfOvr)      // Are there any overrides 
                z[i].fcst.hsi.mfAdj = z[i].ho.mf;
        }   // end for i

        if (controlHSOverrides)     // If overrides are controlled 
            controlMFOvr();
        else
            controlMFNoOvr();

        redistHS(2);      // MF
        doHSTotals1(1);
        mfLand();
        mfTransactions();

        // SF
        sfCalc();
        for (i = 0; i < NUM_LUZS; i++)
        {
            if (z[i].sfOvr)      // Are there any overrides
                z[i].fcst.hsi.sfAdj = z[i].ho.sf;
        }     // end for i

        if (controlHSOverrides)      // If overrides are controlled
            controlSFOvr();
        else
            controlSFNoOvr();

        redistHS(1);      // SF 
        doHSTotals1(2);
        sfLand();
        sfTransactions();

        // mh 
        mhCalc();
        for (i = 0; i < NUM_LUZS; i++)
        {
            if (z[i].mhOvr)      // If there are there any overrides
            z[i].fcst.hsi.mhAdj = z[i].ho.mh;
        }   // end for i

        if (controlHSOverrides)      // If overrides are controlled
            controlmhOvr();
        else
            controlmhNoOvr();

        mhTransactions();
        updateGQ();

        updatecapacity(TN.capacity3);
        // LUZ and regional totals
        doHSTotals1(3);

        // Process outliers
        getMFOutliers();
        getSFOutliers();

        // Write auxillary reports to ASCII
        flags.housingChange = true;
        printAuxHS();
        closeFiles();
        writeToStatusBox("Housing Stock Allocation Completed");
         MessageBox.Show("Housing Stock Allocation Completed");

    }   // end beginHSWork()

    // *************************************************************************

    /* method buildBaseRatios() */
    /// <summary>
    /// Method to create base emp and hh ratios for employment equation.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 05/27/97   tb    Initial coding
    *                 07/25/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    void buildBaseRatios()
    {
      int i;
      int rege = 0;
      int regh = 0;
      for (i = 0; i < NUM_LUZS; i++)
      {
        // Compute regional totals for hh and base civ emp
        rege += z[i].baseData.e.civ;
        regh += z[i].histHH.L0;
      }   // end for i

      for (i = 0; i < NUM_LUZS; i++)
      {
        // Compute ratios for hh and base civ emp
        if (rege > 0)
          z[i].fcst.civR = (double)z[i].baseData.e.civ / (double)rege;
        if (regh > 0)
          z[i].fcst.hhR = (double)z[i].histHH.L0 / (double)regh;
      }   // end for i      
    }     // End method buildBaseRatios()

    /*****************************************************************************/

    #region Control Procs

    /* method controlMFNoOvr() */
    /// <summary>
    /// Method to control the MF housing stock forecast excluding overrides.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 07/16/97   tb    Initial coding
    *                 08/05/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    void controlMFNoOvr()
    {
      bool trans = false;     // Reset translation flag
      bool[] oFlag = new bool[NUM_LUZS];
      int i, diff = 0, ovrTotal = 0, adjRegTotal, ret;
      int[] pt = new int[NUM_LUZS], ubound = new int[NUM_LUZS];

      double abSum = 0, summ = 0;
      double posAdj = 1.0;
      double negAdj = 1.0;
      // -------------------------------------------------------------
      //writeToStatusBox( "Controlling MF with overrides excluded.." );

      // Build accumulators
      for (i = 0; i < NUM_LUZS; i++)
      {
        if (!z[i].mfOvr)    // If not overridden
        {
          abSum += Math.Abs(z[i].fcst.hsi.mfAdj);
          summ += z[i].fcst.hsi.mfAdj;
        }  // end if

        else      // Compute the total of the overridden LUZs
          ovrTotal += z[i].ho.mf;
      }   // end for i

      // Compute regional total adjusted for overrides
      adjRegTotal = rc.fcst.hsi.mfAdj - ovrTotal;

      /* If the adjusted regional total = 0, then all LUZs are overridden -
      * return, since none of the following logic will work. */
      if (adjRegTotal == 0)
        return;

      if (abSum > 0)
      {
        // Adjustment for LUZs with employment growth
        posAdj = (abSum + (adjRegTotal - summ)) / abSum;
        // Adjustment for LUZs with negative growth
        negAdj = (abSum - (adjRegTotal - summ)) / abSum;
      }  //end if

      /* Check negAdj for < 0 and compute translation to get it to be at least 0.10 */
      if (negAdj < 0)
      {
        trans = true;
        diff = (int)(0.5 + ((double)(.9 * abSum + summ - adjRegTotal)) / NUM_LUZS);
        abSum = 0;
        summ = 0;
        for (i = 0; i < NUM_LUZS; i++)
        {
          if (!z[i].mfOvr)
          {
            z[i].fcst.hsi.mfAdj += diff;
            abSum += Math.Abs(z[i].fcst.hsi.mfAdj);
            summ += z[i].fcst.hsi.mfAdj;
          }   // end if
        }   // end for i

        // Recompute adjustments
        // Adjustment for LUZs with employment growth
        if (abSum > 0)
          posAdj = (abSum + (adjRegTotal - summ)) / abSum;

        // Adjustment for LUZs with negative growth
        if (abSum > 0)
          negAdj = (abSum - (adjRegTotal - summ)) / abSum;
      }     // End if

      for (i = 0; i < NUM_LUZS; i++)
      {
        if (z[i].fcst.hsi.mfAdj > 0 && !z[i].mfOvr)
          z[i].fcst.hsi.mfAdj = (int)(z[i].fcst.hsi.mfAdj * posAdj);
        else if (z[i].fcst.hsi.mfAdj < 0 && !z[i].mfOvr)
          z[i].fcst.hsi.mfAdj = (int)(z[i].fcst.hsi.mfAdj * negAdj);
        if (trans && !z[i].mfOvr)
          z[i].fcst.hsi.mfAdj -= diff;      // Adjust for translation if any
      }   // end for i

      // Now use the + - roundoff to match the regional totals
      for (i = 0; i < NUM_LUZS; i++)
      {
        pt[i] = z[i].fcst.hsi.mfAdj;            // Pass the increment
        oFlag[i] = z[i].mfOvr;                  /* Pass the override flag for control */
        ubound[i] = z[i].capacity.totalMF;      // Pass capacity as control
      }   // end for i

      ret = UDMUtils.roundItUpperLimit(pt, oFlag, ubound, adjRegTotal, NUM_LUZS);
      if (ret > 0)
        MessageBox.Show("controlMFNoOvr rounditUpperLimit didn't converge, difference = " + ret);

      //mfOut.WriteLine( "MF AFTER CONTROLLING" );
      // Restore the rounded values
      for (i = 0; i < NUM_LUZS; i++)
      {
        z[i].fcst.hsi.mfAdj = pt[i];
       
      }   // end for i
      //mfOut.Close();
    }     // End procedure controlMFNoOvr()

    /*****************************************************************************/

    /* method controlMFOvr() */
    /// <summary>
    /// Method to control the MF HS forecast with overrides included.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 07/16/97   tb    Initial coding
    *                 08/05/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    void controlMFOvr()
    {
      bool trans = false; ;     // Reset translation flag
      int i, diff = 0, ret;
      int[] pt = new int[NUM_LUZS];
      int[] ubound = new int[NUM_LUZS];

      double abSum = 0, summ = 0;
      double posAdj = 1.0;
      double negAdj = 1.0;
      // ----------------------------------------------------------------

      //writeToStatusBox( "Controlling MF with overrides included.." );

      // Build accumulators
      for (i = 0; i < NUM_LUZS; i++)
      {
        abSum += Math.Abs(z[i].fcst.hsi.mfAdj);
        summ += z[i].fcst.hsi.mfAdj;
      }   // end for i

      // Adjustment for LUZS with mf growth
      if (abSum > 0)
        posAdj = (abSum + (rc.fcst.hsi.mfAdj - summ)) / abSum;

      // Adjustment for LUZS with negative growth
      if (abSum > 0)
        negAdj = (abSum - (rc.fcst.hsi.mfAdj - summ)) / abSum;

      /* Check negAdj for < 0 and compute translation to get it to be at 
      * least 0.10 */
      if (negAdj < 0)
      {
        trans = true;
        diff = (int)(0.5 + ((double)(0.9 * abSum + summ - rc.fcst.hsi.mfAdj)) / NUM_LUZS);
        abSum = 0;
        summ = 0;

        for (i = 0; i < NUM_LUZS; i++)
        {
          z[i].fcst.hsi.mfAdj += diff;
          abSum += Math.Abs(z[i].fcst.hsi.mfAdj);
          summ += z[i].fcst.hsi.mfAdj;
        }   // end for i

        // Recompute adjustments

        // Adjustment for LUZS with employment growth
        if (abSum > 0)
          posAdj = (abSum + (rc.fcst.hsi.mfAdj - summ)) / abSum;

        // Adjustment for LUZS with negative growth
        if (abSum > 0)
          negAdj = (abSum - (rc.fcst.hsi.mfAdj - summ)) / abSum;
      }     // End if negAdj

      for (i = 0; i < NUM_LUZS; i++)
      {
        if (z[i].fcst.hsi.mfAdj > 0)
          z[i].fcst.hsi.mfAdj = (int)(z[i].fcst.hsi.mfAdj * posAdj);
        else if (z[i].fcst.hsi.mfAdj < 0)
          z[i].fcst.hsi.mfAdj = (int)(z[i].fcst.hsi.mfAdj * negAdj);
        if (trans)
          z[i].fcst.hsi.mfAdj -= diff;      // Adjust for translation if any
      }   //  end for i

      // Now use the + - roundoff to match the regional totals
      for (i = 0; i < NUM_LUZS; i++)
      {
        pt[i] = z[i].fcst.hsi.mfAdj;            /* Save the increment in array for passing */
        ubound[i] = z[i].capacity.totalMF;      // Pass capacity as control
      }

      ret = UDMUtils.roundIt(pt, ubound, rc.fcst.hsi.mfAdj, NUM_LUZS, 0);
      if (ret > 0)
        MessageBox.Show("controlMFOvr roundIt didn't converge, difference = " + ret);

      // Restore the rounded values
      for (i = 0; i < NUM_LUZS; i++)
        z[i].fcst.hsi.mfAdj = pt[i];

    }     // End method controlMFOvr()*/

    /*****************************************************************************/

    /* method controlmhNoOvr()*/
    /// <summary>
    /// Method to control the mh HS forecast excluding overrides.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 07/16/97   tb    Initial coding
    *                 08/06/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    void controlmhNoOvr()
    {
      bool trans = false;
      bool[] oFlag = new bool[NUM_LUZS];
      int diff = 0, ovrTotal = 0, adjRegTotal, ret, i;
      int[] pt = new int[NUM_LUZS], ubound = new int[NUM_LUZS];
      double abSum = 0, summ = 0, posAdj = 0, negAdj = 0;
      // ---------------------------------------------------------------------
      //writeToStatusBox( "Controlling mh with overrides excluded.." );

      // Build accumulators
      for (i = 0; i < NUM_LUZS; i++)
      {
        if (!z[i].mhOvr)     // If not overridden
        {
          abSum += Math.Abs(z[i].fcst.hsi.mhAdj);
          summ += z[i].fcst.hsi.mhAdj;
        }   // end if

        else      // Compute the total of the overriden LUZs
          ovrTotal += z[i].ho.mh;
      }   // end for i

      // Compute regional total adjusted for overrides
      adjRegTotal = rc.fcst.hsi.mhAdj - ovrTotal;

      // Adjustment for LUZs with employment growth
      if (abSum > 0)
        posAdj = (abSum + (adjRegTotal - summ)) / abSum;

      // Adjustment for LUZs with negative growth
      if (abSum > 0)
        negAdj = (abSum - (adjRegTotal - summ)) / abSum;

      /* Check negAdj for < 0 and compute translation to get it to be at 
      * least 0.10 */
      if (negAdj < 0)
      {
        trans = true;
        diff = (int)(0.5 + ((double)(0.9 * abSum + summ -
            adjRegTotal)) / NUM_LUZS);
        abSum = 0;
        summ = 0;
        for (i = 0; i < NUM_LUZS; i++)
        {
          if (!z[i].mhOvr)
          {
            z[i].fcst.hsi.mhAdj += diff;
            abSum += Math.Abs(z[i].fcst.hsi.mhAdj);
            summ += z[i].fcst.hsi.mhAdj;
          }   // end if
        }    // end for i  
        // Recompute adjustments
        if (abSum > 0)
        {
          // Adjustment for LUZs with employment growth
          posAdj = (abSum + (adjRegTotal - summ)) / abSum;
          // Adjustment for LUZs with negative growth
          negAdj = (abSum - (adjRegTotal - summ)) / abSum;
        }   // end if
      }     // End if

      for (i = 0; i < NUM_LUZS; i++)
      {
        if (z[i].fcst.hsi.mhAdj > 0 && !z[i].mhOvr)
          z[i].fcst.hsi.mhAdj = (int)(z[i].fcst.hsi.mhAdj * posAdj);
        else if (z[i].fcst.hsi.mhAdj < 0 && !z[i].mhOvr)
          z[i].fcst.hsi.mhAdj = (int)(z[i].fcst.hsi.mhAdj * negAdj);
        if (trans && !z[i].mhOvr)
          z[i].fcst.hsi.mhAdj -= diff;      // Adjust for translation if any
      }   // end for i

      // Now use the +/- roundoff to match the regional totals
      for (i = 0; i < NUM_LUZS; i++)
      {
        pt[i] = z[i].fcst.hsi.mhAdj;      // Pass the increment
        oFlag[i] = z[i].mhOvr;            /* Pass the override flag for control */
        ubound[i] = 9999;                 /* mh have no capacities - use dummy capacity as control */
      }   // end for i
      ret = UDMUtils.roundItUpperLimit(pt, oFlag, ubound, adjRegTotal, NUM_LUZS);
      if (ret > 0)
        MessageBox.Show("controlmhNoOvr roundItUpperLimit did not converge - diference = " + ret);
      // Restore the rounded values
      for (i = 0; i < NUM_LUZS; i++)
        z[i].fcst.hsi.mhAdj = pt[i];
    }     // End method controlmhNoOvr()

    /*****************************************************************************/

    /* method controlmhOvr() */
    /// <summary>
    /// Method to control the mh HS forecast with overrides included.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 07/16/97   tb    Initial coding
    *                 08/06/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    void controlmhOvr()
    {
      bool trans = false;
      int diff = 0, ret, i;
      int[] pt = new int[NUM_LUZS];
      int[] bd = new int[NUM_LUZS];

      double abSum = 0, summ = 0, posAdj = 0, negAdj = 0;
      // -----------------------------------------------------------------------  
      //writeToStatusBox( "Controlling mh with overrides included.." );

      // Build accumulators
      for (i = 0; i < NUM_LUZS; i++)
      {
        abSum += Math.Abs(z[i].fcst.hsi.mhAdj);
        summ += z[i].fcst.hsi.mhAdj;
      }   // end for i

      // Adjustment for LUZs with mh growth
      if (abSum > 0)
        posAdj = (abSum + (rc.fcst.hsi.mhAdj - summ)) / abSum;

      // Adjustment for LUZs with negative growth
      if (abSum > 0)
        negAdj = (abSum - (rc.fcst.hsi.mhAdj - summ)) / abSum;

      /* Check negAdj for < 0 and compute translation to get it to be at least 0.10 */
      if (negAdj < 0)
      {
        trans = true;
        diff = (int)(0.5 + ((double)(0.9 * abSum + summ - rc.fcst.hsi.mhAdj)) / NUM_LUZS);
        abSum = 0;
        summ = 0;

        for (i = 0; i < NUM_LUZS; i++)
        {
          z[i].fcst.hsi.mhAdj += diff;
          abSum += Math.Abs(z[i].fcst.hsi.mhAdj);
          summ += z[i].fcst.hsi.mhAdj;
        }   // end for i

        // Recompute adjustments

        if (abSum > 0)
        {
          // Adjustment for LUZs with employment growth
          posAdj = (abSum + (rc.fcst.hsi.mhAdj - summ)) / abSum;
          // Adjustment for LUZs with negative growth
          negAdj = (abSum - (rc.fcst.hsi.mhAdj - summ)) / abSum;
        }   // end if
      }     // End if negAdj

      for (i = 0; i < NUM_LUZS; i++)
      {
        if (z[i].fcst.hsi.mhAdj > 0)
          z[i].fcst.hsi.mhAdj = (int)(z[i].fcst.hsi.mhAdj * posAdj);
        else if (z[i].fcst.hsi.mhAdj < 0)
          z[i].fcst.hsi.mhAdj = (int)(z[i].fcst.hsi.mhAdj * negAdj);
        if (trans)
          z[i].fcst.hsi.mhAdj -= diff;      // Adjust for translation if any
      }   // end for i

      // Now use the + - roundoff to match the regional totals
      for (i = 0; i < NUM_LUZS; i++)
      {
        pt[i] = z[i].fcst.hsi.mhAdj;      /* Save the increment in array for passing */
        bd[i] = z[i].baseData.hs.mh;
      }  // end for i

      ret = UDMUtils.roundItNeg(pt, bd,rc.fcst.hsi.mhAdj, NUM_LUZS);
      if (ret != 0)
        MessageBox.Show("controlmhOvr roundItNoLimit did not converge, difference = " + ret);

      // Restore the rounded values
      for (i = 0; i < NUM_LUZS; i++)
        z[i].fcst.hsi.mhAdj = pt[i];
    }     // End method controlmhOvr()

    /*****************************************************************************/

    /* method controlSFNoOvr() */
    /// <summary>
    /// Method to control the SF HS forecast excluding overrides.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 07/16/97   tb    Initial coding
    *                 08/05/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    void controlSFNoOvr()
    {
      bool trans = false;
      bool[] oFlag = new bool[NUM_LUZS];
      int i, diff = 0, ovrTotal = 0, adjRegTotal, ret;
      int[] pt = new int[NUM_LUZS], ubound = new int[NUM_LUZS];

      double abSum = 0, summ = 0;
      double posAdj = 1;
      double negAdj = 1;
      // ----------------------------------------------------------------------   
      //writeToStatusBox( "Controlling SF with overrides excluded.." );

      // Build accumulators
      for (i = 0; i < NUM_LUZS; i++)
      {
        if (!z[i].sfOvr)     // If not overridden
        {
          abSum += Math.Abs(z[i].fcst.hsi.sfAdj);
          summ += z[i].fcst.hsi.sfAdj;
        }   // end if

        else      // Compute the total of the overriden LUZs
          ovrTotal += z[i].ho.sf;
      }   // end for i

      // Compute regional total adjusted for overrides
      adjRegTotal = rc.fcst.hsi.sfAdj - ovrTotal;

      /* If the adjusted regional total = 0, then all LUZs are overridden - 
      * return, as none of the following logic will work. */
      if (adjRegTotal == 0)
        return;

      if (abSum > 0)
      {
        // Adjustment for LUZs with employment growth
        posAdj = (abSum + (adjRegTotal - summ)) / abSum;
        // Adjustment for LUZs with negative growth
        negAdj = (abSum - (adjRegTotal - summ)) / abSum;
      }   // end if

      /* Check negAdj for < 0 and compute translation to get it to be at 
      * least 0.10 */
      if (negAdj < 0)
      {
        trans = true;
        diff = (int)(0.5 + ((double)(0.9 * abSum + summ - adjRegTotal)) / NUM_LUZS);
        abSum = 0;
        summ = 0;
        for (i = 0; i < NUM_LUZS; i++)
        {
          if (!z[i].sfOvr)
          {
            z[i].fcst.hsi.sfAdj += diff;
            abSum += Math.Abs(z[i].fcst.hsi.sfAdj);
            summ += z[i].fcst.hsi.sfAdj;
          }   // end if
        }   // end for i

        // Recompute adjustments
        if (abSum > 0)
        {
          // Adjustment for LUZs with employment growth
          posAdj = (abSum + (adjRegTotal - summ)) / abSum;
          // Adjustment for LUZs with negative growth
          negAdj = (abSum - (adjRegTotal - summ)) / abSum;
        }   // end if
      }   // end if

      for (i = 0; i < NUM_LUZS; i++)
      {
        if (z[i].fcst.hsi.sfAdj > 0 && !z[i].sfOvr)
          z[i].fcst.hsi.sfAdj = (int)(z[i].fcst.hsi.sfAdj * posAdj);
        else if (z[i].fcst.hsi.sfAdj < 0 && !z[i].sfOvr)
          z[i].fcst.hsi.sfAdj = (int)(z[i].fcst.hsi.sfAdj * negAdj);
        if (trans && !z[i].sfOvr)
          z[i].fcst.hsi.sfAdj -= diff;      // Adjust for translation if any
      }   // end for i


      // Now use the + - roundoff to match the regional totals
      for (i = 0; i < NUM_LUZS; i++)
      {
        pt[i] = z[i].fcst.hsi.sfAdj;      // Pass the increment
        oFlag[i] = z[i].sfOvr;            // Pass the override flag for control
        ubound[i] = z[i].capacity.totalSF;      // Pass capacity as control
      }   // end for i

      ret = UDMUtils.roundItUpperLimit(pt, oFlag, ubound, adjRegTotal, NUM_LUZS);
      if (ret > 0)
        MessageBox.Show("controlSFNoOvr roundItUpperLimit didn't converge, difference = " + ret);

      // Restore the rounded values
      for (i = 0; i < NUM_LUZS; i++)
        z[i].fcst.hsi.sfAdj = pt[i];

      sfOut.Close();
    }     // End method controlSFNoOvr()

    /*****************************************************************************/

    /* method controlSFOvr() */
    /// <summary>
    /// Method to control the SF HS forecast with overrides included.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 07/16/97   tb    Initial coding
    *                 08/05/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    void controlSFOvr()
    {
      bool trans = false;
      int i, diff = 0, ret;
      int[] pt = new int[NUM_LUZS];
      int[] ubound = new int[NUM_LUZS];

      double abSum = 0, summ = 0;
      double posAdj = 1;
      double negAdj = 1;
      // ---------------------------------------------------------------------
      //writeToStatusBox( "Controlling SF with overrides included.." );

      // Build accumulators
      for (i = 0; i < NUM_LUZS; i++)
      {
        abSum += Math.Abs(z[i].fcst.hsi.sfAdj);
        summ += z[i].fcst.hsi.sfAdj;
      }   // end for i

      if (abSum > 0)
      {
        // Adjustment for LUZs with SF growth
        posAdj = (abSum + (rc.fcst.hsi.sfAdj - summ)) / abSum;
        // Adjustment for LUZs with negative growth
        negAdj = (abSum - (rc.fcst.hsi.sfAdj - summ)) / abSum;
      }   // end if

      /* Check negAdj for < 0 and compute translation to get it to be at least 0.10 */
      if (negAdj < 0)
      {
        trans = true;
        diff = (int)(0.5 + ((double)(.9 * abSum + summ - rc.fcst.hsi.sfAdj)) / NUM_LUZS);
        abSum = 0;
        summ = 0;

        for (i = 0; i < NUM_LUZS; i++)
        {
          z[i].fcst.hsi.sfAdj += diff;
          abSum += Math.Abs(z[i].fcst.hsi.sfAdj);
          summ += z[i].fcst.hsi.sfAdj;
        }   // end if

        // Recompute adjustments
        if (abSum > 0)
        {
          // Adjustment for LUZs with employment growth
          posAdj = (abSum + (rc.fcst.hsi.sfAdj - summ)) / abSum;
          // Adjustment for LUZs with negative growth
          negAdj = (abSum - (rc.fcst.hsi.sfAdj - summ)) / abSum;
        }   // end if
      }     // End if

      for (i = 0; i < NUM_LUZS; ++i)
      {
        if (z[i].fcst.hsi.sfAdj > 0)
          z[i].fcst.hsi.sfAdj = (int)(z[i].fcst.hsi.sfAdj * posAdj);
        else if (z[i].fcst.hsi.sfAdj < 0)
          z[i].fcst.hsi.sfAdj = (int)(z[i].fcst.hsi.sfAdj * negAdj);
        if (trans)
          z[i].fcst.hsi.sfAdj -= diff;      // Adjust for translation if any
      }   // end for i

      // Now use the + - roundoff to match the regional totals
      for (i = 0; i < NUM_LUZS; i++)
      {
        pt[i] = z[i].fcst.hsi.sfAdj;            /* Save the increment in array for passing */
        ubound[i] = z[i].capacity.totalSF;      // Pass capacity as control
      }   // end for i

      ret = UDMUtils.roundIt(pt, ubound, rc.fcst.hsi.sfAdj, NUM_LUZS, 0);
      if (ret > 0)
        MessageBox.Show("controlSFOvr roundit did not converge difference = " + ret);
      // Restore the rounded values
      for (i = 0; i < NUM_LUZS; i++)
        z[i].fcst.hsi.sfAdj = pt[i];
    }     // End method controlSFOvr()

    /*****************************************************************************/

    #endregion Control Procs

    #region Close files
    /* method closeFiles() */
    /// <summary>
    /// Method to close all HS ASCII output files.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 07/07/97   tb    Initial coding
    *                 08/05/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    void closeFiles()
    {
      try
      {
        hsChange.Close();
        hsChange1.Close();
        hsRates.Close();
        hsOut.Close();
        hsOvr.Close();
        redisHS.Close();
      }
      catch (IOException)
      {
        MessageBox.Show("Error closing files.");
        Close();
      }
    }     // End method closeFiles()

    /*****************************************************************************/
    #endregion Close Files

    #region computeHSRates

    /* method computeHSRates() */
    /// <summary>
    /// Method to compute vacancy rates, er/hh and hhs.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 07/09/97   tb    Initial coding
    *                 02/22/02   tb    Put in constraint for vacancy rate 
    *                                  adjusted by regional change
    *                 08/05/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    void computeHSRates()
    {
      double vFactorSF = 0, vFactorMF = 0, vFactormh = 0, erFactor = 0,
              hhsFactor = 0;
      int i;
      // ---------------------------------------------------------------------
      //writeToStatusBox( "Computing regional vacancy er/hh and pph.." );

      // Regional rates

      // Base year rates
      if (reg.baseData.hh.total > 0)
      {
        /* base year er/hh and hhs */
        reg.baseData.r.erHH = (double)reg.baseData.p.er / (double)reg.baseData.hh.total;
        reg.baseData.r.hhs = (double)reg.baseData.p.hhp / (double)reg.baseData.hh.total;
      }   // end if

      // Vacancy rates

      // Base year total vacancy rate
      if (reg.baseData.hs.total > 0)
        reg.baseData.r.v = (1 - (double)reg.baseData.hh.total / (double)reg.baseData.hs.total) * 100;

      // Base year vacancy by structure type
      if (reg.baseData.hs.sf > 0)
        reg.baseData.r.vSF = (1 - (double)reg.baseData.hh.sf / (double)reg.baseData.hs.sf) * 100;

      if (reg.baseData.hs.mf > 0)
        reg.baseData.r.vMF = (1 - (double)reg.baseData.hh.mf / (double)reg.baseData.hs.mf) * 100;

      if (reg.baseData.hs.mh > 0)
        reg.baseData.r.vmh = (1 - (double)reg.baseData.hh.mh / (double)reg.baseData.hs.mh) * 100;

      // Forecast year rates
      if (rc.fcst.hh.total > 0)
      {
        // Forecast year er/hh and hhs
        reg.fcst.r.erHH = (double)rc.fcst.p.er / (double)rc.fcst.hh.total;
        reg.fcst.r.hhs = (double)rc.fcst.p.hhp / (double)rc.fcst.hh.total;
      }   // end if

      // Compute regional change factors
      if (reg.baseData.r.erHH > 0)
        erFactor = reg.fcst.r.erHH / reg.baseData.r.erHH;

      if (reg.baseData.r.hhs > 0)
        hhsFactor = reg.fcst.r.hhs / reg.baseData.r.hhs - 1;

      // Total vac rate
      if (rc.fcst.hs.total > 0)
        reg.fcst.r.v = (1 - (double)rc.fcst.hh.total / (double)rc.fcst.hs.total) * 100;

      // Structure types
      if (rc.fcst.hs.sf > 0)
        reg.fcst.r.vSF = (1 - (double)rc.fcst.hh.sf / (double)rc.fcst.hs.sf) * 100;
      if (rc.fcst.hs.mf > 0)
        reg.fcst.r.vMF = (1 - (double)rc.fcst.hh.mf / (double)rc.fcst.hs.mf) * 100;
      if (rc.fcst.hs.mh > 0)
        reg.fcst.r.vmh = (1 - (double)rc.fcst.hh.mh / (double)rc.fcst.hs.mh) * 100;

      // Regional change rates for vac rates by structure
      if (reg.baseData.r.vSF > 0)
        vFactorSF = reg.fcst.r.vSF / reg.baseData.r.vSF;

      if (reg.baseData.r.vMF > 0)
        vFactorMF = reg.fcst.r.vMF / reg.baseData.r.vMF;

      if (reg.baseData.r.vmh > 0)
        vFactormh = reg.fcst.r.vmh / reg.baseData.r.vmh;

      // LUZ rates

      //writeToStatusBox( "Computing LUZ vacancy er/hh and hhs" );
      for (i = 0; i < NUM_LUZS; i++)
      {
        // Base year rates
        if (z[i].baseData.hh.total > 0)
        {
          // Base year er/hh and hhs
          z[i].baseData.r.erHH = (double)z[i].baseData.p.er / (double)z[i].baseData.hh.total;
          z[i].baseData.r.hhs = (double)z[i].baseData.p.hhp / (double)z[i].baseData.hh.total;
        }   // end if

        // Vacancy rates
        // Base year total vacancy rate
        if (z[i].baseData.hs.total > 0)
          z[i].baseData.r.v = (1 - (double)z[i].baseData.hh.total / (double)z[i].baseData.hs.total) * 100;

        // Base year vacancy by structure type
        if (z[i].baseData.hs.sf > 0)
          z[i].baseData.r.vSF = (1 - (double)z[i].baseData.hh.sf / (double)z[i].baseData.hs.sf) * 100;

        if (z[i].baseData.hs.mf > 0)
          z[i].baseData.r.vMF = (1 - (double)z[i].baseData.hh.mf / (double)z[i].baseData.hs.mf) * 100;
        if (z[i].baseData.hs.mh > 0)
          z[i].baseData.r.vmh = (1 - (double)z[i].baseData.hh.mh / (double)z[i].baseData.hs.mh) * 100;

        if (z[i].baseData.hs.total < 50)     // Exclude small LUZs
        {
          // Set rates = regional rates
          z[i].fcst.r.vSF = reg.fcst.r.vSF;
          z[i].fcst.r.vMF = reg.fcst.r.vMF;
          z[i].fcst.r.vmh = reg.fcst.r.vmh;
          z[i].fcst.r.erHH = reg.fcst.r.erHH;
          z[i].fcst.r.hhs = reg.fcst.r.hhs;
          z[i].fcst.r.regOvr = true;       /* mark regional overrides used */
          continue;
        }     // end if      

        /* Forecast rates - apply regional growth factors to base year LUZ 
        * rates */
        z[i].fcst.r.vSF = z[i].baseData.r.vSF * vFactorSF;
        if (z[i].fcst.r.vSF > 50)
          z[i].fcst.r.vSF = 50;

        z[i].fcst.r.vMF = z[i].baseData.r.vMF * vFactorMF;
        if (z[i].fcst.r.vMF > 50)
          z[i].fcst.r.vMF = 50;

        z[i].fcst.r.vmh = z[i].baseData.r.vmh * vFactormh;
        if (z[i].fcst.r.vmh > 50)
          z[i].fcst.r.vmh = 50;

        // Apply regional change rate to luz xf
        z[i].fcst.r.erHH = z[i].baseData.r.erHH * erFactor;

        // Adjust household size differently for +/- changes
        if (hhsFactor >= 0)
          z[i].fcst.r.hhs = z[i].baseData.r.hhs * (1 + (hhsFactor / (z[i].baseData.r.hhs / reg.baseData.r.hhs)));
        else
          z[i].fcst.r.hhs = z[i].baseData.r.hhs * (1 + (hhsFactor * (z[i].baseData.r.hhs / reg.baseData.r.hhs)));
      }     // End for i
      printHSRates();
    }     // End procedure computeHSRates()

    /*****************************************************************************/

    #endregion computeHSRates

    #region doHSTotal

    /* method doHSTotal1() */
    /// <summary>
    /// Method to sum LUZ totals after redistribution and build regional totals.
    /// </summary>
    /// <param name="type">HS type; 1 = MF, 2 = SF, 3 = mh</param>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 05/29/97   tb    Initial coding
    *                 08/05/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    void doHSTotals1(int type)
    {
      int i;
      StreamWriter xxSF = null, xxMF = null;
      // --------------------------------------------------------------------  
      switch (type)
      {
        case 1:     // MF
          try
          {
            xxMF = new StreamWriter(new FileStream(networkPath + string.Format(appSettings["xxMF"].Value), FileMode.Create));
            xxMF.AutoFlush = true;
          }
          catch (Exception e)
          {
            MessageBox.Show(e.ToString(), "Runtime Exception");
          }
          reg.fcst.hsi.mfAdj = 0;
          reg.fcst.hsi.mf = 0;
          reg.fcst.hs.mf = 0;
          for (i = 0; i < NUM_LUZS; i++)
          {
            xxMF.WriteLine("{0},{1},{2},{3},{4}", (i + 1),z[i].fcst.hsi.mfAdj, z[i].site.mf, z[i].fcst.le.mf, z[i].fcst.lh.mf);
            z[i].fcst.hsi.mf = z[i].fcst.hsi.mfAdj + z[i].site.mf + z[i].fcst.le.mf + z[i].fcst.lh.mf;
            z[i].fcst.hs.mf = z[i].fcst.hsi.mf + z[i].baseData.hs.mf;
            reg.fcst.hsi.mf += z[i].fcst.hsi.mf;
            reg.fcst.hsi.mfAdj += z[i].fcst.hsi.mfAdj;
            reg.fcst.hs.mf += z[i].fcst.hs.mf;

            if (z[i].baseData.hs.mf != 0)
              z[i].fcst.pct.mf = (double)z[i].fcst.hsi.mf / (double)z[i].baseData.hs.mf * 100;
          }   // end for i
          xxMF.Close();
          break;

        case 2:     // SF
          try
          {
            xxSF = new StreamWriter(new FileStream(networkPath + string.Format(appSettings["xxSF"].Value), FileMode.Create));
            xxSF.AutoFlush = true;
          }
          catch (Exception e)
          {
            MessageBox.Show(e.ToString(), "Runtime Exception");
            Close();
          }
          reg.fcst.hsi.sfAdj = 0;
          reg.fcst.hsi.sf = 0;
          reg.fcst.hs.sf = 0;
          for (i = 0; i < NUM_LUZS; i++)
          {
            xxSF.WriteLine("{0},{1},{2},{3},{4}", (i + 1), z[i].fcst.hsi.sfAdj, z[i].site.sf, z[i].fcst.le.sf, z[i].fcst.lh.sf);
            z[i].fcst.hsi.sf = z[i].fcst.hsi.sfAdj + z[i].site.sf + z[i].fcst.le.sf + z[i].fcst.lh.sf;
            z[i].fcst.hs.sf = z[i].fcst.hsi.sf + z[i].baseData.hs.sf;
            reg.fcst.hsi.sf += z[i].fcst.hsi.sf;
            reg.fcst.hsi.sfAdj += z[i].fcst.hsi.sfAdj;
            reg.fcst.hs.sf += z[i].fcst.hs.sf;
            if (z[i].baseData.hs.sf != 0)
              z[i].fcst.pct.sf = (double)z[i].fcst.hsi.sf / (double)z[i].baseData.hs.sf * 100;
          } // end for i
          xxSF.Close();
          break;

        case 3:     /* mh */
          reg.fcst.hsi.mhAdj = 0;
          reg.fcst.hsi.mh = 0;
          reg.fcst.hs.mh = 0;
          for (i = 0; i < NUM_LUZS; i++)
          {
            z[i].fcst.hsi.mh = z[i].fcst.hsi.mhAdj + z[i].site.mh + z[i].fcst.le.mh + z[i].fcst.lh.mh;
            z[i].fcst.hs.mh = z[i].fcst.hsi.mh + z[i].baseData.hs.mh;
            reg.fcst.hsi.mh += z[i].fcst.hsi.mh;
            reg.fcst.hsi.mhAdj += z[i].fcst.hsi.mhAdj;
            reg.fcst.hs.mh += z[i].fcst.hs.mh;
          }   // end for i
          // Region and totals
          reg.fcst.hs.total = 0;
          reg.fcst.hsi.total = 0;

          for (i = 0; i < NUM_LUZS; i++)
          {
            // Add site spec and units lost to allocated totals
            z[i].fcst.hsi.total = z[i].fcst.hsi.sf + z[i].fcst.hsi.mf + z[i].fcst.hsi.mh;
            // Build total (levels)
            z[i].fcst.hs.total = z[i].fcst.hs.sf + z[i].fcst.hs.mf + z[i].fcst.hs.mh;
            // Regional increment
            reg.fcst.hsi.total += z[i].fcst.hsi.total;
            // Regional levels
            reg.fcst.hs.total += z[i].fcst.hs.total;
          }   // end for i
          break;
      }     // End switch
    }     // End method doHSTotals1()

    /*****************************************************************************/

    #endregion doHSTotal

    #region Extract Procs

    /* method extractHSOvr() */
    /// <summary>
    /// Method to create base emp and hh ratios for employment equation.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 05/21/97   tb    Initial coding
    *                 08/04/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    void extractHSOvr(bool doHSsfOvr, bool doHSmfOvr)
    {
      SqlDataReader rdr;
      int zi;
      int ov;
      int hoecount = 0;
      // ----------------------------------------------------------------------
      
      try
      {
          hoe = new StreamWriter(new FileStream(networkPath + String.Format(appSettings["hoe"].Value), FileMode.Create));
          hoe.AutoFlush = true;
      }
      catch (IOException e)
      {
          MessageBox.Show(e.ToString(), "IO Exception");
          Close();
      }
      // LUZ housing override struct is HO
      if (doHSsfOvr)
      {
        //writeToStatusBox( "Extracting LUZ SF overrides.." );
        sqlCommand.CommandText = String.Format(appSettings["selectAll"].Value, TN.luzSFOvr, scenarioID, bYear);
        
        try
        {
          sqlConnection.Open();
          rdr = sqlCommand.ExecuteReader();
          while (rdr.Read())
          {
            // only get luz and sf_ovr 
              // skip scenario and increment
            luzB = rdr.GetInt16(2);
            ov = rdr.GetInt32(3);

            zi = luzB - 1;
            z[zi].hOvr = true;      // Mark this LUZ as having overrides

            // Check overrides against capacity and reset with alert if greater

            // Stock - sf
            if (ov > z[zi].capacity.totalSF)
            {
                ++hoecount;
              z[zi].ho.sf = z[zi].capacity.totalSF;
              hoe.WriteLine("WARNING - Resetting sf ovr to capacity for LUZ " + luzB + " ov = " + ov + " cap = " + z[zi].capacity.totalSF);
            }
            else
              z[zi].ho.sf = ov;

            sfOvrFlags[zi] = z[zi].sfOvr = true;

            
          }     // End while
          rdr.Close();
        }     // End try

        catch (Exception e)
        {
          MessageBox.Show(e.ToString(), "Runtime Exception");
          Close();
        }
        finally
        {
          sqlConnection.Close();
        }
      }   // end if doHSsfOvr

      if (doHSmfOvr)
      {
        sqlCommand.CommandText = String.Format(appSettings["selectAll"].Value, TN.luzMFOvr, scenarioID, bYear);
       
        try
        {
          sqlConnection.Open();
          rdr = sqlCommand.ExecuteReader();
          while (rdr.Read())
          {
            // get luz and mf_ovr 
            // skip scernario and increment
            luzB = rdr.GetInt16(2);          
            ov = rdr.GetInt32(3);
            zi = luzB - 1;
            z[zi].hOvr = true;      // Mark this LUZ as having overrides

            // Stock - mf
            if (ov > z[zi].capacity.totalMF)
            {
                ++hoecount;
              z[zi].ho.mf = z[zi].capacity.totalMF;
              hoe.WriteLine("WARNING - Resetting mf ovr to capacity for LUZ " + luzB + " ov = " + ov + " cap = " + z[zi].capacity.totalMF);
            }   // end if
            else
              z[zi].ho.mf = ov;

            mfOvrFlags[zi] = z[zi].mfOvr = true;
          }     // End while
          rdr.Close();
        }     // End try

        catch (Exception e)
        {
          MessageBox.Show(e.ToString(), "Runtime Exception");
          Close();
        }
        finally
        {
          sqlConnection.Close();
        }
        hoe.Close();

        if (hoecount > 0)
            MessageBox.Show("Override exceptions count = " + hoecount);
      }   // end if doHsmfOvr
    }     // End method extractHSOvr()

    /*****************************************************************************/

    /* method extractImpedPM() */
    /// <summary>
    /// Method to download PM travel time, transit time and fractees.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 05/27/97   tb    Initial coding
    *                 07/25/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    void extractImpedPM()
    {

      SqlDataReader rdr;
      int z1, z2, imp;
      double frac;

      //writeToStatusBox( "Extracting PM impedence matrix.." );

      sqlCommand.CommandText = String.Format(appSettings["selectAll"].Value, TN.impedPM, scenarioID, bYear);
      try
      {
        sqlConnection.Open();
        rdr = sqlCommand.ExecuteReader();
        while (rdr.Read())
        {
          z1 = rdr.GetInt16(2);  // skip scenario and increment
          z2 = rdr.GetInt16(3);
          imp = rdr.GetInt16(4);
          impedPM[z1 - 1, z2 - 1] = imp;
        }   // while
        rdr.Close();

      }   // end try
      catch (Exception e)
      {
        MessageBox.Show(e.ToString(), "Runtime Exception");
        Close();
      }
      finally
      {
        sqlConnection.Close();
      }

      //writeToStatusBox( "Extracting transit impedence matrix.." );
      sqlCommand.CommandText = sqlCommand.CommandText = String.Format(appSettings["selectAll"].Value, TN.impedTran, scenarioID, bYear);
      try
      {
        sqlConnection.Open();
        rdr = sqlCommand.ExecuteReader();
        while (rdr.Read())
        {
          z1 = rdr.GetInt16(2); // skip scenario and increment
          z2 = rdr.GetInt16(3);
          imp = rdr.GetInt16(4);
          impedTran[z1 - 1, z2 - 1] = imp;
        }   // end while
        rdr.Close();
      }
      catch (Exception e)
      {
        MessageBox.Show(e.ToString(), "Runtime Exception");
        Close();
      }
      finally
      {
        sqlConnection.Close();
      }

      //writeToStatusBox( "Extracting fractees" );
      sqlCommand.CommandText = sqlCommand.CommandText = String.Format(appSettings["selectAll"].Value, TN.fractee, scenarioID, bYear);
      try
      {
        sqlConnection.Open();
        rdr = sqlCommand.ExecuteReader();
        while (rdr.Read())
        {
          z1 = rdr.GetInt16(2); // skip increment and scenario
          frac = rdr.GetDouble(3);
          fractees[z1 - 1] = frac;
        }   // end while
        rdr.Close();
      }
      catch (Exception e)
      {
        MessageBox.Show(e.ToString(), "Runtime Exception");
        Close();
      }
      finally
      {
        sqlConnection.Close();
      }

    }     // End method extractImpedPM()

    /*****************************************************************************/

    /* method extractLost() */
    /// <summary>
    /// Method to extract units lost and employment increment for housing model.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 07/09/97   tb    Initial coding
    *                 08/04/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    void extractLost()
    {
        SqlDataReader rdr;
        int zi;
        // ----------------------------------------------------------------------

        /* Units lost from redevelopment are stored as negative numbers in increment variables. */
        writeToStatusBox("Extracting units lost and employment increment from capacityNext..");
        sqlCommand.CommandText = String.Format(appSettings["select08"].Value, TN.capacity3, scenarioID, bYear);
       
        try
        {
            sqlConnection.Open();
            rdr = sqlCommand.ExecuteReader();
            while (rdr.Read())
            {
                luzB = rdr.GetInt16(0);
                rcn[0] = rdr.GetInt32(1);
                rcn[1] = rdr.GetInt32(2);
                rcn[2] = rdr.GetInt32(3);
                rcn[3] = rdr.GetInt32(4);
                zi = luzB - 1;
                z[zi].fcst.le.sf = rcn[0];
                reg.fcst.le.sf += rcn[0];
                z[zi].fcst.le.mf += rcn[1];
                reg.fcst.le.mf += rcn[1];
                z[zi].fcst.le.mh += rcn[2];
                reg.fcst.le.mh += rcn[2];
                z[zi].fcst.ei.civ = rcn[3];
                reg.fcst.ei.civ += rcn[3];
            }   // end while
        rdr.Close();
        }
        catch (Exception e)
        {
            MessageBox.Show(e.ToString(), "Runtime Exception");
        }
        finally
        {
            sqlConnection.Close();
        }
    }     // End method extractLost()

    /*****************************************************************************/

    /* method extractRegionalControls() */
    /// <summary>
    /// Method to get regional controls for the housing model.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 07/09/97   tb    Initial coding
    *                 08/04/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    void extractRegionalControls()
    {

      SqlDataReader rdr;
      int yr, i;

      rc = new RegionalControls();
      // ----------------------------------------------------------------------

      // rcn[] holds all 42 forecast variables but we only use five here
      // Site spec data structure is site
      // Housing levels data structure is hs
      // Housing increment data structure is hsi
      writeToStatusBox("Extracting regional controls..");
      sqlCommand.CommandText = String.Format(appSettings["select19"].Value, TN.regfcst,scenarioID, bYear, fYear); // get base year year only from this table";
      
      try
      {
        sqlConnection.Open();
        rdr = sqlCommand.ExecuteReader();
        while (rdr.Read())
        {
          yr = rdr.GetInt16(0);
          rcn[0] = rdr.GetInt32(1);
          rcn[1] = rdr.GetInt32(2);
          rcn[2] = rdr.GetInt32(3);
          rcn[3] = rdr.GetInt32(4);
          rcn[4] = rdr.GetInt32(5);
          rcn[5] = rdr.GetInt32(6);
          rcn[6] = rdr.GetInt32(7);
          rcn[7] = rdr.GetInt32(8);
          rcn[8] = rdr.GetInt32(9);
          if (yr == bYear)     // Base year - skip loading hs data - these come out of the temp adjusted data table
          {
            rc.baseData.hs.sf = rcn[0];
            rc.baseData.hs.mf = rcn[1];
            rc.baseData.hs.mh = rcn[2];
            rc.baseData.hh.total = rcn[3];
            rc.baseData.hh.sf = rcn[4];
            rc.baseData.hh.mf = rcn[5];
            rc.baseData.hh.mh = rcn[6];
            rc.baseData.p.hhp = rcn[7];
            rc.baseData.p.er = rcn[8];
            //rc.baseData.hs.total = rcn[0] + rcn[1] + rcn[2];
          }   // end if
          else     // Forecast year
          {
            rc.fcst.hs.sf = rcn[0];
            rc.fcst.hs.mf = rcn[1];
            rc.fcst.hs.mh = rcn[2];
            rc.fcst.hh.total = rcn[3];
            rc.fcst.hh.sf = rcn[4];
            rc.fcst.hh.mf = rcn[5];
            rc.fcst.hh.mh = rcn[6];
            rc.fcst.p.hhp = rcn[7];
            rc.fcst.p.er = rcn[8];
            rc.fcst.hs.total = rcn[0] + rcn[1] + rcn[2];
          }   // end else
        }     // End while
        rdr.Close();
      }     // End try

      catch (Exception e)
      {
        MessageBox.Show(e.ToString(), "Runtime Exception");
        Close();
      }
      finally
      {
        sqlConnection.Close();
      }
            
      // Build regional controls from LUZ ss data
      for (i = 0; i < NUM_LUZS; i++)
      {
        rc.site.sf += z[i].site.sf;     // sf ss
        rc.site.mf += z[i].site.mf;     // mf ss
        rc.site.mh += z[i].site.mh;     // mh ss
      }   // end for i

      // Regional control increments
      rc.fcst.hsi.sf = rc.fcst.hs.sf - rc.baseData.hs.sf;
      rc.fcst.hsi.mf = rc.fcst.hs.mf - rc.baseData.hs.mf;
      rc.fcst.hsi.mh = rc.fcst.hs.mh - rc.baseData.hs.mh;
      rc.fcst.hsi.total = rc.fcst.hs.total - rc.baseData.hs.total;

      // Get adjusted hs
      rc.fcst.hsi.sfAdj = rc.fcst.hsi.sf - rc.site.sf - reg.fcst.le.sf;
      if (rc.fcst.hsi.sfAdj < 0)
      {
        MessageBox.Show("WARNING!!! - Regional adjusted hs sf increment (FOR SS) < 0");
        rc.fcst.hsi.sfAdj = 0;
      }   // end if

      rc.fcst.hsi.mfAdj = rc.fcst.hsi.mf - rc.site.mf - reg.fcst.le.mf;
      if (rc.fcst.hsi.mfAdj < 0)
      {
        MessageBox.Show("WARNING!!! - Regional adjusted hs mf increment (FOR SS) < 0");
        rc.fcst.hsi.mfAdj = 0;
      }   // end if`

      rc.fcst.hsi.mhAdj = rc.fcst.hsi.mh - rc.site.mh - reg.fcst.le.mh;
      

    }     // End method extractRegionalControls()

    /*****************************************************************************/

    /* method getmhCap() */
    /// <summary>
    /// Method to get mh capacity data for potential lost units.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 07/08/97   tb    Initial coding
    *                 08/07/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    void getmhCap()
    {

        SqlDataReader rdr;
        int z1;

        //writeToStatusBox( "Extracting mh capacity before adjusting.." );

        sqlCommand.CommandText = string.Format(appSettings["select22"].Value, TN.capacity3, scenarioID, bYear);
        try
        {
            sqlConnection.Open();
            rdr = sqlCommand.ExecuteReader();
            while (rdr.Read())
            {
                luzB = rdr.GetInt16(0);
                z1 = rdr.GetInt32(1);
                z[luzB - 1].fcst.plmh = z1;
            }   // end while ,s
            rdr.Close();
        }   // end try
        catch (SqlException e)
        {
            MessageBox.Show(e.ToString(), "SQL Exception");
            Close();
        }
        finally
        {
            sqlConnection.Close();
        }

    }     // End procedure getmhCap()

    /*****************************************************************************/

    #endregion Extract Procs

    #region Get Outliers

    /* method getMFOutliers() */
    /// <summary>
    /// Method to build values to determine MF HS outliers.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 07/17/97   tb    Initial coding
    *                 08/06/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    void getMFOutliers()
    {
      int i;

      //writeToStatusBox( "Building HS MF outliers.." );
      if (reg.histMF.c5 > 0)
        reg.histMF.r5 = (double)reg.fcst.hsi.mf / (double)reg.histMF.c5;
      if (reg.histMF.L5 > 0)
        reg.histMF.pct5 = (double)reg.histMF.c5 / (double)reg.histMF.L5 * 100;

      for (i = 0; i < NUM_LUZS; i++)
      {
        hsOut.WriteLine("LUZ " + (i + 1));
        if (z[i].histMF.L5 > 0)
          z[i].histMF.pct5 = (double)z[i].histMF.c5 / (double)z[i].histMF.L5 * 100;
        // Compute LUZ change ratios
        if (z[i].histMF.c5 > 0)
          // 5 year ratio
          z[i].histMF.r5 = (double)z[i].fcst.hsi.mf / (double)z[i].histMF.c5;

        hsOut.WriteLine("hi.r5 = " + z[i].histMF.r5 + " hsi.mf = " + z[i].fcst.hsi.mf + " hi.c5 = " + z[i].histMF.c5);

        if (reg.histMF.r5 > 0)
          z[i].mfOut.r5 = z[i].histMF.r5 / reg.histMF.r5;
        else
          z[i].mfOut.r5 = 0;

        hsOut.WriteLine("mfOut.r5 = " + z[i].mfOut.r5 + " hi.r5 = " + z[i].histMF.r5 + " reg.histMF.r5 = " + reg.histMF.r5);

        hsOut.WriteLine("hsi.mf = " + z[i].fcst.hsi.mf + " hi.c5 = " + z[i].histMF.c5);
        if (z[i].fcst.hsi.mf * z[i].histMF.c5 < 0)     /* This means signs are different */
          z[i].mfOut.outCode = 1;
        else if (Math.Abs(z[i].mfOut.r5) > 1.2 || Math.Abs(z[i].mfOut.r5) < 0.70)
          z[i].mfOut.outCode = 2;

        if (z[i].mfOut.outCode > 0)      /* If any of the outlier codes are > 0 set the flag */
          flags.hOut = true;
        hsOut.WriteLine("outCode = " + z[i].mfOut.outCode);

        if (z[i].mfOut.outCode > 1)
        {
          if (Math.Abs(z[i].mfOut.r5) < 0.70)
            z[i].mfOut.inc5 = (int)(z[i].mfOut.r5 * 0.70 * z[i].histMF.c5);
          else if (Math.Abs(z[i].mfOut.r5) > 1.2)
            z[i].mfOut.inc5 = (int)(z[i].mfOut.r5 * 1.20 * z[i].histMF.c5);
          z[i].mfOut.diff5 = z[i].fcst.hsi.mf - z[i].mfOut.inc5;
          hsOut.WriteLine("mfOut.inc5 = " + z[i].mfOut.inc5 + " mfOut.r5 * 0.70 = " + z[i].mfOut.r5 + " histMF.c5 = " + z[i].histMF.c5);
        }   // end if
      }     // End for i
    }     // End method getMFOutliers()

    /*****************************************************************************/

    /* method getSFOutliers() */
    /// <summary>
    /// Method to build values to determine SF HS outliers.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 07/17/97   tb    Initial coding
    *                 08/06/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    void getSFOutliers()
    {
      int i;

      //writeToStatusBox( "Building HS SF outliers.." );
      if (reg.histSF.c5 > 0)
        reg.histSF.r5 = (double)reg.fcst.hsi.sf / (double)reg.histSF.c5;
      if (reg.histSF.L5 > 0)
        reg.histSF.pct5 = (double)reg.histSF.c5 / (double)reg.histSF.L5 * 100;

      for (i = 0; i < NUM_LUZS; i++)
      {
        hsOut.WriteLine("LUZ " + (i + 1));
        if (z[i].histSF.L5 > 0)
          z[i].histSF.pct5 = (double)z[i].histSF.c5 / (double)z[i].histSF.L5 * 100;

        // Compute LUZ change ratios
        if (z[i].histSF.c5 > 0)
          z[i].histSF.r5 = (double)z[i].fcst.hsi.sf / (double)z[i].histSF.c5;      // 5 year ratio
        hsOut.WriteLine("hi.r5 = " + z[i].histSF.r5 + " hi.sf = " + z[i].fcst.hsi.sf + " hi.c5 = " + z[i].histSF.c5);

        if (reg.histSF.r5 > 0)
          z[i].sfOut.r5 = z[i].histSF.r5 / reg.histSF.r5;
        else
          z[i].sfOut.r5 = 0;
        hsOut.WriteLine("sfOut.r5 = " + z[i].sfOut.r5 + " hi.r5 = " + z[i].histSF.r5 + " reg.histSF.r5 = " +
                        reg.histSF.r5);
        hsOut.WriteLine("hsi.sf = " + z[i].fcst.hsi.sf + " hi.c5 = " + z[i].histSF.c5);
        if (z[i].fcst.hsi.sf * z[i].histSF.c5 < 0)     /* This means signs are different */
          z[i].sfOut.outCode = 1;
        else if (Math.Abs(z[i].sfOut.r5) > 1.2 || Math.Abs(z[i].sfOut.r5) < 0.70)
          z[i].sfOut.outCode = 2;
        if (z[i].sfOut.outCode > 0)      /* If any of the outlier codes are > 0 set the flag */
          flags.hOut = true;
        hsOut.WriteLine("outCode = " + z[i].sfOut.outCode);

        if (z[i].sfOut.outCode > 1)
        {
          if (Math.Abs(z[i].sfOut.r5) < 0.70)
            z[i].sfOut.inc5 = (int)(z[i].sfOut.r5 * 0.70 * z[i].histSF.c5);
          else if (Math.Abs(z[i].sfOut.r5) > 1.2)
            z[i].sfOut.inc5 = (int)(z[i].sfOut.r5 * 1.20 * z[i].histSF.c5);

          z[i].sfOut.diff5 = z[i].fcst.hsi.sf - z[i].sfOut.inc5;
          hsOut.WriteLine("sfOut.inc5 = " + z[i].sfOut.inc5 + " sfOut.r5 * 0.70 = " + z[i].sfOut.r5 +
                      " histSF.c5 = " + z[i].histSF.c5);
        }   // end if
      }     // End for i
    }     // End method getSFOutliers()

    /*****************************************************************************/

    #endregion Get Outliers

    #region Load Procs

    /* method loadDecrements() */
    /// <summary>
    /// Method to load temporary SQL table for SF, MF, or mh decrements updates.
    /// </summary>
    /// <param name="type">Which increments set to bulk load. 1 = SF; 2 = MF; 3 =mh</param>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 07/17/97   tb    Initial coding
    *                 08/06/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    bool loadDecrements(int type)
    {
        string t1 = "";
        string t2 = "";
        // Validate parameter
        if (type < 1 || type > 3)
            return false;

        if (type == 1)
        {
            t1 = TN.SFDecrements;
            t2 = networkPath + String.Format(appSettings["sfCd"].Value);
        }  // end if
            
        else if (type == 2)
        {
            t1 = TN.MFDecrements;
            t2 = networkPath + String.Format(appSettings["mfCd"].Value);
        }  // end else if

        else if (type == 3)
        {
            t1 = TN.mhDecrements;
            t2 = networkPath + String.Format(appSettings["mhCd"].Value);
        }  // end else if
           
        sqlCommand.CommandText = String.Format(appSettings["truncate"].Value, t1);
        try
        {
            sqlConnection.Open();
            sqlCommand.ExecuteNonQuery();
        }
        catch (SqlException s)
        {
            MessageBox.Show(s.ToString(), "SQL Exception");
        Close();
        }
        finally
        {
            sqlConnection.Close();
        }

        sqlCommand.CommandText = String.Format(appSettings["bulkInsert"].Value, t1,t2);
        
        try
        {
            sqlConnection.Open();
            sqlCommand.ExecuteNonQuery();
        }
        catch (SqlException s)
        {
            MessageBox.Show(s.ToString(), "SQL Exception");
        }
        finally
        {
            sqlConnection.Close();
        }

        return true;
    }     // End procedure loadDecrements()

    /*****************************************************************************/

    /* method loadIncrements() */
    /// <summary>
    /// Method to load temporary SQL table for SF, MF, or mh increments updates.
    /// </summary>
    /// <param name="type">Which increments set to bulk load. 1 = SF; 2 = MF; 
    /// 3 =mh</param>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 07/17/97   tb    Initial coding
    *                 08/06/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    bool loadIncrements(int type)
    {
     
      string t1 = "";
      string t2 = "";

      // Validate parameter
      if (type < 1 || type > 3)
          return false;

      if (type == 1)
      {
          t1 = TN.SFIncrements;
          t2 = networkPath + String.Format(appSettings["sfCi"].Value);
      }  // end if

      else if (type == 2)
      {
          t1 = TN.MFIncrements;
          t2 = networkPath + String.Format(appSettings["mfCi"].Value);
      }  // end else if

      else if (type == 3)
      {
          t1 = TN.mhIncrements;
          t2 = networkPath + String.Format(appSettings["mhCi"].Value);
      }  // end else if

      sqlCommand.CommandText = String.Format(appSettings["truncate"].Value, t1);
      try
      {
          sqlConnection.Open();
          sqlCommand.ExecuteNonQuery();
      }
      catch (SqlException s)
      {
          MessageBox.Show(s.ToString(), "SQL Exception");
          Close();
      }
      finally
      {
          sqlConnection.Close();
      }

      
      sqlCommand.CommandText = String.Format(appSettings["bulkInsert"].Value, t1, t2);

      try
      {
          sqlConnection.Open();
          sqlCommand.ExecuteNonQuery();
      }
      catch (SqlException s)
      {
          MessageBox.Show(s.ToString(), "SQL Exception");
      }
      finally
      {
          sqlConnection.Close();
      }

      return true;
    }     // End procedure loadIncrements()

    /*****************************************************************************/

    #endregion Load Procs

    #region MF Procs
    /* method mfCalc() */
    /// <summary>
    /// Method to calculate MF distribution. MF stock gets undajusted for site 
    /// spec and lost units.  Adjusted stock computed done in doHSTot1()
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 07/11/97   tb    Initial coding
    *                 08/05/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    void mfCalc()
    {
      int i, j;
      int realIndex;
      int ret;

      double[] attrMF = new double[NUM_LUZS];
      int[] dests = new int[NUM_LUZS];
      int[] erInc = new int[NUM_LUZS];
      int[] ubound = new int[NUM_LUZS];      /* Dummy array filled with 99999
                                              * used in roundIt call */
      int[,] dIndex1 = new int[NUM_LUZS, 2];
      int[] dIndex2 = new int[NUM_LUZS];
      // ------------------------------------------------------------------------

      writeToStatusBox("Computing MF distribution..");
      try
      {
        mfOut = new StreamWriter(new FileStream(networkPath + String.Format(appSettings["mfOut"].Value), FileMode.Create));
      }
      catch (IOException e)
      {
        MessageBox.Show(e.ToString());
        Close();
      }
      for (i = 0; i < NUM_LUZS; i++)
        ubound[i] = 99999;

      //writeToStatusBox( "   storing MF attractors.." );
      for (i = 0; i < NUM_LUZS; i++)
      {
        // Store the MF attractor override, if applicable.
        if (z[i].hOvr && z[i].ho.attrMF != 0)
          attrMF[i] = (double)z[i].ho.attrMF;
        else      // Otherwise, store the total MF capacity as attractor.
          attrMF[i] = (double)z[i].capacity.totalMF * z[i].homePriceIndex;
      }   // end for i

      /* Allocate employed residents to MF based on regional share of housing stock change */
      for (i = 0; i < NUM_LUZS; i++)
      {
        mfOut.WriteLine("   allocating er LUZ " + (i + 1));
        if (MF_DEBUG&& i < 3)
        {
          mfOut.WriteLine("      attractor MF CAPACITY");
          for (j = 0; j < NUM_LUZS; j++)
          {
            mfOut.Write("{0,7:F0}", attrMF[j]);
            if (j > 0 && j % 10 == 0)
              mfOut.WriteLine();
          }   // end for j
          mfOut.WriteLine();
          mfOut.WriteLine();
        }   // end if

        prob2(i, attrMF);

        // Allocated civilian employment increment
        z[i].fcst.eri.total = z[i].fcst.ei.civ + z[i].site.civ;
        if (rc.fcst.hsi.total > 0)
        {
          z[i].fcst.eri.mfAlloc = (int)(0.5 + z[i].fcst.eri.total * (double)rc.fcst.hsi.mf / (double)rc.fcst.hsi.total);
          mfOut.WriteLine("      mf_alloc = " + z[i].fcst.eri.mfAlloc + " eri.tot = " + z[i].fcst.eri.total + " rc.mf = " + rc.fcst.hsi.mf + " rc.tot = " + rc.fcst.hsi.total);
          mfOut.Flush();
        }   // end if
        else
          z[i].fcst.eri.mfAlloc = 0;

        // Split into transit or autos
        z[i].fcst.eri.mfTransit = (int)(0.5 + (double)z[i].fcst.eri.mfAlloc * fractees[i]);

        z[i].fcst.eri.mfAuto = z[i].fcst.eri.mfAlloc - z[i].fcst.eri.mfTransit;

        mfOut.WriteLine("      mf_transit = " + z[i].fcst.eri.mfTransit + " mf_auto = " + z[i].fcst.eri.mfAuto);
        mfOut.Flush();

        // Now, do the orig-dest probability computations
        for (j = 0; j < NUM_LUZS; j++)
        {
          dests[j] = (int)((double)z[i].fcst.eri.mfAuto * allocProb[j, 0] + (double)z[i].fcst.eri.mfTransit * allocProb[j, 1]);
          mfOut.WriteLine("      j = {0,3} auto = {1,5} prob0 = {2,7:F4} " + "transit = {3,5} prob1 = {4,7:F4} dest = {5,5}",
                      j, z[i].fcst.eri.mfAuto, allocProb[j, 0], z[i].fcst.eri.mfTransit, allocProb[j, 1], dests[j]);
        }   // end for j

        for (j = 0; j < NUM_LUZS; j++)
        {
          dIndex1[j, 0] = j;
          dIndex1[j, 1] = dests[j];
        }   // end for j

        // Sort them
        UDMUtils.insertsort(dIndex1, NUM_LUZS);
        //UDMUtils.quickSort( dIndex1, 0, NUM_LUZS - 1 );

        if (MF_DEBUG&& i < 3)
        {
          mfOut.WriteLine("      DESTS-UNADJUSTED");
          for (j = 0; j < NUM_LUZS; j++)
          {
            mfOut.Write("{0,5}", dests[j]);
            if (j > 0 && j % 20 == 0)
              mfOut.WriteLine();
          }   // end for j
          mfOut.WriteLine();
          mfOut.WriteLine();
        }   // end if

        for (j = 0; j < NUM_LUZS; j++)
          dIndex2[j] = dIndex1[j, 1];

        ret = UDMUtils.roundIt(dIndex2, ubound, z[i].fcst.eri.mfAlloc, NUM_LUZS, 0);
        if (ret > 0)
        {
          MessageBox.Show("mfCalc roundIt did not converge, difference = " + ret + " LUZ = " + (i + 1));
          return;
        }   // end if

        // Restore the sorted data to original order
        for (j = 0; j < NUM_LUZS; j++)
        {
          realIndex = dIndex1[j, 0];
          dests[realIndex] = dIndex2[j];
        }   // end for j

        if (MF_DEBUG&& i < 3)
        {
          mfOut.WriteLine("      DESTS-ADJUSTED");
          for (j = 0; j < NUM_LUZS; j++)
          {
            mfOut.Write("{0,5}", dests[j]);
            if (j > 0 && j % 20 == 0)
              mfOut.WriteLine();
          }   // end for j
          mfOut.WriteLine();
          mfOut.WriteLine();
        }   // end if

        for (j = 0; j < NUM_LUZS; j++)
          allocAll[j, i] = dests[j];
        mfOut.Flush();

      }     // End for i

      for (i = 0; i < NUM_LUZS; i++)
      {
        for (j = 0; j < NUM_LUZS; j++)
        {
          erInc[j] += (int)allocAll[j, i];
        }   // end for j
      }  // end for i

      mfOut.WriteLine("BEFORE CONTROLLING MF");
      // Convert employed res at place of residence to MF
      for (i = 0; i < NUM_LUZS; i++)
      {
        // Apply vacancy rate to get adjusted MF stock
        if (z[i].fcst.r.erHH > 0)
          z[i].fcst.hsi.mfAdj = (int)(0.5 + (double)erInc[i] / z[i].fcst.r.erHH / (1 - z[i].fcst.r.vMF / 100));

        /* Check for negative here.  If the unit increment is negative, 
        * replace with historical change (assuming it's positive, adjusted 
        * by the regional 5-year % change */
        if (z[i].fcst.hsi.mfAdj < 0)
        {
          z[i].fcst.hsi.mfAdj = (int)(0.5 + (double)z[i].histMF.c5 * ((double)rc.fcst.hsi.mf / (double)reg.histMF.c5));
          if (z[i].fcst.hsi.mfAdj < 0)     // Constrain this to 0
            z[i].fcst.hsi.mfAdj = 0;
        }   // end if
        mfOut.WriteLine("luz = {0,3} er = {1,6} mf_adj = {2,6} cap = {3,6}", (i + 1), erInc[i], z[i].fcst.hsi.mfAdj, z[i].capacity.totalMF);
        mfOut.Flush();
      }     // End for i
      mfOut.Close();
      //writeToStatusBox("at end of mfcalc");
    }     // End method mfCalc()

    //*************************************************************************************************************

    /* method MFDecrements() */
    /// <summary>
    /// Method to distribute LUZ MF decrements to transcations.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 12/01/09   tb    Initial coding - added for sr13 to handle programmed demos for mf
    * --------------------------------------------------------------------------
    */
    private void MFDecrements(int[] list, int counter)
    {

      SqlDataReader rdr;
      int i, luzControl, temp, k, zid, lz, nt, kk;
      int luz, mgra, LCKey, lu, devCode, eluu, mf, capMf;
      int[] nTCap = new int[NUM_LUZS];
      double weight;
              
      mfCd = new StreamWriter(new FileStream(networkPath + String.Format(appSettings["mfCd"].Value),FileMode.Create));
      sqlCommand.CommandText = String.Format(appSettings["select09"].Value, TN.capacity3, TN.accessWeights, fYear, scenarioID, bYear);
      
      try
      {
        sqlConnection.Open();
        rdr = sqlCommand.ExecuteReader();
        while (rdr.Read())
        {
          luz = rdr.GetInt16(0);

          mgra = rdr.GetInt16(1);
          weight = rdr.GetDouble(2);
          LCKey = rdr.GetInt32(3);
          mf = rdr.GetInt32(4);
          devCode = rdr.GetByte(5);
          lu = rdr.GetInt16(6);
          capMf = rdr.GetInt32(7);
          zid = 999;      // Set the default for not found

          // Is the luz from query response in this list
          for (k = 0; k < counter; k++)
          {
            if (list[k] == luz)
            {
              zid = k;      // Save the list position as index
              break;
            }   // end if
          }   // end for k

          if (zid != 999)      // Make sure the index is legitimate
          {
            nt = nTCap[zid];      // Store index position

            // Save developed only if land use is correct
            eluu = 1;     /* set eluu for default */
            if (devCode == 1)
              eluu = 2;

            if (eluu > 0)      /* This gets done for devcode = 1 and correct land use or devcode = 6 */
            {
              tcapd[zid, nt] = new TCapD();
              tcapd[zid, nt].luz = luz;
              tcapd[zid, nt].done = false;         // Initialize the used flag
              tcapd[zid, nt].mgra = mgra;          // MGRA ID
              tcapd[zid, nt].LCKey = LCKey;            // LCKey
              tcapd[zid, nt].mf = mf;            // Base year emp
              tcapd[zid, nt].capMf = capMf;      // Civ emp cap
              tcapd[zid, nt].devCode = devCode;   // devcode
              nTCap[zid]++;

            }  // end if eluu
          }     // End if zid
        }     // End while
        rdr.Close();
      }     // End try

      catch (SqlException s)
      {
        MessageBox.Show(s.Message, "SQL Error");
        Close();
      }
      finally
      {
        sqlConnection.Close();
      }

      for (i = 0; i < counter; i++)
      {

        zid = i;
        lz = list[i] - 1;     // Get the LUZ ID from the list 

        if (lz == 110) // temporary debug stop
          lz = 110;
        k = 0;
        luzControl = -z[lz].fcst.hsi.mfAdj;      /* Set the luzControl value to the adjusted mf */
        // While the luzControl > 0 and the list is unfinished
        while (k < nTCap[i] && luzControl > 0)
        {
          // Get the minimum of base year emp and luz_control
          temp = Math.Min(luzControl, tcapd[zid, k].mf);
          tcapd[zid, k].mf -= temp;         // Adjust base year emp
          tcapd[zid, k].chgMf = -temp;      // Adjust the increment holder
          tcapd[zid, k].capMf += temp;      // Adjust the capacity
          luzControl -= temp;                // Adjust the loop luzControl
          tcapd[zid, k].done = true;          // Mark this entry as adjusted
          if (tcapd[zid, k].devCode == 1)     //reset any developed mf land to emf infill
            tcapd[zid, k].devCode = 6;          // All   of these become redev
          tcapd[zid, k].pCap_hs = 0;             // Reset emp and hs pCap to 0

          k++;                               // Increment the loop list counter
        }   // end while

        for (kk = 1; kk < NUM_MF_LAND; kk++)
        {
          // Add site spec to acreage used
          z[lz].useCap.ac.amf[kk] += z[lz].site.ac.amf[kk];
          // Compute remaining acreage
          z[lz].remCap.ac.amf[kk] = z[lz].capacity.ac.amf[kk] - z[lz].useCap.ac.amf[kk];
          z[lz].useCap.ac.totalMFAcres += z[lz].useCap.ac.amf[kk];
          z[lz].remCap.ac.totalMFAcres += z[lz].remCap.ac.amf[kk];
          reg.useCap.ac.amf[kk] += z[lz].useCap.ac.amf[kk];     /* Used acreage by category */

          // Remaining acreage by category
          reg.remCap.ac.amf[kk] += z[lz].remCap.ac.amf[kk];
          // Total used
          reg.useCap.ac.totalMFAcres += z[lz].useCap.ac.amf[kk];
          // Total remaining
          reg.remCap.ac.totalMFAcres += z[lz].remCap.ac.amf[kk];
        }   // end for kk

        if (luzControl > 0)      // Is there mf left to allocate
          MessageBox.Show("EXCEPTION -- Cannot allocate LUZ MF decrement to transactions, LUZ " + (lz + 1) + " CONTROL = " + luzControl);
        // Update the capacity records with new data
        for (k = 0; k < nTCap[i]; k++)
        {
          if (tcapd[zid, k].done)
            writeMfDecrements(tcapd[zid, k]);      /* Write the update data to ASCII for bulk load */
        }   // end for k
      }     // End for i

      // Close the output file
      try
      {
        mfCd.Close();
      }
      catch (IOException io)
      {
        MessageBox.Show(io.Message);
        Close();
      }

      loadDecrements(2);        /* Bulk load the ASCII data to the emp_dec table */
      updateMFDecrements();
    }     // End method MFDecrements()

    /*****************************************************************************/

    /* method MFIncrements() */
    /// <summary>
    /// Method to distribute LUZ MF increments to transactions.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 07/17/97   tb    Initial coding
    *                 06/22/99   tb    Fixed error in pcap calculation - base 
    *                                  year pcap was not included in query to 
    *                                  fill tcapi arrays
    *                 08/05/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    void MFIncrements(int[] list, int counter)
    {
        SqlDataReader rdr;
        StreamWriter xx = null, xy = null;
        int luz, mgra, LCKey, lu, plu, devCode, mfLU, sf, mf, mh, csf, cmf,
            cmh;
        int i, j, k, kk, zid, nt;
        int msf, mmh, temp, lz;
        int tempSF, tempmh;
        int masterControl;
        int[] control = new int[NUM_MF_LAND];     // MF land use control totals
        int[] nTCap = new int[NUM_LUZS];
        int[] tlh = new int[NUM_LUZS];
        double acres, capRatio, temp1, weight;
        double capDiff, pCap_hs;
        // -----------------------------------------------------------------------

        try
        {
            mfCi = new StreamWriter(new FileStream(networkPath + String.Format(appSettings["mfCi"].Value), FileMode.Create));
        }
        catch (IOException e)
        {
            MessageBox.Show(e.ToString(), "IO Exception");
            Close();
        }
        try
        {
            xx = new StreamWriter(new FileStream(networkPath + "xx.txt", FileMode.Create));
        }
        catch (IOException e)
        {
            MessageBox.Show(e.ToString(), "IO Exception");
            Close();
        }

        try
        {
            xy = new StreamWriter(new FileStream(networkPath + "xy.txt", FileMode.Create));
        }
        catch (IOException e)
        {
            MessageBox.Show(e.ToString(), "IO Exception");
            Close();
        }
        sqlCommand.CommandText = String.Format(appSettings["select10"].Value, TN.capacity3, TN.accessWeights, fYear, scenarioID, bYear);
        xx.AutoFlush = true;
        try
        {
            sqlConnection.Open();
            rdr = sqlCommand.ExecuteReader();
            while (rdr.Read())
            {
                luz = rdr.GetInt16(0);
                mgra = rdr.GetInt16(1);
                weight = rdr.GetDouble(2);
                LCKey = rdr.GetInt32(3);
                lu = rdr.GetInt16(4);
                plu = rdr.GetInt16(5);
                devCode = rdr.GetByte(6);
                acres = rdr.GetDouble(7);
                mfLU = rdr.GetByte(8);
                sf = rdr.GetInt32(9);
                mf = rdr.GetInt32(10);
                mh = rdr.GetInt32(11);
                csf = rdr.GetInt32(12);
                cmf = rdr.GetInt32(13);
                cmh = rdr.GetInt32(14);
                pCap_hs = rdr.GetDouble(15);

                zid = 999;      // Set the default for not found

                // Is the LUZ in the query in this list
                for (k = 0; k < counter; k++)
                {
                    if (list[k] == luz)
                    {
                        zid = k;      // Save the list position as index
                        break;
                    }   // end if
                }   // end for k

                if (zid != 999)      // Do this only for good indexes
                {
                    nt = nTCap[zid];      // Get the second dimension index
                    tCapI[zid, nt] = new TCap();
                    tCapI[zid, nt].mgra = mgra;
                    tCapI[zid, nt].LCKey = LCKey;
                    tCapI[zid, nt].pCap_hs = pCap_hs;
                    tCapI[zid, nt].lu = lu;
                    tCapI[zid, nt].plu = plu;
                    tCapI[zid, nt].devCode = devCode;
                    tCapI[zid, nt].acres = acres;
                    tCapI[zid, nt].udmMFLU = mfLU;
                    tCapI[zid, nt].sf = sf;
                    tCapI[zid, nt].mf = mf;
                    tCapI[zid, nt].mh = mh;
                    tCapI[zid, nt].capSF = csf;
                    tCapI[zid, nt].capMF = cmf;
                    tCapI[zid, nt].capmh = cmh;
                    tCapI[zid, nt].chgSF = 0;
                    tCapI[zid, nt].chgMF = 0;
                    tCapI[zid, nt].chgmh = 0;
                    tCapI[zid, nt].done = false;
                    if (zid == 0)
                        xx.WriteLine(zid + "," + nt + "," + LCKey + "," + tCapI[zid,nt].capMF);

                    nTCap[zid]++;     // Increment the second dimension index

                    // Check for max array size, MAX_TRANS is somewhat arbitrary
                    if (nTCap[zid] == MAX_TRANS)
                    {
                        MessageBox.Show("FATAL ERROR - in MFIncrements tCapI array index exceeds array bound = " + MAX_TRANS);
                        Close();
                    }   // end if
                }     // End if zid
            }     // End while
            rdr.Close();
            xx.Close();
        }     // End try

        catch (Exception e)
        {
            MessageBox.Show(e.ToString(), "Runtime Exception");
            Close();
        }
        finally
        {
            sqlConnection.Close();
        }

        for (i = 0; i < counter; i++)
        {
            zid = i;
            lz = list[i] - 1;     // Get LUZ ID
            if (lz == 113)
                lz = 113;
            for (j = 1; j < NUM_MF_LAND; j++)
            {
                // Store mfLand distribution as controls
                control[j] = z[lz].fcst.hsi.mfLand[j];

                // These are the incremental MF allocated to land use
                z[lz].useCap.ac.amf[j] = 0;     /* Initialize the dev acres used totals */
                z[lz].remCap.ac.amf[j] = 0;     /* Initialize the dev acres remaining totals */
            }   // end for j

            masterControl = 0;

            /* Build master control as boolean or with control values.  Master 
            * control will be used as loop control as long as it > 0, meaning
            * there is still capacity to fill. */
            for (j = 1; j < NUM_MF_LAND; j++)
                 masterControl += control[j];
           
            k = 0;      // Start the counter

            // Allocation control loop
            while (k < nTCap[i] && masterControl > 0)
            {
                
                tCapI[zid, k].done = true;     // Mark as used
                tCapI[zid, k].oldCapSF = tCapI[zid, k].capSF;
                tCapI[zid, k].oldCapMF = tCapI[zid, k].capMF;
                tCapI[zid, k].oldCapmh = tCapI[zid, k].capmh;

                // Get UDM MF land array index 1 - 3
                kk = tCapI[zid, k].udmMFLU;

                /* Get the minimum of MF capacity and control (mfLand) for this land use */
                
                temp = Math.Min(control[kk], tCapI[zid, k].capMF);
                if (temp > masterControl)
                    temp = masterControl;
                // Allocate to this land use if temp > 0
                if (temp > 0)
                {   if (zid == 0)
                       xy.WriteLine(k + "," + tCapI[zid, k].LCKey + "," + temp + "," + masterControl);                       
                    tCapI[zid, k].mf += temp;        // Adjust base year MF
                    tCapI[zid, k].chgMF = temp;      // Store increment
                    tCapI[zid, k].capMF -= temp;     // Adjust capacity
                    control[kk] -= temp;            // Adjust this control value

                    /* Things to do if the transaction capacity has been reduced to zero */
                    if (tCapI[zid, k].capMF == 0)
                    {
                        tCapI[zid, k].pCap_hs = 1.0;      // Set the pCap to 100%
                        tCapI[zid, k].lu = tCapI[zid, k].plu;      /* Reset the base land use to the planned */
                        tCapI[zid, k].devCode = 1;     // Mark as developed
                        // Keep track of the acres
                        z[lz].useCap.ac.amf[kk] += tCapI[zid, k].acres;
                    }   // end if

                    else    /* This is a partialy developed record - compute some residuals */
                    {
                        tCapI[zid, k].oldpCap_hs = tCapI[zid, k].pCap_hs;     /* Save the original pCap */
                        // Estimate original (2000) capacity
                        if (tCapI[zid, k].pCap_hs != 1)
                            temp1 = (double)tCapI[zid, k].oldCapMF / (1 - tCapI[zid, k].pCap_hs);
                        else
                            temp1 = 0;

                        // Recompute pCap
                        if (temp1 > 0)
                            tCapI[zid, k].pCap_hs = 1 - ((double)tCapI[zid, k].capMF / temp1);
                        // Allocate acreage
                        z[lz].useCap.ac.amf[kk] += tCapI[zid, k].acres * (tCapI[zid, k].pCap_hs - tCapI[zid, k].oldpCap_hs);
                    }     // End else

                    /* If this is a residential to res dev - there are some units to address */
                    if (kk == 1 || kk == 2)      // This is a res redev or infill code
                    {
                        // Capacity difference
                        capDiff = tCapI[zid, k].pCap_hs - tCapI[zid, k].oldpCap_hs;
                        capRatio = 1 - tCapI[zid, k].oldpCap_hs;      // Ratio
                        msf = (int)((double)Math.Abs(tCapI[zid, k].capSF) / capRatio * (double)capDiff);
                        tempSF = Math.Min(msf, tCapI[zid, k].sf);
                        mmh = (int)((double)Math.Abs(tCapI[zid, k].capmh) / capRatio * (double)capDiff);
                        tempmh = Math.Min(mmh, tCapI[zid, k].mh);

                        tCapI[zid, k].sf -= tempSF;      // Adjust base year SF
                        tCapI[zid, k].mh -= tempmh;      // Adjust base year mh

                        tCapI[zid, k].chgSF = -tempSF;     // Set value for sf increment
                        tCapI[zid, k].chgmh = -tempmh;     // Set value for mh increment
                        tCapI[zid, k].capSF += tempSF;     // Add to capacity SF
                        tCapI[zid, k].capmh += tempmh;     // Add to capacity mh

                        /* units lost */
                        if (zid == 0 && tempmh > 0)
                            xy.WriteLine(k + "," + tCapI[zid, k].LCKey + "," + temp + "," + tempmh);
                        z[lz].fcst.lh.sf -= tempSF;
                        z[lz].fcst.lh.mh -= tempmh;
                        reg.fcst.lh.sf -= tempSF;
                        reg.fcst.lh.mh -= tempmh;
                    }     // End if kk

                    // Rebuild master control for checking loop
                    masterControl = 0;
                    for (j = 1; j < NUM_MF_LAND; j++)
                        masterControl += control[j];
                  
                }     // End if temp > 0

                k++;      // Increment the counter
                
            }     // End while
            xy.Close();

            for (kk = 1; kk < NUM_MF_LAND; kk++)
            {
                // Add site spec to acreage used
                z[lz].useCap.ac.amf[kk] += z[lz].site.ac.amf[kk];

                // Compute remaining acreage
                z[lz].remCap.ac.amf[kk] = z[lz].capacity.ac.amf[kk] - z[lz].useCap.ac.amf[kk];
                z[lz].useCap.ac.totalMFAcres += z[lz].useCap.ac.amf[kk];
                z[lz].remCap.ac.totalMFAcres += z[lz].remCap.ac.amf[kk];

                // Used acreage by category
                reg.useCap.ac.amf[kk] += z[lz].useCap.ac.amf[kk];
                // Remaining acreage by category
                reg.remCap.ac.amf[kk] += z[lz].remCap.ac.amf[kk];
                // Total used
                reg.useCap.ac.totalMFAcres += z[lz].useCap.ac.amf[kk];
                // Total remaining
                reg.remCap.ac.totalMFAcres += z[lz].remCap.ac.amf[kk];
            }   // end for kk

            if (masterControl > 0)     // Is there capacity remaining
            {
                MessageBox.Show("EXCEPTION -- CANNOT ALLOCATE LUZ MF INCREMENT TO TRANSACTIONS, LUZ " + (lz + 1));
            }   // end if

            // Update the capacity records with new data
            for (k = 0; k < nTCap[i]; k++)
            {
                if (tCapI[zid, k].done)      // Skip records whose flags are not set
                    // Write the updated data to ASCII for bulk loading
                    mfCi.WriteLine((lz + 1) + "," + tCapI[zid, k].LCKey + "," +
                            tCapI[zid, k].devCode + "," + tCapI[zid, k].lu + "," +
                            tCapI[zid, k].pCap_hs + "," + tCapI[zid, k].sf + "," +
                            tCapI[zid, k].mf + "," + tCapI[zid, k].mh + "," +
                            tCapI[zid, k].capSF + "," + tCapI[zid, k].capMF + "," +
                            tCapI[zid, k].capmh + "," + tCapI[zid, k].chgSF + "," +
                            tCapI[zid, k].chgMF + "," + tCapI[zid, k].chgmh);
            }   // end for k

        }     // End for i

        mfCi.Close();

        loadIncrements(2);
        updateNonMHIncrements(2);     // Update capacity from the mfInc table
        for (i = 0; i < NUM_LUZS; ++i)
            tlh[i] = z[i].fcst.lh.mh;
    }     // End method MFIncrements()

    /*****************************************************************************/

    /* method mfLand() */
    /// <summary>
    /// Method to distribute LUZ HS MF forecast to three land use types.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 07/17/97   tb    Initial coding
    *                 08/05/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    void mfLand()
    {
        int ztt, zDiff, mult, i, j;
        int loopCount;
        double ratio;

        // Distribute MF increment to MF land uses

        // Land use arrays start at 1, 0 is dummy
        for (i = 0; i < NUM_LUZS; i++)
        {
            if (i == 113)
                i = 113;
            if (z[i].fcst.hsi.mfAdj > 0)
            {
                ztt = 0;
                if (z[i].capacity.totalMF != 0)
                    ratio = (double)z[i].fcst.hsi.mfAdj / (double)z[i].capacity.totalMF;
                else
                    ratio = 0;

                // Allocate to MF and keep sum of computed distributions
                for (j = 1; j < NUM_MF_LAND; j++)      // 3 emp land categories
                {
                    z[i].fcst.hsi.mfLand[j] = (int)(0.5 + (double)z[i].capacity.mf[j] * ratio);
                    ztt += z[i].fcst.hsi.mfLand[j];
                }   // end for j

                zDiff = z[i].fcst.hsi.mfAdj - ztt;      /* Get difference due to rounding */
                if (zDiff > 0)
                    mult = 1;
                else
                    mult = -1;
                loopCount = 0;
                while (zDiff != 0 && loopCount < 10000)      /* If there is a difference */
                {
                    // Distribute to non-zero cells
                    for (j = 1; j < NUM_MF_LAND; j++)
                    {
                        if ((z[i].fcst.hsi.mfLand[j] + mult <= z[i].capacity.mf[j]))
                        {
                            z[i].fcst.hsi.mfLand[j] += mult;
                            zDiff -= mult;
                        }   // end if
                        // Is the difference gone
                        if (zDiff == 0)
                            break;      // Get out of for loop
                    }   // end for j
                    loopCount++;
                }     // End while

                if (loopCount >= 10000)
                {
                    MessageBox.Show("Allocating MF to land failed for LUZ " + (i + 1));

                }  //end if
            }     // End if

            /* Compute regional totals and remaining capacity for MF land categories */
            for (j = 1; j < NUM_MF_LAND; j++)
            {
                // Regional increment by land use type
                reg.fcst.hsi.mfLand[j] += z[i].fcst.hsi.mfLand[j];
                // LUZ remaining capacity by land use and total
                z[i].remCap.mf[j] = z[i].capacity.mf[j] - z[i].fcst.hsi.mfLand[j];
                z[i].remCap.totalMF += z[i].remCap.mf[j];
                // Regional total remaining capacity by land use type
                reg.remCap.mf[j] += z[i].remCap.mf[j];
            }   // end for j

            // Regional total remaining capacity all emp land categories
            reg.remCap.totalMF += z[i].remCap.totalMF;
        }     // End for i
    }     // End method mfLand()

    /*****************************************************************************/

    /* method mfTransactions() */
    /// <summary>
    /// Method to distribute LUZ HS MF to transcations - master control routine.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 07/17/97   tb    Initial coding
    *                 08/05/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    void mfTransactions()
    {
        int ki = 0, kd = 0, i;
        int[] iList = new int[NUM_LUZS], dList = new int[NUM_LUZS];
        writeToStatusBox("Distributing MF to transactions..");

        for (i = 0; i < NUM_LUZS; i++)
        {
            if (z[i].fcst.hsi.mfAdj >= 0)     // Build increment list of LUZs
                iList[ki++] = i + 1;
            else
                dList[kd++] = i + 1;

        }   // end for i

        // Call increment or decrement
        if (kd > 0)
            MFDecrements(dList, kd);
        if (ki > 0)
            MFIncrements(iList, ki);

    }     // End method mfTransactions()

    /*****************************************************************************/

    #endregion MF Procs

    #region MH Procs
    /* method mhCalc() */
    /// <summary>
    /// Method to calculate mh HS distribution.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 11/14/97   tb    Initial coding
    *                 08/06/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    void mhCalc()
    {
        StreamWriter xxmh = null;
        int temp11 = 0, i;
        int[] temp10 = new int[NUM_LUZS];
        int[] baseMh = new int[NUM_LUZS];
        int[] siteMh = new int[NUM_LUZS];
        int[] lhMh = new int[NUM_LUZS];
        int[] leMh = new int[NUM_LUZS];
        int[] plMh = new int[NUM_LUZS];
        int[] zadj = new int[NUM_LUZS];
        int[] remMH = new int[NUM_LUZS];
        try
        {
            xxmh = new StreamWriter(new FileStream(networkPath + "xxmh.txt", FileMode.Create));
        }
        catch (IOException e)
        {
            MessageBox.Show(e.ToString(), "IO Exception");
            Close();
        }
        writeToStatusBox("Computing mh distribution..");
        xxmh.AutoFlush = true;
        // Adjust regional control for units lost to SF and MF redev
        rc.fcst.hsi.mhAdj -= reg.fcst.lh.mh;

        /* Sum capHSmh from capacity to LUZs to get potential lost units before adjusting */
        getmhCap();

        for (i = 0; i < NUM_LUZS; i++)
        {
            /* mh forecast is base adjusted for sitespec and losses, factored to regional total */
            baseMh[i] = z[i].baseData.hs.mh;
            siteMh[i] = z[i].site.mh;
            lhMh[i] = z[i].fcst.lh.mh;
            leMh[i] = z[i].fcst.le.mh;
            plMh[i] = z[i].fcst.plmh; // this capacity (planned)
           
            temp10[i] = z[i].site.mh + z[i].fcst.lh.mh + z[i].fcst.le.mh; // this should be remaining MH
            remMH[i] = z[i].baseData.hs.mh + temp10[i];
            temp11 += temp10[i];   // this should be remaining MH before progam adjustments
        }   // end for i
        // compute mh adjustment from remaining total
        
        // Factor to regional total
        for (i = 0; i < NUM_LUZS; i++)
        {
            if (temp11 != 0 && remMH[i] > 0)
                z[i].fcst.hsi.mhAdj = (int)(0.5 + (double)temp10[i] * ((double)rc.fcst.hsi.mhAdj / (double)temp11));
            zadj[i] = z[i].fcst.hsi.mhAdj;
            xxmh.WriteLine((i + 1) + "," + z[i].baseData.hs.mh + "," + z[i].site.mh + "," + z[i].fcst.lh.mh + "," + z[i].fcst.le.mh + ",0," + zadj[i]);
            
        }   // end for i
        // need to control zadj
        UDMUtils.roundItNeg(zadj, remMH, rc.fcst.hsi.mhAdj, NUM_LUZS);
        for (i = 0; i < NUM_LUZS; ++i)
        {
            z[i].fcst.hsi.mhAdj = zadj[i];
            xxmh.WriteLine((i + 1) + "," + z[i].baseData.hs.mh + "," + z[i].site.mh + "," + z[i].fcst.lh.mh + "," + z[i].fcst.le.mh + ",0," + zadj[i]);
        }  // end for
        xxmh.Close();
    }     // End method mhCalc()

    /*****************************************************************************/

    /* method mhDecrements() */
    /// <summary>
    /// Method to distribute LUZ SF decrements to transcations.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 12/01/09   tb    Initial coding - added for sr13 to handle programmed demos for sf
    * --------------------------------------------------------------------------
    */
    private void mhDecrements(int[] list, int counter)
    {

        SqlDataReader rdr;
        int i, luzControl, temp, k, zid, lz, nt;
        int luz, mgra, LCKey, lu, devCode, eluu, mh, capMh;
        int[] nTCap = new int[NUM_LUZS];
        double weight;

        mhCd = new StreamWriter(new FileStream(networkPath + String.Format(appSettings["mhCd"].Value), FileMode.Create));

        sqlCommand.CommandText = string.Format(appSettings["select21"].Value, TN.capacity3, TN.accessWeights, fYear, scenarioID, bYear);
        try
        {
            sqlConnection.Open();
            rdr = sqlCommand.ExecuteReader();
            while (rdr.Read())
            {
                luz = rdr.GetInt16(0);

                mgra = rdr.GetInt16(1);
                weight = rdr.GetDouble(2);
                LCKey = rdr.GetInt32(3);
                mh = rdr.GetInt32(4);
                devCode = rdr.GetByte(5);
                lu = rdr.GetInt16(6);
                capMh = rdr.GetInt32(7);
                zid = 999;      // Set the default for not found

                // Is the luz from query response in this list
                for (k = 0; k < counter; k++)
                {
                    if (list[k] == luz)
                    {
                        zid = k;      // Save the list position as index
                        break;
                    }   // end if
                }   // end for k

                if (zid != 999)      // Make sure the index is legitimate
                {
                    nt = nTCap[zid];      // Store index position

                    // Save developed only if land use is correct
                    eluu = 1;     /* set eluu for default */
                    if (devCode == 1)
                        eluu = 2;

                    if (eluu > 0)      /* This gets done for devcode = 1 and correct land use or devcode = 9 */
                    {
                        tcapd[zid, nt] = new TCapD();
                        tcapd[zid, nt].luz = luz;
                        tcapd[zid, nt].done = false;         // Initialize the used flag
                        tcapd[zid, nt].luz = luz;
                        tcapd[zid, nt].mgra = mgra;          // MGRA ID
                        tcapd[zid, nt].LCKey = LCKey;            // LCKey
                        tcapd[zid, nt].mh = mh;            // Base year emp
                        tcapd[zid, nt].capMh = capMh;      // Civ emp cap
                        tcapd[zid, nt].devCode = devCode;   // devcode
                        nTCap[zid]++;

                    }  // end if eluu
                }     // End if zid
            }     // End while
            rdr.Close();
        }     // End try

        catch (SqlException s)
        {
            MessageBox.Show(s.Message, "SQL Error");
        }
        finally
        {
            sqlConnection.Close();
        }

        for (i = 0; i < counter; i++)
        {
            zid = i;
            lz = list[i] - 1;     // Get the LUZ ID from the list 

            if (lz == 71) // temporary debug stop
                lz = 71;
            k = 0;
            luzControl = -z[lz].fcst.hsi.mhAdj;      /* Set the luzControl value to the adjusted mh */
            // While the luzControl > 0 and the list is unfinished
            while (k < nTCap[i] && luzControl > 0)
            {
                // Get the minimum of base year and luz_control
                temp = Math.Min(luzControl, tcapd[zid, k].mh);
                tcapd[zid, k].mh -= temp;         // Adjust base year
                tcapd[zid, k].chgMh = -temp;      // Adjust the increment holder
                tcapd[zid, k].capMh += temp;      // Adjust the capacity
                luzControl -= temp;                // Adjust the loop luzControl
                tcapd[zid, k].done = true;          // Mark this entry as adjusted
                if (tcapd[zid, k].devCode == 1)     //reset any developed mh land to mh redev
                    tcapd[zid, k].devCode = 9;          // All   of these become redev
                tcapd[zid, k].pCap_hs = 0;             // Reset hs pCap to 0

                k++;                               // Increment the loop list counter
            }   // end while

            if (luzControl > 0)      // Is there anything left to allocate
                MessageBox.Show("EXCEPTION -- Cannot allocate LUZ mh decrement to transactions, LUZ " + (lz + 1) + " CONTROL = " + luzControl);
            // Update the capacity records with new data
            for (k = 0; k < nTCap[i]; k++)
            {
                if (tcapd[zid, k].done)
                    writeMhDecrements(tcapd[zid, k]);      /* Write the update data to ASCII for bulk load */
            }   // end for k
        }     // End for i

        // Close the output file
        try
        {
            mhCd.Close();
        }
        catch (IOException io)
        {
            MessageBox.Show(io.Message);
        }

        loadDecrements(3);        /* Bulk load the ASCII data to the _dec table */
        updateMhDecrements();
    }     // End method mhDecrements()

    /*****************************************************************************/

    /* method mhIncrements() */
    /// <summary>
    /// Method to distribute LUZ mh increments to transactions.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 11/18/97   tb    Initial coding
    *                 08/06/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    void mhIncrements(int[] list, int counter)
    {
        SqlDataReader rdr;

        int i, k, zid, nt, luz, mgra, LCKey, mh, chgmh;
        int lz,counterMHI = 0;
        int summ;
        int[] ntCap = new int[NUM_LUZS];
        int[] tempmh = new int[MAX_TRANS];
        int luzControl, ret;
        double pCap;
        // -------------------------------------------------------------------------

        try
        {
            mhCi = new StreamWriter(new FileStream(networkPath + String.Format(appSettings["mhCi"].Value), FileMode.Create));
            mhCi.AutoFlush = true;
        }
        catch (IOException e)
        {
            MessageBox.Show(e.ToString(), "IO Exception");
            Close();
        }

        //sqlCommand.CommandText = "SELECT luz, mgra, LCKey, pcap_hs, hs_mh, " +
        //                        "chg_hs_mh FROM " + TN.capacity3 + " WHERE scenario = " + scenarioID + " and increment = " + bYear +
        //                        " and hs_mh > 0 AND site = 0 AND dev_code NOT BETWEEN 5 and 9 AND phase < " + fYear;
        sqlCommand.CommandText = string.Format(appSettings["select20"].Value, TN.capacity3, scenarioID, bYear, fYear);
        try
        {
            sqlConnection.Open();
            rdr = sqlCommand.ExecuteReader();
            while (rdr.Read())
            {
                luz = rdr.GetInt16(0);
                mgra = rdr.GetInt16(1);
                LCKey = rdr.GetInt32(2);
                pCap = rdr.GetDouble(3);
                mh = rdr.GetInt32(4);
                chgmh = rdr.GetInt32(5);
                zid = 999;      // Set the default for not found

                // Check to see if this LUZ is in this list
                for (k = 0; k < counter; k++)
                {
                    if (list[k] == luz)
                    {
                        zid = k;      // Save the list position as index
                        break;
                    }  // end if
                }   // end for k

                if (zid != 999)      // Do this only for good indexes
                {
                    nt = ntCap[zid];      // Get the second dimension index
                    tCapI[zid, nt] = new TCap();
                    tCapI[zid, nt].luz = luz;
                    tCapI[zid, nt].mgra = mgra;
                    tCapI[zid, nt].LCKey = LCKey;
                    tCapI[zid, nt].pCap_hs = pCap;
                    tCapI[zid, nt].mh = mh;
                    tCapI[zid, nt].chgmh = chgmh;
                    tCapI[zid, nt].done = false;
                    ntCap[zid]++;     // Increment the second dimension index

                    // Check for max array size, MAX_TRANS is somewhat arbitrary
                    if (ntCap[zid] == MAX_TRANS)
                    {
                        MessageBox.Show("FATAL ERROR - in mhIncrements tCapI array - index exceeds array bound = " + MAX_TRANS);
                        Close();
                    }   // end if
                }   // end if
            }     // End while
            rdr.Close();
        }     // End try

        catch (Exception e)
        {
            MessageBox.Show(e.ToString(), "Runtime Exception");
            Close();
        }
        finally
        {
            sqlConnection.Close();
        }

        for (i = 0; i < counter; i++)
        {
            zid = i;
            lz = list[i] - 1;     // Get LUZ ID
            luzControl = z[lz].fcst.hsi.mhAdj;      // Store mh as luzControl

            // Store mh in temp array for pachinko call
            summ = 0;
            for (k = 0; k < ntCap[zid]; k++)
            {
                tempmh[k] = tCapI[zid, k].mh;
                summ += tCapI[zid, k].mh;
            }   // end for k

            if (ntCap[zid] > 0)
            {
                ret = UDMUtils.pachinko(this, (summ + luzControl), tempmh, ntCap[zid]);
                if (ret > 0)     // Is there capacity remaining
                {
                    MessageBox.Show("EXCEPTION - Cannot allocate LUZ MH increment to transactions, LUZ " + (lz + 1) + " remainder = " + ret);
                }   // end if

                // Restore rounded mh forecast
                for (k = 0; k < ntCap[zid]; k++)
                {
                    if (tCapI[zid, k].mh != tempmh[k])
                    {
                        // Store increment
                        tCapI[zid, k].chgmh += tempmh[k] - tCapI[zid, k].mh;
                        tCapI[zid, k].mh = tempmh[k];      // Replace mh
                        tCapI[zid, k].done = true;         // Mark this guy as changed
                    }   // end if
                }   // end for k

                // Update the capacity records with new data
                for (k = 0; k < ntCap[i]; k++)
                {
                    //  Skip those records whose flags are not set
                    if (tCapI[zid, k].done)
                    {
                        // Write the update date to ASCII for bulk loading
                        mhCi.WriteLine(tCapI[zid, k].luz + "," + tCapI[zid, k].LCKey + "," + tCapI[zid, k].mh + "," + tCapI[zid, k].chgmh);
                        ++counterMHI;
                    }   // end if
                }   // end for k
            }     // End if
        }     // End for i

        mhCi.Flush();

        mhCi.Close();

        if (counterMHI > 0)
        {
            loadIncrements(3);
            updateMHIncrements();     // Update capacity from the mhIncrements table
        }  // end if

    }     // End method mhIncrements()

    /*****************************************************************************/

    /* method mhTransactions() */
    /// <summary>
    /// Method to distribute LUZ mh HS to transcations - master control routine
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 11/17/97   tb    Initial coding
    *                 08/06/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    void mhTransactions()
    {
        int ki = 0, kd = 0, i;
        int[] iList = new int[NUM_LUZS];
        int[] dList = new int[NUM_LUZS];

        //writeToStatusBox( "Distributing HS mh to transactions.." );

        for (i = 0; i < NUM_LUZS; i++)
        {
            // Build increment list of LUZs
            if (z[i].fcst.hsi.mhAdj >= 0)
                iList[ki++] = i + 1;
            else
                dList[kd++] = i + 1;
        }   // end for i

        // Call increments and decrements
        if (ki > 0)
            mhIncrements(iList, ki);
        if (kd > 0)
            mhDecrements(dList, kd);

    }     // End method mhTransactions()

    /*****************************************************************************/

    #endregion MH Procs

    #region Open files

    //*******************************************************************************
    /* method openFiles() */
    /// <summary>
    /// Method to open all of the output files for housing.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 07/07/97   tb    Initial coding
    *                 07/22/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    void openFiles()
    {
        try
        {
            hsChange = new StreamWriter(new FileStream(networkPath + String.Format(appSettings["hsChange"].Value), FileMode.Create));
            hsChange1 = new StreamWriter(new FileStream(networkPath + String.Format(appSettings["hsChange1"].Value), FileMode.Create));
            hsRates = new StreamWriter(new FileStream(networkPath + String.Format(appSettings["hsRates"].Value), FileMode.Create));
            hsOut = new StreamWriter(new FileStream(networkPath + String.Format(appSettings["hsOut"].Value), FileMode.Create));
            hsOvr = new StreamWriter(new FileStream(networkPath + String.Format(appSettings["hsOvr"].Value), FileMode.Create));
            redisHS = new StreamWriter(new FileStream(networkPath + String.Format(appSettings["redisHS"].Value), FileMode.Create));
        }
        catch (IOException io)
        {
            MessageBox.Show("Error opening file.  " + io.Message, "IOException");
        }

        // Assert auto flush on the formatted output files.
        hsChange.AutoFlush = hsChange1.AutoFlush = hsRates.AutoFlush = hsOut.AutoFlush = hsOut.AutoFlush = hsOvr.AutoFlush = redisHS.AutoFlush = true;

        hsRates.WriteLine("HS VACANCY RATES FILE");
        redisHS.WriteLine("LUZ HS REDISTRIBUTION FILE");
    }     // End method openFiles()

    /*****************************************************************************/

    #endregion Open Files

    #region Print Procs
    /* method printAuxHS() */
    /// <summary>
    /// Method to write HS auxillary reports to ASCII.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 07/02/97   tb    Initial coding
    *                 08/07/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    void printAuxHS()
    {
        // Overrides file
        if (flags.hOvr)
            printHSOvr();

        // Outliers file
        if (flags.hOut)
            printHSOutliers();

        // HS change file
        if (flags.housingChange)
        {
            printHS1();
            printHS2();
            printHS3();
            printHS4();
            printHS5();
            printHS6();
            printHSTableSpecial();
        }   // end if
    }     // End method printAuxHS()

    /*****************************************************************************/

    /* method printHS1() */
    /// <summary>
    /// Method to print HS change report #1 (unit change) to ASCII.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 11/26/97   tb    Initial coding
    *                 08/07/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    void printHS1()
    {
        string title11 = " LUZ     Base     Fcst      Chg     %Chg     Chg5   " +
                        " %Chg5   CapShr OutRatio";
        string title12 = "----------------------------------------------------" +
                        "------------------------";
        string title13 = " LUZ     Base     Fcst      Chg     %Chg";
        string title14 = "----------------------------------------";

        int i, lineCount;
        int r1, r2, r3, r4, r5;
        double chg1, r8;

        /****** SF CHANGE ******/
        hsChange.WriteLine("Table 3-1a");
        hsChange.WriteLine("SINGLE FAMILY UNIT CHANGE " + outputLabel);
        hsChange.WriteLine(title11);
        hsChange.WriteLine(title12);
        lineCount = 0;
        r8 = r1 = r2 = r3 = r4 = r5 = 0;

        for (i = 0; i < NUM_LUZS; i++)
        {
            if (reg.capacity.totalSF > 0)
                z[i].fcst.sfCapShare = (int)(0.5 + rc.fcst.hsi.sfAdj * ((double)z[i].capacity.totalSF /
                                      (double)reg.capacity.totalSF));
            if (z[i].fcst.hsi.sf > 0)
                chg1 = (double)z[i].fcst.hsi.sf / (double)z[i].baseData.hs.sf * 100;
            else
                chg1 = 0;

            hsChange.WriteLine("{0,4}{1,9}{2,9}{3,9}{4,9:F1}{5,9}{6,9:F1}{7,9}" +
                                  "{8,9:F1}", (i + 1), z[i].baseData.hs.sf,
                                  z[i].fcst.hs.sf, z[i].fcst.hsi.sf, chg1,
                                  z[i].histSF.c5, z[i].histSF.pct5,
                                  z[i].fcst.sfCapShare, z[i].sfOut.r5);
            r1 += z[i].baseData.hs.sf;
            r2 += z[i].fcst.hs.sf;
            r3 += z[i].fcst.hsi.sf;
            r4 += z[i].histSF.c5;
            r5 += z[i].fcst.sfCapShare;
            lineCount++;

            if (lineCount >= 57)
            {
                hsChange.WriteLine();
                lineCount = 0;      // Reset line count
                hsChange.WriteLine("Table 3-1a");
                hsChange.WriteLine("SINGLE FAMILY UNIT CHANGE " + outputLabel);
                hsChange.WriteLine(title11);
                hsChange.WriteLine(title12);
            }   // end if
        }     // End for

        if (r1 > 0)
            r8 = (double)r3 / (double)r1 * 100;
        else
            r8 = 0;
        if (reg.fcst.hsi.sf > 0)
            chg1 = (double)reg.fcst.hsi.sf / (double)reg.baseData.hs.sf * 100;
        else
            chg1 = 0;
        hsChange.WriteLine();
        hsChange.WriteLine("Sum{0,9}{1,9}{2,9}{3,9:F1}{4,9}      N/A{5,9}    " + "  N/A", r1, r2, r3, r8, r4, r5);
        hsChange.WriteLine("Reg{0,9}{1,9}{2,9}{3,9:F1}{4,9}{5,9:F1}{6,9}     " + " N/A", reg.baseData.hs.sf, reg.fcst.hs.sf,
                            reg.fcst.hsi.sf, chg1, reg.histSF.c5, reg.histSF.pct5, reg.fcst.hsi.sf);

        /****** MF CHANGE ******/
        lineCount = 0;
        hsChange.WriteLine();
        hsChange.WriteLine();
        hsChange.WriteLine("Table 3-1b");
        hsChange.WriteLine("MULTI FAMILY UNIT CHANGE " + outputLabel);
        hsChange.WriteLine(title11);
        hsChange.WriteLine(title12);

        r8 = r1 = r2 = r3 = r4 = r5 = 0;

        for (i = 0; i < NUM_LUZS; i++)
        {
            if (z[i].fcst.hsi.mf > 0)
                chg1 = (double)z[i].fcst.hsi.mf / (double)z[i].baseData.hs.mf * 100;
            else
                chg1 = 0;

            if (reg.capacity.totalSF > 0)
                z[i].fcst.mfCapShare = (int)(0.5 + rc.fcst.hsi.mfAdj * ((double)z[i].capacity.totalMF /
                                      (double)reg.capacity.totalMF));
            hsChange.WriteLine("{0,4}{1,9}{2,9}{3,9}{4,9:F1}{5,9}{6,9:F1}{7,9}" + "{8,9:F1}", (i + 1), z[i].baseData.hs.mf,
                                z[i].fcst.hs.mf, z[i].fcst.hsi.mf, chg1, z[i].histMF.c5, z[i].histMF.pct5,
                                z[i].fcst.mfCapShare, z[i].mfOut.r5);
            r1 += z[i].baseData.hs.mf;
            r2 += z[i].fcst.hs.mf;
            r3 += z[i].fcst.hsi.mf;
            r4 += z[i].histMF.c5;
            r5 += z[i].fcst.mfCapShare;
            lineCount++;

            if (lineCount >= 57)
            {
                hsChange.WriteLine();
                hsChange.WriteLine();
                lineCount = 0;      // Reset line count
                hsChange.WriteLine("Table 3-1b");
                hsChange.WriteLine("MULTI FAMILY UNIT CHANGE " + outputLabel);
                hsChange.WriteLine(title11);
                hsChange.WriteLine(title12);
            }   // end if
        }     // End for

        if (r1 > 0)
            r8 = (double)r3 / (double)r1 * 100;
        else
            r8 = 0;

        if (reg.fcst.hsi.mf > 0)
            chg1 = (double)reg.fcst.hsi.mf / (double)reg.baseData.hs.mf * 100;
        else
            chg1 = 0;
        hsChange.WriteLine();
        hsChange.WriteLine("Sum{0,9}{1,9}{2,9}{3,9:F1}{4,9}      " + "N/A{5,9}      N/A", r1, r2, r3, r8, r4, r5);
        hsChange.WriteLine("Reg{0,9}{1,9}{2,9}{3,9:F1}{4,9}{5,9:F1}{6,9}     " +
                            " N/A", reg.baseData.hs.mf, reg.fcst.hs.mf, reg.fcst.hsi.mf, chg1, reg.histMF.c5,
                            reg.histMF.pct5, reg.fcst.hsi.mf);

        /****** mh CHANGE ******/
        hsChange.WriteLine();
        hsChange.WriteLine();
        hsChange.WriteLine("Table 3-1c");
        hsChange.WriteLine("MOBILE HOME   UNIT CHANGE " + outputLabel);
        hsChange.WriteLine(title13);
        hsChange.WriteLine(title14);
        lineCount = 0;
        r8 = r1 = r2 = r3 = 0;

        for (i = 0; i < NUM_LUZS; i++)
        {
            if (z[i].fcst.hsi.mh > 0)
                chg1 = (double)z[i].fcst.hsi.mh / (double)z[i].baseData.hs.mh * 100;
            else
                chg1 = 0;

            hsChange.WriteLine("{0,4}{1,9}{2,9}{3,9}{4,9:F1}", (i + 1), z[i].baseData.hs.mh, z[i].fcst.hs.mh,
                                z[i].fcst.hsi.mh, chg1);
            r1 += z[i].baseData.hs.mh;
            r2 += z[i].fcst.hs.mh;
            r3 += z[i].fcst.hsi.mh;

            lineCount++;

            if (lineCount >= 57)
            {
                hsChange.WriteLine();
                hsChange.WriteLine();
                lineCount = 0;      // Reset line count
                hsChange.WriteLine(title13);
                hsChange.WriteLine(title14);
            }   // end if
        }   // end for i

        if (r1 > 0)
            r8 = (double)r3 / (double)r1 * 100;
        else
            r8 = 0;

        if (reg.fcst.hsi.mh > 0)
            chg1 = (double)reg.fcst.hsi.mh / (double)reg.baseData.hs.mh * 100;
        else
            chg1 = 0;
        hsChange.WriteLine();
        hsChange.WriteLine("Sum{0,9}{1,9}{2,9}{3,9:F1}", r1, r2, r3, r8);
        hsChange.WriteLine("Reg{0,9}{1,9}{2,9}{3,9:F1}", reg.baseData.hs.mh, reg.fcst.hs.mh, reg.fcst.hsi.mh, chg1);
    }     // End method printHS1()

    /*****************************************************************************/

    /* method printHS2() */
    /// <summary>
    /// Method to print HS change report # 2 (components of change) to ASCII.

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 11/26/97   tb    Initial coding
    *                 08/07/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    void printHS2()
    {
        string title21 = "                           Lost     Lost     Pgrm";
        string title22 = " LUZ    Total     Site   EmpRed    MFRed      Chg";
        string title23 = "-------------------------------------------------";

        string title31 = "                           Lost     Pgrm";
        string title32 = " LUZ    Total     Site   EmpRed      Chg";
        string title33 = "----------------------------------------";
        string title41 = " LUZ    Total       SF       MF       mh";
        string title42 = "----------------------------------------";

        int i;
        int lineCount;
        int r1, r2, r3, r4, r5;

        // SF
        r1 = r2 = r3 = r4 = r5 = 0;

        hsChange.WriteLine();
        hsChange.WriteLine();
        lineCount = 0;
        hsChange.WriteLine("Table 3-2a");
        hsChange.WriteLine("COMPONENTS OF SF UNIT CHANGE " + outputLabel);
        hsChange.WriteLine(title21);
        hsChange.WriteLine(title22);
        hsChange.WriteLine(title23);

        reg.fcst.hsi.sfAdj = 0;

        for (i = 0; i < NUM_LUZS; i++)
        {
            hsChange.WriteLine("{0,4}{1,9}{2,9}{3,9}{4,9}{5,9}", (i + 1),
                                z[i].fcst.hsi.sf, z[i].site.sf, z[i].fcst.le.sf,
                                z[i].fcst.lh.sf, z[i].fcst.hsi.sfAdj);
            r1 += z[i].fcst.hsi.sf;
            r2 += z[i].site.sf;
            r3 += z[i].fcst.le.sf;
            r4 += z[i].fcst.lh.sf;
            r5 += z[i].fcst.hsi.sfAdj;
            reg.fcst.hsi.sfAdj += z[i].fcst.hsi.sfAdj;
            lineCount++;

            if (lineCount >= 57)
            {
                hsChange.WriteLine();
                hsChange.WriteLine();
                lineCount = 0;      // Reset line count
                hsChange.WriteLine("Table 3-2a");
                hsChange.WriteLine("COMPONENTS OF SF UNIT CHANGE " + outputLabel);
                hsChange.WriteLine(title21);
                hsChange.WriteLine(title22);
                hsChange.WriteLine(title23);
            }   // end if
        }   // end for i

        hsChange.WriteLine();
        hsChange.WriteLine("Sum{0,9}{1,9}{2,9}{3,9}{4,9}", r1, r2, r3, r4, r5);

        hsChange.WriteLine("Reg{0,9}{1,9}{2,9}{3,9}{4,9}", reg.fcst.hsi.sf,
                            reg.site.sf, reg.fcst.le.sf, reg.fcst.lh.sf,
                            reg.fcst.hsi.sfAdj);

        // MF
        r1 = r2 = r3 = r4 = r5 = 0;

        lineCount = 0;
        hsChange.WriteLine();
        hsChange.WriteLine();
        hsChange.WriteLine("Table 3-2b");
        hsChange.WriteLine("COMPONENTS OF MF UNIT CHANGE " + outputLabel);
        hsChange.WriteLine(title31);
        hsChange.WriteLine(title32);
        hsChange.WriteLine(title33);

        for (i = 0; i < NUM_LUZS; i++)
        {
            hsChange.WriteLine("{0,4}{1,9}{2,9}{3,9}{4,9}", (i + 1),
                                  z[i].fcst.hsi.mf, z[i].site.mf, z[i].fcst.le.mf,
                                  z[i].fcst.hsi.mfAdj);
            r1 += z[i].fcst.hsi.mf;
            r2 += z[i].site.mf;
            r3 += z[i].fcst.le.mf;
            r4 += z[i].fcst.hsi.mfAdj;
            lineCount++;

            if (lineCount >= 57)
            {
                hsChange.WriteLine();
                hsChange.WriteLine();
                lineCount = 0;      // Reset line
                hsChange.WriteLine("Table 3-2b");
                hsChange.WriteLine("COMPONENTS OF MF UNIT CHANGE " + outputLabel);
                hsChange.WriteLine(title31);
                hsChange.WriteLine(title32);
                hsChange.WriteLine(title33);
            }   // end if
        }   // end for i

        hsChange.WriteLine();
        hsChange.WriteLine("Sum{0,9}{1,9}{2,9}{3,9}", r1, r2, r3, r4);

        hsChange.WriteLine("Reg{0,9}{1,9}{2,9}{3,9}", reg.fcst.hsi.mf, reg.site.mf, reg.fcst.le.mf, reg.fcst.hsi.mfAdj);

        // mh
        r1 = r2 = r3 = r4 = r5 = 0;

        lineCount = 0;
        hsChange.WriteLine();
        hsChange.WriteLine();
        hsChange.WriteLine("Table 3-2c");
        hsChange.WriteLine("COMPONENTS OF mh UNIT CHANGE " + outputLabel);
        hsChange.WriteLine(title21);
        hsChange.WriteLine(title22);
        hsChange.WriteLine(title23);

        for (i = 0; i < NUM_LUZS; i++)
        {
            hsChange.WriteLine("{0,4}{1,9}{2,9}{3,9}{4,9}{5,9}", (i + 1),
                                z[i].fcst.hsi.mh, z[i].site.mh, z[i].fcst.le.mh,
                                z[i].fcst.lh.mh, z[i].fcst.hsi.mhAdj);
            r1 += z[i].fcst.hsi.mh;
            r2 += z[i].site.mh;
            r3 += z[i].fcst.le.mh;
            r4 += z[i].fcst.lh.mh;
            r5 += z[i].fcst.hsi.mhAdj;

            lineCount++;

            if (lineCount >= 57)
            {
                hsChange.WriteLine();
                hsChange.WriteLine();
                lineCount = 0;      // Reset line count
                hsChange.WriteLine("Table 3-2c");
                hsChange.WriteLine("COMPONENTS OF mh UNIT CHANGE " + outputLabel);
                hsChange.WriteLine(title21);
                hsChange.WriteLine(title22);
                hsChange.WriteLine(title23);
            }   // end if
        }    // end for i     
        hsChange.WriteLine();
        hsChange.WriteLine("Sum{0,9}{1,9}{2,9}{3,9}{4,9}", r1, r2, r3, r4, r5);
        hsChange.WriteLine("Reg{0,9}{1,9}{2,9}{3,9}{4,9}", reg.fcst.hsi.mh,
                            reg.site.mh, reg.fcst.le.mh, reg.fcst.lh.mh,
                            reg.fcst.hsi.mhAdj);

        // Total
        r1 = r2 = r3 = r4 = r5 = 0;

        lineCount = 0;
        hsChange.WriteLine();
        hsChange.WriteLine();
        hsChange.WriteLine("Table 3-2d");
        hsChange.WriteLine("COMPONENTS OF HS UNIT CHANGE " + outputLabel);
        hsChange.WriteLine(title41);
        hsChange.WriteLine(title42);

        for (i = 0; i < NUM_LUZS; i++)
        {
            hsChange.WriteLine("{0,4}{1,9}{2,9}{3,9}{4,9}", (i + 1),
                                z[i].fcst.hsi.total, z[i].fcst.hsi.sf,
                                z[i].fcst.hsi.mf, z[i].fcst.hsi.mh);
            r1 += z[i].fcst.hsi.total;
            r2 += z[i].fcst.hsi.sf;
            r3 += z[i].fcst.hsi.mf;
            r4 += z[i].fcst.hsi.mh;
            lineCount++;

            if (lineCount >= 57)
            {
                hsChange.WriteLine();
                hsChange.WriteLine();
                lineCount = 0;      // Reset line count
                hsChange.WriteLine("Table 3-2d");
                hsChange.WriteLine("COMPONENTS OF HS UNIT CHANGE " + outputLabel);
                hsChange.WriteLine(title41);
                hsChange.WriteLine(title42);
            }   // end if
        }   // end for i
        hsChange.WriteLine();
        hsChange.WriteLine("Sum{0,9}{1,9}{2,9}{3,9}", r1, r2, r3, r4);
        hsChange.WriteLine("Reg{0,9}{1,9}{2,9}{3,9}", reg.fcst.hsi.total,
                            reg.fcst.hsi.sf, reg.fcst.hsi.mf,
                            reg.fcst.hsi.mh);
    }     // End procedure printHS2()

    /*****************************************************************************/

    /* method printHS3() */
    /// <summary>
    /// Method to print HS change report #3 (change by land use) to ASCII.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 11/26/97   tb    Initial coding
    *                 08/07/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    void printHS3()
    {
        string title30 = "                       Unit Change          " +
                        "                   Remaining Capacity";
        string title31 = " LUZ    Redev   Infill  Vac-Low  Vac-Urb    " +
                        "Total    Redev   Infill  Vac-Low  Vac_Urb   Total";
        string title32 = "----------------------------------------------------" +
                        "-----------------------------------------";
        string title40 = "                   Unit Change                    " +
                        "Remaining Capacity";
        string title41 = " LUZ    Redev   Infill   Vac-Ag    Total    " +
                        "Redev   Infill   Vac-Ag   Total";
        string title42 = "---------------------------------------------------------------------------";

        int i, j, lineCount, total;
        int[] ra = new int[11];

        // SF
        lineCount = 0;
        hsChange1.WriteLine();
        hsChange1.WriteLine();
        hsChange1.WriteLine("Table 3-3a");
        hsChange1.WriteLine("SF UNIT CHANGE (TOTAL) BY LAND USE " + outputLabel);
        hsChange1.WriteLine(title30);
        hsChange1.WriteLine(title31);
        hsChange1.WriteLine(title32);

        for (i = 0; i < NUM_LUZS; i++)
        {
            total = 0;
            hsChange1.Write("{0,4}", (i + 1));
            for (j = 1; j < NUM_SF_LAND; j++)
            {
                hsChange1.Write("{0,9}", z[i].fcst.hsi.sfLand[j]);
                ra[j] += z[i].fcst.hsi.sfLand[j];
                total += z[i].fcst.hsi.sfLand[j];
            }   // end for j

            ra[5] += total;
            hsChange1.Write("{0,9}", total);
            total = 0;
            for (j = 1; j < NUM_SF_LAND; j++)
            {
                hsChange1.Write("{0,9}", z[i].remCap.sf[j]);
                ra[j + 5] += z[i].remCap.sf[j];
                total += z[i].remCap.sf[j];
            }   // end for j
            ra[10] += total;
            hsChange1.WriteLine("{0,9}", total);
            lineCount++;

            if (lineCount >= 30)
            {
                hsChange1.WriteLine();
                hsChange1.WriteLine();
                lineCount = 0;      // Reset line count
                hsChange1.WriteLine("Table 3-3a");
                hsChange1.WriteLine("SF UNIT CHANGE (TOTAL) BY LAND USE " + outputLabel);
                hsChange1.WriteLine(title30);
                hsChange1.WriteLine(title31);
                hsChange1.WriteLine(title32);
            }   // end if
        }     // End for i

        hsChange1.WriteLine();
        hsChange1.Write("Sum");
        for (j = 1; j < 11; j++)
            hsChange1.Write("{0,9}", ra[j]);
        hsChange1.WriteLine();

        total = 0;
        hsChange1.Write(" Reg");
        for (j = 1; j < NUM_SF_LAND; j++)
        {
            hsChange1.Write("{0,9}", reg.fcst.hsi.sfLand[j]);
            total += reg.fcst.hsi.sfLand[j];
        }   // end for j
        hsChange1.Write("{0,9}", total);

        total = 0;
        for (j = 1; j < NUM_SF_LAND; j++)
        {
            hsChange1.Write("{0,9}", reg.remCap.sf[j]);
            total += reg.remCap.sf[j];
        }   // end for j
        hsChange1.WriteLine("{0,9}", total);

        // MF
        lineCount = 0;
        hsChange1.WriteLine();
        hsChange1.WriteLine();
        hsChange1.WriteLine("Table 3-3b");
        hsChange1.WriteLine("MF UNIT CHANGE (TOTAL) BY LAND USE " + outputLabel);
        hsChange1.WriteLine(title40);
        hsChange1.WriteLine(title41);
        hsChange1.WriteLine(title42);

        for (i = 0; i < NUM_LUZS; i++)
        {
            total = 0;
            hsChange1.Write("{0,4}", i + 1);

            for (j = 1; j < NUM_MF_LAND; j++)
            {
                hsChange1.Write("{0,9}", z[i].fcst.hsi.mfLand[j]);
                ra[j] += z[i].fcst.hsi.mfLand[j];
                total += z[i].fcst.hsi.mfLand[j];
            }   // end for j

            ra[4] += total;
            hsChange1.Write("{0,9}", total);

            total = 0;
            for (j = 1; j < NUM_MF_LAND; j++)
            {
                hsChange1.Write("{0,9}", z[i].remCap.mf[j]);
                ra[j + 4] += z[i].remCap.mf[j];
                total += z[i].remCap.mf[j];
            }   // end for j

            ra[8] += total;
            hsChange1.WriteLine("{0,9}", total);
            lineCount++;

            if (lineCount >= 30)
            {
                hsChange1.WriteLine();
                hsChange1.WriteLine();
                lineCount = 0;      // Reset line count
                hsChange1.WriteLine("Table 3-3b");
                hsChange1.WriteLine("MF UNIT CHANGE (TOTAL) BY LAND USE " + outputLabel);
                hsChange1.WriteLine(title40);
                hsChange1.WriteLine(title41);
                hsChange1.WriteLine(title42);
            }   // end if
        }     // End for i

        hsChange1.WriteLine();
        hsChange1.Write("Sum");
        total = 0;

        for (j = 1; j < 9; j++)
            hsChange1.Write("{0,9}", ra[j]);
        hsChange1.WriteLine();

        hsChange1.Write("Reg");
        for (j = 1; j < NUM_MF_LAND; j++)
        {
            hsChange1.Write("{0,9}", reg.fcst.hsi.mfLand[j]);
            total += reg.fcst.hsi.mfLand[j];
        }   // end for j
        hsChange1.Write("{0,9}", total);

        total = 0;
        for (j = 1; j < NUM_MF_LAND; j++)
        {
            hsChange1.Write("{0,9}", reg.remCap.mf[j]);
            total += reg.remCap.mf[j];
        }   // end for j
        hsChange1.WriteLine("{0,9}", total);
    }     // End method printHS3()

    /*****************************************************************************/

    /* method printHS4() */
    /// <summary>
    /// Method to write HS change report #4 (hs % capacity) to ASCII.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 11/26/97   tb    Initial coding
    *                 08/07/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    void printHS4()
    {
        string title41 = " LUZ    Redev   Infill  Vac-Low  Vac-Urb    Total";
        string title42 = "-------------------------------------------------";

        string title51 = " LUZ    Redev   Infill   Vac-Ag    Total";
        string title52 = "----------------------------------------";
        int i, j, lineCount, tc, th;
        double pct;
        double[] rb = new double[6], rc = new double[6];

        // SF
        lineCount = 0;
        hsChange.WriteLine();
        hsChange.WriteLine();
        hsChange.WriteLine("Table 3-4a");
        hsChange.WriteLine("SF - PERCENT OF CAPACITY BY LAND USE " + outputLabel);
        hsChange.WriteLine(title41);
        hsChange.WriteLine(title42);
        for (i = 0; i < NUM_LUZS; i++)
        {
            th = tc = 0;
            hsChange.Write("{0,4}", (i + 1));
            for (j = 1; j < NUM_SF_LAND; j++)
            {
                th += z[i].fcst.hsi.sfLand[j];
                tc += z[i].capacity.sf[j];
                rb[j] += z[i].fcst.hsi.sfLand[j];
                rc[j] += z[i].capacity.sf[j];
                z[i].useCap.sf[j] = z[i].fcst.hsi.sfLand[j];
                z[i].useCap.totalSF += z[i].useCap.sf[j];
                if (z[i].capacity.sf[j] > 0)
                    z[i].useCap.pSF[j] = pct = (double)z[i].useCap.sf[j] / (double)z[i].capacity.sf[j] * 100;
                else
                    pct = 0;
                hsChange.Write("{0,9:F2}", pct);
            }   // end for

            if (z[i].capacity.totalSF > 0)
                z[i].useCap.pTotalSF = (double)z[i].useCap.totalSF / (double)z[i].capacity.totalSF * 100;

            rb[5] += th;
            rc[5] += tc;

            if (tc > 0)
                hsChange.WriteLine("{0,9:F2}", (double)th / (double)tc * 100);
            else
                hsChange.WriteLine("     0.00");
            lineCount++;
            if (lineCount >= 57)
            {
                hsChange.WriteLine();
                hsChange.WriteLine();
                lineCount = 0;      // Reset line count
                hsChange.WriteLine("Table 3-4a");
                hsChange.WriteLine("SF - PERCENT OF CAPACITY BY LAND USE " + outputLabel);
                hsChange.WriteLine(title41);
                hsChange.WriteLine(title42);
            }   // end if
        }     // End for i

        hsChange.WriteLine();
        hsChange.Write("Sum");
        for (j = 1; j < 6; j++)
        {
            if (rc[j] > 0)
                pct = (double)rb[j] / (double)rc[j] * 100;
            else
                pct = 0;
            hsChange.Write("{0,9:F2}", pct);
        }   // end for j

        hsChange.WriteLine();
        th = tc = 0;
        hsChange.Write("Reg");

        for (j = 1; j < NUM_SF_LAND; j++)
        {
            th += reg.fcst.hsi.sfLand[j];
            tc += reg.capacity.sf[j];
            if (reg.capacity.sf[j] > 0)
                pct = (double)reg.fcst.hsi.sfLand[j] / (double)reg.capacity.sf[j] * 100;
            else
                pct = 0;

            hsChange.Write("{0,9:F2}", pct);
        }   // end for j

        if (tc > 0)
            hsChange.WriteLine("{0,9:F2}", (double)th / (double)tc * 100);

        // MF
        lineCount = 0;
        hsChange.WriteLine();
        hsChange.WriteLine();
        hsChange.WriteLine("Table 3-4b");
        hsChange.WriteLine("MF - PERCENT OF CAPACITY BY LAND USE");
        hsChange.WriteLine(title51);
        hsChange.WriteLine(title52);

        for (i = 0; i < NUM_LUZS; i++)
        {
            th = tc = 0;
            hsChange.Write("{0,4}", (i + 1));

            for (j = 1; j < NUM_MF_LAND; j++)
            {
                th += z[i].fcst.hsi.mfLand[j];
                tc += z[i].capacity.mf[j];
                rb[j] += z[i].fcst.hsi.mfLand[j];
                rc[j] += z[i].capacity.mf[j];
                z[i].useCap.mf[j] = z[i].fcst.hsi.mfLand[j];
                z[i].useCap.totalMF += z[i].useCap.mf[j];
                if (z[i].capacity.mf[j] > 0)
                    z[i].useCap.pMF[j] = pct = (double)z[i].fcst.hsi.mfLand[j] / (double)z[i].capacity.mf[j] * 100;
                else
                    pct = 0;
                hsChange.Write("{0,9:F2}", pct);
            }   // end for j

            if (z[i].capacity.totalMF > 0)
                z[i].useCap.pTotalMF = (double)z[i].useCap.totalMF / (double)z[i].capacity.totalMF * 100;

            rb[4] += th;
            rc[4] += tc;

            if (tc > 0)
                hsChange.WriteLine("{0,9:F2}", (double)th / (double)tc * 100);
            else
                hsChange.WriteLine("     0.00");
            lineCount++;

            if (lineCount >= 57)
            {
                hsChange.WriteLine();
                hsChange.WriteLine();
                lineCount = 0;      // Reset line count
                hsChange.WriteLine("Table 3-4b");
                hsChange.WriteLine("MF - PERCENT OF CAPACITY BY LAND USE");
                hsChange.WriteLine(title51);
                hsChange.WriteLine(title52);
            }   // end if
        }     // End for i

        hsChange.WriteLine();
        hsChange.Write("Sum");
        for (j = 1; j < 5; j++)
        {
            if (rc[j] > 0)
                pct = (double)rb[j] / (double)rc[j] * 100;
            else
                pct = 0;
            hsChange.Write("{0,9:F2}", pct);
        }   // end for j
        hsChange.WriteLine();

        th = tc = 0;
        hsChange.Write("Reg");
        for (j = 1; j < NUM_MF_LAND; ++j)
        {
            th += reg.fcst.hsi.mfLand[j];
            tc += reg.capacity.mf[j];
            if (reg.capacity.mf[j] > 0)
                pct = (double)reg.fcst.hsi.mfLand[j] / (double)reg.capacity.mf[j] * 100;
            else
                pct = 0;
            hsChange.Write("{0,9:F2}", pct);
        }
        if (tc > 0)
            hsChange.WriteLine("{0,9:F2}", (double)th / (double)tc * 100);
    }     // End procedure printHS4()

    /*****************************************************************************/

    /* method printHS5() */
    /// <summary>
    /// Method to write HS change report #5 (% dev acres by land use) to ASCII.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 11/26/97   tb    Initial coding
    *                 08/07/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    void printHS5()
    {
        string title51 = " LUZ    Redev   Infill  Vac-Low  Vac-Urb    Total";
        string title52 = "-------------------------------------------------";

        string title61 = " LUZ    Redev   Infill   Vac-Ag    Total";
        string title62 = "----------------------------------------";
        int i, j, lineCount;
        double th, tc, pct;
        double[] rb = new double[6], rc = new double[6];

        // SF
        lineCount = 0;
        hsChange.WriteLine();
        hsChange.WriteLine();
        hsChange.WriteLine("Table 3-5a");
        hsChange.WriteLine("SF - PERCENT OF DEVELOPABLE ACREAGE BY LAND USE " + outputLabel);
        hsChange.WriteLine(title51);
        hsChange.WriteLine(title52);

        for (i = 0; i < NUM_LUZS; i++)
        {
            th = tc = 0;
            hsChange.Write("{0,4}", (i + 1));
            for (j = 1; j < NUM_SF_LAND; j++)
            {
                th += z[i].useCap.ac.asf[j];
                tc += z[i].capacity.ac.asf[j];
                rb[j] += z[i].useCap.ac.asf[j];
                rc[j] += z[i].capacity.ac.asf[j];
                if (z[i].capacity.ac.asf[j] > 0)
                    pct = (double)z[i].useCap.ac.asf[j] / (double)z[i].capacity.ac.asf[j] * 100;
                else
                    pct = 0;
                rb[5] += z[i].useCap.ac.asf[j];
                rc[5] += z[i].capacity.ac.asf[j];
                hsChange.Write("{0,9:F2}", pct);
            }   // end for j

            if (tc > 0)
                hsChange.WriteLine("{0,9:F2}", (double)th / (double)tc * 100);
            else
                hsChange.WriteLine("     0.00");
            lineCount++;

            if (lineCount >= 57)
            {
                hsChange.WriteLine();
                hsChange.WriteLine();
                lineCount = 0;      // Reset line count
                hsChange.WriteLine("Table 3-5a");
                hsChange.WriteLine("SF - PERCENT OF DEVELOPABLE ACREAGE BY LAND USE " + outputLabel);
                hsChange.WriteLine(title51);
                hsChange.WriteLine(title52);
            }   // end if
        }     // End for i

        hsChange.WriteLine();
        hsChange.Write("Sum");
        for (j = 1; j < NUM_SF_LAND; j++)
        {
            if (rc[j] > 0)
                pct = (double)rb[j] / (double)rc[j] * 100;
            else
                pct = 0;

            hsChange.Write("{0,9:F2}", pct);
        }   // end for j

        if (rc[5] > 0)
            pct = (double)rb[5] / (double)rc[5] * 100;
        else
            pct = 0.0;
        hsChange.WriteLine("{0,9:F2}", pct);
        th = tc = 0;
        hsChange.Write("Reg");
        for (j = 1; j < NUM_SF_LAND; j++)
        {
            th += reg.useCap.ac.asf[j];
            tc += reg.capacity.ac.asf[j];
            if (reg.capacity.ac.asf[j] > 0)
                pct = (double)reg.useCap.ac.asf[j] / (double)reg.capacity.ac.asf[j] * 100;
            else
                pct = 0;
            hsChange.Write("{0,9:F2}", pct);
        }   // end for j

        if (tc > 0)
            hsChange.WriteLine("{0,9:F2}", (double)th / (double)tc * 100);


        // MF
        lineCount = 0;
        hsChange.WriteLine();
        hsChange.WriteLine();
        hsChange.WriteLine("Table 3-5b");
        hsChange.WriteLine("MF - PERCENT OF DEVELOPABLE ACREAGE BY LAND USE " +
                            outputLabel);
        hsChange.WriteLine(title61);
        hsChange.WriteLine(title62);

        for (i = 0; i < NUM_LUZS; i++)
        {
            th = tc = 0;
            hsChange.Write("{0,4}", (i + 1));
            for (j = 1; j < NUM_MF_LAND; ++j)
            {
                th += z[i].useCap.ac.amf[j];
                tc += z[i].capacity.ac.amf[j];
                rb[j] += z[i].useCap.ac.amf[j];
                rc[j] += z[i].capacity.ac.amf[j];
                if (z[i].capacity.ac.amf[j] > 0)
                    pct = (double)z[i].useCap.ac.amf[j] / (double)z[i].capacity.ac.amf[j] * 100;
                else
                    pct = 0;
                rb[4] += z[i].useCap.ac.amf[j];
                rc[4] += z[i].capacity.ac.amf[j];
                hsChange.Write("{0,9:F2}", pct);
            }   // end for j

            if (tc > 0)
                hsChange.WriteLine("{0,9:F2}", (double)th / (double)tc * 100);
            else
                hsChange.WriteLine("     0.00");
            lineCount++;

            if (lineCount >= 57)
            {
                hsChange.WriteLine();
                hsChange.WriteLine();
                lineCount = 0;      // Reset line count
                hsChange.WriteLine("Table 3-5b");
                hsChange.WriteLine("MF - PERCENT OF DEVELOPABLE ACREAGE BY LAND USE " + outputLabel);
                hsChange.WriteLine(title61);
                hsChange.WriteLine(title62);
            }   // end if
        }     // End for i

        hsChange.WriteLine();
        hsChange.WriteLine("Sum");
        for (j = 1; j < NUM_MF_LAND; j++)
        {
            if (rc[j] > 0)
                pct = (double)rb[j] / (double)rc[j] * 100;
            else
                pct = 0;
            hsChange.Write("{0,9:F2}", pct);
        }   // end for j

        if (rc[4] > 0)
            pct = (double)rb[4] / (double)rc[4] * 100;
        else
            pct = 0;

        hsChange.WriteLine("{0,9:F2}", pct);

        th = tc = 0;
        hsChange.Write("Reg");
        for (j = 1; j < NUM_MF_LAND; j++)
        {
            th += reg.useCap.ac.amf[j];
            tc += reg.capacity.ac.amf[j];
            if (reg.capacity.ac.amf[j] > 0)
                pct = (double)reg.useCap.ac.amf[j] / (double)reg.capacity.ac.amf[j] * 100;
            else
                pct = 0;
            hsChange.Write("{0,9:F2}", pct);
        }   // end for j

        if (tc > 0)
            hsChange.WriteLine("{0,9:F2}", (double)th / (double)tc * 100);
    }     // End method printHS5()

    /*****************************************************************************/

    /* method printHS6() */
    /// <summary>
    /// Method to write HS change report #6 (land consumption) to ASCII.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 11/26/97   tb    Initial coding
    *                 08/07/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    void printHS6()
    {
        string title61 = "                  Developable Acres Used" +
                        "                     Developable Acres Remaining";
        string title62 = " LUZ    Redev   Infill  Vac-Low  Vac-Urb    " +
                        "Total    Redev   Infill  Vac-Low  Vac-Urb    Total  ";
        string title63 = "----------------------------------------------------" +
                        "--------------------------------------------";
        string title71 = "             Developable Acres Used            " +
                        "Developable Acres Remaining";
        string title72 = " LUZ    Redev   Infill   Vac-Ag    Total    Redev" +
                        "   Infill   Vac-Ag    Total  ";
        string title73 = "---------------------------------------------------" +
                        "---------------------------";
        int i, j, lineCount = 0;
        double total;
        double[] rb = new double[6], rc = new double[6];

        // SF      
        hsChange1.WriteLine("Table 3-6a");
        hsChange1.WriteLine("SF - LAND CONSUMPTION " + outputLabel);
        hsChange1.WriteLine(title61);
        hsChange1.WriteLine(title62);
        hsChange1.WriteLine(title63);

        for (i = 0; i < NUM_LUZS; i++)
        {
            hsChange1.Write("{0,4}", (i + 1));
            for (j = 1; j < NUM_SF_LAND; j++)
            {
                hsChange1.Write("{0,9:F1}", z[i].useCap.ac.asf[j]);
                rb[j] += z[i].useCap.ac.asf[j];
            }   // end for j

            hsChange1.Write("{0,9:F1}", z[i].useCap.ac.totalSFAcres);
            rb[5] += z[i].useCap.ac.totalSFAcres;

            for (j = 1; j < NUM_SF_LAND; j++)
            {
                hsChange1.Write("{0,9:F1}", z[i].remCap.ac.asf[j]);
                rc[j] += z[i].remCap.ac.asf[j];
            }   // end for j

            hsChange1.WriteLine("{0,9:F1}", z[i].remCap.ac.totalSFAcres);
            rc[5] += z[i].remCap.ac.totalSFAcres;
            lineCount++;

            if (lineCount >= 30)
            {
                hsChange1.WriteLine();
                hsChange1.WriteLine();
                lineCount = 0;
                hsChange1.WriteLine("Table 3-6a");
                hsChange1.WriteLine("SF - LAND CONSUMPTION " + outputLabel);
                hsChange1.WriteLine(title61);
                hsChange1.WriteLine(title62);
                hsChange1.WriteLine(title63);
            }   // end if
        }     // End for i

        hsChange1.WriteLine();
        hsChange1.Write("Sum");
        total = 0;
        for (j = 1; j < NUM_SF_LAND; j++)
        {
            hsChange1.Write("{0,9:F1}", rb[j]);
            total += rb[j];
        }  // end for j
        hsChange1.Write("{0,9:F1}", total);

        total = 0;
        for (j = 1; j < NUM_SF_LAND; j++)
        {
            hsChange1.Write("{0,9:F1}", rc[j]);
            total += rc[j];
        }   // end for j
        hsChange1.WriteLine("{0,9:F1}", total);

        hsChange1.Write("Reg");
        for (j = 1; j < NUM_SF_LAND; j++)
            hsChange1.Write("{0,9:F1}", reg.useCap.ac.asf[j]);
        hsChange1.Write("{0,9:F1}", reg.useCap.ac.totalSFAcres);
        for (j = 1; j < NUM_SF_LAND; j++)
            hsChange1.Write("{0,9:F1}", reg.remCap.ac.asf[j]);
        hsChange1.WriteLine("{0,9:F1}", reg.remCap.ac.totalSFAcres);

        // MF
        lineCount = 0;
        hsChange1.WriteLine("Table 3-6b");
        hsChange1.WriteLine("MF - LAND CONSUMPTION");
        hsChange1.WriteLine(title71);
        hsChange1.WriteLine(title72);
        hsChange1.WriteLine(title73);

        for (i = 0; i < NUM_LUZS; i++)
        {
            hsChange1.Write("{0,4}", (i + 1));
            for (j = 1; j < NUM_MF_LAND; j++)
            {
                hsChange1.Write("{0,9:F1}", z[i].useCap.ac.amf[j]);
                rb[j] += z[i].useCap.ac.amf[j];
            }   // end for j
            hsChange1.Write("{0,9:F1}", z[i].useCap.ac.totalMFAcres);
            rb[4] += z[i].useCap.ac.totalMFAcres;

            for (j = 1; j < NUM_MF_LAND; j++)
            {
                hsChange1.Write("{0,9:F1}", z[i].remCap.ac.amf[j]);
                rc[j] += z[i].remCap.ac.amf[j];
            }   // end for j
            hsChange1.WriteLine("{0,9:F1}", z[i].remCap.ac.totalMFAcres);
            rc[4] += z[i].remCap.ac.totalMFAcres;
            lineCount++;

            if (lineCount >= 30)
            {
                hsChange1.WriteLine();
                hsChange1.WriteLine();
                lineCount = 0;
                hsChange1.WriteLine("Table 3-6b");
                hsChange1.WriteLine("MF - LAND CONSUMPTION");
                hsChange1.WriteLine(title71);
                hsChange1.WriteLine(title72);
                hsChange1.WriteLine(title73);
            }   // end if
        }     // End for i

        hsChange1.WriteLine();
        hsChange1.Write("Sum");
        for (j = 1; j < 5; j++)
            hsChange1.Write("{0,9:F1}", rb[j]);
        for (j = 1; j < 5; j++)
            hsChange1.Write("{0,9:F1}", rc[j]);
        hsChange1.WriteLine();

        hsChange1.Write("Reg");
        for (j = 1; j < NUM_MF_LAND; j++)
            hsChange1.Write("{0,9:F1}", reg.useCap.ac.amf[j]);
        hsChange1.Write("{0,9:F1}", reg.useCap.ac.totalMFAcres);
        for (j = 1; j < NUM_MF_LAND; j++)
            hsChange1.Write("{0,9:F1}", reg.remCap.ac.amf[j]);
        hsChange1.WriteLine("{0,9:F1}", reg.remCap.ac.totalMFAcres);
    }     // End method printHS6()

    /*****************************************************************************/

    /* method printHSOutliers() */
    /// <summary>
    /// Method to write HS outliers reports to ASCII.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 07/02/97   tb    Initial coding
    *                 08/07/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    void printHSOutliers()
    {
        int i;
        string title1 = " LUZ      Code     Ratio       Inc      Inc5     Diff5";
        string title2 = "------------------------------------------------------";

        hsOut.WriteLine();
        hsOut.WriteLine("Table 3-7a");
        hsOut.WriteLine("SINGLE FAMILY OUTLIERS " + outputLabel);
        hsOut.WriteLine(title1);
        hsOut.WriteLine(title2);

        for (i = 0; i < NUM_LUZS; i++)
        {
            if (z[i].sfOut.outCode > 0)
                hsOut.WriteLine("{0,4}{1,10}{2,10:F4}{3,10}{4,10}{5,10}", (i + 1),
                        z[i].sfOut.outCode, z[i].sfOut.r5, z[i].fcst.hsi.sf,
                        z[i].sfOut.inc5, z[i].sfOut.diff5);
        }   // end for i

        hsOut.WriteLine();
        hsOut.WriteLine("Table 3-7b");
        hsOut.WriteLine("MULTI FAMILY OUTLIERS");
        hsOut.WriteLine(title1);
        hsOut.WriteLine(title2);
        for (i = 0; i < NUM_LUZS; i++)
        {
            if (z[i].mfOut.outCode > 0)
                hsOut.WriteLine("{0,4}{1,10}{2,10:F4}{3,10}{4,10}{5,10}", (i + 1),
                      z[i].mfOut.outCode, z[i].mfOut.r5, z[i].fcst.hsi.mf,
                      z[i].mfOut.inc5, z[i].mfOut.diff5);
        }   // end for i
    }     // End method printHSOutliers()

    /*****************************************************************************/

    /* method printHSOvr() */
    /// <summary>
    /// Method to write HS overrides reports to ASCII.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 11/19/97   tb    Initial coding
    *                 08/07/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    void printHSOvr()
    {
        int i, j;
        string title0 = "             Attractors                 Stock        " +
                        "                     SF Land Use";
        string title1 = " LUZ        MF        SF        SF        MF        " +
                        "mh     Redev    Infill   Vac-Low   Vac-Urb     Total";
        string title2 = "----------------------------------------------------" +
                        "----------------------------------------------------";

        int sfLUTotal = 0;
        int regSF = 0;
        int regMF = 0;
        int regmh = 0;
        int[] regSFLU = new int[7];
        int regSFLUTotal = 0;

        hsOvr.WriteLine("LUZ HS OVERRIDES");
        hsOvr.WriteLine(title0);
        hsOvr.WriteLine(title1);
        hsOvr.WriteLine(title2);

        for (i = 0; i < NUM_LUZS; i++)
        {
            if (!z[i].hOvr)     /* skip luzs with no overrides */
                continue;

            sfLUTotal = 0;      // Initialize the land use overrides total
            hsOvr.Write("{0,4}{1,10}{2,10}", (i + 1), z[i].ho.attrSF, z[i].ho.attrMF);
            hsOvr.Write("{0,10}{1,10}{2,10}", z[i].ho.sf, z[i].ho.mf, z[i].ho.mh);

            // Accumulate regional totals
            regSF += z[i].ho.sf;
            regMF += z[i].ho.mf;
            regmh += z[i].ho.mh;

            // Print sf land use overrides
            for (j = 1; j < NUM_SF_LAND; j++)
            {
                hsOvr.Write("{0,10}", z[i].ho.sfLU[j]);     /* Write the LUZ SF land overrides */
                sfLUTotal += z[i].ho.sfLU[j];                 /* Get LUZ total for SF land */
                regSFLU[j] += z[i].ho.sfLU[j];      /* Accumulate SF land overrides for region */
            }   // end for j
            regSFLUTotal += sfLUTotal;      /* Accumulate total HS land overrides for region */
            hsOvr.WriteLine("{0,10}", sfLUTotal);     /* LUZ total HS land overrides */
        }     // End for i

        // Write the regional totals
        hsOvr.Write("Region                {0,10}{1,10}{2,10}", regSF, regMF, regmh);
        for (j = 1; j < NUM_SF_LAND; j++)
            hsOvr.Write("{0,10}", regSFLU[j]);          /* Write the reg HS land overrides total */
        hsOvr.WriteLine("{0,10}", regSFLUTotal);      /* Regional total HS land overrides */
    }     // End method printHSOvr()

    /*****************************************************************************/

    /* method printHSRates() */
    /// <summary>
    /// Method to write LUZ vacancy, er/hh, and hhs rates ASCII.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 07/02/97   tb    Initial coding
    *                 08/05/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    void printHSRates()
    {
        int i;
        StreamWriter hsr = null;
        string title1 = "                 Base Year                               Forecast Year";
        string title2 = " LUZ  Vac-Sf  Vac-Mf  Vac-Mh   ER/HH     HHS  Vac-Sf   Vac-Mf  Vac-Mh   ER/HH    HHS     Ovr";
        string title3 = "--------------------------------------------------------------------------------------------";
        int lineCount = 0;

        // Open hsrates temp for passing rates data to module 4      
        try
        {
            hsr = new StreamWriter(new FileStream(networkPath + String.Format(appSettings["hsRatesPass"].Value), FileMode.Create));
            hsr.AutoFlush = hsRates.AutoFlush = true;
        }
        catch (IOException e)
        {
            MessageBox.Show(e.ToString() + "  Now exiting.", "IO Exception");
            Close();
        }
        hsRates.WriteLine(title1);
        hsRates.WriteLine(title2);
        hsRates.WriteLine(title3);

        for (i = 0; i < NUM_LUZS; i++)
        {
            hsr.WriteLine("{0,4}{1,8:F2}{2,8:F2}{3,8:F2}{4,8:F2}{5,8:F2}{6,8:F2}{7,8:F2}{8,8:F2}{9,8:F2}{10,8:F2}", (i + 1),
                        z[i].baseData.r.vSF, z[i].baseData.r.vMF, z[i].baseData.r.vmh, z[i].baseData.r.erHH,
                        z[i].baseData.r.hhs, z[i].fcst.r.vSF, z[i].fcst.r.vMF, z[i].fcst.r.vmh, z[i].fcst.r.erHH, z[i].fcst.r.hhs);
            hsRates.WriteLine("{0,4}{1,8:F2}{2,8:F2}{3,8:F2}{4,8:F2}{5,8:F2}{6,8:F2}{7,8:F2}{8,8:F2}{9,8:F2}{10,8:F2}",
                            (i + 1), z[i].baseData.r.vSF, z[i].baseData.r.vMF, z[i].baseData.r.vmh, z[i].baseData.r.erHH,
                            z[i].baseData.r.hhs, z[i].fcst.r.vSF, z[i].fcst.r.vMF, z[i].fcst.r.vmh, z[i].fcst.r.erHH, z[i].fcst.r.hhs);
            if (z[i].fcst.r.regOvr == true)
                hsRates.WriteLine("       *");
            else
                hsRates.WriteLine();
            lineCount++;

            if (lineCount >= 57)
            {
                lineCount = 0;
                hsRates.WriteLine(title1);
                hsRates.WriteLine(title2);
                hsRates.WriteLine(title3);
            }   // end if
        }     // End for

        hsr.WriteLine("{0,8:F2}{1,8:F2}{2,8:F2}{3,8:F2}{4,8:F2}{5,8:F2}{6,8:F2}{7,8:F2}{8,8:F2}{9,8:F2}", reg.baseData.r.vSF,
                        reg.baseData.r.vMF, reg.baseData.r.vmh, reg.baseData.r.erHH, reg.baseData.r.hhs, reg.fcst.r.vSF,
                        reg.fcst.r.vMF, reg.fcst.r.vmh, reg.fcst.r.erHH, reg.fcst.r.hhs);
        hsRates.WriteLine();
        hsRates.WriteLine(" Reg{0,8:F2}{1,8:F2}{2,8:F2}{3,8:F2}{4,8:F2}{5,8:F2}{5,8:F2}{6,8:F2}{7,8:F2}{8,8:F2}",
                            reg.baseData.r.vSF, reg.baseData.r.vMF, reg.baseData.r.vmh, reg.baseData.r.erHH,
                            reg.baseData.r.hhs, reg.fcst.r.vSF, reg.fcst.r.vMF, reg.fcst.r.vmh, reg.fcst.r.erHH, reg.fcst.r.hhs);
        hsr.Close();
    }     // End method printHSRates()

    /*****************************************************************************/

    /* method printHSTableSpecial() */
    /// <summary>
    /// Method to combine some data from 3-1, 3-2 tuning and print to ASCII.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 08/03/98   tb    Initial coding
    *                 08/07/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    void printHSTableSpecial()
    {
        string title0 = "                                                                             LDSF";
        string title1 = " LUZ   Base   Fcst    Chg   %Chg  %Chg5 CapShr  SiteC    Cap  %Used    Cap   Used  %Used";
        string title2 = "----------------------------------------------------------------------------------------";
        string title3 = " LUZ   Base   Fcst    Chg   %Chg  %Chg5 CapShr  SiteC    Cap  %Used";
        string title4 = "-------------------------------------------------------------------";
        int i, lineCount = 0;
        double chg1;

        StreamWriter hsDataOnly = null;

        try
        {
            hsDataOnly = new StreamWriter(new FileStream(networkPath + String.Format(appSettings["hsDataOnly"].Value), FileMode.Create));
            hsDataOnly.AutoFlush = true;
        }
        catch (IOException e)
        {
            MessageBox.Show(e.ToString(), "IO Exception");
            Close();
        }

        /****** SF ******/
        hsChange.WriteLine();
        hsChange.WriteLine();
        hsChange.WriteLine("Table 3-SP1");
        hsChange.WriteLine("SF HOUSING VARS " + outputLabel);
        hsChange.WriteLine(title0);
        hsChange.WriteLine(title1);
        hsChange.WriteLine(title2);

        for (i = 0; i < NUM_LUZS; i++)
        {
            if (z[i].baseData.hs.sf > 0)
                chg1 = (double)z[i].fcst.hsi.sf / (double)z[i].baseData.hs.sf * 100;
            else
                chg1 = 0;
            hsChange.WriteLine("{0,4}{1,7}{2,7}{3,7}{4,7:F1}{5,7:F1}{6,7}{7,7}{8,7}{9,7:F1}{10,7}{11,7}{12,7:F1}", (i + 1),
                                z[i].baseData.hs.sf, z[i].fcst.hs.sf, z[i].fcst.hsi.sf, chg1, z[i].histSF.pct5,
                                z[i].fcst.sfCapShare, z[i].site.sf, z[i].capacity.totalSF, z[i].useCap.pTotalSF,
                                z[i].capacity.sf[3], z[i].useCap.sf[3], z[i].useCap.pSF[3]);
            hsDataOnly.WriteLine("{0,4}{1,7}{2,7}{3,7}{4,7:F1}{5,7:F1}{6,7}{7,7}{8,7}{9,7}{10,7}{11,7:F1}{12,7}{13,7}{14,7:F1}",
                                (i + 1), z[i].baseData.hs.sf, z[i].fcst.hs.sf, z[i].fcst.hsi.sf, chg1, z[i].histSF.pct5,
                                z[i].fcst.sfCapShare, z[i].site.sf, z[i].fcst.le.sf, z[i].fcst.lh.sf,
                                z[i].capacity.totalSF, z[i].useCap.pTotalSF, z[i].capacity.sf[3], z[i].useCap.sf[3], z[i].useCap.pSF[3]);
            lineCount++;
            if (lineCount >= 57)
            {
                hsChange.WriteLine();
                hsChange.WriteLine();
                lineCount = 0;      // Reset line count
                hsChange.WriteLine("Table 3-SP1");
                hsChange.WriteLine("SF HOUSING VARS  " + outputLabel);
                hsChange.WriteLine(title0);
                hsChange.WriteLine(title1);
                hsChange.WriteLine(title2);
            }   // end if
        }     // End for

        /****** MF ******/
        lineCount = 0;
        hsChange.WriteLine();
        hsChange.WriteLine();
        hsChange.WriteLine("Table 3-SP2");
        hsChange.WriteLine("MF HOUSING VARS " + outputLabel);
        hsChange.WriteLine(title3);
        hsChange.WriteLine(title4);

        for (i = 0; i < NUM_LUZS; i++)
        {
            if (z[i].baseData.hs.mf > 0)
                chg1 = (double)z[i].fcst.hsi.mf / (double)z[i].baseData.hs.mf * 100;
            else
                chg1 = 0;
            hsChange.WriteLine("{0,4}{1,7}{2,7}{3,7}{4,7:F1}{5,7:F1}{6,7}{7,7}{8,7}{9,7:F1}", (i + 1), z[i].baseData.hs.mf,
                                z[i].fcst.hs.mf, z[i].fcst.hsi.mf, chg1, z[i].histMF.pct5, z[i].fcst.mfCapShare,
                                z[i].site.mf, z[i].capacity.totalMF, z[i].useCap.pTotalMF);
            hsDataOnly.WriteLine("{0,4}{1,7}{2,7}{3,7}{4,7:F1}{5,7:F1}{6,7}{7,7}{8,7}{9,7}{10,7:F1}", (i + 1),
                                z[i].baseData.hs.mf, z[i].fcst.hs.mf, z[i].fcst.hsi.mf, chg1, z[i].histMF.pct5,
                                z[i].fcst.mfCapShare, z[i].site.mf, z[i].fcst.le.mf, z[i].capacity.totalMF, z[i].useCap.pTotalMF);
            lineCount++;

            if (lineCount >= 57)
            {
                hsChange.WriteLine();
                hsChange.WriteLine();
                lineCount = 0;
                hsChange.WriteLine("Table 3-SP2");
                hsChange.WriteLine("MF HOUSING VARS  " + outputLabel);
                hsChange.WriteLine(title3);
                hsChange.WriteLine(title4);
            }   // end if
        }     // End for
        hsDataOnly.Close();
    }     // End method printHSTableSpecial()

    /*****************************************************************************/

    #endregion Print Procs

    #region Prob Procs
    /* method prob1() */
    /// <summary>
    /// Method to calculate 5-minute block probabilities for hs allocation.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 07/10/97   tb    Initial coding
    *                 08/05/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    void prob1()
    {
        StreamWriter p1Out = null;
        double s2 = 1.4142;
        int i, j, k;
        double pl, z;

        for (j = 0; j < 3; j++)     // For each travel time curve
        {
            pl = 0;
            for (k = 5; k <= 115; k += 5)
            {
                i = k / 5;
                if (tt[j].asd != 0)
                    z = (Math.Log((double)k) - Math.Log(tt[j].med)) / (tt[j].asd * s2);   // logNormal z
                else
                    z = 0;

                // Nonlinear adjustment
                z = z * Math.Sqrt(Math.Log((double)k)) * tt[j].nla;
                cumProb[j, i - 1] = (1 + UDMUtils.errorFunction((double)(z / s2))) / 2;      // Cumulative probability
                intProb[j, i - 1] = cumProb[j, i - 1] - pl;     // Interval prob
                pl = cumProb[j, i - 1];
            }   // end for k
            intProb[j, 23] = 1.0 - pl;
            cumProb[j, 23] = 1.0;
        }     // End for j

        try
        {
            p1Out = new StreamWriter(new FileStream(networkPath + String.Format(appSettings["p1Out"].Value), FileMode.Create));
        }
        catch (IOException e)
        {
            MessageBox.Show(e.ToString(), "IO Exception");
            Close();
        }

        for (i = 0; i < 24; i++)
        {
            p1Out.WriteLine("i = {0,2} {1,10:F6} {2,10:F6} {3,10:F6} {4,10:F6}{5,10:F6} {6,10:F6}", i, intProb[0, i], cumProb[0, i], intProb[1, i], cumProb[1, i], intProb[2, i], cumProb[2, i]);
        }   // end for i
        p1Out.Flush();
        p1Out.Close();
    }     // End method prob1()

    /*****************************************************************************/

    /* method prob2() */
    /// <summary>
    /// Method to calculate allocation probabilities for HS.
    /// </summary>
    /// <param name="attractors">Array of attractors</param>
    /// <param name="zi">LUZ index</param>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 07/10/97   tb    Initial coding
    *                 08/05/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    void prob2(int zi, double[] attractors)
    {
        StreamWriter p2Out = null;

        int[] d = new int[NUM_LUZS];     // Vector of travel times from LUZ z

        int i, j, ij;
        int[] kk = new int[NUM_LUZS];
        double shc;
        //------------------------------------------------------------------------------

        try
        {
            p2Out = new StreamWriter(new FileStream(networkPath + String.Format(appSettings["p2Out"].Value), FileMode.Create));
        }
        catch (IOException e)
        {
            MessageBox.Show(e.ToString(), "IO Exception");
            Close();
        }

        for (ij = 0; ij < 2; ij++)
        {
            j = ij;
            // Set index for cbd because it uses a different curve
            if (j == 1 && zi == CBDLUZ)      // cbd LUZ
                j = 2;
            shc = 0;
            // Load travel times
            for (i = 0; i < NUM_LUZS; i++)
            {
                if (ij == 0)
                    d[i] = impedPM[i, zi];       // PM skim tree
                else
                    d[i] = impedTran[i, zi];     // Transit skim tree
            }   // end for i

            // Compute allocation probabilities from zone zi to all zones
            for (i = 0; i < NUM_LUZS; i++)
            {
                if (d[i] < 0)
                    d[i] = 120;
                kk[i] = d[i] / 5;
                kk[i] = Math.Min(kk[i], 23);
                shc += intProb[j, kk[i]] * attractors[i];
                p2Out.WriteLine("luz = {0,3} attra = {1,7:F0} prob = {2,10:F6} " + "shc = {3,10:F2}", (i + 1), attractors[i], intProb[j, kk[i]], shc);
                p2Out.Flush();

                // Accumulate all probabilities within the intrazonal time
                if (i == zi && kk[i] > 0)
                    shc += cumProb[j, kk[i]] * attractors[i];
            }   // end for i

            for (i = 0; i < NUM_LUZS; i++)
            {
                if (shc > 0)
                {
                    // Normalized zonal allocation
                    allocProb[i, ij] = intProb[j, kk[i]] * attractors[i] / shc;
                    p2Out.WriteLine("luz = {0,3} attra = {1,7:F0} prob = {2,10:F6} shc = {3,10:F2} alloc = {4,10:F5}", (i + 1), attractors[i], intProb[j, kk[i]], shc, allocProb[i, ij]);
                    if (i == zi)
                        allocProb[i, ij] = cumProb[j, kk[i]] * attractors[i] / shc;
                }   // end if
            }  // end for i
        }     // End for ij
        p2Out.Close();
    }     // End procedure prob2()

    /*****************************************************************************/

    #endregion Prob Procs

    #region ProcessParams
    /* method processParams() */
    /// <summary>
    /// Method to process housing model input parameters and build table names.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 07/07/97   tb    Initial coding
    *                 02/02/99   tb    Changed name of access_wts because of 
    *                                  system errors
    *                 01/11/02   tb    Changes for Jan, 2002 sr10
    *                 08/01/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    bool processParams()
    {
        if (cboYears.SelectedIndex == -1)
        {
            MessageBox.Show("You have selected an invalid range of years!  Please try again.", "Invalid Years");
            return false;
        }
        if (cboScenario.SelectedIndex != 0)
        {
            MessageBox.Show("You have selected an invalid scenario!  Please try again.", "Invalid Scenario");
            return false;
        }

        // Load flags from input parameters
        scenarioID = cboScenario.SelectedIndex;
        doHSsfOvr = chkUseSFOvr.Checked;
        doHSmfOvr = chkUseMFOvr.Checked;

        controlHSOverrides = chkCtrlOvr.Checked;

        bYear = incrementLabels[cboYears.SelectedIndex];
        fYear = incrementLabels[cboYears.SelectedIndex + 1];

        outputLabel = DateTime.Now.ToString();
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

            RC_DEBUG = bool.Parse(appSettings["RC_DEBUG"].Value);        // write regional controls to file
            SF_DEBUG = bool.Parse(appSettings["SF_DEBUG"].Value);         // Write SF distribution to file
            ZB_DEBUG = bool.Parse(appSettings["ZB_DEBUG"].Value);        // Write luzBase to file
            ZH_DEBUG = bool.Parse(appSettings["ZH_DEBUG"].Value);        // Write luzHistory to file
            ZT_DEBUG = bool.Parse(appSettings["ZT_DEBUG"].Value);        // Write luzTemp to file
            MF_DEBUG = bool.Parse(appSettings["MF_DEBUG"].Value);    //write mf dat to temp file

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


            CBDLUZ = int.Parse(appSettings["CBDLUZ"].Value);
            MAX_TRANS = int.Parse(appSettings["MAX_TRANS"].Value);
            NUM_EMP_LAND = int.Parse(appSettings["NUM_EMP_LAND"].Value);
            NUM_EMP_SECTORS = int.Parse(appSettings["NUM_EMP_SECTORS"].Value);
            NUM_HH_INCOME = int.Parse(appSettings["NUM_HH_INCOME"].Value);
            NUM_LUZS = int.Parse(appSettings["NUM_LUZS"].Value);
            NUM_MGRAS = int.Parse(appSettings["NUM_MGRAS"].Value);
            NUM_MF_LAND = int.Parse(appSettings["NUM_MF_LAND"].Value);
            NUM_SF_LAND = int.Parse(appSettings["NUM_SF_LAND"].Value);
            NUM_TT_PROB = int.Parse(appSettings["NUM_TT_PROB"].Value);

            TN.accessWeights = String.Format(appSettings["accessWeightsTable"].Value);
            TN.capacity = String.Format(appSettings["capacity"].Value);
            TN.capacity3 = String.Format(appSettings["capacity3"].Value);
            TN.capacity2 = String.Format(appSettings["capacity2"].Value);
            TN.fractee = String.Format(appSettings["fracteeTable"].Value);
            TN.impedAM = String.Format(appSettings["impedAM"].Value);
            TN.impedPM = String.Format(appSettings["impedPM"].Value);
            TN.impedTran = String.Format(appSettings["impedTran"].Value);
            TN.luzbase = String.Format(appSettings["luzbase"].Value);
            TN.luzhist = String.Format(appSettings["luzhist"].Value);
            TN.luzMFOvr = String.Format(appSettings["luzMFOvr"].Value);
            TN.luzSFOvr = String.Format(appSettings["luzSFOvr"].Value);
            TN.luzIncomeParms = String.Format(appSettings["luzIncomeParms"].Value);
            TN.luztemp = String.Format(appSettings["luztemp"].Value);
            TN.regfcst = String.Format(appSettings["regfcst"].Value);
            TN.homePrices = String.Format(appSettings["homePrices"].Value);
            TN.mgrabase = String.Format(appSettings["mgrabase"].Value);
            TN.MFDecrements = String.Format(appSettings["MFDecrements"].Value);
            TN.MFIncrements = String.Format(appSettings["MFIncrements"].Value);
            TN.mhDecrements = String.Format(appSettings["mhDecrements"].Value);
            TN.mhIncrements = String.Format(appSettings["mhIncrements"].Value);
           
            TN.SFDecrements = String.Format(appSettings["SFDecrements"].Value);
            TN.SFIncrements = String.Format(appSettings["SFIncrements"].Value);
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

        reg = new Master();
        z = new Master[NUM_LUZS];
        flags = new Flags();
        mfOvrFlags = new bool[NUM_LUZS];
        mhOvrFlags = new bool[NUM_LUZS];
        sfOvrFlags = new bool[NUM_LUZS];
        rcn = new int[42];
        zbi = new int[70];
        impedPM = new int[NUM_LUZS, NUM_LUZS];
        impedTran = new int[NUM_LUZS, NUM_LUZS];
        fractees = new double[NUM_LUZS];
        allocAll = new double[NUM_LUZS, NUM_LUZS];
        allocProb = new double[NUM_LUZS, 2];
        intProb = new double[3, NUM_TT_PROB];
        cumProb = new double[3, NUM_TT_PROB];
        tcapd = new TCapD[NUM_LUZS, MAX_TRANS];
        tCapI = new TCap[NUM_LUZS, MAX_TRANS];
        tt = new TTP[3];
        return true;
    }     // End method processParams()

    /*****************************************************************************/
    #endregion ProcessParams

    #region redistHS
    /* method redistHS() */
  
    /// Method to redist any LUZ HS that exceeds capacity.
    

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 07/08/97   tb    Initial coding
    *                 08/05/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    void redistHS(int kind)
    {
        SqlDataReader rdr;
        int i, j, k = 0, excess, regTotal = 0, dest, imp1;
        int[] excapFlags = new int[NUM_LUZS];
        int[] impedPM = new int[NUM_LUZS];
        int[] reals = new int[NUM_LUZS];
        int[] realt = new int[NUM_LUZS];
        // ----------------------------------------------------------------------
        // SF
        for (i = 0; i < NUM_LUZS; i++)
        {
            if (kind == 1)     // Is this single family
            {
                regTotal += z[i].fcst.hsi.sfAdj;
                if (z[i].fcst.hsi.sfAdj > z[i].capacity.totalSF)
                    // Set flag marking over capacity
                    excapFlags[i] = z[i].exCap = 2;

                // Set flag marking at capacity or overrides used and !controlHSOvr
                else if (z[i].fcst.hsi.sfAdj == z[i].capacity.totalSF || (z[i].sfOvr && !controlHSOverrides))
                    excapFlags[i] = z[i].exCap = 1;
                else
                    excapFlags[i] = z[i].exCap = 0;
            }   // end if

            // Multi-family here
            else
            {
                regTotal += z[i].fcst.hsi.mfAdj;
                if (z[i].fcst.hsi.mfAdj > z[i].capacity.totalMF)
                    // Set flag marking over capacity
                    excapFlags[i] = z[i].exCap = 2;

                // Set flag marking at capacity or overrides used and !controlHSOvr
                else if (z[i].fcst.hsi.mfAdj == z[i].capacity.totalMF || (z[i].mfOvr && !controlHSOverrides))
                    excapFlags[i] = z[i].exCap = 1;
                else
                    excapFlags[i] = z[i].exCap = 0;
            }   // end else
        }     // End for i

        for (i = 0; i < NUM_LUZS; i++)
        {
            // Redistribute from LUZs over capacity to closest LUZ under capacity
            if (z[i].exCap == 2)
            {
                k = 0;
                if (kind == 1)
                    excess = z[i].fcst.hsi.sfAdj - z[i].capacity.totalSF;
                else
                    excess = z[i].fcst.hsi.mfAdj - z[i].capacity.totalMF;

                sqlCommand.CommandText = String.Format(appSettings["select06"].Value, TN.impedPM, (i + 1), scenarioID, bYear);

                /* Don't actually use impedence, just here for sorting the query results */
                try
                {
                    sqlConnection.Open();
                    rdr = sqlCommand.ExecuteReader();
                    while (rdr.Read())
                    {
                        dest = rdr.GetInt16(0);
                        imp1 = rdr.GetInt16(1);
                        impedPM[k++] = dest - 1;      // Convert LUZ id into array index
                    }   // end while
                    rdr.Close();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString(), "Runtime Exception");
                    Close();
                }
                finally
                {
                    sqlConnection.Close();
                }

                // Do the redistribution
                for (k = 0; k < NUM_LUZS; k++)
                {
                    // Index into impedence array for closest LUZ
                    j = impedPM[k];
                    if (z[j].exCap > 0)      /* If this target is at or over cap, get another */
                        continue;

                    /* Stay in this loop until either the excess id gone or the target
                    * is maxed out */
                    while (excess > 0 && z[j].exCap == 0)
                    {
                        reals[i]++;     // Increment counter for source LUZ
                        realt[j]++;     // Increment counter for target sum
                        excess--;       // Decrement excess

                        if (kind == 1)     // SF
                        {
                            z[i].fcst.hsi.sfAdj--;        // Decrement source LUZ
                            z[j].fcst.hsi.sfAdj += 1;     // Increment target LUZ
                            if (z[j].fcst.hsi.sfAdj == z[j].capacity.totalSF)
                                z[j].exCap = 1;
                        }   // end if

                        else     // MF
                        {
                            z[i].fcst.hsi.mfAdj--;      // Decrement source LUZ
                            z[j].fcst.hsi.mfAdj++;      // Increment target LUZ
                            if (z[j].fcst.hsi.mfAdj == z[j].capacity.totalMF)
                                z[j].exCap = 1;
                        }   // end else
                    }     // End while

                    // Out of while loop, check excess
                    if (excess == 0)
                        break;      // Excess has been redistributed, get out of for loop
                }     // End for k
            }     // End if

            redisHS.AutoFlush = true;
            if (reals[i] > 0)
            {
                flags.redisHS = true;
                if (kind == 1)
                    redisHS.WriteLine("EXCESS HS SF REDISTRIBUTED " + reals[i] + " FROM LUZ " + (i + 1));
                else
                    redisHS.WriteLine("EXCESS HS MF REDISTRIBUTED " + reals[i] + " FROM LUZ " + (i + 1));
            }   // end if

            for (k = 0; k < NUM_LUZS; k++)
            {
                if (realt[k] > 0)
                    redisHS.WriteLine("   REDISTRIBUTED " + realt[k] + " TO LUZ " + (k + 1));
            }   // end for k
        }     // End for i

        regTotal = 0;
        for (i = 0; i < NUM_LUZS; i++)
        {
            if (kind == 1)
                regTotal += z[i].fcst.hsi.sfAdj;
            else
                regTotal += z[i].fcst.hsi.mfAdj;
        }   // end for i

    }     // End method redistHS()

    /*****************************************************************************/

    #endregion redistHS
    # region SF Procs
    /* method sfCalc() */
    /// <summary>
    /// Method to recalculate SF HS distribtion.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 07/17/97   tb    Initial coding
    *                 08/05/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    void sfCalc()
    {
        int i, j;
        int realIndex;
        int ret;
        double[] attrSF = new double[NUM_LUZS];
        int[] dests = new int[NUM_LUZS];
        int[] erInc = new int[NUM_LUZS];
        int[] ubound = new int[NUM_LUZS];      /* Dummy array filled with 99999 
                                              * used in roundit call */
        int[,] dIndex1 = new int[NUM_LUZS, 2];
        int[] dIndex2 = new int[NUM_LUZS];

        writeToStatusBox("Computing SF distribution..");
        try
        {
            sfOut = new StreamWriter(new FileStream(networkPath + String.Format(appSettings["sfOut"].Value), FileMode.Create));
            sfOut.AutoFlush = true;
        }
        catch (Exception e)
        {
            MessageBox.Show(e.ToString(), "I/O Exception");
            Close();
        }

        // Dummy load the capacity file - dummy passed to roundIt
        for (i = 0; i < NUM_LUZS; i++)
            ubound[i] = 99999;

        // Adjust regional total for units lost to SF redev
        rc.fcst.hsi.sfAdj -= reg.fcst.lh.sf;

        //writeToStatusBox("   storing SF attractors.." );
        for (i = 0; i < NUM_LUZS; i++)
        {
            // Store the SF attractor override if applicable
            if (z[i].hOvr && z[i].ho.attrSF != 0)
                attrSF[i] = (double)z[i].ho.attrSF;
            else      // Otherwise, store the total SF capacity as attractor
                attrSF[i] = (double)z[i].capacity.totalSF * z[i].homePriceIndex;
        }   // end for i

        /* Allocate employed residents to SF based on regional share of housing stock change. */
        for (i = 0; i < NUM_LUZS; i++)
        {
            sfOut.WriteLine("   allocating er LUZ " + (i + 1));
            if (SF_DEBUG && i < 3)
            {
                sfOut.WriteLine("      attractor SF CAPACITY");
                for (j = 0; j < NUM_LUZS; j++)
                {
                    sfOut.Write("{0,7}", attrSF[j]);
                    if (j > 0 && j % 10 == 0)
                        sfOut.WriteLine();
                }   // end for j
                sfOut.WriteLine();
                sfOut.WriteLine();
            }   // end if

            prob2(i, attrSF);
            z[i].fcst.eri.sfAlloc = z[i].fcst.eri.total - z[i].fcst.eri.mfAlloc;

            // Split into transit or autos
            z[i].fcst.eri.sfTransit = (int)(z[i].fcst.eri.sfAlloc * fractees[i]);
            z[i].fcst.eri.sfAuto = z[i].fcst.eri.sfAlloc - z[i].fcst.eri.sfTransit;
            sfOut.WriteLine("      sf_transit = " + z[i].fcst.eri.sfTransit + " sf_auto = " + z[i].fcst.eri.sfAuto);

            // Now do the orig-dest probability computations.
            for (j = 0; j < NUM_LUZS; j++)
            {
                dests[j] = (int)((double)z[i].fcst.eri.sfAuto * allocProb[j, 0] + (double)z[i].fcst.eri.sfTransit * allocProb[j, 1]);
                sfOut.WriteLine("      j = {0,3} auto = {1,5} prob0 = {2,7:F4} transit = {3,5} prob1 = {4,7:F4} dest = {5,5}",
                            j, z[i].fcst.eri.sfAuto, allocProb[j, 0], z[i].fcst.eri.sfTransit, allocProb[j, 1], dests[j]);
            }   // end for j

            for (j = 0; j < NUM_LUZS; j++)
            {
                dIndex1[j, 0] = j;
                dIndex1[j, 1] = dests[j];
            }   // end for j

            // Sort them
            UDMUtils.quickSort(dIndex1, 0, NUM_LUZS - 1);

            if (SF_DEBUG && i < 3)
            {
                sfOut.WriteLine("      DESTS-UNADJUSTED");
                for (j = 0; j < NUM_LUZS; j++)
                {
                    sfOut.Write("{0,5}", dests[j]);
                    if (j > 0 && j % 20 == 0)
                        sfOut.WriteLine();
                }   // end for 
                sfOut.WriteLine();
                sfOut.WriteLine();
            }   // end if

            // Control to z[i].fcst.eri.sfAlloc
            for (j = 0; j < NUM_LUZS; j++)
                dIndex2[j] = dIndex1[j, 1];

            ret = UDMUtils.roundIt(dIndex2, ubound, z[i].fcst.eri.sfAlloc, NUM_LUZS, 0);
            if (ret > 0)
                MessageBox.Show("sfCalc roundit didn't converge, difference = " + ret + " LUZ " + (i + 1));

            // Restore the sorted data to original order
            for (j = 0; j < NUM_LUZS; j++)
            {
                realIndex = dIndex1[j, 0];
                dests[realIndex] = dIndex2[j];
            }   // end for j

            if (SF_DEBUG && i < 3)
            {
                sfOut.WriteLine("      DESTS-ADJUSTED");
                for (j = 0; j < NUM_LUZS; j++)
                {
                    sfOut.Write("{0,5}", dests[j]);
                    if (j > 0 && j % 20 == 0)
                        sfOut.WriteLine();
                }   // end for j
                sfOut.WriteLine();
                sfOut.WriteLine();
            }   // end if

            for (j = 0; j < NUM_LUZS; j++)
                allocAll[j, i] = dests[j];
        }     // End for i

        for (i = 0; i < NUM_LUZS; i++)
            for (j = 0; j < NUM_LUZS; j++)
                erInc[j] += (int)allocAll[j, i];

        // Convert employed res at place of residence to SF
        for (i = 0; i < NUM_LUZS; i++)
        {
            // Apply vacancy rate
            if (z[i].fcst.r.erHH > 0)
                z[i].fcst.hsi.sfAdj = (int)((double)erInc[i] / z[i].fcst.r.erHH / (1 - z[i].fcst.r.vSF / 100));

            /* Check for negative here.  If the unit increment is negative, replace with historical change (assuming it's positive, adjusted by the regional 5-year percent change. */
            if (z[i].fcst.hsi.sfAdj < 0)
            {
                z[i].fcst.hsi.sfAdj = (int)(z[i].histSF.c5 * reg.histSF.r5);
                if (z[i].fcst.hsi.sfAdj < 0)     // Constrain this to 0
                    z[i].fcst.hsi.sfAdj = 0;
            }   // end if
        }     // End for i
        sfOut.Close();
    }     // End method sfCalc()

    /*****************************************************************************/

    /* method SFDecrements() */
    /// <summary>
    /// Method to distribute LUZ SF decrements to transcations.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 12/01/09   tb    Initial coding - added for sr13 to handle programmed demos for sf
    * --------------------------------------------------------------------------
    */
    private void SFDecrements(int[] list, int counter)
    {

      SqlDataReader rdr;
      int i, luzControl, temp, k, zid, lz, nt, kk;
      int luz, mgra, LCKey, lu, devCode, eluu, sf, capSf;
      int[] nTCap = new int[NUM_LUZS];
      double weight;

      sfCd = new StreamWriter(new FileStream(networkPath + String.Format(appSettings["sfCd"].Value),FileMode.Create));

      sqlCommand.CommandText = String.Format(appSettings["select11"].Value,TN.capacity3,TN.accessWeights,fYear,scenarioID,bYear);
          
      try
      {
        sqlConnection.Open();
        rdr = sqlCommand.ExecuteReader();
        while (rdr.Read())
        {
          luz = rdr.GetInt16(0);

          mgra = rdr.GetInt16(1);
          weight = rdr.GetDouble(2);
          LCKey = rdr.GetInt32(3);
          sf = rdr.GetInt32(4);
          devCode = rdr.GetByte(5);
          lu = rdr.GetInt16(6);
          capSf = rdr.GetInt32(7);
          zid = 999;      // Set the default for not found

          // Is the luz from query response in this list
          for (k = 0; k < counter; k++)
          {
            if (list[k] == luz)
            {
              zid = k;      // Save the list position as index
              break;
            }   // end if
          }   // end for k

          if (zid != 999)      // Make sure the index is legitimate
          {
            nt = nTCap[zid];      // Store index position

            // Save developed only if land use is correct
            eluu = 1;     /* set eluu for default */
            if (devCode == 1)
              eluu = 2;

            if (eluu > 0)      /* This gets done for devcode = 1 and correct land use or devcode = 6 */
            {
              tcapd[zid, nt] = new TCapD();
              tcapd[zid, nt].luz = luz;
              tcapd[zid, nt].done = false;         // Initialize the used flag
              tcapd[zid, nt].mgra = mgra;          // MGRA ID
              tcapd[zid, nt].LCKey = LCKey;            // LCKey
              tcapd[zid, nt].sf = sf;            // Base year emp
              tcapd[zid, nt].capSf = capSf;      // Civ emp cap
              tcapd[zid, nt].devCode = devCode;   // devcode
              nTCap[zid]++;

            }  // end if eluu
          }     // End if zid
        }     // End while
        rdr.Close();
      }     // End try

      catch (SqlException s)
      {
        MessageBox.Show(s.Message, "SQL Error");
        Close();
      }
      finally
      {
        sqlConnection.Close();
      }

      for (i = 0; i < counter; i++)
      {

        zid = i;
        lz = list[i] - 1;     // Get the LUZ ID from the list 

        if (lz == 68) // temporary debug stop
          lz = 68;
        k = 0;
        luzControl = -z[lz].fcst.hsi.sfAdj;      /* Set the luzControl value to the adjusted sf */
        // While the luzControl > 0 and the list is unfinished
        while (k < nTCap[i] && luzControl > 0)
        {
          // Get the minimum of base year emp and luz_control
          temp = Math.Min(luzControl, tcapd[zid, k].sf);
          tcapd[zid, k].sf -= temp;         // Adjust base year
          tcapd[zid, k].chgSf = -temp;      // Adjust the increment holder
          tcapd[zid, k].capSf += temp;      // Adjust the capacity
          luzControl -= temp;                // Adjust the loop luzControl
          tcapd[zid, k].done = true;          // Mark this entry as adjusted
          if (tcapd[zid, k].devCode == 1)     //reset any developed sf land to esf infill
            tcapd[zid, k].devCode = 6;          // All   of these become redev
          tcapd[zid, k].pCap_hs = 0;             // Reset emp and hs pCap to 0

          k++;                               // Increment the loop list counter
        }   // end while

        for (kk = 1; kk < NUM_SF_LAND; kk++)
        {
          // Add site spec to acreage used
          z[lz].useCap.ac.asf[kk] += z[lz].site.ac.asf[kk];
          // Compute remaining acreage
          z[lz].remCap.ac.asf[kk] = z[lz].capacity.ac.asf[kk] - z[lz].useCap.ac.asf[kk];
          z[lz].useCap.ac.totalSFAcres += z[lz].useCap.ac.asf[kk];
          z[lz].remCap.ac.totalSFAcres += z[lz].remCap.ac.asf[kk];
          reg.useCap.ac.asf[kk] += z[lz].useCap.ac.asf[kk];     /* Used acreage by category */

          // Remaining acreage by category
          reg.remCap.ac.asf[kk] += z[lz].remCap.ac.asf[kk];
          // Total used
          reg.useCap.ac.totalSFAcres += z[lz].useCap.ac.asf[kk];
          // Total remaining
          reg.remCap.ac.totalSFAcres += z[lz].remCap.ac.asf[kk];
        }   // end for kk

        if (luzControl > 0)      // Is there employment left to allocate
          MessageBox.Show("EXCEPTION -- Cannot allocate LUZ sf decrement to transactions, LUZ " + (lz + 1) + " CONTROL = " + luzControl);
        // Update the capacity records with new data
        for (k = 0; k < nTCap[i]; k++)
        {
          if (tcapd[zid, k].done)
            writeSfDecrements(tcapd[zid, k]);      /* Write the update data to ASCII for bulk load */
        }   // end for k
      }     // End for i

      // Close the output file
      try
      {
        sfCd.Close();
      }
      catch (IOException io)
      {
        MessageBox.Show(io.Message);
        Close();
      }

      loadDecrements(1);        /* Bulk load the ASCII data to the _dec table */
      updateSFDecrements();
    }     // End method SFDecrements()

    /*****************************************************************************/   

    /* method SFIncrements() */
    /// <summary>
    /// Method to distribute LUZ SF increments to transactions.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 11/14/97   tb    Initial coding
    *                 06/22/99   tb    Fixed error in pcap calculation - base 
    *                                  year pcap was not included in query to 
    *                                  fill tcapi arrays
    *                 08/06/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    void SFIncrements(int[] list, int counter)
    {
      SqlDataReader rdr;
      int luz, mgra, LCKey, lu, plu, devCode, sfLU, sf, mf, mh, csf, cmf,
          cmh;
      int i, j, k, kk, zid, nt;
      int mmh, temp, lz;
      int tempmh;
      int masterControl;
      int[] tlh = new int[NUM_LUZS];
      int[] control = new int[NUM_SF_LAND];     // SF land use control totals
      int[] ntCap = new int[NUM_LUZS];
      double acres, capDiff, capRatio, temp1, weight, pCap;
      // ---------------------------------------------------------------------

      try
      {
        sfCi = new StreamWriter(new FileStream(networkPath + String.Format(appSettings["sfCi"].Value),FileMode.Create));
      }
      catch (IOException e)
      {
        MessageBox.Show(e.ToString(), "IO Exception");
        Close();
      }

      sqlCommand.CommandText = String.Format(appSettings["select12"].Value, TN.capacity3, TN.accessWeights, fYear, scenarioID, bYear);
      
      try
      {
        sqlConnection.Open();
        rdr = sqlCommand.ExecuteReader();
        while (rdr.Read())
        {
          luz = rdr.GetInt16(0);
          mgra = rdr.GetInt16(1);
          weight = rdr.GetDouble(2);
          LCKey = rdr.GetInt32(3);
          lu = rdr.GetInt16(4);
          plu = rdr.GetInt16(5);
          devCode = rdr.GetByte(6);
          acres = rdr.GetDouble(7);
          sfLU = rdr.GetByte(8);
          sf = rdr.GetInt32(9);
          mf = rdr.GetInt32(10);
          mh = rdr.GetInt32(11);
          csf = rdr.GetInt32(12);
          cmf = rdr.GetInt32(13);
          cmh = rdr.GetInt32(14);
          pCap = rdr.GetDouble(15);
          zid = 999;      // Set the default for not found

          // Is the luz in the query in this list
          for (k = 0; k < counter; k++)
          {
            if (list[k] == luz)
            {
              zid = k;      // Save the list position as index
              break;
            }   // end if
          }   // end for k

          if (zid != 999)      // Do this only for good indexes
          {
            nt = ntCap[zid];      // Get the second dimension index
            tCapI[zid, nt] = new TCap();
            tCapI[zid, nt].luz = luz;
            tCapI[zid, nt].mgra = mgra;
            tCapI[zid, nt].LCKey = LCKey;
            tCapI[zid, nt].pCap_hs = pCap;
            tCapI[zid, nt].lu = lu;
            tCapI[zid, nt].plu = plu;
            tCapI[zid, nt].devCode = devCode;
            tCapI[zid, nt].acres = acres;
            tCapI[zid, nt].udmSFLU = sfLU;
            tCapI[zid, nt].sf = sf;
            tCapI[zid, nt].mf = mf;
            tCapI[zid, nt].mh = mh;
            tCapI[zid, nt].capSF = csf;
            tCapI[zid, nt].capMF = cmf;
            tCapI[zid, nt].chgmh = cmh;
            tCapI[zid, nt].chgMF = 0;
            tCapI[zid, nt].chgSF = 0;
            tCapI[zid, nt].chgmh = 0;
            tCapI[zid, nt].done = false;

            ntCap[zid]++;     // Increment the second dimension index

            // Check for max array size, MAX_TRANS is somewhat arbitrary
            if (ntCap[zid] == MAX_TRANS)
            {
              MessageBox.Show("FATAL ERROR - in SFIncrements tCapI array index exceeds array bound = " + MAX_TRANS);
              Close();
            }   // end if
          }     // End if zid
        }     // End while
      }     // End try

      catch (Exception e)
      {
        MessageBox.Show(e.ToString(), "Runtime Exception");
      }
      finally
      {
        sqlConnection.Close();
      }

      for (i = 0; i < counter; i++)
      {
        zid = i;
        lz = list[i] - 1;     // Get LUZ ID
       
        for (j = 1; j < NUM_SF_LAND; j++)
        {
          /* Store sfLand distribution as controls.  These are the incremental SF allocated to land use */
          control[j] = z[lz].fcst.hsi.sfLand[j];
          z[lz].useCap.ac.asf[j] = 0;     /* Initialize the dev acres used  totals */
          z[lz].remCap.ac.asf[j] = 0;     /* Initialize the dev acres remaining totals */
        }   // end for j
        masterControl = 0;

        /* Build master control as boolean or with control values.  Master 
        * control will be used as loop control as long as it > 0  meaning,
        * there is still capacity to fill. */
        for (j = 1; j < NUM_SF_LAND; j++)
          masterControl += control[j];

        k = 0;      // Start the counter
        // Allocation control loop
        while (k < ntCap[i] && masterControl > 0)
        {
          tCapI[zid, k].done = true;     // Mark as used
          tCapI[zid, k].oldCapSF = tCapI[zid, k].capSF;
          tCapI[zid, k].oldCapMF = tCapI[zid, k].capMF;
          tCapI[zid, k].oldCapmh = tCapI[zid, k].capmh;

          // Get UDM SF land array index 1 - 4
          kk = tCapI[zid, k].udmSFLU;
          /* Get the minimum of SF capacity and control (sfLand) for this land use */
          temp = Math.Min(control[kk], tCapI[zid, k].capSF);
          if (temp > masterControl)
              temp = masterControl;
          // Allocate to this land use if temp > 0
          if (temp > 0)
          {
            tCapI[zid, k].sf += temp;        // Adjust base year SF
            tCapI[zid, k].chgSF = temp;      // Store increment
            tCapI[zid, k].capSF -= temp;     // Adjust capacity
            control[kk] -= temp;            // Adjust this control value

            /* Things to do if the transaction capacity has been reduced to zero */
            if (tCapI[zid, k].capSF == 0)
            {
              tCapI[zid, k].pCap_hs = 1.0;                // Set the pCap to 100%
              tCapI[zid, k].lu = tCapI[zid, k].plu;     /* Reset the base land use to the planned */
              tCapI[zid, k].devCode = 1;               // Mark as developed

              // Keep track of the acres
              z[lz].useCap.ac.asf[kk] += tCapI[zid, k].acres;
            }   // end if

            else      /* This is a partialy developed record - compute some residuals */
            {
              tCapI[zid, k].oldpCap_hs = tCapI[zid, k].pCap_hs;     /* Save the  original pcap */
              // Estimate original capacity
              if (tCapI[zid, k].pCap_hs != 1)
                temp1 = (double)tCapI[zid, k].oldCapSF / (1 - tCapI[zid, k].pCap_hs);
              else
                temp1 = 0;

              // Recompute pCap
              if (temp1 > 0)
                tCapI[zid, k].pCap_hs = 1 - ((double)tCapI[zid, k].capSF / temp1);
              // Allocate acreage
              z[lz].useCap.ac.asf[kk] += tCapI[zid, k].acres * (tCapI[zid, k].pCap_hs - tCapI[zid, k].oldpCap_hs);
            }     // End else

            /* If this is a residential to emp redev - there are some units to address */
            if (kk == 1)     // This is a res redev code
            {
              // Capacity difference
              capDiff = tCapI[zid, k].pCap_hs - tCapI[zid, k].oldpCap_hs;
              capRatio = 1 - tCapI[zid, k].oldpCap_hs;      // Ratio
              mmh = (int)((double)Math.Abs(tCapI[zid, k].capmh) / capRatio * (double)capDiff);
              tempmh = Math.Min(mmh, tCapI[zid, k].mh);
              tCapI[zid, k].mh -= tempmh;        // Adjust base year mh
              tCapI[zid, k].chgmh = -tempmh;     // Set value for mh increment
              tCapI[zid, k].capmh += tempmh;     // Add to capacity mh

              // Units lost
              
              z[lz].fcst.lh.mh -= tempmh;
              reg.fcst.lh.mh -= tempmh;
            }   // end if kk

            // Rebuild master control for checking loop
            masterControl = 0;
            for (j = 1; j < NUM_SF_LAND; j++)
              masterControl += control[j];

          }     // End if temp > 0

          k++;      // Increment counter
        }     // End while

        for (kk = 1; kk < NUM_SF_LAND; kk++)
        {
          // Add site spec to acreage used
          z[lz].useCap.ac.asf[kk] += z[lz].site.ac.asf[kk];

          // Compute remaining acreage
          z[lz].remCap.ac.asf[kk] = z[lz].capacity.ac.asf[kk] - z[lz].useCap.ac.asf[kk];
          z[lz].useCap.ac.totalSFAcres += z[lz].useCap.ac.asf[kk];
          z[lz].remCap.ac.totalSFAcres += z[lz].remCap.ac.asf[kk];

          // Used acreage by category
          reg.useCap.ac.asf[kk] += z[lz].useCap.ac.asf[kk];
          //  Remaining acreage by category
          reg.remCap.ac.asf[kk] += z[lz].remCap.ac.asf[kk];
          // Total used
          reg.useCap.ac.totalSFAcres += z[lz].useCap.ac.asf[kk];
          // Total remaining
          reg.remCap.ac.totalSFAcres += z[lz].remCap.ac.asf[kk];
        }   // end for kk

        if (masterControl > 0)     // Is there capacity remaining
        {
          MessageBox.Show("EXCEPTION -- Cannot allocate LUZ SF increment to transactions, LUZ " + (lz + 1));

        }   // end if

        // Update the capacity records with new data
        for (k = 0; k < ntCap[i]; k++)
        {
          if (tCapI[zid, k].done)     // Skip records whose flags are not set
            // Write the update data to ASCII for bulk loading
            sfCi.WriteLine((lz + 1) + "," +
                tCapI[zid, k].LCKey + "," +
                tCapI[zid, k].devCode + "," + tCapI[zid, k].lu + "," +
                tCapI[zid, k].pCap_hs + "," + tCapI[zid, k].sf + "," +
                tCapI[zid, k].mf + "," + tCapI[zid, k].mh + "," +
                tCapI[zid, k].capSF + "," + tCapI[zid, k].capMF + "," +
                tCapI[zid, k].capmh + "," + tCapI[zid, k].chgSF + "," +
                tCapI[zid, k].chgMF + "," + tCapI[zid, k].chgmh);
        }   // end for k 
        tlh[lz] = z[lz].fcst.lh.mh;
      }     // End for i

      sfCi.Close();

      loadIncrements(1);
      updateNonMHIncrements(1);     // Update capacity from the sfInc table
              
    }     // End method SFIncrements()

    /*****************************************************************************/

    /* method sfLand() */
    /// <summary>
    /// Method to distribute LUZ SF HS forecast to three land use types.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 07/17/97   tb    Initial coding
    *                 08/05/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    void sfLand()
    {
      int i, j, ztt, zDiff, adj;
      int loopCount;
      double ratio, tempSF;
      //writeToStatusBox( "Distributing HS SF to land use categories.." );

      // Distribute SF increment to SF land uses
      // SF land use arrays start at 1, 0 is dummy
      for (i = 0; i < NUM_LUZS; i++)
      {
        //writeToStatusBox( "   processing LUZ " + ( i + 1 ) );
        if (z[i].fcst.hsi.sfAdj > 0)
        {
          ztt = 0;
          if (z[i].capacity.totalSF != 0)
            ratio = (double)z[i].fcst.hsi.sfAdj / (double)z[i].capacity.totalSF;
          else
            ratio = 0;

          // Allocate to SF and keep sum of computed distributions.
          for (j = 1; j < NUM_SF_LAND; j++)      // 4 emp land categories
          {
           
            tempSF = 0.5 + (double)z[i].capacity.sf[j] * ratio;
            z[i].fcst.hsi.sfLand[j] = (int)tempSF;
           
            // Compute total SF land
            ztt += z[i].fcst.hsi.sfLand[j];
          }   // end for j

          zDiff = z[i].fcst.hsi.sfAdj - ztt;      /* Get difference due to 
                                              * rounding */
          if (zDiff > 0)
            adj = 1;
          else
            adj = -1;

          loopCount = 0;
          for (; zDiff != 0 && loopCount < 10000; loopCount++)
          {
            // Distribute to non-zero cells
            for (j = 1; j < NUM_SF_LAND; j++)
            {
              if (z[i].fcst.hsi.sfLand[j] > 0 && (z[i].fcst.hsi.sfLand[j] +
                  adj <= z[i].capacity.sf[j]))
              {
                z[i].fcst.hsi.sfLand[j] += adj;
                zDiff -= adj;
              }   // end if
              if (zDiff == 0)      // Is the difference gone
                break;
            }   // end for j
          }   // end for

          if (zDiff > 0)
            MessageBox.Show("SF land did not resolve for LUZ " + (i + 1) + "zdiff = " + zDiff);
        }     // End if

        /* Compute regional totals and remaining capacity for SF land 
        * categories */
        for (j = 1; j < NUM_SF_LAND; j++)
        {
          // Regional increment by land use type
          reg.fcst.hsi.sfLand[j] += z[i].fcst.hsi.sfLand[j];

          // LUZ remaining capacity by land use and total
          z[i].remCap.sf[j] = z[i].capacity.sf[j] - z[i].fcst.hsi.sfLand[j];
          z[i].remCap.totalSF += z[i].remCap.sf[j];

          // Regional total remaining capacity by land use type
          reg.remCap.sf[j] += z[i].remCap.sf[j];
        }   // end for j

        // Regional total remaining capacity all emp land categories
        reg.remCap.totalSF += z[i].remCap.totalSF;

      }     // End for i
    }     // End method sfLand()

    /*****************************************************************************/

    /* method sfTransactions() */
    /// <summary>
    /// Method to distribute LUZ SF HS to transcations - master control routine.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 07/17/97   tb    Initial coding
    *                 08/05/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    void sfTransactions()
    {
      int ki = 0, kd = 0,i;
      int[] iList = new int[NUM_LUZS];
      int[] dList = new int[NUM_LUZS];

      writeToStatusBox( "Distributing HS SF to transactions.." );

      for (i = 0; i < NUM_LUZS; i++)
      {
        // Build increment list of LUZs
        if (z[i].fcst.hsi.sfAdj >= 0)
          iList[ki++] = i + 1;
        else
          dList[kd++] = i + 1;
      }   // end for i

      // Call increments or decrements
      if (kd > 0)
        SFDecrements(dList, kd);
      if (ki > 0)
        SFIncrements(iList, ki);
    }     // End method sfTransactions()

    /*****************************************************************************/
    #endregion SF Procs

    #region update procedures

    /* method updatecapacity() */
    /// <summary>
    /// Method to reset variables in sitespec records after processing.
    /// </summary>
    /// <param name="capTable">Which capacity table to copy out</param>

    /* Revision History
    * 
    * STR             Date       By    Description
    * -----------------------------------------------------------------------
    *                 03/18/98   tb    Initial coding
    *                 07/18/03   df    C# revision
    * -----------------------------------------------------------------------
    */
    private void updatecapacity(string capTable)
    {

        //writeToStatusBox( "Updating CapacityNext table totals.." );
        sqlCommand.CommandText = String.Format(appSettings["update02"].Value, capTable, scenarioID, bYear);

        try
        {
            this.sqlConnection.Open();
            this.sqlCommand.ExecuteNonQuery();
        }   // end try
        catch (Exception e)
        {
            MessageBox.Show(e.ToString(), e.GetType().ToString());
        }
        finally
        {
            sqlConnection.Close();
        }

    }     // End method updatecapacity()

    // *************************************************************************

    /* method updateGQ() */
    /// <summary>
    /// Method to update the GQ data
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 07/17/97   tb    Initial coding
    *                 08/04/03   df    C# revision
    *                 02/13/06   tb    added gq site spec processing
    * --------------------------------------------------------------------------
    */
    void updateGQ()
    {

      writeToStatusBox( "Updating capacity next table for GQ." );

      sqlCommand.CommandText = String.Format(appSettings["update07"].Value, TN.capacity3, scenarioID, bYear,"gq_civ","siteGQCiv");

      try
      {
          sqlConnection.Open();
          sqlCommand.ExecuteNonQuery();
      }
      catch (SqlException e)
      {
          MessageBox.Show(e.ToString(), "SQL Exception");
      }
      finally
      {
          sqlConnection.Close();
      }

      sqlCommand.CommandText = String.Format(appSettings["update07"].Value, TN.capacity3, scenarioID, bYear,"gq_mil","siteGQMil");

      try
      {
          sqlConnection.Open();
          sqlCommand.ExecuteNonQuery();
      }
      catch (SqlException e)
      {
          MessageBox.Show(e.ToString(), "SQL Exception");
      }
      finally
      {
          sqlConnection.Close();
      }

      sqlCommand.CommandText = String.Format(appSettings["update08"].Value, TN.capacity3, scenarioID, bYear,"siteGQMil");

      try
      {
          sqlConnection.Open();
          sqlCommand.ExecuteNonQuery();
      }
      catch (SqlException e)
      {
          MessageBox.Show(e.ToString(), "SQL Exception");
          Close();
      }
      finally
      {
          sqlConnection.Close();
      }

      sqlCommand.CommandText = String.Format(appSettings["update08"].Value, TN.capacity3, scenarioID, bYear,"siteGQCiv");

      try
      {
          sqlConnection.Open();
          sqlCommand.ExecuteNonQuery();
      }
      catch (SqlException e)
      {
          MessageBox.Show(e.ToString(), "SQL Exception");
          Close();
      }
      finally
      {
          sqlConnection.Close();
      }
    }     // End procedure updateGQ

    //***********************************************************************

    /* method updateMHIncrements() */
    /// <summary>
    /// Method to update the capacity table from the mhInc table.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 07/17/97   tb    Initial coding
    *                 08/04/03   df    C# revision
    *                 02/13/06   tb    added gq site spec processing
    * --------------------------------------------------------------------------
    */
    void updateMHIncrements()
    {

      //writeToStatusBox( "Updating capacity next table for mh increments.." );
      sqlCommand.CommandText = String.Format(appSettings["update13"].Value, TN.capacity3, TN.mhIncrements, scenarioID, bYear);
      
      try
      {
        sqlConnection.Open();
        sqlCommand.ExecuteNonQuery();
      }
      catch (SqlException e)
      {
        MessageBox.Show(e.ToString(), "SQL Exception");
      }
      finally
      {
        sqlConnection.Close();
      }
      
    }     // End procedure updateMHIncrements()

    //**********************************************************************    

    /* method updateNonMHIncrements() */
    /// <summary>
    /// Method to update the capacity table from the sfInc or mfInc table
    /// </summary>
    /// <param name="type">Which table to update from. 1 = SF; 2 = MF</param>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 07/17/97   tb    Initial coding
    *                 08/04/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    void updateNonMHIncrements(int type)
    {
        string hType = "";
        if (type == 1)
            hType = TN.SFIncrements;
        else
            hType = TN.MFIncrements;

        writeToStatusBox("Updating capacity next table for " + hType + " increments..");
        sqlCommand.CommandText = String.Format(appSettings["update11"].Value, TN.capacity3, hType, scenarioID, bYear);

        try
        {
            sqlConnection.Open();
            sqlCommand.ExecuteNonQuery();
        }
        catch (SqlException sq)
        {
            MessageBox.Show(sq.ToString(), "SQL Exception");
        }
        finally
        {
            sqlConnection.Close();
        }

    }     // End method updateNonMHIncrements()

    /*****************************************************************************/

    /* method updateMFDecrements() */
    /// <summary>
    /// Method to update the capacity table from the mf_dec table.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 12/04/09   tb    Initial coding
    * --------------------------------------------------------------------------
    */
    private void updateMFDecrements()
    {

        writeToStatusBox("Updating capacityNext table for MF decrements..");
        sqlCommand.CommandText = String.Format(appSettings["update12"].Value, TN.capacity3, TN.MFDecrements, scenarioID, bYear);
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
    }     // End method updateMFDecrements()

    /*****************************************************************************/

    /* method updateMhDecrements() */
    /// <summary>
    /// Method to update the capacity table from the mh_dec table.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 12/04/09   tb    Initial coding
    * --------------------------------------------------------------------------
    */
    private void updateMhDecrements()
    {
        writeToStatusBox("Updating capacityNext table for mh decrements..");
        sqlCommand.CommandText = string.Format(appSettings["update21"].Value, TN.capacity3, scenarioID, bYear);
       
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
    }     // End method updateMhDecrements()

    /*****************************************************************************/

    /* method updateSFDecrements() */
    /// <summary>
    /// Method to update the capacity table from the sf_dec table.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 12/04/09   tb    Initial coding
    * --------------------------------------------------------------------------
    */
    private void updateSFDecrements()
    {
        writeToStatusBox("Updating capacityNext table for SF decrements..");
        sqlCommand.CommandText = String.Format(appSettings["update17"].Value, TN.capacity3, TN.SFDecrements, scenarioID, bYear);

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
    }     // End method updateSFDecrements()

    /*****************************************************************************/
    # endregion update procedures


    /* method writeMhDecrements() */
    /// <summary>
    /// Method to write updated mh decrements data to temporary capacity ASCII 
    /// table.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 12/02/09   tb    Initial coding
    * --------------------------------------------------------------------------
    */
    private void writeMhDecrements(TCapD t)
    {
      mhCd.WriteLine(t.luz + "," + t.LCKey + "," + t.devCode + "," + t.mh + "," + t.capMh + "," + t.chgMh + "," + t.pCap_hs);
      mhCd.Flush();
    }     // End method writeMhDecrements()

    /*****************************************************************************/

    /* method writeMfDecrements() */
    /// <summary>
    /// Method to write updated mf decrements data to temporary capacity ASCII 
    /// table.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 12/02/09   tb    Initial coding
    * --------------------------------------------------------------------------
    */
    private void writeMfDecrements(TCapD t)
    {
      mfCd.WriteLine(t.luz + "," + t.LCKey + "," + t.devCode + "," + t.mf + "," + t.capMf + "," + t.chgMf + "," + t.pCap_hs);
      mfCd.Flush();
    }     // End method writeMfDecrements()

    /*****************************************************************************/

    /* method writeSfDecrements() */
    /// <summary>
    /// Method to write updated mf decrements data to temporary capacity ASCII 
    /// table.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 12/02/09   tb    Initial coding
    * --------------------------------------------------------------------------
    */
    private void writeSfDecrements(TCapD t)
    {
      sfCd.WriteLine(t.luz + "," + t.LCKey + "," + t.devCode + "," + t.sf + "," + t.capSf + "," + t.chgSf + "," + t.pCap_hs);
      sfCd.Flush();
    }     // End method writeSfDecrements()

    /*****************************************************************************/


    private void btnExit_Click(object sender, System.EventArgs e)
    {
      Close();
    }

    private void HousingStock_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      caller.Visible = true;
    }
  }
    
}