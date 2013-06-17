/* Filename:    Detailed.cs
 * Program:     UDM
 * Version:      7 Sr13
 * Programmers: Terry Beckhelm
 *              Daniel Flyte (C# revision)
 * Description: This form commands all actions associated with module 4, 
 *              detailed characteristics processing.  It is called from UDM, 
 *              after the Detailed Characteristics button has been selected 
 *              from the main form.
 * 
 * Includes Procedures*/
    //  btnRun_Click()
    //  doDCWork()
    //  applyEmpOvr()
    //  applyEROvr()
    //  applyHHSOvr()
    //  applyIncomeOvr()
    //  applyVacOvr()
    //  bAgRedev()
    //  bDev()
    //  bVac()
    //  buildMGRAAcres()
    //  checkBaseTotal()
    //  checkGrandTotal()
    //  closeFiles()
    //  countMatches()
    //  dcCalc()
    //  dcEmployment()
    //  dcIncome()
    //  dcPrint()
    //  doIncomeDistribution
    //  extractEmpLU()
    //  extractDcEmpOvr()
    //  extractDCEROvr()
    //  extractHHSOvr()
    //  extractIncOvr()
    //  extractMGRABase()
    //  extractRegionalControls()
    //  extractTransactions()
    //  extractVacRates()
    //  getDCOutliers()
    //  getLUEmpDist()
    //  getUpdateSum()
    //  IncomeFinish1()
    //  IncomeFinish2()
    //  loadMB()
    //  loadZB()
    //  loadLUZHistory()
    //  processParms()
    //  processSiteSpecTransactions()
    //  storeMB()
    //  storeRC()
    //  mgraMain()
    //  update()
    //  openFiles()
    //  printColSum()
    //  printDCEmp()
    //  printDCHHS()
    //  printDCIncome()
    //  printDCOvr()
    //  printDCPop()
    //  printDCRates()
    //  controlGQ()

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Configuration;

namespace Sandag.TechSvcs.RegionalModels
{
	public class Detailed : BaseForm
	{
    #region Instance Variables
   
    private bool dcEROvr;           // DC erHH overrides
    private bool dcESOvr;           // DC employment sector overrides
    private bool dcHHSOvr;          // DC HHS overrides
    private bool dcIncOvr;          // DC income overrides
    private bool dcVacOvr;          // DC vacancy rates overrides
    private bool controlDCOvr = true;      // DC rates controlling
      
    private int colSame;            /* Flag used to indicate convergence in
                                     * plus/minus redistribution */
    private int numLUCats;
    private int phase;
    private int rowSame;            /* Flag used to indicate convergence in
                                     * plus/minus redistribution */
    private int[] zbi;
    private double[] rat;
    private double[] income_distribution_adjustments;  // annual adjustments to income distributions

    private EmpLU[] elu;            /* Employment distribution by land use 
                                     * category */
    private Master[] zt;
    private MBMaster[] mbData;

    private StreamWriter dcEmp;       // DC employment output
    private StreamWriter dcHHS;       // DC hhs and er output
    private StreamWriter dcInc;       // DC income output
    private StreamWriter dcOut;       // DC results output
    private StreamWriter dcOutliers;  // DC rate outliers output
    private StreamWriter dcOvr;       // DC overrides
    private StreamWriter dcPop;       // DC pop vars output
    private StreamWriter dcRates;     // DC vacancy rates
    private StreamWriter dcGQC;  
   
    private RegionalControls rc;

    private System.Windows.Forms.Button btnRun;
    private System.Windows.Forms.Button btnExit;
    private System.Windows.Forms.TabPage tabGeneral;
    private System.Windows.Forms.CheckBox chkEROvr;
    private System.Windows.Forms.CheckBox chkESOvr;
    private System.Windows.Forms.CheckBox chkVacRatesOvr;
    private System.Windows.Forms.CheckBox chkHHSOvr;
    private System.Windows.Forms.CheckBox chkIncOvr;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label lblYearsIncrement;
    private System.Windows.Forms.ComboBox cboScenario;
    private System.Windows.Forms.ComboBox cboYears;
    private System.Windows.Forms.TabControl tabControl1;
    private Label label2;
    private System.Windows.Forms.Label lblFormTitle;
    #endregion Instance Variables

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Detailed));
            this.btnRun = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.lblFormTitle = new System.Windows.Forms.Label();
            this.tabGeneral = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.chkEROvr = new System.Windows.Forms.CheckBox();
            this.chkESOvr = new System.Windows.Forms.CheckBox();
            this.chkVacRatesOvr = new System.Windows.Forms.CheckBox();
            this.chkHHSOvr = new System.Windows.Forms.CheckBox();
            this.chkIncOvr = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblYearsIncrement = new System.Windows.Forms.Label();
            this.cboScenario = new System.Windows.Forms.ComboBox();
            this.cboYears = new System.Windows.Forms.ComboBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabGeneral.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtStatus
            // 
            this.txtStatus.Location = new System.Drawing.Point(16, 328);
            this.txtStatus.Size = new System.Drawing.Size(352, 88);
            // 
            // btnRun
            // 
            this.btnRun.BackColor = System.Drawing.Color.LightGreen;
            this.btnRun.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRun.Location = new System.Drawing.Point(80, 432);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(88, 40);
            this.btnRun.TabIndex = 1;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = false;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // btnExit
            // 
            this.btnExit.BackColor = System.Drawing.Color.Red;
            this.btnExit.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExit.Location = new System.Drawing.Point(184, 432);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(88, 40);
            this.btnExit.TabIndex = 2;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = false;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // lblFormTitle
            // 
            this.lblFormTitle.Font = new System.Drawing.Font("Garamond", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFormTitle.ForeColor = System.Drawing.Color.Navy;
            this.lblFormTitle.Location = new System.Drawing.Point(8, 16);
            this.lblFormTitle.Name = "lblFormTitle";
            this.lblFormTitle.Size = new System.Drawing.Size(200, 64);
            this.lblFormTitle.TabIndex = 3;
            this.lblFormTitle.Text = "Detailed Characteristics";
            // 
            // tabGeneral
            // 
            this.tabGeneral.BackColor = System.Drawing.SystemColors.Control;
            this.tabGeneral.Controls.Add(this.label2);
            this.tabGeneral.Controls.Add(this.chkEROvr);
            this.tabGeneral.Controls.Add(this.chkESOvr);
            this.tabGeneral.Controls.Add(this.chkVacRatesOvr);
            this.tabGeneral.Controls.Add(this.chkHHSOvr);
            this.tabGeneral.Controls.Add(this.chkIncOvr);
            this.tabGeneral.Controls.Add(this.label1);
            this.tabGeneral.Controls.Add(this.lblYearsIncrement);
            this.tabGeneral.Controls.Add(this.cboScenario);
            this.tabGeneral.Controls.Add(this.cboYears);
            this.tabGeneral.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabGeneral.Location = new System.Drawing.Point(4, 25);
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.Size = new System.Drawing.Size(312, 205);
            this.tabGeneral.TabIndex = 1;
            this.tabGeneral.Text = "General";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(16, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(120, 16);
            this.label2.TabIndex = 20;
            this.label2.Text = "Overrides";
            // 
            // chkEROvr
            // 
            this.chkEROvr.Location = new System.Drawing.Point(16, 181);
            this.chkEROvr.Name = "chkEROvr";
            this.chkEROvr.Size = new System.Drawing.Size(128, 21);
            this.chkEROvr.TabIndex = 19;
            this.chkEROvr.Text = "ER/HH ";
            // 
            // chkESOvr
            // 
            this.chkESOvr.Location = new System.Drawing.Point(16, 157);
            this.chkESOvr.Name = "chkESOvr";
            this.chkESOvr.Size = new System.Drawing.Size(200, 18);
            this.chkESOvr.TabIndex = 18;
            this.chkESOvr.Text = "LUZ Employment Sectors";
            // 
            // chkVacRatesOvr
            // 
            this.chkVacRatesOvr.Location = new System.Drawing.Point(16, 133);
            this.chkVacRatesOvr.Name = "chkVacRatesOvr";
            this.chkVacRatesOvr.Size = new System.Drawing.Size(160, 18);
            this.chkVacRatesOvr.TabIndex = 17;
            this.chkVacRatesOvr.Text = "LUZ Vacancy Rates";
            // 
            // chkHHSOvr
            // 
            this.chkHHSOvr.Location = new System.Drawing.Point(16, 109);
            this.chkHHSOvr.Name = "chkHHSOvr";
            this.chkHHSOvr.Size = new System.Drawing.Size(208, 18);
            this.chkHHSOvr.TabIndex = 16;
            this.chkHHSOvr.Text = "LUZ HHS ";
            // 
            // chkIncOvr
            // 
            this.chkIncOvr.Location = new System.Drawing.Point(16, 85);
            this.chkIncOvr.Name = "chkIncOvr";
            this.chkIncOvr.Size = new System.Drawing.Size(152, 18);
            this.chkIncOvr.TabIndex = 15;
            this.chkIncOvr.Text = "LUZ Income Parms";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(152, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 16);
            this.label1.TabIndex = 14;
            this.label1.Text = "Scenario";
            // 
            // lblYearsIncrement
            // 
            this.lblYearsIncrement.Location = new System.Drawing.Point(16, 10);
            this.lblYearsIncrement.Name = "lblYearsIncrement";
            this.lblYearsIncrement.Size = new System.Drawing.Size(128, 16);
            this.lblYearsIncrement.TabIndex = 13;
            this.lblYearsIncrement.Text = "Increment";
            // 
            // cboScenario
            // 
            this.cboScenario.ItemHeight = 16;
            this.cboScenario.Items.AddRange(new object[] {
            "0 - EP"});
            this.cboScenario.Location = new System.Drawing.Point(160, 34);
            this.cboScenario.Name = "cboScenario";
            this.cboScenario.Size = new System.Drawing.Size(88, 24);
            this.cboScenario.TabIndex = 5;
            // 
            // cboYears
            // 
            this.cboYears.ItemHeight = 16;
            this.cboYears.Items.AddRange(new object[] {
            "2012 - 2020",
            "2020 - 2035",
            "2035 - 2050"});
            this.cboYears.Location = new System.Drawing.Point(16, 34);
            this.cboYears.Name = "cboYears";
            this.cboYears.Size = new System.Drawing.Size(104, 24);
            this.cboYears.TabIndex = 4;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabGeneral);
            this.tabControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.HotTrack = true;
            this.tabControl1.ItemSize = new System.Drawing.Size(67, 21);
            this.tabControl1.Location = new System.Drawing.Point(16, 88);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(320, 234);
            this.tabControl1.TabIndex = 12;
            // 
            // Detailed
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(402, 496);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.lblFormTitle);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnRun);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Detailed";
            this.Text = "Detailed Characteristics";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.Detailed_Closing);
            this.Controls.SetChildIndex(this.btnRun, 0);
            this.Controls.SetChildIndex(this.btnExit, 0);
            this.Controls.SetChildIndex(this.lblFormTitle, 0);
            this.Controls.SetChildIndex(this.tabControl1, 0);
            this.Controls.SetChildIndex(this.txtStatus, 0);
            this.tabGeneral.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

    }
		#endregion
		
    public Detailed( Form form )
	{
		InitializeComponent();
        writeToStatusBox( "Awaiting user input.." );
        caller = form;
	}

    #region Run button processing
    //***************************
    //Includes Procedures
    //  btnRun_Click()
    //  doDCWork()
    //

    private void btnRun_Click(object sender, System.EventArgs e)
    {
      if (!processParams())
        return;
      MethodInvoker mi = new MethodInvoker( doDCWork );
      mi.BeginInvoke( null, null );
    }
    
    /****************************************************************************/

    /* method doDCWork() */
    /// <summary>
    /// Method to perform all slow, processor-intensive methods required for 
    /// detailed characteristics processing.  This method will run on its own 
    /// thread, separate from the UI.
    /// </summary>
      
    /* Revision History
    * 
    * STR             Date       By    Description
    * -------------------------------------------------------------------------
    *                 12/23/97   tb    Initial coding
    *                 08/18/03   df    C# revision
    * -------------------------------------------------------------------------
    */
    void doDCWork()
    {
      try
      {
        openFiles();

        // Load the LUZ emp and HS history array.
        if (!UDMUtils.extractHistory(this, scenarioID, bYear))
        {
          MessageBox.Show("Mod 4 failed on extractHistory");
          Application.Exit();
        }// end if

        if( !UDMUtils.extractLUZTemp( this, scenarioID, bYear ) )
        {
          MessageBox.Show("Mod 4 failed on extractLUZTemp");
          Application.Exit();
        }// end if

        if( !UDMUtils.copyCapacity( this, 4 , scenarioID, bYear,fYear) )
        {
          MessageBox.Show("Mod 4 failed on copyCapacity");
          Application.Exit();
        }// end if
        
        // Load the base data
        if( !UDMUtils.extractLUZBase( this, zbi , scenarioID, bYear) )
        {
          MessageBox.Show("Mod 4 failed om extractLUZBase");
          Application.Exit();
        }// end if

        // Load temp vacancy rate stuff from module 3
        if( !DCUtils.extractDCRates( this, z, reg, dcOut ) )
        {
          MessageBox.Show("Mod 4 failed on extractDCRates");
          Application.Exit();
        }// end if

        for( int i = 0; i < NUM_LUZS; i++ )
          z[i].incomeSwitch = INCOMESWITCH;      // set default income switch
        
        // Load overrides.
        if( dcEROvr )
        {
          extractEROvr();
          applyEROvr();
        }// end if
        if( dcESOvr )
        {
          extractEmpOvr();
          applyEmpOvr();
        }// end if
        if( dcHHSOvr )
        {
          extractHHSOvr();
          applyHHSOvr();
        }// end if
        if( dcIncOvr )
        {
          extractIncomeOvr();
          applyIncomeOvr();
        }// end if
        if( dcVacOvr )
        {
          extractVacOvr();
          applyVacOvr();
        }// end if
        // Load regional controls
        extractRegionalControls(); 
        //control GQ
        ControlGQ(fYear);

        // Get mgraBase data
        extractMGRABase();

        // Extract employment land use distributions
        extractEmpLU();

        // First pass through capacity for transactions data
        extractTransactions();

        // Occupied units (HH), hhp, er, gq
        dcCalc();

        // Compute rates
        DCUtils.doDCRates( this, z, dcOut );
       // Process outliers
        getDCOutliers();

        dcIncome();
        dcEmployment();
          
        // DC output
        dcPrint();
        closeFiles();

        // Process the MGRA data
        mgraMain();
    
        DCUtils.updateLUZHistory(this,z);
        DCUtils.writeLUZHistory(this, z, scenarioID, fYear);
        loadLUZHistory();

        //buildVerificationTable(this.scenarioID,this.fYear,this.capacityNextTable);
        writeToStatusBox("Completed Detailed Characteristics Processing");
        MessageBox.Show( Environment.NewLine + "Completed Detailed Characteristics processing!" );
    }     // End try

    catch( Exception e )
    {
        MessageBox.Show( e.ToString(), e.GetType().ToString() );
    }
  }     // End method doDCWork()
    #endregion Run button processing

    #region apply overrides procedures
    //******************************
    //Includes Procedures
    //  applyEmpOvr()
    //  applyEROvr()
    //  applyHHSOvr()
    //  applyIncomeOvr()
    //  applyVacOvr()

  /*****************************************************************************/
  
  /* method applyEmpOvr() */
  /// <summary>
  /// Method to apply any employment sector overrides.
  /// </summary>
  
  /* Revision History
   * 
   * STR             Date       By    Description
   * --------------------------------------------------------------------------
   *                 12/23/97   tb    Initial coding
   *                 08/20/03   df    C# revision
   * --------------------------------------------------------------------------
   */
    void applyEmpOvr()
    {
      for( int i = 0; i < NUM_LUZS; i++ )
      {
        if( z[i].esOvr)      // If overrides exist
          for( int j = 0; j < NUM_EMP_SECTORS; j++ )
            z[i].fcst.ei.sectorsAdj[j] = z[i].eso.sectors[j];
      }
    }     // End method applyDCEmpOvr()

  /*****************************************************************************/

  /* method applyEROvr() */
  /// <summary>
  /// Method to apply and ER overrides.
  /// </summary>
  
  /* Revision History
   * 
   * STR             Date       By    Description
   * --------------------------------------------------------------------------
   *                 08/20/98   tb    Initial coding
   *                 08/20/03   df    C# revision
   * --------------------------------------------------------------------------
   */
    void applyEROvr()
    {
      for( int i = 0; i < NUM_LUZS; i++ )
      {
        if( z[i].erOvr )      // If overrides exist
        {
          if( z[i].ro.erHH > 0 )
            z[i].fcst.r.erHH = z[i].ro.erHH;
        }
      }
    }     // End method applyDCEROvr()

  /*****************************************************************************/

  /* method applyHHSOvr() */
  /// <summary>
  /// Method to apply any HHS overrides.
  /// </summary>
  
  /* Revision History
   * 
   * STR             Date       By    Description
   * --------------------------------------------------------------------------
   *                 08/20/98   tb    Initial coding
   *                 08/20/03   df    C# revision
   * --------------------------------------------------------------------------
   */
    void applyHHSOvr()
    {
      for( int i = 0; i < NUM_LUZS; i++ )
      {
        if( z[i].hhsOvr )     // If overrides exist
        {
            // HHS
          if( z[i].ro.hhs > 0 )
            z[i].fcst.r.hhs = z[i].ro.hhs;
        }
      }
    }     // End method applyHHSOvr()*/

  /*****************************************************************************/

  /* method applyIncomeOvr() */
  /// <summary>
  /// Method to apply any income overrides.
  /// </summary>
  
  /* Revision History
   * 
   * STR             Date       By    Description
   * --------------------------------------------------------------------------
   *                 08/13/98   tb    Initial coding
   *                 08/20/03   df    C# revision
   * --------------------------------------------------------------------------
   */
    void applyIncomeOvr()
    {
      for( int i = 0; i < NUM_LUZS; i++ )
      {
        if( z[i].iOvr )     // If overrides exist
        {
            // logNormal curve parms
          if( z[i].ro.medianIncome > 0 )
            z[i].fcst.i.median = z[i].ro.medianIncome;
          if( z[i].ro.nla > 0 )
            z[i].fcst.nla = z[i].ro.nla;
          if( z[i].ro.asd > 0 )
            z[i].fcst.asd = z[i].ro.asd;

            // Income model switch
          if( z[i].ro.incomeSwitch > 0 )
            z[i].incomeSwitch = z[i].ro.incomeSwitch;
        }
      }     // End for
    }     // End method applyIncomeOvr()

  /*****************************************************************************/

  /* method applyVacOvr() */
  /// <summary>
  /// Method to apply any vacancy rate overrides.
  /// </summary>
  
  /* Revision History
   * 
   * STR             Date       By    Description
   * --------------------------------------------------------------------------
   *                 12/02/97   tb    Initial coding
   *                 08/20/03   df    C# revision
   * --------------------------------------------------------------------------
   */
    void applyVacOvr()
    {
      for( int i = 0; i < NUM_LUZS; i++ )
      {
        if( z[i].vacOvr )     // If overrides exist
        {
            // Vcancy rates
          if (z[i].ro.vSF > 0 )
            z[i].fcst.r.vSF = z[i].ro.vSF;
          if( z[i].ro.vMF > 0 )
            z[i].fcst.r.vMF = z[i].ro.vMF;
          if( z[i].ro.vmh > 0 )
            z[i].fcst.r.vmh = z[i].ro.vmh;
        }
      }
    }     // End method applyVacOvr()
