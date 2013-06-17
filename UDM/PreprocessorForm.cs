/* Filename:    Preprocessor.cs
 * Program:     UDM
 * Version:    7.0 sr13
 * Programmers: Terry Beckhelm
 *              Daniel Flyte (C# revision)
 * Description: This form commands all actions associated with module 1, 
 *              preprocessing.  It is called from UDM, after the preprocessor 
 *              has been selected.
 * 
 * Includes procedures
 *  beginPreprocessorWork()
 *  buildLUZTemp()
 *  processParams()
 *  updateSiteSpec()
 *  updatecapacity()
 *  
 *  addSitespecAcres()
 *  moveZ()
 *  printLUZPre()
 *  storeAcres()
 *  storeCapAcres()
 *  storeCapCounts()
 *  storeZ()
 *  
 */

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Threading;
using System.Configuration;



namespace Sandag.TechSvcs.RegionalModels
{
  public class PreprocessorForm : BaseForm
  { 
    #region Instance fields
   
    private FileStream fos;
    private StreamWriter sw;
   
    private MGRAWeight[] weights;
    private StreamWriter excep = null;       // Exceptions file output
    private StreamWriter luzPre = null;      // LUZ capacity output data
    private string AccessWeightsFileName;

    private System.Windows.Forms.Label lblScenario;
    private System.Windows.Forms.ComboBox cboScenario;
    private System.Windows.Forms.ComboBox cboYearSelect;
    private System.Windows.Forms.Button btnRun;
    private System.Windows.Forms.Label lblTitle;
    private System.Windows.Forms.Button btnExit;
    private CheckBox chkDoAccessWeights;
    private System.Windows.Forms.Label lblYears;

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
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblYears = new System.Windows.Forms.Label();
            this.lblScenario = new System.Windows.Forms.Label();
            this.cboScenario = new System.Windows.Forms.ComboBox();
            this.cboYearSelect = new System.Windows.Forms.ComboBox();
            this.btnRun = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.chkDoAccessWeights = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // txtStatus
            // 
            this.txtStatus.Location = new System.Drawing.Point(12, 173);
            this.txtStatus.Size = new System.Drawing.Size(320, 66);
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Garamond", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.Navy;
            this.lblTitle.Location = new System.Drawing.Point(8, 16);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(176, 32);
            this.lblTitle.TabIndex = 12;
            this.lblTitle.Text = "Preprocessor";
            // 
            // lblYears
            // 
            this.lblYears.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblYears.Location = new System.Drawing.Point(16, 64);
            this.lblYears.Name = "lblYears";
            this.lblYears.Size = new System.Drawing.Size(120, 16);
            this.lblYears.TabIndex = 11;
            this.lblYears.Text = "Increment";
            // 
            // lblScenario
            // 
            this.lblScenario.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblScenario.Location = new System.Drawing.Point(176, 64);
            this.lblScenario.Name = "lblScenario";
            this.lblScenario.Size = new System.Drawing.Size(72, 16);
            this.lblScenario.TabIndex = 10;
            this.lblScenario.Text = "Scenario";
            // 
            // cboScenario
            // 
            this.cboScenario.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboScenario.Items.AddRange(new object[] {
            "0 - EP"});
            this.cboScenario.Location = new System.Drawing.Point(176, 88);
            this.cboScenario.Name = "cboScenario";
            this.cboScenario.Size = new System.Drawing.Size(96, 24);
            this.cboScenario.TabIndex = 9;
            // 
            // cboYearSelect
            // 
            this.cboYearSelect.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboYearSelect.Items.AddRange(new object[] {
            "2012 - 2020",
            "2020 - 2035",
            "2035 - 2050"});
            this.cboYearSelect.Location = new System.Drawing.Point(16, 88);
            this.cboYearSelect.Name = "cboYearSelect";
            this.cboYearSelect.Size = new System.Drawing.Size(128, 24);
            this.cboYearSelect.TabIndex = 8;
            // 
            // btnRun
            // 
            this.btnRun.BackColor = System.Drawing.Color.LightGreen;
            this.btnRun.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRun.Location = new System.Drawing.Point(48, 263);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(80, 32);
            this.btnRun.TabIndex = 13;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = false;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // btnExit
            // 
            this.btnExit.BackColor = System.Drawing.Color.Red;
            this.btnExit.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExit.Location = new System.Drawing.Point(136, 263);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(80, 32);
            this.btnExit.TabIndex = 14;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = false;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // chkDoAccessWeights
            // 
            this.chkDoAccessWeights.AutoSize = true;
            this.chkDoAccessWeights.Checked = true;
            this.chkDoAccessWeights.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDoAccessWeights.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkDoAccessWeights.Location = new System.Drawing.Point(19, 136);
            this.chkDoAccessWeights.Name = "chkDoAccessWeights";
            this.chkDoAccessWeights.Size = new System.Drawing.Size(149, 17);
            this.chkDoAccessWeights.TabIndex = 15;
            this.chkDoAccessWeights.Text = "Build Access Weights";
            this.chkDoAccessWeights.UseVisualStyleBackColor = true;
            // 
            // PreprocessorForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(354, 321);
            this.Controls.Add(this.chkDoAccessWeights);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.lblYears);
            this.Controls.Add(this.lblScenario);
            this.Controls.Add(this.cboScenario);
            this.Controls.Add(this.cboYearSelect);
            this.Name = "PreprocessorForm";
            this.Text = "UDM - Preprocessor";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.Preprocessor_Closing);
            this.Controls.SetChildIndex(this.cboYearSelect, 0);
            this.Controls.SetChildIndex(this.cboScenario, 0);
            this.Controls.SetChildIndex(this.lblScenario, 0);
            this.Controls.SetChildIndex(this.lblYears, 0);
            this.Controls.SetChildIndex(this.btnRun, 0);
            this.Controls.SetChildIndex(this.lblTitle, 0);
            this.Controls.SetChildIndex(this.txtStatus, 0);
            this.Controls.SetChildIndex(this.btnExit, 0);
            this.Controls.SetChildIndex(this.chkDoAccessWeights, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

    }
		#endregion
	
    public PreprocessorForm( Form form )
    {
      caller = form;
      writeToStatusBox( "Awaiting user input.." );
      InitializeComponent();
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
    *                 08/17/98   tb    Moved from emp_main
    *                 07/17/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    private void btnRun_Click( object sender, System.EventArgs e )
    {
        if (!processParams(this))
            return;
        MethodInvoker mi = new MethodInvoker( beginPreprocessorWork );
        mi.BeginInvoke( null, null );
    }     // End method btnRun_Click()

    // **************************************************************************

    /* method beginPreprocessorWork() */
    /// <summary>
    /// Method to begin run button processing.
    /// </summary>
    
    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 08/17/98   tb    Moved from emp_main
    *                 07/17/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    private void beginPreprocessorWork()
    {
        try
        {
            // Load the LUZ employment and HS history array.
            if( !UDMUtils.extractHistory( this, scenarioID, bYear ) )
             Close();
            
            // Create next increment capacity table
            if( !UDMUtils.copyCapacity( this, 1,scenarioID, bYear, fYear ) )
                Close();

            // Build the LUZ and LUZ aggregations data or use temp table
            buildLUZTemp();
            updateSitespec();
            updatecapacity( TN.capacity1 );
            if (DoAccessWeights)
            {
                processAccessWeights();
                bulkLoadAccessWeightsFromASCII();
            } // end if
            writeToStatusBox("Completed UDM PreProcessing");
            MessageBox.Show("Completed UDM Preprocessing" );
        }   // end try

        catch(Exception e)
        {
            MessageBox.Show(e.ToString(),e.GetType().ToString());
            Application.Exit();
        }

    }   // end beginPreprocessorWork()

    /*****************************************************************************/

    /* method buildLUZTemp() */
    /// <summary>
    /// Method to build intermediate luz table; aggregate sitespec records to 
    /// luz; aggregate incremental employment capacities to luz by udm land use
    /// types; aggregate incremental residential capacities to luz by udm land 
    /// use types; and aggregate developable acres for sitespec and other by
    /// udm land use types.
    /// </summary>
    /// <param name="sw">Which type of table to build</param>
    /// <param name="tableName">Name of the table</param>

    /* Revision History
    * 
    * STR             Date       By    Description
    * ------------------------------------------------------------------------
    *                 04/09/97   tb    Initial coding
    *                 04/11/97   tb    Added acres to computations
    *                 07/17/03   df    C# revision
    * ------------------------------------------------------------------------
    */
    private void buildLUZTemp()
    {
        int luz, civ, sf, mf, mh, gqc, gqm, type,i,j;
        int phase, site, uel, usl, uml;
        int zone;
        string vtext = "";
        int[] v1 = new int[6];
        int[] v3 = new int[17];
        double acres;
        double[] v2 = new double[17];
        double[] v4 = new double[17];

        int capCount = 0, zi, luzTableCount;
        // ------------------------------------------------------------------
      
        System.Data.SqlClient.SqlDataReader rdr;

        writeToStatusBox( "Building intermediate LUZ table.." );
        sqlCommand.CommandText = String.Format(appSettings["deleteFrom"].Value, TN.luztemp, scenarioID, bYear);
        try
        {
            sqlConnection.Open();
            sqlCommand.ExecuteNonQuery();
        }
        catch (Exception sq)
        {
            MessageBox.Show(sq.ToString(), "SQL Exception");
        }
        finally
        {
            sqlConnection.Close();
        }
 
        // Aggregate sitespec acres to luzs
        //writeToStatusBox( "First pass through capacity.." );
        sqlCommand.CommandText = String.Format(appSettings["select03"].Value, TN.capacity, scenarioID, bYear);
     
        try
        {
            sqlConnection.Open();
            rdr = sqlCommand.ExecuteReader();
            while( rdr.Read() )
            {
                luz = rdr.GetInt16( 0 );
                phase = rdr.GetInt16( 1 );
                site = rdr.GetInt16( 2 );
                uel = rdr.GetByte( 3 );
                usl = rdr.GetByte( 4 );
                uml = rdr.GetByte( 5 );
                acres = ( double )rdr.GetDouble( 6 );
                civ = rdr.GetInt32( 7 );
                sf = rdr.GetInt32( 8 );
                mf = rdr.GetInt32( 9 );
                mh = rdr.GetInt32( 10 );
                gqc = rdr.GetInt32( 11 );
                gqm = rdr.GetInt32( 12 );
                zone = z[luz-1].zone;
                zi = luz - 1;
                capCount++;
                if( capCount % 100000 == 0 )
                    writeToStatusBox( "Processed " + capCount + " records from capacity" );

                // Handle site spec records first.
                if( site > 0 )
                {
                    // Accumulate emp and hs only for this phase
                    if( phase < fYear )
                        storeZ( zi, zone, civ, sf, mf, mh, gqc, gqm );// Store the query results in various luz arrays.
    
                    if( site !=  99 )     // Exclude roads for acreage
                    {
                        /* Accumulate all acres for site spec records and check land use category for storage. */
                        if( uel > 0 )     // Employment acreas
                            storeAcres( zi, zone, acres, 1, uel );   // Store the emp acres
                        if( usl > 0 )
                            storeAcres( zi, zone, acres, 2, usl );   // Store the SF acres
                        if( uml > 0 )
                            storeAcres( zi, zone, acres, 3, uml );   // Store the MF acres
                    }     // End if site != 99
                }     // End if site > 0 (sitespec)

                // All non site spec records, exclude 99 and 999 also.
                else if( site == 0 )
                {
                    // Accumulate emp and hs for this phase only
                    if( phase < fYear )
                    {
                        if( uel > 0 )
                            storeCapCounts( zi, zone, civ, 1, uel );    /* Store the emp counts */
                        if( usl > 0 )
                            storeCapCounts( zi, zone, sf, 2, usl );     /* Store the SF counts */
                        if( uml > 0 )
                            storeCapCounts( zi, zone, mf, 3, uml );     /* Store the MF counts */
                    }  // end if
                    if( uel > 0 )
                        storeCapAcres( zi, zone, acres, 1, uel );     /* Store the emp acres */
                    else if( usl > 0 )
                        storeCapAcres( zi, zone, acres, 2, usl );     /* Store the emp acres */
                    else if( uml > 0 )
                        storeCapAcres( zi, zone, acres, 3, uml );     /* Store the emp acres.*/
                }     // End else no site spec
            }     // End while rdr
            rdr.Close();

            writeToStatusBox( "Processed " + capCount + " records with devCode > 2 from capacity" + Environment.NewLine );
       
        }     // End try

        catch( Exception e )
        {
            MessageBox.Show( e.ToString(), e.GetType().ToString() );
            Application.Exit();
        }
        finally
        {
        sqlConnection.Close();
        }
        // Build LUZ aggregation temp table
      
        luzTableCount = 0;

        for(i = 0; i < NUM_LUZS; i++ )
        {
            type = 1;
            luz = i + 1;        
            // Add sitespec to non sitespec for total acres
            addSitespecAcres( z[i] );
            moveZ( z[i], v1, v2, v3, v4 );
            vtext = " VALUES (" + scenarioID + "," + bYear + "," + type + ", " + luz + ", ";
        
            for (j = 0; j <= 5; ++j)
              vtext += v1[j] + ", ";
            for (j = 0; j <=16; ++j)
              vtext += v2[j] + ", ";
            for (j = 0; j <=16; ++j)
              vtext += v3[j] + ", ";
            for (j = 0; j <= 15; ++j)
              vtext += v4[j] + ",";
            vtext += v4[16] + ")";
            sqlCommand.CommandText = String.Format(appSettings["insertInto"].Value,TN.luztemp,vtext);                                              
            try
            {
              sqlConnection.Open();
              sqlCommand.ExecuteNonQuery(); 
        
              luzTableCount++;
            }
            catch( Exception e )
            {
              MessageBox.Show( e.ToString(), e.GetType().ToString() );
              Application.Exit();
            }
            finally
            {
              sqlConnection.Close();
            }
        }     // End for

        type = 3;
        luz = 999;
        // Add sitespec to non-sitespec for total acres
        addSitespecAcres( reg );

        moveZ( reg, v1, v2, v3, v4 );
        vtext = " VALUES (" + scenarioID + "," + bYear + "," + type + ", " + luz + ", ";
        for(i = 0; i < v1.Length; i++ )
            vtext += v1[i] + ", ";
        for(i = 0; i < v2.Length; i++ )
            vtext += v2[i] + ", ";
        for(i = 0; i < v3.Length; i++ )
            vtext += v3[i] + ", ";
        for(i = 0; i < v4.Length - 1; i++ )
            vtext += v4[i] + ", ";
        vtext += v4[v4.Length - 1] + ")";

        sqlCommand.CommandText = String.Format(appSettings["insertInto"].Value,TN.luztemp,vtext);
        try
        {
            sqlConnection.Open();
            sqlCommand.ExecuteNonQuery();
        
        }
        catch (Exception e)
        {
            MessageBox.Show(e.ToString(),e.GetType().ToString());
            Application.Exit();
        }
        finally
        {
            sqlConnection.Close();
        }
        luzTableCount++;
        printLUZPre();
        writeToStatusBox( "Processed " + luzTableCount + " records to luz_temp table" + Environment.NewLine );
    }     // End method buildLUZTemp()

    /*****************************************************************************/

    /* method processParams() */
    /// <summary>
    /// Method to process module 1 (preprocessor) parameters, including all 
    /// necessary table names, variables, and files.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * ------------------------------------------------------------------------
    *                 08/17/98   tb    Initial coding
    *                 02/02/99   tb    Changed name of access_wts because of 
    *                                  system errors
    *                 01/11/02   tb    Changes for Jan, 2002 sr10
    *                 07/17/03   df    C# revision
    * ------------------------------------------------------------------------
    */
    private bool processParams(BaseForm form)
    {
       
        // Process input parameters
        if( cboYearSelect.SelectedIndex == -1 )
        {
            MessageBox.Show( "Invalid selection of years! Please try again." );
            return false;
        }
        if( cboScenario.SelectedIndex == -1 )
        {
            MessageBox.Show( "Invalid scenario! Please try again." );
            return false;
        }  // end if

        if (chkDoAccessWeights.Checked)
            DoAccessWeights = true;
        scenarioID = cboScenario.SelectedIndex;
        bYear = incrementLabels[cboYearSelect.SelectedIndex];
        fYear = incrementLabels[cboYearSelect.SelectedIndex + 1];     
        outputLabel = scenLabels[scenarioID] + " " + incrementLabels[cboYearSelect.SelectedIndex] + 
                      " - " + incrementLabels[cboYearSelect.SelectedIndex + 1] + " " +  DateTime.Now;

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

            NUM_EMP_LAND = int.Parse(appSettings["NUM_EMP_LAND"].Value);
            NUM_MF_LAND = int.Parse(appSettings["NUM_MF_LAND"].Value);
            NUM_SF_LAND = int.Parse(appSettings["NUM_SF_LAND"].Value);
           
            NUM_LUZS = int.Parse(appSettings["NUM_LUZS"].Value); 
            NUM_MGRAS = int.Parse(appSettings["NUM_MGRAS"].Value);

            ZB_DEBUG = bool.Parse(appSettings["ZB_DEBUG"].Value);        // Write luzBase to file
            ZH_DEBUG = bool.Parse(appSettings["ZH_DEBUG"].Value);        // Write luzHistory to file
            ZT_DEBUG = bool.Parse(appSettings["ZT_DEBUG"].Value);        // Write luzTemp to file

            TN.accessWeights = String.Format(appSettings["accessWeightsTable"].Value);
            TN.capacity = String.Format(appSettings["capacity"].Value);
            TN.capacity1 = String.Format(appSettings["capacity1"].Value);
            TN.luzbase = String.Format(appSettings["luzbase"].Value);
            TN.luzhist = String.Format(appSettings["luzhist"].Value);
            TN.luztemp = String.Format(appSettings["luztemp"].Value);
            TN.mgrabase = String.Format(appSettings["mgrabase"].Value);
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

        AccessWeightsFileName = networkPath + String.Format(appSettings["accessWeightsFile"].Value);

        try
        {
            luzPre = new StreamWriter(new FileStream(networkPath + String.Format(appSettings["luzPre"].Value), FileMode.Create));
        }   // end try
        catch (IOException i)
        {
            MessageBox.Show(i.ToString(), i.GetType().ToString());
            Application.Exit();
        }   // end catch

        fos = new FileStream(AccessWeightsFileName, FileMode.Create);
        sw = new StreamWriter(fos);
        z = new Master[NUM_LUZS];
        reg = new Master();
        return true;
    }     // End method processPreParams()

    /*****************************************************************************/

    /* method updateSitespec() */
    /// <summary>
    /// Method to reset variables in sitespec records after processing.
    /// </summary>
    /// <param name="sw">Switch</param>
    /// <param name="parm">Index value</param>
    /// <param name="val">Civ, sf, or mf to be stored</param>

    /* Revision History
    * 
    * STR             Date       By    Description
    * -------------------------------------------------------------------------
    *                 04/10/97   tb    Initial coding
    *                 07/18/03   df    C# revision
    * -------------------------------------------------------------------------
    */
    private void updateSitespec()
    {
        int ii,i,j;
        int[] exg = new int[8];
        int[,] exOut = new int[100,8];     // Possible exceptions for sitespec
        
        // ------------------------------------------------------------------
      
        writeToStatusBox( "Updating sitespec records.." );
        writeToStatusBox( "..Resetting pcap and base year values" );
     
        sqlCommand.CommandText= String.Format(appSettings["update15"].Value, TN.capacity1, bYear,fYear,scenarioID,bYear); 
        try
        {
	        sqlConnection.Open();
	        sqlCommand.ExecuteNonQuery();	 
        }
        catch (Exception e)
        {
            MessageBox.Show(e.ToString(),e.GetType().ToString());
        }
        finally
        {
            sqlConnection.Close();
        }

        // update sitemil for 1st increment only
      
        sqlCommand.CommandText = String.Format(appSettings["update18"].Value, TN.capacity1, fYear,bYear,scenarioID,bYear); 
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

        writeToStatusBox( "Resetting capacity values.." );

        sqlCommand.CommandText = String.Format(appSettings["update16"].Value, TN.capacity1, fYear,scenarioID,bYear); 
        try
        {
		    sqlConnection.Open();
		    sqlCommand.ExecuteNonQuery();
        }
        catch( Exception e )
        {
            MessageBox.Show(e.ToString(),e.GetType().ToString());
        }
        finally
        {
            sqlConnection.Close();
        }

        writeToStatusBox( "Checking for negative site spec forecasts.." );
        ii = 0;
        string stext1, stext2;
        // Scan the site spec records for negative forecast.
        SqlDataReader rdr;
        stext1 = " mgra, sphere, LCKey, emp_civ, hs_sf, hs_mf, hs_mh "; 
        stext2 = " (emp_civ < 0 OR hs_sf < 0 OR hs_mf < 0 OR hs_mh < 0) ";
        sqlCommand.CommandText= String.Format(appSettings["select16"].Value, stext1, TN.capacity1,stext2,scenarioID,bYear); 
        try
        {
		    sqlConnection.Open();
	
            rdr = sqlCommand.ExecuteReader();
            while( rdr.Read() )
            {
                exg[0] = rdr.GetInt32( 0 );
                exg[1] = rdr.GetInt32( 1 );
                exg[2] = rdr.GetInt32( 2 );
                exg[3] = rdr.GetInt32( 3 );
                exg[4] = rdr.GetInt32( 4 );
                exg[5] = rdr.GetInt32( 5 );
                exg[6] = rdr.GetInt32( 6 );
          
                for(j = 0; j < 7; j++ )
                    exOut[ii,j] = exg[j];
                ii++;
            }     // End while
		    rdr.Close();
       
            if( ii > 0 )
            {
                flags.except = true;
                try
                {
                    excep = new StreamWriter( new FileStream(networkPath + String.Format(appSettings["preExcep"].Value),FileMode.Create ) );
                    excep.AutoFlush = true;
                }
                catch( IOException e )
                {
                    MessageBox.Show( e.ToString(), e.GetType().ToString() );
                }

                excep.WriteLine( "Negative site spec forecasts" );
                excep.WriteLine( "  mgra   sph    id   civ   sf    mf    mh" );
                excep.WriteLine();

                for(i = 0; i < ii; i++ )
                {
                    for(j = 0; j < 8; j++ )
                        excep.Write( "{0,6}", exOut[i,j] );
                    excep.WriteLine();
                }   // end for i
                excep.WriteLine();
                excep.WriteLine();
            }   // end if
          
        }     // End try
      
        catch (Exception e)
        {
            MessageBox.Show(e.ToString(),e.GetType().ToString());
        }
        finally
        {
            sqlConnection.Close();
        } 

        writeToStatusBox( "Constraining base year values - empCiv.." );

        sqlCommand.CommandText = String.Format(appSettings["update03"].Value, TN.capacity1, fYear,scenarioID,bYear); 
        try
        {
		    sqlConnection.Open();
		    sqlCommand.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            MessageBox.Show(e.ToString(),e.GetType().ToString());
        }
        finally
        {
            sqlConnection.Close();
        }
      
        writeToStatusBox( "Constraining base year values - SF HS.." );
     
        sqlCommand.CommandText = String.Format(appSettings["update04"].Value, TN.capacity1, fYear,scenarioID, bYear,"hs_sf"); 
        try
        {
		    sqlConnection.Open();
		    sqlCommand.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            MessageBox.Show(e.ToString(),e.GetType().ToString());
        }
        finally
        {
            sqlConnection.Close();
        }

        writeToStatusBox( "Constraining base year values - MF HS.." );
      
        sqlCommand.CommandText = String.Format(appSettings["update04"].Value, TN.capacity1, fYear,scenarioID,bYear,"hs_mf"); 
        try
        {
		    sqlConnection.Open();
		    sqlCommand.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            MessageBox.Show(e.ToString(),e.GetType().ToString());
        }
        finally
        {
            sqlConnection.Close();
        }

        writeToStatusBox( "Constraining base year values - mh HS.." );
     
        sqlCommand.CommandText = String.Format(appSettings["update04"].Value, TN.capacity1, fYear,scenarioID,bYear,"hs_mh"); 
        try
        {
		    sqlConnection.Open();
		    sqlCommand.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            MessageBox.Show(e.ToString(),e.GetType().ToString());
        }
        finally
        {
            sqlConnection.Close();
        }

        writeToStatusBox( "Computing totals.." );
      
        sqlCommand.CommandText = String.Format(appSettings["update14"].Value, TN.capacity1, scenarioID, bYear); 
        try
        {
		    sqlConnection.Open();
		    sqlCommand.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            MessageBox.Show(e.ToString(),e.GetType().ToString());
        }
        finally
        {
            sqlConnection.Close();
        }
    }     // End method updateSitespec()

    /*****************************************************************************/

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
    private void updatecapacity( string capTable )
    {

        writeToStatusBox( "Updating CapacityNext table totals.." );      
     
        sqlCommand.CommandText = String.Format(appSettings["update20"].Value, TN.capacity1, scenarioID, bYear); 
        try
        {
		    sqlConnection.Open();
		    sqlCommand.ExecuteNonQuery();
        }   // end try
        catch (Exception e)
        {
            MessageBox.Show(e.ToString(),e.GetType().ToString());
        }
        finally
        {
            sqlConnection.Close();
        } 
    }     // End method updatecapacity()

    // *************************************************************************

     #region Various Helper Methods

    /*****************************************************************************/

    /* method addSitespecAcres() */
    /// <summary>
    /// Method to add sitespec acres total.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 05/22/97   tb    Initial coding
    *                 07/17/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    private void addSitespecAcres( Master zl )
    {
        int j;
        zl.capacity.ac.totalEmpAcres = 0;
        for( j = 1; j < NUM_EMP_LAND; j++ )
        {
            zl.capacity.ac.ae[j] += zl.site.ac.ae[j];
            zl.capacity.ac.totalEmpAcres += zl.capacity.ac.ae[j];
        }  // end for

        zl.capacity.ac.totalSFAcres = 0;
        for( j = 1; j < NUM_SF_LAND; j++ )
        {
            zl.capacity.ac.asf[j] += zl.site.ac.asf[j];
            zl.capacity.ac.totalSFAcres += zl.capacity.ac.asf[j];
        }  // end for

        zl.capacity.ac.totalMFAcres = 0;
        for( j = 1; j < NUM_MF_LAND; j++ )
        {
            zl.capacity.ac.amf[j] += zl.site.ac.amf[j];
            zl.capacity.ac.totalMFAcres += zl.capacity.ac.amf[j];
        } // emd for
    }     // End method addSitespecAcres()
    
    /*****************************************************************************/

    /* method moveZ() */
    /// <summary>
    /// Method to store luz aggregation data inyo SQL array for insert.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 04/14/97   tb    Initial coding
    *                 07/17/03   df    C# revision
    * --------------------------------------------------------------------------   
    */
    private void moveZ( Master zl, int[] v1, double[] v2, int[] v3,double[] v4 )
    {
        v1[0] = zl.site.civ;
        v1[1] = zl.site.sf;
        v1[2] = zl.site.mf;
        v1[3] = zl.site.mh;
        v1[4] = zl.site.gqCiv;
        v1[5] = zl.site.gqMil;

        v2[0] = zl.site.ac.ae[0];
        v2[1] = zl.site.ac.ae[1];
        v2[2] = zl.site.ac.ae[2];
        v2[3] = zl.site.ac.ae[3];
        v2[4] = zl.site.ac.ae[4];
        v2[5] = zl.site.ac.ae[5];
        v2[6] = zl.site.ac.ae[6];
        v2[7] = zl.site.ac.totalEmpAcres;
        v2[8] = zl.site.ac.asf[1];
        v2[9] = zl.site.ac.asf[2];
        v2[10] = zl.site.ac.asf[3];
        v2[11] = zl.site.ac.asf[4];
        v2[12] = zl.site.ac.totalSFAcres;
        v2[13] = zl.site.ac.amf[1];
        v2[14] = zl.site.ac.amf[2];
        v2[15] = zl.site.ac.amf[3];
        v2[16] = zl.site.ac.totalMFAcres;

        v3[0] = zl.capacity.e[0];
        v3[1] = zl.capacity.e[1];
        v3[2] = zl.capacity.e[2];
        v3[3] = zl.capacity.e[3];
        v3[4] = zl.capacity.e[4];
        v3[5] = zl.capacity.e[5];
        v3[6] = zl.capacity.e[6];
        v3[7] = zl.capacity.totalEmp;
        v3[8] = zl.capacity.sf[1];
        v3[9] = zl.capacity.sf[2];
        v3[10] = zl.capacity.sf[3];
        v3[11] = zl.capacity.sf[4];
        v3[12] = zl.capacity.totalSF;
        v3[13] = zl.capacity.mf[1];
        v3[14] = zl.capacity.mf[2];
        v3[15] = zl.capacity.mf[3];
        v3[16] = zl.capacity.totalMF;

        v4[0] = zl.capacity.ac.ae[0];
        v4[1] = zl.capacity.ac.ae[1];
        v4[2] = zl.capacity.ac.ae[2];
        v4[3] = zl.capacity.ac.ae[3];
        v4[4] = zl.capacity.ac.ae[4];
        v4[5] = zl.capacity.ac.ae[5];
        v4[6] = zl.capacity.ac.ae[6];
        v4[7] = zl.capacity.ac.totalEmpAcres;
        v4[8] = zl.capacity.ac.asf[1];
        v4[9] = zl.capacity.ac.asf[2];
        v4[10] = zl.capacity.ac.asf[3];
        v4[11] = zl.capacity.ac.asf[4];
        v4[12] = zl.capacity.ac.totalSFAcres;
        v4[13] = zl.capacity.ac.amf[1];
        v4[14] = zl.capacity.ac.amf[2];
        v4[15] = zl.capacity.ac.amf[3];
        v4[16] = zl.capacity.ac.totalMFAcres;
    }     // End method moveZ()

    /*****************************************************************************/

      /* method printLUZPre() */
      /// <summary>
      /// Method to write luz employment to ASCII.
      /// </summary>
      /// <param name="capTable">Which capacity table to copy out</param>
      
      /* Revision History
      * 
      * STR             Date       By    Description
      * --------------------------------------------------------------------------
      *                 04/17/97   tb    Initial coding
      *                 07/18/03   df    C# revision
      * --------------------------------------------------------------------------
      */
      private void printLUZPre()
      {
        int numAreas = NUM_LUZS, lineCount = 0, i, j;
        string title1 = "LUZS " + outputLabel;
        string title2 = " LUZ       SF       MF       mh      CIV" +
            "     GQC      GQM";
        string title2a = "----------------------------------------------------" +
            "---------------";
        string title3 = " LUZ     Total     Redev    Infill   Vac-Ind   " +
            "Vac-Com   Vac-Off   Vac-Sch";
        string title3a = "----------------------------------------------------" +
            "----------------------";
        string title4 = " LUZ      SF   Redev  Infill  Vac-LD  Vac-UD      MF " +
            "  Redev  Infill  Vac-AG";
        string title4a = "----------------------------------------------------" +
            "------------------------";

        // Site spec
        luzPre.WriteLine( "Table 1-1" );
        luzPre.WriteLine( "SITE SPEC ACTIVITIES - " + title1 );
        luzPre.WriteLine();
        luzPre.WriteLine();
        luzPre.WriteLine( title2 );
        luzPre.WriteLine( title2a );

        for( i = 0; i < numAreas; i++ )
        {
          luzPre.WriteLine( "{0,4}{1,9}{2,9}{3,9}{4,9}{5,9}{6,9}", i + 1,
            z[i].site.sf, z[i].site.mf, z[i].site.mh, z[i].site.civ, z[i].site.gqCiv, z[i].site.gqMil );
          lineCount++;
          if( lineCount >= 57 )
          {
            luzPre.WriteLine();
            luzPre.WriteLine();
            lineCount = 0;            // Reset line count
            luzPre.WriteLine( "Table 1-1" );
            luzPre.WriteLine( "SITE SPEC ACTIVITIES - " + title1 );
            luzPre.WriteLine();
            luzPre.WriteLine();
            luzPre.WriteLine( title2 );
            luzPre.WriteLine( title2a );
          }   // end if
          luzPre.Flush();
        }   // end for

        luzPre.WriteLine();
        luzPre.WriteLine( "Reg " + "{0,9}{1,9}{2,9}{3,9}{4,9}{5,9}",
          reg.site.sf, reg.site.mf, reg.site.mh, reg.site.civ,reg.site.gqCiv, reg.site.gqMil );
        luzPre.Flush();

        // Civilian employment capacities
        lineCount = 0;
        luzPre.WriteLine();
        luzPre.WriteLine();
        luzPre.WriteLine( "Table 1-2" );
        luzPre.WriteLine( "CIVILIAN EMPLOYMENT CAPACITY - " + title1 );
        luzPre.WriteLine();
        luzPre.WriteLine( title3 );
        luzPre.WriteLine( title3a );
        for ( i = 0; i < numAreas; i++ )
        {
          luzPre.Write( "{0,4}{1,10:F1}", i + 1, z[i].capacity.totalEmp );

          for( j = 1; j < NUM_EMP_LAND; j++ )     // 6 emp categories
            luzPre.Write( "{0,10:F1}", z[i].capacity.e[j] );
          luzPre.WriteLine();
          lineCount++;
          
          if( lineCount >= 57 )
          {
            luzPre.WriteLine();
            luzPre.WriteLine();
            lineCount = 0;            // Reset line count
            luzPre.WriteLine( "Table 1-2" );
            luzPre.WriteLine( "EMPLOYMENT CAPACITY - " + title1 );
            luzPre.WriteLine();
            luzPre.WriteLine( title3 );
            luzPre.WriteLine( title3a );
          }   // end if
        }     // End for i

        luzPre.WriteLine();
        luzPre.Write( "Reg {0,10:F1}", reg.capacity.totalEmp );
                
        for( j = 1; j < NUM_EMP_LAND; j++ )     // 6 emp categories
          luzPre.Write( "{0,10:F1}", reg.capacity.e[j] );
        luzPre.WriteLine();
        luzPre.Flush();

        // Housing unit capacities       
        lineCount = 0;
        luzPre.WriteLine();
        luzPre.WriteLine();
        luzPre.WriteLine( "Table 1-3" );
        luzPre.WriteLine( "HOUSING UNIT CAPACITIES - " + title1 );
        luzPre.WriteLine();
        luzPre.WriteLine( title4 );
        luzPre.WriteLine( title4a );

        for( i = 0; i < numAreas; i++ )
        {
          luzPre.Write( "{0,4}{1,8}", i + 1, z[i].capacity.totalSF );
          for( j = 1; j < NUM_SF_LAND; j++ )      // 4 sf categories
            luzPre.Write( "{0,8}", z[i].capacity.sf[j] );
          luzPre.Write( "{0,8}", z[i].capacity.totalMF );

          for( j = 1; j < NUM_MF_LAND; j++ )      //  mf categories
            luzPre.Write( "{0,8}", z[i].capacity.mf[j] );
                  
          luzPre.WriteLine();
          lineCount++;

          if( lineCount >= 57 )
          {
            luzPre.WriteLine();
            luzPre.WriteLine();
            lineCount = 0;            // Reset line count
            luzPre.WriteLine( "Table 1-3" );
            luzPre.WriteLine( "UNIT CAPACITIES - " + title1 + "\n" );
            luzPre.WriteLine( title4 );
            luzPre.WriteLine( title4a );
          }   // end if
          luzPre.Flush();
        }     // End for i
                
        luzPre.Write( "Reg {0,8}", reg.capacity.totalSF );
        for( j = 1; j < NUM_SF_LAND; j++ )      // 4 SF categories
          luzPre.Write( "{0,8}", reg.capacity.sf[j] );
        luzPre.Write( "{0,8}", reg.capacity.totalMF );
        for( j = 1; j < NUM_MF_LAND; j++ )      // MF categories
          luzPre.Write( "{0,8}", reg.capacity.mf[j] );
        luzPre.WriteLine();
        luzPre.Flush();
            
        // Developable employment acres - total
        lineCount = 0;
        luzPre.WriteLine();
        luzPre.WriteLine();
        luzPre.WriteLine( "Table 1-4" );
        luzPre.WriteLine( "DEVELOPABLE EMPLOYMENT ACRES - ALL SOURCES - " + 
            title1 + "\n" );
        luzPre.WriteLine( title3 );
        luzPre.WriteLine( title3a );
        for( i = 0; i < numAreas; i++ )
        {
          luzPre.Write( "{0,4}{1,10:F1}", i + 1, z[i].capacity.ac.totalEmpAcres );
          for( j = 1; j < NUM_EMP_LAND; j++ )     // 6 emp categories
              luzPre.Write( "{0,10:F1}",z[i].capacity.ac.ae[j] );
          luzPre.WriteLine();
          lineCount++;

          if( lineCount >= 57 )
          {
            luzPre.WriteLine();
            luzPre.WriteLine();
            lineCount = 0;            // Reset line count
            luzPre.WriteLine( "Table 1-4" );
            luzPre.WriteLine( "DEVELOPABLE EMPLOYMENT ACRES - ALL SOURCES - " + title1 );
            luzPre.WriteLine();
            luzPre.WriteLine( title3 );
            luzPre.WriteLine( title3a );
          }   // end if
          luzPre.Flush();
        }   // end for

        luzPre.Write( "Reg {0,10:F1}", reg.capacity.ac.totalEmpAcres );
                
        for( j = 1; j < NUM_EMP_LAND; j++ )     /* 6 emp categories */
          luzPre.Write( "{0,10:F1}", reg.capacity.ac.ae[j] );
        luzPre.WriteLine();
        luzPre.Flush();

        // Developable employment acres - sitespec
        lineCount = 0;
        luzPre.WriteLine();
        luzPre.WriteLine();
        luzPre.WriteLine( "Table 1-5" );
        luzPre.WriteLine( "DEVELOPABLE EMPLOYMENT ACRES - SITESPEC - " + title1 );
        luzPre.WriteLine();
        luzPre.WriteLine( title3 );
        luzPre.WriteLine( title3a );
        for( i = 0; i < numAreas; i++ )
        {
          luzPre.Write( "{0,4}{0,10:F1}", i +1 , 
            z[i].site.ac.totalEmpAcres );
          for( j = 1; j < NUM_EMP_LAND; j++ )     // 6 emp categories
            luzPre.Write( "{0,10:F1}", z[i].site.ac.ae[j] );
          luzPre.WriteLine();
          lineCount++;
                
          if( lineCount >= 57 )
          {
            luzPre.WriteLine();
            luzPre.WriteLine();
            lineCount = 0;            // Reset line count
            luzPre.WriteLine( "Table 1-5" );
            luzPre.WriteLine( "DEVELOPABLE EMPLOYMENT ACRES - SITESPEC - " + title1 );
            luzPre.WriteLine();
            luzPre.WriteLine( title3 );
            luzPre.WriteLine( title3a );
          }   // end if
          luzPre.Flush();
        }     // End for i

        luzPre.Write( "Reg {0,10:F1}", reg.site.ac.totalEmpAcres );
                
        for( j = 1; j < NUM_EMP_LAND; j++ )     // 6 emp categories
          luzPre.Write( "{0,10:F1}", reg.site.ac.ae[j] );
        luzPre.WriteLine();
        luzPre.Flush();

        // Developable housing unit acres - all sources
        lineCount = 0;
        luzPre.WriteLine();
        luzPre.WriteLine();
        luzPre.WriteLine( "Table 1-6" );
        luzPre.WriteLine( "DEVELOPABLE HOUSING UNIT ACRES - ALL SOURCES  - " + title1 );
        luzPre.WriteLine();
        luzPre.WriteLine( title4 );
        luzPre.WriteLine( title4a );

        for( i = 0; i < numAreas; i++ )
        {
          luzPre.Write( "{0,4}{1,8:F1}", i + 1, 
            z[i].capacity.ac.totalSFAcres );
          for( j = 1; j < NUM_SF_LAND; j++ )      // 4 sf categories
            luzPre.Write( "{0,8:F1}", z[i].capacity.ac.asf[j] );
          luzPre.Write( "{0,8:F1}", z[i].capacity.ac.totalMFAcres );

          for( j = 1; j < NUM_MF_LAND; j++ )      //  mf categories
            luzPre.Write( "{0,8:F1}", z[i].capacity.ac.amf[j] );          
          luzPre.WriteLine();
          lineCount++;
                
          if( lineCount >= 57 )
          {
            luzPre.WriteLine();
            luzPre.WriteLine();
            lineCount = 0;            // Reset line count
            luzPre.WriteLine( "Table 1-6" );
            luzPre.WriteLine( "DEVELOPABLE HOUSING UNIT ACRES - ALL SOURCES " +
                "- " + title1 );
            luzPre.WriteLine( title4 );
            luzPre.WriteLine( title4a );
          }   // end if
        }     // End for i

        luzPre.Write( "Reg{0,8:F1}", reg.capacity.ac.totalSFAcres );
                
        for( j = 1; j < NUM_SF_LAND; j++ )      // 4 sf categories
          luzPre.Write( "{0,8:F1}", reg.capacity.ac.asf[j] );
                
        luzPre.Write( "{0,8:F1}", reg.capacity.ac.totalMFAcres );
                
        for( j = 1; j < NUM_MF_LAND; j++ )      //  mf categories
          luzPre.Write( "{0,8:F1}", reg.capacity.ac.amf[j] );
        luzPre.WriteLine();
        luzPre.Flush();
            
        // Developable housing unit acres - sitespec
        lineCount = 0;
        luzPre.WriteLine();
        luzPre.WriteLine();
        luzPre.WriteLine( "Table 1-7" );
        luzPre.WriteLine( "DEVELOPABLE HOUSING UNIT ACRES - SITESPEC  - " + title1 );
        luzPre.WriteLine( title4 );
        luzPre.WriteLine( title4a );
        for( i = 0; i < numAreas; i++ )
        {
          luzPre.Write( "{0,4}{1,8:F1}", i + 1, 
            z[i].site.ac.totalSFAcres );
          for( j = 1; j < NUM_SF_LAND; j++ )      // 4 sf categories
            luzPre.Write( "{0,8:F1}", z[i].site.ac.asf[j] );
          luzPre.Write( "{0,8:F1}", z[i].site.ac.totalMFAcres );

          for( j = 1; j < NUM_MF_LAND; j++ )     //  mf categories
            luzPre.Write( "{0,8:F1}", z[i].site.ac.amf[j] );
          luzPre.WriteLine();
          lineCount++;
            
          if(lineCount >= 57 )
          {
            luzPre.WriteLine();
            luzPre.WriteLine();
            lineCount = 0;            // Reset line count
            luzPre.WriteLine( "Table 1-7" );
            luzPre.WriteLine( "DEVELOPABLE HOUSING UNIT ACRES - SITESPEC  - " + title1 );
            luzPre.WriteLine();
            luzPre.WriteLine( title4 );
            luzPre.WriteLine( title4a );
          }   // end if
          luzPre.Flush();
        }     // End for i
                    
        luzPre.Write( "Reg {0,10:F1}", reg.site.ac.totalSFAcres );
        for( j = 1; j < NUM_SF_LAND; j++ )      // 4 sf categories
          luzPre.Write( "{0,8:F1}", reg.site.ac.asf[j] );
        luzPre.Write( "{0,8:F1}", reg.site.ac.totalMFAcres );
        for( j = 1; j < NUM_MF_LAND; j++ )      //  mf categories
          luzPre.Write( "{0,8:F1}", reg.site.ac.amf[j] );
        luzPre.WriteLine();
        luzPre.Flush();
        luzPre.Close();
      }     // End method printLUZPre()

      /*****************************************************************************/

      /* method storeAcres() */
      /// <summary>
      /// Method to store acres from luz query results into z arrays.
      /// </summary>
      /// <param name="sw">Switch</param>
      /// <param name="parm">Index value</param>

      /* Revision History
      * 
      * STR             Date       By    Description
      * --------------------------------------------------------------------------
      *                 05/22/97   tb    Initial coding
      *                 07/17/03   df    C# revision
      * --------------------------------------------------------------------------
      */
      private void storeAcres( int i, int zone, double acres, int sw, int parm )
      {
        int uel, usl, uml;      
        switch( sw )
        {
          case 1:     // Employment acres
            uel = parm;
            z[i].site.ac.ae[uel] += acres;
            z[i].site.ac.totalEmpAcres += acres;
            reg.site.ac.ae[uel] += acres;
            reg.site.ac.totalEmpAcres += acres;
            break;
            
          case 2:     // sf acres
            usl = parm;
            z[i].site.ac.asf[usl] += acres;
            z[i].site.ac.totalSFAcres += acres;
            reg.site.ac.asf[usl] += acres;
            reg.site.ac.totalSFAcres += acres;
            break;

          case 3:     /* mf acres */
            uml = parm;
            z[i].site.ac.amf[uml] += acres;
            z[i].site.ac.totalMFAcres += acres;
            reg.site.ac.amf[uml] += acres;
            reg.site.ac.totalMFAcres += acres;
            break;
        }     // End switch
      }     // End method storeAcres()

      /*****************************************************************************/

    /* method storeCapAcres() */
    /// <summary>
    /// Method to store acres from luz query results into z capacity arrays.
    /// </summary>
    /// <param name="sw">Switch</param>
    /// <param name="parm">Index value</param>
    /// <param name="acres">Acres to be stored</param>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 05/22/97   tb    Initial coding
    *                 07/17/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    private void storeCapAcres( int i, int zone, double acres, int sw,int parm )
    {
      int uel,usl,uml;
      switch( sw )
      {
        case 1:     // Employment acres
          uel = parm;
          z[i].capacity.ac.ae[uel] += acres;
          z[i].capacity.ac.totalEmpAcres += acres;
          reg.capacity.ac.ae[uel] += acres;
          reg.capacity.ac.totalEmpAcres += acres;
          break;
            
        case 2:     // sf acres
          usl = parm;
          z[i].capacity.ac.asf[usl] += acres;
          z[i].capacity.ac.totalSFAcres += acres;
          reg.capacity.ac.asf[usl] += acres;
          reg.capacity.ac.totalSFAcres += acres;
          break;

        case 3:     // mf acres
          uml = parm;
          z[i].capacity.ac.amf[uml] += acres;
          z[i].capacity.ac.totalMFAcres += acres;
          reg.capacity.ac.amf[uml] += acres;
          reg.capacity.ac.totalMFAcres += acres;
          break;
      }     // End switch
    }     // End method storeCapAcres()
    
    /*****************************************************************************/

    /* method storeCapCounts() */
    /// <summary>
    /// Method to store counts from luz query results into z capacity arrays.
    /// </summary>
    /// <param name="sw">Switch</param>
    /// <param name="parm">Index value</param>
    /// <param name="val">Civ, sf, or mf to be stored</param>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 05/22/97   tb    Initial coding
    *                 07/17/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    private void storeCapCounts( int i, int zone, int val, int sw, int parm )
    {
      int uel,usl,uml;
      switch(sw)
      {
        case 1:     // Employment counts
          uel = parm;
          z[i].capacity.e[uel] += val ;
          z[i].capacity.totalEmp += val;
          reg.capacity.e[uel] += val;
          reg.capacity.totalEmp += val;
          break;
            
        case 2:     // SF counts
          usl = parm;
          z[i].capacity.sf[usl] += val;
          z[i].capacity.totalSF += val;
          reg.capacity.sf[usl] += val;
          reg.capacity.totalSF += val;
          break;

        case 3:     // mf counts
          uml = parm;
          z[i].capacity.mf[uml] += val;
          z[i].capacity.totalMF += val;
          reg.capacity.mf[uml] += val;
          reg.capacity.totalMF += val;
          break;
      }   // end switch
    }     // End method storeCapCounts()

    /*****************************************************************************/

    /* method storeZ() */
    /// <summary>
    /// Method to store luz query results into z array.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 05/22/97   tb    Initial coding
    *                 07/17/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    private void storeZ( int i, int zone, int civ, int sf, int mf,int mh, int gqc, int gqm )
    {
      z[i].site.civ += civ;
      z[i].site.sf += sf;
      z[i].site.mf += mf;
      z[i].site.mh += mh;
      z[i].site.gqCiv += gqc;
      z[i].site.gqMil += gqm;
                          
      // Regional total
      reg.site.civ += civ;
      reg.site.sf += sf;
      reg.site.mf += mf;
      reg.site.mh += mh;
      reg.site.gqCiv += gqc;
      reg.site.gqMil += gqm;
    }     // End method storeZ()

    //********************************************************************************

    #endregion Various Helper Methods

    #region Access Weights Code
    /* method processData() */
    /// <summary>
    /// Method to extract data from the SQL table into the mgraWeights array and
    /// compute the weights.
    /// </summary>

    /* Revision History
     * 
     * STR             Date       By    Description
     * --------------------------------------------------------------------------
     *                 07/01/03   df    Initial coding (C# revision of earlier code)
     *                 08/04/09   tb    added to proprocessor code
     *                 07/23/12   tb    changes for SR13 table names
     * --------------------------------------------------------------------------
     */
    private void processAccessWeights()
    {
        System.Data.SqlClient.SqlDataReader rdr;
        weights = new MGRAWeight[NUM_MGRAS];
        double dx, dy;
        //--------------------------------------------------------------------------------------
        writeToStatusBox("Building Access Weights");
        sqlCommand.CommandText = String.Format(appSettings["select01"].Value, TN.mgrabase, scenarioID,bYear); 
      
        /* try to begin reading from mgrabasein the database and storing records */
        try
        {
            sqlConnection.Open();
            rdr = sqlCommand.ExecuteReader();
            int recordsIn = 0;

            /* read in data to weights array from SQL tables */
            while (recordsIn < NUM_MGRAS)
            {
                rdr.Read();
                weights[recordsIn] = new MGRAWeight();
                weights[recordsIn].mgra = rdr.GetInt32(0); 
                weights[recordsIn].luz = rdr.GetInt16(1);
                weights[recordsIn].activity = rdr.GetInt32(2);
                weights[recordsIn].x = rdr.GetInt32(3);
                weights[recordsIn].y = rdr.GetInt32(4);
                recordsIn++;
            }   // end while
            rdr.Close();

        }   // end try
        /* if an SQL error occurrs, it is that the mgra base table can not be found */
        catch (Exception e)
        {
            MessageBox.Show(e.ToString(), "Runtime Exception");
        Close();
        }
        finally
        {
            sqlConnection.Close();
        }

        /* process the records to determine the weighting */
        for (int i = 0; i < NUM_MGRAS; i++)
        {
            for (int j = 0; j < NUM_MGRAS; j++)
            {
                /* distance formula */
                dx = Math.Abs(weights[i].x - weights[j].x);
                dy = Math.Abs(weights[i].y - weights[j].y);

                /* computation in feet- this is 1/4 mile radius */
                if (dx < 26400 && dy < 26400)
                {
                    if (Math.Sqrt(dx * dx + dy * dy) < 26400)
                    {
                         weights[i].weight += weights[j].activity;
                    }  // end if
                }  // end if

            }     /* end for j */

            if (i > 0 && i % 5000 == 0)
            {
                writeToStatusBox("Processed " + i + " mgras for Access Weights");
            }  // end if

            sw.WriteLine(scenarioID + "," + bYear + "," + weights[i].mgra + ", " + weights[i].luz + ", " + weights[i].weight);
            sw.Flush();
        }     /* end for i */
        sw.Close();
    }     /* end method processData() */

    /*****************************************************************************/

    /* method bulkLoadAccessWeightsFromASCII() */
    /// <summary>
    /// Method to bulk load the ASCII file containing the mgras with LUZs and 
    /// their respective weights to the access weights SQL table.  If the table 
    /// already exists, it will be truncated and reloaded.  Otherwise, an all new
    /// table will be created.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 01/06/98   tb    Initial coding
    *                 07/01/03   df    C# revision
    *                 08/04/09   tb    added to proprocessor code
    * --------------------------------------------------------------------------
    */
    private void bulkLoadAccessWeightsFromASCII()
    {

        //------------------------------------------------------------------------
        writeToStatusBox("Bulk loading to " + TN.accessWeights + " table.");

        sqlCommand.CommandText = String.Format(appSettings["deleteFrom"].Value, TN.accessWeights,scenarioID,bYear);

        /* truncate the TN.accessWeights */
        try
        {
            sqlConnection.Open();
            sqlCommand.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            MessageBox.Show("Error - deleting from table" + e.Message);
        }
        finally
        {
            sqlConnection.Close();
        }
      

        /* finally bulk load the ASCII file to the accessWeights table in the database */

        sqlCommand.CommandText = String.Format(appSettings["bulkInsert"].Value, TN.accessWeights, AccessWeightsFileName);
        try
        {
            sqlConnection.Open();
            sqlCommand.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            MessageBox.Show("Error bulk loading your table.  " + e.Message);
        }
        finally
        {
            sqlConnection.Close();
        }

    }  // end bulkLoadAccessWeightsFromAscii

    /*****************************************************************************/

    #endregion


    private void btnExit_Click( object sender, System.EventArgs e )
    {
      Close();
    }

    private void Preprocessor_Closing( object sender, System.ComponentModel.CancelEventArgs e )
    {
      caller.Visible = true;    
    }
    /// <summary>
    /// Data structure to contain the various weights and mgra/distance variables
    /// necessary to compute access weights.
    /// </summary>

    public class MGRAWeight
    {
      public int mgra, luz, weight, activity, x, y;
    }
  }     // End class Preprocessor()
}     // End namespace udm.preprocessor