#endregion

    #region build procedures
    //*******************************
    //Includes procedures
    //  bAgRedev()
    //  bDev()
    //  bVac()
    //  buildMGRAAcres()
    //  buildVerificationTable()
  /*****************************************************************************/

  /* method bAgRedev() */
  /// <summary>
  /// Method to build agricultural redevelopment acreage.
  /// </summary>
  
  /* Revision History
   * 
   * STR             Date       By    Description
   * --------------------------------------------------------------------------
   *                 08/27/96   tb    Initial coding
   *                 08/22/03   df    C# revision
   * --------------------------------------------------------------------------
   */
    void bAgRedev( int plu, double[] mAcres, double acres )
    {
      int id = 99;

      /* Ag industrial.  Light industry, airports, railways, commo, row and military */
      if( UDMUtils.inIndustrial( plu ) )
        id = 37;

      /* Ag commercial.  Commercial, public service, hospitals, recreation, hotels */
      else if( UDMUtils.inCommercial( plu ) )
        id = 38;

        // Ag office
      else if( UDMUtils.inOffice( plu ) )
        id = 39;

        // Ag schools
      else if( UDMUtils.inSchools( plu ) )
        id = 40;

      else      // These cover the single values
      {
        switch( plu )
        {
          case 1000:      // Ag low density SF
            id = 34;
            break;
          case 1100:      // Ag SF
          case 1110:
          case 1120:
          case 1190:
            id = 35;
            break;
          case 1200:      // Ag MF
          case 1280:
            id = 36;
            break;
          case 4112:      // Ag roads
          case 4117:      // Railroad row
          case 4118:      // Ag fwys
            id = 41;
              break;
        }
      }
      if( id != 99 )
      {
        try
        {
          mAcres[id] += acres;
        }
        catch( Exception e )
        {
          MessageBox.Show( e.ToString(), e.GetType().ToString() );
          Close();
        }
      }

    }     // End method bAgRedev()

  /*****************************************************************************/

  /* method bDev() */
  /// <summary>
  /// Method to build developed acreage.
  /// </summary>
  
  /* Revision History
   * 
   * STR             Date       By    Description
   * --------------------------------------------------------------------------
   *                 08/27/96   tb    Initial coding
   *                 08/20/03   df    C# revision
   * --------------------------------------------------------------------------
   */
    void bDev( int lu, double[] mAcres, double acres )
    {
      int id = 99;
          
        // Other developed
      if( UDMUtils.inOther( lu ) )
        id = 4;

        // Ag developed, extractive or landfills
      else if( UDMUtils.inAg( lu ))
        id = 5;

        /* Developed industrial.  Light industry, airports, railways, commo, row and military */
      else if( UDMUtils.inIndustrial( lu ) )
        id = 6;

        /* Developed commercial.  Commercial, pub serv, hospitals, recreation,hotels */
      else if( UDMUtils.inCommercial( lu ) )
        id = 7;

        // Developed office
      else if( UDMUtils.inOffice( lu ) )
        id = 8;

        // Developed schools
      else if( UDMUtils.inSchools( lu ) )
        id = 9;

        // Developed parks
      else if( UDMUtils.inDevParks( lu ) )
        id = 11;

        // Water
      else if( UDMUtils.inWater( lu ) )
        id = 13;

      else      // These cover the single values
      {
        switch( lu )
        {
          case 1000:      // Developed low density SF
            id = 0;
            break;
          case 1100:      // Developed SF
          case 1110:
          case 1120:
          case 1190:
            id = 1;
            break;
          case 1200:      // Developed MF
          case 1280:
          case 1290:
            id = 2;
            break;
          case 1300:      // Developed mh
            id = 3;
            break;
          case 4112:      // Developed roads
          case 4117:      // Railroad row
          case 4118:      // Developed fwys
            id = 10;
            break;
          case 6701:      // Military
          case 6702:
          case 6703:
            id = 12;
            break;
          case 9700:        //mixed use
            id = 14;
            break;
        }
      }
      if( id != 99)
        mAcres[id] += acres;
    }     // End method bDev()

  /*****************************************************************************/

  /* method bVac() */
  /// <summary>
  /// Method to build vacant acreage.
  /// </summary>
  
  /* Revision History
   * 
   * STR             Date       By    Description
   * --------------------------------------------------------------------------
   *                 08/27/96   tb    Initial coding
   *                 08/20/03   df    C# revision
   * --------------------------------------------------------------------------
   */
    void bVac( int plu, double[] mAcres, double acres )
    {
      int id = 99;

        // Other vacant
      if( UDMUtils.inOther( plu ) )
        id = 19;

        // Vacant ag, extractive or landfills
      else if( UDMUtils.inAg( plu ))
        id = 20;

        /* Vacant industrial.  Light industry, airports, railways, commo, row
         * and military */
      else if( UDMUtils.inIndustrial( plu ) )
        id = 21;

        /* Vacant commercial.  Commercial, public service, hospitals,
         * recreation, hotels */
      else if( UDMUtils.inCommercial( plu ) )
        id = 22;

        // Vacant office
      else if( UDMUtils.inOffice( plu ) )
        id = 23;

        // Vacant schools
      else if( UDMUtils.inSchools( plu ) )
        id = 24;
      else if (plu >= 7600 && plu <= 7700)     //vac parks
        id = 27;

      else
      {
        switch( plu )
        {
          case 1000:      // Vacant low density SF
            id = 15;
            break;
          case 1100:      // Vacant SF
          case 1110:
          case 1120:
          case 1190:
            id = 16;
            break;
          case 1200:      // Vacant MF
          case 1280:
            id = 17;
            break;
          case 1300:      // Vacant mh
            id = 18;
            break;
          case 4112:      // Vacant roads
          case 4117:
          case 4118:
            id = 25;
            break;
          case 9700:     // mixed use
            id = 26;
            break;
        }
      }
      if( id != 99 )
        mAcres[id]  += acres;
    }     // End method bVac()

  /*****************************************************************************/

  /* method buildMGRAAcres() */
  /// <summary>
  /// Method to split mgra capacity data to MGRAs by LU devCode or LU
  /// </summary>
  
  /* Revision History
   * 
   * STR             Date       By    Description
   * --------------------------------------------------------------------------
   *                 03/10/97   tb    Initial coding
   *                 08/20/03   df    C# revision
   * --------------------------------------------------------------------------
   */
    void buildMGRAAcres( int mgra, int lu, int devCode, int plu, double acres,double pCap_emp, double pCap_Hs,int emp )
    {
      double tempDev, tempRem,multiplier;
        /* Create acre splits from pCap.  These may not be used depending on devCode. */
      multiplier = pCap_emp + pCap_Hs;
      if (multiplier > 1)
        multiplier = 1;
      tempDev = acres * multiplier;
      tempRem = acres - tempDev;
      
      try
      {

        switch( devCode )
        {
          case 1:     // Developed
          case 17:    // screwed up site spec records
            bDev( lu, mbData[mgra].fcst.acres, acres );
            break;

          case 2:     // Constrained
            mbData[mgra].fcst.acres[50] += acres;
            break;

          case 3:     // Vacant
            bDev( plu, mbData[mgra].fcst.acres, tempDev );
            bVac( plu, mbData[mgra].fcst.acres, tempRem );
            break;

          case 4:     // Employment infill
            bDev( plu, mbData[mgra].fcst.acres, acres );
            mbData[mgra].fcst.acres[46] += tempRem;
            break;

          case 5:     // Single family infill
            bDev( plu, mbData[mgra].fcst.acres, acres );
            mbData[mgra].fcst.acres[44] += tempRem;
            break;

          case 6:     // Multi-family infill
           
            bDev( plu, mbData[mgra].fcst.acres, acres );
            mbData[mgra].fcst.acres[45] += tempRem;
            break;

          case 7:     // Res-emp redev
           
            bDev( lu, mbData[mgra].fcst.acres, tempRem );
            
            bDev( plu, mbData[mgra].fcst.acres, tempDev );
            // Now process the redev category
            switch( lu )
            {
              case 1100:      // SF - emp redev
              case 1110:
              case 1120:
              case 1190:
                mbData[mgra].fcst.acres[29] += tempRem;
                break;

              case 1200:      // MF - emp redev
              case 1280:
                mbData[mgra].fcst.acres[30] += tempRem;
                break;

              case 1300:
                mbData[mgra].fcst.acres[33] += tempRem;
                break;
            }
            break;      // End case 7

          case 8:     // SF-MF redev
           
            bDev( lu, mbData[mgra].fcst.acres, tempRem );
           
            bDev( plu, mbData[mgra].fcst.acres, tempDev );
            // Now process the redev category
            mbData[mgra].fcst.acres[28] += tempRem;
            break;

          case 9:          // mh-res redev
           
            bDev( lu, mbData[mgra].fcst.acres, tempRem);
            bDev( plu, mbData[mgra].fcst.acres, tempDev );

            // Now process the redev category
            switch (plu)
            {
              case 1100:     /* mh - sf redev */
              case 1110:
              case 1120:
              case 1190:
                mbData[mgra].fcst.acres[31] += tempRem;
                break;

              case 1200:     /* mh - mf redev */
              case 1280:
                mbData[mgra].fcst.acres[32] += tempRem;
                break;
            }
            break;

          case 10:      // Ag redev
            
            bDev( lu, mbData[mgra].fcst.acres, tempRem );
          
            bDev( plu, mbData[mgra].fcst.acres, tempDev );
            // Now process the redev category
            bAgRedev( plu, mbData[mgra].fcst.acres, tempRem );
            break;

          case 11:      // Emp-res redev
           
            bDev( lu, mbData[mgra].fcst.acres, tempRem );
           
            bDev( plu, mbData[mgra].fcst.acres, tempDev );
            // Now process the redev category
            mbData[mgra].fcst.acres[42] += tempRem;
            break;

          case 12:      // Emp redev
            
            bDev( lu, mbData[mgra].fcst.acres, tempRem );
           
            bDev( plu, mbData[mgra].fcst.acres, tempDev);
            // Now process the redev category
            mbData[mgra].fcst.acres[43] += tempRem;
            break;

          case 13:   // res to road or freeway
          case 14:   // emp to road or freeway
            bDev(lu,mbData[mgra].fcst.acres,tempDev);
            bDev( plu, mbData[mgra].fcst.acres, tempRem );
            break;
          
          case 15:  //emp or res to mixed use
            bDev(lu,mbData[mgra].fcst.acres,tempRem);
            mbData[mgra].fcst.acres[14] += tempDev;
            break;

          case 16:  //vacant to mixed use
           
            mbData[mgra].fcst.acres[14] += tempDev;
            mbData[mgra].fcst.acres[26] += tempRem;
            break;
        
        }     // End switch
      }
      catch( Exception e )
      {
        MessageBox.Show( e.ToString(), e.GetType().ToString() );
        Close();
      }

      // process the employment records for employment by land use type
      if (emp > 0)
      {
        try
        {
          /* indus */
          /* light industry, airports, railways, commo, road row, and military */
                 
          if( ( lu >= 2001 && lu <= 2105 ) || ( lu >= 4100 && lu <= 4111 ) ||
            ( lu >= 4113 && lu <= 4116 ) || lu == 4119 || lu == 4120 ||
            lu == 5001 )
            mbData[mgra].fcst.empLU[0] += emp;

            /* dev comm */
            /* commercial, pub serv, hospitals, recreation, hotels */
          else if( lu == 5000 || ( lu >= 5002 && lu <= 5009 ) ||
            ( lu >= 6100 && lu <= 6509 ) || ( lu >= 7200 && lu <= 7210 ) ||
            ( lu >= 1500 && lu <= 1503 ) || lu == 9700)
            mbData[mgra].fcst.empLU[1] += emp;

            /* dev_office */
          else if( lu >= 6000 && lu <= 6003 )
            mbData[mgra].fcst.empLU[2] += emp;

          else
            mbData[mgra].fcst.empLU[3] += emp;
        }
        catch( Exception e )
        {
          MessageBox.Show( e.ToString(), e.GetType().ToString() );
          Close();
        }
      }   // end if
    }     // End method buildMGRAAcres()

    //*****************************************************************************   
   
    #endregion

    #region misc check procedures
    //*******************************
    //Inclused procedures
    //  checkBaseTotal()
    //  checkGrandTotal()
    //  countMatches()

  /*****************************************************************************/

  /* method checkBaseTotal() */
  /// <summary>
  /// Method to estimate values for rows with control totals and no base data.
  /// </summary>
  
  /* Revision History
   * 
   * STR             Date       By    Description
   * --------------------------------------------------------------------------
   *                 08/08/94   tb    Initial coding
   *                 08/26/03   df    C# revision
   * --------------------------------------------------------------------------
   */
    void checkBaseTotal( int numRows, int numCols, int[,] matrix, int[] nuRowt,
                         int[] nuColt, int colTotal )
    {
      int total;
      int i,j;
      int[] rowSum = new int[NUM_LUZS];
      int[] colSum = new int[NUM_HH_INCOME];
        // Compute row and column totals
      total = getUpdateSum( numRows, numCols, matrix, rowSum, colSum );
      for( j = 0; j < numCols; j++ )
      {
        for( i = 0; i < numRows; i++ )
        {
          if( rowSum[i] == 0 && nuRowt[i] > 0)
            matrix[i,j] = ( int )( ( double )( nuRowt[i] * nuColt[j] ) /
                          ( double )colTotal );
        }   // end for
      }
    }     // End method checkBaseTotal()

  /*****************************************************************************/

  /* method checkGrandTotal() */
  /// <summary>
  /// Method to compute new row and column totals and redistribute if unequal.
  /// Returns colTotal.
  /// </summary>
  
  /* Revision History
   * 
   * STR             Date       By    Description
   * --------------------------------------------------------------------------
   *                 08/08/94   tb    Initial coding
   *                 08/25/03   df    C# revision
   * --------------------------------------------------------------------------
   */
    int checkGrandTotal( int numRows, int numCols, int[,] matrix, int[] nuRowt,
                         int[] nuColt )
    {
      int rowTotal = 0, colTotal = 0;
      int rSum = 0, total = 0, rt, rTot;
      int ii, i, j;
      int[] rowSum = new int[NUM_LUZS];
      int[] colSum = new int[NUM_HH_INCOME];
        // Compute total of row control vector
      for( i = 0; i < numRows; i++ )
        rowTotal += nuRowt[i];

      total = getUpdateSum( numRows, numCols, matrix, rowSum, colSum );

        // Total col vectors
      for( j= 0; j < numCols; j++ )
        colTotal += nuColt[j];
       
        /* Check new row and col control totals.  If they are not equal adjust,
         * otherwise continue with allocation. */
      if( colTotal != rowTotal )
      {
          // The grand totals are different, adjust the rows.
        for( i = 0; i < numRows; i++ )
            /* Keep track of cases where old row total = new row total.  Don't
             * want to mess with these */
          if( rowSum[i] == nuRowt[i] )
            rSum += nuRowt[i];          
        total = 0;      // Reset total
        rt = 0;
        ii = 0;
        for( i = 0; i < numRows; i++ )
        {
          rTot = rowTotal - rSum;
          if( rTot == 0 || ( nuRowt[i] == rowSum[i] ) )
            continue;
          else
          {
              // Redistribute based on column grant total
            nuRowt[i] = ( int )( 0.5 + ( double )nuRowt[i] *
                        ( double )( colTotal - rSum ) / ( double )rTot );
            total += nuRowt[i];
            if( nuRowt[i] <= rt )
              continue;
            else
            {
              rt = nuRowt[i];
              ii = i;
            }
          }     // End else
        }     // End for i

        // Force row control totals to grand control total
        nuRowt[ii] += colTotal - total - rSum;
      }     // End if colTotal
      return colTotal;
    }     // End procedure checkGrandTotal()

  /*****************************************************************************/

    /* method closeFiles() */
    /// <summary>
    /// Method to close all ASCII output files.
    /// </summary>

    /* Revision History
     * 
     * STR             Date       By    Description
     * --------------------------------------------------------------------------
     *                 01/21/10   tb    Initial coding
     * --------------------------------------------------------------------------
     */
    void closeFiles()
    {
      try
      {
       
        dcEmp.Close();
        dcHHS.Close();
        dcInc.Close();
        dcOut.Close();
        dcOutliers.Close();
        dcOvr.Close();
        dcPop.Close();
        dcRates.Close();
      }
      catch (IOException i)
      {
        MessageBox.Show("Error closing file.  " + i.Message, "IO Exception");
      }
    }     // End method closeFiles()

    /*****************************************************************************/

  /* method countMatches() */
  /// <summary>
  /// Method to count the matches in row and col totals.
  /// </summary>
  
  /* Revision History
   * 
   * STR             Date       By    Description
   * --------------------------------------------------------------------------
   *                 08/08/94   tb    Initial coding
   *                 08/25/03   df    C# revision
   * --------------------------------------------------------------------------
   */
    void countMatches( int numRows, int numCols, int[] rowSum, int[] colSum,
                       int[] nuRowt, int[] nuColt )
    {
      rowSame = 0;
      colSame = 0;
      for( int i = 0; i < numRows; i++ )     
        if( ( double )rowSum[i] / ( double )nuRowt[i] >= .999999 &&
            ( double )rowSum[i] / ( double )nuRowt[i] <= 1.000003 )
          rowSame++;
      for( int j = 0; j < numCols; j++ )
        if( ( double )colSum[j] / ( double )nuColt[j] >= .999999 &&
            ( double )colSum[j] / ( double )nuColt[j] <= 1.000003 )
          colSame++;
    }     // End method countMatches()
    #endregion

    #region main dc computation procedures

    //*****************************
    //Includes procedures
    //  dcCalc()
    //  dcEmployment()
    //  dcIncome()
    //  dcPrint()
    //  doIncomeDistribution

  /*****************************************************************************/

  /* method dcCalc() */
  /// <summary>
  /// Method to call routines to calculate hh, hhp, er, gq.
  /// </summary>
  
  /* Revision History
   * 
   * STR             Date       By    Description
   * --------------------------------------------------------------------------
   *                 12/08/97   tb    Initial coding
   *                 08/22/03   df    C# revision
   * --------------------------------------------------------------------------
   */
    void dcCalc()
    {
      DCUtils.dcCalcHH( this, z, dcOut, rc, controlDCOvr );
      DCUtils.dcCalcHHP( this, z, rc, controlDCOvr );
      DCUtils.dcCalcGQ( this, z, rc );
      DCUtils.dcCalcER( this, z, rc, dcOut, controlDCOvr );

    }     // End method dcCalc()

  /*****************************************************************************/

  /* method dcEmployment() */
  /// <summary>
  /// Method to calculate employment distribution.
  /// </summary>
  
  /* Revision History
   * 
   * STR             Date       By    Description
   * --------------------------------------------------------------------------
   *                 12/10/97   tb    Initial coding
   *                 08/25/03   df    C# revision
   *                 11/18/09   tb    major changes to the way that the luz emp increment (decrement) is distributed to the sectors
   *                                  because for sr13, we had large job losses and the pachinko didn't work
   *                                  the normal cumulative distribution can yield some values that fall below the first random integer (they can be less than 1 
   *                                  and never get picked) since we set the randomizer to return integers 1 - 100.  Have to change this, either generate 
   *                                  floating point values 0 - 100 or use integers 1 - numElements.  
   *                                  1, first try returning a float and see what kind of pattern we get;  2. sort the luzs in ascending order to get the
   *                                  small changes out of the way 
   * --------------------------------------------------------------------------
   */
    void dcEmployment()
    {
      Boolean[] usez = new Boolean[NUM_LUZS];
      int i, j, k;
      int ret,zTempSum,luzIncrementSum;
    
      int[] zTemp = new int[NUM_EMP_SECTORS];
      int[] passer = new int[NUM_EMP_SECTORS];
      int[] zpasser = new int[NUM_LUZS];
      int[] zbase = new int[NUM_LUZS];
      int[] zsite = new int[NUM_LUZS];
      int[] control = new int[NUM_EMP_SECTORS];
      int[] tsum = new int[NUM_EMP_SECTORS];
      int zTempSum1 = 0, adjSum = 0;
      int[] saveZumAdj = new int[NUM_LUZS];
      int luz_marginal,luz_marginal_old,loopcount = 0;
      //int zii = 0;
      string str,str1;
      StreamWriter nx = null;
      StreamWriter ns = null;
      //-----------------------------------------------------------------------------

      writeToStatusBox( "Computing LUZ employment sectors.." );
      try
      {
        nx = new StreamWriter(new FileStream(networkPath + string.Format(appSettings["nx"].Value), FileMode.Create));
       
        nx.AutoFlush = true;
      }
      catch( IOException e )
      {
        MessageBox.Show( e.ToString(), "IOException" );
        Close();
      }
      try
      {
        ns = new StreamWriter(new FileStream(networkPath +  string.Format(appSettings["ns"].Value), FileMode.Create));
      
        ns.AutoFlush = true;
      }
      catch (IOException e)
      {
        MessageBox.Show(e.ToString(), "IOException");
        Close();
      }     

      //*************************************************************************************************************************
      // Control the employment distribution
      // 11/23/09 - we're trying a new way to do the luz emp distributions because of the sr13 issues with negative employment
      // the original UDM doc explains that if we have negative regional controls, and we do, that we do a proportional decrement 
      // across the luzs by sector, adjusting each z[i].fcst.ei.adj accordingly, so that there are no negatives in the pachinko.
      // trying that first;  our fallback is to do a 2-way update on the luz empXsector distibution and factor everything down to
      // the new regional controls - that's a more meaningful coding exercise, although we probably have that code somewhere in 
      // this udm project.

      // some additional constraints 
      // there are luzs that have no emp change in the first increment, no sitespec
      // there are also luzs with no employment and no emp change got to exclude these from neg adjustments

      //--------------------------------------------------------------------------------------------------------------------------
      // check the adjusted regional controls for job loss and deal with those first
      double efactor;
      int esectorbase, esectorfcst;
      for (j = 0; j < NUM_LUZS; ++j)
      {
        saveZumAdj[j] = z[j].fcst.ei.adj;
      } // end for j

      for (i = 0; i < NUM_LUZS; ++i)
      {
        usez[i] = true;
        if (z[i].fcst.e.civ == z[i].baseData.e.civ)  // exclude any luzs with no change from any adjustments
          usez[i] = false;
      }  // end for i
       
      for (j = 0; j < NUM_EMP_SECTORS; ++j)
      {
        esectorbase = rc.baseData.e.sectors[j];
        esectorfcst = rc.fcst.e.sectors[j];

        if (rc.fcst.ei.sectorsAdj[j] < 0)       // work with negative adjustments (adjusted for sitespec)
        {
          for (i = 0; i < NUM_LUZS; ++i)
          {
            if (!usez[i])
            {
              esectorbase -= z[i].baseData.e.sectors[j];
              esectorfcst -= z[i].baseData.e.sectors[j];
            }  // end if
          } // end for i
          efactor = (double)esectorfcst / (double)esectorbase;
          for (i = 0; i < NUM_LUZS; ++i)
          {
            zbase[i] = 0;
            zsite[i] = 0;
            zpasser[i] = 0;
            if (usez[i])
            {
              zpasser[i] = (int)((double)z[i].baseData.e.sectors[j] * efactor);
              zbase[i] = z[i].baseData.e.sectors[j];
            }  // end if
            
            zsite[i] = z[i].site.sectors[j];
            
          }  // end for
          ret = UDMUtils.roundItNeg(zpasser, zbase, esectorfcst,NUM_LUZS);
          for (i = 0; i < NUM_LUZS; ++i)
          {
            //if (i == 18)  // temporary debug stop
            //  zii = 0;
            if (usez[i])
            {
              z[i].fcst.ei.sectorsAdj[j] = zpasser[i] - zbase[i] - zsite[i];
              z[i].fcst.ei.adj -= z[i].fcst.ei.sectorsAdj[j];
              z[i].fcst.ei.sectors[j] = zpasser[i] - zbase[i];
            }   // end if
          }  // end for

        } // end if
      }   // end for j

      // this is the end of the adjustments for sector job loss
      //-------------------------------------------------------------------------------------------------------------------------

      for (j = 0; j < NUM_LUZS; ++j)
      {
         saveZumAdj[j] = z[j].fcst.ei.adj;
      } // end for j
      
      zTempSum = 0;
      for (j = 0; j < NUM_EMP_SECTORS; ++j)
      {
        if (rc.fcst.ei.sectorsAdj[j] > 0)
        {
          zTemp[j] = rc.fcst.ei.sectorsAdj[j];
          zTempSum += zTemp[j];
        }   // end if
      }   // end for j
      luzIncrementSum = 0;

      for (i = 0; i < NUM_LUZS; ++i)
        luzIncrementSum += z[i].fcst.ei.adj;
      str = "";

      if (luzIncrementSum != zTempSum)
        MessageBox.Show("Employment Increments don't match luzIncrement = " + luzIncrementSum + "; regIncrement = " + zTempSum);

      for (j = 0; j < NUM_EMP_SECTORS; ++j)
        str = str + zTemp[j] + ",";
       
        //nx.WriteLine(str);
        
      for (i = 0; i < NUM_LUZS; ++i)
      {
        for (j = 0; j < NUM_EMP_SECTORS; ++j)
          passer[j] = 0;

        //if (i == 54) // this is a temporary debug stop
        //  zii = 0;
        luz_marginal_old = luz_marginal = z[i].fcst.ei.adj;
        if (luz_marginal >= 0)
          continue;
        //otherwise we have a negative luz marginal
        usez[i] = false;
        // do all negative marginals first
        writeToStatusBox("processing luz number " + (i+1) + " luz id = " + (i+1).ToString() + " in Sector controlling");
        // cycle through the col marginals and add 1 at a time and adjust luz marginal and passer
        loopcount = 0;
        while (luz_marginal < 0 && loopcount < 10000)
        {
          for (j = 0; j < NUM_EMP_SECTORS; ++j)
          {
            if (zTemp[j] > 0 && z[i].baseData.e.sectors[j] > 0 && Math.Abs(passer[j]) < z[i].baseData.e.sectors[j])
            {
              ++zTemp[j];
              ++luz_marginal;
              --passer[j];
              if (luz_marginal == 0)
                break;
            }  // end if
          }   // end for
          ++loopcount;
        }  // end while
        z[i].fcst.ei.adj = 0;
        for (j = 0; j < NUM_EMP_SECTORS; ++j)
        {
          z[i].fcst.ei.sectorsAdj[j] += passer[j];
          z[i].fcst.ei.adj += z[i].fcst.ei.sectorsAdj[j];
        }  // end for j
        zTempSum1 = 0;
        adjSum = 0;
        for (j = 0; j < NUM_EMP_SECTORS; ++j)
          zTempSum1 += zTemp[j];
        for (j = 0; j < NUM_LUZS; ++j)
        {
          if (usez[j])
            adjSum += z[j].fcst.ei.adj;
        }  // end for j
      }   // end for i

      for (i = 0; i < NUM_LUZS; ++i)
      {
          if (i == 140)
              j = 0;
          writeToStatusBox("Controlling EMP Sectors LUZ = " + (i+1));
        luz_marginal_old = luz_marginal = z[i].fcst.ei.adj;
        if (!usez[i])
          continue;
        usez[i] = false;
        if (luz_marginal == 0)
          continue;

        for (j = 0; j < NUM_EMP_SECTORS; ++j)
        {
          control[j] = z[i].baseData.e.sectors[j]; 
          passer[j] = 0;
        }   // end for j
        ret = UDMUtils.specialPachinkoEmp(passer, control, zTemp, ref luz_marginal, NUM_EMP_SECTORS);
        if (ret != 0 && ret != 9999)
          MessageBox.Show("specialPachinkoEmp didn't solve for emp sector distribution for LUZ " + (i + 1));
        else if (ret == 9999)
          MessageBox.Show("specialPachinkoEmp has an empty control array");

        str = i.ToString() + " , ";
        for (j = 0; j < NUM_EMP_SECTORS; ++j)
        {
          z[i].fcst.ei.sectorsAdj[j] += passer[j];
          str = str + passer[j] + ",";
        }   // end for j


        zTempSum1 = 0;
        adjSum = 0; 
        for (j = 0; j < NUM_EMP_SECTORS; ++j)
          zTempSum1 += zTemp[j];
        for (j = 0; j < NUM_LUZS; ++j)
        {
          if (usez[j])
            adjSum += z[j].fcst.ei.adj;
        }
        str = str + luz_marginal_old;
        //nx.WriteLine(str);
      
        str = str+ i + ",";
        for (j = 0; j < NUM_EMP_SECTORS; ++j)
          str = str + zTemp[j] + ",";

        nx.WriteLine(str);

      }   // end for i

      
      //Sector overrides are entered self - controlling (we hope), therefore Sector employment overrides
      if( dcESOvr )
        applyEmpOvr();
      
      // Compute total change, levels and % chg
      for( i = 0; i < NUM_LUZS; i++ )
      {
        str = (i+1) + ",";
        str1 = (i + 1) + ",";
        // save the luz sector data for examination
        for (j = 0; j < NUM_EMP_SECTORS - 1; ++j)
          str = str + z[i].fcst.ei.sectorsAdj[j] + ",";
        str = str + z[i].fcst.ei.sectorsAdj[NUM_EMP_SECTORS-1];
        nx.WriteLine(str);
  
        z[i].fcst.e.adj = 0;      // Reinitialize total
        z[i].baseData.e.civ = 0;
        z[i].fcst.ei.civ = 0;
        z[i].fcst.e.civ = 0;
        for( k = 0; k < NUM_EMP_SECTORS; k++ )
        {
          // Compute sector increment from adjusted and siteSpec
          z[i].fcst.ei.sectors[k] = z[i].fcst.ei.sectorsAdj[k] + z[i].site.sectors[k];
          // Levels
          z[i].baseData.e.civ += z[i].baseData.e.sectors[k];
          z[i].fcst.e.sectors[k] = z[i].baseData.e.sectors[k] + z[i].fcst.ei.sectors[k];
          z[i].fcst.ei.civ += z[i].fcst.ei.sectors[k];
          z[i].fcst.e.civ += z[i].fcst.e.sectors[k];

          // Pct change
          if( z[i].baseData.e.sectors[k] > 0 )
            z[i].fcst.pct.sectors[k] = ( double )z[i].fcst.ei.sectors[k] /( double )z[i].baseData.e.sectors[k] * 100;
        }     // End for k

          // Pct change total civ */
        if( z[i].baseData.e.civ > 0 )
          z[i].fcst.pct.civ = ( double )z[i].fcst.ei.civ /( double )z[i].baseData.e.civ * 100;

        for (j = 0; j < NUM_EMP_SECTORS - 1; ++j)
          str1 = str1 + z[i].fcst.e.sectors[j] + ",";
        str1 = str1 + z[i].fcst.e.sectors[NUM_EMP_SECTORS - 1];
        ns.WriteLine(str1);
      }     // End for i
      nx.Close();
      ns.Close();
    }     // End method dcEmployment()

  /*****************************************************************************/

  /* method dcIncome() */
  /// <summary>
  /// Method to calculate income distribution.
  /// </summary>
  
  /* Revision History
   * 
   * STR             Date       By    Description
   * --------------------------------------------------------------------------
   *                 12/10/97   tb    Initial coding
   *                 08/25/03   df    C# revision
   * --------------------------------------------------------------------------
   */
    void dcIncome()
    {
      bool negFlag;
      int it, i, j,temphh,maxbin,maxval,diff;
      int[] luzHHTotal = new int[NUM_LUZS];
      int[,] luzInc= new int[NUM_LUZS,NUM_HH_INCOME];
      int[] rowTotal = new int[NUM_LUZS];
      int[] colTotal = new int[NUM_HH_INCOME];
      double temp;
     
      //writeToStatusBox( "Computing LUZ income distribution.." );
      // Compute regional income distribution
      it = 0;
      for( j = 0; j < NUM_HH_INCOME; j++ )
        it += rc.fcst.i.hh[j];

      for( j = 0; j < NUM_HH_INCOME; j++ )
      {
        if( it > 0 )
          rc.fcst.i.dist[j] = ( double )rc.fcst.i.hh[j] / ( double )it;
      }   // end for j

      // LUZs
      for( i = 0; i < NUM_LUZS; i++ )
      {
          if (i == 37)
              i = 37;
        /* compute initial luz distribition */
        it = 0;
        for( j = 0; j < NUM_HH_INCOME; j++ )
          it += z[i].baseData.i.hh[j];

        for( j = 0; j < NUM_HH_INCOME; j++ )
        {
          if( it > 0 )
            z[i].baseData.i.dist[j] = ( double )z[i].baseData.i.hh[j] /( double )it;
          else
            z[i].baseData.i.dist[j] = rc.fcst.i.dist[j];
        }

          // Update fcst median if not overridden
        if( z[i].ro.medianIncome == 0 )
          z[i].fcst.i.median = ( int )( ( double )z[i].baseData.i.median *(( double )rc.fcst.i.median /( double )rc.baseData.i.median ));
        
        // Determine income method
        switch( z[i].incomeSwitch )
        {
          case 1:     // Regional distribution
            for( j = 0; j < NUM_HH_INCOME; j++ )
            {
              z[i].fcst.i.dist[j] = rc.fcst.i.dist[j];
              z[i].fcst.i.hh[j] = ( int )( 0.5 +( double )z[i].fcst.hh.total * z[i].fcst.i.dist[j] );            
            }   // end for j
            break;

          case 2:     // Base year distribution
            //adjust the base distributions by the adjustment factors and normalize
            temp = 0;
            for( j = 0; j < NUM_HH_INCOME; j++ )
            {
              z[i].fcst.i.dist[j] = z[i].baseData.i.dist[j] * income_distribution_adjustments[j];
              temp += z[i].fcst.i.dist[j];
            }   // end for j
            for (j = 0; j < NUM_HH_INCOME; ++j)
            {
              if (temp > 0)
                z[i].fcst.i.dist[j] /= temp;
              else
                z[i].fcst.i.dist[j] = 0;
              z[i].fcst.i.hh[j] = ( int )( 0.5 + ( double )z[i].fcst.hh.total * z[i].fcst.i.dist[j] );
            }   // end for j
            break;

          case 3:      // (Default) log normal curve
            doIncomeDist( z[i].fcst.nla, z[i].fcst.asd, z[i].fcst.i.median, z[i].fcst.i.dist );
              // Add base year adjustments to distributions if not overridden
            if( !z[i].iOvr )
            {
              for( j = 0; j < NUM_HH_INCOME; j++ )
              {
                  // Add adjustment to log normal
                if( z[i].fcst.i.dist[j] + z[i].fcst.incomeAdj[j] > 0 )
                  z[i].fcst.i.dist[j] += z[i].fcst.incomeAdj[j];
              }   // end for j
            }   // end if
              // Compute HH from distribution
            for( j = 0; j < NUM_HH_INCOME; j++ )
              z[i].fcst.i.hh[j] = ( int )( 0.5 + (double )z[i].fcst.hh.total * z[i].fcst.i.dist[j] );
            break;
        }     // End switch
        
        negFlag = false;
          // Check distributions for negative values
        for( j = 0; j < NUM_HH_INCOME; j++ )
        {
          if( z[i].fcst.i.dist[j] < 0 )
          {
            negFlag = true;
            break;
          }   // end if
        }   // end for j

        if( negFlag )
        {
          MessageBox.Show( "FATAL ERROR - dcIncome computed negative " + "distributions for LUZ " + ( i + 1 ) );
        }

        // Store LUZ HH distribution in temp array for normalization
        for( j = 0; j < NUM_HH_INCOME; j++ )
        {
          luzInc[i,j] = z[i].fcst.i.hh[j];
        }   // end for j
        // Store LUZ HH total in temp array for normalization
        luzHHTotal[i] = z[i].fcst.hh.total;
       
      }     // End for i
      // control the luz forecasts only to row totals
      // add the forecast distribution for each luz, find the largest bin and put the residual there
      // this obviates the defm regional controls which cause too much redistribution

      for( i = 0; i < NUM_LUZS; i++ )
      {
        temphh = 0;
        maxbin = 0;
        maxval = 0;
        for( j = 0; j < NUM_HH_INCOME; j++ )
        {
          temphh += z[i].fcst.i.hh[j];
          if (z[i].fcst.i.hh[j] > maxval)
            maxbin = j;
        }   // end for j
        if (temphh != luzHHTotal[i])
        {
          diff = luzHHTotal[i] - temphh;
          z[i].fcst.i.hh[maxbin] += diff;
          temphh += diff;
        }   // end if
      }   //end for i

      // Compute new medians, increments and % changes
      for( i = 0; i < NUM_LUZS; i++ )
      {
        // Restore the temp data to the HH array
        for( j = 0; j < NUM_HH_INCOME; j++ )
                 
        // Median income
        z[i].fcst.i.median = UDMUtils.medianIncome( z[i].fcst.i.hh );

        // Increment
        for( j = 0; j < NUM_HH_INCOME; j++ )
        {
          z[i].fcst.ii.hh[j] = z[i].fcst.i.hh[j] - z[i].baseData.i.hh[j];
          if( z[i].baseData.i.hh[j] > 0 )
            z[i].fcst.pct.incomeHH[j] = ( ( double )z[i].fcst.ii.hh[j] /( double )z[i].baseData.i.hh[j] ) * 100;
        }   // end for j
        z[i].fcst.ii.median = z[i].fcst.i.median - z[i].baseData.i.median;
        if( z[i].baseData.i.median > 0 )
          z[i].fcst.pct.medianIncome = ( double )z[i].fcst.ii.median /( double )z[i].baseData.i.median;
      }     // End for i
     
    }     // End method dcIncome()

  /*****************************************************************************/

  /* method dcPrint() */
  /// <summary>
  /// Method to control DC printing.
  /// </summary>
  
  /* Revision History
   * 
   * STR             Date       By    Description
   * --------------------------------------------------------------------------
   *                 12/23/97   tb    Initial coding
   *                 08/27/03   df    C# revision
   * --------------------------------------------------------------------------
   */
    void dcPrint()
    {
      writeToStatusBox("Printing DC data");
      printDCEmp();
      printDCHHS();
      printDCIncome();
      printDCOvr();
      printDCPop();
      printDCRates();
    }     // End method dcPrint()

  /*****************************************************************************/

  /* method doIncomeDist() */
  /// <summary>
  /// Method to compute income distribution from log normal curve.
  /// </summary>
  
  /* Revision History
   * 
   * STR             Date       By    Description
   * --------------------------------------------------------------------------
   *                 12/10/97   tb    Initial coding
   *                 08/25/03   df    C# revision
   * --------------------------------------------------------------------------
   */
    void doIncomeDist( double nla, double asd, int median, double[] dist )
    {
      int i;
      int[] bounds = {15000,30000,45000,60000,75000,100000,125000,150000,200000,350000};
      double logMed;      // Log median
      double[] logIncome = new double[NUM_HH_INCOME];
      
      double[] adj = new double[NUM_HH_INCOME];
      double[,] p = new double[2,NUM_HH_INCOME];
      double[] zEst = new double[NUM_HH_INCOME];      // Estimated z values
      double[] z1 = new double[NUM_HH_INCOME];        /* Difference between bound and
                                             * median */
      double sqrt2;     // Square root of 2

      // Get log of income bounds
      for(i = 0; i < NUM_HH_INCOME; i++ )
        logIncome[i] = Math.Log( ( double )bounds[i] );

      sqrt2 = Math.Sqrt( 2 );
      
        // Log median
      if( median > 0 )
        logMed = Math.Log( median );
      else
        logMed = 1;

      Array.Clear(p, 0, p.Length);
      Array.Clear(adj, 0, adj.Length);
      Array.Clear(zEst, 0, zEst.Length);
      Array.Clear(z1, 0, z1.Length);

      for(i = 0; i < NUM_HH_INCOME - 1; i++ )
      {
        z1[i] = logIncome[i] - logMed;
        adj[i] = asd * ( Math.Pow( logIncome[i], nla ) );
        zEst[i] = z1[i] * adj[i];
        p[0,i] = ( 1 + UDMUtils.errorFunction( ( double )zEst[i] / sqrt2 ) ) /2;
        if( i > 0 )
          p[1,i] = p[0,i] - p[0,i - 1];
      }  // end for i

      p[1,0] = p[0,0];
      p[1,NUM_HH_INCOME - 1] = 1 - p[0,NUM_HH_INCOME - 2];

      for(i = 0; i < NUM_HH_INCOME; i++ )
        dist[i] = p[1,i];
    }     // End method doIncomeDist()
    #endregion  

    #region Data Extraction Methods
    //***********************************
    //Includes procedures
    //  extractEmpLU()
    //  extractDcEmpOvr()
    //  extractDCEROvr()
    //  extractHHSOvr()
    //  extractIncOvr()
    //  extractMGRABase()
    //  extractRegionalControls()
    //  extractTransactions()
    //  extractVacRates()
  /*****************************************************************************/

  /* method extractEmpLU() */
  /// <summary>
  /// Method to extract employment land use distribution.
  /// </summary>
  
  /* Revision History
  * 
  * STR             Date       By    Description
  * --------------------------------------------------------------------------
  *                 12/04/97   tb    Initial coding
  *                 08/20/03   df    C# revision
  * --------------------------------------------------------------------------
  */
  void extractEmpLU()
  {
    SqlDataReader rdr;
    
    int lu, i, j;      
    double[] ed = new double[NUM_EMP_SECTORS + 1];
    //-----------------------------------------------------------------------

    writeToStatusBox( "Extracting exployment distribution by land use.." );
    sqlCommand.CommandText = String.Format(appSettings["selectSimple"].Value, TN.empDistByLU);
    numLUCats = 0;
    try
    {
      sqlConnection.Open();
      rdr = sqlCommand.ExecuteReader();
      while( rdr.Read() )
      {
        lu = rdr.GetInt16( 0 );
        for( i = 0; i < ed.Length; i++ )
          ed[i] = rdr.GetDouble( i + 1 );
        elu[numLUCats] = new EmpLU();
        elu[numLUCats].plu = lu;
        for( j = 0; j < NUM_EMP_SECTORS + 1; ++j)
          elu[numLUCats].dist[j] = ed[j];
        ++numLUCats;
      }   // end while
      rdr.Close();
    }     // End try

    catch( Exception e )
    {
      MessageBox.Show( e.ToString(), e.GetType().ToString() );
      Close();
    }
    finally
    {
      sqlConnection.Close();
    } 
  }     /* end procedure extractEmpLU()*/

  /*****************************************************************************/

  /* method extractDCEmpOvr() */
  /// <summary>
  /// Method to extract employment sector overrides from SQL database table.
  /// </summary>
  
  /* Revision History
  * 
  * STR             Date       By    Description
  * --------------------------------------------------------------------------
  *                 12/01/97   tb    Initial coding
  *                 08/20/03   df    C# revision
  * --------------------------------------------------------------------------
  */
  void extractEmpOvr()
  {
    SqlDataReader rdr;
    int[] sec = new int[NUM_EMP_SECTORS];
    int increment, zi, sc;

    // LUZ employment sector override data structure is ESO.
    writeToStatusBox( "Extracting LUZ employment sector overrides.." );
    sqlCommand.CommandText = String.Format(appSettings["selectAll"].Value, TN.luzEmpSectorOvr, scenarioID, bYear);
    
    try
    {
      sqlConnection.Open();
      rdr = sqlCommand.ExecuteReader();
      while( rdr.Read() )
      {
        sc = rdr.GetByte( 0 );
        increment = rdr.GetInt16( 1 );
        luzB = rdr.GetInt16( 2 );
        for( int i = 0; i < NUM_EMP_SECTORS; i++ )
          sec[i] = rdr.GetInt32( i + 3 );
        zi = luzB -1;
        z[zi].esOvr = true;     // Mark this LUZ as having sector overrides
          
          // Perform consistency checks here
        for( int i = 0; i < NUM_EMP_SECTORS; i++ )
          z[zi].eso.sectors[i] = sec[i];
        flags.esOvr = true;     // Mark overrides exist
      }   // end while
      rdr.Close();
    }     // End try
    catch( Exception e )
    {
      MessageBox.Show( e.ToString(), "Runtime Exception" );
      Close();
    }
    finally
    {
      sqlConnection.Close();
    } 
  
  }     // End method extractEmpOvr()

  /*****************************************************************************/

  /* method extractDCEROvr() */
  /// <summary>
  /// Method to extract erHH overrides from SQL database table.
  /// </summary>
  
  /* Revision History
   * 
   * STR             Date       By    Description
   * --------------------------------------------------------------------------
   *                 12/01/97   tb    Initial coding
   *                 08/20/03   df    C# revision
   * --------------------------------------------------------------------------
  */
  void extractEROvr()
  {
   
        SqlDataReader rdr;
        int zi,sc,inc;
        writeToStatusBox( "Extracting LUZ ER/HH rates overrides.." );
        sqlCommand.CommandText = String.Format(appSettings["selectAll"].Value, TN.luzEROvr, scenarioID, bYear);
     
        try
        {
            sqlConnection.Open();
            rdr = sqlCommand.ExecuteReader();
            while( rdr.Read() )
            {
                sc = rdr.GetByte(0);
                inc = rdr.GetInt32(1);
                luzB = rdr.GetInt16( 2 );
                rat[0] = rdr.GetDouble( 1 );
                zi = luzB - 1;
                z[zi].erOvr = true;     // Mark this LUZ as having ER overrides
                // Perform consistency checks here
                // Constrain erHH
                if( rat[0] > 0 && rat[0] < 0.5 )
                    rat[0] = 0.5;
                else if (rat[0] > 5)
                    rat[0] = 5;
                z[zi].ro.erHH = rat[0];
            }   // end while
            rdr.Close();
        }     // End try

        catch( Exception e )
        {
            MessageBox.Show( e.ToString(), "Runtime Exception" );
        }
        finally
        {
            sqlConnection.Close();
        } 
    
  }     // End method extractDCEROvr()

  /*****************************************************************************/

  /* method extractHHSOvr() */
  /// <summary>
  /// Method to extract any HHS overrides from SQL database table.
  /// </summary>
  
  /* Revision History
   * 
   * STR             Date       By    Description
   * --------------------------------------------------------------------------
   *                 08/20/98   tb    Initial coding
   *                 08/20/03   df    C# revision
   * --------------------------------------------------------------------------
   */
    void extractHHSOvr()
    {
     
      SqlDataReader rdr;
      int zi,sc,inc;
      writeToStatusBox( "Extracting LUZ HHS overrides.." );
      sqlCommand.CommandText = String.Format(appSettings["selectAll"].Value, TN.luzHHSOvr, scenarioID, bYear);
      
      try
      {
        sqlConnection.Open();
        rdr = sqlCommand.ExecuteReader();
        while( rdr.Read() )
        {
          sc = rdr.GetByte(0);
          inc = rdr.GetInt32(1);
          luzB = rdr.GetInt16( 2 );
          rat[0] = rdr.GetDouble( 3 );
          zi = luzB - 1;
          z[zi].hhsOvr = true;      // Mark this LUZ as having rates overrides
          
          // Perform consistency checks here
          // Constrain hhs
          if( rat[0] > 0 && rat[0] < 1.5 )
            rat[0] = 1.5;
          else if( rat[0] > 4.5 )
            rat[0] = 4.5;
          z[zi].ro.hhs = rat[0];
        }   // end while
        rdr.Close();
      }     // End try
      
      catch( Exception e )
      {
        MessageBox.Show( e.ToString(), "Runtime Exception" );
        Close();
      }
      finally
      {
        sqlConnection.Close();
      } 

  }     /* end procedure extracHHSOvr()*/

  /*****************************************************************************/

  /* method extractIncOvr() */
  /// <summary>
  /// Method to extract any income overrides from SQL database table.
  /// </summary>
  
  /* Revision History
   * 
   * STR             Date       By    Description
   * --------------------------------------------------------------------------
   *                 12/01/97   tb    Initial coding
   *                 08/20/03   df    C# revision
   * --------------------------------------------------------------------------
   */
    void extractIncomeOvr()
    {
     
      SqlDataReader rdr;
      int zi, sec,sc,inc;

      // LUZ employment sector override data structure is eso.
      writeToStatusBox( "Extracting LUZ income parameter overrides.." );
      sqlCommand.CommandText = String.Format(appSettings["selectAll"].Value, TN.luzIncOvr, scenarioID, bYear);
      
      try
      {
        sqlConnection.Open();
        rdr = sqlCommand.ExecuteReader();
        while( rdr.Read() )
        {
          sc = rdr.GetByte(0);
          inc = rdr.GetInt32(1);
          luzB = rdr.GetInt16( 2 );
          rat[0] = rdr.GetInt32( 3 );
          rat[1] = rdr.GetDouble( 4 );
          rat[2] = rdr.GetDouble( 5 );
          sec = rdr.GetByte( 6 );
          zi = luzB - 1;
          z[zi].ro.medianIncome = ( int )rat[0];
          z[zi].ro.asd = rat[1];
          z[zi].ro.nla = rat[2];
          z[zi].ro.incomeSwitch = sec;
          z[zi].iOvr = true;
        }   // end while
        rdr.Close();
      }     // End try
      
      catch( Exception e )
      {
        MessageBox.Show( e.ToString(), "Runtime Exception" );
        Close();
      }   // end catch
      finally
      {
        sqlConnection.Close();
      } 
     
    }     // End method extractIncOvr()

  /*****************************************************************************/

  /* method extractMGRABase() */
  /// <summary>
  /// Method to extract MGRABase data from SQL database table.
  /// </summary>
  
  /* Revision History
   * 
   * STR             Date       By    Description
   * --------------------------------------------------------------------------
   *                 05/22/97   tb    Initial coding
   *                 08/20/03   df    C# revision
   * --------------------------------------------------------------------------
   */
    void extractMGRABase()
    {
      SqlDataReader rdr;

      int mgra, iter;
      int[] zbi = new int[71];
      double[] sbi = new double[51];
      int [] detHH = new int[18];
      int counter, si;
      //-----------------------------------------------------------------------

      counter = 0;
      iter = 0;
      writeToStatusBox( "Extracting mgraBase data.." );
      sqlCommand.CommandText = String.Format(appSettings["selectAll"].Value, TN.mgrabase, scenarioID, bYear);
      
      try
      {
        sqlConnection.Open();
        rdr = sqlCommand.ExecuteReader();
        while( rdr.Read() )
        {
            iter = 2;
            // skip 0 and 1 - scenario and incrment
          mgra = rdr.GetInt32( iter++ );
          if (mgra == 14744)
              mgra = 14744;
          luzB = rdr.GetInt16( iter++ );
          for( int i =0; i <= 52; i++ )
            zbi[i] = rdr.GetInt32( iter++ );   // socioeconomic stuff
          for( int i = 0; i < 51; i++ )
            sbi[i] = rdr.GetDouble(iter++ );
                    
          si = mgra - 1;
          storeMB( si, luzB, zbi, sbi );
          counter++;
          if( counter % 10000 == 0 )
            writeToStatusBox( "processed " + counter + " records from mgraBase" );
         
        }   // end while 
        rdr.Close();
      }   // end try
      catch( Exception e )
      {
        MessageBox.Show( e.ToString(), "Runtime Exception" );
        Close();
      }   // end catch
      finally
      {
        sqlConnection.Close();
      } 
      // get lost units from employment run
      
    }     // End method extractMGRABase()

  /*****************************************************************************/

  /* method extractRegionalControls() */
  /// <summary>
  /// Method to extract detailed characteristics regional controls.
  /// </summary>
  
  /* Revision History
   * 
   * STR             Date       By    Description
   * --------------------------------------------------------------------------
   *                 12/05/97   tb    Initial coding
   *                 08/20/03   df    C# revision
   * --------------------------------------------------------------------------
   */
    void extractRegionalControls()
    {
     
      SqlDataReader rdr;
      int yr,i,sc;
      int[] rcn = new int[68];
      rc = new RegionalControls();
      //-----------------------------------------------------------------------
      
      // Site spec struct is site.
      // Housing levels struct is hs.
      // Housing increment struct is hsi.
      // Housing levels struct is hh.
      // Housing increment struct is hhi.
      
      income_distribution_adjustments = new double[10];
      writeToStatusBox( "Extracting income distribution adjustments" );
      sqlCommand.CommandText = String.Format(appSettings["selectSimple"].Value, TN.incomeDistAdj);
     
      try
      {
        sqlConnection.Open();
        rdr = sqlCommand.ExecuteReader();
        while( rdr.Read() )
        {
          
          for(i = 0; i < NUM_HH_INCOME; i++ )
          {
            income_distribution_adjustments[i] = new double();
            income_distribution_adjustments[i] = rdr.GetDouble(i);
          }   // end for i    
        }   // end while 
        rdr.Close();

      }   // end try
      catch( Exception e )
      {
        MessageBox.Show( e.ToString(), e.GetType().ToString() );
        Close();
      }   // end catch
      finally
      {
        sqlConnection.Close();
      } 

      writeToStatusBox( "Extracting regional controls.." );
      sqlCommand.CommandText = String.Format(appSettings["select17"].Value, TN.regfcst, scenarioID, bYear,fYear);
      
      try
      {
        sqlConnection.Open();
        rdr = sqlCommand.ExecuteReader();
        while( rdr.Read() )
        {
          sc = rdr.GetByte(0);
          yr = rdr.GetInt16( 1 );
          for(i = 0; i < rcn.Length; i++ )
            rcn[i] = rdr.GetInt32( i + 2 );
          if( yr == bYear )
            storeRC( rcn, 1 );
          else
            storeRC( rcn, 2 );
         
        }   // end while 
        rdr.Close();

      }   // end try
      catch( Exception e )
      {
        MessageBox.Show( e.ToString(), e.GetType().ToString() );
        Close();
      }   // end catch
      finally
      {
        sqlConnection.Close();
      } 

      // Build regional controls from LUZ ss data
      for(i = 0; i < NUM_LUZS; i++ )
      {
        rc.site.gqCiv += z[i].site.gqCiv;     // Civ siteSpec
        rc.site.gqMil += z[i].site.gqMil;     // Mil siteSpec
      }   // end for i
      
      // rc.site.gqCiv and gqMil are computed in extractLUZTemp()
      rc.fcst.pi.gqCiv = rc.fcst.p.gqCiv - rc.baseData.p.gqCiv;
      rc.fcst.pi.gqMil = rc.fcst.p.gqMil - rc.baseData.p.gqMil;
      rc.fcst.pi.gqCivAdj = rc.fcst.pi.gqCiv - rc.site.gqCiv;
      rc.fcst.pi.gqMilAdj = rc.fcst.pi.gqMil - rc.site.gqMil;
           
      // Region control for employment
      rc.fcst.ei.civ = rc.fcst.e.civ - rc.baseData.e.civ;
      rc.fcst.ei.adj = rc.fcst.ei.civ - reg.site.civ;     /* Adjusted for sitespec */
      
      for(i = 0; i < NUM_EMP_SECTORS; i++ )
      {
        rc.fcst.ei.sectors[i] = rc.fcst.e.sectors[i] - rc.baseData.e.sectors[i];

      }   //end for i

      // Income distribution
      for(i = 0; i < 8; i++ )
        rc.fcst.ii.hh[i] = rc.fcst.i.hh[i] - rc.baseData.i.hh[i];

      // HHP
      rc.fcst.pi.hhp = rc.fcst.p.hhp - rc.baseData.p.hhp;

      // Group quarters
      rc.fcst.p.gq = rc.fcst.p.gqCiv + rc.fcst.p.gqMil;
      rc.baseData.p.gq = rc.baseData.p.gqCiv + rc.baseData.p.gqMil;
      rc.fcst.pi.gq = rc.fcst.p.gq - rc.baseData.p.gq;

      // Pop
      rc.fcst.p.pop = rc.fcst.p.gq + rc.fcst.p.hhp;
      rc.baseData.p.pop = rc.baseData.p.gq + rc.baseData.p.hhp;
      rc.fcst.pi.pop = rc.fcst.p.pop - rc.baseData.p.pop;

      // HH
      rc.fcst.hh.total = rc.fcst.hh.sf + rc.fcst.hh.mf + rc.fcst.hh.mh;
      rc.baseData.hh.total = rc.baseData.hh.sf + rc.baseData.hh.mf + rc.baseData.hh.mh;
      rc.fcst.hhi.total = rc.fcst.hh.total - rc.baseData.hh.total;

      // HS
      rc.fcst.hs.total = rc.fcst.hs.sf + rc.fcst.hs.mf + rc.fcst.hs.mh;
      rc.baseData.hs.total = rc.baseData.hs.sf + rc.baseData.hs.mf + rc.baseData.hs.mh;
      rc.fcst.hsi.total = rc.fcst.hs.total - rc.baseData.hs.total;

      // Vacancy rate
      rc.fcst.r.v = ( 1.0 - ( double )rc.fcst.hh.total /( double )rc.fcst.hs.total ) * 100;

      // Pct change
      // HHP
      if( rc.baseData.p.hhp > 0 )
        rc.fcst.pct.hhp = ( double )rc.fcst.pi.hhp /( double )rc.baseData.p.hhp * 100;

      // HH
      if( rc.baseData.hh.total > 0 )
        rc.fcst.pct.hh = ( double )rc.fcst.hhi.total /( double )rc.baseData.hh.total * 100;

      // GQ
      if( rc.baseData.p.gq > 0 )
        rc.fcst.pct.gq = ( double )rc.fcst.pi.gq /( double )rc.baseData.p.gq * 100;

      // Pop
      if( rc.baseData.p.pop > 0 )
        rc.fcst.pct.pop = ( double )rc.fcst.pi.pop /( double )rc.baseData.p.pop * 100;

      // Emp sectors
      for(i = 0; i < NUM_EMP_SECTORS; i++ )
      {
        if( rc.baseData.e.sectors[i] > 0 )
          rc.fcst.pct.sectors[i] = ( double )rc.fcst.ei.sectors[i] /( double )rc.baseData.e.sectors[i] * 100;
      }   // end for i

      // Income
      for (i = 0; i < 8; i++ )
      {
        if( rc.baseData.i.hh[i] > 0 )
          rc.fcst.pct.incomeHH[i] = ( double )rc.fcst.ii.hh[i] /( double )rc.baseData.i.hh[i] * 100;
      }   // end for i

      if( rc.baseData.i.median > 0 )
        rc.fcst.pct.medianIncome = ( double )rc.fcst.ii.median /( double )rc.baseData.i.median * 100;
    }     // End method extractRegionalControls()

  /*****************************************************************************/

  /* method extractTransactions() */
  /// <summary>
  /// Method to extract capacity data for transactions processing.
  /// </summary>
  
  /* Revision History
   * 
   * STR             Date       By    Description
   * --------------------------------------------------------------------------
   *                 12/04/97   tb    Initial coding
   *                 08/20/03   df    C# revision
   * --------------------------------------------------------------------------
   */
    void extractTransactions()
    {
      int j;
      SqlDataReader rdr;
      Transactions tr = new Transactions();
      int recordsCount = 0, zi, mi;
      StreamWriter tt = null, ts = null;
      //-----------------------------------------------------------------------

      try
      {
        tt = new StreamWriter(new FileStream(networkPath + String.Format(appSettings["tt"].Value),FileMode.Create ) );
        ts = new StreamWriter(new FileStream(networkPath + String.Format(appSettings["ts"].Value), FileMode.Create));
        
        ts.AutoFlush = tt.AutoFlush = true;
      }
      catch( IOException e )
      {
        MessageBox.Show( "Error opening file.  " + e.ToString(), e.GetType().ToString() );
        Close();
      }
      writeToStatusBox( "Extracting capacity transactions data.." );
      sqlCommand.CommandText = String.Format(appSettings["select02"].Value, TN.capacity4, scenarioID, bYear);
      
      try
      {
        sqlConnection.Open();
        rdr = sqlCommand.ExecuteReader();
        while( rdr.Read() )
        {
          phase = rdr.GetInt16( 0 );
          tr.mgra = rdr.GetInt16( 1 );
          tr.luz = rdr.GetInt16( 2 );
          tr.site = rdr.GetInt16( 3 );
          tr.LCKey = rdr.GetInt32( 4 );
          tr.lu = rdr.GetInt16( 5 );
          tr.plu = rdr.GetInt16( 6 );
          tr.gqCiv = rdr.GetInt32( 7 );
          tr.gqMil = rdr.GetInt32( 8 );
          tr.chgempCiv = rdr.GetInt32( 9 );
          tr.sf = rdr.GetInt32( 10 );
          tr.mf = rdr.GetInt32( 11 );
          tr.mh = rdr.GetInt32( 12 );
          tr.devCode = rdr.GetByte( 13 );
          tr.pCap_emp = rdr.GetDouble( 14 );
          tr.pCap_hs = rdr.GetDouble(15);
          tr.acres = rdr.GetDouble( 16 );
          tr.civ = rdr.GetInt32(17);
          tr.mil = rdr.GetInt32(18);
          tr.emp = tr.civ + tr.mil;
          if( recordsCount++ % 100000 == 0 )
            writeToStatusBox( "   Processing record " + (recordsCount-1)  + " from capacity 4.." );
          mi = tr.mgra - 1;
          zi = tr.luz - 1;

          //rebuild the fcst.p.gqCiv with the updated gq numbers  then we won't do any gq controlling later on
          mbData[mi].fcst.p.gqCiv += tr.gqCiv;
          mbData[mi].fcst.p.gqMil += tr.gqMil;

          z[zi].fcst.p.gqCiv += tr.gqCiv;
          z[zi].fcst.p.gqMil += tr.gqMil;
          z[zi].fcst.e.civ += tr.civ;
          z[zi].fcst.e.mil += tr.mil;
          z[zi].fcst.e.total += tr.civ + tr.mil;
          if( phase < fYear )
          {
            // MGRA and LUZ indexes
           
            // Store the housing stock
            mbData[mi].fcst.hsi.sf += tr.sf;
            mbData[mi].fcst.hsi.mf += tr.mf;
            mbData[mi].fcst.hsi.mh += tr.mh;  

            z[zi].fcst.hsi.sf += tr.sf;
            z[zi].fcst.hsi.mf += tr.mf;
            z[zi].fcst.hsi.mh += tr.mh;   // ditto on stock for luzs


              // For siteSpec records store the gqs and employment
            if( tr.site > 0 )
              processSiteSpecTransactions( tr, mi, zi, ts );
            else      // Store the employment
            {
              mbData[mi].fcst.ei.adj += tr.chgempCiv;
              z[zi].fcst.ei.adj += tr.chgempCiv;
            }   // end else
          }     // End if

            // Process the acreage
          if (tr.devCode == 3 && mi == 0)
            mi = 0;
          buildMGRAAcres( mi, tr.lu, tr.devCode, tr.plu, tr.acres, tr.pCap_emp,tr.pCap_hs,tr.emp );
         
        }     // End while 
        rdr.Close();

        //compute the adjusted regional sectors from site spec sums

        for (j = 0; j < NUM_EMP_SECTORS; ++j)
          rc.fcst.ei.sectorsAdj[j] = rc.fcst.ei.sectors[j] - reg.site.sectors[j];
      }     // End try

      catch( Exception e )
      {
        MessageBox.Show( e.ToString(), e.GetType().ToString() );
        Close();
      }   // end catch
      finally
      {
        sqlConnection.Close();
      } 

      //tt.Close();
      ts.Close();


        // Compute acreage totals
      for( recordsCount = 0; recordsCount < NUM_MGRAS; recordsCount++ )
      {
        // dev
        for( j = 0; j <= 14; j++ )
          mbData[recordsCount].fcst.acres[48] += mbData[recordsCount].fcst.acres[j];
     
        // Vacant - indexes 15 - 27
        for( j = 15; j <= 27; j++ )
          mbData[recordsCount].fcst.acres[49] += mbData[recordsCount].fcst.acres[j];
       
        // Total - dev + vac + unusable
        mbData[recordsCount].fcst.acres[47] = mbData[recordsCount].fcst.acres[48] +  mbData[recordsCount].fcst.acres[49] + mbData[recordsCount].fcst.acres[50];
      }   // end for
     
    }     // End method extractTransactions()

  /*****************************************************************************/

  /* method extractVacRates() */
  /// <summary>
  /// Method to extract vacancy rate overrides from SQL Database table.
  /// </summary>
  
  /* Revision History
   * 
   * STR             Date       By    Description
   * --------------------------------------------------------------------------
   *                 08/20/98   tb    Initial coding
   *                 08/20/03   df    C# revision
   * --------------------------------------------------------------------------
   */
    void extractVacOvr()
    {
     
      SqlDataReader rdr;
      int zi,sc,inc;
      writeToStatusBox( "Extracting LUZ vacancy rates overrides.." );
      sqlCommand.CommandText = String.Format(appSettings["selectAll"].Value,TN.luzVacOvr,scenarioID,bYear);
     
      try
      {
        sqlConnection.Open();
        rdr = sqlCommand.ExecuteReader();
        while( rdr.Read() )
        {
          sc = rdr.GetByte(0);
          inc = rdr.GetInt32(1);
          luzB = rdr.GetInt16( 2 );
          rat[0] = rdr.GetDouble( 3 );
          rat[1] = rdr.GetDouble( 4 );
          rat[2] = rdr.GetDouble( 5 );
          zi = luzB - 1;
          z[zi].vacOvr = true;      // Mark this LUZ as having rates overrides

          // Perform consistency checks here
          // Constrain vacancy rates - SF
          if( rat[0] < 0 )
            rat[0] = 0;
          else if( rat[0] > 50.00 )
            rat[0] = 50.00;
          
          // Constrain vacancy rates - MF
          if( rat[1] < 0 )
            rat[1] = 0;
          else if( rat[1] > 50 )
            rat[1] = 50;
          
          // Constrain vacancy rates - mh
          if( rat[2] < 0 )
            rat[2] = 0;
          else if( rat[2] > 50 )
            rat[2] = 50;
          z[zi].ro.vSF = rat[0];
          z[zi].ro.vMF = rat[1];
          z[zi].ro.vmh = rat[2];
         
        }     // End while
        rdr.Close();
      }     // End try

      catch( Exception e )
      {
        MessageBox.Show( e.ToString(), "Runtime Exception" );
        Close();
      }
      finally
      {
        sqlConnection.Close();
      } 
     
    }     // End method extractVacOvr()

    #endregion Data Extraction Methods

    #region others
    //***************************
    //Includes procedures
    //  getDCOutliers()
    //  getLUEmpDist()
    //  getUpdateSum()
    //  IncomeFinish1()
    //  IncomeFinish2()
    //  loadMB()
    //  loadZB()
    //  processParms()
    //  processSiteSpecTransactions()
    //  storeMB()
    //  storeRC()

  /*****************************************************************************/

  /* method getDCOutliers() */
  /// <summary>
  /// Method to determine dc rates outliers.
  /// </summary>
  
  /* Revision History
   * 
   * STR             Date       By    Description
   * --------------------------------------------------------------------------
   *                 12/09/97   tb    Initial coding
   *                 08/25/03   df    C# revision
   * --------------------------------------------------------------------------
   */
    void getDCOutliers()
    { 
      int flagg;
      double t9HHS, t9ER, t10 = 0, t11 = 0;
      double t9SF, t9MF, t9mh;
      string label0 = "                                        Pct        Pct";
      string label1 = "LUZ Check Flag      Base      Fcst    Region       LUZ     Ratio";
      string label2 = "----------------------------------------------------------------";      
      string label4 = "----------------------------------------------------------------";      
      string[] check1 = {" HHS  ", " ER/HH"};
      string[] check2 = {" SFVAC", " MFVAC", " mhVAC"};
      writeToStatusBox( "Computing LUZ DC outliers.." );
      dcOutliers.WriteLine( "DETAILED CHARACTERISTICS OUTLIERS REPORT - HHS & ER/HH" );
      dcOutliers.WriteLine();
      dcOutliers.WriteLine( label0 );
      dcOutliers.WriteLine( label1 );
      dcOutliers.WriteLine( label2 );

      // HHS & ER
      // Get regional change
      if( reg.baseData.r.hhs > 0 )
        reg.fcst.pct.hhs = ( double )reg.fcst.r.hhs /( double )reg.baseData.r.hhs;
      if( reg.baseData.r.erHH > 0 )
        reg.fcst.pct.erHH = ( double )reg.fcst.r.erHH /( double )reg.baseData.r.erHH;       
      if( reg.baseData.r.hhs > 0 )
        t9HHS = reg.fcst.r.hhs / reg.baseData.r.hhs;
      else
        t9HHS = 0;
      if( reg.baseData.r.erHH > 0 )
        t9ER = reg.fcst.r.erHH / reg.baseData.r.erHH;
      else
        t9ER = 0;
      for( int i = 0; i < NUM_LUZS; i++ )
      {
          // HHS
        flagg = 0;
          // Skip LUZs with < 50 pop in base and fcst
        if( z[i].fcst.p.hhp > 50 && z[i].baseData.p.hhp > 50 )
        {
          t10 = 0;
          t11 = 0;
          if( z[i].baseData.r.hhs > 0 )
            t10 = z[i].fcst.r.hhs / z[i].baseData.r.hhs;
          if( t9HHS > 0 )
            t11 = t10 / t9HHS;
          if( ( t9HHS <= 1 && t10 > 1 ) || ( t9HHS >= 1 && t10 < 1 ) )
            flagg = 1;
          else if( t11 < .95 || t11 > 1.05 )
            flagg = 2;
        }  // end if
          // Flagg any LUZs with < 50 in base and > 50 in fcst
        else if( z[i].baseData.p.hhp < 50 && z[i].fcst.p.hhp > 50 )
          flagg = 3;
            
        if( flagg > 0 && flagg < 3 )
        {
          dcOutliers.WriteLine( "{0,3}{1}{2,5}{3,10:F2}{4,10:F2}{5,10:F2}{6,10:F2}{7,10:F2}", i + 1, check1[0],
                                flagg, z[i].baseData.r.hhs, z[i].fcst.r.hhs,t9HHS, t10, t11 );
          flags.dcOut = true;
        }   // end if
        else if( flagg == 3 )
        {
          dcOutliers.WriteLine( "{0,3}{1}{2,5}{3,10}{4,10}", i + 1, check1[0],flagg, z[i].baseData.p.hhp, z[i].fcst.p.hhp );
          flags.dcOut = true;
        }  // end else if

          // ER/HH
        flagg = 0;
          // Skip LUZs with < 50 ER in base and fcst
        if( z[i].fcst.p.er > 50 && z[i].baseData.p.er > 50 )
        {
          t10 = 0;
          t11 = 0;
          if( z[i].baseData.r.erHH > 0 )
            t10 = z[i].fcst.r.erHH / z[i].baseData.r.erHH;
          if( t9ER > 0 )
            t11 = t10 / t9ER;
          if( ( t9ER <= 1 && t10 > 1) || ( t9ER >= 1 && t10 < 1 ) )
            flagg = 1;
          else if( t11 < 0.95 || t11 > 1.05 )
            flagg = 2;
        }  // end if

          // < 50 in base and > 50 in fcst
        else if( z[i].baseData.p.er < 50 && z[i].fcst.p.er > 50 )
          flagg = 3;

        if( flagg > 0 && flagg < 3 )
          dcOutliers.WriteLine( "{0,3}{1}{2,5}{3,10:F2}{4,10:F2}{5,10:F2}{6,10:F2}{7,10:F2}", i + 1, check1[1], flagg,
                                z[i].baseData.r.erHH, z[i].fcst.r.erHH, t9ER,t10, t11 );
        else if( flagg == 3 )
        {
          dcOutliers.WriteLine( "{0,3}{1}{2,5}{3,10}{4,10}", i + 1, check1[0],flagg, z[i].baseData.p.er, z[i].fcst.p.er );
          flags.dcOut = true;
        }  // end else if
      }     // End for i

      dcOutliers.WriteLine();
      dcOutliers.WriteLine( label2 );
      dcOutliers.WriteLine();
      dcOutliers.WriteLine( "1 - base and fcst hhp > 50; ratio < 0" );
      dcOutliers.WriteLine( "2 - base and fcst hhp > 50; ratio < .95 or > " + "1.05" );
      dcOutliers.WriteLine( "3 - base hhp < 50; fcst hhp > 50" );
      dcOutliers.WriteLine();
      dcOutliers.WriteLine();
      dcOutliers.WriteLine( "DETAILED CHARACTERISTICS OUTLIERS REPORT - " + "VACANCY RATES" );
      dcOutliers.WriteLine();
      dcOutliers.WriteLine( label1 );
      dcOutliers.WriteLine( label2 );

        // Get regional change - SF
      if( reg.baseData.r.vSF > 0 )
        reg.fcst.pct.vSF = ( double )reg.fcst.r.vSF /( double )reg.baseData.r.vSF;
       
        // Get regional change  - MF
      if( reg.baseData.r.vMF > 0 )
        reg.fcst.pct.vMF = ( double )reg.fcst.r.vMF /( double )reg.baseData.r.vMF;

        // Get regional change - mh
      if( reg.baseData.r.vmh > 0 )
        reg.fcst.pct.vmh = ( double )reg.fcst.r.vmh /( double )reg.baseData.r.vmh;

      if( reg.baseData.r.vSF > 0 )
          t9SF = reg.fcst.r.vSF / reg.baseData.r.vSF;
      else
          t9SF = 0;
      if( reg.baseData.r.vMF > 0 )
        t9MF = reg.fcst.r.vMF / reg.baseData.r.vMF;
      else
        t9MF = 0;
      if( reg.baseData.r.vmh > 0 )
        t9mh = reg.fcst.r.vmh / reg.baseData.r.vmh;
      else
        t9mh = 0;

      for( int i = 0; i < NUM_LUZS; i++ )
      {
          // SF
        flagg = 0;
          // Skip LUZs with < 25 SF HS in base and fcst
        if( z[i].fcst.hs.sf > 25 && z[i].baseData.hs.sf > 25 )
        {
          t10 = 0;
          t11 = 0;
          if( z[i].baseData.r.vSF > 0 )
            t10 = z[i].fcst.r.vSF / z[i].baseData.r.vSF;
          if( t9SF > 0 )
            t11 = t10 / t9SF;
          if( ( t9SF <= 1 && t10 > 1 ) || ( t9SF >= 1 && t10 < 1 ) )
            flagg = 1;
          else if( t11 < .80 || t11 > 1.20 )
            flagg = 2;
        }
          // < 25 in base and > 25 in fcst
        else if( z[i].baseData.hs.sf < 25 && z[i].fcst.hs.sf > 25 )
          flagg = 3;
        if( flagg > 0 && flagg < 3 )
        {
          dcOutliers.WriteLine( "{0,3}{1}{2,5}{3,10:F2}{4,10:F2}{5,10:F2}{6,10:F2}{7,10:F2}", i + 1, check2[0],
                                flagg, z[i].baseData.r.vSF, z[i].fcst.r.vSF, t9SF, t10, t11 );
          flags.dcOut = true;
        }
        else if( flagg == 3 )
        {
          dcOutliers.WriteLine( "{0,3}{1}{2,5}{3,10}{4,10}", i + 1, check2[0],flagg, z[i].baseData.hs.sf, z[i].fcst.hs.sf );
          flags.dcOut = true;
        }

          // MF
        flagg = 0;
          // Skip luzs with < 25 MF HS in base and fcst
        if( z[i].fcst.hs.mf > 25 && z[i].baseData.hs.mf > 25 )
        {
          t10 = 0;
          t11 = 0;
          if( z[i].baseData.r.vMF > 0 )
            t10 = z[i].fcst.r.vMF / z[i].baseData.r.vMF;
          if( t9MF > 0 )
            t11 = t10 / t9MF;
          if( ( t9MF <= 1 && t10 > 1 ) || ( t9MF >= 1 && t10 < 1 ) )
            flagg = 1;
          else if( t11 < 0.80 || t11 > 1.20 )
            flagg = 2;
        }
          // < 25 in base and > 25 in fcst
        else if( z[i].baseData.hs.mf < 25 && z[i].fcst.hs.mf > 25 )
          flagg = 3;
        if( flagg > 0 && flagg < 3 )
        {
          dcOutliers.WriteLine( "{0,3}{1}{2,5}{3,10:F2}{4,10:F2}{5,10:F2}{6,10:F2}{7,10:F2}", i + 1, check2[1], flagg,
                                z[i].baseData.r.vMF, z[i].fcst.r.vMF, t9MF,t10, t11 );
          flags.dcOut = true;
        }
        else if( flagg == 3 )
        {
          dcOutliers.WriteLine( "{0,3}{1}{2,5}{3,10}{4,10}", i + 1, check2[1],flagg, z[i].baseData.hs.mf, z[i].fcst.hs.mf );
          flags.dcOut = true;
        }

          // mh
        flagg = 0;
          // Skip LUZs with < 25 mh HS in base and fcst
        if( z[i].fcst.hs.mh > 25 && z[i].baseData.hs.mh > 25 )
        {
          t10 = 0;
          t11 = 0;
          if( z[i].baseData.r.vmh > 0 )
            t10 = z[i].fcst.r.vmh / z[i].baseData.r.vmh;
          if( t9mh > 0 )
            t11 = t10 / t9mh;
          if( ( t9mh <= 1 && t10 > 1) || ( t9mh >= 1 && t10 < 1 ) )
            flagg = 1;
          else if( t11 < 0.80 || t11 > 1.20 )
            flagg = 2;
        }
          // < 25 in base and > 25 in fcst
        else if( z[i].baseData.hs.mh < 25 && z[i].fcst.hs.mh > 25 )
          flagg = 3;
        if( flagg > 0 && flagg < 3 )
        {
          dcOutliers.WriteLine( "{0,3}{1}{2,5}{3,10:F2}{4,10:F2}{5,10:F2}{6,10:F2}{7,10:F2}", i + 1, check2[2], flagg, z[i].baseData.r.vmh, z[i].fcst.r.vmh, t9mh,t10, t11 );
          flags.dcOut = true;
        }
        else if( flagg == 3 )
        {
          dcOutliers.WriteLine( "{0,3){1}{2,5}{3,10}{4,10}", i + 1, check2[2],flagg, z[i].baseData.hs.mh, z[i].fcst.hs.mh );
          flags.dcOut = true;
        }
      }     // End for i
      
      dcOutliers.WriteLine();
      dcOutliers.WriteLine( label4 );
      dcOutliers.WriteLine();
      dcOutliers.WriteLine( "1 - base and fcst hs > 25; ratio < 0" );
      dcOutliers.WriteLine( "2 - base and fcst hs > 25; ratio < .8 or > 1.2");
      dcOutliers.WriteLine( "3 - base hs < 25; fcst hs > 25" );
    }     // End method getDCOutliers()

  /*****************************************************************************/

  /* method getLUEmpDist() */
  /// <summary>
  /// Method to hit empLU lookup table and return distribution of employment
  /// (14 sectors)
  /// </summary>
  
  /* Revision History
   * 
   * STR             Date       By    Description
   * --------------------------------------------------------------------------
   *                 12/04/97   tb    Initial coding
   *                 08/20/03   df    C# revision
   * --------------------------------------------------------------------------
   */
    bool getEmpLUDist( int lu, double[] dist, ref int maxId )
    {
      int i,j;
      double maxVal = 0;
      for(i = 0; i < numLUCats; i++ )
      {
        if( lu == elu[i].plu )
        {
          for(j = 1; j < NUM_EMP_SECTORS; j++ )      // Skip ag land
          {
            dist[j] = elu[i].dist[j];
            if( dist[j] > maxVal )      // Find the largest % for use in maxId
            {
              maxId = j;
              maxVal = dist[j];
            }  // end if
          }  // end for j
          return true;
        }     // End if
      }     // End for i
      return false;
    }     // End method getEmpLUDist()

  /*****************************************************************************/

  /* method getUpdateSum() */
  /// <summary>
  /// Method to compute row and column sums and total.
  /// </summary>
  
  /* Revision History
   * 
   * STR             Date       By    Description
   * --------------------------------------------------------------------------
   *                 08/08/94   tb    Initial coding
   *                 08/20/03   df    C# revision
   * --------------------------------------------------------------------------
   */
    int getUpdateSum( int numRows, int numCols, int[,] matrix, int[] rowSum,
                      int[] colSum )
    {
      int total = 0;
      for( int i = 0; i < numRows; i++ )
        rowSum[i] = 0;
      for( int j = 0; j < numCols; j++ )
      {
        colSum[j] = 0;
        for( int i = 0; i < numRows; i++ )
        {
          rowSum[i] += matrix[i,j];
          colSum[j] += matrix[i,j];
          total += matrix[i,j];
        } // end for i
      }  // end for j
      return total;
    }     // End method getUpdateSum()

  /*****************************************************************************/

  /* method incomeFinish1() */
  /// <summary>
  /// Method to perform final rounding for income distribution estimates.  This
  /// method is for sets with row totals not matching controls.
  /// </summary>
  
  /* Revision History
   * 
   * STR             Date       By    Description
   * --------------------------------------------------------------------------
   *                 07/27/98   tb    Initial coding
   *                 08/25/03   df    C# revision
   * --------------------------------------------------------------------------
   */
    void incomeFinish1()
    {
      int rowTotal = 0, colTotal = 0;
      int[] colDiff = new int[NUM_HH_INCOME];
      int[] colSum = new int[NUM_HH_INCOME];
      int[] rowSum = new int[NUM_LUZS];
      int[] rowDiff = new int[NUM_LUZS];
      int i, j;
      writeToStatusBox( "LUZ income finish routine 1.." );
        // Compute differences in column (sector) sums and regional controls
      for( j = 0; j < NUM_HH_INCOME; j++ )
      {
        colSum[j] = 0;
        for( i = 0; i < NUM_LUZS; i++ )
          colSum[j] += z[i].fcst.i.hh[j];
        colDiff[j] = rc.fcst.i.hh[j] - colSum[j];
        colTotal += colDiff[j];
      }  // end for j
   
      for( i = 0; i < NUM_LUZS; i++ )
      {
        rowSum[i] = 0;
        for( j = 0; j < NUM_HH_INCOME; j++ )
          rowSum[i] += z[i].fcst.i.hh[j];
        rowDiff[i] = z[i].fcst.hh.total - rowSum[i];
        rowTotal += rowDiff[i];
      }  // end for i
      writeToStatusBox( "   rowTotal = " + rowTotal + " colTotal = " +  colTotal );

      // Now run through LUZs, get negatives (subtract to meet luz control)and make adjustment in first sector with neg regional control difference 
      for( i = 0; i < NUM_LUZS; i++ )
      {
          /* Find a LUZ with a negative rowDiff - this means the total is > LUZ control */
        if( rowDiff[i] >= 0 )
          continue;

          // Find a sector with a negative and make the adjustment
        for (j = 0; j < NUM_HH_INCOME; ++j)
          if( colDiff[j] < 0 )
          {
            z[i].fcst.i.hh[j]--;
            colDiff[j]++;
            rowDiff[i]++;
            break;
          }  // end if
      }  // end for j

      for( i = 0;i < NUM_LUZS; i++ )
      {
          /* Now, find a LUZ with a positive rowDiff.  This means the total is < LUZ control */
        if( rowDiff[i] <= 0 )
          continue;

          // Find a sector with a positive and make the adjustment
        for( j = 0; j < NUM_HH_INCOME; j++ )
        {
          if( colDiff[j] > 0 )
          {
            z[i].fcst.i.hh[j]++;
            colDiff[j]--;
            rowDiff[i]--;
            break;
          }  // end if
        }      // end for j       
      }     // End for i

        /* At this time, all the colDiffs should be 0, but there will be
         * offsetting + and - in the rowDiffs.  Go back through the remaining
         * LUZs and make all adjustments to self employed.  They should offset
         */
      for( i = 0; i < NUM_LUZS; i++ )
      {
        if( rowDiff[i] == 0 )
          continue;

        if( rowDiff[i] > 0 )
        {
            // Make final adjustments in middle group
          z[i].fcst.i.hh[5]+= rowDiff[i];
          colDiff[5] += rowDiff[i];
          rowDiff[i] = 0;
        }  // end if

        else if( rowDiff[i] < 0 )
        {
          z[i].fcst.i.hh[5] += rowDiff[i];
          colDiff[5] += rowDiff[i];
          rowDiff[i] = 0;
        }  // end else if
      }  // end for

        // Recompute sums and compare to totals
      rowTotal = colTotal = 0;
        
        // Compute differences in column (sector) sums and regional controls
      for( j = 0; j < NUM_HH_INCOME; j++ )
      {
        colSum[j] = 0;
        for( i = 0; i < NUM_LUZS; i++ )
          colSum[j] += z[i].fcst.i.hh[j];
        colDiff[j] = rc.fcst.i.hh[j] - colSum[j];
        colTotal += colDiff[j];
      }
       
      for( i = 0; i < NUM_LUZS; i++ )
      {
        rowSum[i] = 0;
        for( j = 0; j < NUM_HH_INCOME; j++ )
          rowSum[i] += z[i].fcst.i.hh[j];
        rowDiff[i] = z[i].fcst.hh.total - rowSum[i];
        rowTotal += rowDiff[i];
      }
      writeToStatusBox( "   rowTotal = " + rowTotal + " colTotal = "  + colTotal );
    }     // End method incomeFinish1()

  /*****************************************************************************/

  /* method incomeFinish2() */
  /// <summary>
  /// Method to perform final rounding for income distribution estimates.  This
  /// method is for sets with col totals not matching controls.  This is the
  /// normal case where all the row dists add up, but the regional column
  /// totals are slightly off from regional totals.
  /// </summary>
  
  /* Revision History
   * 
   * STR             Date       By    Description
   * --------------------------------------------------------------------------
   *                 07/27/98   tb    Initial coding
   *                 08/25/03   df    C# revision
   * --------------------------------------------------------------------------
   */
    void incomeFinish2()
    {
      int maxHH = 1, maxId = 99999;
      int[] colTotal = new int[NUM_HH_INCOME];
      int[] colDiff = new int[NUM_HH_INCOME];
      
      //writeToStatusBox( "LUZ income finish routine 2.." );
      
      // Find the LUZ with the most HH and do all adjustments there.
      for( int i = 0; i < NUM_LUZS; i++ )
      {
        if( z[i].fcst.hh.total > maxHH )
        {
          maxHH = z[i].fcst.hh.total;
          maxId = i;
        }  // end if
      }  // end for
      
      for( int j = 0; j < NUM_HH_INCOME; j++ )
      {
        for( int i = 0; i < NUM_LUZS; i++ )
          colTotal[j] += z[i].fcst.i.hh[j];
        colDiff[j] = rc.fcst.i.hh[j] - colTotal[j];
      }  // end for

      for( int j= 0; j < NUM_HH_INCOME; j++ )
      {
        if( colDiff[j] == 0 )
          continue;
        z[maxId].fcst.i.hh[j] += colDiff[j];
        colDiff[j] = 0;
      }  // end for
    }     // End method incomeFinish2()

  /*****************************************************************************/

  /* method loadMB() */
  /// <summary>
  /// Method to load mgraBase table to SQL Server.
  /// </summary>
  
  /* Revision History
   * 
   * STR             Date       By    Description
   * --------------------------------------------------------------------------
   *                 12/17/97   tb    Initial coding
   *                 09/05/03   df    C# revision
   * --------------------------------------------------------------------------
   */
    private void loadMB()
    {
      //we do two deletes and two writes here - the extra is for the mgrabase adjusted or prison population for popsyn controls

      writeToStatusBox( "Loading mgraBase table to SQL Server.." );
      String tf = "";
      tf = String.Format(networkPath + String.Format(appSettings["tempMB"].Value));

      // delete from mgrabase for this scenario and increment
      sqlCommand.CommandText = String.Format(appSettings["deleteFrom"].Value, TN.mgrabase, scenarioID,fYear);
      try
      {
          sqlConnection.Open();
          sqlCommand.ExecuteNonQuery();
      }
      catch (SqlException e)
      {
          MessageBox.Show(e.ToString(), "SqlException");
          Application.Exit();
      }
      finally
      {
          sqlConnection.Close();
      }

      // delete from mgrabase for this scenario and increment
      sqlCommand.CommandText = String.Format(appSettings["deleteFrom"].Value, TN.mgrabase_adjusted_for_prisons, scenarioID, fYear);
      try
      {
          sqlConnection.Open();
          sqlCommand.ExecuteNonQuery();
      }
      catch (SqlException e)
      {
          MessageBox.Show(e.ToString(), "SqlException");
          Application.Exit();
      }
      finally
      {
          sqlConnection.Close();
      } 


      // now two buld loads including the mgrabase adjusted for prisons

      // Load the mb table
      //writeToStatusBox( "   bulk loading mgraBase next table.." );
      sqlCommand.CommandText = String.Format(appSettings["bulkInsert"].Value, TN.mgrabase, tf);
      
      try
      {  
        sqlConnection.Open();
        sqlCommand.ExecuteNonQuery(); 
      }
      catch( SqlException e )
      {
        MessageBox.Show( e.ToString(), "SqlException" );
        Application.Exit();
      }
      finally
      {
        sqlConnection.Close();
      }

      //writeToStatusBox( "   bulk loading mgraBase next table.." );
      sqlCommand.CommandText = String.Format(appSettings["bulkInsert"].Value, TN.mgrabase_adjusted_for_prisons, tf);

      try
      {
          sqlConnection.Open();
          sqlCommand.ExecuteNonQuery();
      }
      catch (SqlException e)
      {
          MessageBox.Show(e.ToString(), "SqlException");
          Application.Exit();
      }
      finally
      {
          sqlConnection.Close();
      } 
     
      // now extract prison pop from capacity - gq_civ on lu = 1401
     // truncate prison temp table
      sqlCommand.CommandText = String.Format(appSettings["truncate"].Value, TN.prison_temp);

      try
      {
          sqlConnection.Open();
          sqlCommand.ExecuteNonQuery();
      }
      catch (SqlException e)
      {
          MessageBox.Show(e.ToString(), "SqlException");
          Application.Exit();
      }
      finally
      {
          sqlConnection.Close();
      }

      sqlCommand.CommandText = String.Format(appSettings["insertPrisonTemp"].Value, TN.prison_temp,TN.capacity,fYear);

      try
      {
          sqlConnection.Open();
          sqlCommand.ExecuteNonQuery();
      }
      catch (SqlException e)
      {
          MessageBox.Show(e.ToString(), "SqlException");
          Application.Exit();
      }
      finally
      {
          sqlConnection.Close();
      }

      sqlCommand.CommandText = String.Format(appSettings["updateForPrisons"].Value, TN.mgrabase_adjusted_for_prisons,TN.prison_temp,fYear);

      try
      {
          sqlConnection.Open();
          sqlCommand.ExecuteNonQuery();
      }
      catch (SqlException e)
      {
          MessageBox.Show(e.ToString(), "SqlException");
          Application.Exit();
      }
      finally
      {
          sqlConnection.Close();
      } 
     
    }     // End method loadMB()

  /*****************************************************************************/

  /* method loadZB() */
  /// <summary>
  /// Method to load aumbase table to SQL Server.
  /// </summary>
  
  /* Revision History
   * 
   * STR             Date       By    Description
   * --------------------------------------------------------------------------
   *                 12/17/97   tb    Initial coding
   *                 09/05/03   df    C# revision
   * --------------------------------------------------------------------------
   */
    private void loadZB()
    {
        writeToStatusBox("Loading LUZBase table to SQL Server..");
        String tf = "";
        tf = String.Format(networkPath + String.Format(appSettings["tempZB"].Value));
        // delete from luzbase for this scenario and increment
        sqlCommand.CommandText = String.Format(appSettings["deleteFrom"].Value, TN.luzbase, scenarioID, fYear);
        try
        {
            sqlConnection.Open();
            sqlCommand.ExecuteNonQuery();
        }
        catch (SqlException e)
        {
            MessageBox.Show(e.ToString(), "SqlException");
            Application.Exit();
        }
        finally
        {
            sqlConnection.Close();
        } 
        // Load the zb table
      
        sqlCommand.CommandText = String.Format(appSettings["bulkInsert"].Value, TN.luzbase, tf);
        try
        { 
            sqlConnection.Open();
            sqlCommand.ExecuteNonQuery(); 
        }
        catch( SqlException e )
        {
            MessageBox.Show( e.ToString(), "SqlException" );
        }
        finally
        {
            sqlConnection.Close();
        } 
     
    }     // End method loadZB()

    //******************************************************************************

    /* procedure loadLUZHistory() */

    /* load luz history table*/

    /* Revision History
         STR            Date        By   Description
         -------------------------------------------------------------------------
                        12/24/97    tb   initial coding
         -------------------------------------------------------------------------
    */
    /*---------------------------------------------------------------------------*/

    public void loadLUZHistory()
    {
    
        // Load the history table
        
        String tf = "";
        tf = String.Format(networkPath + String.Format(appSettings["tempZH"].Value));
        // delete from luzbase for this scenario and increment
        sqlCommand.CommandText = String.Format(appSettings["deleteFrom"].Value, TN.luzhist, scenarioID, fYear);
        try
        {
            sqlConnection.Open();
            sqlCommand.ExecuteNonQuery();
        }
        catch (SqlException e)
        {
            MessageBox.Show(e.ToString(), "SqlException");
            Application.Exit();
        }
        finally
        {
            sqlConnection.Close();
        } 

        sqlCommand.CommandText = String.Format(appSettings["bulkInsert"].Value, TN.luzhist, tf);
      
        try
        {
          sqlConnection.Open();
          sqlCommand.ExecuteNonQuery();
        }
        catch( SqlException e )
        {
          MessageBox.Show( e.ToString(), "SqlException" );
          Application.Exit();
        }
        finally
        {
          sqlConnection.Close();
        } 
  
  }     /* end procedure loadLUZHistory()*/

  /******************************************************************************/

    /* method processParams() */
    /// <summary>
    /// Method to process input parameters and build table names.
    /// </summary>

    /* Revision History
     * 
     * STR             Date       By    Description
     * --------------------------------------------------------------------------
     *                 12/01/97   tb    Initial coding
     *                 11/12/98   tb    Changes for final sr9 forecast
     *                 01/11/02   tb    Changes for Jan, 2002 sr10
     *                 08/19/03   df    C# revision
     * --------------------------------------------------------------------------
     */
    private bool processParams()
    {
      scenarioID = cboScenario.SelectedIndex;
      if( cboYears.SelectedIndex == -1)
      {
        MessageBox.Show( "You have selected an invalid range of years!  Please try again.", "Invalid Years" );
        return false;
      }  // end if
      if( scenarioID > 1) 
      {
        MessageBox.Show( "You have selected an invalid scenario!  Please try again.", "Invalid Scenario" );
        return false;
      }  // end if
      
      dcEROvr = chkEROvr.Checked;
      dcESOvr = chkESOvr.Checked;
      dcHHSOvr = chkHHSOvr.Checked;
      dcIncOvr = chkIncOvr.Checked;
      dcVacOvr = chkVacRatesOvr.Checked;
      bYear = incrementLabels[cboYears.SelectedIndex];
      fYear = incrementLabels[cboYears.SelectedIndex + 1];

      outputLabel = scenLabels[scenarioID] + " " + incrementLabels[cboYears.SelectedIndex] + " - " + incrementLabels[cboYears.SelectedIndex + 1] + " " +DateTime.Now;
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
          
          ZB_DEBUG = bool.Parse(appSettings["ZB_DEBUG"].Value);        // Write luzBase to file
          ZH_DEBUG = bool.Parse(appSettings["ZH_DEBUG"].Value);        // Write luzHistory to file
          ZT_DEBUG = bool.Parse(appSettings["ZT_DEBUG"].Value);        // Write luzTemp to file
         
          UPDATE_DEBUG = bool.Parse(appSettings["UPDATE_DEBUG"].Value);
          PM_DEBUG = bool.Parse(appSettings["PM_DEBUG"].Value);
          MEMP_DEBUG = bool.Parse(appSettings["MEMP_DEBUG"].Value); 

          CBDLUZ = int.Parse(appSettings["CBDLUZ"].Value);
          INCOMESWITCH = int.Parse(appSettings["defaultIncomeSwitch"].Value);
          MAX_MGRAS_IN_LUZ = int.Parse(appSettings["MAX_MGRAS_IN_LUZ"].Value);
          MAX_TRANS = int.Parse(appSettings["MAX_TRANS"].Value);
          NUM_EMP_LAND = int.Parse(appSettings["NUM_EMP_LAND"].Value);
          NUM_EMP_SECTORS = int.Parse(appSettings["NUM_EMP_SECTORS"].Value);
          NUM_HH_INCOME = int.Parse(appSettings["NUM_HH_INCOME"].Value);
          NUM_LUZS = int.Parse(appSettings["NUM_LUZS"].Value);
          NUM_MGRAS = int.Parse(appSettings["NUM_MGRAS"].Value);

          TN.capacity = String.Format(appSettings["capacity"].Value);
          TN.capacity3 = String.Format(appSettings["capacity3"].Value);
          TN.capacity4 = String.Format(appSettings["capacity4"].Value);
          TN.empDistByLU = String.Format(appSettings["empDistByLU"].Value);
          TN.homePrices = String.Format(appSettings["homePrices"].Value);
          TN.incomeDistAdj = String.Format(appSettings["incDistAdj"].Value);
          TN.luzbase = String.Format(appSettings["luzbase"].Value);
          TN.luzhist = String.Format(appSettings["luzhist"].Value);
          TN.luzIncomeParms = String.Format(appSettings["luzIncomeParms"].Value);
          TN.luztemp = String.Format(appSettings["luztemp"].Value);
          TN.regfcst = String.Format(appSettings["regfcst"].Value);
          TN.updateGQC = String.Format(appSettings["updateGQC"].Value);
          TN.mgrabase = String.Format(appSettings["mgrabase"].Value);
          TN.mgrabase_adjusted_for_prisons = String.Format(appSettings["mgrabase_adjusted_for_prisons"].Value);
          TN.prison_temp = String.Format(appSettings["prison_temp"].Value);
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

      flags = new Flags();
      elu = new EmpLU[100];
      rat = new double[10];
      mbData = new MBMaster[NUM_MGRAS];
      
      reg = new Master();
      zt = new Master[NUM_LUZS];
      z = new Master[NUM_LUZS];
      zbi = new int[70];
      return true;
    }     // End method processParams()

    /*****************************************************************************/

    /* method processSiteSpecTransactions() */
    /// <summary>
    /// Method to process site specific transactions for HS and emp.
    /// </summary>

    /* Revision History
     * 
     * STR             Date       By    Description
     * --------------------------------------------------------------------------
     *                 12/04/97   tb    Initial coding
     *                 08/21/03   df    C# revision
     * --------------------------------------------------------------------------
     */
    void processSiteSpecTransactions( Transactions tr, int mi, int zi,StreamWriter ts )
    {
      bool success;
      int j, ttt, ret;
      int maxId = 0, maxVal;
      int[] baseEmp = new int[NUM_EMP_SECTORS];
      int[] empTemp = new int[NUM_EMP_SECTORS];
      double[] dist = new double[NUM_EMP_SECTORS];

      mbData[mi].site.gqCiv += tr.gqCiv;
      mbData[mi].site.gqMil += tr.gqMil;
      mbData[mi].fcst.pi.gqCiv += tr.gqCiv;
      mbData[mi].fcst.pi.gqMil += tr.gqMil;

      mbData[mi].fcst.pi.gqCiv += tr.gqCiv;
      mbData[mi].fcst.pi.gqMil += tr.gqMil;

      if( tr.chgempCiv == 0 )
        return;

      // Get employment distribution for increments
      if( tr.chgempCiv > 0 )
      {
        success = getEmpLUDist( tr.plu, dist, ref maxId );
        if( success )     // If the plu was in the lookup table
          for( j = 0; j < NUM_EMP_SECTORS; j++ )      // Distribute the employment
            empTemp[j] = ( int )( 0.5 +  tr.chgempCiv * dist[j] );
        else
        {
          MessageBox.Show( "WARNING - NO MATCH IN EMPLOYMENT-LAND USE LOOKUP TABLE FOR " + tr.plu );
          //writeToStatusBox( "Applying base year distribution.." );
          for( j = 0; j < NUM_EMP_SECTORS; j++ )
            empTemp[j] = ( int )( 0.5 + tr.chgempCiv * mbData[mi].baseData.eDist[j] );
        }  // end else
        ttt = 0;
        for( j = 0; j < NUM_EMP_SECTORS; j++ )      // Get the sum skipping ag
          ttt += empTemp[j];

        //If the sum is 0, then we have a small tr.chgempCiv that doesn't get distributed to any cells.  Put all of it in the largest percentage.
        if( ttt == 0 )
          empTemp[maxId] = tr.chgempCiv;
    
      }     // End if tr.chgempCiv > 0
      
      else if( tr.chgempCiv < 0 )
      {
        ttt = 0;
      
        /* Start with cons - no ag sitespec for negative sitespec.  If we
          * are removing all jobs set the sector values equal to the base
          * year data */
        if( Math.Abs( tr.chgempCiv ) == mbData[mi].baseData.e.civ )
          for( j = 1; j < NUM_EMP_SECTORS; j++ )
            empTemp[j] = -mbData[mi].baseData.e.sectors[j];
        else      // We are removing less than total emp from base year data
        {
          maxId = maxVal = 0;
          for( j = 1; j < NUM_EMP_SECTORS; j++ )
          {
            /* Find the largest sector from base in case the distribution yields a 0 change that doesn't get allocated. */
            if( mbData[mi].baseData.e.sectors[j] > maxVal )
            {
              maxId = j;
              maxVal = mbData[mi].baseData.e.sectors[j];
            }  // end if
            empTemp[j] = ( int )( 0.5 + tr.chgempCiv *
              mbData[mi].baseData.eDist[j] );
            ttt += empTemp[j];
            baseEmp[j] = mbData[mi].baseData.e.sectors[j];
            // Constrain negatives to base year value
            if( Math.Abs( empTemp[j] ) > mbData[mi].baseData.e.sectors[j] )
              empTemp[j] = -mbData[mi].baseData.e.sectors[j];
          }     // End for j
          
          /* If the sum is 0 then we have a small tr.chgempCiv that doesn't get
            * distributed to any cells.  Put all of it in the largest percentage */
          if( ttt == 0 )
            empTemp[maxId] = tr.chgempCiv;
        }     // End else          
      }     // End else if tr.chgempCiv < 0
      ttt = 0;
      for( j = 0; j < NUM_EMP_SECTORS; j++ )      // Get the sum skipping ag
        ttt += empTemp[j];

      // Call pachinko for rounding if totals don't match
      if( ttt != tr.chgempCiv )
      {
        if( tr.chgempCiv > 0 )
          ret = UDMUtils.pachinko( this, tr.chgempCiv, empTemp, NUM_EMP_SECTORS);
        else
          ret = UDMUtils.pachinkoNeg( this, tr.chgempCiv, empTemp, baseEmp, NUM_EMP_SECTORS );
        if( ret == 0 )      /* did pachinko resolve differences */
          ts.Write( "{0,6}{1,4}{2,6}{3,6}", mi + 1, zi + 1, tr.LCKey, tr.plu );
        else
          MessageBox.Show( "processSiteSpec pachinko didn't converge mgra = " + tr.mgra + " LCKey = " + tr.LCKey );
      }  // end if

      // Distribute employment to sectors
      for( j = 0; j < NUM_EMP_SECTORS; j++ )
      {
        ts.Write( "{0,6}", empTemp[j] );
        mbData[mi].site.sectors[j] += empTemp[j];
        z[zi].site.sectors[j] += empTemp[j];
        reg.site.sectors[j] += empTemp[j];
      }  // end for
      ts.WriteLine();
      mbData[mi].site.civ += tr.chgempCiv;
    }     // End method processSiteSpecTransactions()*/

    /*****************************************************************************/

    /* method storeMB() */
    /// <summary>
    /// Method to store mgraBase query results into mbData array.
    /// </summary>

    /* Revision History
     * 
     * STR             Date       By    Description
     * --------------------------------------------------------------------------
     *                 12/03/97   tb    Initial coding
     *                 08/21/03   df    C# revision
     * --------------------------------------------------------------------------
     */
    void storeMB( int i, int luzm, int[] zbi, double[] sbi)
    {
        int j, iter = 0; ;
      mbData[i] = new MBMaster();
      mbData[i].luz = luzm;
      mbData[i].baseData.p.pop = zbi[iter++];
      mbData[i].baseData.p.hhp = zbi[iter++];
      mbData[i].baseData.p.er = zbi[iter++];
      mbData[i].baseData.p.gq = zbi[iter++];
      mbData[i].baseData.p.gqCiv = zbi[iter++];
      mbData[i].baseData.p.gqCivCol = zbi[iter++];
      mbData[i].baseData.p.gqCivOth = zbi[iter++];
      mbData[i].baseData.p.gqMil = zbi[iter++];

      mbData[i].baseData.hs.total = zbi[iter++];
      mbData[i].baseData.hs.sf = zbi[iter++];
      mbData[i].baseData.hs.mf = zbi[iter++];
      mbData[i].baseData.hs.mh = zbi[iter++];
     
      mbData[i].baseData.hh.total = zbi[iter++];
      mbData[i].baseData.hh.sf = zbi[iter++];
      mbData[i].baseData.hh.mf = zbi[iter++];
      mbData[i].baseData.hh.mh = zbi[iter++];

      // Employment categories
      mbData[i].baseData.e.total = zbi[iter++];
      mbData[i].baseData.e.civ = zbi[iter++];
      mbData[i].baseData.e.mil = zbi[iter++];
      
      // Employment by sector
      for(j = 0; j < NUM_EMP_SECTORS; j++ )
      {
          mbData[i].baseData.e.sectors[j] = zbi[iter++];
        // Compute base year employment distribution by sector.
        if( mbData[i].baseData.e.civ > 0 )
          mbData[i].baseData.eDist[j] = ( double )mbData[i].baseData.e.sectors[j] /( double )mbData[i].baseData.e.civ;
        else
          mbData[i].baseData.eDist[j] = 0;
      }  // end for
      for (j = 0; j < 4; ++j)
          mbData[i].baseData.e.empLand[j] = zbi[iter++];
      
      for(j = 0; j < 10; j++ )
          mbData[i].baseData.i.hh[j] = zbi[iter++];

      for(j = 0; j <= 50; j++ )
          mbData[i].baseData.acres[j] = sbi[j];
      iter = 0;
      
      
    }     // End method storeMB()

    /*****************************************************************************/

    /* method storeRC() */
    /// <summary>
    /// Method to store regional controls for detailed characteristics.
    /// </summary>
    /// <param name="rcn">Array of regional controls</param>
    /// <param name="sw">Which year; 1 = base year, 2 = forecast year</param>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 12/05/97   tb    Initial coding
    *                 08/22/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    void storeRC(int[] rcn, int sw)
    {
      int i,iter = 0;
      switch( sw )      // Base year or forecast year switch
      {
        case 1:     // Base year
          // Emp civ and mil emp
          rc.baseData.e.civ = rcn[iter++];
          rc.baseData.e.mil = rcn[iter++];
          // Employment sectors
          for(i = 0; i < NUM_EMP_SECTORS; i++ )
            rc.baseData.e.sectors[i] = rcn[iter++];
          rc.baseData.hs.total = rcn[iter++];
          rc.baseData.hs.sf = rcn[iter++];
          rc.baseData.hs.mf = rcn[iter++];
          rc.baseData.hs.mh = rcn[iter++];

          rc.baseData.hh.total = rcn[iter++];
          rc.baseData.hh.sf = rcn[iter++];
          rc.baseData.hh.mf = rcn[iter++];
          rc.baseData.hh.mh = rcn[iter++];
          rc.baseData.p.pop = rcn[iter++];
          rc.baseData.p.hhp = rcn[iter++];

          rc.baseData.p.gq = rcn[iter++];
          rc.baseData.p.gqCiv = rcn[iter++];
          rc.baseData.p.gqCivCol = rcn[iter++];
          rc.baseData.p.gqCivOth = rcn[iter++];
          rc.baseData.p.gqMil = rcn[iter++];
          rc.baseData.p.er = rcn[iter++];
          ++iter;          // Skip mean income ()
          rc.baseData.i.median = rcn[iter++];

          // Income distribution
          for(i = 0; i < 10; i++ )
              rc.baseData.i.hh[i] = rcn[iter++];
          
          break;
    
        case 2:     // Forecast year
          
          // Emp civ and emp mil
          rc.fcst.e.civ = rcn[iter++];
          rc.fcst.e.mil = rcn[iter++];
          // Employment sectors
          for(i = 0; i < NUM_EMP_SECTORS; i++ )
              rc.fcst.e.sectors[i] = rcn[iter++];
          rc.fcst.hs.total = rcn[iter++];
          rc.fcst.hs.sf = rcn[iter++];
          rc.fcst.hs.mf = rcn[iter++];
          rc.fcst.hs.mh = rcn[iter++];
          
          rc.fcst.hh.total = rcn[iter++];
          rc.fcst.hh.sf = rcn[iter++];
          rc.fcst.hh.mf = rcn[iter++];
          rc.fcst.hh.mh = rcn[iter++];
          rc.fcst.p.pop = rcn[iter++];
          rc.fcst.p.hhp = rcn[iter++];

          rc.fcst.p.gq = rcn[iter++];
          rc.fcst.p.gqCiv = rcn[iter++];
          rc.fcst.p.gqCivCol = rcn[iter++];
          rc.fcst.p.gqCivOth = rcn[iter++];
          rc.fcst.p.gqMil = rcn[iter++];
          rc.fcst.p.er = rcn[iter++];
          ++iter;     // Skip mean income
          rc.fcst.i.median = rcn[iter++];

          // Income distribution
          for(i = 0; i < 10; i++ )
              rc.fcst.i.hh[i] = rcn[iter++];
                    
          break;
      }     // End switch
    }     // End method storeRC()
    #endregion

    #region MGRAMAIN

  /*****************************************************************************/

  /* method mgraMain() */
  /// <summary>
  /// Method to perform perform MGRA detailed characteristics.
  /// </summary>
  
  /* Revision History
   * 
   * STR             Date       By    Description
   * --------------------------------------------------------------------------
   *                 12/15/97   tb    Initial coding
   *                 09/03/03   df    C# revision
   * --------------------------------------------------------------------------
   */
    private void mgraMain()
    {
        int i,gtot = 0;
        double gqco = 0;
        int[] passer = new int[NUM_MGRAS];
        int[] gqciv = new int[NUM_MGRAS];

        writeToStatusBox("MGRA MAIN");
        DCUtils.mgraCalcHH( this , mbData, z );
        DCUtils.mgraCalcHHP( this , mbData, z );

        // get value for gq_civ_other

        for (i = 0; i < NUM_MGRAS; i++ )
        {
            if (mbData[i].baseData.p.gqCiv > 0)
                gqco = (double)mbData[i].baseData.p.gqCivOth / (double)mbData[i].baseData.p.gqCiv;
            else
                gqco = 0;
            mbData[i].fcst.p.gqCivOth = (int)(gqco * mbData[i].fcst.p.gqCiv + 0.5);
            if (mbData[i].fcst.p.gqCivOth > mbData[i].fcst.p.gqCiv)
                mbData[i].fcst.p.gqCivOth = mbData[i].fcst.p.gqCiv;
            passer[i] = mbData[i].fcst.p.gqCivOth;
            gtot += passer[i];
            gqciv[i] = mbData[i].fcst.p.gqCiv;
        }  // end for
        
        // control the gqCivOther
        if (gtot != rc.fcst.p.gqCivOth)
            UDMUtils.roundIt(passer, gqciv, rc.fcst.p.gqCivOth, NUM_MGRAS, 1);
        // recover the controlled gq_civ_other
        gtot = 0;
        // do gq and pop toals
        for (i = 0; i < NUM_MGRAS; ++i)
        {
            gtot += passer[i];
            mbData[i].fcst.p.gqCivOth = passer[i];
            // derive gq_civ_college
            mbData[i].fcst.p.gqCivCol = mbData[i].fcst.p.gqCiv - mbData[i].fcst.p.gqCivOth;

            mbData[i].fcst.p.gq = mbData[i].fcst.p.gqCiv + mbData[i].fcst.p.gqMil;
            mbData[i].fcst.p.pop = mbData[i].fcst.p.hhp + mbData[i].fcst.p.gq;
        }  // end for
      
        DCUtils.mgraCalcER( this , mbData, z );
        DCUtils.mgraCalcIncome( this , mbData, z );
        DCUtils.mgraCalcCiv( this , mbData, z );
        DCUtils.mgraCalcSectors( this , mbData, z );
        DCUtils.writeMB( this , mbData, zt, z , scenarioID, fYear);
        DCUtils.writeZB( this , z, scenarioID, fYear);
        loadMB();
        loadZB();
        UDMUtils.copyCapacity( this , 5 , scenarioID, bYear,fYear);
    }     // End method mgraMain()
    #endregion
 

    #region update
    /*****************************************************************************/

  /* method update() */
  /// <summary>
  /// Method to adjust a positively-valued matrix to specified row and column
  /// totals.
  /// </summary>
  /// <param name="matrix">Incoming data array</param>
  /// <param name="nuColt">Individual column control total</param>
  /// <param name="numCols">Number of rows</param>
  /// <param name="numRows">Number of rows</param>
  /// <param name="nuRowt">Individual row control total</param>

  /* Revision History
   * 
   * STR             Date       By    Description
   * --------------------------------------------------------------------------
   *                 08/08/94   tb    Initial coding
   *                 08/16/94   tb    Replaced stand alone with callable proc
   *                 12/09/97   tb    Copied from est_inc
   *                 08/22/03   df    C# revision
   * --------------------------------------------------------------------------
   */
    bool update( int numRows, int numCols, int[,] matrix, int[] nuRowt,int[] nuColt )
    {
      bool continueLoop = true;
      int colTotal;     // Grand total of control rows and cols
      int i, j, ii, jj;
      int cTot, total, rt, ct, loopCount = 0;

      // Incoming individual row and column totals
      int[] rowSum = new int[NUM_LUZS], colSum = new int[NUM_HH_INCOME];
      int[] cSum = new int[NUM_HH_INCOME];
      
      // Check for negative cells
      for( i = 0; i < numRows; i++ )
        for( j = 0; j < numCols; j++ )
          if( matrix[i,j] < 0 )
            return false;

      // Check sum of new control row and cols and redistribute if necessary
      colTotal = checkGrandTotal( numRows, numCols, matrix, nuRowt, nuColt );
      checkBaseTotal( numRows, numCols, matrix, nuRowt, nuColt, colTotal );
      total = getUpdateSum( numRows, numCols, matrix, rowSum, colSum );
      countMatches( numRows, numCols, rowSum, colSum, nuRowt, nuColt );

      while( continueLoop && loopCount < 100 )
      {
        // Factor matrix to control totals by iteration
        // Initialize summation arrays
        for( j = 0; j < numCols; j++ )
        {
          colSum[j] = 0;
          cSum[j] = 0;
          for( i = 0; i < numRows; i++ )
          {
            if( rowSum[i] == nuRowt[i])
              cSum[j] += matrix[i,j];
            colSum[j] += matrix[i,j];
          }
        }
        countMatches( numRows, numCols, rowSum, colSum, nuRowt, nuColt );

        //if( UPDATE_DEBUG )
        //{
          //writeToStatusBox( "stop 1 iteration = " + loopCount + " row " +
           //                 "matches = " + rowSame + " col matches = " +
            //                colSame );
          //printColSum( numRows, numCols, rowSum, colSum, nuRowt, nuColt );
        //}

        for( j =0; j < numCols; j++ )
        {
          total = 0;
          ct = 0;
          ii = 0;
          cTot = colSum[j] - cSum[j];
          if( cTot > 0 )
          {
            for( i = 0; i < numRows; i++ )
            {
              if( rowSum[i] == nuRowt[i] )
                continue;
              if( cTot > 0 )
	              matrix[i,j] = ( int )( 0.5 + ( double )matrix[i,j] *
                              ( double )( nuColt[j] - cSum[j] ) /
                              ( double )cTot );
              if( matrix[i,j] < 0 )
                matrix[i,j] = 0;
              total += matrix[i,j];
              if( matrix[i,j] <= ct )
                continue;
              else
                ct = matrix[i,j];
              ii = i;
            }
            matrix[ii,j] += nuColt[j] - total - cSum[j];
          }
        }     // End for j

          /* Check difference between intermediate row totals and projected
           * row totals */
        total = getUpdateSum( numRows, numCols, matrix, rowSum, colSum );
        countMatches( numRows, numCols, rowSum, colSum, nuRowt, nuColt );

        //if( UPDATE_DEBUG )
        //{
          //writeToStatusBox( "stop 2 iteration = " + loopCount + " row " +
           //                 "matches = " + rowSame + " col matches = " +
            //                colSame );
          //printColSum( numRows, numCols, rowSum, colSum, nuRowt, nuColt );
        //}
        
          // If the rows match, bail out
        if( rowSame == numRows && colSame == numCols )
        {
          continueLoop = false;
          continue;
        }
                      
          // Factor matrix elements to row totals and round
        for( i = 0; i < numRows; i++ )
        {
          total = 0;
          rt = 0;
          jj = 0;
          for( j = 0; j < numCols; j++ )
          {
            if( rowSum[i] > 0 )
              matrix[i,j] = ( int )( 0.5 + ( double ) matrix[i,j] *
                            ( double )nuRowt[i] / ( double )rowSum[i] );
            total += matrix[i,j];
            if( matrix[i,j] <= rt )
              continue;
            rt = matrix[i,j];
            jj = j;
          }
            // Force matrix elements to row control total
          matrix[i,jj] += nuRowt[i] - total;
        }
        loopCount++;
      }     // End while loop

      //if( UPDATE_DEBUG )
      //{
        //total = getUpdateSum( numRows, numCols, matrix, rowSum, colSum );
        //printColSum( numRows, numCols, rowSum, colSum, nuRowt, nuColt );
        //for( i = 0; i < numRows; i++ )
          //for( j = 0; j < numCols; j++ )
     	      //writeToStatusBox( matrix[i,j].ToString() );
      //}
      return true;
    }     // End method update()
    //*************************************************************************

    #endregion
    #region ControlGQ
    /* method ControlGQ() */
    /// <summary>
    /// Method to perform GQ controlling.
    /// </summary>
  
    /* Revision History
     * 
     * STR             Date       By    Description
     * --------------------------------------------------------------------------
     *                 12/08/97   tb    Initial coding
     *                 08/22/03   df    C# revision
     * --------------------------------------------------------------------------
     */
    void ControlGQ(int fYear)
    {
      
      dcGQC = new StreamWriter(new FileStream(networkPath + String.Format(appSettings["dcGQC"].Value), FileMode.Create));
      SqlDataReader rdr;
      Random rand = new Random(0);
      int where,i,siteGQCiv,siteGQMil,numGQ= 0,GQSum,controlGQ;
      
      int [] gq = new Int32[2000];
      int [] gqold = new Int32[2000];
      int [] lckey = new Int32[2000];
      writeToStatusBox("Controlling GQ Civ");
      sqlCommand.CommandText = String.Format(appSettings["select13"].Value,TN.capacity4,fYear,scenarioID,bYear);
    
      siteGQCiv = 0;
      try
      {
        sqlConnection.Open();
        rdr = sqlCommand.ExecuteReader();
        while( rdr.Read() )
        {
         siteGQCiv =  rdr.GetInt32(0);
        }          
        rdr.Close();
       
      }     // End try

      catch( Exception e )
      {
        MessageBox.Show( e.ToString(), e.GetType().ToString() );
      }  // end catch
      finally
      {
        sqlConnection.Close();
      } 

      // gq_civ
      sqlCommand.CommandText = String.Format(appSettings["select14"].Value, TN.capacity4, scenarioID, bYear);

      i = 0;
      GQSum = 0;
      try
      {
        sqlConnection.Open();
        rdr = sqlCommand.ExecuteReader();
        while( rdr.Read() )
        {
          lckey[i] = rdr.GetInt32( 0 );
          gqold[i] = rdr.GetInt32(1);
          gq[i++] = rdr.GetInt32(1);
          GQSum += rdr.GetInt32(1);
        }          
        rdr.Close();

        numGQ = i;
      }     // End try

      catch( Exception e )
      {
        MessageBox.Show( e.ToString(), e.GetType().ToString() );
      }  // end catch
      finally
      {
        sqlConnection.Close();
      } 

      // control these gq's to regional total
      controlGQ = rc.fcst.p.gqCiv - siteGQCiv;
      while (GQSum != controlGQ)
      {
        where = ( int )( rand.NextDouble() * numGQ );
        if (GQSum > controlGQ)
        {
          if (gq[where] > 0)
          {
            --GQSum;
            --gq[where];
          }  // end if
        }   // end if
        else
        {
          ++gq[where];
          ++GQSum;
        }  // end else
      }  // end while
      writeToStatusBox("Updating GQ Civ");

      for (i = 0; i < numGQ; ++i)
      {
        if (gq[i] != gqold[i])
        {
          dcGQC.WriteLine(lckey[i].ToString() + "," + gq[i].ToString());

          
        }   // end if
      }   // end for i
      dcGQC.Close();
      // bulk load the gq updates
      sqlCommand.CommandText = "truncate table updateGQC";
      try
      {
        sqlConnection.Open();
        sqlCommand.ExecuteNonQuery();
      }
      catch (Exception e)
      {
        MessageBox.Show(e.ToString(), e.GetType().ToString());
      }
      finally
      {
        sqlConnection.Close();
      }

      string fn = networkPath + String.Format(appSettings["dcGQC"].Value);
      sqlCommand.CommandText = String.Format(appSettings["bulkInsert"].Value, TN.updateGQC, fn);
     
      try
      {
        sqlConnection.Open();
        sqlCommand.ExecuteNonQuery();
      }
      catch (Exception e)
      {
        MessageBox.Show(e.ToString(), e.GetType().ToString());
      }
      finally
      {
        sqlConnection.Close();
      }

          sqlCommand.CommandText = String.Format(appSettings["update09"].Value,TN.capacity4,TN.updateGQC,scenarioID,bYear);
      //sqlCommand.CommandText = "update " + TN.capacity4 + " set gq_civ = GQC  from " + TN.capacity4 + " c, updateGQC u where c.lckey = u.lckey";
      try
      {
        sqlConnection.Open();
        sqlCommand.ExecuteNonQuery();
      }
      catch (Exception e)
      {
        MessageBox.Show(e.ToString(), e.GetType().ToString());
      }
      finally
      {
        sqlConnection.Close();
      } 

      // gq_mil
      writeToStatusBox("Controlling GQ Mil");
      sqlCommand.CommandText = "select sum(siteGQMil) from " + TN.capacity4 + " where site > 0 and phase < " + fYear.ToString();

      siteGQMil = 0;
      try
      {
        sqlConnection.Open();
        rdr = sqlCommand.ExecuteReader();
        while (rdr.Read())
        {
          siteGQMil = rdr.GetInt32(0);
        }
        rdr.Close();

      }     // End try

      catch (Exception e)
      {
        MessageBox.Show(e.ToString(), e.GetType().ToString());
      }  // end catch
      finally
      {
        sqlConnection.Close();
      }

      sqlCommand.CommandText = String.Format(appSettings["select15"].Value, TN.capacity4, scenarioID, bYear);
      
      i = 0;
      GQSum = 0;
      try
      {
        sqlConnection.Open();
        rdr = sqlCommand.ExecuteReader();
        while( rdr.Read() )
        {
          lckey[i] = rdr.GetInt32( 0 );
          gqold[i] = rdr.GetInt32(1);
          gq[i++] = rdr.GetInt32(1);
          GQSum += rdr.GetInt32(1);
        }          
        rdr.Close();
        numGQ = i;
      }     // End try

      catch( Exception e )
      {
        MessageBox.Show( e.ToString(), e.GetType().ToString() );
      }  // end catch
      finally
      {
        sqlConnection.Close();
      } 
      // control these gq's to regional total
      controlGQ = rc.fcst.p.gqMil - siteGQMil;
      while (GQSum != controlGQ)
      {
        where = ( int )( rand.NextDouble() * numGQ );
        if (GQSum > controlGQ)
        {
          if (gq[where] > 0)
          {
            --GQSum;
            --gq[where];
          }
        }   // end if
        else
        {
          ++gq[where];
          ++GQSum;
        }   // end else
      }  // end while

      writeToStatusBox("Updating GQ Mil");
      for (i = 0; i < numGQ; ++i)
      {
        if (gq[i] != gqold[i])
        {
          sqlCommand.CommandText = String.Format(appSettings["update10"].Value, TN.capacity4, gq[i], lckey[i], scenarioID, bYear);
         
          try
          {
            sqlConnection.Open();
            sqlCommand.ExecuteNonQuery();

          }   // end try
          catch( Exception e )
          {
            MessageBox.Show( e.ToString(), e.GetType().ToString() );
            Close();
          }  // end catch
          finally
          {
            sqlConnection.Close();
          } 
        }   // end if
      }   // end for i
    }   // end method ControlGQ()

    //**************************************************************************************
    #endregion

    #region open files and print routines
    //******************************************
    //Includes procedures
    //  openFiles()
    //  printColSum()
    //  printDCEmp()
    //  printDCHHS()
    //  printDCIncome()
    //  printDCOvr()
    //  printDCPop()
    //  printDCRates()

    /*****************************************************************************/

    /* method openFiles() */
    /// <summary>
    /// Method to prepare and open all ASCII output files.
    /// </summary>
  
    /* Revision History
     * 
     * STR             Date       By    Description
     * --------------------------------------------------------------------------
     *                 12/02/97   tb    Initial coding
     *                 08/19/03   df    C# revision
     * --------------------------------------------------------------------------
     */
    void openFiles()
    {
      try
      {

        dcEmp = new StreamWriter( new FileStream( networkPath + String.Format(appSettings["dcEmp"].Value), FileMode.Create ) );
        dcHHS = new StreamWriter(new FileStream(networkPath + String.Format(appSettings["dcHHS"].Value), FileMode.Create));
        dcInc = new StreamWriter(new FileStream(networkPath + String.Format(appSettings["dcInc"].Value), FileMode.Create));
        dcOut = new StreamWriter(new FileStream(networkPath + String.Format(appSettings["dcOut"].Value), FileMode.Create));
        dcOutliers = new StreamWriter(new FileStream(networkPath + String.Format(appSettings["dcOutliers"].Value), FileMode.Create));
        dcOvr = new StreamWriter(new FileStream(networkPath + String.Format(appSettings["dcOvr"].Value), FileMode.Create));
        dcPop = new StreamWriter(new FileStream(networkPath + String.Format(appSettings["dcPop"].Value), FileMode.Create));
        dcRates = new StreamWriter(new FileStream(networkPath + String.Format(appSettings["dcRates"].Value), FileMode.Create));

        // Assert auto-flush property on all files.
        dcEmp.AutoFlush = true;
        dcHHS.AutoFlush = true;
        dcInc.AutoFlush = true;
        dcOut.AutoFlush = true;
        dcOutliers.AutoFlush = true;
        dcOvr.AutoFlush = true;
        dcPop.AutoFlush = true;
        dcRates.AutoFlush = true;
      }
      catch( IOException i )
      {
        MessageBox.Show( "Error opening file.  " + i.Message, "IO Exception" );
      }
    }     // End method openFiles()

    /*****************************************************************************/

    /* method printColSum() */
    /// <summary>
    /// Method to perform debug output for column sum and total.
    /// </summary>
  
    /* Revision History
     * 
     * STR             Date       By    Description
     * --------------------------------------------------------------------------
     *                 08/08/94   tb    Initial coding
     *                 08/19/03   df    C# revision
     * --------------------------------------------------------------------------
     */
    void printColSum( int numRows, int numCols, int[] rowSum, int[] colSum,
      int[] nuRowt, int[] nuColt )
    {
      for( int i = 0; i < numCols; i++ )
        writeToStatusBox( "col " + i + " computed sum = " + colSum[i] + " target sum = " + nuColt[i] );
      for( int j = 0; j < numRows; j++ )
        writeToStatusBox( "row " + j + " computed sum = " + rowSum[j] + " target sum = " + nuRowt[j] );
    }     // End procedure printColSum()

    /*****************************************************************************/

    /* method printDCEmp() */
    /// <summary>
    /// Method to write DC employment to ASCII.
    /// </summary>
  
    /* Revision History
     * 
     * STR             Date       By    Description
     * --------------------------------------------------------------------------
     *                 12/19/97   tb    Initial coding
     *                 08/27/03   df    C# revision
     * --------------------------------------------------------------------------
     */
    void printDCEmp()
    {
      int i, j, groupCount;
      int t;
      
     
      string title1 = "LUZ          Ag    Con    Mfg    Wht    Ret    TWU   INFO    FIN    PBS   EDHS     LH   OSER    GOV    SEDW Total";
      string title2 = "-----------------------------------------------------------------------------------------------------------------";
     
      groupCount = 0;
      dcEmp.WriteLine( "DETAILED CHARACTERISTICS CIVILIAN EMPLOYMENT " + outputLabel );
      dcEmp.WriteLine( title1 );
      dcEmp.WriteLine( title2 );

      for (i = 0; i < NUM_LUZS; i++ )
      {
        t = 0;
        dcEmp.Write( "{0,3} Base", i + 1 );
        for( j = 0; j < NUM_EMP_SECTORS; j++ )
          dcEmp.Write( "{0,7}", z[i].baseData.e.sectors[j] );
        dcEmp.WriteLine( "{0,7}", z[i].baseData.e.civ );
        dcEmp.Write( "    Fcst" );
        for( j = 0; j < NUM_EMP_SECTORS; j++ )
          dcEmp.Write( "{0,7}", z[i].fcst.e.sectors[j] );
        dcEmp.WriteLine( "{0,7}", z[i].fcst.e.civ );
        dcEmp.Write( "    Chng" );
        for( j = 0; j < NUM_EMP_SECTORS; j++ )
          dcEmp.Write( "{0,7}", z[i].fcst.ei.sectors[j] );
        dcEmp.WriteLine( "{0,7}", z[i].fcst.ei.civ );
        dcEmp.Write( "    Site" );
        for( j = 0; j < NUM_EMP_SECTORS; j++ )
          dcEmp.Write( "{0,7}", z[i].site.sectors[j] );
        dcEmp.WriteLine( "{0,7}", z[i].site.civ );
        dcEmp.Write( "     Mod" );
        for( j = 0; j < NUM_EMP_SECTORS; j++ )
        {
          dcEmp.Write( "{0,7}", z[i].fcst.ei.sectorsAdj[j] );
          t += z[i].fcst.ei.sectorsAdj[j];
        }  // end for j
        dcEmp.WriteLine( "{0,7}{1,7}", t, z[i].fcst.ei.adj );
              
        dcEmp.Write( "    %Chg" );
        for( j = 0; j < NUM_EMP_SECTORS; j++ )
          dcEmp.Write( "{0,7:F1}", z[i].fcst.pct.sectors[j] );
        dcEmp.WriteLine( "{0,7:F1}", z[i].fcst.pct.civ );
        dcEmp.WriteLine();
        groupCount++;

        if( groupCount >= 7 )
        {
          dcEmp.WriteLine( title2 );
          dcEmp.WriteLine();
          dcEmp.WriteLine();
          groupCount = 0;     // Reset line count
          dcEmp.WriteLine( "DETAILED CHARACTERISTICS CIVILIAN EMPLOYMENT " + outputLabel );
          dcEmp.WriteLine( title1 );
          dcEmp.WriteLine( title2 );
        }  // end if
      }     // End for i
      
    }     // End method printDCEmp()

    /*****************************************************************************/

    /* method printDCHHS() */
    /// <summary>
    /// Method to write DC HHS and ER/HH to ASCII.
    /// </summary>
  
    /* Revision History
     * 
     * STR             Date       By    Description
     * --------------------------------------------------------------------------
     *                 12/19/97   tb    Initial coding
     *                 08/27/03   df    C# revision
     * --------------------------------------------------------------------------
     */
    void printDCHHS()
    {
      string title0 = "               BASE               FCST                % CHG";
      string title1 = " LUZ       HHS     ER/HH       HHS     ER/HH       HHS     ER/HH";
      string title2 = "----------------------------------------------------------------";
      int lineCount = 0;
                  
     
      dcHHS.WriteLine( "DETAILED CHARACTERISTICS - HHS; ER/HH " + outputLabel);
      dcHHS.WriteLine( title0 );
      dcHHS.WriteLine( title1 );
      dcHHS.WriteLine( title2 );

      for( int i = 0; i < NUM_LUZS; i++ )
      {
        dcHHS.WriteLine( "{0,4}{1,10:F2}{2,10:F2}{3,10:F2}{4,10:F2}{5,10:F2}{6,10:F2}", i + 1, 
                        z[i].baseData.r.hhs, z[i].baseData.r.erHH, z[i].fcst.r.hhs,z[i].fcst.r.erHH, z[i].fcst.pct.hhs,z[i].fcst.pct.erHH );
        lineCount++;
        if( lineCount >= 57 )
        {
          dcHHS.WriteLine();
          dcHHS.WriteLine();
          lineCount = 0;
          dcHHS.WriteLine( "DETAILED CHARACTERISTICS - HHS; ER/HH " + outputLabel );
          dcHHS.WriteLine( title0 );
          dcHHS.WriteLine( title1 );
          dcHHS.WriteLine( title2 );
        } // end if
      }  // end for i

    }     // End method printDCHHS()

    /*****************************************************************************/

    /* method printDCIncome() */
    /// <summary>
    /// Method to write DC income to ASCII.
    /// </summary>
  
    /* Revision History
     * 
     * STR             Date       By    Description
     * --------------------------------------------------------------------------
     *                 12/19/97   tb    Initial coding
     *                 08/27/03   df    C# revision
     * --------------------------------------------------------------------------
     */
    void printDCIncome()
    {
      int t, i, j, groupCount = 0;
      string title1 = "LUZ         <15  15-29  30-44  45-59  60-74  74-99   00-24  25-50 50-99   200+  Total   Cont Median";
      string title2 = "---------------------------------------------------------------------------------------------------";
            
      try
      {
       
        dcInc.AutoFlush = true;
      }
      catch( IOException e )
      {
        MessageBox.Show( e.ToString(), "IOException" );
      }
      dcInc.WriteLine( "DETAILED CHARACTERISTICS - INCOME DIST " + outputLabel );
      dcInc.WriteLine( title1 );
      dcInc.WriteLine( title2 );
      for( i = 0; i < NUM_LUZS; i++ )
      {
        t = 0;
        dcInc.Write( "{0,3} Base", i + 1 );
        for( j = 0; j < NUM_HH_INCOME; j++ )
        {
          dcInc.Write( "{0,7}", z[i].baseData.i.hh[j] );
          t += z[i].baseData.i.hh[j];
        }     // End for j
        dcInc.WriteLine( "{0,7}{1,7}{2,7}", t, z[i].baseData.hh.total,
          z[i].baseData.i.median );
        t = 0;
        dcInc.Write( "    Fcst" );
        for( j = 0; j < NUM_HH_INCOME; j++ )
        {
          dcInc.Write( "{0,7}", z[i].fcst.i.hh[j] );
          t += z[i].fcst.i.hh[j];
        }  // end for j

        dcInc.WriteLine( "{0,7}{1,7}{2,7}", t, z[i].fcst.hh.total, z[i].fcst.i.median );

        t = 0;
        
        dcInc.Write( "    Chng" );
        for( j = 0; j < NUM_HH_INCOME; j++ )
        {
          dcInc.Write( "{0,7}", z[i].fcst.ii.hh[j] );
          t += z[i].fcst.ii.hh[j];
        }  // end for j
        dcInc.WriteLine( "{0,7}{1,7}{2,7}", t, z[i].fcst.hhi.total,z[i].fcst.ii.median );
        dcInc.Write( "    %Chg" );
        for( j = 0; j < NUM_HH_INCOME; j++ )
          dcInc.Write( "{0,7:F1}", z[i].fcst.pct.incomeHH[j] );
        dcInc.WriteLine();
        dcInc.WriteLine();
        groupCount++;
        if( groupCount >= 10 )
        {
          dcInc.WriteLine( title2 );
          dcInc.WriteLine();
          dcInc.WriteLine();
          groupCount = 0;     // Reset line count
          dcInc.WriteLine( "DETAILED CHARACTERISTICS - INCOME DIST " + outputLabel );
          dcInc.WriteLine( title1 );
          dcInc.WriteLine( title2 );
        }  // end if
      }     // End for i
     
    }     // End method printDCIncome()

    /*****************************************************************************/

    /* method printDCOvr() */
    /// <summary>
    /// Method to write DC overrides reports to ASCII.
    /// </summary>
  
    /* Revision History
     * 
     * STR             Date       By    Description
     * --------------------------------------------------------------------------
     *                 12/17/97   tb    Initial coding
     *                 08/27/03   df    C# revision
     * --------------------------------------------------------------------------
     */
    void printDCOvr()
    {
      int i;
      string title0 = "RATES AND INCOME PARMS";
      string title1 = " LUZ   Vac-Sf  Vac_Mf  Vac_Mh   ER/HH    HHS  Median     ASD     NLA     ISW";
      string title2 = "----------------------------------------------------------------------------";
                 
      try
      {
        
        dcOvr.AutoFlush = true;
      }
      catch( IOException e )
      {
        MessageBox.Show( e.ToString(), "IOException" );
      }

      dcOvr.WriteLine( title0 + " " + outputLabel );
      dcOvr.WriteLine( title1 );
      dcOvr.WriteLine( title2 );
      
      for(i = 0; i < NUM_LUZS; i++ )
      {
        // Skip LUZs with no overrides
        if( z[i].erOvr || z[i].hhsOvr || z[i].vacOvr )
        {
          dcOvr.Write( "{0,4}{1,8:F2}{2,8:F2}{3,8:F2}", i + 1, z[i].ro.vSF, z[i].ro.vMF, z[i].ro.vmh );
          dcOvr.Write( "{0,8:F2}{1,8:F2}{2,8}{3,8:F2}{4,8:F2}{5,8}", z[i].ro.erHH, z[i].ro.hhs, z[i].ro.medianIncome,z[i].ro.asd, z[i].ro.nla, z[i].ro.incomeSwitch );
        }  // end if
      }  // end for i
      dcOvr.WriteLine();
      dcOvr.WriteLine();
      
    }     // End method printDCOvr()

    /*****************************************************************************/

    /* method printDCPop() */
    /// <summary>
    /// Method to write DC pop variables to ASCII.  */
    /// </summary>
  
    /* Revision History
     * 
     * STR             Date       By    Description
     * --------------------------------------------------------------------------
     *                 12/19/97   tb    Initial coding
     *                 08/27/03   df    C# revision
     * --------------------------------------------------------------------------
     */
    void printDCPop()
    {
      int i;
      int lineCount = 0;
      int[] t = new int[8];
      string title0 = "                     BASE                             FCST                           % CHG";
      string title1 = " LUZ     POP     HHP      GQ      HH     POP     HHP      GQ      HH     POP     HHP      GQ      HH";   
      string title2 = "----------------------------------------------------------------------------------------------------";
           
      try
      {
        dcPop.AutoFlush = true;
      }
      catch( IOException e )
      {
        MessageBox.Show( e.ToString(), "IOException" );
      }
      dcPop.WriteLine( "DETAILED CHARACTERISTICS - POP " + outputLabel );
      dcPop.WriteLine( title0 );
      dcPop.WriteLine( title1 );
      dcPop.WriteLine( title2 );
      for(i = 0; i < NUM_LUZS; i++ )
      {
        dcPop.Write( "{0,4}{1,8}{2,8}{3,8}{4,8}", i + 1, z[i].baseData.p.pop, z[i].baseData.p.hhp, z[i].baseData.p.gq, z[i].baseData.hh.total );
        dcPop.Write( "{0,8}{1,8}{2,8}{3,8}", z[i].fcst.p.pop, z[i].fcst.p.hhp, z[i].fcst.p.gq, z[i].fcst.hh.total );
        dcPop.WriteLine( "{0,8:F1}{1,8:F1}{2,8:F1}{3,8:F1}", z[i].fcst.pct.pop, z[i].fcst.pct.hhp, z[i].fcst.pct.gq,z[i].fcst.pct.hh );
        t[0] += z[i].baseData.p.pop;
        t[1] += z[i].baseData.p.hhp;
        t[2] += z[i].baseData.p.gq;
        t[3] += z[i].baseData.hh.total;
        t[4] += z[i].fcst.p.pop;
        t[5] += z[i].fcst.p.hhp;
        t[6] += z[i].fcst.p.gq;
        t[7] += z[i].fcst.hh.total;
        lineCount++;

        if( lineCount >= 30 )
        {
          dcPop.WriteLine();
          dcPop.WriteLine();
          lineCount = 0;      // Reset line count
          dcPop.WriteLine( "DETAILED CHARACTERISTICS - POP " + outputLabel );
          dcPop.WriteLine( title0 );
          dcPop.WriteLine( title1 );
          dcPop.WriteLine( title2 );
        }  // end if
      }     // End for i

      dcPop.Write( " Sum" );
      for(i = 0; i < 8; i++ )
        dcPop.Write( "{0,8}", t[i] );
      dcPop.WriteLine();

    }     // End method printDCPop()

    /*****************************************************************************/

    /* method printDCRates() */
    /// <summary>
    /// Method to write LUZ vacancy rates to ASCII.  */
    /// </summary>
  
    /* Revision History
     * 
     * STR             Date       By    Description
     * --------------------------------------------------------------------------
     *                 12/19/97   tb    Initial coding
     *                 08/27/03   df    C# revision
     * --------------------------------------------------------------------------
     */
    void printDCRates()
    {
      string title1 = "                 Base Year                        Forecast Year                    Percent Change";
      string title2 = " LUZ     Vac  Vac-Sf  Vac-Mf  Vac-Mh     Vac  Vac-Sf  Vac-Mf  Vac-Mh     Vac  Vac_Sf  Vac-Mf  Vac_mh";      
      string title3 = "---------------------------------------------------------------------------------------------------------";
      int i, lineCount = 0;
            
      try
      {
        
        dcRates.AutoFlush = true;
      }
      catch( IOException e )
      {
        MessageBox.Show( e.ToString(), "IOException" );
      }
      dcRates.WriteLine( "DETAILED CHARACTERISTICS - VAC RATES " + outputLabel );
      dcRates.WriteLine( title1 );
      dcRates.WriteLine( title2 );
      dcRates.WriteLine( title3 );
      for( i = 0; i < NUM_LUZS; i++ )
      {
        dcRates.Write( "{0,4}{1,8:F2}{2,8:F2}{3,8:F2}{4,8:F2}", i + 1,z[i].baseData.r.v, z[i].baseData.r.vSF,z[i].baseData.r.vMF, z[i].baseData.r.vmh );
        dcRates.Write( "{0,8:F2}{1,8:F2}{2,8:F2}{3,8:F2}", z[i].fcst.r.v, z[i].fcst.r.vSF, z[i].fcst.r.vMF, z[i].fcst.r.vmh );
        dcRates.WriteLine( "{0,8:F2}{1,8:F2}{2,8:F2}{3,8:F2}", z[i].fcst.pct.v,z[i].fcst.pct.vSF, z[i].fcst.pct.vMF, z[i].fcst.pct.vmh );
        lineCount++;

        if( lineCount >= 30 )
        {
          dcRates.WriteLine();
          dcRates.WriteLine();
          lineCount = 0;      // Reset line count
          dcRates.WriteLine( "DETAILED CHARACTERISTICS - VAC RATES " + outputLabel );
          dcRates.WriteLine( title1 );
          dcRates.WriteLine( title2 );
          dcRates.WriteLine( title3 );
        }  // end if
      }     // End for i

    }     // End method printDCRates()

    /*****************************************************************************/
    #endregion

    private void btnExit_Click( object sender, System.EventArgs e )
    {
      Close();
    }
    
    private void Detailed_Closing( object sender, CancelEventArgs e )
    {
      caller.Visible = true;
    }

  }     // End class Detailed
}